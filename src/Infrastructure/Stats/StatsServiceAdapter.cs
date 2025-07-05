using Game.Domain.Shared.Services;
using Game.Domain.Enemies.ValueObjects;
using Game.Domain.Buildings.ValueObjects;
using Game.Infrastructure.Managers;

namespace Game.Infrastructure.Stats;

public class StatsServiceAdapter : IStatsService
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

    public EnemyStats GetDefaultEnemyStats()
    {
        var data = _statsManager.GetDefaultEnemyStats();
        return ConvertToEnemyStats(data);
    }

    public BuildingStats GetBuildingStats(string buildingType)
    {
        var data = _statsManager.GetBuildingStats(buildingType);
        return ConvertToBuildingStats(data);
    }

    public BuildingStats GetDefaultBuildingStats()
    {
        var data = _statsManager.GetDefaultBuildingStats();
        return ConvertToBuildingStats(data);
    }

    private EnemyStats ConvertToEnemyStats(EnemyStatsData data)
    {
        return new EnemyStats(
            health: data.health,
            speed: data.speed,
            damage: data.damage,
            reward: data.reward
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
            description: data.description
        );
    }

    public bool HasEnemyType(string enemyType)
    {
        return _statsManager.HasEnemyType(enemyType);
    }

    public bool HasBuildingType(string buildingType)
    {
        return _statsManager.HasBuildingType(buildingType);
    }

    public void ReloadConfigurations()
    {
        _statsManager.ReloadConfigurations();
    }
}
