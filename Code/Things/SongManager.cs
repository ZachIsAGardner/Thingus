using Raylib_cs;

namespace Thingus;

public enum SongTrackState
{
    Normal,
    FadeIn,
    FadeOut
}

public class SongTrack
{
    public string Name;
    public Music Song;
    public float Volume;

    public SongTrackState State;

    public SongTrack(string name, Music song, float volume = 1f)
    {
        Name = name;
        Song = song;
        Volume = volume;

        Song.Looping = false;
    }

    public void Update()
    {
        if (State == SongTrackState.Normal)
        {

        }
        else if (State == SongTrackState.FadeIn)
        {
            Volume = Volume.MoveOverTime(1, 0.1f);
            if (Volume == 1)
            {
                State = SongTrackState.Normal;
            }
        }
        else if (State == SongTrackState.FadeOut)
        {
            Volume = Volume.MoveOverTime(0, 0.1f);
            if (Volume == 0)
            {
                State = SongTrackState.Normal;
            }
        }
    }

    public void FadeIn()
    {
        State = SongTrackState.FadeIn;
    }

    public void FadeOut()
    {
        State = SongTrackState.FadeOut;
    }
}

public class SongManager : Thing
{
    public List<SongTrack> Tracks = new List<SongTrack>() { };
    public static float Pitch = 1f;

    public void Play(string name)
    {
        if (Tracks.Any(t => t.Name == name)) return;

        Tracks.ForEach(t =>
        {
            Raylib.SeekMusicStream(t.Song, 0);
        });
        Tracks.Clear();
        if (name?.HasValue() != true) return;
        if (Library.Songs.ContainsKey(name))
        {
            Music song = Library.Songs[name];
            Raylib.PlayMusicStream(song);
            Tracks.Add(new SongTrack(name, song));
        }
    }

    public void Play(List<string> names)
    {
        if (Tracks.Any(t => names.Contains(t.Name))) return;

        Tracks.ForEach(t =>
        {
            Raylib.SeekMusicStream(t.Song, 0);
        });
        Tracks.Clear();
        if (names == null || names.Count <= 0) return;

        int i = 0;
        names.ForEach(name =>
        {
            if (Library.Songs.ContainsKey(name))
            {
                Music song = Library.Songs[name];
                Raylib.PlayMusicStream(song);
                Tracks.Add(new SongTrack(name, song, i == 0 ? 1f : 0f));
            }
            i++;
        });
    }

    public void Switch(string name)
    {
        Tracks.ForEach(t =>
        {
            if (t.Name == name) t.FadeIn();
            else t.FadeOut();
        });
    }

    public void Seek(string name, float position)
    {
        SongTrack track = Tracks.Find(t => t.Name == name);
        if (track == null) return;
        Raylib.SeekMusicStream(track.Song, position);
    }

    public void Seek(float position)
    {
        Tracks.ForEach(t =>
        {
            Raylib.SeekMusicStream(t.Song, position);
        });
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
            t.Update();
            Raylib.SetMusicVolume(t.Song, Game.MusicVolume * t.Volume);
            Raylib.SetMusicPitch(t.Song, Pitch);
            Raylib.UpdateMusicStream(t.Song);
            if (t.Song.LoopStart != null && Raylib.GetMusicTimePlayed(t.Song) >= t.Song.LoopEnd)
            {
                Raylib.SeekMusicStream(t.Song, t.Song.LoopStart.Value);
            }

            // Raylib.SeekMusicStream(t.Song, 175);

            if (!Raylib.IsMusicStreamPlaying(t.Song))
            {
                Raylib.PlayMusicStream(t.Song);
                Raylib.SeekMusicStream(t.Song, t.Song.LoopStart.Value);
            }
        });
    }
}
