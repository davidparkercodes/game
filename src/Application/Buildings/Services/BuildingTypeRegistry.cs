using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Game.Domain.Buildings.Services;
using Game.Domain.Buildings.ValueObjects;
using Game.Application.Buildings.Configuration;

namespace Game.Application.Buildings.Services;

public class BuildingTypeRegistry : IBuildingTypeRegistry
{
    private readonly Dictionary<string, BuildingType> _typesByInternalId;
    private readonly Dictionary<string, BuildingType> _typesByConfigKey;
    private readonly Dictionary<string, List<BuildingType>> _typesByCategory;
    private readonly IBuildingStatsProvider _buildingStatsProvider;
    private const string DEFAULT_CONFIG_PATH = "data/simulation/building-stats.json";

    public BuildingTypeRegistry(IBuildingStatsProvider buildingStatsProvider)
    {
        _buildingStatsProvider = buildingStatsProvider ?? throw new ArgumentNullException(nameof(buildingStatsProvider));
        _typesByInternalId = new Dictionary<string, BuildingType>();
        _typesByConfigKey = new Dictionary<string, BuildingType>();
        _typesByCategory = new Dictionary<string, List<BuildingType>>();
        
        LoadTypesFromConfig();
    }

    public BuildingType? GetByInternalId(string internalId)
    {
        return _typesByInternalId.TryGetValue(internalId, out var buildingType) ? buildingType : null;
    }

    public BuildingType? GetByConfigKey(string configKey)
    {
        return _typesByConfigKey.TryGetValue(configKey, out var buildingType) ? buildingType : null;
    }

    public IEnumerable<BuildingType> GetByCategory(string category)
    {
        return _typesByCategory.TryGetValue(category, out var types) ? types : Enumerable.Empty<BuildingType>();
    }

    public BuildingType? GetDefaultType()
    {
        return _typesByInternalId.Values.FirstOrDefault(t => 
            GetMetadataForType(t.InternalId)?.is_default == true);
    }

    public BuildingType? GetCheapestType()
    {
        var cheapestConfigKey = _typesByConfigKey.Keys
            .Select(key => new { Key = key, Stats = _buildingStatsProvider.GetBuildingStats(key) })
            .Where(x => x.Stats.Cost > 0)
            .OrderBy(x => x.Stats.Cost)
            .FirstOrDefault()?.Key;
            
        return cheapestConfigKey != null ? GetByConfigKey(cheapestConfigKey) : null;
    }

    public bool IsValidConfigKey(string configKey)
    {
        return _typesByConfigKey.ContainsKey(configKey);
    }

    public bool IsValidInternalId(string internalId)
    {
        return _typesByInternalId.ContainsKey(internalId);
    }

    public IEnumerable<BuildingType> GetAllTypes()
    {
        return _typesByInternalId.Values;
    }

    public IEnumerable<string> GetAllCategories()
    {
        return _typesByCategory.Keys;
    }

    public IEnumerable<BuildingType> GetAllByTier(int tier)
    {
        // Since building types don't currently have tier support, filter by cost as a proxy
        // This could be enhanced later when building tiers are added to metadata
        var tierThresholds = new Dictionary<int, (int min, int max)>
        {
            { 1, (0, 50) },     // Starter buildings
            { 2, (51, 150) },   // Intermediate buildings  
            { 3, (151, 300) },  // Advanced buildings
            { 4, (301, int.MaxValue) } // High-end buildings
        };
        
        if (!tierThresholds.ContainsKey(tier))
        {
            return Enumerable.Empty<BuildingType>();
        }
        
        var (minCost, maxCost) = tierThresholds[tier];
        
        return _typesByConfigKey.Values
            .Select(buildingType => new { 
                Type = buildingType, 
                Stats = _buildingStatsProvider.GetBuildingStats(buildingType.ConfigKey) 
            })
            .Where(x => x.Stats.Cost >= minCost && x.Stats.Cost <= maxCost)
            .Select(x => x.Type);
    }

    private void LoadTypesFromConfig()
    {
        try
        {
            var configPath = FindConfigFile(DEFAULT_CONFIG_PATH);
            var jsonContent = File.ReadAllText(configPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };
            
            var config = JsonSerializer.Deserialize<EnhancedBuildingStatsConfig>(jsonContent, options);
            
            if (config?.building_types_metadata?.registry == null)
            {
                throw new InvalidOperationException("Building types metadata not found in config file");
            }

            foreach (var kvp in config.building_types_metadata.registry)
            {
                var internalId = kvp.Key;
                var metadata = kvp.Value;
                
                var buildingType = new BuildingType(
                    internalId: internalId,
                    configKey: metadata.config_key,
                    displayName: metadata.display_name,
                    category: metadata.category
                );
                
                _typesByInternalId[internalId] = buildingType;
                _typesByConfigKey[metadata.config_key] = buildingType;
                
                if (!_typesByCategory.ContainsKey(metadata.category))
                {
                    _typesByCategory[metadata.category] = new List<BuildingType>();
                }
                _typesByCategory[metadata.category].Add(buildingType);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load building types from config: {ex.Message}", ex);
        }
    }
    
    private BuildingTypeMetadata? GetMetadataForType(string internalId)
    {
        try
        {
            var configPath = FindConfigFile(DEFAULT_CONFIG_PATH);
            var jsonContent = File.ReadAllText(configPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };
            
            var config = JsonSerializer.Deserialize<EnhancedBuildingStatsConfig>(jsonContent, options);
            return config?.building_types_metadata?.registry?.TryGetValue(internalId, out var metadata) == true ? metadata : null;
        }
        catch
        {
            return null;
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
