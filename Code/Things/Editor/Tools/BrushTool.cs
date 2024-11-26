using Thingus;

public class BrushTool : Tool
{
    public override string Name => "Brush";
    public override KeyboardKey Shortcut => KeyboardKey.B;
    public override int TileNumber => 3;

    public BrushTool(Editor editor) : base(editor)
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

        editor.Mouse.TileNumber = 3;

        dragMousePosition = null;
        dragCameraPosition = null;

        if (editor.Room == null)
        {
            return;
        }

        if (editor.InteractDisabled) return;

        if (Input.LeftMouseButtonIsHeld && editor.LastGridInteractPosition != editor.GridPosition)
        {
            editor.AddCell(editor.GridPosition);
        }

        if (Input.RightMouseButtonIsHeld && editor.LastGridInteractPosition != editor.GridPosition)
        {
            editor.RemoveCell(editor.GridPosition);
        }

        base.Update();
    }

    public override void Draw()
    {
        base.Draw();
    }
}