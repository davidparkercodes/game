using System;
using System.Text.Json.Serialization;

namespace Game.Application.Buildings.Configuration;

public class BuildingDefaultsConfig
{
    [JsonPropertyName("building_defaults")]
    public BuildingDefaultsData BuildingDefaults { get; set; } = new();
}

public class BuildingDefaultsData
{
    [JsonPropertyName("stats")]
    public BuildingStatsDefaults Stats { get; set; } = new();
    
    [JsonPropertyName("visuals")]
    public BuildingVisualsDefaults Visuals { get; set; } = new();
    
    [JsonPropertyName("performance")]
    public BuildingPerformanceDefaults Performance { get; set; } = new();
    
    [JsonPropertyName("placement")]
    public BuildingPlacementDefaults Placement { get; set; } = new();
    
    [JsonPropertyName("visual")]
    public BuildingVisualDefaults Visual { get; set; } = new();
    
    [JsonPropertyName("audio")]
    public BuildingAudioDefaults Audio { get; set; } = new();
    
    [JsonPropertyName("interaction")]
    public BuildingInteractionDefaults Interaction { get; set; } = new();
    
    [JsonPropertyName("effects")]
    public BuildingEffectsDefaults Effects { get; set; } = new();
}

public class BuildingStatsDefaults
{
    [JsonPropertyName("default_cost")]
    public int DefaultCost { get; set; }
    
    [JsonPropertyName("default_damage")]
    public int DefaultDamage { get; set; }
    
    [JsonPropertyName("default_range")]
    public float DefaultRange { get; set; }
    
    [JsonPropertyName("default_attack_speed")]
    public float DefaultAttackSpeed { get; set; }
}

public class BuildingVisualsDefaults
{
    [JsonPropertyName("range_circle_segments")]
    public int RangeCircleSegments { get; set; }
    
    [JsonPropertyName("range_circle_width")]
    public float RangeCircleWidth { get; set; }
    
    [JsonPropertyName("range_circle_color")]
    public ColorConfig RangeCircleColor { get; set; } = new();
    
    [JsonPropertyName("rotation_speed")]
    public float RotationSpeed { get; set; }
    
    [JsonPropertyName("rotation_threshold")]
    public float RotationThreshold { get; set; }
}

public class BuildingPerformanceDefaults
{
    [JsonPropertyName("max_pooled_bullets")]
    public int MaxPooledBullets { get; set; }
    
    [JsonPropertyName("collision_layers")]
    public int[] CollisionLayers { get; set; } = Array.Empty<int>();
}

public class BuildingPlacementDefaults
{
    [JsonPropertyName("grid_size")]
    public int GridSize { get; set; }
    
    [JsonPropertyName("snap_to_grid")]
    public bool SnapToGrid { get; set; }
    
    [JsonPropertyName("placement_preview_color")]
    public ColorConfig PlacementPreviewColor { get; set; } = new();
    
    [JsonPropertyName("invalid_placement_color")]
    public ColorConfig InvalidPlacementColor { get; set; } = new();
}

public class BuildingVisualDefaults
{
    [JsonPropertyName("selection_outline_color")]
    public ColorConfig SelectionOutlineColor { get; set; } = new();
    
    [JsonPropertyName("selection_outline_width")]
    public float SelectionOutlineWidth { get; set; }
    
    [JsonPropertyName("health_bar_height")]
    public float HealthBarHeight { get; set; }
    
    [JsonPropertyName("health_bar_width")]
    public float HealthBarWidth { get; set; }
    
    [JsonPropertyName("health_bar_offset_y")]
    public float HealthBarOffsetY { get; set; }
}

public class BuildingAudioDefaults
{
    [JsonPropertyName("placement_sound")]
    public string PlacementSound { get; set; } = "";
    
    [JsonPropertyName("sell_sound")]
    public string SellSound { get; set; } = "";
    
    [JsonPropertyName("upgrade_sound")]
    public string UpgradeSound { get; set; } = "";
    
    [JsonPropertyName("invalid_placement_sound")]
    public string InvalidPlacementSound { get; set; } = "";
}

public class BuildingInteractionDefaults
{
    [JsonPropertyName("click_radius")]
    public float ClickRadius { get; set; }
    
    [JsonPropertyName("double_click_time")]
    public float DoubleClickTime { get; set; }
    
    [JsonPropertyName("hold_time_for_sell")]
    public float HoldTimeForSell { get; set; }
}

public class BuildingEffectsDefaults
{
    [JsonPropertyName("damage_flash_duration")]
    public float DamageFlashDuration { get; set; }
    
    [JsonPropertyName("damage_flash_color")]
    public ColorConfig DamageFlashColor { get; set; } = new();
    
    [JsonPropertyName("construction_effect_duration")]
    public float ConstructionEffectDuration { get; set; }
}

public class ColorConfig
{
    [JsonPropertyName("r")]
    public float R { get; set; }
    
    [JsonPropertyName("g")]
    public float G { get; set; }
    
    [JsonPropertyName("b")]
    public float B { get; set; }
    
    [JsonPropertyName("a")]
    public float A { get; set; }
}
