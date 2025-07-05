using System.Collections.Generic;
using Game.Domain.Buildings.Services;
using Game.Domain.Buildings.ValueObjects;
using Game.Domain.Enemies.Services;
using Game.Domain.Enemies.ValueObjects;

namespace Game.Domain.Shared.Services;

public interface ITypeManagementService
{
    // Building Type Management
    IBuildingTypeRegistry BuildingTypeRegistry { get; }
    BuildingType? GetBuildingByConfigKey(string configKey);
    IEnumerable<BuildingType> GetBuildingsByCategory(string category);
    BuildingType? GetDefaultBuilding();
    BuildingType? GetCheapestBuilding();
    bool IsValidBuildingType(string configKey);
    
    // Enemy Type Management  
    IEnemyTypeRegistry EnemyTypeRegistry { get; }
    EnemyType? GetEnemyByConfigKey(string configKey);
    IEnumerable<EnemyType> GetEnemiesByCategory(string category);
    IEnumerable<EnemyType> GetEnemiesByTier(int tier);
    EnemyType? GetDefaultEnemy();
    EnemyType? GetBasicEnemy();
    bool IsValidEnemyType(string configKey);
    
    // Wave Progression Support
    IEnumerable<EnemyType> GetEnemiesForWave(int waveNumber);
    EnemyType? GetEnemyTypeForWave(int waveNumber, int enemyIndex);
    bool IsEnemyAvailableForWave(string configKey, int waveNumber);
    
    // Validation and Diagnostics
    bool ValidateAllConfigurations();
    IEnumerable<string> GetConfigurationErrors();
    void LogRegistryStatus();
}
