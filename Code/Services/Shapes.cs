using System.Numerics;
using Raylib_cs;

namespace Thingus;

public enum OutlineStyle
{
    Full,
    Drop
}

public static class Shapes
{
    public static Vector2 CoordinatesFromNumber(int tileNumber, Texture2D texture, int tileSize = 0)
    {
        if (tileSize == 0) tileSize = CONSTANTS.TILE_SIZE;
        if (tileSize > texture.Width || tileSize > texture.Height || tileSize == 0) return Vector2.Zero;

        var sheetWidth = texture.Width;
        var tilesPerRow = sheetWidth / tileSize;
        int row = 0;

        while (tileNumber >= tilesPerRow)
        {
            tileNumber -= tilesPerRow;
            row++;
        }

        return new Vector2(tileNumber * tileSize, row * tileSize);
    }

    public static Vector2 Adjust(Vector2 position, DrawMode drawMode, AdjustFrom adjustFrom)
    { 
        // Relative
        if (drawMode == DrawMode.Relative)
        {
            position += Viewport.Position;
        }
        // Absolute
        else if (drawMode == DrawMode.Absolute)
        {
            if (adjustFrom != AdjustFrom.None)
            {
                if (Viewport.Margin.X < 0)
                {
                    // Left
                    if ((adjustFrom == AdjustFrom.Auto && position.X < CONSTANTS.VIRTUAL_WIDTH / 2f)
                        || adjustFrom == AdjustFrom.TopLeft
                        || adjustFrom == AdjustFrom.Left
                        || adjustFrom == AdjustFrom.BottomLeft)
                    {
                        position.X -= Viewport.Margin.X;
                    }
                    // Right
                    else if ((adjustFrom == AdjustFrom.Auto && position.X >= CONSTANTS.VIRTUAL_WIDTH / 2f) 
                        || adjustFrom == AdjustFrom.TopRight 
                        || adjustFrom == AdjustFrom.Right 
                        || adjustFrom == AdjustFrom.BottomRight)
                    {
                        position.X += Viewport.Margin.X;
                    } 
                }

                if (Viewport.Margin.Y < 0)
                {
                    // Top
                    if ((adjustFrom == AdjustFrom.Auto && position.Y < CONSTANTS.VIRTUAL_HEIGHT / 2f)
                        || adjustFrom == AdjustFrom.TopLeft
                        || adjustFrom == AdjustFrom.Top
                        || adjustFrom == AdjustFrom.TopRight)
                    {
                        position.Y -= Viewport.Margin.Y;
                    }
                    // Bottom
                    else if ((adjustFrom == AdjustFrom.Auto && position.Y >= CONSTANTS.VIRTUAL_HEIGHT / 2f) 
                        || adjustFrom == AdjustFrom.BottomLeft 
                        || adjustFrom == AdjustFrom.Bottom 
                        || adjustFrom == AdjustFrom.BottomRight)
                    {
                        position.Y += Viewport.Margin.Y;
                    } 
                }
            }
        }

        return position;
    }

    public static void DrawSprite(Texture2D texture, Vector2? position = null, int tileNumber = 0, int tileSize = 0, float rotation = 0, Color? color = null, Vector2? scale = null, DrawMode drawMode = DrawMode.Relative, Vector2? origin = null, bool flipHorizontally = false, bool flipVertically = false, AdjustFrom adjustFrom = AdjustFrom.Auto, Rectangle? source = null, Rectangle? destination = null)
    {
        if (scale == null) scale = new Vector2(1);

        float width = 0;
        float height = 0;
        if (tileSize == 0)
        {
            width = texture.Width;
            height = texture.Height;
        }
        else
        {
            width = height = tileSize;
        }


        Vector2 p = Adjust(position ?? Vector2.Zero, drawMode, adjustFrom);

        Vector2 coord = CoordinatesFromNumber(tileNumber, texture, tileSize);
        
        Raylib.DrawTexturePro(
            texture: texture,
            source: source ?? new Rectangle(coord.X, coord.Y, width * (flipHorizontally ? -1 : 1), height * (flipVertically ? -1 : 1)),
            dest: destination ?? new Rectangle(p.X, p.Y, width * scale.Value.X, height * scale.Value.Y),
            origin: origin ?? new Vector2((width * scale.Value.X) / 2, (height * scale.Value.Y) / 2),
            rotation: rotation,
            tint: (color ?? PaletteBasic.White).ToRaylib()
        );
    }

    public static void DrawText(string text, Vector2? position = null, Font? font = null, Color? color = null, DrawMode drawMode = DrawMode.Relative, Color? outlineColor = null, OutlineStyle outlineStyle = OutlineStyle.Full, AdjustFrom adjustFrom = AdjustFrom.Auto)
    {
        if (font == null) font = Library.Font;

        Vector2 p = Adjust(position ?? Vector2.Zero, drawMode, adjustFrom);

        if (outlineColor != null)
        {
            List<Vector2> angles = new List<Vector2>() { };

            if (outlineStyle == OutlineStyle.Full)
            {
                angles = Utility.Angles;
            }
            else if (outlineStyle == OutlineStyle.Drop)
            {
                angles = new List<Vector2>()
                {
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(0, 1),
                };
            }

            angles.ForEach(a =>
            {
                Raylib.DrawTextPro(
                    font: font.Value.ToRaylib(),
                    text: text,
                    position: p + a,
                    origin: new Vector2(0),
                    rotation: 0,
                    fontSize: font.Value.BaseSize,
                    spacing: 0,
                    tint: outlineColor.Value.ToRaylib()
                );
            });
        }

        Raylib.DrawTextPro(
            font: font.Value.ToRaylib(),
            text: text,
            position: p,
            origin: new Vector2(0),
            rotation: 0,
            fontSize: font.Value.BaseSize,
            spacing: 0,
            tint: (color ?? PaletteBasic.Black).ToRaylib()
        );
    }
}
