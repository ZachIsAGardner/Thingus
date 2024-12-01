using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class ThingModel
{
    public string Name;
    public Vector2 Position;
    public Vector2 Bounds;
    public int? TileNumber = null;
    public int? TileSize = null;
    public int Layer = 0;
    public BlendMode? BlendMode = null;
    public DrawMode DrawMode;
    public int DrawOrder;
    public int UpdateOrder;
    public List<string> Tags = new List<string>() { };
    public List<ThingProperty> Properties = new List<ThingProperty>() { };

    public ThingModel() { }
    public ThingModel(ThingImport import, MapCell cell)
    {
        Name = cell.Name;
        Position = cell.Position;
        Bounds = cell.Bounds;
        TileNumber = cell.TileNumber;
        TileSize = import.TileSize;
        if (import.Tags != null) Tags = import.Tags;
        Properties = import.Properties.Concat(cell.Options.Select(o => new ThingProperty() { Name = o.Name, Value = o.Value })).ToList();
        
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
