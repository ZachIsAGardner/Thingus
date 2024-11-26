using System.Numerics;

namespace Thingus;

public class Control : Thing
{
    public Vector2 Bounds;
    public Color Color = PaletteBasic.White;
    public AdjustFrom AdjustFrom = AdjustFrom.None;
    public int TileNumber = 0;
    public int TileSize = 0;

    public bool Hovered { get; protected set; }
    public bool Held { get; protected set; }

    public override void Update()
    {
        base.Update();

        if (Color.A <= 0)
        {
            Hovered = false;
            return;
        }

        Vector2 mouse = Vector2.Zero;
        if (DrawMode == DrawMode.Relative) mouse = Input.MousePositionRelative();
        if (DrawMode == DrawMode.Absolute) mouse = Input.MousePositionAbsolute();
        if (DrawMode == DrawMode.Texture && SubViewport != null)
        {
            if (!SubViewport.Hovered)
            {
                Hovered = false;
                return;
            }
            if (SubViewport.DrawMode == DrawMode.Relative) mouse = Input.MousePositionRelative() + SubViewport.Scroll - SubViewport.Position;
            if (SubViewport.DrawMode == DrawMode.Absolute) mouse = Input.MousePositionAbsolute() + SubViewport.Scroll - SubViewport.Position;
        }

            
        Hovered = Utility.CheckRectangleOverlap(
            mouse,
            mouse,
            GlobalPosition,
            GlobalPosition + Bounds
        );
    }
}