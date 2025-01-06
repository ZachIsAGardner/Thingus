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
        Raylib.SetSoundVolume(sound, volume * Game.SoundEffectsVolume);
        Raylib.SetSoundPitch(sound, pitch);
        Raylib.SetSoundPan(sound, pan);
        Raylib.PlaySound(sound);
        return sound;
    }

    public override void Update()
    {
        Tracks = Tracks.Where(t => !t.Stopped).ToList();
        
        Tracks.ForEach(t =>
        {
            t.Update();
            Raylib.SetMusicVolume(t.Song, Game.SoundEffectsVolume * t.Volume);
            Log.Write("V: " + t.Volume);
            Raylib.SetMusicPitch(t.Song, t.Pitch);
            Raylib.UpdateMusicStream(t.Song);
            if (t.Song.LoopStart != null && Raylib.GetMusicTimePlayed(t.Song) >= t.Song.LoopEnd)
            {
                Raylib.SeekMusicStream(t.Song, t.Song.LoopStart.Value);
            }

            if (!Raylib.IsMusicStreamPlaying(t.Song))
            {
                Raylib.PlayMusicStream(t.Song);
                Raylib.SeekMusicStream(t.Song, t.Song.LoopStart.Value);
            }
        });
    }
}