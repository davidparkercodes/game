using Game.Domain.Enemies.ValueObjects;
using Game.Domain.Buildings.ValueObjects;

namespace Game.Infrastructure.Interfaces;

public interface IStatsService
{
    EnemyStatsData GetEnemyStats(string enemyType);
    EnemyStatsData GetDefaultEnemyStats();
    BuildingStatsData GetBuildingStats(string buildingType);
    BuildingStatsData GetDefaultBuildingStats();
    bool HasEnemyType(string enemyType);
    bool HasBuildingType(string buildingType);
    void ReloadConfigurations();
}
