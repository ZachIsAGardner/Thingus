using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class RectangleTool : Tool
{
    public override string Name => "Rectangle";
    public override KeyboardKey Shortcut => KeyboardKey.U;
    public override int TileNumber => 12;

    bool erase = false;
    RectangleSelection selection;

    public RectangleTool(Editor editor) : base(editor) { }

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

        dragMousePosition = null;
        dragCameraPosition = null;

        editor.Mouse.TileNumber = 12;

        if (editor.Room == null)
        {
            return;
        }

        if (editor.InteractDisabled) return;

        if (Input.LeftMouseButtonIsHeld && editor.LastGridInteractPosition != editor.GridPosition && selection == null)
        {
            selection = new RectangleSelection(editor.GridPosition, () => editor.GridPosition, new Color(0, 255, 0, 50));
            erase = false;
        }

        if (Input.RightMouseButtonIsHeld && editor.LastGridInteractPosition != editor.GridPosition && selection == null)
        {
            selection = new RectangleSelection(editor.GridPosition, () => editor.GridPosition, new Color(0, 0, 0, 50));
            erase = true;
        }

        if (Input.LeftMouseButtonIsReleased && selection != null && !erase)
        {
            selection.Positions.ForEach(p =>
            {
                editor.AddCell(p);
            });
        }

        if (Input.RightMouseButtonIsReleased && selection != null && erase)
        {
            selection.Positions.ForEach(p =>
            {
                editor.RemoveCell(p);
            });
        }

        if (Input.LeftMouseButtonIsReleased || Input.RightMouseButtonIsReleased)
        {
            selection?.Destroy();
            selection = null;
            erase = false;
        }

        base.Update();
    }

    public override void Draw()
    {
        base.Draw();
    }
}
