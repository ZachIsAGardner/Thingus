using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class AbsoluteViewportLayer : ViewportLayer
{
    public AbsoluteViewportLayer() { }
    public AbsoluteViewportLayer(int index) : base(index) { }

    public override void DrawToTexture()
    {
        base.DrawToTexture();

        Raylib.BeginTextureMode(Texture);

        Raylib.ClearBackground(PaletteBasic.Blank.ToRaylib());

        Raylib.BeginMode2D(Camera);
        Game.DrawThings.Where(x => x.DrawMode == DrawMode.Absolute).ToList().ForEach(x => x.Draw());
        Raylib.EndMode2D();

        Raylib.EndTextureMode();
    }
}