using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class SubViewport : Control
{
    public RenderTexture2D? RenderTexture;
    public Vector2 Scroll;
    public Vector2 Max => new Vector2(
        RenderTexture.Value.Texture.Width - Bounds.X,
        RenderTexture.Value.Texture.Height - Bounds.Y
    );

    public SubViewport() { }
    public SubViewport(Vector2 bounds, Vector2 area) 
    {
        Bounds = bounds;
        RenderTexture = Raylib.LoadRenderTexture((int)area.X, (int)area.Y);
    }

    public override void Update()
    {
        base.Update();

        // Debug
        // if (Input.IsHeld(KeyboardKey.Left)) Position.X -= 50 * Time.Delta;
        // if (Input.IsHeld(KeyboardKey.Right)) Position.X += 50 * Time.Delta;
        // if (Input.IsHeld(KeyboardKey.Up)) Position.Y -= 50 * Time.Delta;
        // if (Input.IsHeld(KeyboardKey.Down)) Position.Y += 50 * Time.Delta;
        // if (Input.IsHeld(KeyboardKey.H)) Scroll.X -= 50 * Time.Delta;
        // if (Input.IsHeld(KeyboardKey.J)) Scroll.X += 50 * Time.Delta;
        // if (Input.IsHeld(KeyboardKey.N)) Scroll.Y -= 50 * Time.Delta;
        // if (Input.IsHeld(KeyboardKey.M)) Scroll.Y += 50 * Time.Delta;
        
        if (Scroll.Y < 0) Scroll.Y = 0;
        if (Scroll.Y > Max.Y) Scroll.Y = Max.Y;
        if (Scroll.X < 0) Scroll.X = 0;
        if (Scroll.X > Max.X) Scroll.X = Max.X;
    }

    public override void DrawToTexture()
    {
        base.DrawToTexture();

        if (RenderTexture == null) return;

        Raylib.BeginTextureMode(RenderTexture.Value);
        Raylib.ClearBackground(PaletteBasic.Blank.ToRaylib());
        Game.DrawThings.Where(d => d.DrawMode == DrawMode.Texture && d.SubViewport == this).ToList().ForEach(d =>
        {
            d.Draw();
        });
        Raylib.EndTextureMode();
    }

    public override void Draw()
    {
        base.Draw();

        if (RenderTexture == null) return;

        DrawSprite(
            texture: RenderTexture.Value.Texture, 
            position: Position,
            source: new Rectangle(Scroll.X, -Bounds.Y - Scroll.Y, Bounds.X, -Bounds.Y),
            destination: new Rectangle(GlobalPosition.X, GlobalPosition.Y, Bounds.X, Bounds.Y),
            origin: new Vector2(0)
        );
    }
}