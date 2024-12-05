using System.Reflection;
using Raylib_cs;

namespace Thingus;

public class ThingImport
{
    public string Name;
    public string Thing;
    public int? TileSize = null;
    public int Layer = 0;
    public BlendMode? BlendMode = null;
    public List<string> Tags = new List<string>() { };
    public Dictionary<string, string> Properties = new Dictionary<string, string>() { };
    public List<ThingOption> Options = new List<ThingOption>() { };
    public string StampType = "Single";
    public List<string> AutoKeys = new List<string>() { };
    public string Category;
    public int Token;
    public bool VisibleOnlyInEditor;

    public ThingImport() { }

    public Thing Create(Thing root, MapCell cell)
    {
        ThingModel model = new ThingModel(this, cell);
        MethodInfo method = Utility.FindCreateMethod(Thing);
        return root.AddChild(method.Invoke(null, new object[] { root, model }) as Thing);
    }
}
