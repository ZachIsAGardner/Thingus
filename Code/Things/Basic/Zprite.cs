using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class Zprite : Sprite
{
    float lastDrawOrderOffset = 0;

    public Zprite(
        string name, Vector2? position = null, DrawMode drawMode = DrawMode.Relative, float drawOrder = 0, float updateOrder = 0,
        int? tileSize = null, int? tileNumber = null, Color? color = null, Vector2? scale = null, float rotation = 0f, Vector2? origin = null
    )
    : base(
        name, position, drawMode, drawOrder, updateOrder,
        tileSize, tileNumber, color, scale, rotation, origin
    )
    {
    }

    public override void Update()
    {
        base.Update();
        DrawOrderOffset = ((GlobalPosition.Y - (Texture.Height / 2f)) / 64f).RoundTo(1);
        if (lastDrawOrderOffset != DrawOrderOffset) Game.QueueDrawReorder();
        lastDrawOrderOffset = DrawOrderOffset;
    }

    public override void Draw()
    {
        base.Draw();

        // Shapes.DrawText($"{Name}: {DrawOrder + DrawOrderOffset}", position: GlobalPosition, color: PaletteBasic.White, outlineColor: PaletteBasic.Black);
    }
}
