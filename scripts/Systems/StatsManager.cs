using Godot;
using System;
using System.Text.Json;

public partial class StatsManager : Node
{
    public static StatsManager Instance { get; private set; }
    
    public EnemyStatsConfig _enemyStats;
    public BuildingStatsConfig _buildingStats;
    
    private const string ENEMY_STATS_PATH = "res://data/stats/enemy_stats.json";
    private const string BUILDING_STATS_PATH = "res://data/stats/building_stats.json";

    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadConfigurations();
        }
        else
        {
            QueueFree();
        }
    }

    private void LoadConfigurations()
    {
        LoadEnemyStats();
        LoadBuildingStats();
        GD.Print("📊 StatsManager: All stat configurations loaded");
    }

    private void LoadEnemyStats()
    {
        try
        {
            if (FileAccess.FileExists(ENEMY_STATS_PATH))
            {
                using var file = FileAccess.Open(ENEMY_STATS_PATH, FileAccess.ModeFlags.Read);
                string jsonContent = file.GetAsText();
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };
                
                _enemyStats = JsonSerializer.Deserialize<EnemyStatsConfig>(jsonContent, options);
                
                GD.Print($"✅ Enemy stats loaded: {_enemyStats.enemy_types.Count} enemy types");
            }
            else
            {
                GD.PrintErr($"❌ Enemy stats file not found: {ENEMY_STATS_PATH}");
                _enemyStats = new EnemyStatsConfig();
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"❌ Failed to load enemy stats: {e.Message}");
            _enemyStats = new EnemyStatsConfig();
        }
    }

    private void LoadBuildingStats()
    {
        try
        {
            if (FileAccess.FileExists(BUILDING_STATS_PATH))
            {
                using var file = FileAccess.Open(BUILDING_STATS_PATH, FileAccess.ModeFlags.Read);
                string jsonContent = file.GetAsText();
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };
                
                _buildingStats = JsonSerializer.Deserialize<BuildingStatsConfig>(jsonContent, options);
                
                GD.Print($"✅ Building stats loaded: {_buildingStats.building_types.Count} building types");
            }
            else
            {
                GD.PrintErr($"❌ Building stats file not found: {BUILDING_STATS_PATH}");
                _buildingStats = new BuildingStatsConfig();
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"❌ Failed to load building stats: {e.Message}");
            _buildingStats = new BuildingStatsConfig();
        }
    }

    // Enemy stats methods
    public EnemyStatsData GetEnemyStats(string enemyType)
    {
        if (_enemyStats?.enemy_types?.ContainsKey(enemyType) == true)
        {
            return _enemyStats.enemy_types[enemyType];
        }
        
        GD.PrintErr($"⚠️ Enemy type '{enemyType}' not found, using default stats");
        return _enemyStats?.default_stats ?? new EnemyStatsData();
    }

    public EnemyStatsData GetDefaultEnemyStats()
    {
        return _enemyStats?.default_stats ?? new EnemyStatsData();
    }

    // Building stats methods
    public BuildingStatsData GetBuildingStats(string buildingType)
    {
        if (_buildingStats?.building_types?.ContainsKey(buildingType) == true)
        {
            return _buildingStats.building_types[buildingType];
        }
        
        GD.PrintErr($"⚠️ Building type '{buildingType}' not found, using default stats");
        return _buildingStats?.default_stats ?? new BuildingStatsData();
    }

    public BuildingStatsData GetDefaultBuildingStats()
    {
        return _buildingStats?.default_stats ?? new BuildingStatsData();
    }

    // Utility methods
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
        GD.Print("🔄 Reloading stat configurations...");
        LoadConfigurations();
    }
}
