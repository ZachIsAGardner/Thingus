using System.Numerics;
using Thingus;

public class FillTool : Tool
{
    public override string Name => "Fill";
    public override KeyboardKey Shortcut => KeyboardKey.G;
    public override int TileNumber => 6;

    public FillTool(Editor editor) : base(editor)
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

        editor.Mouse.TileNumber = 6;

        dragMousePosition = null;
        dragCameraPosition = null;

        if (editor.Room == null)
        {
            return;
        }

        if (editor.InteractDisabled) return;

        if (Input.LeftMouseButtonIsHeld && editor.LastGridInteractPosition != editor.GridPosition)
        {
            StartFill();
        }

        base.Update();
    }

    void StartFill()
    {
        FloodFill(editor.GridPosition);
    }

    void FloodFill(Vector2 position)
    {
        if (editor.Room == null || editor.Stamp == null) return;

        if (position.Y < editor.Room.Position.Y || position.Y - CONSTANTS.TILE_SIZE_HALF > editor.Room.Position.Y + editor.Room.Bounds.Y || position.X < editor.Room.Position.X || position.X - CONSTANTS.TILE_SIZE_HALF > editor.Room.Position.X + editor.Room.Bounds.X)
        {
            return;
        }

        Stack<Vector2> tiles = new Stack<Vector2>();
        Stack<Vector2> visited = new Stack<Vector2>();
        MapCell targetCell = editor.Room.Map.Cells.Find(c => c.Position + editor.Room.Position == position);
        ThingImport targetTextureInfo = null;
        if (targetCell != null) targetTextureInfo = Library.ThingImports.Get(targetCell.Name);
        if (targetCell != null && targetCell.Name == editor.Stamp.Import.Name) return;
        tiles.Push(position);
        visited.Push(position);

        int max = 10000;
        while (tiles.Count > 0)
        {
            max--;
            if (max <= 0)
            {
                // Timed out
                break;
            }

            Vector2 a = tiles.Pop();
            // make sure we stay within bounds
            if (a.X - CONSTANTS.TILE_SIZE_HALF <= editor.Room.Position.X + editor.Room.Bounds.X && a.X >= editor.Room.Position.X &&
                    a.Y - CONSTANTS.TILE_SIZE_HALF <= editor.Room.Position.Y + editor.Room.Bounds.Y && a.Y >= editor.Room.Position.Y)
            {
                MapCell cell = editor.Room.Map.Cells.Find(c => c.Position + editor.Room.Position == a && c.Import.Layer == LayerControl.CurrentLayer);

                if (cell?.Name == targetCell?.Name && (targetTextureInfo == null || cell == null || cell.TileNumber == targetCell.TileNumber))
                {
                    editor.AddCell(a);

                    Vector2 left = new Vector2(a.X - CONSTANTS.TILE_SIZE, a.Y);
                    Vector2 right = new Vector2(a.X + CONSTANTS.TILE_SIZE, a.Y);
                    Vector2 up = new Vector2(a.X, a.Y - CONSTANTS.TILE_SIZE);
                    Vector2 down = new Vector2(a.X, a.Y + CONSTANTS.TILE_SIZE);
                    if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Oblique)
                    {
                        left = new Vector2(a.X - CONSTANTS.TILE_SIZE_OBLIQUE, a.Y);
                        right = new Vector2(a.X + CONSTANTS.TILE_SIZE_OBLIQUE, a.Y);
                        up = new Vector2(a.X - CONSTANTS.TILE_SIZE_THIRD, a.Y - CONSTANTS.TILE_SIZE_THIRD);
                        down = new Vector2(a.X + CONSTANTS.TILE_SIZE_THIRD, a.Y + CONSTANTS.TILE_SIZE_THIRD);

                    }
                    else if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Isometric)
                    {
                        // TODO
                    }
                    
                    if (left.X >= editor.Room.Position.X && !visited.Contains(left))
                    {
                        tiles.Push(left);
                        visited.Push(left);
                    }

                    if (right.X - CONSTANTS.TILE_SIZE_HALF <= editor.Room.Position.X + editor.Room.Bounds.X && !visited.Contains(right))
                    {
                        tiles.Push(right);
                        visited.Push(right);
                    }

                    if (up.Y >= editor.Room.Position.Y && !visited.Contains(up))
                    {
                        tiles.Push(up);
                        visited.Push(up);
                    }

                    if (down.Y - CONSTANTS.TILE_SIZE_HALF <= editor.Room.Position.Y + editor.Room.Bounds.Y && !visited.Contains(down))
                    {
                        tiles.Push(down);
                        visited.Push(down);
                    }
                }
            }
        }
    }
}