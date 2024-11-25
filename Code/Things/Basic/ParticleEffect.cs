using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class Particle
{
    public ParticleEffect ParticleEffect;
    public Vector2 Position;
    public Texture2D Texture;
    public Vector2 Velocity;
    public float Dampening;
    public Color Color;
    public float Rotation;
    public float Time;

    public float FadeStart;
    float alpha = 1;

    public Particle() { }
    public Particle(ParticleEffect particleEffect, Vector2 position, Texture2D? texture, Vector2 velocity, float dampening, Color color, float rotation, float time, float fadeStart) 
    {
        ParticleEffect = particleEffect;
        Position = position;
        Texture = texture.Value;
        Velocity = velocity;
        Dampening = dampening;
        Color = color;
        Rotation = rotation;
        Time = time;
        FadeStart = fadeStart;
    }

    public void Update()
    {
        Time -= Thingus.Time.Delta;

        Velocity = new Vector2(Velocity.X.MoveOverTime(0, Dampening), Velocity.Y.MoveOverTime(0, Dampening));
        Position += Velocity * Thingus.Time.ModifiedDelta;
        if (FadeStart > 0f && Time <= FadeStart)
        {
            alpha = Time / FadeStart;
        }
    }

    public void Draw()
    {
        Shapes.DrawSprite(
            texture: Texture,
            position: Position,
            rotation: Rotation,
            color: Color.WithAlpha(alpha),
            drawMode: ParticleEffect.DrawMode
        );
    }
}

public class ParticleEffect : Thing
{
    public List<Particle> Particles = new List<Particle>() { };

    public ParticleEffect() { }
    public ParticleEffect(
        string name = null, Vector2? position = null, DrawMode drawMode = DrawMode.Relative, float drawOrder = 0, float updateOrder = 0,
        int amount = 1, int range = 16, Texture2D? texture = null, float speed = 1f, float dampening = 1f, float time = 1f, Color? color = null
    ) : base (name, position, drawMode, drawOrder, updateOrder) 
    {
        Position = position ?? Vector2.Zero;

        for (int i = 0; i < amount; i++)
        {
            Vector2 displacement = Utility.Displacement(range);
            Particles.Add(new Particle(
                particleEffect: this,
                position: GlobalPosition + displacement,
                texture: texture,
                velocity: displacement * speed,
                dampening: dampening,
                color: color ?? PaletteBasic.White,
                rotation: Chance.Range(0, 360f),
                time: time,
                fadeStart: 0.125f
            ));
        }
    }

    public override void Update()
    {
        base.Update();

        Particles.ForEach(p =>
        {
            p.Update();
        });

        Particles = Particles.Where(p => p.Time > 0).ToList();

        if (Particles.Count <= 0)
        {
            Destroy();
        }
    }

    public override void Draw()
    {
        base.Draw();

        Particles.ForEach(p =>
        {
            p.Draw();
        });
    }
}