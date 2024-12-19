using System.Numerics;
using Raylib_cs;

namespace Thingus;

// TODO
public class CheckboxControl : HorizontalFlexControl
{
    public string Text;
    public bool Value;
    public event Action<object> OnChanged;

    ButtonControl control;

    public CheckboxControl(string text, bool value, int width, Texture2D? texture = null, Texture2D? checkboxTexture = null)
    {
        Text = text;
        Value = value;

        Bounds.X = width;
        Bounds.Y = 16;
        Texture = texture ?? Library.Textures["WhiteSlice"];
        Color = PaletteBasic.Blank;
        HighlightColor = Theme.Dark;

        control = new ButtonControl();
        control.Texture = checkboxTexture ?? Library.Textures["Checkbox"];
        control.HighlightOverride = this;
        control.Bounds = new Vector2(16);
        control.DrawOrder = 101;
        control.TileSize = 16;
        if (Value) control.TileNumber = 1;
        else control.TileNumber = 0;
        AddChild(control);

        TextControl textControl = new TextControl();
        textControl.HighlightOverride = this;
        textControl.Text = Text;

        OnPressed += () => 
        {
            Toggle();
        };
        AddChild(textControl);

        textControl.Refresh();
        Refresh();
    }

    public void Toggle()
    {
        if (control.TileNumber == 1) control.TileNumber = 0;
        else control.TileNumber = 1;

        Value = control.TileNumber == 1;

        if (OnChanged != null) OnChanged.Invoke(Value);
    }
}