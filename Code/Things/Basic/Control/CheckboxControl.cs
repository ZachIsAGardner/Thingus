using System.Numerics;

namespace Thingus;

// TODO
public class CheckboxControl : HorizontalFlexControl
{
    public string Text;
    public bool Value;

    public CheckboxControl(string text, bool value, int width)
    {
        Text = text;
        Value = value;

        Bounds.X = width;
        Bounds.Y = 16;
        Texture = Library.Textures["WhiteSlice"];
        Color = PaletteBasic.Blank;
        HighlightColor = Theme.Dark;

        ButtonControl control = new ButtonControl();
        control.Texture = Library.Textures["Checkbox"];
        control.Bounds = new Vector2(16);
        control.DrawOrder = 101;
        control.TileSize = 16;
        if (Value) control.TileNumber = 1;
        else control.TileNumber = 0;
        AddChild(control);

        TextControl textControl = new TextControl();
        textControl.Text = Text;

        OnPressed += () => 
        {
            if (control.TileNumber == 1) control.TileNumber = 0;
            else control.TileNumber = 1;

            Value = control.TileNumber == 1;
        };
        AddChild(textControl);

        textControl.Refresh();
        Refresh();
    }
}