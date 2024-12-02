using System.Numerics;

namespace Thingus;

public static class TokenGrid
{
    public static Vector2 Bounds = new Vector2(64);
    static int[][] grid = new int[64][];

    public static void Start()
    {
        Reset();
    }

    public static void Reset() => Reset(new Vector2(64));
    public static void Reset(Vector2 bounds)
    {
        Bounds = bounds;
        grid = new int[(int)bounds.Y][];
        for (int r = 0; r < grid.Count(); r++)
        {
            grid[r] = new int[(int)bounds.X];
        }
    }

    public static int Get(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Bounds.X || y >= Bounds.Y) return -1;
        return grid[y][x];
    }

    public static void Set(int x, int y, int value)
    {
        if (x < 0 || y < 0 || x >= Bounds.X || y >= Bounds.Y) return;
        if (value > grid[y][x]) grid[y][x] = value;
    }
}