using System.Linq;
using System.Numerics;
using Thingus;

public class Cli : Thing
{
    List<string> lines = new List<string>() { };
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
            current = "";
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
            position: new Vector2(CONSTANTS.VIRTUAL_WIDTH / 2f, CONSTANTS.VIRTUAL_HEIGHT / 2f),
            scale: new Vector2(CONSTANTS.VIRTUAL_WIDTH - 8, CONSTANTS.VIRTUAL_HEIGHT - 8),
            color: new Color(0, 50, 0, 245)
        );

        int i = 0;
        foreach (string line in lines)
        {
            DrawText(
                text: $"* {line}",
                position: new Vector2(6, 4 + (Library.Font.BaseSize * i)),
                color: Utility.HexToColor("#a5ffa5")
            );
            i++;
        }

        DrawText(
            text: $"$ {current}{(caret ? "|" : "")}",
            position: new Vector2(6, 4 + (Library.Font.BaseSize * i)),
            color: Colors.Green
        );
    }

    void Command(string line)
    {
        if (!line.HasValue())
        {
            return;
        }

        List<string> words = line.Split(" ").Select(w => w.ToLower()).ToList();
        string command = words[0];
        List<string> arguments = words.Slice(1, words.Count - 1);

        if (command == "load") Load(arguments);
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
            lines.Add($"loaded {mapName}");
        }
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