using System.Numerics;

namespace Thingus;

public class VerticalFlexControl : Control
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
            if (last != null) position.Y += last.Bounds.Y + (last.Padding * 2) + Spacing;
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
            position: GlobalPosition - new Vector2(Padding),
            width: (int)Bounds.X + (Padding * 2),
            height: (int)Bounds.Y + (Padding * 2),
            color: Color
        );
    }
}