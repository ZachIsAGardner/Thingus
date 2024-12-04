using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class TextureControl : Control
{
    public override void Draw()
    {
        base.Draw();

        if (Texture == null) return;

        Vector2 coord = Shapes.CoordinatesFromNumber(TileNumber, Texture.Value, TileSize);

        Rectangle source = new Rectangle(coord.X, coord.Y, Bounds.X, Bounds.Y);
        Rectangle destination = new Rectangle(GlobalPosition.X, GlobalPosition.Y, Bounds.X, Bounds.Y);

        DrawSprite(
            texture: Texture,
            origin: new Vector2(0),
            color: Pressed != null && (IsHovered || IsHeld || IsHighlighted) ? HighlightColor : Color,

            source: source,
            destination: destination
        );
    }
}