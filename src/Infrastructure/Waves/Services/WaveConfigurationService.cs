using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Godot;
using Game.Domain.Enemies.Services;
using Game.Domain.Enemies.ValueObjects;
using Game.Infrastructure.Waves.Models;

namespace Game.Infrastructure.Waves.Services;

public class WaveConfigurationService : IWaveConfigurationService
{
    private readonly Dictionary<string, WaveConfiguration> _cachedWaveSets = new Dictionary<string, WaveConfiguration>();
    
    public WaveConfiguration LoadWaveSet(string difficulty = "default")
    {
        var configurationPath = GetWaveSetPath(difficulty);
        return LoadWaveSet(configurationPath);
    }
    
    public string[] GetAvailableWaveSets()
    {
        var waveSets = new List<string>();
        var waveDirectory = "res://config/levels/";
        
        try
        {
            if (IsInGodotRuntime())
            {
                var directory = DirAccess.Open(waveDirectory);
                if (directory != null)
                {
                    directory.ListDirBegin();
                    string fileName = directory.GetNext();
                    
                    while (!string.IsNullOrEmpty(fileName))
                    {
                        if (fileName.EndsWith(".json") && !directory.CurrentIsDir())
                        {
                            var setName = fileName.Replace("_waves.json", "").Replace(".json", "");
                            waveSets.Add(setName);
                        }
                        fileName = directory.GetNext();
                    }
                    
                    directory.ListDirEnd();
                }
            }
        }
        catch (Exception exception)
        {
            GD.PrintErr($"WaveConfigurationService: Failed to scan wave directory: {exception.Message}");
        }
        
        if (waveSets.Count == 0)
        {
            waveSets.Add("default");
        }
        
        GD.Print($"WaveConfigurationService: Found {waveSets.Count} wave sets: {string.Join(", ", waveSets)}");
        return waveSets.ToArray();
    }
    
    public void ClearCache()
    {
        _cachedWaveSets.Clear();
        GD.Print("WaveConfigurationService: Wave set cache cleared");
    }
    
    private static string GetWaveSetPath(string difficulty)
    {
        return difficulty.ToLower() switch
        {
            "easy" => "res://data/waves/easy_waves.json",
            "hard" or "nightmare" => "res://data/waves/hard_waves.json",
            "default" or "enhanced" => "res://data/waves/default_waves.json",
            _ => $"res://data/waves/{difficulty}_waves.json"
        };
    }

    public WaveConfiguration LoadWaveSetFromPath(string configurationPath)
    {
        if (string.IsNullOrEmpty(configurationPath))
        {
            GD.PrintErr("WaveConfigurationService: Configuration path is null or empty");
            return CreateDefaultWaveSet();
        }
        
        if (_cachedWaveSets.TryGetValue(configurationPath, out var cachedWaveSet))
        {
            GD.Print($"WaveConfigurationService: Using cached wave set for {configurationPath}");
            return cachedWaveSet;
        }

        if (!IsInGodotRuntime())
        {
            GD.Print("WaveConfigurationService: Not in Godot runtime, using default wave set");
            return CreateDefaultWaveSet();
        }

        if (!Godot.FileAccess.FileExists(configurationPath))
        {
            GD.PrintErr($"WaveConfigurationService: Wave configuration file not found at {configurationPath}");
            return CreateDefaultWaveSet();
        }

        try
        {
            var file = Godot.FileAccess.Open(configurationPath, Godot.FileAccess.ModeFlags.Read);
            if (file == null)
            {
                GD.PrintErr($"WaveConfigurationService: Failed to open wave configuration file at {configurationPath}");
                return CreateDefaultWaveSet();
            }

            string jsonText = file.GetAsText();
            file.Close();

            if (string.IsNullOrWhiteSpace(jsonText))
            {
                GD.PrintErr($"WaveConfigurationService: Wave configuration file is empty at {configurationPath}");
                return CreateDefaultWaveSet();
            }

            var json = new Json();
            var parseResult = json.Parse(jsonText);
            if (parseResult != Error.Ok)
            {
                GD.PrintErr($"WaveConfigurationService: JSON parsing failed for {configurationPath}: {parseResult}");
                return CreateDefaultWaveSet();
            }

            var jsonData = json.Data.AsGodotDictionary();
            var waveSet = ParseWaveSetFromJson(jsonData);
            
            if (!ValidateWaveSet(waveSet))
            {
                GD.PrintErr($"WaveConfigurationService: Wave set validation failed for {configurationPath}");
                return CreateDefaultWaveSet();
            }

            GD.Print($"WaveConfigurationService: Successfully loaded wave set '{waveSet.SetName}' with {waveSet.Waves.Count} waves from {configurationPath}");
            var waveConfiguration = ConvertToWaveConfiguration(waveSet);
            _cachedWaveSets[configurationPath] = waveConfiguration;
            return waveConfiguration;
        }
        catch (Exception exception)
        {
            GD.PrintErr($"WaveConfigurationService: Exception loading wave configuration from {configurationPath}: {exception.Message}");
            return CreateDefaultWaveSet();
        }
    }

