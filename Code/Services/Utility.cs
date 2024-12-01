using System.Numerics;
using System.Reflection;
using Raylib_cs;

namespace Thingus;

public static class Utility
{
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
        MethodInfo method = type.GetMethod("Create", new[] { typeof(Thing), typeof(ThingModel) });
        return method;
    }

    public static Type FindType(string name)
    {
        if (name == null) return null;
        Type type = Type.GetType($"Thingus.{name}");
        if (type == null) type = Type.GetType($"{CONSTANTS.NAMESPACE}.{name}");
        return type;
    }
}
