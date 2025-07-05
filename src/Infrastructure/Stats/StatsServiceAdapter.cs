using Game.Infrastructure.Interfaces;
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

    public EnemyStatsData GetEnemyStats(string enemyType)
    {
        return _statsManager.GetEnemyStats(enemyType);
    }

    public EnemyStatsData GetDefaultEnemyStats()
    {
        return _statsManager.GetDefaultEnemyStats();
    }

    public BuildingStatsData GetBuildingStats(string buildingType)
    {
        return _statsManager.GetBuildingStats(buildingType);
    }

    public BuildingStatsData GetDefaultBuildingStats()
    {
        return _statsManager.GetDefaultBuildingStats();
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
