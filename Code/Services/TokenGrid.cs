using System.Numerics;

namespace Thingus;

public static class TokenGrid
{
    public static List<List<int>> Grid = new List<List<int>>() { };
    public static Vector2 Bounds = new Vector2((Grid.Count > 0) ? Grid.First().Count() : 0, Grid.Count());

    public static void Start()
    {
        Reset();
    }

    public static void Reset()
    {
        Grid.Clear();
    }

    public static int Get(int x, int y)
    {
        // if (x < 0 || y < 0 || x >= Bounds.X || y >= Bounds.Y) return -1;
        if (x < 0 || y < 0) return -1;
        return Grid.Get(y, x);
    }

    public static void Set(int x, int y, int value)
    {
        // if (x < 0 || y < 0 || x >= Bounds.X || y >= Bounds.Y) return;
        if (x < 0 || y < 0) return;
        if (value > Grid.Get(y, x)) Grid.Set(y, x, value);
    }
}