using System;
using System.Text.Json;
using Game.Domain.Buildings.Services;
using Game.Domain.Enemies.Services;
using Game.Domain.Enemies.ValueObjects;
using Game.Domain.Buildings.ValueObjects;
using Game.Application.Buildings.Configuration;
using Game.Application.Enemies.Configuration;

namespace Game.Infrastructure.Stats;

public class StatsService : IBuildingStatsProvider, IEnemyStatsProvider
{
    private EnemyStatsConfig _enemyStats = null!;
    private BuildingStatsConfig _buildingStats = null!;
    
    private const string ENEMY_STATS_PATH = "res://config/entities/enemies/enemy_stats.json";
    private const string BUILDING_STATS_PATH = "res://config/entities/buildings/building_stats.json";

    public StatsService()
    {
        LoadConfigurations();
    }

    public EnemyStats GetEnemyStats(string enemyType)
    {
        var data = GetEnemyStatsData(enemyType);
        return ConvertToEnemyStats(data);
    }

    public BuildingStats GetBuildingStats(string buildingType)
    {
        var data = GetBuildingStatsData(buildingType);
        return ConvertToBuildingStats(data);
    }

    // Legacy methods for backward compatibility
    private EnemyStatsData GetEnemyStatsData(string enemyType)
    {
        if (_enemyStats?.enemy_types?.ContainsKey(enemyType) == true)
        {
            return _enemyStats.enemy_types[enemyType];
        }
        
        return _enemyStats?.default_stats ?? new EnemyStatsData();
    }

    private BuildingStatsData GetBuildingStatsData(string buildingType)
    {
        if (_buildingStats?.building_types?.ContainsKey(buildingType) == true)
        {
            return _buildingStats.building_types[buildingType];
        }
        
        return _buildingStats?.default_stats ?? new BuildingStatsData();
    }

    private EnemyStats ConvertToEnemyStats(EnemyStatsData data)
    {
        return new EnemyStats(
            maxHealth: data.max_health,
            speed: data.speed,
            damage: data.damage,
            rewardGold: data.reward_gold,
            rewardXp: data.reward_xp,
            description: data.description
        );
    }

    private BuildingStats ConvertToBuildingStats(BuildingStatsData data)
    {
        return new BuildingStats(
            cost: data.cost,
            damage: data.damage,
            range: data.range,
            attackSpeed: data.attack_speed,
            bulletSpeed: data.bullet_speed,
            shootSound: "",
            impactSound: "",
            description: data.description
        );
    }

    public bool HasEnemyStats(string enemyType)
    {
        return _enemyStats?.enemy_types?.ContainsKey(enemyType) == true;
    }

    public bool HasBuildingStats(string buildingType)
    {
        return _buildingStats?.building_types?.ContainsKey(buildingType) == true;
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
                
                _enemyStats = JsonSerializer.Deserialize<EnemyStatsConfig>(jsonContent, options) ?? new EnemyStatsConfig();
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
                
                _buildingStats = JsonSerializer.Deserialize<BuildingStatsConfig>(jsonContent, options) ?? new BuildingStatsConfig();
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
