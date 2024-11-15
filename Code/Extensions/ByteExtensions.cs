namespace Thingus;

public static class ByteExtensions
{
    public static byte MoveOverTime(this byte num, byte destination, float time, bool useModifiedDelta = true, float max = 0, float min = 0.1f)
    {
        int a = (int)num;
        int b = (int)destination;
        return (byte)a.MoveOverTime(b, time, useModifiedDelta, max, min);
    }
}
