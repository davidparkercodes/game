using System;
using System.Collections.Generic;
using Godot;
using Game.Infrastructure.Interfaces;
using Game.Infrastructure.Configuration;

namespace Game.Infrastructure.Waves;

public class WaveConfigService : IWaveConfigService
{
    public WaveSetConfig LoadWaveSet(string jsonPath)
    {
        if (string.IsNullOrEmpty(jsonPath) || !IsInGodotRuntime() || !FileAccess.FileExists(jsonPath))
        {
            return CreateDefaultWaveSet();
        }

        try
        {
            var file = FileAccess.Open(jsonPath, FileAccess.ModeFlags.Read);
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
            return waveSet;
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

    public WaveSetConfig CreateDefaultWaveSet()
    {
        var waveSet = new WaveSetConfig();
        waveSet.SetName = "Default Waves";
        waveSet.Description = "Auto-generated default wave progression";
        
        for (int i = 1; i <= 10; i++)
        {
            var wave = new WaveConfig();
            wave.WaveNumber = i;
            wave.WaveName = $"Wave {i}";
            wave.BonusMoney = 25 + (i * 5);
            wave.Description = $"Standard wave {i} with {5 + i * 2} enemies";
            
            var enemyGroup = new EnemySpawnGroup();
            enemyGroup.EnemyType = "Basic";
            enemyGroup.Count = 5 + (i * 2);
            enemyGroup.SpawnInterval = Mathf.Max(0.5f, 2.0f - (i * 0.1f));
            enemyGroup.HealthMultiplier = 1.0f + (i * 0.15f);
            enemyGroup.SpeedMultiplier = 1.0f + (i * 0.05f);
            enemyGroup.MoneyReward = 10 + (i * 2);
            
            wave.EnemyGroups.Add(enemyGroup);
            waveSet.Waves.Add(wave);
        }

        return waveSet;
    }

    private static WaveSetConfig ParseWaveSetFromJson(Godot.Collections.Dictionary jsonData)
    {
        var waveSet = new WaveSetConfig();
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

    private static WaveConfig ParseWaveFromJson(Godot.Collections.Dictionary waveData)
    {
        var wave = new WaveConfig();
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
}
