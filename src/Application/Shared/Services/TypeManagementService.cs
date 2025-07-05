using System;
using System.Collections.Generic;
using System.Linq;
using Game.Domain.Buildings.Services;
using Game.Domain.Buildings.ValueObjects;
using Game.Domain.Enemies.Services;
using Game.Domain.Enemies.ValueObjects;
using Game.Domain.Shared.Services;

namespace Game.Application.Shared.Services;

public class TypeManagementService : ITypeManagementService
{
    public IBuildingTypeRegistry BuildingTypeRegistry { get; }
    public IEnemyTypeRegistry EnemyTypeRegistry { get; }

    public TypeManagementService(IBuildingTypeRegistry buildingTypeRegistry, IEnemyTypeRegistry enemyTypeRegistry)
    {
        BuildingTypeRegistry = buildingTypeRegistry ?? throw new ArgumentNullException(nameof(buildingTypeRegistry));
        EnemyTypeRegistry = enemyTypeRegistry ?? throw new ArgumentNullException(nameof(enemyTypeRegistry));
    }

    // Building Type Management
    public BuildingType? GetBuildingByConfigKey(string configKey)
    {
        return BuildingTypeRegistry.GetByConfigKey(configKey);
    }

    public IEnumerable<BuildingType> GetBuildingsByCategory(string category)
    {
        return BuildingTypeRegistry.GetByCategory(category);
    }

    public BuildingType? GetDefaultBuilding()
    {
        return BuildingTypeRegistry.GetDefaultType();
    }

    public BuildingType? GetCheapestBuilding()
    {
        return BuildingTypeRegistry.GetCheapestType();
    }

    public bool IsValidBuildingType(string configKey)
    {
        return BuildingTypeRegistry.IsValidConfigKey(configKey);
    }

    // Enemy Type Management
    public EnemyType? GetEnemyByConfigKey(string configKey)
    {
        return EnemyTypeRegistry.GetByConfigKey(configKey);
    }

    public IEnumerable<EnemyType> GetEnemiesByCategory(string category)
    {
        return EnemyTypeRegistry.GetByCategory(category);
    }

    public IEnumerable<EnemyType> GetEnemiesByTier(int tier)
    {
        return EnemyTypeRegistry.GetByTier(tier);
    }

    public EnemyType? GetDefaultEnemy()
    {
        return EnemyTypeRegistry.GetDefaultType();
    }

    public EnemyType? GetBasicEnemy()
    {
        return EnemyTypeRegistry.GetBasicType();
    }

    public bool IsValidEnemyType(string configKey)
    {
        return EnemyTypeRegistry.IsValidConfigKey(configKey);
    }

    // Wave Progression Support
    public IEnumerable<EnemyType> GetEnemiesForWave(int waveNumber)
    {
        return EnemyTypeRegistry.GetEnemiesForWave(waveNumber);
    }

    public EnemyType? GetEnemyTypeForWave(int waveNumber, int enemyIndex)
    {
        return EnemyTypeRegistry.GetEnemyTypeForWaveProgression(waveNumber, enemyIndex);
    }

    public bool IsEnemyAvailableForWave(string configKey, int waveNumber)
    {
        return EnemyTypeRegistry.IsEnemyAvailableForWave(configKey, waveNumber);
    }

    // Validation and Diagnostics
    public bool ValidateAllConfigurations()
    {
        var errors = GetConfigurationErrors().ToList();
        return !errors.Any();
    }

