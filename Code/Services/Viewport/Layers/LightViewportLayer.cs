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
    int size = 16;

    public override void Update()
    {
        base.Update();

        tileSize = 256;
        size = 16;

        lights = Game.GetThings<Light>().Where(l => l.GlobalVisible).ToList();
        // darkness = darkness.MoveOverTime(255 - lights.Count * 5, 0.01f);
    }

    public override void RefreshProjection()
    {
        base.RefreshProjection();

        lightTextureSheet = Raylib.LoadRenderTexture(tileSize * size, tileSize * size);
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
                inner: l.Color.WithAlpha(l.Strength).ToRaylib(),
                outer: PaletteBasic.Blank.ToRaylib()
            );
            x++;
            if (x >= size)
            {
                y++;
                x = 0;
            }
        });
        Raylib.EndTextureMode();

        // Draw to Texture
        int lightness = 255 - (int)(Darkness * 255);
        Raylib.BeginTextureMode(Texture);
        Raylib.ClearBackground(new Raylib_cs.Color(lightness, lightness, lightness, 255));
        // Raylib.ClearBackground(Raylib_cs.Color.Blue);
        Raylib.BeginMode2D(Camera);

        x = 0;
        y = 0;
        lights.ForEach(l =>
        {
            Vector2 position = l.GlobalPosition;
            if (l.DrawMode == DrawMode.Relative) position -= Viewport.CameraPosition;
            if (l.DrawMode == DrawMode.Absolute) position -= (Viewport.Margin * Viewport.VirtualRatio.Value);
            // - new Vector2(l.GlobalOffset.X, l.GlobalOffset.Y)
            Shapes.DrawSprite(
                texture: lightTextureSheet.Texture,
                position: position,
                scale: new Vector2(1),
                tileSize: tileSize,
                source: new Rectangle((x * tileSize), -(tileSize + (y * tileSize)), tileSize, tileSize),
                color: new Color(255, 255, 255, 255)
            );
            x++;
            if (x >= size)
            {
                y++;
                x = 0;
            }
        });
        Raylib.EndMode2D();
        Raylib.EndTextureMode();
    }

    public override void DrawToWindow()
    {
        // Debug
        // Raylib.DrawTexturePro(
        //     texture: lightTextureSheet.Texture,
        //     source: new Rectangle(0, 0, lightTextureSheet.Texture.Width, -lightTextureSheet.Texture.Height),
        //     dest: new Rectangle(
        //         Viewport.Rectangle.X + Viewport.Offset.X,
        //         Viewport.Rectangle.Y + Viewport.Offset.Y,
        //         lightTextureSheet.Texture.Width,
        //         lightTextureSheet.Texture.Height
        //     ),
        //     origin: new Vector2(0),
        //     rotation: Viewport.Rotation,
        //     tint: PaletteBasic.White.ToRaylib()
        // );

        Raylib.BeginBlendMode(BlendMode.Multiplied);
        Raylib.DrawTexturePro(
            texture: Texture.Texture,
            source: Rectangle,
            // source: new Rectangle(0, 0, Texture.Texture.Width, -Texture.Texture.Height),
            dest: new Rectangle(
                (Viewport.Rectangle.X + Viewport.Rectangle.Width / 2f) + Viewport.Offset.X,
                (Viewport.Rectangle.Y + Viewport.Rectangle.Height / 2f) + Viewport.Offset.Y,
                Viewport.Rectangle.Width,
                Viewport.Rectangle.Height
            ),
            origin: new Vector2(Viewport.Rectangle.Width / 2f, Viewport.Rectangle.Height / 2f),
            rotation: Viewport.Rotation,
            tint: PaletteBasic.White.ToRaylib()
        );
        Raylib.EndBlendMode();
    }
}