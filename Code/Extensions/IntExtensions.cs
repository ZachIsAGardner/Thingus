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
}
