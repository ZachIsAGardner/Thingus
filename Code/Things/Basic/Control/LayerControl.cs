using System.Numerics;

namespace Thingus;

// TODO
public class LayerControl : HorizontalFlexControl
{
    public static string CurrentLayer;

    public string Text;
    public bool Visible;

    public LayerControl(string text, bool visible, int width)
    {
        Text = text;
        Visible = visible;

        Bounds.X = width;
        Bounds.Y = 16;
        Texture = Library.Textures["WhiteSlice"];
        Color = PaletteBasic.Blank;
        HighlightColor = Theme.Dark;

        ButtonControl control = new ButtonControl();
        control.Texture = Library.Textures["Visible"];
        control.Bounds = new Vector2(16);
        control.DrawOrder = 101;
        control.TileSize = 16;
        if (Visible) control.TileNumber = 1;
        else control.TileNumber = 0;
        AddChild(control);

        TextControl textControl = new TextControl();
        textControl.Text = Text;

        control.OnPressed += () => 
        {
            if (control.TileNumber == 1) control.TileNumber = 0;
            else control.TileNumber = 1;

            Visible = control.TileNumber == 1;

            Game.Things.Where(t => t.Layer == Text).ToList().ForEach(t => t.SetVisible(Visible));
        };
        OnPressed += () =>
        {
            CurrentLayer = Text;
        };
        AddChild(textControl);

        textControl.Refresh();
        Refresh();
    }

    public override bool ShouldShowHighlight => base.ShouldShowHighlight || CurrentLayer == Text;

    public override void Update()
    {
        base.Update();
    }

    public override void Init()
    {
        base.Init();
    }
}