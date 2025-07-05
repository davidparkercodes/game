using System.Collections.Generic;

namespace Game.Application.Simulation.ValueObjects;

public class WaveSetConfiguration
{
    public string SetName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<WaveConfiguration> Waves { get; set; } = new List<WaveConfiguration>();
}

public class WaveConfiguration
{
    public int WaveNumber { get; set; }
    public string WaveName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public float PreWaveDelay { get; set; }
    public float PostWaveDelay { get; set; }
    public int BonusMoney { get; set; }
    public List<EnemyGroupConfiguration> EnemyGroups { get; set; } = new List<EnemyGroupConfiguration>();
}

public class EnemyGroupConfiguration
{
    public string EnemyType { get; set; } = string.Empty;
    public int Count { get; set; }
    public float SpawnInterval { get; set; }
    public float StartDelay { get; set; }
    public float HealthMultiplier { get; set; } = 1.0f;
    public float SpeedMultiplier { get; set; } = 1.0f;
    public int MoneyReward { get; set; }
}
