using System.Numerics;
using Raylib_cs;

namespace Thingus;

public static class Vector2Extensions
{
    public static float Force(this Vector2 vector2)
    {
        return (vector2.X.Abs() + vector2.Y.Abs()) / 2f;
    }

    public static Vector2 Sign(this Vector2 vector2)
    {
        return new Vector2(MathF.Sign(vector2.X), MathF.Sign(vector2.Y));
    }
    public static Vector2 ToNearest(this Vector2 vector2, int nearest)
    {
        return new Vector2(
            (float)(Math.Round(vector2.X / nearest) * nearest),
            (float)(Math.Round(vector2.Y / nearest) * nearest)
        );
    }

    public static Vector2 RoundTo(this Vector2 v2, int place = 1)
    {
        return  new Vector2(
            (float)Math.Round(v2.X, place),
            (float)Math.Round(v2.Y, place)
        );
    }

    public static Vector2 MoveOverTime(this Vector2 v2, Vector2 destination, float time, bool useModifiedDelta = true, float max = 0, float min = 0.1f)
    {
        return new Vector2(
            v2.X.MoveOverTime(destination.X, time, useModifiedDelta, max, min),
            v2.Y.MoveOverTime(destination.Y, time, useModifiedDelta, max, min)
        );
    }

    public static Vector2 DirectionTo(this Vector2 v2, Vector2 other)
    {
        return (other - v2).Normalized();
    }

    public static Vector2 DirectionFrom(this Vector2 v2, Vector2 other)
    {
        return (v2 - other).Normalized();
    }

    public static float DistanceFrom(this Vector2 vector2, Vector2 other)
    {
        return (vector2.X - other.X).Abs() + (vector2.Y - other.Y).Abs();
    }

    public static Vector2 Normalized(this Vector2 vector2)
    {
        return Raymath.Vector2Normalize(vector2);
    }

    public static float ToRadians(this Vector2 vector2, Vector2? other = null)
    {
        return MathF.Atan2(other?.Y ?? 0 - vector2.Y, other?.X ?? 0 - vector2.X);
    }

    public static float ToDegrees(this Vector2 vector2, Vector2? other = null)
    {
        return vector2.ToRadians(other) * (180f / MathF.PI);
    }
}
