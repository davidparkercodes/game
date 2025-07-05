using Game.Domain.Enemies.ValueObjects;
using Game.Domain.Buildings.ValueObjects;

namespace Game.Infrastructure.Managers;

public class StatsManager
{
    public static StatsManager Instance { get; private set; }

    static StatsManager()
    {
        Instance = new StatsManager();
    }

    public EnemyStatsData GetEnemyStats(string enemyType)
    {
        return enemyType switch
        {
            "basic_enemy" => EnemyStatsData.Empty,
            "fast_enemy" => new EnemyStatsData(50, 100f, 5, 8, 3, "Fast but weak enemy"),
            "tank_enemy" => new EnemyStatsData(200, 25f, 20, 20, 10, "Slow but tough enemy"),
            _ => EnemyStatsData.Empty
        };
    }

    public BuildingStatsData GetBuildingStats(string buildingType)
    {
        return buildingType switch
        {
            "basic_tower" => BuildingStatsData.Empty,
            "cannon_tower" => new BuildingStatsData(150, 50, 120f, 0.5f, 75, "High damage but slow attack speed"),
            "machine_gun_tower" => new BuildingStatsData(120, 15, 100f, 3.0f, 60, "Fast attack speed but low damage"),
            _ => BuildingStatsData.Empty
        };
    }

    public EnemyStatsData GetDefaultEnemyStats()
    {
        return EnemyStatsData.Empty;
    }

    public BuildingStatsData GetDefaultBuildingStats()
    {
        return BuildingStatsData.Empty;
    }

    public bool HasEnemyType(string enemyType)
    {
        return enemyType is "basic_enemy" or "fast_enemy" or "tank_enemy";
    }

    public bool HasBuildingType(string buildingType)
    {
        return buildingType is "basic_tower" or "cannon_tower" or "machine_gun_tower";
    }

    public void ReloadConfigurations()
    {
        // TODO: Implement configuration reloading
    }
}
