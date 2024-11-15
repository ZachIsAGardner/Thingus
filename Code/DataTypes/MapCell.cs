using System.Numerics;
using Newtonsoft.Json;

namespace Thingus;

public class MapCell
{
    public string Name;
    public Vector2 Position;
    public int? TileNumber;

    public MapCell() { }
    public MapCell(string name, Vector2 position, int? tileNumber = null)
    {
        Name = name;
        Position = position;
    }

    [JsonIgnore]
    public ThingImport Import => import ?? (import = Library.ThingImports[Name]);
    [JsonIgnore]
    ThingImport import = null;

    [JsonIgnore]
    public Map Map = null;

    public Thing Create(Thing root)
    {
        Thing thing = Import.Create(root, this);
        thing.Map = Map;
        thing.Cell = this;
        if (thing.Name == null) thing.Name = Import.Name;
        thing.AfterCreatedFromMap();
        return thing;
    }
}