    public IEnumerable<string> GetConfigurationErrors()
    {
        var errors = new List<string>();

        try
        {
            // Validate building types
            var buildings = BuildingTypeRegistry.GetAllTypes().ToList();
            if (!buildings.Any())
            {
                errors.Add("No building types found in registry");
            }

            var buildingCategories = BuildingTypeRegistry.GetAllCategories().ToList();
            if (!buildingCategories.Any())
            {
                errors.Add("No building categories found in registry");
            }

            // Check for required building categories
            var requiredBuildingCategories = new[] { "starter", "precision", "rapid", "heavy" };
            foreach (var category in requiredBuildingCategories)
            {
                if (!BuildingTypeRegistry.GetByCategory(category).Any())
                {
                    errors.Add($"Required building category '{category}' has no types");
                }
            }

            // Validate enemy types
            var enemies = EnemyTypeRegistry.GetAllTypes().ToList();
            if (!enemies.Any())
            {
                errors.Add("No enemy types found in registry");
            }

            var enemyCategories = EnemyTypeRegistry.GetAllCategories().ToList();
            if (!enemyCategories.Any())
            {
                errors.Add("No enemy categories found in registry");
            }

            // Check for required enemy categories
            var requiredEnemyCategories = new[] { "basic", "fast", "tank", "elite", "boss" };
            foreach (var category in requiredEnemyCategories)
            {
                if (!EnemyTypeRegistry.GetByCategory(category).Any())
                {
                    errors.Add($"Required enemy category '{category}' has no types");
                }
            }

            // Check for default types
            if (BuildingTypeRegistry.GetDefaultType() == null && BuildingTypeRegistry.GetCheapestType() == null)
            {
                errors.Add("No default or cheapest building type available for fallback");
            }

            if (EnemyTypeRegistry.GetDefaultType() == null && EnemyTypeRegistry.GetBasicType() == null)
            {
                errors.Add("No default or basic enemy type available for fallback");
            }

            // Validate enemy tier progression
            var enemyTiers = EnemyTypeRegistry.GetAllTiers().ToList();
            if (!enemyTiers.Contains(1))
            {
                errors.Add("Missing tier 1 enemies (required for early waves)");
            }
        }
        catch (Exception ex)
        {
            errors.Add($"Validation failed with exception: {ex.Message}");
        }

        return errors;
    }

    public void LogRegistryStatus()
    {
        Console.WriteLine("=== Type Registry Status ===");
        
        // Building Registry Status
        var buildings = BuildingTypeRegistry.GetAllTypes().ToList();
        var buildingCategories = BuildingTypeRegistry.GetAllCategories().ToList();
        Console.WriteLine($"Building Types: {buildings.Count} types across {buildingCategories.Count} categories");
        
        foreach (var category in buildingCategories.OrderBy(c => c))
        {
            var categoryBuildings = BuildingTypeRegistry.GetByCategory(category).ToList();
            Console.WriteLine($"  - {category}: {categoryBuildings.Count} buildings");
        }
        
        var defaultBuilding = BuildingTypeRegistry.GetDefaultType();
        var cheapestBuilding = BuildingTypeRegistry.GetCheapestType();
        Console.WriteLine($"  Default: {defaultBuilding?.DisplayName ?? "None"}");
        Console.WriteLine($"  Cheapest: {cheapestBuilding?.DisplayName ?? "None"}");
        
        // Enemy Registry Status
        var enemies = EnemyTypeRegistry.GetAllTypes().ToList();
        var enemyCategories = EnemyTypeRegistry.GetAllCategories().ToList();
        var enemyTiers = EnemyTypeRegistry.GetAllTiers().ToList();
        Console.WriteLine($"Enemy Types: {enemies.Count} types across {enemyCategories.Count} categories and {enemyTiers.Count} tiers");
        
        foreach (var category in enemyCategories.OrderBy(c => c))
        {
            var categoryEnemies = EnemyTypeRegistry.GetByCategory(category).ToList();
            Console.WriteLine($"  - {category}: {categoryEnemies.Count} enemies");
        }
        
        foreach (var tier in enemyTiers)
        {
            var tierEnemies = EnemyTypeRegistry.GetByTier(tier).ToList();
            Console.WriteLine($"  Tier {tier}: {tierEnemies.Count} enemies");
        }
        
        var defaultEnemy = EnemyTypeRegistry.GetDefaultType();
        var basicEnemy = EnemyTypeRegistry.GetBasicType();
        Console.WriteLine($"  Default: {defaultEnemy?.DisplayName ?? "None"}");
        Console.WriteLine($"  Basic: {basicEnemy?.DisplayName ?? "None"}");
        
        // Validation Status
        var errors = GetConfigurationErrors().ToList();
        Console.WriteLine($"Configuration Errors: {errors.Count}");
        foreach (var error in errors)
        {
            Console.WriteLine($"  - {error}");
        }
        
        Console.WriteLine("=============================");
    }
}
