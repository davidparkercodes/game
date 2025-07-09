using Godot;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Game.Infrastructure.Configuration.Services;

public class TowerUpgradeVisualsConfig
{
    private static TowerUpgradeVisualsConfig? _instance;
    public static TowerUpgradeVisualsConfig Instance => _instance ??= new TowerUpgradeVisualsConfig();

    private TowerUpgradeVisualsData? _config;
    private const string ConfigPath = "res://data/ui/tower_upgrade_visuals_config.json";

    public TowerUpgradeVisualsData Config => _config ??= LoadConfig();

    private TowerUpgradeVisualsData LoadConfig()
    {
        try
        {
            var file = Godot.FileAccess.Open(ConfigPath, Godot.FileAccess.ModeFlags.Read);
            if (file == null)
            {
                GD.PrintErr($"❌ Failed to open tower upgrade visuals config file: {ConfigPath}");
                return GetDefaultConfig();
            }

            string content = file.GetAsText();
            file.Close();

            var configWrapper = JsonSerializer.Deserialize<TowerUpgradeVisualsConfigWrapper>(content);
            if (configWrapper?.upgrade_visuals == null)
            {
                GD.PrintErr("❌ Invalid tower upgrade visuals config structure");
                return GetDefaultConfig();
            }

            GD.Print($"✅ Tower upgrade visuals config loaded successfully");
            return configWrapper.upgrade_visuals;
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"❌ Error loading tower upgrade visuals config: {ex.Message}");
            return GetDefaultConfig();
        }
    }

    private TowerUpgradeVisualsData GetDefaultConfig()
    {
        return new TowerUpgradeVisualsData
        {
            SizeScaling = new SizeScalingConfig
            {
                ScaleIncreasePerLevel = 0.10f,
                MinScale = 0.8f,
                MaxScale = 2.0f,
                EnableScaling = true
            },
            ColorTinting = new ColorTintingConfig
            {
                EnableColorTinting = true,
                UpgradeColors = new Dictionary<string, ColorConfig>
                {
                    ["level_1"] = new ColorConfig { R = 1.1f, G = 1.0f, B = 1.0f, A = 1.0f },
                    ["level_2"] = new ColorConfig { R = 1.0f, G = 1.1f, B = 1.0f, A = 1.0f },
                    ["level_3"] = new ColorConfig { R = 1.0f, G = 1.0f, B = 1.1f, A = 1.0f },
                    ["level_4_plus"] = new ColorConfig { R = 1.2f, G = 1.2f, B = 1.0f, A = 1.0f }
                }
            },
            Animation = new AnimationConfig
            {
                ScaleAnimationDuration = 0.3f,
                ScaleAnimationEnabled = true,
                ScaleAnimationEase = "ease_out"
            },
            Collision = new CollisionConfig
            {
                ScaleCollisionWithSize = true,
                CollisionScaleMultiplier = 1.0f
            }
        };
    }

    public float GetScaleForLevel(int level)
    {
        if (!Config.SizeScaling.EnableScaling || level <= 0)
            return 1.0f;

        float scale = 1.0f + (Config.SizeScaling.ScaleIncreasePerLevel * level);
        return Mathf.Clamp(scale, Config.SizeScaling.MinScale, Config.SizeScaling.MaxScale);
    }

    public Color GetColorForLevel(int level)
    {
        if (!Config.ColorTinting.EnableColorTinting || level <= 0)
            return Colors.White;

        string colorKey = level switch
        {
            1 => "level_1",
            2 => "level_2", 
            3 => "level_3",
            _ => "level_4_plus"
        };

        if (Config.ColorTinting.UpgradeColors.TryGetValue(colorKey, out var colorConfig))
        {
            return new Color(colorConfig.R, colorConfig.G, colorConfig.B, colorConfig.A);
        }

        return Colors.White;
    }
}

public class TowerUpgradeVisualsConfigWrapper
{
    [JsonPropertyName("upgrade_visuals")]
    public TowerUpgradeVisualsData upgrade_visuals { get; set; } = new TowerUpgradeVisualsData();
}

public class TowerUpgradeVisualsData
{
    [JsonPropertyName("size_scaling")]
    public SizeScalingConfig SizeScaling { get; set; } = new SizeScalingConfig();
    
    [JsonPropertyName("color_tinting")]
    public ColorTintingConfig ColorTinting { get; set; } = new ColorTintingConfig();
    
    [JsonPropertyName("animation")]
    public AnimationConfig Animation { get; set; } = new AnimationConfig();
    
    [JsonPropertyName("collision")]
    public CollisionConfig Collision { get; set; } = new CollisionConfig();
}

public class SizeScalingConfig
{
    [JsonPropertyName("scale_increase_per_level")]
    public float ScaleIncreasePerLevel { get; set; } = 0.10f;
    
    [JsonPropertyName("min_scale")]
    public float MinScale { get; set; } = 0.8f;
    
    [JsonPropertyName("max_scale")]
    public float MaxScale { get; set; } = 2.0f;
    
    [JsonPropertyName("enable_scaling")]
    public bool EnableScaling { get; set; } = true;
}

public class ColorTintingConfig
{
    [JsonPropertyName("enable_color_tinting")]
    public bool EnableColorTinting { get; set; } = true;
    
    [JsonPropertyName("upgrade_colors")]
    public Dictionary<string, ColorConfig> UpgradeColors { get; set; } = new Dictionary<string, ColorConfig>();
}

public class ColorConfig
{
    [JsonPropertyName("r")]
    public float R { get; set; } = 1.0f;
    
    [JsonPropertyName("g")]
    public float G { get; set; } = 1.0f;
    
    [JsonPropertyName("b")]
    public float B { get; set; } = 1.0f;
    
    [JsonPropertyName("a")]
    public float A { get; set; } = 1.0f;
}

public class AnimationConfig
{
    [JsonPropertyName("scale_animation_duration")]
    public float ScaleAnimationDuration { get; set; } = 0.3f;
    
    [JsonPropertyName("scale_animation_enabled")]
    public bool ScaleAnimationEnabled { get; set; } = true;
    
    [JsonPropertyName("scale_animation_ease")]
    public string ScaleAnimationEase { get; set; } = "ease_out";
}

public class CollisionConfig
{
    [JsonPropertyName("scale_collision_with_size")]
    public bool ScaleCollisionWithSize { get; set; } = true;
    
    [JsonPropertyName("collision_scale_multiplier")]
    public float CollisionScaleMultiplier { get; set; } = 1.0f;
}
