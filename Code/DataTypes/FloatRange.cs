namespace Thingus;

public struct FloatRange
{
    public float Start;
    public float End;

    public FloatRange(float start, float end)
    {
        Start = start;
        End = end;
    }

    public float Random()
    {
        return Chance.Range(Start, End);
    }
}