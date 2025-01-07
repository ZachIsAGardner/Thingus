using Raylib_cs;

namespace Thingus;

public class SoundEffectManager : Thing
{
    public List<SongTrack> Tracks = new List<SongTrack>() { };
    
    public SongTrack PlayLooped(string name, float volume = 1f, float pitch = 1f, float pan = 0.5f)
    {     
        Music song = Library.Songs[name];
        Raylib.PlayMusicStream(song);
        SongTrack track = new SongTrack(name, song, volume, pitch, pan);
        Tracks.Add(track);
        return track;
    }

    public Sound Play(string name, float volume = 1f, float pitch = 1f, float pan = 0.5f)
    {
        Sound sound = Library.SoundEffects[name];
        Raylib.SetSoundVolume(sound, volume * Settings.SoundEffectsVolume);
        Raylib.SetSoundPitch(sound, pitch);
        Raylib.SetSoundPan(sound, pan);
        Raylib.PlaySound(sound);
        return sound;
    }

    public override void Update()
    {
        Tracks.ToList().ForEach(t =>
        {
            t.Update();
        });
    }

    // Updated in Game
    public void UpdateTracks()
    {
        if (Settings.Mute) return;

        Tracks = Tracks.Where(t => !t.Stopped).ToList();

        Tracks.ToList().ForEach(t =>
        {
            if (Game.Root.Dynamic.Active) Raylib.SetMusicVolume(t.Song, Settings.SoundEffectsVolume * t.Volume);
            else Raylib.SetMusicVolume(t.Song, 0);
            Raylib.SetMusicPitch(t.Song, t.Pitch);
            Raylib.UpdateMusicStream(t.Song);

            // Custom Loop
            if (t.Song.LoopStart != null && Raylib.GetMusicTimePlayed(t.Song) >= t.Song.LoopEnd)
            {
                Raylib.SeekMusicStream(t.Song, t.Song.LoopStart.Value);
            }
            // Regular Loop
            else if (!Raylib.IsMusicStreamPlaying(t.Song))
            {
                Raylib.PlayMusicStream(t.Song);
                Raylib.SeekMusicStream(t.Song, t.Song.LoopStart.Value);
            }
        });
    }
}