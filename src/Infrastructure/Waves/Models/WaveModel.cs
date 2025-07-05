using System.Collections.Generic;

namespace Game.Infrastructure.Waves.Models;

internal class WaveModel
{
    public int WaveNumber { get; set; }
    public string WaveName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public float PreWaveDelay { get; set; }
    public float PostWaveDelay { get; set; } = 2.0f;
    public int BonusMoney { get; set; } = 25;
    public List<EnemySpawnGroup> EnemyGroups { get; set; } = new List<EnemySpawnGroup>();
}
