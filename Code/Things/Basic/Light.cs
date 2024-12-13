using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class Light : Thing
{
    public float Radius = 80;
    public float Flicker = 0;
    public Color Color = PaletteBasic.White;
    public float Strength = 1f;
    float flickerOffset = 0;

    float decay = 2f;
    bool decaying = false;
    bool remove = false;


    public Light(int? radius = null, Color? color = null, Vector2? position = null, float strength = 1f) : base(position: position)
    {
        if (radius != null) Radius = radius.Value;
        if (color != null) Color = color.Value;
    }

    public override void Start()
    {
        base.Start();
        flickerOffset = Chance.Range(0f, 2f);
    }

    public override void Update()
    {
        base.Update();

        if (decaying)
        {
            Flicker = Flicker.MoveOverTime(-Radius, 0.1f);
            Color = Color.MoveOverTime(PaletteBasic.Blank, 0.1f);
            if (Flicker <= -Radius)
            {
                Flicker = -Radius;
                remove = true;
                base.Destroy();
            }
        }
        else
        {
            Flicker = MathF.Sin((flickerOffset + Time.Elapsed) * 1) * 5;
            
            if (Radius == 0) Flicker = 0;
        }
    }

    public override void Destroy()
    {
        decaying = true;
        Position = GlobalPosition;
        Game.Root.Dynamic.AddChild(this);
    }
}
