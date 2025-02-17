using System.Linq;
using System.Numerics;
using Thingus;

public class Cli : Thing
{
    List<string> lines = new List<string>() { };
    List<string> pastCommands = new List<string>() { };
    int pastCommandIndex = -1;
    string current = "";

    bool caret;
    float caretTime = 0;

    float cooldown = 0.05f;

    public Cli()
    {
        DrawMode = DrawMode.Absolute;
        DrawOrder = 9999;
        lines.Add("-----------");
        lines.Add("| THINGUS |");
        lines.Add("-----------");
        lines.Add($"v{CONSTANTS.FULL_VERSION}");
        Help();
    }

    public override void SetActive(bool active)
    {
        base.SetActive(active);

        cooldown = 0.05f;
        current = "";
    }

    public override void Update()
    {
        base.Update();

        cooldown -= Time.Delta;
        if (cooldown > 0) return;

        caretTime -= Time.Delta;
        if (caretTime <= 0) 
        {
            caret = !caret;
            caretTime = 0.5f;
        }
        if (Input.IsPressed(KeyboardKey.Up))
        {
            if (pastCommands.Count > 0)
            {
                if (pastCommandIndex == -1)
                {
                    pastCommandIndex = pastCommands.Count - 1;
                }
                else
                {
                    pastCommandIndex--;
                    if (pastCommandIndex < 0) pastCommandIndex = 0;
                }
                current = pastCommands[pastCommandIndex];
            }
        }
        if (Input.IsPressed(KeyboardKey.Down))
        {
            if (pastCommands.Count > 0)
            {
                if (pastCommandIndex == -1) 
                {

                }
                else
                {
                    pastCommandIndex++;
                    if (pastCommandIndex >= pastCommands.Count)
                    {
                        pastCommandIndex = -1;
                        current = "";
                    }
                    else
                    {
                        current = pastCommands[pastCommandIndex];
                    }
                }
            }
        }

        string lastCurrent = current;
        current = Input.ApplyKeyboardToString(current);
        if (Input.IsRepeating(KeyboardKey.Enter))
        {
            lines.Add(current);
            Command(current);
            pastCommands.Add(current);
            current = "";
        }
        if (lastCurrent != current)
        {
            pastCommandIndex = -1;
        }

        while (lines.Count >= (CONSTANTS.VIRTUAL_HEIGHT - 24) / Library.Font.BaseSize)
        {
            lines.RemoveAt(0);
        }
    }

    public override void Draw()
    {
        base.Draw();

        DrawSprite(
            texture: Library.Textures["Pixel"],
            position: Viewport.Adjust(new Vector2(4)),
            scale: new Vector2(CONSTANTS.VIRTUAL_WIDTH - 8, CONSTANTS.VIRTUAL_HEIGHT - 24),
            color: Theme.Dark.WithAlpha(0.8f),
            origin: new Vector2(0)
        );

        int i = 0;
        foreach (string line in lines)
        {
            DrawText(
                text: $"* {line}",
                position: Viewport.Adjust(new Vector2(6, 4 + (Library.Font.BaseSize * i))),
                color: Theme.Primary
            );
            i++;
        }

        DrawText(
            text: $"$ {current}{(caret ? "|" : "")}",
            position: Viewport.Adjust(new Vector2(6, 4 + (Library.Font.BaseSize * i))),
            color: Theme.Primary
        );
    }

    void Command(string line)
    {
        if (!line.HasValue())
        {
            return;
        }

        List<string> words = line.Split(" ").ToList();
        string command = words[0];
        List<string> arguments = words.Slice(1, words.Count - 1);

        if (command == "load") Load(arguments);
        else if (command == "edit") Edit(arguments);
        else if (command == "newm") NewMap(arguments);
        else if (command == "res") Reset();
        else if (command == "help") Help();
        else if (command == "clear") Clear();
        else if (command == "quit") Quit();
        else NoCommand();
    }

    void Load(List<string> arguments)
    {
        if (arguments.Count <= 0)
        {
            lines.Add("error: provide the name of a map.");
            return;
        }

        string mapName = arguments[0];
        Map map = Library.Maps.Get(mapName);
        if (map == null)
        {
            lines.Add($"error: no such map.");
        }
        else
        {
            DeveloperStorage.Map = map?.Name;
            DeveloperStorage.Room = map?.Name;
            DeveloperStorage.Save();
            Game.Root.Load(map);
            lines.Add($"loaded {mapName}.map");
        }
    }

    void Edit(List<string> arguments)
    {
        if (arguments.Count <= 0)
        {
            lines.Add("error: provide the name of a map.");
            return;
        }

        string mapName = arguments[0];
        Map map = Library.Maps.Get(mapName);
        if (map == null)
        {
            lines.Add($"error: no such map.");
        }
        else
        {
            DeveloperStorage.Map = map?.Name;
            DeveloperStorage.Room = map?.Name;
            DeveloperStorage.Save();
            Game.Root.Load(map);
            Game.Root.DeveloperTools.ToggleEditor(true);
            lines.Add($"editing {mapName}.map");
        }
    }

    void NewMap(List<string> arguments)
    {
        if (arguments.Count <= 0)
        {
            lines.Add("error: provide the name of a map.");
            return;
        }

        string mapName = arguments[0];
        Map map = new Map(mapName);
        map.Path = $"Content/Maps/{mapName}";
        MapCell root = new MapCell("Thing");
        root.Map = map;
        map.Cells.Add(root);
        map.Save();
        Library.Maps[mapName] = map;
        DeveloperStorage.Map = map?.Name;
        DeveloperStorage.Save();
        Game.Root.Load(map);
        Game.Root.DeveloperTools.ToggleEditor(false);
        lines.Add($"created {mapName}.map");
    }

    public void Toggle(bool? show = null)
    {
        bool v = show ?? !Active;
        SetActive(v);
        SetVisible(v);
    }

    void Reset()
    {
        Game.Root.Load(CONSTANTS.START_MAP);
        Toggle(false);
    }

    void Help()
    {
        lines.Add("valid commands: load, newm, help, clear.");
        lines.Add("");
        lines.Add("`: Toggle CLI");
        lines.Add("1: Toggle Editor");
        lines.Add("2: Toggle Debug Info");
        lines.Add("L: Refresh Assets");
        lines.Add("");
        lines.Add("F1: Toggle Frame Advance");
        lines.Add("F2: Advance Frame");
        lines.Add("");
        lines.Add("In Editor:");
        lines.Add("WASD: Move Camera");
        lines.Add("B: Brush, U: Rectangle, G: Fill, Q: Wand, R: Room, E: Edit");
        lines.Add("");
    }

    void Clear()
    {
        lines.Clear();
    }

    void Quit()
    {
        Game.Quit();
    }

    void NoCommand()
    {
        lines.Add("error: no such command.");
    }
}