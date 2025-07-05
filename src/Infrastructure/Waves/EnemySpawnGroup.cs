namespace Game.Infrastructure.Waves;

internal class EnemySpawnGroup
{
    public string EnemyType { get; set; } = "Basic";
    public int Count { get; set; } = 5;
    public float SpawnInterval { get; set; } = 1.0f;
    public float StartDelay { get; set; } = 0.0f;
    public float HealthMultiplier { get; set; } = 1.0f;
    public float SpeedMultiplier { get; set; } = 1.0f;
    public int MoneyReward { get; set; } = 10;
}
