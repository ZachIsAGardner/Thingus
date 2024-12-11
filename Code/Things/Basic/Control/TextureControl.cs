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

        Rectangle source = new Rectangle(coord.X, coord.Y, (TileSize > 0 ? TileSize : Bounds.X), (TileSize > 0 ? TileSize : Bounds.Y));
        Rectangle destination = new Rectangle(GlobalPosition.X, GlobalPosition.Y, Bounds.X, Bounds.Y);

        DrawSprite(
            texture: Texture,
            origin: new Vector2(0),
            color: ShouldShowHighlight ? HighlightColor : Color,

            source: source,
            destination: destination
        );
    }
}