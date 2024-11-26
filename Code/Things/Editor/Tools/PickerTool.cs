
using Thingus;

public class PickerTool : Tool
{
    public override string Name => "Picker";
    public override KeyboardKey Shortcut => KeyboardKey.I;
    public override int TileNumber => 2;

    public PickerTool(Editor editor) : base(editor)
    {

    }

    public override void Update()
    {
        if (Input.MiddleMouseButtonIsHeld)
        {
            Move();
            return;
        }

        dragMousePosition = null;
        dragCameraPosition = null;

        editor.Mouse.TileNumber = 2;

        if (editor.Room == null)
        {
            return;
        }

        Picker();

        base.Update();
    }

    public override void Draw()
    {
        base.Draw();
    }
}