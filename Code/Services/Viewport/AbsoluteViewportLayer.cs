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

        Raylib.ClearBackground(Colors.Blank.ToRaylib());

        Raylib.BeginMode2D(Camera);
        Game.DrawThings.Where(x => x.DrawMode == DrawMode.Absolute).ToList().ForEach(x => x.Draw());
        if (Game.FrameAdvance) Shapes.DrawText("FRAME ADVANCE", position: new Vector2(CONSTANTS.VIRTUAL_WIDTH / 2f, 16), color: Colors.Red, outlineColor: Colors.Black);
        Raylib.EndMode2D();

        Raylib.EndTextureMode();
    }
}