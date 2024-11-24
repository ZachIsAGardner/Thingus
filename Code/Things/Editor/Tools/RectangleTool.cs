using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class RectangleTool : Tool
{
    Vector2? start = null;
    bool erase = false;
    List<Vector2> positions = new List<Vector2>() { };
    Texture2D square;

    public RectangleTool(Editor editor) : base(editor)
    {
        if (Game.ProjectionType == ProjectionType.Grid)
        {
            square = Library.Textures[$"{CONSTANTS.TILE_SIZE}Square"];
        }
        else if (Game.ProjectionType == ProjectionType.Oblique)
        {
            square = Library.Textures[$"{CONSTANTS.TILE_SIZE}ObliqueBox"];
        }
        else if (Game.ProjectionType == ProjectionType.Isometric)
        {
            // TODO
        }
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
            int row = 1;
            int column = 1;
            if (Game.ProjectionType == ProjectionType.Grid)
            {
                row = (int)(distance.X / CONSTANTS.TILE_SIZE).Abs();
                column = (int)(distance.Y / CONSTANTS.TILE_SIZE).Abs();
            }
            else if (Game.ProjectionType == ProjectionType.Oblique)
            {
                row = (int)(distance.X / CONSTANTS.TILE_SIZE_OBLIQUE).Abs();
                column = (int)(distance.Y / CONSTANTS.TILE_SIZE_THIRD).Abs();
            }
            else if (Game.ProjectionType == ProjectionType.Isometric)
            {
                // TODO
            }

            for (int r = 0; r <= row; r++)
            {
                for (int c = 0; c <= column; c++)
                {
                    Vector2 position = start.Value;
                    if (Game.ProjectionType == ProjectionType.Grid)
                    { 
                        position += new Vector2(
                            r * CONSTANTS.TILE_SIZE * distance.X.Sign(),
                            c * CONSTANTS.TILE_SIZE * distance.Y.Sign()
                        );
                    }
                    else if (Game.ProjectionType == ProjectionType.Oblique)
                    { 
                        position += new Vector2(
                            r * CONSTANTS.TILE_SIZE_OBLIQUE * distance.X.Sign(),
                            c * CONSTANTS.TILE_SIZE_THIRD * distance.Y.Sign()
                        );
                        if (c % 2 != 0)
                        {
                            position.X += CONSTANTS.TILE_SIZE_THIRD;
                        }
                    }
                    else if (Game.ProjectionType == ProjectionType.Isometric)
                    { 
                        // TODO
                    }

                    positions.Add(position);
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
                Shapes.DrawSprite(texture: square, position: p, color: erase ? new Color(0, 0, 0, 64) : new Color(0, 255, 0, 64));
            });
        }
    }
}
