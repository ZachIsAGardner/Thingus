using Raylib_cs;

namespace Thingus;

public class SongManager : Thing
{
    public List<(string Name, Music Music)> Tracks = new List<(string Name, Music Music)>() { };
    public static float Pitch = 1f;

    public void Play(string name)
    {
        if (Tracks.Any(t => t.Name == name)) return;

        Tracks.ForEach(t =>
        {
            Raylib.SeekMusicStream(t.Music, 0);
        });
        Tracks.Clear();
        if (name?.HasValue() != true) return;
        if (Library.Songs.ContainsKey(name))
        {
            Music song = Library.Songs[name];
            Raylib.PlayMusicStream(song);
            Tracks.Add((name, song));
        }
    }

    public void Stop()
    {
        Tracks.Clear();
    }

    public override void Update()
    {
        base.Update();

        if (Game.Mute) return;

        Tracks.ForEach(t =>
        {
            Raylib.SetMusicVolume(t.Music, Game.MusicVolume);
            Raylib.SetMusicPitch(t.Music, Pitch);
            Raylib.UpdateMusicStream(t.Music);
        });
    }
}
