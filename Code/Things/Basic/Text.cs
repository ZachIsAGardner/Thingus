using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class Text : Thing
{
    public Font Font;
    public Color Color;
    public Color? OutlineColor;
    public AdjustFrom AdjustFrom = AdjustFrom.Auto;

    RenderTexture2D? texture;
    Rectangle rectangle;
    public string Content = "";

    // Shake
    float shakeXOffset;
    float shakeXDuration = 0;
    float shakeXTime = 0;
    float shakeXMagnitude = 1;
    int shakeXDirection = 1;
    float shakeXStep = 0;

    float shakeYOffset;
    float shakeYDuration = 0;
    float shakeYTime = 0;
    float shakeYMagnitude = 1;
    int shakeYDirection = 1;
    float shakeYStep = 0;

    float shakeStepDuration = 0.01f;

    public Text() { }
    public Text(string content, Vector2 position, Font? font = null, DrawMode drawMode = DrawMode.Relative, Color? color = null, Color? outlineColor = null, int order = 0, AdjustFrom adjustFrom = AdjustFrom.Auto)
    {
        this.Content = content;
        Position = position;
        Color = color ?? PaletteBasic.Black;
        OutlineColor = outlineColor;
        Font = font ?? Library.Font;
        DrawMode = drawMode;
        DrawOrder = order;
        AdjustFrom = adjustFrom;
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();

        CheckShake();
    }

    public override void Draw()
    {
        base.Draw();

        Shapes.DrawText(
            font: Font,
            text: Content,
            position: GlobalPosition
                + new Vector2(shakeXOffset, shakeYOffset),
            color: Color,
            outlineColor: OutlineColor,
            drawMode: DrawMode,
            adjustFrom: AdjustFrom
        );
    }

    public void ShakeX(float magnitude = 2, float duration = 0.25f)
    {
        shakeXMagnitude = magnitude;
        shakeXTime = shakeXDuration = duration;
        shakeXStep = 0;
    }

    public void ShakeY(float magnitude = 2, float duration = 0.25f)
    {
        shakeYMagnitude = magnitude;
        shakeYTime = shakeYDuration = duration;
        shakeYStep = 0;
    }

    public void Shake(float magnitude = 2, float duration = 0.25f)
    {
        if (Chance.OneIn(2)) ShakeX(magnitude, duration);
        else ShakeY(magnitude, duration);
    }

    void CheckShake()
    {
        if (shakeXTime > 0)
        {
            shakeXTime -= Time.Delta;
            shakeXStep -= Time.Delta;
            if (shakeXStep <= 0)
            {
                shakeXOffset = shakeXMagnitude * shakeXDirection * (shakeXTime / shakeXDuration);
                shakeXDirection *= -1;
                shakeXStep = shakeStepDuration;
            }
        }
        else
        {
            shakeXOffset = 0;
        }

        if (shakeYTime > 0)
        {
            shakeYTime -= Time.Delta;
            shakeYStep -= Time.Delta;
            if (shakeYStep <= 0)
            {
                shakeYOffset = shakeYMagnitude * shakeYDirection * (shakeYTime / shakeYDuration);
                shakeYDirection *= -1;
                shakeYStep = shakeStepDuration;
            }
        }
        else
        {
            shakeYOffset = 0;
        }
    }
}
