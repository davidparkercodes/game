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
        _defaultEnemyStats = new EnemyStatsData(100, 60f, 10, 5, 10, "Default enemy");
        _enemyStats["basic_enemy"] = new EnemyStatsData(100, 60f, 10, 5, 10, "Basic enemy unit with standard stats");
        _enemyStats["fast_enemy"] = new EnemyStatsData(60, 90f, 8, 7, 12, "Fast but fragile enemy unit");
        _enemyStats["tank_enemy"] = new EnemyStatsData(200, 30f, 15, 12, 20, "Heavy armored enemy with high health");
        _enemyStats["elite_enemy"] = new EnemyStatsData(150, 75f, 20, 15, 25, "Elite enemy with balanced stats");
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
        _defaultBuildingStats = new BuildingStatsData(50, 10, 150f, 1.0f, 25, "Default building", 900, 12.0f);
        _buildingStats["basic_tower"] = new BuildingStatsData(50, 10, 120f, 1.2f, 25, "Basic tower with balanced stats", 900, 12.0f);
        _buildingStats["sniper_tower"] = new BuildingStatsData(75, 35, 200f, 2.5f, 40, "Long range, high damage tower with slow fire rate", 1200, 12.0f);
        _buildingStats["rapid_tower"] = new BuildingStatsData(75, 6, 100f, 0.4f, 35, "Fast firing tower with lower damage", 800, 12.0f);
        _buildingStats["heavy_tower"] = new BuildingStatsData(100, 50, 150f, 3.0f, 75, "Heavy damage tower with slow fire rate", 700, 16.0f);
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
}
