
using Thingus;

public class HandTool : Tool
{
    public override string Name => "Hand";
    public override KeyboardKey Shortcut => KeyboardKey.H;
    public override int TileNumber => 5;
    public override bool ShowPreview => false;

    public HandTool(Editor editor) : base(editor)
    {

    }

    public override void Update()
    {
        if (Input.AltIsHeld)
        {
            Picker();
            return;
        }

        editor.Mouse.TileNumber = 5;

        if (editor.Room == null)
        {
            return;
        }

        if (Input.MiddleMouseButtonIsHeld || Input.LeftMouseButtonIsHeld)
        {
            Move();
        }
        else
        {
            dragMousePosition = null;
            dragCameraPosition = null;
        }

        base.Update();
    }

    public override void Draw()
    {
        base.Draw();
    }
}