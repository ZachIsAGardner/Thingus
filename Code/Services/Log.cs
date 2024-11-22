using System.Numerics;
using Raylib_cs;

namespace Thingus;

public static class Log
{
    class LogMessage
    {
        public int Index;
        static int index;
        public string Message;

        public LogMessage() { }
        public LogMessage(string message)
        {
            Message = message;
            Index = index;
            index++;
        }
    }

    static Font font = Library.FontSmall;

    static int width = 180;
    static int height = CONSTANTS.VIRTUAL_HEIGHT - 4;

    static List<LogMessage> logs = new List<LogMessage>() { };

    public static void Clear()
    {
        logs.Clear();
    }

    public static void Write(object message)
    {
        logs.Add(new LogMessage(message?.ToString() ?? "null"));
        while (logs.Count >= (height / (font.BaseSize + 1f))) logs.RemoveAt(0);
    }

    public static void Update()
    {

    }

    public static void Draw()
    {
        int i = 0;
        Shapes.DrawSprite(
            texture: Library.Textures["Pixel"],
            position: new Vector2(CONSTANTS.VIRTUAL_WIDTH - (width / 2f) - 4, CONSTANTS.VIRTUAL_HEIGHT / 2f) + Viewport.AdjustFromTopRight,
            scale: new Vector2(width, height),
            color: new Color(0, 50, 0, 245)
        );
        Shapes.DrawText(
            font: font,
            text: "LOG",
            position: new Vector2(CONSTANTS.VIRTUAL_WIDTH - (width), 4) + Viewport.AdjustFromTopRight,
            color: Colors.Green
        );
        logs.Reversed().ForEach(l =>
        {
            Vector2 p = new Vector2(CONSTANTS.VIRTUAL_WIDTH - (width), 4 + (font.BaseSize + 1) * (i + 1)) + Viewport.AdjustFromTopRight;
            Shapes.DrawText(font: font, text: l.Index + ": " + l.Message, position: p, color: l.Index % 2 == 0 ? Colors.White : Utility.HexToColor("#a5ffa5"));
            i++;
        });
    }
}
