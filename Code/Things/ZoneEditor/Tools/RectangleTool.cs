using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class RectangleTool : Tool
{
    Vector2? start = null;
    bool erase = false;
    List<Vector2> positions = new List<Vector2>() { };
    Texture2D square = Library.Textures[$"{CONSTANTS.TILE_SIZE}White"];

    public RectangleTool(Editor editor) : base(editor)
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

        dragMousePosition = null;
        dragCameraPosition = null;

        editor.Mouse.TileNumber = 12;

        if (editor.Room == null)
        {
            return;
        }

        editor.TogglePreview(true);

        if (editor.InteractDisabled) return;

        if (Input.LeftMouseButtonIsHeld && editor.LastGridInteractPosition != editor.GridPosition && start == null)
        {
            start = editor.GridPosition;
            erase = false;
        }

        if (Input.RightMouseButtonIsHeld && editor.LastGridInteractPosition != editor.GridPosition && start == null)
        {
            start = editor.GridPosition;
            erase = true;
        }

        if (start != null)
        {
            positions.Clear();
            Vector2 distance = editor.GridPosition - start.Value;
            int row = (int)(distance.X / CONSTANTS.TILE_SIZE).Abs();
            int column = (int)(distance.Y / CONSTANTS.TILE_SIZE).Abs();
            for (int r = 0; r <= row; r++)
            {
                for (int c = 0; c <= column; c++)
                {
                    positions.Add(start.Value + new Vector2(r * CONSTANTS.TILE_SIZE * distance.X.Sign(), c * CONSTANTS.TILE_SIZE * distance.Y.Sign()));
                }
            }
        }

        if (Input.LeftMouseButtonIsReleased && start != null && !erase)
        {
            positions.ForEach(p =>
            {
                editor.AddCell(p);
            });
        }

        if (Input.RightMouseButtonIsReleased && start != null && erase)
        {
            positions.ForEach(p =>
            {
                editor.RemoveCell(p);
            });
        }

        if (Input.LeftMouseButtonIsReleased || Input.RightMouseButtonIsReleased)
        {
            editor.LastGridInteractPosition = null;
            start = null;
            erase = false;
            positions.Clear();
        }

        base.Update();
    }

    public override void Draw()
    {
        base.Draw();

        if (start != null)
        {
            positions.ForEach(p =>
            {
                Shapes.DrawSprite(texture: square, position: p, color: erase ? new Color(0, 0, 0, 150) : CONSTANTS.PRIMARY_COLOR);
            });
        }
    }
}
