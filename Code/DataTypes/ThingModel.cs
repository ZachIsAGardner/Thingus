using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class ThingModel
{
    public string Name;
    public Thing Root;
    public Vector2 Position;
    public Vector2 Bounds;
    public int? TileNumber = null;
    public int? TileSize = null;
    public string Layer = "Main";
    public BlendMode? BlendMode = null;
    public DrawMode DrawMode;

    public List<string> Tags = new List<string>() { };
    public Dictionary<string, string> Properties = new Dictionary<string, string>() { };
    public int Token;

    public ThingModel() { }
    public ThingModel(ThingImport import, MapCell cell)
    {
        Name = cell.Name;
        Position = cell.Position;
        Bounds = cell.Bounds;

        TileNumber = cell.TileNumber;
        TileSize = import.TileSize;
        if (import.Tags != null) Tags = import.Tags;
        Properties = import.Properties;
        cell.Options.ForEach(o => Properties[o.Name] = o.Value);
        Token = import.Token;
        Layer = import.Layer;
        BlendMode = import.BlendMode;
    }
    public ThingModel(MapCell cell)
    {
        Name = cell.Name;
        Position = cell.Position;
        Bounds = cell.Bounds;
        TileNumber = cell.TileNumber;
    }
    public ThingModel(string name, Vector2 position)
    {
        Name = name;
        Position = position;
    }
}
