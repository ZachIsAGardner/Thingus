using Newtonsoft.Json;
using Raylib_cs;

namespace Thingus;

public static class Library
{
    public static Font Font;
    public static Font FontSmall;

    public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>() { };
    public static Dictionary<string, Font> Fonts = new Dictionary<string, Font>() { };
    public static Dictionary<string, Music> Songs = new Dictionary<string, Music>() { };
    public static Dictionary<string, Sound> SoundEffects = new Dictionary<string, Sound>() { };
    public static Dictionary<string, Shader> Shaders = new Dictionary<string, Shader>() { };
    public static Dictionary<string, Map> Maps = new Dictionary<string, Map>() { };
    public static Dictionary<string, ThingImport> ThingImports = new Dictionary<string, ThingImport>() { };

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

        Import("_Thingus/Content");
        Import("Content");
        Font = Fonts["MonogramExtended"];
        FontSmall = Fonts["Pico8"];
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
            }
            else if ((file.EndsWith(".ogg") || file.EndsWith(".wav")) && file.Contains("/Songs"))
            {
                Songs[name] = Raylib.LoadMusicStream(file);
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
        }
    }
}
