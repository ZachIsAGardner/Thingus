using System.Numerics;
using Thingus;

public class WandTool : Tool
{
    public override string Name => "Wand";
    public override KeyboardKey Shortcut => KeyboardKey.Q;
    public override int TileNumber => 13;

    public WandTool(Editor editor) : base(editor)
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

        editor.Mouse.TileNumber = 13;

        dragMousePosition = null;
        dragCameraPosition = null;

        if (editor.Room == null)
        {
            return;
        }

        if (editor.InteractDisabled) return;

        if (Input.LeftMouseButtonIsHeld && editor.LastGridInteractPosition != editor.GridPosition)
        {
            StartWand();
        }

        base.Update();
    }

    void StartWand()
    {
        FloodWand(editor.GridPosition);
    }

    void FloodWand(Vector2 position)
    {
        if (editor.Room == null || editor.Stamp == null) return;

        if (position.Y <= editor.Room.Position.Y || position.Y - CONSTANTS.TILE_SIZE_HALF >= editor.Room.Position.Y + editor.Room.Bounds.Y || position.X <= editor.Room.Position.X || position.X - CONSTANTS.TILE_SIZE_HALF >= editor.Room.Position.X + editor.Room.Bounds.X)
        {
            return;
        }

        MapCell targetCell = editor.Room.Map.Cells.Find(c => c.Position + editor.Room.Position == position);
        if (targetCell == null) return;
        ThingImport targetThingImport = null;
        if (targetCell != null) targetThingImport = Library.ThingImports.Get(targetCell.Name);
        if (targetCell != null && targetCell.Name == editor.Stamp.Import.Name) return;

        List<MapCell> targetCells = editor.Room.Map.Cells.Where(c => c.Name == targetCell.Name && (targetThingImport == null || targetCell.TileNumber == c.TileNumber)).ToList();
        foreach (MapCell cell in targetCells)
        {
            editor.AddCell(cell.Position + editor.Room.Position);
        }
    }
}