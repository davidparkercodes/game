using System.Collections.Generic;
using System.Text.Json.Serialization;
using Game.Application.Buildings.Configuration;

namespace Game.Application.Enemies.Configuration;

public class BossConfig
{
    [JsonPropertyName("boss_config")]
    public BossData BossData { get; set; } = new();
}

public class BossData
{
    [JsonPropertyName("default_settings")]
    public DefaultSettingsConfig DefaultSettings { get; set; } = new();

    [JsonPropertyName("spawn_conditions")]
    public SpawnConditionsConfig SpawnConditions { get; set; } = new();

    [JsonPropertyName("boss_types")]
    public Dictionary<string, BossTypeConfig> BossTypes { get; set; } = new();

    [JsonPropertyName("scaling")]
    public ScalingConfig Scaling { get; set; } = new();

    [JsonPropertyName("effects")]
    public EffectsConfig Effects { get; set; } = new();
}

public class DefaultSettingsConfig
{
    [JsonPropertyName("default_scale_multiplier")]
    public float DefaultScaleMultiplier { get; set; }

    [JsonPropertyName("immunity_duration_seconds")]
    public int ImmunityDurationSeconds { get; set; }

    [JsonPropertyName("final_phase_threshold")]
    public float FinalPhaseThreshold { get; set; }
}

public class SpawnConditionsConfig
{
    [JsonPropertyName("waves")]
    public int[] Waves { get; set; } = System.Array.Empty<int>();

    [JsonPropertyName("min_wave_for_spawn")]
    public int MinWaveForSpawn { get; set; }

    [JsonPropertyName("spawn_probability")]
    public float SpawnProbability { get; set; }

    [JsonPropertyName("max_bosses_per_wave")]
    public int MaxBossesPerWave { get; set; }
}

public class BossTypeConfig
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("health")]
    public int Health { get; set; }

    [JsonPropertyName("speed")]
    public float Speed { get; set; }

    [JsonPropertyName("damage")]
    public int Damage { get; set; }

    [JsonPropertyName("armor")]
    public int Armor { get; set; }

    [JsonPropertyName("collision_radius")]
    public float CollisionRadius { get; set; }

    [JsonPropertyName("reward_money")]
    public int RewardMoney { get; set; }

    [JsonPropertyName("reward_score")]
    public int RewardScore { get; set; }

    [JsonPropertyName("can_fly")]
    public bool CanFly { get; set; }

    [JsonPropertyName("visual")]
    public VisualConfig Visual { get; set; } = new();

    [JsonPropertyName("audio")]
    public AudioConfig Audio { get; set; } = new();

    [JsonPropertyName("abilities")]
    public Dictionary<string, AbilityConfig> Abilities { get; set; } = new();
}

public class VisualConfig
{
    [JsonPropertyName("sprite_region")]
    public string SpriteRegion { get; set; } = "";

    [JsonPropertyName("scale_multiplier")]
    public float ScaleMultiplier { get; set; }

    [JsonPropertyName("color_tint")]
    public ColorConfig ColorTint { get; set; } = new();
}

public class AudioConfig
{
    [JsonPropertyName("spawn_sound")]
    public string SpawnSound { get; set; } = "";

    [JsonPropertyName("death_sound")]
    public string DeathSound { get; set; } = "";

    [JsonPropertyName("hurt_sound")]
    public string HurtSound { get; set; } = "";
}

public class AbilityConfig
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("cooldown")]
    public float Cooldown { get; set; }

    [JsonPropertyName("damage_multiplier")]
    public float DamageMultiplier { get; set; }

    [JsonPropertyName("speed_multiplier")]
    public float SpeedMultiplier { get; set; }

    [JsonPropertyName("duration")]
    public float Duration { get; set; }

    [JsonPropertyName("heal_amount")]
    public int HealAmount { get; set; }

    [JsonPropertyName("heal_interval")]
    public float HealInterval { get; set; }

    [JsonPropertyName("max_heal_count")]
    public int MaxHealCount { get; set; }

    [JsonPropertyName("damage")]
    public int Damage { get; set; }

    [JsonPropertyName("radius")]
    public float Radius { get; set; }

    [JsonPropertyName("warning_time")]
    public float WarningTime { get; set; }

    [JsonPropertyName("dodge_chance")]
    public float DodgeChance { get; set; }

    [JsonPropertyName("dodge_duration")]
    public float DodgeDuration { get; set; }
}

public class ScalingConfig
{
    [JsonPropertyName("health_multiplier_per_wave")]
    public float HealthMultiplierPerWave { get; set; }

    [JsonPropertyName("damage_multiplier_per_wave")]
    public float DamageMultiplierPerWave { get; set; }

    [JsonPropertyName("speed_multiplier_per_wave")]
    public float SpeedMultiplierPerWave { get; set; }

    [JsonPropertyName("reward_multiplier_per_wave")]
    public float RewardMultiplierPerWave { get; set; }
}

public class EffectsConfig
{
    [JsonPropertyName("spawn_effect")]
    public EffectConfig SpawnEffect { get; set; } = new();

    [JsonPropertyName("death_effect")]
    public EffectConfig DeathEffect { get; set; } = new();
}

public class EffectConfig
{
    [JsonPropertyName("particle_count")]
    public int ParticleCount { get; set; }

    [JsonPropertyName("effect_duration")]
    public float EffectDuration { get; set; }

    [JsonPropertyName("screen_shake_intensity")]
    public float ScreenShakeIntensity { get; set; }

    [JsonPropertyName("camera_zoom_effect")]
    public bool CameraZoomEffect { get; set; }
}

