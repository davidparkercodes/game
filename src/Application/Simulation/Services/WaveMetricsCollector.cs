using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using Game.Application.Simulation.ValueObjects;

namespace Game.Application.Simulation.Services;

public interface IWaveMetricsCollector
{
    void StartWaveTracking(int waveNumber, string waveName);
    void RecordEnemySpawn(string enemyType, TimeSpan gameTime);
    void RecordEnemyKill(string enemyType, TimeSpan gameTime);
    void EndWaveTracking(int totalEnemies, int enemiesKilled, int enemiesLeaked, int moneyEarned, int livesLost);
    WaveMetrics? GetLastWaveMetrics();
    List<WaveMetrics> GetAllWaveMetrics();
    SimulationMetrics GenerateSimulationMetrics(string scenarioName, TimeSpan totalDuration, bool overallSuccess);
    void ExportMetrics(string filePath, SimulationMetrics metrics);
    void Reset();
}

public class WaveMetricsCollector : IWaveMetricsCollector
{
    private readonly List<WaveMetrics> _waveMetrics = new List<WaveMetrics>();
    private readonly List<EnemySpawnTiming> _currentWaveSpawnTimings = new List<EnemySpawnTiming>();
    private readonly Dictionary<string, TimeSpan> _spawnTimes = new Dictionary<string, TimeSpan>();
    
    private Stopwatch? _currentWaveStopwatch;
    private int _currentWaveNumber;
    private string _currentWaveName = string.Empty;

    public void StartWaveTracking(int waveNumber, string waveName)
    {
        _currentWaveNumber = waveNumber;
        _currentWaveName = waveName;
        _currentWaveSpawnTimings.Clear();
        _spawnTimes.Clear();
        _currentWaveStopwatch = Stopwatch.StartNew();
    }

    public void RecordEnemySpawn(string enemyType, TimeSpan gameTime)
    {
        var enemyId = $"{enemyType}_{_currentWaveSpawnTimings.Count}";
        var spawnTiming = new EnemySpawnTiming(enemyType, gameTime);
        _currentWaveSpawnTimings.Add(spawnTiming);
        _spawnTimes[enemyId] = gameTime;
    }

    public void RecordEnemyKill(string enemyType, TimeSpan gameTime)
    {
        // Find the most recent spawn of this enemy type that hasn't been killed yet
        for (int i = _currentWaveSpawnTimings.Count - 1; i >= 0; i--)
        {
            var timing = _currentWaveSpawnTimings[i];
            if (timing.EnemyType == enemyType && !timing.WasKilled)
            {
                _currentWaveSpawnTimings[i] = timing.WithDeathTime(gameTime);
                break;
            }
        }
    }

    public void EndWaveTracking(int totalEnemies, int enemiesKilled, int enemiesLeaked, int moneyEarned, int livesLost)
    {
        if (_currentWaveStopwatch == null)
            return;

        _currentWaveStopwatch.Stop();
        
        var waveMetrics = new WaveMetrics(
            _currentWaveNumber,
            _currentWaveName,
            _currentWaveStopwatch.Elapsed,
            totalEnemies,
            enemiesKilled,
            enemiesLeaked,
            moneyEarned,
            livesLost,
            new List<EnemySpawnTiming>(_currentWaveSpawnTimings)
        );

        _waveMetrics.Add(waveMetrics);
        _currentWaveStopwatch = null;
    }

    public WaveMetrics? GetLastWaveMetrics()
    {
        return _waveMetrics.Count > 0 ? _waveMetrics[_waveMetrics.Count - 1] : null;
    }

    public List<WaveMetrics> GetAllWaveMetrics()
    {
        return new List<WaveMetrics>(_waveMetrics);
    }

    public SimulationMetrics GenerateSimulationMetrics(string scenarioName, TimeSpan totalDuration, bool overallSuccess)
    {
        var simulationMetrics = new SimulationMetrics(
            scenarioName,
            totalDuration,
            overallSuccess,
            GetAllWaveMetrics()
        );

        // Add custom analytics
        AddPerformanceMetrics(simulationMetrics);
        AddDifficultyAnalysis(simulationMetrics);
        AddProgressionMetrics(simulationMetrics);

        return simulationMetrics;
    }