    private static bool IsInGodotRuntime()
    {
        try
        {
            return Godot.Engine.IsEditorHint() || !Godot.Engine.IsEditorHint();
        }
        catch
        {
            return false;
        }
    }

    public WaveConfiguration CreateDefaultWaveSet()
    {
        GD.Print("WaveConfigurationService: Creating default wave set as fallback");
        return new WaveConfiguration("Default Waves", 0, "{\"setName\": \"Default Waves\", \"description\": \"Fallback wave set\", \"waves\": []}");
    }

    private static bool ValidateWaveSet(WaveSetModel waveSet)
    {
        if (waveSet == null)
        {
            GD.PrintErr("WaveConfigurationService: Wave set is null");
            return false;
        }

        if (string.IsNullOrWhiteSpace(waveSet.SetName))
        {
            GD.PrintErr("WaveConfigurationService: Wave set name is null or empty");
            return false;
        }

        if (waveSet.Waves == null || waveSet.Waves.Count == 0)
        {
            GD.PrintErr("WaveConfigurationService: Wave set contains no waves");
            return false;
        }

        for (int i = 0; i < waveSet.Waves.Count; i++)
        {
            var wave = waveSet.Waves[i];
            if (!ValidateWave(wave, i + 1))
            {
                return false;
            }
        }

        GD.Print($"WaveConfigurationService: Wave set validation passed for '{waveSet.SetName}'");
        return true;
    }

    private static bool ValidateWave(WaveModel wave, int expectedWaveNumber)
    {
        if (wave == null)
        {
            GD.PrintErr($"WaveConfigurationService: Wave {expectedWaveNumber} is null");
            return false;
        }

        if (wave.WaveNumber != expectedWaveNumber)
        {
            GD.PrintErr($"WaveConfigurationService: Wave number mismatch - expected {expectedWaveNumber}, got {wave.WaveNumber}");
            return false;
        }

        if (string.IsNullOrWhiteSpace(wave.WaveName))
        {
            GD.PrintErr($"WaveConfigurationService: Wave {expectedWaveNumber} has no name");
            return false;
        }

        if (wave.EnemyGroups == null || wave.EnemyGroups.Count == 0)
        {
            GD.PrintErr($"WaveConfigurationService: Wave {expectedWaveNumber} has no enemy groups");
            return false;
        }

        foreach (var group in wave.EnemyGroups)
        {
            if (!ValidateEnemyGroup(group, expectedWaveNumber))
            {
                return false;
            }
        }

        return true;
    }

    private static bool ValidateEnemyGroup(EnemySpawnGroup group, int waveNumber)
    {
        if (group == null)
        {
            GD.PrintErr($"WaveConfigurationService: Enemy group in wave {waveNumber} is null");
            return false;
        }

        if (string.IsNullOrWhiteSpace(group.EnemyType))
        {
            GD.PrintErr($"WaveConfigurationService: Enemy group in wave {waveNumber} has no enemy type");
            return false;
        }

        if (group.Count <= 0)
        {
            GD.PrintErr($"WaveConfigurationService: Enemy group in wave {waveNumber} has invalid count: {group.Count}");
            return false;
        }

        if (group.SpawnInterval < 0)
        {
            GD.PrintErr($"WaveConfigurationService: Enemy group in wave {waveNumber} has negative spawn interval: {group.SpawnInterval}");
            return false;
        }

        if (group.HealthMultiplier <= 0)
        {
            GD.PrintErr($"WaveConfigurationService: Enemy group in wave {waveNumber} has invalid health multiplier: {group.HealthMultiplier}");
            return false;
        }

        if (group.SpeedMultiplier <= 0)
        {
            GD.PrintErr($"WaveConfigurationService: Enemy group in wave {waveNumber} has invalid speed multiplier: {group.SpeedMultiplier}");
            return false;
        }

        return true;
    }

