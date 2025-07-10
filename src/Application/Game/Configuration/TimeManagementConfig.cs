using System;
using System.Text.Json.Serialization;

namespace Game.Application.Game.Configuration;

public class TimeManagementConfig
{
    [JsonPropertyName("time_management")]
    public TimeManagementData TimeManagement { get; set; } = new();
    
    public float[] SpeedOptions => TimeManagement.GameSpeed.SpeedOptions;
    public int DefaultSpeedIndex => TimeManagement.GameSpeed.DefaultSpeedIndex;
    public float SpeedTolerance => TimeManagement.GameSpeed.SpeedTolerance;
}

public class TimeManagementData
{
    [JsonPropertyName("wave_timing")]
    public WaveTimingConfig WaveTiming { get; set; } = new();
    
    [JsonPropertyName("game_speed")]
    public GameSpeedConfig GameSpeed { get; set; } = new();
    
    [JsonPropertyName("wave_progression")]
    public WaveProgressionConfig WaveProgression { get; set; } = new();
    
    [JsonPropertyName("timers")]
    public TimersConfig Timers { get; set; } = new();
    
    [JsonPropertyName("difficulty_scaling")]
    public DifficultyScalingConfig DifficultyScaling { get; set; } = new();
    
    [JsonPropertyName("special_events")]
    public SpecialEventsConfig SpecialEvents { get; set; } = new();
}

public class WaveTimingConfig
{
    [JsonPropertyName("time_between_waves")]
    public float TimeBetweenWaves { get; set; }
    
    [JsonPropertyName("preparation_time")]
    public float PreparationTime { get; set; }
    
    [JsonPropertyName("warning_time")]
    public float WarningTime { get; set; }
    
    [JsonPropertyName("countdown_start")]
    public float CountdownStart { get; set; }
}

public class GameSpeedConfig
{
    [JsonPropertyName("speed_options")]
    public float[] SpeedOptions { get; set; } = Array.Empty<float>();
    
    [JsonPropertyName("default_speed_index")]
    public int DefaultSpeedIndex { get; set; }
    
    [JsonPropertyName("speed_tolerance")]
    public float SpeedTolerance { get; set; }
    
    [JsonPropertyName("normal_speed")]
    public float NormalSpeed { get; set; }
    
    [JsonPropertyName("fast_forward_speed")]
    public float FastForwardSpeed { get; set; }
    
    [JsonPropertyName("slow_motion_speed")]
    public float SlowMotionSpeed { get; set; }
    
    [JsonPropertyName("pause_speed")]
    public float PauseSpeed { get; set; }
    
    [JsonPropertyName("max_speed_multiplier")]
    public float MaxSpeedMultiplier { get; set; }
}

public class WaveProgressionConfig
{
    [JsonPropertyName("base_wave_duration")]
    public float BaseWaveDuration { get; set; }
    
    [JsonPropertyName("duration_increase_per_wave")]
    public float DurationIncreasePerWave { get; set; }
    
    [JsonPropertyName("max_wave_duration")]
    public float MaxWaveDuration { get; set; }
    
    [JsonPropertyName("enemy_spawn_interval")]
    public float EnemySpawnInterval { get; set; }
    
    [JsonPropertyName("spawn_interval_decrease_per_wave")]
    public float SpawnIntervalDecreasePerWave { get; set; }
    
    [JsonPropertyName("min_spawn_interval")]
    public float MinSpawnInterval { get; set; }
}

public class TimersConfig
{
    [JsonPropertyName("auto_start_next_wave")]
    public bool AutoStartNextWave { get; set; }
    
    [JsonPropertyName("auto_start_delay")]
    public float AutoStartDelay { get; set; }
    
    [JsonPropertyName("show_countdown")]
    public bool ShowCountdown { get; set; }
    
    [JsonPropertyName("show_wave_progress")]
    public bool ShowWaveProgress { get; set; }
    
    [JsonPropertyName("show_next_wave_preview")]
    public bool ShowNextWavePreview { get; set; }
}

public class DifficultyScalingConfig
{
    [JsonPropertyName("enemy_count_multiplier_per_wave")]
    public float EnemyCountMultiplierPerWave { get; set; }
    
    [JsonPropertyName("enemy_health_multiplier_per_wave")]
    public float EnemyHealthMultiplierPerWave { get; set; }
    
    [JsonPropertyName("enemy_speed_multiplier_per_wave")]
    public float EnemySpeedMultiplierPerWave { get; set; }
    
    [JsonPropertyName("max_enemies_per_wave")]
    public int MaxEnemiesPerWave { get; set; }
}

public class SpecialEventsConfig
{
    [JsonPropertyName("boss_waves")]
    public int[] BossWaves { get; set; } = Array.Empty<int>();
    
    [JsonPropertyName("bonus_waves")]
    public int[] BonusWaves { get; set; } = Array.Empty<int>();
    
    [JsonPropertyName("rush_waves")]
    public int[] RushWaves { get; set; } = Array.Empty<int>();
    
    [JsonPropertyName("preparation_time_bonus_waves")]
    public float PreparationTimeBonusWaves { get; set; }
    
    [JsonPropertyName("preparation_time_boss_waves")]
    public float PreparationTimeBossWaves { get; set; }
}
