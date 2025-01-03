using Raylib_cs;

namespace Thingus;

public struct Font
{
    Raylib_cs.Font font;

    public Texture2D Texture => font.Texture;
    public int BaseSize => font.BaseSize;
    public int Height;
    public int Width;

    public Font(Raylib_cs.Font font)
    {
        this.font = font;
    }

    public Raylib_cs.Font ToRaylib() => font;
}
