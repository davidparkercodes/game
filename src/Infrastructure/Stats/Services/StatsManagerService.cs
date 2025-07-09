using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Game.Domain.Enemies.ValueObjects;
using Game.Domain.Buildings.ValueObjects;
using Game.Application.Enemies.Configuration;
using Game.Application.Buildings.Configuration;
using Godot;

namespace Game.Infrastructure.Stats.Services;

public class StatsManagerService
{
    public static StatsManagerService Instance { get; private set; }
    
    private Dictionary<string, EnemyStatsData> _enemyStats;
    private EnemyStatsData _defaultEnemyStats;
    private Dictionary<string, BuildingStatsData> _buildingStats;
    private BuildingStatsData _defaultBuildingStats;
    private const string ENEMY_STATS_PATH = "res://data/stats/enemy_stats.json";
    private const string BUILDING_STATS_PATH = "res://data/stats/building_stats.json";

    static StatsManagerService()
    {
        Instance = new StatsManagerService();
    }
    
    private StatsManagerService()
    {
        _enemyStats = new Dictionary<string, EnemyStatsData>();
        _buildingStats = new Dictionary<string, BuildingStatsData>();
        LoadEnemyStats();
        LoadBuildingStats();
    }
    
    private void LoadEnemyStats()
    {
        try
        {
            if (IsInGodotRuntime() && Godot.FileAccess.FileExists(ENEMY_STATS_PATH))
            {
                using var file = Godot.FileAccess.Open(ENEMY_STATS_PATH, Godot.FileAccess.ModeFlags.Read);
                string jsonContent = file.GetAsText();
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };
                
                var config = JsonSerializer.Deserialize<EnemyStatsConfig>(jsonContent, options);
                
                if (config?.enemy_types != null)
                {
                    foreach (var kvp in config.enemy_types)
                    {
                        _enemyStats[kvp.Key] = kvp.Value;
                    }
                }
                
                _defaultEnemyStats = config?.default_stats ?? new EnemyStatsData(100, 60f, 10, 5, 10, "Default enemy");
                
                GD.Print($"✅ StatsManagerService: Loaded {_enemyStats.Count} enemy types from config");
            }
            else
            {
                GD.PrintErr($"⚠️ StatsManagerService: Enemy stats file not found at {ENEMY_STATS_PATH}, using fallback stats");
                LoadFallbackStats();
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"❌ StatsManagerService: Error loading enemy stats: {ex.Message}");
            LoadFallbackStats();
        }
    }
    
    private void LoadFallbackStats()
    {
        // NOTE: Hardcoded values here are ACCEPTABLE as emergency fallbacks when config files fail to load
        // These should match the values in data/stats/enemy_stats.json for consistency
        _defaultEnemyStats = new EnemyStatsData(100, 60f, 10, 5, 10, "Default enemy (fallback)");
        _enemyStats[Domain.Entities.EnemyConfigKeys.BasicEnemy] = new EnemyStatsData(100, 60f, 10, 5, 10, "Basic enemy (fallback)");
        _enemyStats[Domain.Entities.EnemyConfigKeys.FastEnemy] = new EnemyStatsData(60, 90f, 8, 7, 12, "Fast enemy (fallback)");
        _enemyStats[Domain.Entities.EnemyConfigKeys.TankEnemy] = new EnemyStatsData(200, 30f, 15, 12, 20, "Tank enemy (fallback)");
        _enemyStats[Domain.Entities.EnemyConfigKeys.EliteEnemy] = new EnemyStatsData(150, 75f, 20, 15, 25, "Elite enemy (fallback)");
    }
    
    private static bool IsInGodotRuntime()
    {
        try
        {
            return Godot.Engine.IsEditorHint() || !Godot.Engine.IsEditorHint();
        }
        catch
        {
            return false;
        }
    }

    public EnemyStatsData GetEnemyStats(string enemyType)
    {
        if (_enemyStats.TryGetValue(enemyType, out var stats))
        {
            return stats;
        }
        
        GD.PrintErr($"⚠️ StatsManagerService: Enemy type '{enemyType}' not found, using default stats");
        return _defaultEnemyStats;
    }

    private void LoadBuildingStats()
    {
        
        try
        {
            if (IsInGodotRuntime() && Godot.FileAccess.FileExists(BUILDING_STATS_PATH))
            {
                using var file = Godot.FileAccess.Open(BUILDING_STATS_PATH, Godot.FileAccess.ModeFlags.Read);
                string jsonContent = file.GetAsText();
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };
                
                var config = JsonSerializer.Deserialize<BuildingStatsConfig>(jsonContent, options);
                
                if (config?.building_types != null)
                {
                    foreach (var kvp in config.building_types)
                    {
                        _buildingStats[kvp.Key] = kvp.Value;
                    }
                }
                
                _defaultBuildingStats = config?.default_stats ?? new BuildingStatsData(50, 10, 150f, 1.0f, 25, "Default building", 900, 12.0f);
                
                GD.Print($"✅ StatsManagerService: Loaded {_buildingStats.Count} building types from config");
            }
            else
            {
                GD.PrintErr($"⚠️ StatsManagerService: Building stats file not found at {BUILDING_STATS_PATH}, using fallback stats");
                LoadFallbackBuildingStats();
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"❌ StatsManagerService: Error loading building stats: {ex.Message}");
            LoadFallbackBuildingStats();
        }
    }
    
    private void LoadFallbackBuildingStats()
    {
        // NOTE: Hardcoded values here are ACCEPTABLE as emergency fallbacks when config files fail to load
        // These should match the values in data/stats/building_stats.json for consistency
        _defaultBuildingStats = new BuildingStatsData(50, 10, 150f, 1.0f, 25, "Default building (fallback)", 900, 12.0f);
        _buildingStats[Domain.Entities.BuildingConfigKeys.BasicTower] = new BuildingStatsData(50, 10, 120f, 1.2f, 25, "Basic tower (fallback)", 900, 12.0f);
        _buildingStats[Domain.Entities.BuildingConfigKeys.SniperTower] = new BuildingStatsData(75, 35, 200f, 2.5f, 40, "Sniper tower (fallback)", 1200, 12.0f);
        _buildingStats[Domain.Entities.BuildingConfigKeys.RapidTower] = new BuildingStatsData(75, 6, 100f, 0.4f, 35, "Rapid tower (fallback)", 800, 12.0f);
        _buildingStats[Domain.Entities.BuildingConfigKeys.HeavyTower] = new BuildingStatsData(100, 50, 150f, 3.0f, 75, "Heavy tower (fallback)", 700, 16.0f);
    }

    public BuildingStatsData GetBuildingStats(string buildingType)
    {
        if (_buildingStats.TryGetValue(buildingType, out var stats))
        {
            return stats;
        }
        
        GD.PrintErr($"⚠️ StatsManagerService: Building type '{buildingType}' not found, using default stats");
        return _defaultBuildingStats;
    }

    public EnemyStatsData GetDefaultEnemyStats()
    {
        return _defaultEnemyStats;
    }

    public BuildingStatsData GetDefaultBuildingStats()
    {
        return _defaultBuildingStats;
    }

    public bool HasEnemyType(string enemyType)
    {
        return _enemyStats.ContainsKey(enemyType);
    }

    public bool HasBuildingType(string buildingType)
    {
        return _buildingStats.ContainsKey(buildingType);
    }

    public void ReloadConfigurations()
    {
        LoadEnemyStats();
        LoadBuildingStats();
    }
    
    public (int damage, float range, float attackSpeed) GetUpgradeStats(string towerType, int currentLevel)
    {
        if (!_buildingStats.TryGetValue(towerType, out var stats))
        {
            GD.PrintErr($"⚠️ StatsManagerService: Tower type '{towerType}' not found for upgrade stats");
            return (0, 0, 0);
        }
        
        // Calculate upgrade multiplier for the next level
        float upgradeMultiplier = GetUpgradeMultiplier(towerType);
        float totalMultiplier = 1.0f + (upgradeMultiplier - 1.0f) * (currentLevel + 1);
        
        return (
            damage: (int)(stats.damage * totalMultiplier),
            range: stats.range * totalMultiplier,
            attackSpeed: stats.attack_speed * totalMultiplier
        );
    }
    
    public int GetUpgradeCost(string towerType, int currentLevel)
    {
        if (!_buildingStats.TryGetValue(towerType, out var stats))
        {
            GD.PrintErr($"⚠️ StatsManagerService: Tower type '{towerType}' not found for upgrade cost");
            return 0;
        }
        
        // Calculate upgrade cost based on base upgrade cost and current level
        // Each level increases cost by 50% (configurable)
        int baseCost = stats.upgrade_cost;
        float multiplier = 1.0f + (currentLevel * 0.5f);
        
        return (int)(baseCost * multiplier);
    }
    
    public float GetUpgradeMultiplier(string towerType)
    {
        // TODO: Load from configuration when upgrade_multiplier is added to BuildingStatsData
        // For now, use default multiplier of 1.5 (50% improvement)
        return 1.5f;
    }
    
    public float GetSellPercentage(string towerType)
    {
        // TODO: Load from configuration when sell_percentage is added to BuildingStatsData
        // For now, use default sell percentage of 75%
        return 0.75f;
    }
    
    public int GetMaxUpgradeLevel(string towerType)
    {
        // TODO: Load from configuration when max_upgrade_level is added to BuildingStatsData
        // For now, use default max level of 3
        return 3;
    }
}
