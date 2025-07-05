using System;
using System.Linq;
using Game.Domain.Shared.Services;

namespace Game.Application.Shared.Services;

public class DebugCommands
{
    private readonly ITypeManagementService _typeManagementService;

    public DebugCommands(ITypeManagementService typeManagementService)
    {
        _typeManagementService = typeManagementService ?? throw new ArgumentNullException(nameof(typeManagementService));
    }

    public void ListAllBuildingTypes()
    {
        Console.WriteLine("=== Building Types Registry ===");
        
        var buildings = _typeManagementService.BuildingTypeRegistry.GetAllTypes().ToList();
        var categories = _typeManagementService.BuildingTypeRegistry.GetAllCategories().ToList();
        
        Console.WriteLine($"Total Building Types: {buildings.Count}");
        Console.WriteLine($"Total Categories: {categories.Count}");
        Console.WriteLine();
        
        // List by category
        foreach (var category in categories.OrderBy(c => c))
        {
            var categoryBuildings = _typeManagementService.GetBuildingsByCategory(category).ToList();
            Console.WriteLine($"Category '{category}': {categoryBuildings.Count} buildings");
            
            foreach (var building in categoryBuildings.OrderBy(b => b.ConfigKey))
            {
                Console.WriteLine($"  - {building.ConfigKey} ({building.InternalId}) -> \"{building.DisplayName}\"");
            }
            Console.WriteLine();
        }
        
        // Show default/fallback types
        var defaultBuilding = _typeManagementService.GetDefaultBuilding();
        var cheapestBuilding = _typeManagementService.GetCheapestBuilding();
        
        Console.WriteLine("Fallback Options:");
        Console.WriteLine($"  Default: {defaultBuilding?.DisplayName ?? "None"}");
        Console.WriteLine($"  Cheapest: {cheapestBuilding?.DisplayName ?? "None"}");
        Console.WriteLine("=================================");
    }

    public void ListAllEnemyTypes()
    {
        Console.WriteLine("=== Enemy Types Registry ===");
        
        var enemies = _typeManagementService.EnemyTypeRegistry.GetAllTypes().ToList();
        var categories = _typeManagementService.EnemyTypeRegistry.GetAllCategories().ToList();
        var tiers = _typeManagementService.EnemyTypeRegistry.GetAllTiers().ToList();
        
        Console.WriteLine($"Total Enemy Types: {enemies.Count}");
        Console.WriteLine($"Total Categories: {categories.Count}");
        Console.WriteLine($"Total Tiers: {tiers.Count}");
        Console.WriteLine();
        
        // List by category
        foreach (var category in categories.OrderBy(c => c))
        {
            var categoryEnemies = _typeManagementService.GetEnemiesByCategory(category).ToList();
            Console.WriteLine($"Category '{category}': {categoryEnemies.Count} enemies");
            
            foreach (var enemy in categoryEnemies.OrderBy(e => e.ConfigKey))
            {
                Console.WriteLine($"  - {enemy.ConfigKey} ({enemy.InternalId}) -> \"{enemy.DisplayName}\" [Tier {enemy.Tier}]");
            }
            Console.WriteLine();
        }
        
        // List by tier
        Console.WriteLine("By Tier:");
        foreach (var tier in tiers.OrderBy(t => t))
        {
            var tierEnemies = _typeManagementService.GetEnemiesByTier(tier).ToList();
            Console.WriteLine($"  Tier {tier}: {tierEnemies.Count} enemies ({string.Join(", ", tierEnemies.Select(e => e.ConfigKey))})");
        }
        Console.WriteLine();
        
        // Show default/fallback types
        var defaultEnemy = _typeManagementService.GetDefaultEnemy();
        var basicEnemy = _typeManagementService.GetBasicEnemy();
        
        Console.WriteLine("Fallback Options:");
        Console.WriteLine($"  Default: {defaultEnemy?.DisplayName ?? "None"}");
        Console.WriteLine($"  Basic: {basicEnemy?.DisplayName ?? "None"}");
        Console.WriteLine("=============================");
    }

    public void ValidateConfigConsistency()
    {
        Console.WriteLine("=== Configuration Consistency Validation ===");
        
        var validationService = new StartupValidationService(_typeManagementService);
        bool isValid = validationService.ValidateOnStartup();
        
        if (isValid)
        {
            Console.WriteLine("✅ All configurations are consistent and valid");
        }
        else
        {
            Console.WriteLine("❌ Configuration issues detected - see above for details");
        }
        
        // Run detailed validation
        validationService.ValidateSpecificConfigurations();
        
        Console.WriteLine("=============================================");
    }

    public void ShowWaveProgression(int maxWave = 10)
    {
        Console.WriteLine($"=== Wave Progression (Waves 1-{maxWave}) ===");
        
        for (int wave = 1; wave <= maxWave; wave++)
        {
            var availableEnemies = _typeManagementService.GetEnemiesForWave(wave).ToList();
            var selectedEnemy = _typeManagementService.GetEnemyTypeForWave(wave, 0);
            
            Console.WriteLine($"Wave {wave}:");
            Console.WriteLine($"  Available: {availableEnemies.Count} types ({string.Join(", ", availableEnemies.Select(e => e.ConfigKey))})");
            Console.WriteLine($"  Selected: {selectedEnemy?.ConfigKey ?? "None"}");
            Console.WriteLine();
        }
        
        Console.WriteLine("==========================================");
    }

    public void ShowAllDebugInfo()
    {
        Console.WriteLine("=== FULL DEBUG INFORMATION ===");
        Console.WriteLine();
        
        ListAllBuildingTypes();
        Console.WriteLine();
        
        ListAllEnemyTypes();
        Console.WriteLine();
        
        ShowWaveProgression();
        Console.WriteLine();
        
        ValidateConfigConsistency();
        
        Console.WriteLine("===============================");
    }
}
