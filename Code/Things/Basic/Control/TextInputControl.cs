using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class TextInputControl : Control
{
    public string Title;
    public string Text;
    public Font? Font;

    bool caret;
    float caretTime = 0;

    public TextInputControl(string title) 
    {
        Title = title;

        Refresh();
    }

    public override void Start()
    {
        base.Start();

        OnPressed += () => { };
    }

    public override void Destroy()
    {
        LoseFocus();
        base.Destroy();
    }

    public override void Focus()
    {
        base.Focus();

        TextInputControl = this;
    }

    public override void LoseFocus()
    {
        base.LoseFocus();

        if (TextInputControl == this) TextInputControl = null;
    }

    public override void Update()
    {
        base.Update();

        if (IsHovered && Input.LeftMouseButtonIsPressed)
        {
            Focus();
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
                LoseFocus();
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
            texture: Library.Textures["WhiteSlice"],
            tileSize: 5,
            position: GlobalPosition - Padding,
            width: (int)(Bounds.X + (Padding.X * 2)),
            height: (int)(Bounds.Y + (Padding.Y * 2)),
            color: ShouldShowHighlight ? HighlightColor : PaletteBasic.Blank
        );

        string title = Title;
        int length = title.Width(Font.Value);
        if (Title?.HasValue() == true) title += " ";

        DrawNineSlice(
            texture: Texture,
            tileSize: 5,
            position: new Vector2(length, 0) + GlobalPosition - Padding,
            width: (int)(Bounds.X + (Padding.X * 2)) - length,
            height: (int)(Bounds.Y + (Padding.Y * 2)),
            color: ShouldShowHighlight ? Color.Blend(HighlightColor, 0.35f) : Color
        );

        DrawText(
            text: $"{title}{Text}{(caret ? "|" : "")}",
            color: ShouldShowHighlight ? TextHighlightColor : TextColor,
            font: Font,
            position: GlobalPosition + TextPadding
        );
    }
}