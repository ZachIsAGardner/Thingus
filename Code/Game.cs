using System.Numerics;
using System.Runtime.InteropServices;
using Raylib_cs;

namespace Thingus;

public enum GameMode
{
    Play,
    Edit
}

public enum ProjectionType
{
    Grid,
    Isometric,
    Oblique
}

public static class Game
{
    public static Root Root;
    public static List<Thing> Things = new List<Thing>() { };
    public static string CharacterType = null;

    static bool queueDrawReorder;
    public static List<Thing> DrawThings = new List<Thing>() { };
    static bool queueUpdateReorder;
    public static List<Thing> UpdateThings = new List<Thing>() { };

    public static GameMode Mode = GameMode.Play;

    public static Dictionary<string, List<Thing>> TypeThings = new Dictionary<string, List<Thing>>(StringComparer.OrdinalIgnoreCase);

    static bool queueClear = false;

    public static bool FrameAdvance = false;
    public static bool FrameStep = false;

    public static bool Mute = true;

    public static Map LastMap = null;

    public static PlatformType Platform;

    public static List<T> GetThings<T>() where T : Thing
    {
        TypeThings.TryGetValue(typeof(T).Name, out List<Thing> entry);
        if (entry != null)
        {
            return entry.Select(c => c as T).ToList();
        }
        else
        {
            return new List<T>() { };
        }
    }

    public static T GetThing<T>() where T : Thing => GetThings<T>().FirstOrDefault();

    public static void Start()
    {
        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
        Raylib.SetConfigFlags(ConfigFlags.VSyncHint);
        Raylib.InitWindow(CONSTANTS.VIRTUAL_WIDTH * CONSTANTS.DEFAULT_SCREEN_MULTIPLIER, CONSTANTS.VIRTUAL_HEIGHT * CONSTANTS.DEFAULT_SCREEN_MULTIPLIER, CONSTANTS.TITLE);
        Raylib.SetWindowIcon(Raylib.LoadImage("Icon.png"));
        Raylib.SetTargetFPS(60);
        Raylib.InitAudioDevice();

        Library.Refresh();
        Viewport.Start();
        Lang.Start();

        // Get platform
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) Platform = PlatformType.Windows;
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) Platform = PlatformType.Mac;
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) Platform = PlatformType.Linux;

        Root = new Root();
    }

    public static void QueueClear()
    {
        queueClear = true;
    }

    public static void Clear()
    {
        Things.Clear();
        UpdateThings.Clear();
        DrawThings.Clear();
        Root = new Root();
    }

    public static void Process()
    {
        Start();

        Raylib.SetExitKey(Raylib_cs.KeyboardKey.Null);
        while (!Raylib.WindowShouldClose())
        {
            Update();
            Draw();
        }

        Raylib.CloseWindow();
    }

    public static void Update()
    {
        if (CONSTANTS.IS_DEBUG)
        {
            if (Input.IsPressed(KeyboardKey.F1)) FrameAdvance = !FrameAdvance;
            if (Input.IsPressed(KeyboardKey.F2)) FrameStep = true;

            if (FrameAdvance)
            {
                if (FrameStep) FrameStep = false;
                else return;
            }
        }

        Time.Update();
        Input.Update();

        UpdateThings.ForEach(t =>
        {
            if (!t.DidStart) t.Start();
            t.Update();
        });

        UpdateThings.ForEach(t =>
        {
            t.LateUpdate();
        });

        Viewport.Update();

        if (queueClear)
        {
            queueClear = false;
            Clear();
        }

        if (queueDrawReorder)
        {
            queueDrawReorder = false;
            DrawThings = Things.Where(t => t.GlobalVisible).OrderBy(t => t.DrawOrder + t.DrawOrderOffset).ToList();
        }

        if (queueUpdateReorder)
        {
            queueUpdateReorder = false;
            UpdateThings = Things.Where(t => t.GlobalActive).OrderBy(t => t.UpdateOrder + t.UpdateOrderOffset).ToList();
        }
    }

    public static void Draw()
    {
        Raylib.BeginDrawing();
        Viewport.Draw();
        Raylib.EndDrawing();
    }

    public static void QueueReorder()
    {
        queueDrawReorder = true;
        queueUpdateReorder = true;
    }

    public static void QueueDrawReorder()
    {
        queueDrawReorder = true;
    }

    public static void QueueUpdateReorder()
    {
        queueUpdateReorder = true;
    }

    public static void PlaySound(string name, float volume = 1f, float pitch = 1f, float pan = 0.5f)
    {
        if (Mute) return;
        Sound sound = Library.SoundEffects[name];
        Raylib.SetSoundVolume(sound, volume);
        Raylib.SetSoundPitch(sound, pitch);
        Raylib.SetSoundPan(sound, pan);
        Raylib.PlaySound(sound);
    }

    public static void PlaySong(string name)
    {
        Root.SongManager.Play(name);
    }

    public static void StopSong()
    {
        Root.SongManager.Stop();
    }
}
