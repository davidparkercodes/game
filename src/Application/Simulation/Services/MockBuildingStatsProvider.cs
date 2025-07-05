using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Game.Domain.Buildings.Services;
using Game.Domain.Buildings.ValueObjects;
using Game.Application.Buildings.Configuration;

namespace Game.Application.Simulation.Services;

public class MockBuildingStatsProvider : IBuildingStatsProvider
{
    private readonly Dictionary<string, BuildingStats> _buildingStats;
    private readonly BuildingStatsConfig _config;
    private readonly IBuildingTypeRegistry? _buildingTypeRegistry;
    private const string DEFAULT_CONFIG_PATH = "data/simulation/building-stats.json";

    public MockBuildingStatsProvider(IBuildingTypeRegistry? buildingTypeRegistry = null, string? configPath = null)
    {
        _buildingTypeRegistry = buildingTypeRegistry; // Allow null for backward compatibility
        
        var actualConfigPath = FindConfigFile(configPath ?? DEFAULT_CONFIG_PATH);
        Console.WriteLine($"DEBUG: Loading building stats from: {actualConfigPath}");
        _config = LoadBuildingStatsConfig(actualConfigPath);
        Console.WriteLine($"DEBUG: Loaded config with {_config.building_types?.Count ?? 0} building types");
        _buildingStats = ConvertToBuildingStats(_config.building_types);
        Console.WriteLine($"DEBUG: Converted to {_buildingStats.Count} building stats");
        foreach (var key in _buildingStats.Keys)
        {
            Console.WriteLine($"DEBUG: Available building type: {key}");
        }
    }

    public BuildingStats GetBuildingStats(string buildingType)
    {
        if (_buildingStats.TryGetValue(buildingType, out var stats))
        {
            return stats;
        }

        // Use BuildingTypeRegistry for intelligent fallbacks (if available)
        if (_buildingTypeRegistry != null)
        {
            // First try to get the default type from registry
            var defaultType = _buildingTypeRegistry.GetDefaultType();
            if (defaultType != null && _buildingStats.ContainsKey(defaultType.ConfigKey))
            {
                return _buildingStats[defaultType.ConfigKey];
            }
            
            // If no default, try to get the cheapest type as fallback
            var cheapestType = _buildingTypeRegistry.GetCheapestType();
            if (cheapestType != null && _buildingStats.ContainsKey(cheapestType.ConfigKey))
            {
                return _buildingStats[cheapestType.ConfigKey];
            }
        }
        else
        {
            // Fallback when no registry is available (backward compatibility)
            // Return default stats for unknown building types (fallback to basic_tower if available)
            if (_buildingStats.ContainsKey("basic_tower"))
            {
                return _buildingStats["basic_tower"];
            }
        }
        
        // If registry methods fail, return any available building type
        foreach (var availableStats in _buildingStats.Values)
        {
            return availableStats;
        }
        
        // This should never happen if config is valid, but provide a safe fallback
        throw new InvalidOperationException($"No building stats available for '{buildingType}' and no fallback found. Check config file: {DEFAULT_CONFIG_PATH}");
    }

    public bool HasBuildingStats(string buildingType)
    {
        return _buildingStats.ContainsKey(buildingType);
    }

    public void SetBuildingStats(string buildingType, BuildingStats stats)
    {
        _buildingStats[buildingType] = stats;
    }

    public void SetCostMultiplier(float multiplier)
    {
        var keys = new List<string>(_buildingStats.Keys);
        foreach (var key in keys)
        {
            var stats = _buildingStats[key];
            _buildingStats[key] = new BuildingStats(
                cost: (int)(stats.Cost * multiplier),
                damage: stats.Damage,
                range: stats.Range,
                fireRate: stats.FireRate,
                bulletSpeed: stats.BulletSpeed,
                shootSound: stats.ShootSound,
                impactSound: stats.ImpactSound,
                description: stats.Description
            );
        }
    }

    private static string FindConfigFile(string relativePath)
    {
        // First try the relative path as-is
        if (File.Exists(relativePath))
        {
            return relativePath;
        }
        
        // Try looking in common directories relative to current directory
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
            {
                return searchPath;
            }
        }
        
        // Return the original path if nothing found (will fail with clear error)
        return relativePath;
    }

    private static BuildingStatsConfig LoadBuildingStatsConfig(string configPath)
    {
        try
        {
            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException($"Building stats config file not found: {configPath}");
            }

            var jsonContent = File.ReadAllText(configPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };
            
            return JsonSerializer.Deserialize<BuildingStatsConfig>(jsonContent, options) ?? new BuildingStatsConfig();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load building stats from {configPath}: {ex.Message}", ex);
        }
    }

    private static Dictionary<string, BuildingStats> ConvertToBuildingStats(Dictionary<string, BuildingStatsData> rawStats)
    {
        var result = new Dictionary<string, BuildingStats>();
        
        foreach (var kvp in rawStats)
        {
            var raw = kvp.Value;
            Console.WriteLine($"DEBUG: Processing building '{kvp.Key}': range={raw.range}, attack_speed={raw.attack_speed}, cost={raw.cost}");
            
            // Skip invalid entries (like default_stats with zero values)
            if (raw.range <= 0 || raw.attack_speed <= 0)
            {
                Console.WriteLine($"DEBUG: Skipping '{kvp.Key}' due to invalid values (range={raw.range}, attack_speed={raw.attack_speed})");
                continue;
            }
            
            result[kvp.Key] = new BuildingStats(
                cost: raw.cost,
                damage: raw.damage,
                range: raw.range,
                fireRate: raw.attack_speed,
                bulletSpeed: raw.bullet_speed,
                shootSound: "",
                impactSound: "",
                description: raw.description
            );
        }
        
        return result;
    }
}
