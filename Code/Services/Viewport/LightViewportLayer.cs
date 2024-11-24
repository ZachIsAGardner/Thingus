using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class LightViewportLayer : ViewportLayer
{
    public LightViewportLayer() { }
    public LightViewportLayer(int index) : base(index) { }

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

        lightTextureSheet = Raylib.LoadRenderTexture(tileSize * 32, tileSize);
    }

    public override void DrawToTexture()
    {
        base.DrawToTexture();

        Raylib.BeginTextureMode(lightTextureSheet);
        Raylib.ClearBackground(PaletteBasic.Blank.ToRaylib());
        int i = 0;
        lights.ForEach(l =>
        {
            float radius = l.Radius + l.Flicker;
            if (radius > tileSize)
            {
                // ...
            }
            Raylib.DrawCircleGradient(
                centerX: (tileSize / 2) + (tileSize * i),
                centerY: tileSize / 2,
                radius: radius,
                color1: l.Color.ToRaylib(),
                color2: PaletteBasic.Blank.ToRaylib()
            );
            i++;
        });
        Raylib.EndTextureMode();

        int lightness = 255 - (int)(Darkness);

        Raylib.BeginTextureMode(Texture);
        // Raylib.ClearBackground(new Color(
        //     255 - (int)Darkness,
        //     255 - (int)Darkness,
        //     255 - (int)Darkness,
        //     255
        // ));
        Raylib.ClearBackground(new Raylib_cs.Color(lightness, lightness, lightness, 255));
        Raylib.BeginMode2D(Camera);

        // Shapes.DrawSprite(Library.Textures["Vignette"], origin: new Vector2(0), position: new Vector2(-32), scale: new Vector2(1), color: new Color(255, 255, 255, lightness));
        i = 0;
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