using System.Numerics;

namespace Thingus;

public class VerticalFlexControl : Control
{    
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
            if (last != null) position.Y += last.Bounds.Y + (last.Padding.Y * 2) + Spacing;
            c.DrawMode = DrawMode;
            if (c.DrawOrder < DrawOrder + 1) c.DrawOrder = DrawOrder + 1;
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