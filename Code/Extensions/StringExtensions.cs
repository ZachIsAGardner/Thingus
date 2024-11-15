namespace Thingus;

public static class StringExtensions
{
    public static int? ToInt(this string str)
    {
        int? result = null;

        bool success = int.TryParse(str, out int parsed);
        if (success) result = parsed;

        return result;
    }

    public static float? ToFloat(this string str)
    {
        float? result = null;

        bool success = float.TryParse(str, out float parsed);
        if (success) result = parsed;

        return result;
    }

    public static int? ToInt(this char str)
    {
        int? result = null;

        bool success = int.TryParse($"{str}", out int parsed);
        if (success) result = parsed;

        return result;
    }

    public static float? ToFloat(this char str)
    {
        float? result = null;

        bool success = float.TryParse($"{str}", out float parsed);
        if (success) result = parsed;

        return result;
    }
}
