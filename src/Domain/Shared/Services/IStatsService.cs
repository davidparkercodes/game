using Game.Domain.Buildings.ValueObjects;
using Game.Domain.Enemies.ValueObjects;

namespace Game.Domain.Shared.Services;

public interface IStatsService
{
    // Enemy stats
    EnemyStats GetEnemyStats(string enemyType);
    EnemyStats GetDefaultEnemyStats();
    bool HasEnemyType(string enemyType);
    
    // Building stats  
    BuildingStats GetBuildingStats(string buildingType);
    BuildingStats GetDefaultBuildingStats();
    bool HasBuildingType(string buildingType);
    
    // Configuration management
    void ReloadConfigurations();
}