    private static WaveSetModel ParseWaveSetFromJson(Godot.Collections.Dictionary jsonData)
    {
        var waveSet = new WaveSetModel();
        
        try
        {
            waveSet.SetName = jsonData.GetValueOrDefault("setName", "Unknown Wave Set").AsString();
            waveSet.Description = jsonData.GetValueOrDefault("description", "").AsString();

            if (jsonData.ContainsKey("waves"))
            {
                var wavesArray = jsonData["waves"].AsGodotArray();
                if (wavesArray != null)
                {
                    foreach (var waveData in wavesArray)
                    {
                        try
                        {
                            var waveDict = waveData.AsGodotDictionary();
                            if (waveDict != null)
                            {
                                var wave = ParseWaveFromJson(waveDict);
                                if (wave != null)
                                {
                                    waveSet.Waves.Add(wave);
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            GD.PrintErr($"WaveConfigurationService: Failed to parse wave data: {exception.Message}");
                        }
                    }
                }
            }
            else
            {
                GD.PrintErr("WaveConfigurationService: JSON data does not contain 'waves' key");
            }
        }
        catch (Exception exception)
        {
            GD.PrintErr($"WaveConfigurationService: Failed to parse wave set: {exception.Message}");
        }

        return waveSet;
    }

    private static WaveModel ParseWaveFromJson(Godot.Collections.Dictionary waveData)
    {
        var wave = new WaveModel();
        
        try
        {
            wave.WaveNumber = waveData.GetValueOrDefault("waveNumber", 1).AsInt32();
            wave.WaveName = waveData.GetValueOrDefault("waveName", "Wave").AsString();
            wave.Description = waveData.GetValueOrDefault("description", "").AsString();
            wave.PreWaveDelay = waveData.GetValueOrDefault("preWaveDelay", 0.0f).AsSingle();
            wave.PostWaveDelay = waveData.GetValueOrDefault("postWaveDelay", 2.0f).AsSingle();
            wave.BonusMoney = waveData.GetValueOrDefault("bonusMoney", 25).AsInt32();

            if (waveData.ContainsKey("enemyGroups"))
            {
                var groupsArray = waveData["enemyGroups"].AsGodotArray();
                if (groupsArray != null)
                {
                    foreach (var groupData in groupsArray)
                    {
                        try
                        {
                            var groupDict = groupData.AsGodotDictionary();
                            if (groupDict != null)
                            {
                                var group = ParseEnemyGroupFromJson(groupDict);
                                if (group != null)
                                {
                                    wave.EnemyGroups.Add(group);
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            GD.PrintErr($"WaveConfigurationService: Failed to parse enemy group in wave {wave.WaveNumber}: {exception.Message}");
                        }
                    }
                }
            }
            else
            {
                GD.PrintErr($"WaveConfigurationService: Wave {wave.WaveNumber} does not contain 'enemyGroups' key");
            }
        }
        catch (Exception exception)
        {
            GD.PrintErr($"WaveConfigurationService: Failed to parse wave: {exception.Message}");
        }

        return wave;
    }

    private static EnemySpawnGroup ParseEnemyGroupFromJson(Godot.Collections.Dictionary groupData)
    {
        var group = new EnemySpawnGroup();
        
        try
        {
            group.EnemyType = groupData.GetValueOrDefault("enemyType", Domain.Entities.EnemyConfigKeys.BasicEnemy).AsString();
            group.Count = groupData.GetValueOrDefault("count", 5).AsInt32();
            group.SpawnInterval = groupData.GetValueOrDefault("spawnInterval", 1.0f).AsSingle();
            group.StartDelay = groupData.GetValueOrDefault("startDelay", 0.0f).AsSingle();
            group.HealthMultiplier = groupData.GetValueOrDefault("healthMultiplier", 1.0f).AsSingle();
            group.SpeedMultiplier = groupData.GetValueOrDefault("speedMultiplier", 1.0f).AsSingle();
            group.MoneyReward = groupData.GetValueOrDefault("moneyReward", 10).AsInt32();
        }
        catch (Exception exception)
        {
            GD.PrintErr($"WaveConfigurationService: Failed to parse enemy group: {exception.Message}");
            return new EnemySpawnGroup();
        }
        
        return group;
    }

    private WaveConfiguration ConvertToWaveConfiguration(WaveSetModel waveSetConfiguration)
    {
        var serializedData = JsonSerializer.Serialize(waveSetConfiguration);
        return new WaveConfiguration(
            waveSetConfiguration.SetName ?? "Default Waves",
            waveSetConfiguration.Waves?.Count ?? 0,
            serializedData
        );
    }
}
