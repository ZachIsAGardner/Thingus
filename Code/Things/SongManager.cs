using Raylib_cs;

namespace Thingus;

public class SongManager : Thing
{
    public List<Music> Tracks = new List<Music>() { };
    public static float Pitch = 1f;

    public void Play(string name)
    {
        Tracks.ForEach(t =>
        {
            Raylib.SeekMusicStream(t, 0);
        });
        Tracks.Clear();
        if (Library.Songs.ContainsKey(name))
        {
            Music song = Library.Songs[name];
            Raylib.PlayMusicStream(song);
            Tracks.Add(song);
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
            Raylib.SetMusicPitch(t, Pitch);
            Raylib.UpdateMusicStream(t);
        });
    }
}
