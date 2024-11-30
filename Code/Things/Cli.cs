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

    public override void Init()
    {
        base.Init();

        DrawMode = DrawMode.Absolute;
        DrawOrder = 9999;
        lines.Add("-----------");
        lines.Add("| THINGUS |");
        lines.Add("-----------");
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
        current += Input.GetKeyboardString();
        if (Input.IsRepeating(KeyboardKey.Backspace) && current.Count() > 0)
        {
            if (Input.AltIsHeld || Input.CtrlIsHeld) current = "";
            else current = current.Remove(current.Count() - 1);
        }
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

        while (lines.Count >= (CONSTANTS.VIRTUAL_HEIGHT - 8) / Library.Font.BaseSize)
        {
            lines.RemoveAt(0);
        }
    }

    public override void Draw()
    {
        base.Draw();

        DrawSprite(
            texture: Library.Textures["Pixel"],
            position: Viewport.Adjust(new Vector2(CONSTANTS.VIRTUAL_WIDTH / 2f, CONSTANTS.VIRTUAL_HEIGHT / 2f)),
            scale: new Vector2(CONSTANTS.VIRTUAL_WIDTH - 8, CONSTANTS.VIRTUAL_HEIGHT - 8),
            color: new Color(0, 50, 0, 245)
        );

        int i = 0;
        foreach (string line in lines)
        {
            DrawText(
                text: $"* {line}",
                position: Viewport.Adjust(new Vector2(6, 4 + (Library.Font.BaseSize * i))),
                color: PaletteAapSplendor128.NightlyAurora
            );
            i++;
        }

        DrawText(
            text: $"$ {current}{(caret ? "|" : "")}",
            position: Viewport.Adjust(new Vector2(6, 4 + (Library.Font.BaseSize * i))),
            color: PaletteBasic.Green
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
        else if (command == "newm") NewMap(arguments);
        else if (command == "help") Help();
        else if (command == "clear") Clear();
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
            Game.Root.Load(map);
            lines.Add($"loaded {mapName}.map");
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
        Game.Root.Load(map);
        lines.Add($"created {mapName}.map");
    }

    void Help()
    {
        lines.Add("valid commands: load, newm, help, clear.");
        lines.Add("");
        lines.Add("`: Toggle CLI");
        lines.Add("1: Toggle Editor");
        lines.Add("2: Toggle Debug Info");
        lines.Add("");
        lines.Add("F1: Toggle Frame Advance");
        lines.Add("F2: Advance Frame");
        lines.Add("");
    }

    void Clear()
    {
        lines.Clear();
    }

    void NoCommand()
    {
        lines.Add("error: no such command.");
    }
}