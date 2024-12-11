using System.Numerics;
using System.Reflection;
using Newtonsoft.Json;

namespace Thingus;

public class MapCell
{
    public string Name;
    public string Thing;
    public Vector2 Position;
    public string Parent;

    public int? TileNumber;
    public Vector2 Bounds;

    public List<ThingProperty> Options = new List<ThingProperty>() { };

    public MapCell() { }
    public MapCell(string name, string thing = null, Vector2? position = null, int? tileNumber = null, Vector2? bounds = null, string parent = null)
    {
        Name = name;
        Thing = thing;
        Position = position ?? Vector2.Zero;
        Parent = parent;
        TileNumber = tileNumber;
        if (bounds != null) Bounds = bounds.Value;
    }

    [JsonIgnore]
    public ThingImport Import 
    { 
        get
        {
            if (import != null) return import;
            ThingImport thingImport = Library.ThingImports.Get(Thing);
            ThingImport nameImport = Library.ThingImports.Get(Name);
            if (thingImport == null && nameImport != null) return nameImport;
            if (thingImport != null && nameImport == null) return thingImport;
            if (thingImport != null && nameImport != null)
            {
                Type thingType = Utility.FindType(thingImport.Name);
                Type nameType = Utility.FindType(nameImport.Name);
                if (thingType.IsAssignableFrom(nameType)) return nameImport;
                if (nameType.IsAssignableFrom(thingType)) return thingImport;
            }
            return null;
        }
    }
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
            thing.ParentMap = Map;
            thing.ParentCell = this;
            thing.Map = map;
            thing.Cell = map.Cells[0];
        }
        // Import
        else if (Import != null)
        {
            thing = Import.Create(root, this);
            thing.Map = Map;
            thing.Cell = this;
            thing.ApplyImport(Import);
        }
        // No Import
        else
        {
            ThingModel model = new ThingModel(this);
            model.Root = root;

            string thingTypeName = Thing;
            string nameTypeName = Name;
            
            MethodInfo thingMethod = Utility.FindCreateMethod(Thing);
            if (thingMethod == null && Thing?.Contains("_") == true)
            {
                thingMethod = Utility.FindCreateMethod(Thing.Split("_").First());
                thingTypeName = Thing.Split("_").First();
            }
            MethodInfo nameMethod = Utility.FindCreateMethod(Name);
            if (nameMethod == null && Name?.Contains("_") == true)
            {
                nameMethod = Utility.FindCreateMethod(Name.Split("_").First());
                nameTypeName = Name.Split("_").First();
            }
            MethodInfo method = null;
            if (thingMethod == null && nameMethod != null) method = nameMethod;
            if (thingMethod != null && nameMethod == null) method = thingMethod;
            if (thingMethod != null && nameMethod != null)
            {
                Type thingType = Utility.FindType(thingTypeName);
                Type nameType = Utility.FindType(nameTypeName);
                if (thingType.IsAssignableFrom(nameType)) method = nameMethod;
                if (nameType.IsAssignableFrom(thingType)) method = thingMethod;
            }

            thing = root.AddChild(method.Invoke(null, new object[] { model }) as Thing);
            thing.Map = Map;
            thing.Cell = this;
        }
        if (thing.Name == null) thing.Name = Name;
        thing.AfterCreatedFromMap();
        return thing;
    }
}
