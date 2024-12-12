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
        // Bounds = new Vector2(Raylib.MeasureText(Text, Font.Value.BaseSize), Font.Value.BaseSize);
    }

    public override void Draw()
    {
        base.Draw();

        DrawNineSlice(
            texture: Library.Textures["WhiteSlice"],
            tileSize: 5,
            position: GlobalPosition - Padding,
            width: (int)(Bounds.X + (Padding.X * 2)),
            height: (int)(Bounds.Y + (Padding.Y * 2)),
            color: ShouldShowHighlight ? HighlightColor : PaletteBasic.Blank
        );

        if (Font != null && Text != null)
        {
            int x = 0;
            int y = 0;
            foreach (char character in Text)
            {
                if (x == 0 && character == ' ') continue;

                DrawText(
                    text: character,
                    color: ShouldShowHighlight ? TextHighlightColor : TextColor,
                    font: Font,
                    position: GlobalPosition + TextPadding + new Vector2(x, y)
                );
                x += 6;
                if (x > Bounds.X && character == ' ')
                {
                    x = 0;
                    y += Font.Value.BaseSize;
                }
            }
        }
    }
}