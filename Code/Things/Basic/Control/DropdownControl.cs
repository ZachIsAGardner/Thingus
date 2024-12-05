using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class DropdownControl : Control
{
    public string Title;
    public List<string> Options;
    public string Text;
    public Font? Font;

    VerticalFlexControl window;

    public DropdownControl(string title, List<string> options) 
    {
        Title = title;
        Options = options;
        Refresh();
    }

    public override void Start()
    {
        base.Start();

        Pressed = () => { };
    }

    public override void Destroy()
    {
        LoseFocus();
        base.Destroy();
    }

    void Window()
    {
        window = new VerticalFlexControl();
        window.Bounds.X = Bounds.X;
        window.Bounds.Y = Options.Count() * 16;
        window.Padding = new Vector2(1);
        window.Texture = Library.Textures["BoxInsideSlice"];
        window.DrawOrder = DrawOrder + 10;
        window.DrawMode = DrawMode;
        window.Position.Y = 16;

        foreach (string option in Options)
        {
            TextControl control = new TextControl();
            control.Bounds.X = Bounds.X;
            control.Bounds.Y = 16;
            control.Text = option;
            control.TextPadding.X = 3;
            control.HighlightColor = Theme.Dark;
            control.TextHighlightColor = PaletteBasic.White;
            control.Pressed = () =>
            {
                Input.Holdup = true;
                Text = option;
                LoseFocus();
            };
            window.AddChild(control);
        }
        window.Refresh();
        AddChild(window);
    }

    public override void Focus()
    {
        base.Focus();

        Window();
    }

    public override void LoseFocus()
    {
        base.LoseFocus();

        window?.Destroy();
        window = null;
    }

    public override void Update()
    {
        base.Update();

        if (IsFocused && window != null && !window.IsHovered && Input.LeftMouseButtonIsPressed)
        {
            LoseFocus();
        }

        if (!IsFocused && IsHovered && Input.LeftMouseButtonIsPressed)
        {
            Focus();
        }

        Refresh();
    }

    public void Refresh()
    {
        if (Font == null) Font = Library.Font;
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
            color: Pressed != null && IsHovered ? HighlightColor : PaletteBasic.Blank
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
            color: Pressed != null && IsHovered ? Color.Blend(HighlightColor, 0.35f) : Color
        );

        DrawText(
            text: $"{title}{Text}",
            color: Pressed != null && IsHovered ? TextHighlightColor : TextColor,
            font: Font,
            position: GlobalPosition + TextPadding
        );

        DrawSprite(
            texture: Library.Textures["DropdownButton"],
            position: GlobalPosition + new Vector2(8) + new Vector2(Bounds.X - 16, 0)
        );
    }
}