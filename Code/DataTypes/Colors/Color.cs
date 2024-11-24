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

    public Raylib_cs.Color ToRaylib() => new Raylib_cs.Color(R, G, B, A);
}
