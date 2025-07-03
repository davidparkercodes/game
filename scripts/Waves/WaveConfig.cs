using Godot;
using System.Collections.Generic;

[System.Serializable]
public partial class WaveConfig : Resource
{
	[Export] public int WaveNumber { get; set; }
	[Export] public string WaveName { get; set; } = "";
	[Export] public Godot.Collections.Array<EnemySpawnGroup> EnemyGroups { get; set; } = new();
	[Export] public float PreWaveDelay { get; set; } = 0.0f;
	[Export] public float PostWaveDelay { get; set; } = 2.0f;
	[Export] public int BonusMoney { get; set; } = 25;
	[Export] public string Description { get; set; } = "";
}

[System.Serializable]
public partial class EnemySpawnGroup : Resource
{
	[Export] public string EnemyType { get; set; } = "Basic";
	[Export] public int Count { get; set; } = 5;
	[Export] public float SpawnInterval { get; set; } = 1.0f;
	[Export] public float StartDelay { get; set; } = 0.0f;
	[Export] public float HealthMultiplier { get; set; } = 1.0f;
	[Export] public float SpeedMultiplier { get; set; } = 1.0f;
	[Export] public int MoneyReward { get; set; } = 10;
}

[System.Serializable] 
public partial class WaveSetConfig : Resource
{
	[Export] public new string SetName { get; set; } = "Default Wave Set";
	[Export] public string Description { get; set; } = "";
	[Export] public Godot.Collections.Array<WaveConfig> Waves { get; set; } = new();
}
