using System.Numerics;
using System.Text;
using Raylib_cs;

namespace Thingus;

public static class StringExtensions
{
        public static string ToSnakeCase(this string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }
            if (text.Length < 2)
            {
                return text;
            }
            var sb = new StringBuilder();
            sb.Append(char.ToLowerInvariant(text[0]));
            for (int i = 1; i < text.Length; ++i)
            {
                char c = text[i];
                if (char.IsUpper(c))
                {
                    sb.Append('_');
                    sb.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

    public static string ToUpperSnakeCase(this string text) => text.ToSnakeCase().ToUpper();
    
    public static Vector2 ToVector2(this string str)
    {
        if (str == null) return Vector2.Zero;
        
        if (str.ToLower() == "up") return new Vector2(0, -1);
        if (str.ToLower() == "down") return new Vector2(0, 1);
        if (str.ToLower() == "left") return new Vector2(-1, 0);
        if (str.ToLower() == "right") return new Vector2(1, 0);
        return Vector2.Zero;
    }

    public static int Width(this string str, Font? font = null)
    {
        return Raylib.MeasureText(str, (font ?? Library.Font).BaseSize);
    }
    public static bool HasValue(this string str)
    {
        if (str == null) return false;
        return str.Trim().Count() > 0;
    }

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
