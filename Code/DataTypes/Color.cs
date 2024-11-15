namespace Thingus;

public static class Colors
{
    public static Color Blank = new Color(0, 0, 0, 0);
    public static Color Black = new Color("#050403");
    public static Color Gray6 = new Color("#282c3c");
    public static Color Gray5 = new Color("#464762");
    public static Color Gray4 = new Color("#696682");
    public static Color Gray3 = new Color("#9a97b9");
    public static Color Gray2 = new Color("#c5c7dd");
    public static Color Gray1 = new Color("#e6e7f0");
    public static Color White = new Color("#ffffff");
    public static Color Blue = new Color("#0000ff");
    public static Color Red = new Color("#ff0000");
    public static Color Green = new Color("#00ff00");
    public static Color Cyan = new Color("#00ffff");
    public static Color Yellow = new Color("#ffff00");
    public static Color Purple = new Color("#ff00ff");
}

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

    public Raylib_cs.Color ToRaylib() => new Raylib_cs.Color(R, G, B, A);
}
