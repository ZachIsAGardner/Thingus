using System.Numerics;
using Newtonsoft.Json;
using Raylib_cs;

namespace Thingus;

public class Root : Thing
{
    public Thing Dynamic;
    public Thing Shared;
    public SongManager SongManager;
    public DeveloperTools DeveloperTools;

    public Room Room;

    public Root()
    {
        DrawMode = DrawMode.Absolute;
    }

    public override void Init()
    {
        base.Init();
    }

    public override void Start()
    {
        base.Start();
        
        Dynamic = AddChild(new Thing());
        Dynamic.Name = "Dynamic";

        Shared = AddChild(new Thing());
        Shared.Name = "Shared";

        SongManager = AddChild(new SongManager()) as SongManager;
        if (CONSTANTS.IS_DEBUG) DeveloperTools = AddChild(new DeveloperTools()) as DeveloperTools;
        if (CONSTANTS.START_MAP != null) Load(CONSTANTS.START_MAP);
    }

    public override void Update()
    {
        base.Update();

        Shortcuts();
    }

    public void Shortcuts()
    {
        if ((Input.CtrlIsHeld && Input.IsPressed(KeyboardKey.F)) || (Input.AltIsHeld && Input.IsPressed(KeyboardKey.Enter)))
        {
            Raylib.ToggleBorderlessWindowed();
            Viewport.RefreshProjection();
        }
    }

    public void Load(string name)
    {
        Map map = Library.Maps[name];
        Load(map);
    }

    public void Load(Map map)
    {
        if (DeveloperTools?.Editor != null)
        {
            DeveloperTools.Editor.Reset();
        }

        Dynamic.Clear();
        Thing thing = map.Load(Dynamic);
        Game.LastMap = map;
        Game.Mode = GameMode.Play;

        Room = Game.GetThing<Room>();
        
        // Room = Library.Maps[name].Load(Dynamic);
        // if (Room.Type == "Zone")
        // {
        //     Zone = Room;
        //     Room = Zone.GetThings<Room>().FirstOrDefault(r => r.Type != "Zone");
        // }
    }
}
