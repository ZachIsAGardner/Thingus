using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class AbsoluteViewportLayer : ViewportLayer
{
    public AbsoluteViewportLayer(int order) : base(order) { }

    public override void DrawToTexture()
    {
        base.DrawToTexture();

        Raylib.BeginTextureMode(Texture);

        Raylib.ClearBackground(PaletteBasic.Blank.ToRaylib());

        Raylib.BeginMode2D(Camera);
        Game.DrawThings.Where(x => x.DrawMode == DrawMode.Absolute).ToList().ForEach(x => x.Draw());
        Game.DrawThings.Where(x => x.DrawMode == DrawMode.Absolute).ToList().ForEach(x => x.LateDraw());
        Raylib.EndMode2D();

        Raylib.EndTextureMode();
    }
}