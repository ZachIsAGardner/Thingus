using System.Numerics;

namespace Thingus;

public class HorizontalFlexControl : Control
{
    public override void Update()
    {
        base.Update();

        Control last = null;
        Vector2 position = new Vector2(0, 0);
        Children.Select(c => c as Control).ToList().ForEach(c =>
        {
            if (last != null) position.X += last.Bounds.X;
            c.DrawMode = DrawMode;
            c.AdjustFrom = AdjustFrom;
            c.SubViewport = SubViewport;
            c.Position = position;
            last = c;
        });
    }

    public override void Draw()
    {
        base.Draw();

        DrawSprite(
            texture: Library.Textures["Pixel"],
            position: GlobalPosition,
            scale: Bounds,
            origin: new Vector2(0)
        );
    }
}