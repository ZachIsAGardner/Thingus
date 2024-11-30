using System.Numerics;
using Thingus;

public class EditTool : Tool
{
    public override string Name => "Edit";
    public override KeyboardKey Shortcut => KeyboardKey.V;
    public override bool ShowPreview => false;
    public override int TileNumber => 0;

    Thing lastThing;
    Thing window;

    public EditTool(Editor editor) : base(editor)
    {

    }

    public override void Update()
    {
        if (Input.AltIsHeld)
        {
            Picker();
            return;
        }

        if (Input.MiddleMouseButtonIsHeld)
        {
            Move();
            return;
        }

        editor.Mouse.TileNumber = 0;

        dragMousePosition = null;
        dragCameraPosition = null;

        if (editor.Room == null) return;
        if (editor.InteractDisabled) return;

        Thing thing = editor.Room.Children.Find(c => c.GlobalPosition == editor.GridPosition);
        if (thing != lastThing && lastThing != null)
        {
            Sprite sprite = lastThing.GetThing<Sprite>();
            if (sprite != null) sprite.Color = PaletteBasic.White;
        }

        if (thing != null)
        {
            editor.Mouse.TileNumber = 1;
            Sprite sprite = thing.GetThing<Sprite>();
            if (sprite != null) sprite.Color = PaletteBasic.Green;
            if (Input.LeftMouseButtonIsPressed)
            {
                if (window != null)
                {
                    CloseWindow();
                }

                window = Window(thing);
            }
        }
        else
        {
            if (Input.LeftMouseButtonIsPressed)
            {
                if (window != null)
                {
                    CloseWindow();
                }
            }
        }
        lastThing = thing;


        base.Update();
    }

    void CloseWindow()
    {
        window.Destroy();
        window = null;
    }

    Thing Window(Thing thing)
    {
        int width = 80;
        int height = 48;
        VerticalFlexControl flex = new VerticalFlexControl();
        flex.Position = thing.GlobalPosition + new Vector2((width / 2) - 8, -height);
        flex.Bounds = new Vector2(width, height);
        flex.Texture = Library.Textures["BoxThinSlice"];
        flex.TileSize = 5;
        flex.Padding = 3;
        flex.DrawOrder = 100;

        HorizontalFlexControl title = new HorizontalFlexControl();
        title.Texture = Library.Textures["BoxThinLightSlice"];
        title.Bounds = new Vector2(width, 8);
        title.Padding = 3;
        title.Justify = FlexJustify.Between;
        title.Offset = new Vector2(2, -3);
        flex.AddChild(title);

        TextControl textControl = new TextControl();
        textControl.Text = thing.Name;
        textControl.Color = PaletteBasic.LightGray;
        textControl.Padding = 0;
        title.AddChild(textControl);

        ButtonControl close = new ButtonControl();
        close.Pressed = () => 
        {
            CloseWindow();
        };
        close.Texture = Library.Textures["CrossButton"];
        close.Bounds = new Vector2(16);
        close.DrawOrder = 101;
        close.TileSize = 16;
        title.AddChild(close);

        textControl.Refresh();

        foreach (ThingOption option in thing.Cell.Import.Options)
        {
            if (option.Type == "Checkbox") Checkbox(thing, option, flex);
            if (option.Type == "TextInput") TextInput(thing, option, flex);
        }

        return flex;
    }

    Control Checkbox(Thing thing, ThingOption option, Control flex)
    {
        HorizontalFlexControl h = new HorizontalFlexControl();
        h.Bounds.X = flex.Bounds.X;
        h.Bounds.Y = 16;
        h.Texture = Library.Textures["WhiteSlice"];
        h.Color = PaletteBasic.Blank;
        h.HighlightColor = PaletteBasic.DarkGreen;
        flex.AddChild(h);

        ButtonControl control = new ButtonControl();
        control.Texture = Library.Textures["Checkbox"];
        control.Bounds = new Vector2(16);
        control.DrawOrder = 101;
        control.TileSize = 16;
        h.AddChild(control);

        TextControl textControl = new TextControl();
        textControl.Text = option.Name;

        h.Pressed = () => 
        {
            if (control.TileNumber == 1) control.TileNumber = 0;
            else control.TileNumber = 1;
        };
        h.AddChild(textControl);

        return control;
    }

    Control TextInput(Thing thing, ThingOption option, Control flex)
    {
        Control control = new TextInputControl(thing, option, flex);
        control.DrawOrder = 100000;
        flex.AddChild(control);
        return control;
    }
}