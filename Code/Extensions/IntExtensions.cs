namespace Thingus;

public static class IntExtensions
{
    public static int MoveOverTime(this int num, int destination, float time, bool useModifiedDelta = true, float max = 0, float min = 0.1f)
    {
        if (min < 1) min = 1;
        return (int)(((float)num).MoveOverTime(destination, time, useModifiedDelta, max, min));
    }

    public static int ToNearest(this int i, int nearest)
    {
        return (int)(MathF.Round(i / nearest) * nearest);
    }

    public static string ToMoney(this int num)
    {
        string result = num.ToString();
        if (!result.Contains(".")) result += ".00";
        while (result.Split(".").Last().Count() < 2)
        {
            result += "0";
        }
        bool minus = result.Contains("-");
        result = result.Replace("-", "");


        return  (minus ? "-" : "") + "$" + result;
    }
}
