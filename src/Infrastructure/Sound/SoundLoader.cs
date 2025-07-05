using System.Collections.Generic;
using Godot;
using Game.Domain.Audio.ValueObjects;

namespace Game.Infrastructure.Sound;

public static class SoundLoader
{
    public static Dictionary<string, AudioStream> LoadSounds()
    {
        var sounds = new Dictionary<string, AudioStream>();
        
        // TODO: Implement actual sound loading from assets
        // For now, return empty dictionary to avoid runtime errors
        
        return sounds;
    }

    public static SoundConfig LoadConfiguration()
    {
        // TODO: Implement actual sound configuration loading
        // For now, return a default configuration
        
        return new SoundConfig();
    }
}

public class SoundConfig
{
    private readonly Dictionary<string, SoundConfigData> _sounds = new();

    public Dictionary<string, SoundConfigData> GetAllSounds()
    {
        return _sounds;
    }

    public void AddSound(string key, SoundConfigData config)
    {
        _sounds[key] = config;
    }

    public SoundConfigData GetSound(string key)
    {
        return _sounds.ContainsKey(key) ? _sounds[key] : new SoundConfigData(key);
    }
}
