using System.Numerics;

namespace Thingus;

public class GridFlexControl : Control
{
    public int Spacing;

    public override void Update()
    {
        base.Update();

        Refresh();
    }

    public void Refresh()
    {
        Control last = null;
        Vector2 position = new Vector2(0, 0);
        Children.Select(c => c as Control).ToList().ForEach(c =>
        {
            if (last != null)
            {
                position.X += last.Bounds.X + Spacing;
                if (position.X + c.Bounds.X > Bounds.X)
                {
                    position.X = 0;
                    position.Y += last.Bounds.Y + Spacing;
                }
            }
            c.DrawMode = DrawMode;
            c.DrawOrder = DrawOrder + 1;
            c.SubViewport = SubViewport;
            c.Position = position;
            last = c;
        });
    }

    public override void Draw()
    {
        base.Draw();
        
        DrawNineSlice(
            texture: Texture,
            tileSize: 5,
            position: GlobalPosition - Padding,
            width: (int)(Bounds.X + (Padding.X * 2)),
            height: (int)(Bounds.Y + (Padding.Y * 2)),
            color: Color
        );
    }
}