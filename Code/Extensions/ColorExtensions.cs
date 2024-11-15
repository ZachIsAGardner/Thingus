namespace Thingus;

public static class ColorExtensions
{
    public static Color MoveOverTime(this Color color, Color destination, float time, bool useModifiedDelta = true, float max = 0, float min = 0.1f)
    {
        return new Color(
            color.R.MoveOverTime(destination.R, time, useModifiedDelta, max, min),
            color.G.MoveOverTime(destination.G, time, useModifiedDelta, max, min),
            color.B.MoveOverTime(destination.B, time, useModifiedDelta, max, min),
            color.A.MoveOverTime(destination.A, time, useModifiedDelta, max, min)
        );
    }
}
