using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class AdjusterControl : Control
{
    public AdjustFrom AdjustFrom = AdjustFrom.TopLeft;

    public override void Update()
    {
        base.Update();
        Adjust();
    }

    void Adjust()
    {
        Position = Vector2.Zero;

        Position = Viewport.Adjust(Position, AdjustFrom);
    }
}