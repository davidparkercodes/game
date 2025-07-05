using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Game.Domain.Enemies.Services;
using Game.Domain.Enemies.ValueObjects;
using Game.Application.Enemies.Configuration;
using Game.Application.Enemies.Services;

namespace Game.Application.Simulation.Services;

public class MockEnemyStatsProvider : IEnemyStatsProvider
{
    private readonly Dictionary<string, EnemyStats> _enemyStats;
    private readonly EnemyStatsConfig _config;
    private float _healthMultiplier = 1.0f;
    private float _speedMultiplier = 1.0f;
    private const string DEFAULT_CONFIG_PATH = "data/simulation/enemy-stats.json";
    
    public IEnemyTypeRegistry EnemyTypeRegistry { get; private set; }

    public MockEnemyStatsProvider(string configPath = null)
    {
        var actualConfigPath = FindConfigFile(configPath ?? DEFAULT_CONFIG_PATH);
        _config = LoadEnemyStatsConfig(actualConfigPath);
        _enemyStats = ConvertToEnemyStats(_config.enemy_types);
        EnemyTypeRegistry = new EnemyTypeRegistry();
    }

    public EnemyStats GetEnemyStats(string enemyType)
    {
        if (_enemyStats.TryGetValue(enemyType, out var stats))
        {
            // Apply multipliers for difficulty scaling
            return new EnemyStats(
                maxHealth: (int)(stats.MaxHealth * _healthMultiplier),
                speed: stats.Speed * _speedMultiplier,
                damage: stats.Damage,
                rewardGold: stats.RewardGold,
                rewardXp: stats.RewardXp,
                description: stats.Description
            );
        }

        // Return default stats for unknown enemy types using EnemyTypeRegistry
        var defaultType = EnemyTypeRegistry.GetDefaultType() ?? EnemyTypeRegistry.GetBasicType();
        if (defaultType != null && _enemyStats.ContainsKey(defaultType.ConfigKey))
        {
            var defaultStats = _enemyStats[defaultType.ConfigKey];
            return new EnemyStats(
                maxHealth: (int)(defaultStats.MaxHealth * _healthMultiplier),
                speed: defaultStats.Speed * _speedMultiplier,
                damage: defaultStats.Damage,
                rewardGold: defaultStats.RewardGold,
                rewardXp: defaultStats.RewardXp,
                description: defaultStats.Description
            );
        }
        
        // If no basic_enemy, return the first available enemy type
        foreach (var availableStats in _enemyStats.Values)
        {
            return new EnemyStats(
                maxHealth: (int)(availableStats.MaxHealth * _healthMultiplier),
                speed: availableStats.Speed * _speedMultiplier,
                damage: availableStats.Damage,
                rewardGold: availableStats.RewardGold,
                rewardXp: availableStats.RewardXp,
                description: availableStats.Description
            );
        }
        
        // This should never happen if config is valid, but provide a safe fallback
        throw new InvalidOperationException($"No enemy stats available for '{enemyType}' and no fallback found. Check config file: {DEFAULT_CONFIG_PATH}");
    }

    public bool HasEnemyStats(string enemyType)
    {
        return _enemyStats.ContainsKey(enemyType);
    }

    public void SetHealthMultiplier(float multiplier)
    {
        _healthMultiplier = multiplier;
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        _speedMultiplier = multiplier;
    }

    public void SetEnemyStats(string enemyType, EnemyStats stats)
    {
        _enemyStats[enemyType] = stats;
    }

    public EnemyStats GetScaledStatsForWave(string enemyType, int waveNumber)
    {
        // Get base stats from config or fallback using EnemyTypeRegistry
        EnemyStats baseStats;
        if (_enemyStats.TryGetValue(enemyType, out var stats))
        {
            baseStats = stats;
        }
        else
        {
            var defaultType = EnemyTypeRegistry.GetDefaultType() ?? EnemyTypeRegistry.GetBasicType();
            if (defaultType != null && _enemyStats.ContainsKey(defaultType.ConfigKey))
            {
                baseStats = _enemyStats[defaultType.ConfigKey];
            }
            else
            {
                baseStats = _enemyStats.Values.First();
            }
        }
        
        // Use config-driven scaling values (for now use hardcoded values since we removed WaveScaling)
        var waveHealthMultiplier = 1.0f + (waveNumber - 1) * 0.15f;
        var waveSpeedMultiplier = 1.0f + (waveNumber - 1) * 0.05f;
        
        return new EnemyStats(
            maxHealth: (int)(baseStats.MaxHealth * _healthMultiplier * waveHealthMultiplier),
            speed: baseStats.Speed * _speedMultiplier * waveSpeedMultiplier,
            damage: baseStats.Damage + (waveNumber - 1) / 3,
            rewardGold: (int)(baseStats.RewardGold * (1.0f + (waveNumber - 1) * 0.1f)),
            rewardXp: (int)(baseStats.RewardXp * (1.0f + (waveNumber - 1) * 0.1f)),
            description: baseStats.Description
        );
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

    private static EnemyStatsConfig LoadEnemyStatsConfig(string configPath)
    {
        try
        {
            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException($"Enemy stats config file not found: {configPath}");
            }

            var jsonContent = File.ReadAllText(configPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };
            
            return JsonSerializer.Deserialize<EnemyStatsConfig>(jsonContent, options) ?? new EnemyStatsConfig();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load enemy stats from {configPath}: {ex.Message}", ex);
        }
    }

    private static Dictionary<string, EnemyStats> ConvertToEnemyStats(Dictionary<string, EnemyStatsData> rawStats)
    {
        var result = new Dictionary<string, EnemyStats>();
        
        foreach (var kvp in rawStats)
        {
            var raw = kvp.Value;
            
            // Skip invalid entries (like default_stats with zero values)
            if (raw.max_health <= 0 || raw.speed <= 0)
            {
                continue;
            }
            
            result[kvp.Key] = new EnemyStats(
                maxHealth: raw.max_health,
                speed: raw.speed,
                damage: raw.damage,
                rewardGold: raw.reward_gold,
                rewardXp: raw.reward_xp,
                description: raw.description
            );
        }
        
        return result;
    }
}
