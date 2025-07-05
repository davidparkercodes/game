using System;
using System.Collections.Generic;
using System.Linq;
using Game.Domain.Shared.Services;

namespace Game.Application.Shared.Services;

public class StartupValidationService
{
    private readonly ITypeManagementService _typeManagementService;

    public StartupValidationService(ITypeManagementService typeManagementService)
    {
        _typeManagementService = typeManagementService ?? throw new ArgumentNullException(nameof(typeManagementService));
    }

    public bool ValidateOnStartup()
    {
        Console.WriteLine("=== Starting Type Registry Validation ===");
        
        try
        {
            // Log current registry status
            _typeManagementService.LogRegistryStatus();
            
            // Perform validation
            var isValid = _typeManagementService.ValidateAllConfigurations();
            var errors = _typeManagementService.GetConfigurationErrors().ToList();
            
            if (isValid)
            {
                Console.WriteLine("✅ All type registries are valid and properly configured");
                return true;
            }
            else
            {
                Console.WriteLine($"❌ Found {errors.Count} configuration errors:");
                foreach (var error in errors)
                {
                    Console.WriteLine($"  - {error}");
                }
                
                Console.WriteLine("\n⚠️  Application may experience issues with hardcoded type fallbacks");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Startup validation failed with exception: {ex.Message}");
            return false;
        }
        finally
        {
            Console.WriteLine("=== Type Registry Validation Complete ===\n");
        }
    }

    public void ValidateSpecificConfigurations()
    {
        Console.WriteLine("=== Detailed Configuration Validation ===");
        
        try
        {
            ValidatePlacementStrategies();
            ValidateWaveProgression();
            ValidateRegistryIntegrity();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Detailed validation failed: {ex.Message}");
        }
        
        Console.WriteLine("=== Detailed Validation Complete ===\n");
    }

    private void ValidatePlacementStrategies()
    {
        Console.WriteLine("Validating placement strategies...");
        
        // Check if required building categories exist for placement strategies
        var requiredCategories = new[] { "starter", "precision", "rapid", "heavy" };
        var missingCategories = new List<string>();
        
        foreach (var category in requiredCategories)
        {
            var buildings = _typeManagementService.GetBuildingsByCategory(category).ToList();
            if (!buildings.Any())
            {
                missingCategories.Add(category);
            }
            else
            {
                Console.WriteLine($"  ✅ Category '{category}': {buildings.Count} buildings available");
            }
        }
        
        if (missingCategories.Any())
        {
            Console.WriteLine($"  ⚠️  Missing building categories: {string.Join(", ", missingCategories)}");
        }
        
        // Check fallback options
        var defaultBuilding = _typeManagementService.GetDefaultBuilding();
        var cheapestBuilding = _typeManagementService.GetCheapestBuilding();
        
        if (defaultBuilding != null)
        {
            Console.WriteLine($"  ✅ Default building fallback: {defaultBuilding.DisplayName}");
        }
        else if (cheapestBuilding != null)
        {
            Console.WriteLine($"  ⚠️  Using cheapest building fallback: {cheapestBuilding.DisplayName}");
        }
        else
        {
            Console.WriteLine("  ❌ No building fallback available");
        }
    }

    private void ValidateWaveProgression()
    {
        Console.WriteLine("Validating wave progression...");
        
        // Check if required enemy categories exist for wave progression
        var requiredCategories = new[] { "basic", "fast", "tank", "elite", "boss" };
        var missingCategories = new List<string>();
        
        foreach (var category in requiredCategories)
        {
            var enemies = _typeManagementService.GetEnemiesByCategory(category).ToList();
            if (!enemies.Any())
            {
                missingCategories.Add(category);
            }
            else
            {
                Console.WriteLine($"  ✅ Category '{category}': {enemies.Count} enemies available");
            }
        }
        
        if (missingCategories.Any())
        {
            Console.WriteLine($"  ⚠️  Missing enemy categories: {string.Join(", ", missingCategories)}");
        }
        
        // Test wave progression for first few waves
        Console.WriteLine("  Testing wave progression logic:");
        for (int wave = 1; wave <= 8; wave++)
        {
            var availableEnemies = _typeManagementService.GetEnemiesForWave(wave).ToList();
            var selectedEnemy = _typeManagementService.GetEnemyTypeForWave(wave, 0);
            
            Console.WriteLine($"    Wave {wave}: {availableEnemies.Count} available, selected: {selectedEnemy?.DisplayName ?? "None"}");
        }
        
        // Check fallback options
        var defaultEnemy = _typeManagementService.GetDefaultEnemy();
        var basicEnemy = _typeManagementService.GetBasicEnemy();
        
        if (defaultEnemy != null)
        {
            Console.WriteLine($"  ✅ Default enemy fallback: {defaultEnemy.DisplayName}");
        }
        else if (basicEnemy != null)
        {
            Console.WriteLine($"  ⚠️  Using basic enemy fallback: {basicEnemy.DisplayName}");
        }
        else
        {
            Console.WriteLine("  ❌ No enemy fallback available");
        }
    }

    private void ValidateRegistryIntegrity()
    {
        Console.WriteLine("Validating registry integrity...");
        
        // Check for duplicate config keys
        var buildingConfigKeys = _typeManagementService.BuildingTypeRegistry.GetAllTypes()
            .Select(t => t.ConfigKey).ToList();
        var duplicateBuildings = buildingConfigKeys
            .GroupBy(k => k)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key).ToList();
            
        if (duplicateBuildings.Any())
        {
            Console.WriteLine($"  ❌ Duplicate building config keys: {string.Join(", ", duplicateBuildings)}");
        }
        else
        {
            Console.WriteLine($"  ✅ Building config keys are unique ({buildingConfigKeys.Count} total)");
        }
        
        var enemyConfigKeys = _typeManagementService.EnemyTypeRegistry.GetAllTypes()
            .Select(t => t.ConfigKey).ToList();
        var duplicateEnemies = enemyConfigKeys
            .GroupBy(k => k)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key).ToList();
            
        if (duplicateEnemies.Any())
        {
            Console.WriteLine($"  ❌ Duplicate enemy config keys: {string.Join(", ", duplicateEnemies)}");
        }
        else
        {
            Console.WriteLine($"  ✅ Enemy config keys are unique ({enemyConfigKeys.Count} total)");
        }
        
        // Check for empty categories
        var emptyBuildingCategories = _typeManagementService.BuildingTypeRegistry.GetAllCategories()
            .Where(cat => !_typeManagementService.GetBuildingsByCategory(cat).Any()).ToList();
            
        var emptyEnemyCategories = _typeManagementService.EnemyTypeRegistry.GetAllCategories()
            .Where(cat => !_typeManagementService.GetEnemiesByCategory(cat).Any()).ToList();
            
        if (emptyBuildingCategories.Any())
        {
            Console.WriteLine($"  ⚠️  Empty building categories: {string.Join(", ", emptyBuildingCategories)}");
        }
        
        if (emptyEnemyCategories.Any())
        {
            Console.WriteLine($"  ⚠️  Empty enemy categories: {string.Join(", ", emptyEnemyCategories)}");
        }
        
        if (!emptyBuildingCategories.Any() && !emptyEnemyCategories.Any())
        {
            Console.WriteLine("  ✅ All categories have at least one type");
        }
    }
}
