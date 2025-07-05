using System;
using System.Text.Json;
using Game.Infrastructure.Interfaces;

namespace Game.Infrastructure.Stats;

public class StatsService : IStatsService
{
    private EnemyStatsConfig _enemyStats;
    private BuildingStatsConfig _buildingStats;
    
    private const string ENEMY_STATS_PATH = "res://data/stats/enemy_stats.json";
    private const string BUILDING_STATS_PATH = "res://data/stats/building_stats.json";

    public StatsService()
    {
        LoadConfigurations();
    }

    public EnemyStatsData GetEnemyStats(string enemyType)
    {
        if (_enemyStats?.enemy_types?.ContainsKey(enemyType) == true)
        {
            return _enemyStats.enemy_types[enemyType];
        }
        
        return _enemyStats?.default_stats ?? new EnemyStatsData();
    }

    public EnemyStatsData GetDefaultEnemyStats()
    {
        return _enemyStats?.default_stats ?? new EnemyStatsData();
    }

    public BuildingStatsData GetBuildingStats(string buildingType)
    {
        if (_buildingStats?.building_types?.ContainsKey(buildingType) == true)
        {
            return _buildingStats.building_types[buildingType];
        }
        
        return _buildingStats?.default_stats ?? new BuildingStatsData();
    }

    public BuildingStatsData GetDefaultBuildingStats()
    {
        return _buildingStats?.default_stats ?? new BuildingStatsData();
    }

    public bool HasEnemyType(string enemyType)
    {
        return _enemyStats?.enemy_types?.ContainsKey(enemyType) == true;
    }

    public bool HasBuildingType(string buildingType)
    {
        return _buildingStats?.building_types?.ContainsKey(buildingType) == true;
    }

    public void ReloadConfigurations()
    {
        LoadConfigurations();
    }

    private void LoadConfigurations()
    {
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
                
                _enemyStats = JsonSerializer.Deserialize<EnemyStatsConfig>(jsonContent, options);
            }
            else
            {
                _enemyStats = new EnemyStatsConfig();
            }
        }
        catch (Exception)
        {
            _enemyStats = new EnemyStatsConfig();
        }
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
                
                _buildingStats = JsonSerializer.Deserialize<BuildingStatsConfig>(jsonContent, options);
            }
            else
            {
                _buildingStats = new BuildingStatsConfig();
            }
        }
        catch (Exception)
        {
            _buildingStats = new BuildingStatsConfig();
        }
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
}
