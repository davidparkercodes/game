using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Application.Simulation.ValueObjects;

public class WaveMetrics
{
    public int WaveNumber { get; }
    public string WaveName { get; }
    public TimeSpan WaveDuration { get; }
    public int TotalEnemies { get; }
    public int EnemiesKilled { get; }
    public int EnemiesLeaked { get; }
    public float CompletionRate { get; }
    public float DifficultyRating { get; }
    public int MoneyEarned { get; }
    public int LivesLost { get; }
    public List<EnemySpawnTiming> EnemySpawnTimings { get; }

    public WaveMetrics(
        int waveNumber,
        string waveName,
        TimeSpan waveDuration,
        int totalEnemies,
        int enemiesKilled,
        int enemiesLeaked,
        int moneyEarned,
        int livesLost,
        List<EnemySpawnTiming> enemySpawnTimings)
    {
        WaveNumber = waveNumber;
        WaveName = waveName;
        WaveDuration = waveDuration;
        TotalEnemies = totalEnemies;
        EnemiesKilled = enemiesKilled;
        EnemiesLeaked = enemiesLeaked;
        CompletionRate = totalEnemies > 0 ? (float)enemiesKilled / totalEnemies : 0f;
        DifficultyRating = CalculateDifficultyRating();
        MoneyEarned = moneyEarned;
        LivesLost = livesLost;
        EnemySpawnTimings = enemySpawnTimings ?? new List<EnemySpawnTiming>();
    }

    private float CalculateDifficultyRating()
    {
        if (TotalEnemies == 0) return 0f;
        
        // Calculate difficulty based on completion rate, lives lost, and wave duration
        float baseRating = 1.0f - CompletionRate;
        float livesLostFactor = LivesLost / 20.0f; // Assuming 20 max lives
        float timeFactor = (float)Math.Min(WaveDuration.TotalSeconds / 60.0, 1.0); // Normalize to 1 minute max
        
        return (float)Math.Min(baseRating + (livesLostFactor * 0.3f) + (timeFactor * 0.1f), 5.0f);
    }

    public bool IsSuccessful => CompletionRate >= 1.0f && LivesLost == 0;
    public bool IsPartialSuccess => CompletionRate >= 0.8f;
    public bool IsFailure => CompletionRate < 0.5f;
}

public class EnemySpawnTiming
{
    public string EnemyType { get; }
    public TimeSpan SpawnTime { get; }
    public TimeSpan? DeathTime { get; }
    public bool WasKilled { get; }
    public TimeSpan SurvivalDuration => DeathTime.HasValue ? DeathTime.Value - SpawnTime : TimeSpan.Zero;

    public EnemySpawnTiming(string enemyType, TimeSpan spawnTime, TimeSpan? deathTime = null)
    {
        EnemyType = enemyType;
        SpawnTime = spawnTime;
        DeathTime = deathTime;
        WasKilled = deathTime.HasValue;
    }

    public EnemySpawnTiming WithDeathTime(TimeSpan deathTime)
    {
        return new EnemySpawnTiming(EnemyType, SpawnTime, deathTime);
    }
}

public class SimulationMetrics
{
    public string ScenarioName { get; }
    public TimeSpan TotalDuration { get; }
    public bool OverallSuccess { get; }
    public int TotalWavesAttempted { get; }
    public int TotalWavesCompleted { get; }
    public float OverallCompletionRate { get; }
    public float AverageDifficultyRating { get; }
    public List<WaveMetrics> WaveMetrics { get; }
    public Dictionary<string, object> CustomMetrics { get; }

    public SimulationMetrics(
        string scenarioName,
        TimeSpan totalDuration,
        bool overallSuccess,
        List<WaveMetrics> waveMetrics,
        Dictionary<string, object>? customMetrics = null)
    {
        ScenarioName = scenarioName;
        TotalDuration = totalDuration;
        OverallSuccess = overallSuccess;
        WaveMetrics = waveMetrics ?? new List<WaveMetrics>();
        TotalWavesAttempted = WaveMetrics.Count;
        TotalWavesCompleted = WaveMetrics.Count(w => w.IsSuccessful);
        OverallCompletionRate = TotalWavesAttempted > 0 ? (float)TotalWavesCompleted / TotalWavesAttempted : 0f;
        AverageDifficultyRating = WaveMetrics.Any() ? WaveMetrics.Average(w => w.DifficultyRating) : 0f;
        CustomMetrics = customMetrics ?? new Dictionary<string, object>();
    }

    public void AddCustomMetric(string key, object value)
    {
        CustomMetrics[key] = value;
    }

    public T? GetCustomMetric<T>(string key)
    {
        return CustomMetrics.TryGetValue(key, out var value) && value is T ? (T)value : default;
    }
}
