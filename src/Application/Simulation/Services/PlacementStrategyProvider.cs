using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Game.Application.Simulation.Configuration;
using Game.Domain.Buildings.Services;
using Game.Domain.Shared.ValueObjects;

namespace Game.Application.Simulation.Services;

public class PlacementStrategyProvider : IPlacementStrategyProvider
{
    private readonly PlacementStrategyConfig _config;
    private readonly IBuildingTypeRegistry _buildingTypeRegistry;
    private const string DEFAULT_CONFIG_PATH = "data/simulation/placement_strategies.json";

    public PlacementStrategyProvider(IBuildingTypeRegistry buildingTypeRegistry, string? configPath = null)
    {
        _buildingTypeRegistry = buildingTypeRegistry ?? throw new ArgumentNullException(nameof(buildingTypeRegistry));
        _config = LoadPlacementStrategyConfig(configPath ?? DEFAULT_CONFIG_PATH);
    }

    public string GetInitialBuildingCategory()
    {
        return _config.strategies.initial_wave.building_category;
    }

    public IEnumerable<Position> GetInitialBuildingPositions()
    {
        return _config.strategies.initial_wave.positions
            .Select(pos => new Position(pos[0], pos[1]));
    }

    public int GetMaxCostPerBuilding()
    {
        return _config.strategies.initial_wave.max_cost_per_building;
    }

    public string? GetUpgradeBuildingCategory(int waveNumber, int availableMoney)
    {
        var waveKey = $"wave_{waveNumber}";
        
        if (_config.strategies.wave_upgrades.TryGetValue(waveKey, out var upgradeConfig))
        {
            if (availableMoney >= upgradeConfig.cost_threshold)
            {
                return upgradeConfig.category;
            }
        }
        
        return null;
    }

    public Position? GetUpgradeBuildingPosition(int waveNumber)
    {
        var waveKey = $"wave_{waveNumber}";
        
        if (_config.strategies.wave_upgrades.TryGetValue(waveKey, out var upgradeConfig))
        {
            if (upgradeConfig.position.Count >= 2)
            {
                return new Position(upgradeConfig.position[0], upgradeConfig.position[1]);
            }
        }
        
        return null;
    }

    public string GetFallbackBuildingType()
    {
        if (_config.fallback_strategy.use_default_type)
        {
            var defaultType = _buildingTypeRegistry.GetDefaultType();
            if (defaultType != null)
            {
                return defaultType.ConfigKey;
            }
        }
        
        if (_config.fallback_strategy.use_cheapest_type)
        {
            var cheapestType = _buildingTypeRegistry.GetCheapestType();
            if (cheapestType != null)
            {
                return cheapestType.ConfigKey;
            }
        }
        
        return _config.fallback_strategy.emergency_fallback;
    }

    private static PlacementStrategyConfig LoadPlacementStrategyConfig(string configPath)
    {
        try
        {
            var actualPath = FindConfigFile(configPath);
            if (!File.Exists(actualPath))
            {
                throw new FileNotFoundException($"Placement strategy config file not found: {actualPath}");
            }

            var jsonContent = File.ReadAllText(actualPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };
            
            return JsonSerializer.Deserialize<PlacementStrategyConfig>(jsonContent, options) 
                   ?? new PlacementStrategyConfig();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load placement strategy from {configPath}: {ex.Message}", ex);
        }
    }

    private static string FindConfigFile(string relativePath)
    {
        if (File.Exists(relativePath))
            return relativePath;
        
        var searchPaths = new[]
        {
            relativePath,
            Path.Combine("..", relativePath),
            Path.Combine("..", "..", relativePath),
            Path.Combine("..", "..", "..", relativePath),
            Path.Combine(Environment.CurrentDirectory, relativePath),
        };
        
        foreach (var searchPath in searchPaths)
        {
            if (File.Exists(searchPath))
                return searchPath;
        }
        
        return relativePath;
    }
}
