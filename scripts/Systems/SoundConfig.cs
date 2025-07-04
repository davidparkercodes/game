using Godot;
using System.Collections.Generic;

public class SoundConfigData
{
    public string File { get; set; } = "";
    public string Category { get; set; } = "SFX";
    public float Volume { get; set; } = 0.0f;
    public string Description { get; set; } = "";
}

public class CategoryConfigData
{
    public float DefaultVolume { get; set; } = 1.0f;
    public string Description { get; set; } = "";
}

public class SoundConfiguration
{
    public Dictionary<string, Dictionary<string, SoundConfigData>> Sounds { get; set; } = new();
    public Dictionary<string, CategoryConfigData> Categories { get; set; } = new();
    
    public Dictionary<string, SoundConfigData> GetAllSounds()
    {
        var allSounds = new Dictionary<string, SoundConfigData>();
        
        foreach (var category in Sounds)
        {
            foreach (var sound in category.Value)
            {
                allSounds[sound.Key] = sound.Value;
            }
        }
        
        return allSounds;
    }
}
