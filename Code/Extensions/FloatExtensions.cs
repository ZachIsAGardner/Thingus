using Raylib_cs;
namespace Thingus;

public static class FloatExtensions
{
    public static string ToMoney(this float num)
    {
        string result = RoundTo(num, 2).ToString();
        if (!result.Contains(".")) result += ".00";
        while (result.Split(".").Last().Count() < 2)
        {
            result += "0";
        }
        return "$" + result;
    }

    public static float ToNearest(this float i, float nearest)
    {
        return (float)(Math.Round(i / nearest) * nearest);
    }

    public static float RoundTo(this float num, int place = 1)
    {
        return (float)Math.Round(num, place);
    }

    public static float Abs(this float num)
    {
        return Math.Abs(num);
    }

    public static float Floor(this float i)
    {
        return (float)Math.Floor(i);
    }

    public static bool IsNaN(this float i)
    {
        return float.IsNaN(i);
    }

    public static float Sign(this float num)
    {
        if (IsNaN(num)) return 0;
        return Math.Sign(num);
    }

    public static float Lerp(this float num, float destination, float amount)
    {
        return Raymath.Lerp(num, destination, amount);
    }

    public static float MoveOverTime(this float num, float destination, float time, bool useModifiedDelta = true, float max = 0, float min = 0.1f)
    {
        if (time >= 1) return num;

        float result = num.Lerp(
            destination,
            1 - MathF.Pow(time, (useModifiedDelta ? Time.ModifiedDelta : Time.Delta))
        );

        float difference = (result - num).Abs();
        float direction = (result - num).Sign();

        if (difference == 0) { return destination; }
        else if (max != 0 && difference > max) result = num + (max * direction);
        else if (min != 0 && difference < min)
        {
            result = num + (min * direction);
            if (direction == 1 && result > destination)
            {
                result = destination;
            }
            if (direction == -1 && result < destination)
            {
                result = destination;
            }
        }

        return result;

    }
}
