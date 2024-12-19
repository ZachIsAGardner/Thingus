using System.Numerics;
using Raylib_cs;

namespace Thingus;

public enum OutlineStyle
{
    Full,
    Drop
}

public enum TextJustification
{
    TopLeft,
    Center
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

    public static void DrawSprite(Texture2D? texture, Vector2? position = null, int tileNumber = 0, int tileSize = 0, float rotation = 0, Color? color = null, Vector2? scale = null, DrawMode drawMode = DrawMode.Relative, Vector2? origin = null, bool flipHorizontally = false, bool flipVertically = false, Rectangle? source = null, Rectangle? destination = null)
    {
        if (texture == null) return;

        if (scale == null) scale = new Vector2(1);

        float width = 0;
        float height = 0;
        if (tileSize == 0)
        {
            width = texture.Value.Width;
            height = texture.Value.Height;
        }
        else
        {
            width = height = tileSize;
        }

        // Vector2 p = Adjust(position ?? Vector2.Zero, drawMode, adjustFrom);
        if (position == null) position = Vector2.Zero;
        if (drawMode == DrawMode.Relative)
        {
            position += Viewport.Position;
            if (destination != null)
            {
                destination = new Rectangle(destination.Value.X + Viewport.Position.X, destination.Value.Y + Viewport.Position.Y, destination.Value.Width, destination.Value.Height);
            }
        }

        Vector2 coord = CoordinatesFromNumber(tileNumber, texture.Value, tileSize);
        
        Raylib.DrawTexturePro(
            texture: texture.Value,
            source: source ?? new Rectangle(coord.X, coord.Y, width * (flipHorizontally ? -1 : 1), height * (flipVertically ? -1 : 1)),
            dest: destination ?? new Rectangle(position.Value.X, position.Value.Y, width * scale.Value.X, height * scale.Value.Y),
            origin: origin ?? new Vector2((width * scale.Value.X) / 2, (height * scale.Value.Y) / 2),
            rotation: rotation,
            tint: (color ?? PaletteBasic.White).ToRaylib()
        );
    }

    public static void DrawText(string text, Vector2? position = null, Font? font = null, Color? color = null, DrawMode drawMode = DrawMode.Relative, Color? outlineColor = null, OutlineStyle outlineStyle = OutlineStyle.Full, TextJustification justification = TextJustification.TopLeft)
    {
        if (font == null) font = Library.Font;

        // Vector2 p = Adjust(position ?? Vector2.Zero, drawMode, adjustFrom);
        if (position == null) position = Vector2.Zero;
        if (drawMode == DrawMode.Relative)
        {
            position += Viewport.Position;
        }

        Vector2 offset = Vector2.Zero;

        if (justification == TextJustification.Center)
        {
            offset.X = (text.Width(font) - font.Value.BaseSize) * -0.5f;
            offset.Y = -font.Value.BaseSize / 2f;
        }

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
                    position: position.Value + offset + a,
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
            position: position.Value + offset,
            origin: new Vector2(0),
            rotation: 0,
            fontSize: font.Value.BaseSize,
            spacing: 0,
            tint: (color ?? PaletteBasic.Black).ToRaylib()
        );
    }

    public static void DrawNineSlice(Texture2D? texture, Vector2 position, int tileSize, int width, int height, Vector2? origin = null, DrawMode drawMode = DrawMode.Relative, Color? color = null)
    {
        if (texture == null) return;

        int maxX = width - tileSize;
        int maxY = height - tileSize;
        if (origin == null) origin = new Vector2(0, 0);
        for (int x = 0; x < width; x += tileSize)
        {
            if (x != 0 && x < maxX)
            {
                for (int y = 0; y < height; y += tileSize)
                {
                    if (x != 0 && y != 0 && x < maxX && y < maxY)
                    {
                        // Center
                        DrawSprite(
                            texture: texture, 
                            position: position + origin + new Vector2(x, y),
                            tileNumber: 4,
                            tileSize: tileSize,
                            rotation: 0f,
                            color: color ?? PaletteBasic.White,
                            drawMode: drawMode,
                            origin: new Vector2(0)
                        );
                    }
                }
                
                // Top
                DrawSprite(
                    texture: texture, 
                    position: position + origin + new Vector2(x, 0),
                    tileNumber: 1,
                    tileSize: tileSize,
                    rotation: 0f,
                    color: color ?? PaletteBasic.White,
                    drawMode: drawMode,
                    origin: new Vector2(0)
                );

                // Bottom
                DrawSprite(
                    texture: texture, 
                    position: position + origin + new Vector2(x, maxY),
                    tileNumber: 7,
                    tileSize: tileSize,
                    rotation: 0f,
                    color: color ?? PaletteBasic.White,
                    drawMode: drawMode,
                    origin: new Vector2(0)
                );
            }
        }

        for (int y = 0; y < height; y += tileSize)
        {
            if (y != 0 && y < maxY)
            {
                // Left
                DrawSprite(
                    texture: texture, 
                    position: position + origin + new Vector2(0, y),
                    tileNumber: 3,
                    tileSize: tileSize,
                    rotation: 0f,
                    color: color ?? PaletteBasic.White,
                    drawMode: drawMode,
                    origin: new Vector2(0)
                );

                // Right
                DrawSprite(
                    texture: texture, 
                    position: position + origin + new Vector2(maxX, y),
                    tileNumber: 5,
                    tileSize: tileSize,
                    rotation: 0f,
                    color: color ?? PaletteBasic.White,
                    drawMode: drawMode,
                    origin: new Vector2(0)
                );
            }
        }

        // Top Left
        DrawSprite(
            texture: texture, 
            position: position + origin + new Vector2(0, 0),
            tileNumber: 0,
            tileSize: tileSize,
            rotation: 0f,
            color: color ?? PaletteBasic.White,
            drawMode: drawMode,
            origin: new Vector2(0)
        );

        // Top Right
        DrawSprite(
            texture: texture, 
            position: position + origin + new Vector2(maxX, 0),
            tileNumber: 2,
            tileSize: tileSize,
            rotation: 0f,
            color: color ?? PaletteBasic.White,
            drawMode: drawMode,
            origin: new Vector2(0)
        );

        // Bottom Left
        DrawSprite(
            texture: texture, 
            position: position + origin + new Vector2(0, maxY),
            tileNumber: 6,
            tileSize: tileSize,
            rotation: 0f,
            color: color ?? PaletteBasic.White,
            drawMode: drawMode,
            origin: new Vector2(0)
        );

        // Bottom Right
        DrawSprite(
            texture: texture, 
            position: position + origin + new Vector2(maxX, maxY),
            tileNumber: 8,
            tileSize: tileSize,
            rotation: 0f,
            color: color ?? PaletteBasic.White,
            drawMode: drawMode,
            origin: new Vector2(0)
        );
    }
}
