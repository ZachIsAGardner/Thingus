using Newtonsoft.Json;

namespace Thingus;

public class CONSTANTS
{
    class ConstantsContents
    {
        public string TITLE;
        public string NAMESPACE;
        public string START_MAP;
        public int? VIRTUAL_WIDTH;
        public int? VIRTUAL_HEIGHT;
        public int? DEFAULT_SCREEN_MULTIPLIER;
        public int? TILE_SIZE;
        public string? PRIMARY_COLOR;
        public string? PROJECTION_TYPE;
        public string? SAVE_FOLDER;
    }

    public static readonly string TITLE = "Thingus Game";
    public static readonly string NAMESPACE = null;
    public static readonly string START_MAP = "Init";
    public static readonly int VIRTUAL_WIDTH = 320;
    public static readonly int VIRTUAL_HEIGHT = 180;
    public static readonly int DEFAULT_SCREEN_MULTIPLIER = 3;
    public static readonly int TILE_SIZE = 16;
    public static int TILE_SIZE_HALF => TILE_SIZE / 2;
    public static int TILE_SIZE_THIRD => TILE_SIZE / 3;
    public static int TILE_SIZE_QUARTER => TILE_SIZE / 4;
    public static int TILE_SIZE_OBLIQUE => TILE_SIZE - TILE_SIZE_THIRD;
    public static readonly Color PRIMARY_COLOR = PaletteBasic.Gray;
    public static ProjectionType PROJECTION_TYPE = ProjectionType.Grid;
    public static string SAVE_FOLDER = "thingus";

    static CONSTANTS()
    {
        string contents = File.ReadAllText("CONSTANTS.json");
        ConstantsContents c = JsonConvert.DeserializeObject<ConstantsContents>(contents);

        if (c.TITLE != null) TITLE = c.TITLE;
        if (c.NAMESPACE != null) NAMESPACE = c.NAMESPACE;
        if (c.START_MAP != null) START_MAP = c.START_MAP;
        if (c.VIRTUAL_WIDTH != null) VIRTUAL_WIDTH = c.VIRTUAL_WIDTH.Value;
        if (c.VIRTUAL_HEIGHT != null) VIRTUAL_HEIGHT = c.VIRTUAL_HEIGHT.Value;
        if (c.DEFAULT_SCREEN_MULTIPLIER != null) DEFAULT_SCREEN_MULTIPLIER = c.DEFAULT_SCREEN_MULTIPLIER.Value;
        if (c.TILE_SIZE != null) TILE_SIZE = c.TILE_SIZE.Value;
        if (c.PRIMARY_COLOR != null) PRIMARY_COLOR = Color.HexToColor(c.PRIMARY_COLOR);
        if (c.PROJECTION_TYPE != null)
        {
            if (c.PROJECTION_TYPE.ToLower() == "grid") PROJECTION_TYPE = ProjectionType.Grid;
            if (c.PROJECTION_TYPE.ToLower() == "oblique") PROJECTION_TYPE = ProjectionType.Oblique;
            if (c.PROJECTION_TYPE.ToLower() == "isometric") PROJECTION_TYPE = ProjectionType.Isometric;
        }
        if (c.SAVE_FOLDER != null) SAVE_FOLDER = c.SAVE_FOLDER;
    }
}
