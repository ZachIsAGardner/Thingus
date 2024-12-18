using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class SliderControl : Control
{
    public string Text;
    public Font? Font;
    float lastValue;
    public float Value;

    ButtonControl scrollHead;
    bool scrolling = false;
    Vector2 scrollHeadStart;
    Vector2 mouseStart;

    public event Action<object> OnChanged;

    public SliderControl()
    {

    }

    public override void Start()
    {
        base.Start();

        scrollHead = new ButtonControl();
        scrollHead.Texture = Library.Textures["ScrollHead"];
        scrollHead.Bounds = new Vector2(6, 12);
        scrollHead.Padding = new Vector2(8);
        scrollHead.DrawOrder = 10;
        scrollHead.DrawMode = DrawMode;
        scrollHead.Position.X = (Bounds.X - scrollHead.Bounds.X) * Value;
        scrollHead.Position.Y = 6;
        scrollHead.OnPressed += () =>
        {
            scrolling = true;
            scrollHeadStart = scrollHead.Position;
            mouseStart = Input.MousePosition(DrawMode);
        };
        scrollHead.Released = () =>
        {
            scrolling = false;
            Value = scrollHead.Position.X / (Bounds.X - scrollHead.Bounds.X);
        };
        AddChild(scrollHead);
    }

    public override void Update()
    {
        base.Update();

        if (scrolling)
        {
            Vector2 difference = Input.MousePosition(DrawMode) - mouseStart;
            scrollHead.Position.X = scrollHeadStart.X + difference.X;
        }
        else if (IsHovered)
        {
            scrollHead.Position.X -= Input.MouseWheel * 5;
        }

        if (scrollHead.Position.X < 0) scrollHead.Position.X = 0;
        if (scrollHead.Position.X > Bounds.X - scrollHead.Bounds.X) scrollHead.Position.X = Bounds.X - scrollHead.Bounds.X;

        if (scrolling) Value = scrollHead.Position.X / (Bounds.X - scrollHead.Bounds.X);

        Refresh();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();

        if (lastValue != Value)
        {
            if (OnChanged != null) OnChanged.Invoke(Value);
        }
        lastValue = Value;
    }

    public void Increment()
    {
        Value += 0.1f;
        if (Value > 1f) Value = 1f;
        scrollHead.Position.X = (Bounds.X - scrollHead.Bounds.X) * Value;
    }

    public void Decrement()
    {
        Value -= 0.1f;
        if (Value < 0) Value = 0;
        scrollHead.Position.X = (Bounds.X - scrollHead.Bounds.X) * Value;
    }

    public override void Refresh()
    {
        base.Refresh();

        if (Font == null) Font = Library.Font;
        // Bounds = new Vector2(Raylib.MeasureText(Text, Font.Value.BaseSize), Font.Value.BaseSize);
    }

    public override void Draw()
    {
        base.Draw();

        // DrawNineSlice(
        //     texture: Texture,
        //     tileSize: 5,
        //     position: GlobalPosition,
        //     width: (int)(Bounds.X),
        //     height: (int)(Bounds.Y),
        //     color: Color
        // );

        DrawNineSlice(
            texture: Texture,
            tileSize: 5,
            position: GlobalPosition + new Vector2(4) + new Vector2(0, 4),
            width: (int)(Bounds.X - (4 * 2)),
            height: (int)(Bounds.Y - (4 * 2)),
            color: scrollHead?.ShouldShowHighlight == true ? HighlightColor : Color
        );

        DrawText(
            text: $"{Text}: {(Value * 100).RoundTo(0)}",
            color: scrollHead?.ShouldShowHighlight == true ? TextHighlightColor : TextColor,
            font: Font,
            position: GlobalPosition + TextPadding + new Vector2(0, -5)
        );
    }
}