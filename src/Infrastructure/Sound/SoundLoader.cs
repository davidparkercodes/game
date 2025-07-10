using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using Godot;
using Game.Domain.Audio.ValueObjects;
using Game.Domain.Audio.Enums;

namespace Game.Infrastructure.Sound;

public static class SoundLoader
{
    private const string SOUND_CONFIG_PATH = "res://config/audio/sound_config.json";
    
    public static Dictionary<string, AudioStream> LoadSounds()
    {
        GD.Print($"🎵 SoundLoader.LoadSounds starting...");
        var sounds = new Dictionary<string, AudioStream>();
        
        GD.Print($"🎵 Loading sound configuration...");
        var config = LoadConfiguration();
        
        GD.Print($"🎵 Configuration loaded, found {config.GetAllSounds().Count} sound entries");
        
        foreach (var soundData in config.GetAllSounds())
        {
            try
            {
                GD.Print($"🔊 Attempting to load sound: {soundData.Key} from {soundData.Value.FilePath}");
                var audioStream = GD.Load<AudioStream>(soundData.Value.FilePath);
                if (audioStream != null)
                {
                    sounds[soundData.Key] = audioStream;
                    GD.Print($"✅ Loaded sound: {soundData.Key} from {soundData.Value.FilePath}");
                }
                else
                {
                    GD.PrintErr($"❌ Failed to load sound: {soundData.Key} from {soundData.Value.FilePath}");
                }
            }
            catch (System.Exception ex)
            {
                GD.PrintErr($"❌ Error loading sound {soundData.Key}: {ex.Message}");
            }
        }
        
        GD.Print($"🔊 Loaded {sounds.Count} sounds total");
        return sounds;
    }

    public static SoundConfig LoadConfiguration()
    {
        var config = new SoundConfig();
        
        GD.Print($"📄 SoundLoader.LoadConfiguration starting...");
        
        try
        {
            GD.Print($"📄 Checking for sound config file: {SOUND_CONFIG_PATH}");
            if (!Godot.FileAccess.FileExists(SOUND_CONFIG_PATH))
            {
                GD.PrintErr($"❌ Sound config file not found: {SOUND_CONFIG_PATH}");
                return config;
            }
            
            GD.Print($"✅ Sound config file found!");
            
            using var file = Godot.FileAccess.Open(SOUND_CONFIG_PATH, Godot.FileAccess.ModeFlags.Read);
            string jsonContent = file.GetAsText();
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            
            GD.Print($"📄 Parsing JSON document...");
            var jsonDoc = JsonDocument.Parse(jsonContent);
            
            GD.Print($"📄 Getting sounds element...");
            var soundsElement = jsonDoc.RootElement.GetProperty("sounds");
            
            var categoryCount = 0;
            foreach (var _ in soundsElement.EnumerateObject()) categoryCount++;
            GD.Print($"📄 Found {categoryCount} sound categories");
            
            // Parse each category
            foreach (var categoryProperty in soundsElement.EnumerateObject())
            {
                string categoryName = categoryProperty.Name;
                GD.Print($"📄 Processing category: {categoryName}");
                
                int soundCount = 0;
                foreach (var soundProperty in categoryProperty.Value.EnumerateObject())
                {
                    string soundKey = soundProperty.Name;
                    var soundElement = soundProperty.Value;
                    
                    string filePath = soundElement.GetProperty("file").GetString() ?? "";
                    string categoryStr = soundElement.GetProperty("category").GetString() ?? "SFX";
                    float volume = soundElement.GetProperty("volume").GetSingle();
                    string description = soundElement.GetProperty("description").GetString() ?? "";
                    
                    var soundData = new SoundConfigData(soundKey, volume, filePath, false);
                    
                    config.AddSound(soundKey, soundData);
                    soundCount++;
                    GD.Print($"📄 Loaded sound config: {soundKey} -> {filePath}");
                }
                GD.Print($"📄 Category {categoryName} loaded {soundCount} sounds");
            }
            
            GD.Print($"✅ Sound configuration loaded from {SOUND_CONFIG_PATH}");
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"❌ Error loading sound configuration: {ex.Message}");
        }
        
        return config;
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
