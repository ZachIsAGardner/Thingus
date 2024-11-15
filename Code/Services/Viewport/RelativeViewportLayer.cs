using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class RelativeViewportLayer : ViewportLayer
{
    public RelativeViewportLayer() { }
    public RelativeViewportLayer(int index) : base(index) { }

    public override void Update()
    {
        base.Update();
        Camera.Target = Viewport.CameraPosition;
        Camera.Zoom = Viewport.VirtualRatio.Value * Viewport.Zoom;
    }

    public override void DrawToTexture()
    {
        base.DrawToTexture();

        Raylib.BeginTextureMode(Texture);

        Raylib.ClearBackground(Colors.Black.ToRaylib());

        Raylib.BeginMode2D(Camera);
        // Children.ForEach(x => x.Draw());
        Game.DrawThings.Where(x => x.DrawMode == DrawMode.Relative).ToList().ForEach(x => x.Draw());
        Raylib.EndMode2D();

        Raylib.EndTextureMode();
    }
}