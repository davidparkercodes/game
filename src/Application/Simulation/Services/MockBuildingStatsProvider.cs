using System;
using System.Collections.Generic;
using System.IO;
using Game.Domain.Buildings.Services;
using Game.Domain.Buildings.ValueObjects;

namespace Game.Application.Simulation.Services;

public class MockBuildingStatsProvider : IBuildingStatsProvider
{
    private readonly Dictionary<string, BuildingStats> _buildingStats;
    private readonly BuildingStatsConfig _config;
    private const string DEFAULT_CONFIG_PATH = "data/simulation/building-stats.json";

    public MockBuildingStatsProvider(string configPath = null)
    {
        var actualConfigPath = configPath ?? DEFAULT_CONFIG_PATH;
        _config = ConfigLoader.LoadBuildingStats(actualConfigPath);
        _buildingStats = new Dictionary<string, BuildingStats>(_config.Buildings);
    }

    public BuildingStats GetBuildingStats(string buildingType)
    {
        if (_buildingStats.TryGetValue(buildingType, out var stats))
        {
            return stats;
        }

        // Return default stats for unknown building types (fallback to basic_tower if available)
        if (_buildingStats.ContainsKey("basic_tower"))
        {
            return _buildingStats["basic_tower"];
        }
        
        // If no basic_tower, return the first available building type
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
}
