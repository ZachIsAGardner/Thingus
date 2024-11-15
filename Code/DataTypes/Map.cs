using System.Numerics;
using System.Reflection;
using Newtonsoft.Json;

namespace Thingus;

public class Map
{
    public const string PATH = "Content/Maps";
    
    public string Name;
    public string Type = "Room";
    public List<MapCell> Cells = new List<MapCell>() { };

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
        Cells = Cells.Where(c => !(c.Position == cell.Position && c.Import.Layer == cell.Import.Layer)).ToList();
        Cells.Add(cell);
        cell.Map = this;
    }

    public MapCell GetCell(Vector2 position, int? layer = null)
    {
        return layer != null
            ? Cells.Find(c => c.Position == position && c.Import.Layer == layer)
            : Cells.Where(c => c.Position == position).OrderByDescending(c => c.Import.Layer).FirstOrDefault();
    }

    public void Save()
    {
        string content = JsonConvert.SerializeObject(this);
        File.WriteAllText($"{PATH}/{Name}.map", content);
    }

    public Room Load(Thing root, Vector2? position = null, MapCell mapCell = null)
    {
        // Code Behind
        MethodInfo method = Utility.FindCreateMethod(Name);
        Room room = null;
        if (method != null)
        {
            room = method.Invoke(null, new object?[] { root, new ThingModel(Name, position ?? Vector2.Zero) }) as Room;
        }
        else
        {
            room = new Room(Name, position ?? Vector2.Zero);
        }

        root.AddChild(room);
        root = room;
        room.Map = this;
        room.Cell = mapCell;
        room.Type = Type;

        // Cells
        foreach (MapCell cell in Cells)
        {
            Thing thing = null;
            if (cell.Name.StartsWith("="))
            {
                thing = Library.Maps[cell.Name.Replace("=", "")].Load(root, cell.Position, cell);
            }
            else
            {
                thing = cell.Create(root);
            }
        }

        return room;
    }
}
