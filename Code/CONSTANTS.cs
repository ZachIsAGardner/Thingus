using Newtonsoft.Json;

namespace Thingus;

public class CONSTANTS
{
    class ConstantsContents
    {
        public string NAMESPACE;
        public string START_ROOM;
        public int? VIRTUAL_WIDTH;
        public int? VIRTUAL_HEIGHT;
        public int? DEFAULT_SCREEN_MULTIPLIER;
        public int? TILE_SIZE;
    }

    public static readonly string NAMESPACE = "";
    public static readonly string START_ROOM = "";
    public static readonly int VIRTUAL_WIDTH = 320;
    public static readonly int VIRTUAL_HEIGHT = 180;
    public static readonly int DEFAULT_SCREEN_MULTIPLIER = 3;
    public static readonly int TILE_SIZE = 16;

    static CONSTANTS()
    {
        string contents = File.ReadAllText("CONSTANTS.json");
        ConstantsContents c = JsonConvert.DeserializeObject<ConstantsContents>(contents);

        NAMESPACE = c.NAMESPACE;
        START_ROOM = c.START_ROOM;
        if (c.VIRTUAL_WIDTH.HasValue) VIRTUAL_WIDTH = c.VIRTUAL_WIDTH.Value;
        if (c.VIRTUAL_HEIGHT.HasValue) VIRTUAL_HEIGHT = c.VIRTUAL_HEIGHT.Value;
        if (c.DEFAULT_SCREEN_MULTIPLIER.HasValue) DEFAULT_SCREEN_MULTIPLIER = c.DEFAULT_SCREEN_MULTIPLIER.Value;
        if (c.TILE_SIZE.HasValue) TILE_SIZE = c.TILE_SIZE.Value;
    }
}
