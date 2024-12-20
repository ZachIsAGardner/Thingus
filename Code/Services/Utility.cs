using System.Numerics;
using System.Reflection;
using Raylib_cs;

namespace Thingus;

public static class Utility
{
    public static List<Vector2> ExtraAngles => new List<Vector2>()
    {
        new Vector2(-1, -1),
        new Vector2(-0.425f, -1),
        new Vector2(0, -1),
        new Vector2(0.425f, -1),
        new Vector2(1, -1),
        new Vector2(1, -0.425f),
        new Vector2(1, 0),
        new Vector2(1, 0.425f),
        new Vector2(1, 1),
        new Vector2(0.425f, 1),
        new Vector2(0, 1),
        new Vector2(-0.425f, 1),
        new Vector2(-1, 1),
        new Vector2(-1, 0.425f),
        new Vector2(-1, 0),
        new Vector2(-1, -0.425f),
    };
    
    public static List<Vector2> Angles => new List<Vector2>()
    {
        new Vector2(-1, -1),
        new Vector2(0, -1),
        new Vector2(1, -1),
        new Vector2(1, 0),
        new Vector2(1, 1),
        new Vector2(0, 1),
        new Vector2(-1, 1),
        new Vector2(-1, 0),
    };

    public static List<Vector2> CardinalAngles => new List<Vector2>()
    {
        new Vector2(0, -1),
        new Vector2(1, 0),
        new Vector2(0, 1),
        new Vector2(-1, 0)
    };

    public static Rectangle GetSourceRectangle(int tileNumber, Texture2D texture, int tileSize = 0)
    {
        if (tileSize == 0) tileSize = CONSTANTS.TILE_SIZE;
        Vector2 coord = CoordinatesFromNumber(tileNumber, texture, tileSize);
        return new Rectangle((int)coord.X, (int)coord.Y, tileSize, tileSize);
    }

    public static Vector2 CoordinatesFromNumber(int tileNumber, Texture2D texture, int tileSize = 0)
    {
        if (tileSize == 0) tileSize = CONSTANTS.TILE_SIZE;
        if (tileSize > texture.Width || tileSize > texture.Height) return Vector2.Zero;

        var sheetWidth = texture.Width;
        var tilesPerRow = sheetWidth / tileSize;
        tileNumber--;
        int row = 0;

        while (tileNumber >= tilesPerRow)
        {
            tileNumber -= tilesPerRow;
            row++;
        }

        return new Vector2(tileNumber * tileSize, row * tileSize);
    }

    public static Vector2 Displacement(float magnitude = 1f)
    {
        return new Vector2(Chance.Range(-magnitude, magnitude), Chance.Range(-magnitude, magnitude));
    }

    public static bool CheckRectangleOverlap(Vector2 aTopLeft, Vector2 aBottomRight, Vector2 bTopLeft, Vector2 bBottomRight)
    {
        // If one Collider is on left side of other  
        if (aTopLeft.X >= bBottomRight.X || bTopLeft.X >= aBottomRight.X)
        {
            return false;
        }

        // If one Collider is above other  
        if (aTopLeft.Y >= bBottomRight.Y || bTopLeft.Y >= aBottomRight.Y)
        {
            return false;
        }

        return true;
    }

    public static MethodInfo FindCreateMethod(string name)
    {
        if (name == null) return null;
        Type type = FindType(name);
        if (type == null) return null;
        MethodInfo method = type.GetMethod("Create", new[] { typeof(ThingModel) });
        return method;
    }

    public static Type FindType(string name)
    {
        if (name == null) return null;
        Type type = Type.GetType($"Thingus.{name}");
        if (type == null) type = Type.GetType($"{CONSTANTS.NAMESPACE}.{name}");
        return type;
    }

    public static Vector2 WorldToGridPosition(Vector2 position)
    {
        int r = -1;
        int c = -1;
        if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Grid)
        {
            r = (int)(position.Y / CONSTANTS.TILE_SIZE);
            c = (int)(position.X / CONSTANTS.TILE_SIZE);
        }
        else if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Oblique)
        {
            Vector2 p = new Vector2(
                position.X - (CONSTANTS.TILE_SIZE_THIRD / 2), 
                position.Y - (CONSTANTS.TILE_SIZE_THIRD / 2)
            );
            // for (int i = 0; i < height; i++)
            // {
            //     p.Y += CONSTANTS.TILE_SIZE_OBLIQUE;
            // }
            p.X += CONSTANTS.TILE_SIZE_THIRD;
            r = (int)(p.Y / CONSTANTS.TILE_SIZE_THIRD) - 1;
            p.X -= CONSTANTS.TILE_SIZE_THIRD * r;
            p.X += CONSTANTS.TILE_SIZE_OBLIQUE * ((int)(TokenGrid.Bounds.Y / 2) - 1);
            c = (int)(p.X / CONSTANTS.TILE_SIZE_OBLIQUE);
            // if (wrap)
            // {
            //     if (c < 0) c = (int)TokenGrid.Bounds.X + c;
            //     if (r < 0) r = (int)TokenGrid.Bounds.Y + r;
            // }
        }
        else if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Isometric)
        {
            // TODO
        }
        return new Vector2(c, r);
    }

    public static Vector2 GridToWorldPosition(Vector2 position)
    {
        Vector2 result = new Vector2(0);
        
        if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Grid)
        {
            result = new Vector2(position.X * CONSTANTS.TILE_SIZE, position.Y * CONSTANTS.TILE_SIZE);
        }
        else if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Oblique)
        {
            result = new Vector2(
            (position.X * CONSTANTS.TILE_SIZE_OBLIQUE)
                - CONSTANTS.TILE_SIZE_THIRD / 2,
            (position.Y * CONSTANTS.TILE_SIZE_THIRD)
                + CONSTANTS.TILE_SIZE_THIRD / 2
            );
            result.X += CONSTANTS.TILE_SIZE_THIRD * position.Y;
            result.X += CONSTANTS.TILE_SIZE_OBLIQUE * -((int)(TokenGrid.Bounds.Y / 2) - 1);

        }
        else if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Isometric)
        {
            // TODO
        }
        return result;
    }

    public static Vector2 WorldToScreenPosition(Vector2 position)
    {
        return position - Viewport.CameraPosition + (Viewport.Margin * Viewport.VirtualRatio.Value);
    }

    public static Vector2 ScreenToWorldPosition(Vector2 position)
    {
        return position + Viewport.CameraPosition - (Viewport.Margin * Viewport.VirtualRatio.Value);
    }
}
