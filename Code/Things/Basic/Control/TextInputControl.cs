using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class TextInputControl : Control
{
    public string Title;
    public string Text;
    public Font? Font;
    public Action LoseFocus;

    bool caret;
    float caretTime = 0;

    public TextInputControl(string title) 
    {
        Title = title;
    }

    public override void Start()
    {
        base.Start();

        Pressed = () => { };
    }

    public override void Destroy()
    {
        if (LoseFocus != null) LoseFocus();
        if (FocusedControl == this) FocusedControl = null;
        base.Destroy();
    }

    public override void Update()
    {
        base.Update();

        if (IsHovered && Input.LeftMouseButtonIsPressed)
        {
            Game.GetThings<Control>().ForEach(c => c.IsFocused = false);
            IsFocused = true;
            FocusedControl = this;
        }

        if (IsFocused)
        {
            caretTime -= Time.Delta;
            if (caretTime <= 0) 
            {
                caret = !caret;
                caretTime = 0.5f;
            }

            Text = Input.ApplyKeyboardToString(Text);
            if ((Input.LeftMouseButtonIsPressed && !IsHovered) || Input.IsPressed(KeyboardKey.Enter))
            {
                if (LoseFocus != null) LoseFocus();
                IsFocused = false;
                FocusedControl = null;
            } 
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
        // Bounds = new Vector2(Raylib.MeasureText(Text, Font.Value.BaseSize), Font.Value.BaseSize);
    }

    public override void Draw()
    {
        base.Draw();

        DrawNineSlice(
            texture: Texture,
            tileSize: 5,
            position: GlobalPosition - Padding,
            width: (int)(Bounds.X + (Padding.X * 2)),
            height: (int)(Bounds.Y + (Padding.Y * 2)),
            color: Pressed != null && IsHovered ? HighlightColor : Color
        );

        DrawText(
            text: $"{Title}:{Text}{(caret ? "|" : "")}",
            color: Pressed != null && IsHovered ? TextHighlightColor : TextColor,
            font: Font,
            position: GlobalPosition + TextPadding
        );
    }
}