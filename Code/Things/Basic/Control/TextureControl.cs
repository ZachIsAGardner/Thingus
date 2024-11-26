using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class TextureControl : Control
{
    public Texture2D Texture;

    public override void Draw()
    {
        base.Draw();

        Vector2 position = Shapes.Adjust(GlobalPosition, DrawMode, AdjustFrom);

        Shapes.DrawSprite(
            texture: Texture,
            tileNumber: TileNumber,
            tileSize: TileSize,
            destination: new Rectangle(position.X, position.Y, Bounds.X, Bounds.Y),
            origin: new Vector2(0),
            color: Color,
            drawMode: DrawMode
        );
    }
}