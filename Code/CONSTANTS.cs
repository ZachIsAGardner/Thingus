using Newtonsoft.Json;

namespace Thingus;

public class CONSTANTS
{
    class ConstantsContents
    {
        public string TITLE;
        public string NAMESPACE;
        public string START_ROOM;
        public int? VIRTUAL_WIDTH;
        public int? VIRTUAL_HEIGHT;
        public int? DEFAULT_SCREEN_MULTIPLIER;
        public int? TILE_SIZE;
        public string? PRIMARY_COLOR;
    }

    public static readonly string TITLE = "Thingus Game";
    public static readonly string NAMESPACE = null;
    public static readonly string START_ROOM = "Init";
    public static readonly int VIRTUAL_WIDTH = 320;
    public static readonly int VIRTUAL_HEIGHT = 180;
    public static readonly int DEFAULT_SCREEN_MULTIPLIER = 3;
    public static readonly int TILE_SIZE = 16;
    public static readonly Color PRIMARY_COLOR = Colors.Gray3;

    static CONSTANTS()
    {
        string contents = File.ReadAllText("CONSTANTS.json");
        ConstantsContents c = JsonConvert.DeserializeObject<ConstantsContents>(contents);

        if (c.TITLE != null) TITLE = c.TITLE;
        if (c.NAMESPACE != null) NAMESPACE = c.NAMESPACE;
        if (c.START_ROOM != null) START_ROOM = c.START_ROOM;
        if (c.VIRTUAL_WIDTH != null) VIRTUAL_WIDTH = c.VIRTUAL_WIDTH.Value;
        if (c.VIRTUAL_HEIGHT != null) VIRTUAL_HEIGHT = c.VIRTUAL_HEIGHT.Value;
        if (c.DEFAULT_SCREEN_MULTIPLIER != null) DEFAULT_SCREEN_MULTIPLIER = c.DEFAULT_SCREEN_MULTIPLIER.Value;
        if (c.TILE_SIZE != null) TILE_SIZE = c.TILE_SIZE.Value;
        if (c.PRIMARY_COLOR != null) PRIMARY_COLOR = Utility.HexToColor(c.PRIMARY_COLOR);
    }
}
