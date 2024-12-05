using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class LightViewportLayer : ViewportLayer
{
    public LightViewportLayer(int order) : base(order) { }

    List<Light> lights = new List<Light>() { };
    RenderTexture2D lightTextureSheet;
    public float Darkness = 0;
    int tileSize = 256;

    public override void Update()
    {
        base.Update();

        lights = Game.GetThings<Light>().Where(l => l.GlobalVisible).ToList();
        // darkness = darkness.MoveOverTime(255 - lights.Count * 5, 0.01f);
    }

    public override void RefreshProjection()
    {
        base.RefreshProjection();

        lightTextureSheet = Raylib.LoadRenderTexture(tileSize * 64, tileSize);
    }

    public override void DrawToTexture()
    {
        base.DrawToTexture();

        // Make light texture sheet
        Raylib.BeginTextureMode(lightTextureSheet);
        Raylib.ClearBackground(PaletteBasic.Blank.ToRaylib());
        int x = 0;
        int y = 0;
        lights.ForEach(l =>
        {
            float radius = l.Radius + l.Flicker;
            if (radius > tileSize)
            {
                radius = tileSize;
            }
            Raylib.DrawCircleGradient(
                centerX: (tileSize / 2) + (tileSize * x),
                centerY: (tileSize / 2) + (tileSize * y),
                radius: radius,
                color1: l.Color.ToRaylib(),
                color2: PaletteBasic.Blank.ToRaylib()
            );
            // if (x >= 16)
            // {
            //     y++;
            //     x = 0;
            // }
            x++;
        });
        Raylib.EndTextureMode();

        // Draw to Texture
        int lightness = 255 - (int)(Darkness * 255);
        Raylib.BeginTextureMode(Texture);
        Raylib.ClearBackground(new Raylib_cs.Color(lightness, lightness, lightness, 255));
        Raylib.BeginMode2D(Camera);

        int i = 0;
        lights.ForEach(l =>
        {
            Vector2 position = new Vector2(l.GlobalPosition.X, l.GlobalPosition.Y)
                - new Vector2(Viewport.CameraPosition.X, Viewport.CameraPosition.Y);
            Shapes.DrawSprite(
                texture: lightTextureSheet.Texture,
                position: position,
                scale: new Vector2(1),
                tileSize: tileSize,
                tileNumber: i,
                color: new Color(255, 255, 255, 255)
            );
            i++;
        });
        Raylib.EndMode2D();
        Raylib.EndTextureMode();
    }

    public override void DrawToWindow()
    {
        Raylib.BeginBlendMode(BlendMode.Multiplied);
        Raylib.DrawTexturePro(
            texture: Texture.Texture,
            source: Rectangle,
            dest: Viewport.Rectangle,
            origin: new Vector2(0f, 0f),
            rotation: Viewport.Rotation,
            tint: PaletteBasic.White.ToRaylib()
        );
        Raylib.EndBlendMode();
    }
}