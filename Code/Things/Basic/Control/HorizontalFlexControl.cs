using System.Numerics;

namespace Thingus;

public class HorizontalFlexControl : Control
{
    public FlexJustify Justify = FlexJustify.Start;

    public override void Update()
    {
        base.Update();

        Refresh();
    }

    public void Refresh()
    {
        if (Justify == FlexJustify.Start)
        {
            Control last = null;
            Vector2 position = new Vector2(0, 0);
            Children.Select(c => c as Control).ToList().ForEach(c =>
            {
                if (last != null) position.X += last.Bounds.X + (last.Padding.X * 2) + Spacing;
                c.DrawMode = DrawMode;
                c.DrawOrder = DrawOrder + 1;
                c.SubViewport = SubViewport;
                c.Position = position + Offset;
                last = c;
            });
        }
        // TODO
        else if (Justify == FlexJustify.Between)
        {
            List<Control> children = Children.Select(c => c as Control).ToList();
            float spacing = Bounds.X / children.Count();
            float i = 0;
            children.ForEach(c =>
            {
                c.DrawMode = DrawMode;
                c.DrawOrder = DrawOrder + 1;
                c.SubViewport = SubViewport;
                c.Position = new Vector2(i * spacing, 0) + Offset;
                if (i == children.Count - 1) c.Position.X = Bounds.X - c.Bounds.X;
                i++;
            });
        }
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
            color: Pressed != null && IsHovered ? HighlightColor : Color
        );
    }
}