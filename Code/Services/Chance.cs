namespace Thingus;

public static class Chance
{
    public static int Seed;

    static Random GetRandom()
    {
        Random r = new Random();
        // Random r = new Random(Seed);
        Seed++;
        return r;
    }

    public static int Range(int start, int end)
    {
        int result = GetRandom().Next(
            Math.Min(start, end),
            Math.Max(start, end)
        );

        return result;
    }

    public static float Range(float start, float end)
    {
        double result = (GetRandom().NextDouble() * (end - start) + start);

        return (float)result;
    }

    public static bool OneIn(int num)
    {
        int result = GetRandom().Next(0, num);
        return result == 1;
    }
}
