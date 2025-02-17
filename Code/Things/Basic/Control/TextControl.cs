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

    public override void Refresh()
    {
        base.Refresh();
        
        if (Font == null) Font = Library.Font;
        // Bounds = new Vector2(Raylib.MeasureText(Text, Font.Value.BaseSize), Font.Value.BaseSize);
    }

    public override void Draw()
    {
        base.Draw();

        DrawNineSlice(
            texture: Texture,
            tileSize: 5,
            position: GlobalPosition - Padding,
            width: (int)(Bounds.X + (Padding.X * 2)),
            height: (int)(Bounds.Y + (Padding.Y * 2)),
            color: ShouldShowHighlight ? HighlightColor : Color
        );

        char? lastChar = null;
        if (Font != null && Text != null)
        {
            int x = 0;
            int y = 0;
            foreach (char character in Text)
            {
                if (lastChar == '\\' && character == 'n')
                {
                    x = 0;
                    y += Font.Value.BaseSize;
                    lastChar = character;
                    continue;
                }
                if ((x == 0 && character == ' ') || character == '\\')
                {
                    lastChar = character;
                    continue;
                }


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
                lastChar = character;
            }
        }
    }
}