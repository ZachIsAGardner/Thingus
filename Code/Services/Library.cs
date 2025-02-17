using Newtonsoft.Json;
using Raylib_cs;

namespace Thingus;

public static class Library
{
    public static Font Font;
    public static Font FontSmall;

    public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>(StringComparer.OrdinalIgnoreCase) { };
    public static Dictionary<string, Font> Fonts = new Dictionary<string, Font>(StringComparer.OrdinalIgnoreCase) { };
    public static Dictionary<string, Music> Songs = new Dictionary<string, Music>(StringComparer.OrdinalIgnoreCase) { };
    public static Dictionary<string, Sound> SoundEffects = new Dictionary<string, Sound>(StringComparer.OrdinalIgnoreCase) { };
    public static Dictionary<string, Shader> Shaders = new Dictionary<string, Shader>(StringComparer.OrdinalIgnoreCase) { };
    public static Dictionary<string, Map> Maps = new Dictionary<string, Map>(StringComparer.OrdinalIgnoreCase) { };
    public static Dictionary<string, ThingImport> ThingImports = new Dictionary<string, ThingImport>(StringComparer.OrdinalIgnoreCase) { };
    public static Dictionary<string, LangImport> LangImports = new Dictionary<string, LangImport>(StringComparer.OrdinalIgnoreCase) { };

    static List<string> ParseDirectory(string path, List<string> result = null)
    {
        result = result ?? new List<string>();

        if (!Directory.Exists(path)) return result;

        string[] paths = Directory.GetFiles(path);
        string[] directories = Directory.GetDirectories(path);

        result.AddRange(paths);

        foreach (string directory in directories)
        {
            ParseDirectory(directory, result);
        }

        return result;
    }

    public static void Refresh()
    {
        Textures.Clear();
        Fonts.Clear();
        SoundEffects.Clear();
        Shaders.Clear();
        Songs.Clear();
        Maps.Clear();
        ThingImports.Clear();
        LangImports.Clear();

        Import("_Thingus/Content");
        Import("Content");
        Font = Fonts["MonogramExtended"];
        // +1
        Font.Width = 6;
        Font.Height = 7;
        FontSmall = Fonts["Pico8"];
        // +1
        FontSmall.Width = 4;
        FontSmall.Height = 5;
    }

    static void Import(string path)
    {
        List<string> files = ParseDirectory(path)
            .Select(f => f.Replace("\\", "/"))
            .ToList();

        foreach (string file in files)
        {
            string name = file.Split("/").Last().Split(".").First();

            if (file.EndsWith(".png"))
            {
                Textures[name] = (Raylib.LoadTexture(file));
            }
            else if (file.EndsWith(".imp"))
            {
                string contents = File.ReadAllText(file);
                ThingImports[name] = JsonConvert.DeserializeObject<ThingImport>(contents);
                ThingImports[name].Name = name;
                if (ThingImports[name].Layer != null && !Game.Layers.ContainsKey(ThingImports[name].Layer)) Game.Layers[ThingImports[name].Layer] = 0;
                Editor.Categories.Add(ThingImports[name].Category);
            }
            else if ((file.EndsWith(".ogg") || file.EndsWith(".wav")) && file.Contains("/Songs"))
            {
                Music song = Raylib.LoadMusicStream(file);
                song.Looping = false;
                string loopFile = files.Find(f => f.EndsWith($"{name}.loop"));
                if (loopFile != null)
                {
                    string[] loop = File.ReadAllLines(loopFile);
                    if (loop.Count() >= 1) song.LoopStart = loop[0].ToFloat();
                    if (loop.Count() >= 2) song.LoopEnd = loop[1].ToFloat();
                    else song.LoopEnd = Raylib.GetMusicTimeLength(song);
                }
                else
                {
                    song.LoopEnd = Raylib.GetMusicTimeLength(song);
                }
                Songs[name] = song;
            }
            else if ((file.EndsWith(".ogg") || file.EndsWith(".wav")) && file.Contains("/SoundEffects"))
            {
                SoundEffects[name] = Raylib.LoadSound(file);
            }
            else if (file.EndsWith(".map"))
            {
                string contents = File.ReadAllText(file);
                Maps[name] = JsonConvert.DeserializeObject<Map>(contents);
                Maps[name].Name = name;
                Maps[name].Path = file.Split("/").Reverse().Skip(1).Reverse().Join("/");
                Maps[name].Cells.ForEach(c => c.Map = Maps[name]);
            }
            else if (file.EndsWith(".fs"))
            {
                Shaders[name] = new Shader(Raylib.LoadShader(null, file));
            }
            else if (file.EndsWith(".fnt"))
            {
                Fonts[name] = new Font(Raylib.LoadFont(file));
                Raylib.SetTextureFilter(Fonts[name].Texture, TextureFilter.Point);
            }
            else if (file.EndsWith(".lang"))
            {
                LangImports[name] = new LangImport(file);
            }
        }
    }
}
