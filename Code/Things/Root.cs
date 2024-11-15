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

    public Room Zone;
    public Room Room;

    public Root()
    {
        DrawMode = DrawMode.Absolute;

        Dynamic = AddChild(new Thing());
        Dynamic.Name = "Dynamic";

        Shared = AddChild(new Thing());
        Shared.Name = "Shared";

        SongManager = AddChild(new SongManager()) as SongManager;
        DeveloperTools = AddChild(new DeveloperTools()) as DeveloperTools;
    }

    public override void Start()
    {
        base.Start();
        if (CONSTANTS.START_ROOM != null) Load(CONSTANTS.START_ROOM);
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
        if (DeveloperTools.Editor?.Active == true) DeveloperTools.Editor.SetActive(false);
        Dynamic.Children.ToList().ForEach(c => c.Destroy());
        Dynamic.Children = new List<Thing>() { };
        Room = Library.Maps[name].Load(Dynamic);
        Game.Mode = GameMode.Play;
        if (Room.Type == "Zone")
        {
            Zone = Room;
            Room = Zone.GetThings<Room>().FirstOrDefault(r => r.Type != "Zone");
        }
    }
}
