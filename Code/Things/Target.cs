using System.Numerics;

namespace Thingus;

public class Target : Sprite
{
     // Create from Map
    public static Target Create(ThingModel model)
        => new Target(
            position: model.Position,
            model.Properties.Get("Tag")
        );

    // Create from code
    public Target(Vector2 position, string tag) : base("Target", position)
    {
        AddTag(tag);
        VisibleOnlyInDebug = true;
    }

    public override void Update()
    {
        base.Update();
    }
}