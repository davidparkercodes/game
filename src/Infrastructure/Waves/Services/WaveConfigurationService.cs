using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;
using Game.Domain.Enemies.Services;
using Game.Domain.Enemies.ValueObjects;
using Game.Infrastructure.Waves.Models;

namespace Game.Infrastructure.Waves.Services;

public class WaveConfigurationService : IWaveConfigurationService
{
    public WaveConfiguration LoadWaveSet(string configurationPath)
    {
        if (string.IsNullOrEmpty(configurationPath) || !IsInGodotRuntime() || !Godot.FileAccess.FileExists(configurationPath))
        {
            return CreateDefaultWaveSet();
        }

        try
        {
            var file = Godot.FileAccess.Open(configurationPath, Godot.FileAccess.ModeFlags.Read);
            if (file == null)
            {
                return CreateDefaultWaveSet();
            }

            string jsonText = file.GetAsText();
            file.Close();

            var json = new Json();
            var parseResult = json.Parse(jsonText);
            if (parseResult != Error.Ok)
            {
                return CreateDefaultWaveSet();
            }

            var jsonData = json.Data.AsGodotDictionary();
            var waveSet = ParseWaveSetFromJson(jsonData);
            return ConvertToWaveConfiguration(waveSet);
        }
        catch (Exception)
        {
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
        return new WaveConfiguration("Default Waves", 10, "{\"waves\": []}");
    }

    private static WaveSetModel ParseWaveSetFromJson(Godot.Collections.Dictionary jsonData)
    {
        var waveSet = new WaveSetModel();
        waveSet.SetName = jsonData.GetValueOrDefault("setName", "Unknown Wave Set").AsString();
        waveSet.Description = jsonData.GetValueOrDefault("description", "").AsString();

        if (jsonData.ContainsKey("waves"))
        {
            var wavesArray = jsonData["waves"].AsGodotArray();
            foreach (var waveData in wavesArray)
            {
                var waveDict = waveData.AsGodotDictionary();
                var wave = ParseWaveFromJson(waveDict);
                waveSet.Waves.Add(wave);
            }
        }

        return waveSet;
    }

    private static WaveModel ParseWaveFromJson(Godot.Collections.Dictionary waveData)
    {
        var wave = new WaveModel();
        wave.WaveNumber = waveData.GetValueOrDefault("waveNumber", 1).AsInt32();
        wave.WaveName = waveData.GetValueOrDefault("waveName", "Wave").AsString();
        wave.Description = waveData.GetValueOrDefault("description", "").AsString();
        wave.PreWaveDelay = waveData.GetValueOrDefault("preWaveDelay", 0.0f).AsSingle();
        wave.PostWaveDelay = waveData.GetValueOrDefault("postWaveDelay", 2.0f).AsSingle();
        wave.BonusMoney = waveData.GetValueOrDefault("bonusMoney", 25).AsInt32();

        if (waveData.ContainsKey("enemyGroups"))
        {
            var groupsArray = waveData["enemyGroups"].AsGodotArray();
            foreach (var groupData in groupsArray)
            {
                var groupDict = groupData.AsGodotDictionary();
                var group = ParseEnemyGroupFromJson(groupDict);
                wave.EnemyGroups.Add(group);
            }
        }

        return wave;
    }

    private static EnemySpawnGroup ParseEnemyGroupFromJson(Godot.Collections.Dictionary groupData)
    {
        var group = new EnemySpawnGroup();
        group.EnemyType = groupData.GetValueOrDefault("enemyType", "Basic").AsString();
        group.Count = groupData.GetValueOrDefault("count", 5).AsInt32();
        group.SpawnInterval = groupData.GetValueOrDefault("spawnInterval", 1.0f).AsSingle();
        group.StartDelay = groupData.GetValueOrDefault("startDelay", 0.0f).AsSingle();
        group.HealthMultiplier = groupData.GetValueOrDefault("healthMultiplier", 1.0f).AsSingle();
        group.SpeedMultiplier = groupData.GetValueOrDefault("speedMultiplier", 1.0f).AsSingle();
        group.MoneyReward = groupData.GetValueOrDefault("moneyReward", 10).AsInt32();
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
