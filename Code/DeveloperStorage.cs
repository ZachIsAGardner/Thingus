using Newtonsoft.Json;

namespace Thingus;

public static class DeveloperStorage
{
    class DeveloperStorageContents
    {
        public string Map;
        public string Room;
    }

    public static readonly string Path = $"{Paths.UserStorageBasePath()}/DeveloperStorage.json";

    public static string Map;
    public static string Room;

    static DeveloperStorage()
    {
        if (!File.Exists(Path)) return;
        
        string contents = File.ReadAllText(Path);
        DeveloperStorageContents c = JsonConvert.DeserializeObject<DeveloperStorageContents>(contents);

        if (c.Map != null) Map = c.Map;
        if (c.Room != null) Room = c.Room;
    }

    public static void Save()
    {
        DeveloperStorageContents c = new DeveloperStorageContents()
        {
            Map = Map,
            Room = Room
        };

        File.WriteAllText(Path, JsonConvert.SerializeObject(c));
    }
}
