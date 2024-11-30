using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class TextInputControl : Control
{
    public string Text;
    public Font? Font;

    bool caret;
    float caretTime = 0;

    public TextInputControl(Thing thing, ThingOption option, Control flex) { }

    public override void Start()
    {
        base.Start();

        Pressed = () => { };
    }

    public override void Update()
    {
        base.Update();

        if (IsFocused)
        {
            caretTime -= Time.Delta;
            if (caretTime <= 0) 
            {
                caret = !caret;
                caretTime = 0.5f;
            }

            Text = Input.ApplyKeyboardToString(Text);
        }
        else
        {
            caret = false;
            caretTime = 0;
        }

        Refresh();
    }

    public void Refresh()
    {
        if (Font == null) Font = Library.Font;
        Bounds = new Vector2(Raylib.MeasureText(Text, Font.Value.BaseSize), Font.Value.BaseSize);
    }

    public override void Draw()
    {
        base.Draw();

        DrawText(
            text: $"{Text}{(caret ? "|" : "")}",
            color: Pressed != null && IsHovered ? HighlightColor : Color,
            font: Font
        );
    }
}