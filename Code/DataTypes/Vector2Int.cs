using System.Numerics;

public struct Vector2Int
{
    public int X;
    public int Y;

    public Vector2Int(int value)
    {
        X = Y = value;
    }
    
    public Vector2Int(int x, int y)
    {
        X = x;
        Y = y;
    }

    public readonly bool Equals(Vector2 other)
    {
        return X == other.X && Y == other.Y;
    }
}