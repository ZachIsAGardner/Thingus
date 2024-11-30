using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class TextControl : Control
{
    public string Text;
    public Font? Font;

    public override void Update()
    {
        base.Update();

        Refresh();
    }

    public void Refresh()
    {
        if (Font == null) Font = Library.Font;
        Bounds = new Vector2(Raylib.MeasureText(Text, Font.Value.BaseSize), Font.Value.BaseSize);
    }

    public override void Draw()
    {
        base.Draw();

        DrawText(
            text: Text,
            color: Pressed != null && IsHovered ? HighlightColor : Color,
            font: Font
        );
    }
}