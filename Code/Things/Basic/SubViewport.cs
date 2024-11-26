using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class SubViewport : Thing
{
    public Vector2 Bounds;
    public RenderTexture2D? Texture;
    public Vector2 Scroll;

    public override void Start()
    {
        base.Start();

        Bounds = new Vector2(100, 100);
        Texture = Raylib.LoadRenderTexture(100, 200);
    }

    public override void Update()
    {
        base.Update();

        if (Input.IsHeld(KeyboardKey.Left)) Position.X -= 50 * Time.Delta;
        if (Input.IsHeld(KeyboardKey.Right)) Position.X += 50 * Time.Delta;
        if (Input.IsHeld(KeyboardKey.Up)) Position.Y -= 50 * Time.Delta;
        if (Input.IsHeld(KeyboardKey.Down)) Position.Y += 50 * Time.Delta;
        if (Input.IsHeld(KeyboardKey.H)) Scroll.X -= 50 * Time.Delta;
        if (Input.IsHeld(KeyboardKey.J)) Scroll.X += 50 * Time.Delta;
        if (Input.IsHeld(KeyboardKey.N)) Scroll.Y -= 50 * Time.Delta;
        if (Input.IsHeld(KeyboardKey.M)) Scroll.Y += 50 * Time.Delta;
    }

    public override void DrawToTexture()
    {
        base.DrawToTexture();

        if (Texture == null) return;

        Raylib.BeginTextureMode(Texture.Value);
        Raylib.ClearBackground(PaletteBasic.Red.ToRaylib());
        DrawMode = DrawMode.Texture;
        for (int x = 0; x < Texture.Value.Texture.Width; x += 16)
        {
            for (int y = 0; y < Texture.Value.Texture.Height; y += 16)
            {
                
            }
        }
        DrawMode = DrawMode.Absolute;
        Raylib.EndTextureMode();
    }

    public override void Draw()
    {
        base.Draw();

        if (Texture == null) return;

        Vector2 position = Shapes.Adjust(Position, DrawMode, AdjustFrom.TopLeft);

        DrawSprite(
            texture: Texture.Value.Texture, 
            position: Position,
            source: new Rectangle(Scroll.X, Bounds.Y - Scroll.Y, Bounds.X, -Bounds.Y),
            destination: new Rectangle(position.X, position.Y, Bounds.X, Bounds.Y),
            adjustFrom: AdjustFrom.TopLeft,
            origin: Bounds / 2f
        );
    }
}