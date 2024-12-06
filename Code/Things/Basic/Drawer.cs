using System.Numerics;

namespace Thingus;

public class Drawer : Thing
{
    public Action<Drawer> Action = null;

    public Drawer() { }
    public Drawer(
        string name = null, Vector2? position = null, DrawMode drawMode = DrawMode.Relative, float drawOrder = 0, float updateOrder = 0,
        Action<Drawer> action = null
    ) : base(name, position, drawMode, drawOrder, updateOrder)
    {
        Action = action;
    }

    public override void Draw()
    {
        base.Draw();

        if (Action != null) Action(this);
    }
}