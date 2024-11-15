using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class ThingModel
{
    public string Name;
    public Vector2 Position;
    public int? TileNumber = null;
    public int? TileSize = null;
    public int Layer = 0;
    public BlendMode? BlendMode = null;
    public DrawMode DrawMode;
    public int DrawOrder;
    public int UpdateOrder;
    public List<string> Tags = new List<string>() { };
    public Dictionary<string, string> Properties = new Dictionary<string, string>() { };

    public ThingModel() { }
    public ThingModel(ThingImport import, MapCell cell)
    {
        Name = import.Name;
        Position = cell.Position;
        TileNumber = cell.TileNumber;
        TileSize = import.TileSize;
        if (import.Tags != null) Tags = import.Tags;
        Properties = import.Properties;
        Layer = import.Layer;
        BlendMode = import.BlendMode;
    }
    public ThingModel(string name, Vector2 position)
    {
        Name = name;
        Position = position;
    }
}
