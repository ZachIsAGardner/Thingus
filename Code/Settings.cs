using Newtonsoft.Json;

namespace Thingus;

public class Settings
{
    class SettingsContents
    {
        public bool Mute = false;
        public float MusicVolume = 1f;
        public float SoundEffectsVolume = 1f;
        public bool IsFullscreen = true;
        public int Fps = 60;
        public float ScreenShake = 1f;
    }

    public static readonly string Path = $"{Paths.UserStorageBasePath()}/Settings.json";

    public static bool Mute = false;
    public static float MusicVolume = 1f;
    public static float SoundEffectsVolume = 1f;
    public static bool IsFullscreen = true;
    public static int Fps = 60;
    public static float ScreenShake = 1f;

    static Settings()
    {
        if (!File.Exists(Path)) return;
        
        string contents = File.ReadAllText(Path);
        SettingsContents c = JsonConvert.DeserializeObject<SettingsContents>(contents);

        if (c.Mute != null) Mute = c.Mute;
        if (c.MusicVolume != null) MusicVolume = c.MusicVolume;
        if (c.SoundEffectsVolume != null) SoundEffectsVolume = c.SoundEffectsVolume;
        if (c.IsFullscreen != null) IsFullscreen = c.IsFullscreen;
        if (c.Fps != null) Fps = c.Fps;
        if (c.ScreenShake != null) ScreenShake = c.ScreenShake;
    }

    public static void Save()
    {
        SettingsContents c = new SettingsContents()
        {
            Mute = Mute,
            MusicVolume = MusicVolume,
            SoundEffectsVolume = SoundEffectsVolume,
            IsFullscreen = IsFullscreen,
            Fps = Fps,
            ScreenShake = ScreenShake,
        };

        File.WriteAllText(Path, JsonConvert.SerializeObject(c));
    }
}
