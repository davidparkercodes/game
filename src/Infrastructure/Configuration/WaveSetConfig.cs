using System.Collections.Generic;

namespace Game.Infrastructure.Configuration;

public class WaveSetConfig
{
    public string SetName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<WaveConfig> Waves { get; set; } = new();
    public int InitialMoney { get; set; } = 100;
    public int InitialLives { get; set; } = 20;

    public WaveSetConfig()
    {
    }

    public WaveSetConfig(string setName, string description = "")
    {
        SetName = setName ?? string.Empty;
        Description = description;
    }
}

public class WaveConfig
{
    public int WaveNumber { get; set; }
    public string WaveName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<EnemySpawnGroup> EnemyGroups { get; set; } = new();
    public float PreWaveDelay { get; set; } = 0.0f;
    public float PostWaveDelay { get; set; } = 2.0f;
    public int BonusMoney { get; set; } = 25;

    public WaveConfig()
    {
    }

    public WaveConfig(int waveNumber, string waveName = "")
    {
        WaveNumber = waveNumber;
        WaveName = waveName;
    }
}

public class EnemySpawnGroup
{
    public string EnemyType { get; set; } = "Basic";
    public int Count { get; set; } = 5;
    public float SpawnInterval { get; set; } = 1.0f;
    public float StartDelay { get; set; } = 0.0f;
    public float HealthMultiplier { get; set; } = 1.0f;
    public float SpeedMultiplier { get; set; } = 1.0f;
    public int MoneyReward { get; set; } = 10;

    public EnemySpawnGroup()
    {
    }

    public EnemySpawnGroup(string enemyType, int count, float spawnInterval = 1.0f)
    {
        EnemyType = enemyType ?? "Basic";
        Count = count;
        SpawnInterval = spawnInterval;
    }
}
