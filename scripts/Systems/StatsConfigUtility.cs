using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

public static class StatsConfigUtility
{
    /// <summary>
    /// Validates that all required fields are present in enemy stats
    /// </summary>
    public static bool ValidateEnemyStats(EnemyStatsData stats, string enemyType)
    {
        var errors = new List<string>();
        
        if (stats.max_health <= 0)
            errors.Add($"max_health must be > 0 (got {stats.max_health})");
            
        if (stats.speed < 0)
            errors.Add($"speed must be >= 0 (got {stats.speed})");
            
        if (stats.damage < 0)
            errors.Add($"damage must be >= 0 (got {stats.damage})");
            
        if (stats.reward_gold < 0)
            errors.Add($"reward_gold must be >= 0 (got {stats.reward_gold})");
            
        if (stats.reward_xp < 0)
            errors.Add($"reward_xp must be >= 0 (got {stats.reward_xp})");
        
        if (errors.Count > 0)
        {
            GD.PrintErr($"⚠️ Validation errors for enemy '{enemyType}':");
            foreach (var error in errors)
                GD.PrintErr($"   - {error}");
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Validates that all required fields are present in building stats
    /// </summary>
    public static bool ValidateBuildingStats(BuildingStatsData stats, string buildingType)
    {
        var errors = new List<string>();
        
        if (stats.cost <= 0)
            errors.Add($"cost must be > 0 (got {stats.cost})");
            
        if (stats.damage < 0)
            errors.Add($"damage must be >= 0 (got {stats.damage})");
            
        if (stats.range <= 0)
            errors.Add($"range must be > 0 (got {stats.range})");
            
        if (stats.fire_rate <= 0)
            errors.Add($"fire_rate must be > 0 (got {stats.fire_rate})");
            
        if (stats.bullet_speed <= 0)
            errors.Add($"bullet_speed must be > 0 (got {stats.bullet_speed})");
            
        if (string.IsNullOrEmpty(stats.shoot_sound))
            errors.Add("shoot_sound cannot be empty");
            
        if (string.IsNullOrEmpty(stats.impact_sound))
            errors.Add("impact_sound cannot be empty");
        
        if (errors.Count > 0)
        {
            GD.PrintErr($"⚠️ Validation errors for building '{buildingType}':");
            foreach (var error in errors)
                GD.PrintErr($"   - {error}");
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Creates a new enemy stats configuration with default values
    /// </summary>
    public static EnemyStatsData CreateDefaultEnemyStats()
    {
        return new EnemyStatsData
        {
            max_health = 100,
            speed = 60.0f,
            damage = 10,
            reward_gold = 5,
            reward_xp = 10,
            description = "Default enemy configuration"
        };
    }
    
    /// <summary>
    /// Creates a new building stats configuration with default values
    /// </summary>
    public static BuildingStatsData CreateDefaultBuildingStats()
    {
        return new BuildingStatsData
        {
            cost = 15,
            damage = 10,
            range = 120.0f,
            fire_rate = 1.2f,
            bullet_speed = 900.0f,
            shoot_sound = "basic_turret_shoot",
            impact_sound = "basic_bullet_impact",
            description = "Default building configuration"
        };
    }
    
    /// <summary>
    /// Exports enemy stats to JSON string with formatting
    /// </summary>
    public static string ExportEnemyStatsToJson(EnemyStatsConfig config)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        try
        {
            return JsonSerializer.Serialize(config, options);
        }
        catch (Exception e)
        {
            GD.PrintErr($"❌ Failed to export enemy stats: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// Exports building stats to JSON string with formatting
    /// </summary>
    public static string ExportBuildingStatsToJson(BuildingStatsConfig config)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        try
        {
            return JsonSerializer.Serialize(config, options);
        }
        catch (Exception e)
        {
            GD.PrintErr($"❌ Failed to export building stats: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// Lists all available enemy types
    /// </summary>
    public static void ListEnemyTypes()
    {
        if (StatsManager.Instance?._enemyStats?.enemy_types == null)
        {
            GD.Print("⚠️ No enemy stats loaded");
            return;
        }
        
        GD.Print("📋 Available enemy types:");
        foreach (var kvp in StatsManager.Instance._enemyStats.enemy_types)
        {
            var stats = kvp.Value;
            GD.Print($"   • {kvp.Key}: HP={stats.max_health}, Speed={stats.speed}, Damage={stats.damage}");
        }
    }
    
    /// <summary>
    /// Lists all available building types
    /// </summary>
    public static void ListBuildingTypes()
    {
        if (StatsManager.Instance?._buildingStats?.building_types == null)
        {
            GD.Print("⚠️ No building stats loaded");
            return;
        }
        
        GD.Print("📋 Available building types:");
        foreach (var kvp in StatsManager.Instance._buildingStats.building_types)
        {
            var stats = kvp.Value;
            GD.Print($"   • {kvp.Key}: Cost=${stats.cost}, Damage={stats.damage}, Range={stats.range}");
        }
    }
    
    /// <summary>
    /// Validates all loaded configurations
    /// </summary>
    public static bool ValidateAllConfigurations()
    {
        bool allValid = true;
        
        // Validate enemy stats
        if (StatsManager.Instance?._enemyStats?.enemy_types != null)
        {
            foreach (var kvp in StatsManager.Instance._enemyStats.enemy_types)
            {
                if (!ValidateEnemyStats(kvp.Value, kvp.Key))
                    allValid = false;
            }
        }
        
        // Validate building stats
        if (StatsManager.Instance?._buildingStats?.building_types != null)
        {
            foreach (var kvp in StatsManager.Instance._buildingStats.building_types)
            {
                if (!ValidateBuildingStats(kvp.Value, kvp.Key))
                    allValid = false;
            }
        }
        
        if (allValid)
            GD.Print("✅ All stat configurations are valid");
        else
            GD.PrintErr("❌ Some stat configurations have validation errors");
            
        return allValid;
    }
}
