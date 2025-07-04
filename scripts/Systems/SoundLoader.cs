using Godot;
using System.Collections.Generic;
using System.Text.Json;

public static class SoundLoader
{
    private const string ConfigPath = "res://data/audio/sound_config.json";
    
    public static Dictionary<string, AudioStream> LoadSounds()
    {
        var sounds = new Dictionary<string, AudioStream>();
        var config = LoadConfiguration();
        
        if (config == null)
        {
            GD.PrintErr("❌ Failed to load sound configuration");
            return sounds;
        }
        
        var allSounds = config.GetAllSounds();
        int loadedCount = 0;
        int failedCount = 0;
        
        foreach (var soundEntry in allSounds)
        {
            string soundKey = soundEntry.Key;
            var soundData = soundEntry.Value;
            
            try
            {
                var audioStream = GD.Load<AudioStream>(soundData.File);
                if (audioStream != null)
                {
                    sounds[soundKey] = audioStream;
                    loadedCount++;
                    GD.Print($"✅ Loaded sound: {soundKey} from {soundData.File}");
                }
                else
                {
                    GD.PrintErr($"❌ Failed to load sound file: {soundData.File}");
                    failedCount++;
                }
            }
            catch (System.Exception e)
            {
                GD.PrintErr($"❌ Error loading sound {soundKey}: {e.Message}");
                failedCount++;
            }
        }
        
        GD.Print($"🎵 Sound loading complete: {loadedCount} loaded, {failedCount} failed");
        return sounds;
    }
    
    public static SoundConfiguration LoadConfiguration()
    {
        try
        {
            var file = FileAccess.Open(ConfigPath, FileAccess.ModeFlags.Read);
            if (file == null)
            {
                GD.PrintErr($"❌ Cannot open sound config file: {ConfigPath}");
                return CreateDefaultConfiguration();
            }
            
            string jsonText = file.GetAsText();
            file.Close();
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            
            var config = JsonSerializer.Deserialize<SoundConfiguration>(jsonText, options);
            
            if (config == null)
            {
                GD.PrintErr("❌ Failed to deserialize sound configuration");
                return CreateDefaultConfiguration();
            }
            
            GD.Print($"✅ Loaded sound configuration from {ConfigPath}");
            return config;
        }
        catch (System.Exception e)
        {
            GD.PrintErr($"❌ Error loading sound configuration: {e.Message}");
            return CreateDefaultConfiguration();
        }
    }
    
    private static SoundConfiguration CreateDefaultConfiguration()
    {
        GD.Print("⚠️ Creating default sound configuration");
        
        var config = new SoundConfiguration();
        
        config.Categories["SFX"] = new CategoryConfigData { DefaultVolume = 1.0f, Description = "Sound effects" };
        config.Categories["UI"] = new CategoryConfigData { DefaultVolume = 1.0f, Description = "User interface sounds" };
        config.Categories["Music"] = new CategoryConfigData { DefaultVolume = 0.8f, Description = "Background music" };
        
        config.Sounds["turrets"] = new Dictionary<string, SoundConfigData>();
        config.Sounds["projectiles"] = new Dictionary<string, SoundConfigData>();
        config.Sounds["game"] = new Dictionary<string, SoundConfigData>();
        config.Sounds["ui"] = new Dictionary<string, SoundConfigData>();
        config.Sounds["enemies"] = new Dictionary<string, SoundConfigData>();
        config.Sounds["music"] = new Dictionary<string, SoundConfigData>();
        
        return config;
    }
    
    public static void SaveConfiguration(SoundConfiguration config)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            string jsonText = JsonSerializer.Serialize(config, options);
            
            var file = FileAccess.Open(ConfigPath, FileAccess.ModeFlags.Write);
            if (file != null)
            {
                file.StoreString(jsonText);
                file.Close();
                GD.Print($"✅ Saved sound configuration to {ConfigPath}");
            }
            else
            {
                GD.PrintErr($"❌ Cannot save sound config file: {ConfigPath}");
            }
        }
        catch (System.Exception e)
        {
            GD.PrintErr($"❌ Error saving sound configuration: {e.Message}");
        }
    }
}
