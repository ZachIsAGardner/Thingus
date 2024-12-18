using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class DebugPoint : Thing
{
    public DebugPoint() : base() { }
    public DebugPoint(Vector2 position, Color? color = null) : base()
    {
        Position = position;
        AddChild(new Sprite("Pixel", color: color ?? Theme.Primary));
        DrawOrder = 200;
    }

    public override void Start()
    {
        base.Start();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();

        Destroy();
    }
}
