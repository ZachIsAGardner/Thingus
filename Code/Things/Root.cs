using System.Numerics;
using Newtonsoft.Json;
using Raylib_cs;

namespace Thingus;

public class Root : Thing
{
    public Thing Dynamic;
    public Thing Shared;
    public SongManager SongManager;
    public SoundEffectManager SoundEffectManager;
    public DeveloperTools DeveloperTools;

    public Room Room;

    public Root()
    {
        DrawMode = DrawMode.Absolute;
    }

    public override void Start()
    {
        base.Start();
        
        Dynamic = AddChild(new Thing());
        Dynamic.Name = "Dynamic";

        Shared = AddChild(new Thing());
        Shared.Name = "Shared";

        SongManager = AddChild(new SongManager()) as SongManager;
        SoundEffectManager = AddChild(new SoundEffectManager()) as SoundEffectManager;
        if (CONSTANTS.IS_DEBUG) DeveloperTools = AddChild(new DeveloperTools()) as DeveloperTools;
        if (CONSTANTS.START_MAP != null) Load(CONSTANTS.START_MAP);
    }

    public override void Update()
    {
        base.Update();
    }

    public void Load(string name)
    {
        TokenGrid.Reset();
        Map map = Library.Maps.Get(name);
        Load(map);
    }

    public void Load(Map map)
    {
        if (map == null) return;
        
        if (DeveloperTools?.Editor != null)
        {
            DeveloperTools.Editor.Reset();
        }

        Dynamic.Clear();
        SoundEffectManager.Tracks.Clear();
        Thing thing = map.Load(Dynamic);
        if (DeveloperStorage.Map == null)
        {
            DeveloperStorage.Map = map?.Name;
            DeveloperStorage.Save();
        }
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
