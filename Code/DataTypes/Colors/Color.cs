namespace Thingus;

public struct Color
{
    public byte R;
    public byte G;
    public byte B;
    public byte A;

    public Color(string hex)
    {
        System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml(hex.Contains('#') ? hex : $"#{hex}");
        R = color.R;
        G = color.G;
        B = color.B;
        A = color.A;
    }

    public Color(byte r, byte g, byte b, byte a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public Color(int r, int g, int b, int a)
    {
        R = Convert.ToByte(r);
        G = Convert.ToByte(g);
        B = Convert.ToByte(b);
        A = Convert.ToByte(a);
    }

    public override string ToString()
    {
        return $"{{R:{R} G:{G} B:{B} A:{A}}}";
    }

    public Color WithAlpha(float alpha) => new Color(R, G, B, (byte)(alpha * 255));

    public Raylib_cs.Color ToRaylib() => new Raylib_cs.Color(R, G, B, A);

    
    static Dictionary<string, Color> cacheHex = new Dictionary<string, Color>() { };
    static Dictionary<Color, string> cacheColor = new Dictionary<Color, string>() { };

    public static Color HexToColor(string hex)
    {
        if (String.IsNullOrWhiteSpace(hex)) return PaletteBasic.White;

        if (cacheHex.ContainsKey(hex)) return cacheHex[hex];

        try
        {
            System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml(hex.Contains('#') ? hex : $"#{hex}");
            Color result = new Color(color.R, color.G, color.B, color.A);
            cacheHex[hex] = result;
            return result;
        }
        catch (Exception error)
        {
            return PaletteBasic.White;
        }
    }
}
