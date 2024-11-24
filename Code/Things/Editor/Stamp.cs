using Raylib_cs;

namespace Thingus;

public class Stamp
{
    public string Name;
    public Texture2D Texture;
    public ThingImport Import;

    public Stamp() { }
    public Stamp(string name)
    {
        Name = name;
        Texture = Library.Textures[name];
        Import = Library.ThingImports[name];
    }
}