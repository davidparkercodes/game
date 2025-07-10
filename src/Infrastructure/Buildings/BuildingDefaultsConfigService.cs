using System.IO;
using System.Text.Json;
using Game.Application.Buildings.Configuration;
using Game.Application.Buildings.Services;

namespace Game.Infrastructure.Buildings;

public class BuildingDefaultsConfigService : IBuildingDefaultsConfigService
{
    private readonly BuildingDefaultsConfig _config;

    public BuildingDefaultsConfigService()
    {
        var jsonString = File.ReadAllText("config/entities/buildings/building_defaults.json");
        _config = JsonSerializer.Deserialize<BuildingDefaultsConfig>(jsonString) ?? throw new FileNotFoundException("Configuration file 'config/entities/buildings/building_defaults.json' not found.");
    }

    public BuildingDefaultsConfig GetBuildingDefaultsConfig() => _config;

    public int GetDefaultCost() => _config.BuildingDefaults.Stats.DefaultCost;

    public int GetDefaultDamage() => _config.BuildingDefaults.Stats.DefaultDamage;

    public float GetDefaultRange() => _config.BuildingDefaults.Stats.DefaultRange;

    public float GetDefaultAttackSpeed() => _config.BuildingDefaults.Stats.DefaultAttackSpeed;

    public int GetRangeCircleSegments() => _config.BuildingDefaults.Visuals.RangeCircleSegments;

    public float GetRangeCircleWidth() => _config.BuildingDefaults.Visuals.RangeCircleWidth;

    public ColorConfig GetRangeCircleColor() => _config.BuildingDefaults.Visuals.RangeCircleColor;

    public float GetRotationSpeed() => _config.BuildingDefaults.Visuals.RotationSpeed;

    public float GetRotationThreshold() => _config.BuildingDefaults.Visuals.RotationThreshold;

    public int GetMaxPooledBullets() => _config.BuildingDefaults.Performance.MaxPooledBullets;

    public int[] GetCollisionLayers() => _config.BuildingDefaults.Performance.CollisionLayers;
}
