using System.Numerics;
using System.Reflection;
using Newtonsoft.Json;

namespace Thingus;

public class Map
{
    public const string PATH = "Content/Maps";
    
    [JsonIgnore]
    public string Name;
    public List<MapCell> Cells = new List<MapCell>() { };

    [JsonIgnore]
    public string Path;

    public Map() { }
    public Map(string name)
    {
        Name = name;
    }

    public void RemoveCell(Vector2 position)
    {
        Cells = Cells.Where(c => c.Position != position).ToList();
    }

    public void RemoveCell(MapCell cell)
    {
        Cells.Remove(cell);
    }

    public void AddCell(MapCell cell)
    {
        Cells = Cells.Where(c => !(c.Position == cell.Position && (c.Import != null && cell.Import != null && c.Import.Layer == cell.Import.Layer))).ToList();
        Cells.Add(cell);
        cell.Map = this;
    }

    public MapCell GetCell(Vector2 position, int? layer = null)
    {
        return layer != null
            ? Cells.Find(c => c.Position == position && c.Import.Layer == layer)
            : Cells.Where(c => c.Position == position).OrderByDescending(c => c.Import?.Layer).FirstOrDefault();
    }

    public MapCell GetCell(Thing thing)
    {
        return Cells.Where(c => c.Position == thing.Position).OrderByDescending(c => c.Import?.Layer).FirstOrDefault();
    }

    public void Save()
    {
        string content = JsonConvert.SerializeObject(this);
        Directory.CreateDirectory(Path);
        File.WriteAllText($"{Path}/{Name}.map", content);
    }

    public void Delete()
    {
        File.Delete($"{Path}/{Name}.map");
    }

    public Thing Load(Thing root, MapCell mapCell = null)
    {
        Thing mapRoot = null;   
        // Cells
        foreach (MapCell cell in Cells.Where(c => c?.Name != null))
        {
            Thing parent = root;
            if (cell.Parent != null) parent = root.Children.Find(c => c.Name == cell.Parent);
            Thing thing = cell.Create(parent);
            if (cell.Parent == null)
            {
                mapRoot = thing;
                if (mapCell != null) mapRoot.Position += mapCell.Position;
            }
        }

        return mapRoot;
    }
}
