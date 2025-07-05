using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Game.Domain.Enemies.Services;
using Game.Domain.Enemies.ValueObjects;
using Game.Application.Enemies.Configuration;

namespace Game.Application.Enemies.Services;

public class EnemyTypeRegistry : IEnemyTypeRegistry
{
    private readonly Dictionary<string, EnemyType> _typesByInternalId;
    private readonly Dictionary<string, EnemyType> _typesByConfigKey;
    private readonly Dictionary<string, List<EnemyType>> _typesByCategory;
    private readonly Dictionary<int, List<EnemyType>> _typesByTier;
    private const string DEFAULT_CONFIG_PATH = "data/simulation/enemy-stats.json";

    public EnemyTypeRegistry()
    {
        _typesByInternalId = new Dictionary<string, EnemyType>();
        _typesByConfigKey = new Dictionary<string, EnemyType>();
        _typesByCategory = new Dictionary<string, List<EnemyType>>();
        _typesByTier = new Dictionary<int, List<EnemyType>>();
        
        LoadTypesFromConfig();
    }

    public EnemyType? GetByInternalId(string internalId)
    {
        return _typesByInternalId.TryGetValue(internalId, out var enemyType) ? enemyType : null;
    }

    public EnemyType? GetByConfigKey(string configKey)
    {
        return _typesByConfigKey.TryGetValue(configKey, out var enemyType) ? enemyType : null;
    }

    public IEnumerable<EnemyType> GetByCategory(string category)
    {
        return _typesByCategory.TryGetValue(category, out var types) ? types : Enumerable.Empty<EnemyType>();
    }

    public IEnumerable<EnemyType> GetByTier(int tier)
    {
        return _typesByTier.TryGetValue(tier, out var types) ? types : Enumerable.Empty<EnemyType>();
    }

    public EnemyType? GetDefaultType()
    {
        return _typesByInternalId.Values.FirstOrDefault(t => 
            GetMetadataForType(t.InternalId)?.is_default == true);
    }

    public EnemyType? GetBasicType()
    {
        return GetByCategory("basic").FirstOrDefault();
    }

    public bool IsValidConfigKey(string configKey)
    {
        return _typesByConfigKey.ContainsKey(configKey);
    }

    public bool IsValidInternalId(string internalId)
    {
        return _typesByInternalId.ContainsKey(internalId);
    }

    public IEnumerable<EnemyType> GetAllTypes()
    {
        return _typesByInternalId.Values;
    }

    public IEnumerable<string> GetAllCategories()
    {
        return _typesByCategory.Keys;
    }

    public IEnumerable<int> GetAllTiers()
    {
        return _typesByTier.Keys.OrderBy(x => x);
    }

    public IEnumerable<EnemyType> GetEnemiesForWave(int waveNumber)
    {
        // Get enemies that are available for this wave based on tier progression
        var availableEnemies = new List<EnemyType>();
        
        // Tier 1 enemies (basic, fast) available from wave 1
        if (waveNumber >= 1)
        {
            availableEnemies.AddRange(GetByTier(1));
        }
        
        // Tier 2 enemies (tank) available from wave 4
        if (waveNumber >= 4)
        {
            availableEnemies.AddRange(GetByTier(2));
        }
        
        // Tier 3 enemies (elite) available from wave 6
        if (waveNumber >= 6)
        {
            availableEnemies.AddRange(GetByTier(3));
        }
        
        // Tier 4 enemies (boss) available from wave 8
        if (waveNumber >= 8)
        {
            availableEnemies.AddRange(GetByTier(4));
        }
        
        return availableEnemies;
    }
    
    public EnemyType? GetEnemyTypeForWaveProgression(int waveNumber, int enemyIndex)
    {
        // Progressive enemy type introduction using tier-based logic
        var enemyCategory = waveNumber switch
        {
            >= 8 when enemyIndex == 0 => "boss",
            >= 6 when enemyIndex % 4 == 0 => "elite",
            >= 4 when enemyIndex % 3 == 0 => "tank",
            >= 2 when enemyIndex % 2 == 0 => "fast",
            _ => "basic"
        };

        var enemyType = GetByCategory(enemyCategory).FirstOrDefault();
        
        // Fallback to default enemy type if category not found
        if (enemyType.Equals(default(EnemyType)))
        {
            return GetDefaultType() ?? GetBasicType();
        }

        return enemyType;
    }
    
    public bool IsEnemyAvailableForWave(string configKey, int waveNumber)
    {
        var enemyType = GetByConfigKey(configKey);
        if (enemyType == null) return false;
        
        // Check tier-based availability
        return enemyType.Value.Tier switch
        {
            1 => waveNumber >= 1,  // Basic/Fast enemies
            2 => waveNumber >= 4,  // Tank enemies
            3 => waveNumber >= 6,  // Elite enemies
            4 => waveNumber >= 8,  // Boss enemies
            _ => false
        };
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
            
            var config = JsonSerializer.Deserialize<EnhancedEnemyStatsConfig>(jsonContent, options);
            
            if (config?.enemy_types_metadata?.registry == null)
            {
                throw new InvalidOperationException("Enemy types metadata not found in config file");
            }

            foreach (var kvp in config.enemy_types_metadata.registry)
            {
                var internalId = kvp.Key;
                var metadata = kvp.Value;
                
                var enemyType = new EnemyType(
                    internalId: internalId,
                    configKey: metadata.config_key,
                    displayName: metadata.display_name,
                    category: metadata.category,
                    tier: metadata.tier
                );
                
                _typesByInternalId[internalId] = enemyType;
                _typesByConfigKey[metadata.config_key] = enemyType;
                
                if (!_typesByCategory.ContainsKey(metadata.category))
                {
                    _typesByCategory[metadata.category] = new List<EnemyType>();
                }
                _typesByCategory[metadata.category].Add(enemyType);
                
                if (!_typesByTier.ContainsKey(metadata.tier))
                {
                    _typesByTier[metadata.tier] = new List<EnemyType>();
                }
                _typesByTier[metadata.tier].Add(enemyType);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load enemy types from config: {ex.Message}", ex);
        }
    }
    
    private EnemyTypeMetadata? GetMetadataForType(string internalId)
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
            
            var config = JsonSerializer.Deserialize<EnhancedEnemyStatsConfig>(jsonContent, options);
            return config?.enemy_types_metadata?.registry?.TryGetValue(internalId, out var metadata) == true ? metadata : null;
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
