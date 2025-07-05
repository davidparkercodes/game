using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Game.Domain.Buildings.ValueObjects;
using Game.Domain.Enemies.ValueObjects;

namespace Game.Application.Simulation.Services;

public static class ConfigLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    public static BuildingStatsConfig LoadBuildingStats(string configPath)
    {
        try
        {
            var actualPath = FindConfigFile(configPath);
            if (!File.Exists(actualPath))
            {
                throw new FileNotFoundException($"Building stats config file not found: {configPath} (searched: {actualPath})");
            }

            var jsonContent = File.ReadAllText(actualPath);
            var rawConfig = JsonSerializer.Deserialize<BuildingStatsConfigRaw>(jsonContent, JsonOptions);
            
            if (rawConfig?.Buildings == null)
            {
                throw new InvalidOperationException($"Invalid building stats config format in: {configPath}");
            }

            return new BuildingStatsConfig
            {
                Version = rawConfig.Version ?? "1.0",
                Description = rawConfig.Description ?? "",
                Buildings = ConvertBuildingStats(rawConfig.Buildings)
            };
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load building stats from {configPath}: {ex.Message}", ex);
        }
    }

    public static EnemyStatsConfig LoadEnemyStats(string configPath)
    {
        try
        {
            var actualPath = FindConfigFile(configPath);
            if (!File.Exists(actualPath))
            {
                throw new FileNotFoundException($"Enemy stats config file not found: {configPath} (searched: {actualPath})");
            }

            var jsonContent = File.ReadAllText(actualPath);
            var rawConfig = JsonSerializer.Deserialize<EnemyStatsConfigRaw>(jsonContent, JsonOptions);
            
            if (rawConfig?.Enemies == null)
            {
                throw new InvalidOperationException($"Invalid enemy stats config format in: {configPath}");
            }

            return new EnemyStatsConfig
            {
                Version = rawConfig.Version ?? "1.0",
                Description = rawConfig.Description ?? "",
                Enemies = ConvertEnemyStats(rawConfig.Enemies),
                WaveScaling = rawConfig.WaveScaling ?? new WaveScalingConfig()
            };
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load enemy stats from {configPath}: {ex.Message}", ex);
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

    private static Dictionary<string, BuildingStats> ConvertBuildingStats(Dictionary<string, BuildingStatsRaw> rawStats)
    {
        var result = new Dictionary<string, BuildingStats>();
        
        foreach (var kvp in rawStats)
        {
            var raw = kvp.Value;
            result[kvp.Key] = new BuildingStats(
                cost: raw.Cost,
                damage: raw.Damage,
                range: raw.Range,
                fireRate: raw.FireRate,
                bulletSpeed: raw.BulletSpeed,
                shootSound: raw.ShootSound ?? "",
                impactSound: raw.ImpactSound ?? "",
                description: raw.Description ?? ""
            );
        }
        
        return result;
    }

    private static Dictionary<string, EnemyStats> ConvertEnemyStats(Dictionary<string, EnemyStatsRaw> rawStats)
    {
        var result = new Dictionary<string, EnemyStats>();
        
        foreach (var kvp in rawStats)
        {
            var raw = kvp.Value;
            result[kvp.Key] = new EnemyStats(
                maxHealth: raw.MaxHealth,
                speed: raw.Speed,
                damage: raw.Damage,
                rewardGold: raw.RewardGold,
                rewardXp: raw.RewardXp,
                description: raw.Description ?? ""
            );
        }
        
        return result;
    }
}

// Configuration data structures
public class BuildingStatsConfig
{
    public string Version { get; set; } = "";
    public string Description { get; set; } = "";
    public Dictionary<string, BuildingStats> Buildings { get; set; } = new();
}

public class EnemyStatsConfig
{
    public string Version { get; set; } = "";
    public string Description { get; set; } = "";
    public Dictionary<string, EnemyStats> Enemies { get; set; } = new();
    public WaveScalingConfig WaveScaling { get; set; } = new();
}

public class WaveScalingConfig
{
    public float HealthPerWave { get; set; } = 0.15f;
    public float SpeedPerWave { get; set; } = 0.05f;
    public int DamageEveryNWaves { get; set; } = 3;
    public float RewardPerWave { get; set; } = 0.1f;
}

// Raw JSON structures (for deserialization)
internal class BuildingStatsConfigRaw
{
    public string Version { get; set; }
    public string Description { get; set; }
    public Dictionary<string, BuildingStatsRaw> Buildings { get; set; }
}

internal class BuildingStatsRaw
{
    public int Cost { get; set; }
    public int Damage { get; set; }
    public float Range { get; set; }
    public float FireRate { get; set; }
    public float BulletSpeed { get; set; }
    public string ShootSound { get; set; }
    public string ImpactSound { get; set; }
    public string Description { get; set; }
}

internal class EnemyStatsConfigRaw
{
    public string Version { get; set; }
    public string Description { get; set; }
    public Dictionary<string, EnemyStatsRaw> Enemies { get; set; }
    public WaveScalingConfig WaveScaling { get; set; }
}

internal class EnemyStatsRaw
{
    public int MaxHealth { get; set; }
    public float Speed { get; set; }
    public int Damage { get; set; }
    public int RewardGold { get; set; }
    public int RewardXp { get; set; }
    public string Description { get; set; }
}
