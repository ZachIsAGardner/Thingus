using System.Numerics;
using System.Reflection;
using Newtonsoft.Json;

namespace Thingus;

public class MapCell
{
    public string Name;
    public Vector2 Position;
    public string Parent;

    public int? TileNumber;
    public Vector2 Bounds;

    public MapCell() { }
    public MapCell(string name, Vector2 position, int? tileNumber = null, Vector2? bounds = null, string parent = null)
    {
        Name = name;
        Position = position;
        Parent = parent;
        TileNumber = tileNumber;
        if (bounds != null) Bounds = bounds.Value;
    }

    [JsonIgnore]
    public ThingImport Import => import ?? (import = Library.ThingImports.Get(Name));
    [JsonIgnore]
    ThingImport import = null;

    [JsonIgnore]
    public Map Map = null;

    public Thing Create(Thing root)
    {
        Thing thing = null;
        // Sub-Map
        if (Name.StartsWith("="))
        {
            Map map = Library.Maps[Name.Remove(0, 1)];
            thing = map.Load(root, this);
            thing.Map = map;
            thing.Cell = map.GetCell(thing);
        }
        // Import
        else if (Import != null)
        {
            thing = Import.Create(root, this);
            thing.Map = Map;
        }
        // No Import
        else
        {
            ThingModel model = new ThingModel(this);
            MethodInfo method = Utility.FindCreateMethod(Name);
            thing = root.AddChild(method.Invoke(null, new object[] { root, model }) as Thing);
            thing.Map = Map;
            thing.Cell = this;
        }
        if (thing.Name == null) thing.Name = Name;
        thing.AfterCreatedFromMap();
        return thing;
    }
}
