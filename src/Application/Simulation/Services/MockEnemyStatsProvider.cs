using System;
using System.Collections.Generic;
using System.Linq;
using Game.Domain.Enemies.Services;
using Game.Domain.Enemies.ValueObjects;

namespace Game.Application.Simulation.Services;

public class MockEnemyStatsProvider : IEnemyStatsProvider
{
    private readonly Dictionary<string, EnemyStats> _enemyStats;
    private readonly EnemyStatsConfig _config;
    private float _healthMultiplier = 1.0f;
    private float _speedMultiplier = 1.0f;
    private const string DEFAULT_CONFIG_PATH = "data/simulation/enemy-stats.json";

    public MockEnemyStatsProvider(string configPath = null)
    {
        var actualConfigPath = configPath ?? DEFAULT_CONFIG_PATH;
        _config = ConfigLoader.LoadEnemyStats(actualConfigPath);
        _enemyStats = new Dictionary<string, EnemyStats>(_config.Enemies);
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

        // Return default stats for unknown enemy types (fallback to basic_enemy if available)
        if (_enemyStats.ContainsKey("basic_enemy"))
        {
            var defaultStats = _enemyStats["basic_enemy"];
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
        // Get base stats from config or fallback
        EnemyStats baseStats;
        if (_enemyStats.TryGetValue(enemyType, out var stats))
        {
            baseStats = stats;
        }
        else if (_enemyStats.ContainsKey("basic_enemy"))
        {
            baseStats = _enemyStats["basic_enemy"];
        }
        else
        {
            baseStats = _enemyStats.Values.First();
        }
        
        // Use config-driven scaling values
        var scaling = _config.WaveScaling;
        var waveHealthMultiplier = 1.0f + (waveNumber - 1) * scaling.HealthPerWave;
        var waveSpeedMultiplier = 1.0f + (waveNumber - 1) * scaling.SpeedPerWave;
        
        return new EnemyStats(
            maxHealth: (int)(baseStats.MaxHealth * _healthMultiplier * waveHealthMultiplier),
            speed: baseStats.Speed * _speedMultiplier * waveSpeedMultiplier,
            damage: baseStats.Damage + (waveNumber - 1) / scaling.DamageEveryNWaves,
            rewardGold: (int)(baseStats.RewardGold * (1.0f + (waveNumber - 1) * scaling.RewardPerWave)),
            rewardXp: (int)(baseStats.RewardXp * (1.0f + (waveNumber - 1) * scaling.RewardPerWave)),
            description: baseStats.Description
        );
    }
}
