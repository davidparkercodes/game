using Game.Domain.Buildings.ValueObjects;
using Game.Domain.Enemies.ValueObjects;

namespace Game.Domain.Shared.Interfaces;

public interface IStatsProvider
{
    BuildingStats GetBuildingStats(string buildingType);
    EnemyStats GetEnemyStats(string enemyType);
    bool HasBuildingStats(string buildingType);
    bool HasEnemyStats(string enemyType);
}
