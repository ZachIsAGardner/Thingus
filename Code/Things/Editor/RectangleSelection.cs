using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class RectangleSelection : Thing
{
    public List<Vector2> Positions = new List<Vector2>() { };

    Vector2? start = null;
    Func<Vector2> end = null;
    Texture2D square;
    Color color;

    Action state;
    
    public RectangleSelection(Vector2 start, Func<Vector2> end, Color color)
    {
        this.start = start;
        this.end = end;
        this.color = color;
        
        if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Grid)
        {
            square = Library.Textures[$"{CONSTANTS.TILE_SIZE}Square"];
        }
        else if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Oblique)
        {
            square = Library.Textures[$"{CONSTANTS.TILE_SIZE}ObliqueBox"];
        }
        else if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Isometric)
        {
            // TODO
        }

        state = Drag;
    }
    
    public override void Update()
    {
        base.Update();

        if (state != null) state();
    }

    public void ChangeState(Action state)
    {
        if (state == Move) start = end();
        this.state = state;
    }

    public void Drag()
    {
        if (start != null)
        {
            Positions.Clear();
            Vector2 distance = end() - start.Value;
            int row = 1;
            int column = 1;
            if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Grid)
            {
                row = (int)(distance.X / CONSTANTS.TILE_SIZE).Abs();
                column = (int)(distance.Y / CONSTANTS.TILE_SIZE).Abs();
            }
            else if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Oblique)
            {
                row = (int)(distance.X / CONSTANTS.TILE_SIZE_OBLIQUE).Abs();
                column = (int)(distance.Y / CONSTANTS.TILE_SIZE_THIRD).Abs();
            }
            else if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Isometric)
            {
                // TODO
            }

            for (int r = 0; r <= row; r++)
            {
                for (int c = 0; c <= column; c++)
                {
                    Vector2 position = start.Value;
                    if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Grid)
                    { 
                        position += new Vector2(
                            r * CONSTANTS.TILE_SIZE * distance.X.Sign(),
                            c * CONSTANTS.TILE_SIZE * distance.Y.Sign()
                        );
                    }
                    else if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Oblique)
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
                    else if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Isometric)
                    { 
                        // TODO
                    }

                    Positions.Add(position);
                }
            }
        }
    }

    public void Move()
    {
        Vector2 distance = end() - start.Value;
        Position = distance;
    }

    public override void Draw()
    {
        base.Draw();

        Positions.ForEach(p =>
        {
            DrawSprite(
                texture: square, 
                position: GlobalPosition + p, 
                color: color
            );
        });
    }
}