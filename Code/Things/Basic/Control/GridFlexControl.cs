using System.Numerics;

namespace Thingus;

public class GridFlexControl : Control
{
    public override void Update()
    {
        base.Update();

        Control last = null;
        Vector2 position = new Vector2(0, 0);
        Children.Select(c => c as Control).ToList().ForEach(c =>
        {
            if (last != null)
            {
                position.X += last.Bounds.X;
                if (position.X + c.Bounds.X > Bounds.X)
                {
                    position.X = 0;
                    position.Y += last.Bounds.Y;
                }
            }
            c.DrawMode = DrawMode;
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
            origin: new Vector2(0),
            color: Color
        );
    }
}