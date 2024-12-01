using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class TextureControl : Control
{
    public override void Draw()
    {
        base.Draw();

        Rectangle rectangle = new Rectangle(GlobalPosition.X, GlobalPosition.Y, Bounds.X, Bounds.Y);

        DrawSprite(
            texture: Texture,
            origin: new Vector2(0),
            color: Pressed != null && (IsHovered || IsHeld) ? HighlightColor : Color,

            destination: rectangle,
            tileNumber: TileNumber,
            tileSize: TileSize
        );
    }
}