    public void ExportMetrics(string filePath, SimulationMetrics metrics)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var jsonData = JsonSerializer.Serialize(metrics, options);
            File.WriteAllText(filePath, jsonData);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to export metrics to {filePath}: {ex.Message}", ex);
        }
    }

    public void Reset()
    {
        _waveMetrics.Clear();
        _currentWaveSpawnTimings.Clear();
        _spawnTimes.Clear();
        _currentWaveStopwatch?.Stop();
        _currentWaveStopwatch = null;
        _currentWaveNumber = 0;
        _currentWaveName = string.Empty;
    }

    private void AddPerformanceMetrics(SimulationMetrics metrics)
    {
        if (!metrics.WaveMetrics.Any())
            return;

        var totalEnemies = metrics.WaveMetrics.Sum(w => w.TotalEnemies);
        var totalKilled = metrics.WaveMetrics.Sum(w => w.EnemiesKilled);
        var totalLeaked = metrics.WaveMetrics.Sum(w => w.EnemiesLeaked);
        var averageWaveDuration = TimeSpan.FromMilliseconds(
            metrics.WaveMetrics.Average(w => w.WaveDuration.TotalMilliseconds));

        metrics.AddCustomMetric("TotalEnemiesSpawned", totalEnemies);
        metrics.AddCustomMetric("TotalEnemiesKilled", totalKilled);
        metrics.AddCustomMetric("TotalEnemiesLeaked", totalLeaked);
        metrics.AddCustomMetric("KillEfficiency", totalEnemies > 0 ? (float)totalKilled / totalEnemies : 0f);
        metrics.AddCustomMetric("AverageWaveDuration", averageWaveDuration);
        metrics.AddCustomMetric("EnemiesPerSecond", 
            metrics.TotalDuration.TotalSeconds > 0 ? totalEnemies / metrics.TotalDuration.TotalSeconds : 0);
    }

    private void AddDifficultyAnalysis(SimulationMetrics metrics)
    {
        if (!metrics.WaveMetrics.Any())
            return;

        var difficultyProgression = metrics.WaveMetrics.Select(w => w.DifficultyRating).ToList();
        var maxDifficulty = difficultyProgression.Max();
        var minDifficulty = difficultyProgression.Min();
        var difficultySpike = CalculateDifficultySpike(difficultyProgression);

        metrics.AddCustomMetric("MaxDifficultyRating", maxDifficulty);
        metrics.AddCustomMetric("MinDifficultyRating", minDifficulty);
        metrics.AddCustomMetric("DifficultyRange", maxDifficulty - minDifficulty);
        metrics.AddCustomMetric("DifficultySpike", difficultySpike);
        metrics.AddCustomMetric("DifficultyProgression", difficultyProgression);
    }

    private void AddProgressionMetrics(SimulationMetrics metrics)
    {
        if (!metrics.WaveMetrics.Any())
            return;

        var completionRates = metrics.WaveMetrics.Select(w => w.CompletionRate).ToList();
        var moneyProgression = metrics.WaveMetrics.Select(w => w.MoneyEarned).ToList();
        var livesProgression = metrics.WaveMetrics.Select(w => w.LivesLost).ToList();

        metrics.AddCustomMetric("CompletionRateProgression", completionRates);
        metrics.AddCustomMetric("MoneyEarnedProgression", moneyProgression);
        metrics.AddCustomMetric("LivesLostProgression", livesProgression);
        metrics.AddCustomMetric("TotalMoneyEarned", moneyProgression.Sum());
        metrics.AddCustomMetric("TotalLivesLost", livesProgression.Sum());
        
        // Calculate balance score (higher is better)
        var balanceScore = CalculateBalanceScore(completionRates, livesProgression);
        metrics.AddCustomMetric("BalanceScore", balanceScore);
    }

    private float CalculateDifficultySpike(List<float> difficultyProgression)
    {
        if (difficultyProgression.Count < 2)
            return 0f;

        float maxSpike = 0f;
        for (int i = 1; i < difficultyProgression.Count; i++)
        {
            var spike = difficultyProgression[i] - difficultyProgression[i - 1];
            if (spike > maxSpike)
                maxSpike = spike;
        }

        return maxSpike;
    }

    private float CalculateBalanceScore(List<float> completionRates, List<int> livesProgression)
    {
        if (!completionRates.Any())
            return 0f;

        // Balance score considers completion consistency and gradual difficulty increase
        var averageCompletion = completionRates.Average();
        var completionVariance = CalculateVariance(completionRates);
        var livesDistribution = CalculateLivesDistribution(livesProgression);

        // Higher score for consistent completion rates with gradual lives loss
        return Math.Max(0f, averageCompletion - completionVariance + livesDistribution);
    }

    private float CalculateVariance(List<float> values)
    {
        if (!values.Any())
            return 0f;

        var mean = values.Average();
        return values.Sum(v => (v - mean) * (v - mean)) / values.Count;
    }

    private float CalculateLivesDistribution(List<int> livesProgression)
    {
        if (!livesProgression.Any())
            return 0f;

        // Prefer gradual lives loss over sudden spikes
        var maxLives = 20f; // Assuming 20 max lives
        var totalLives = livesProgression.Sum();
        var averageLivesPerWave = totalLives / (float)livesProgression.Count;

        return Math.Max(0f, 1f - (averageLivesPerWave / maxLives));
    }
}
