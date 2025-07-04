using Godot;
using System.Collections.Generic;

public static class SoundConfigUtility
{
    public static void AddSound(string category, string soundKey, string filePath, string soundCategory = "SFX", float volume = 0.0f, string description = "")
    {
        var config = SoundLoader.LoadConfiguration();
        
        if (!config.Sounds.ContainsKey(category))
        {
            config.Sounds[category] = new Dictionary<string, SoundConfigData>();
        }
        
        config.Sounds[category][soundKey] = new SoundConfigData
        {
            File = filePath,
            Category = soundCategory,
            Volume = volume,
            Description = description
        };
        
        SoundLoader.SaveConfiguration(config);
        GD.Print($"✅ Added sound '{soundKey}' to category '{category}'");
    }
    
    public static void RemoveSound(string category, string soundKey)
    {
        var config = SoundLoader.LoadConfiguration();
        
        if (config.Sounds.ContainsKey(category) && config.Sounds[category].ContainsKey(soundKey))
        {
            config.Sounds[category].Remove(soundKey);
            SoundLoader.SaveConfiguration(config);
            GD.Print($"✅ Removed sound '{soundKey}' from category '{category}'");
        }
        else
        {
            GD.PrintErr($"❌ Sound '{soundKey}' not found in category '{category}'");
        }
    }
    
    public static void ListAllSounds()
    {
        var config = SoundLoader.LoadConfiguration();
        var allSounds = config.GetAllSounds();
        
        GD.Print("🎵 All configured sounds:");
        foreach (var sound in allSounds)
        {
            GD.Print($"  - {sound.Key}: {sound.Value.File} ({sound.Value.Category})");
        }
    }
    
    public static void ValidateAllSounds()
    {
        var config = SoundLoader.LoadConfiguration();
        var allSounds = config.GetAllSounds();
        int validCount = 0;
        int invalidCount = 0;
        
        GD.Print("🔍 Validating all sound files...");
        
        foreach (var sound in allSounds)
        {
            bool exists = FileAccess.FileExists(sound.Value.File);
            if (exists)
            {
                validCount++;
                GD.Print($"✅ {sound.Key}: {sound.Value.File}");
            }
            else
            {
                invalidCount++;
                GD.PrintErr($"❌ {sound.Key}: {sound.Value.File} (file not found)");
            }
        }
        
        GD.Print($"📊 Validation complete: {validCount} valid, {invalidCount} invalid");
    }
}
