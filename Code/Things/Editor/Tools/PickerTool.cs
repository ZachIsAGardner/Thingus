
using Thingus;

public class PickerTool : Tool
{
    public override string Name => "Picker";

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

        editor.TogglePreview(true);

        base.Update();
    }

    public override void Draw()
    {
        base.Draw();
    }
}