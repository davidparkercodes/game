using Game.Domain.Buildings.Services;
using Game.Domain.Enemies.Services;
using Game.Domain.Enemies.ValueObjects;
using Game.Domain.Buildings.ValueObjects;
using Game.Infrastructure.Managers;

namespace Game.Infrastructure.Stats;

public class StatsServiceAdapter : IBuildingStatsProvider, IEnemyStatsProvider
{
    private readonly StatsManager _statsManager;

    public StatsServiceAdapter(StatsManager statsManager)
    {
        _statsManager = statsManager ?? throw new System.ArgumentNullException(nameof(statsManager));
    }

    public EnemyStats GetEnemyStats(string enemyType)
    {
        var data = _statsManager.GetEnemyStats(enemyType);
        return ConvertToEnemyStats(data);
    }

    public BuildingStats GetBuildingStats(string buildingType)
    {
        var data = _statsManager.GetBuildingStats(buildingType);
        return ConvertToBuildingStats(data);
    }

    private EnemyStats ConvertToEnemyStats(EnemyStatsData data)
    {
        return new EnemyStats(
            maxHealth: data.max_health,
            speed: data.speed,
            damage: data.damage,
            rewardGold: data.reward_gold,
            rewardXp: data.reward_xp,
            description: data.description
        );
    }

    private BuildingStats ConvertToBuildingStats(BuildingStatsData data)
    {
        return new BuildingStats(
            cost: data.cost,
            damage: data.damage,
            range: data.range,
            fireRate: data.fire_rate,
            bulletSpeed: data.bullet_speed,
            shootSound: "",
            impactSound: "",
            description: data.description
        );
    }

    public bool HasEnemyStats(string enemyType)
    {
        return _statsManager.HasEnemyType(enemyType);
    }

    public bool HasBuildingStats(string buildingType)
    {
        return _statsManager.HasBuildingType(buildingType);
    }
}
