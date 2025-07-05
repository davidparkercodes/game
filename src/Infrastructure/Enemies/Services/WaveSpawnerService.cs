using System;
using Godot;
using Game.Infrastructure.Waves.Models;
using Game.Infrastructure.Game.Services;
using Game.Infrastructure.Rounds.Services;

namespace Game.Infrastructure.Enemies.Services;

public class WaveSpawnerService
{
    public static WaveSpawnerService Instance { get; private set; } = null!;

    public bool IsSpawning { get; private set; } = false;
    public int CurrentWave { get; private set; } = 0;
    public int EnemiesSpawned { get; private set; } = 0;
    public int TotalEnemiesInWave { get; private set; } = 0;

    private Godot.Timer _spawnTimer = null!;
    private WaveModel? _currentWave;

    static WaveSpawnerService()
    {
        Instance = new WaveSpawnerService();
    }

    public void Initialize()
    {
        _spawnTimer = new Godot.Timer();
        _spawnTimer.OneShot = true;
        _spawnTimer.Timeout += OnSpawnTimer;
    }

    public void StartWave(int waveNumber)
    {
        if (waveNumber <= 0)
        {
            GD.PrintErr($"Cannot start wave: Invalid wave number {waveNumber}");
            return;
        }

        _currentWave = CreateWaveConfiguration(waveNumber);
        CurrentWave = waveNumber;
        IsSpawning = true;
        EnemiesSpawned = 0;
        TotalEnemiesInWave = CalculateTotalEnemies(_currentWave);

        GD.Print($"Starting wave {CurrentWave} with {TotalEnemiesInWave} enemies");

        SpawnNextGroup();
    }

    public void StopWave()
    {
        IsSpawning = false;
        _spawnTimer?.Stop();
        GD.Print($"Wave {CurrentWave} stopped");
    }

    public void PauseWave()
    {
        _spawnTimer?.Stop();
        GD.Print($"Wave {CurrentWave} paused");
    }

    public void ResumeWave()
    {
        if (IsSpawning && _currentWave != null)
        {
            SpawnNextGroup();
            GD.Print($"Wave {CurrentWave} resumed");
        }
    }

    private void SpawnNextGroup()
    {
        if (!IsSpawning || _currentWave == null)
            return;

        foreach (var group in _currentWave.EnemyGroups)
        {
            if (group.Count > 0)
            {
                SpawnEnemyGroup(group);
                return;
            }
        }

        CompleteWave();
    }

    private void SpawnEnemyGroup(EnemySpawnGroup group)
    {
        if (group.Count <= 0)
            return;

        SpawnEnemy(group.EnemyType);
        group.Count--;
        EnemiesSpawned++;

        if (IsSpawning && HasMoreEnemies())
        {
            _spawnTimer.WaitTime = group.SpawnInterval;
            _spawnTimer.Start();
        }
        else
        {
            CompleteWave();
        }
    }

    private void SpawnEnemy(string enemyType)
    {
        var spawnPosition = PathService.Instance?.GetSpawnPosition() ?? Vector2.Zero;
        GD.Print($"Spawning {enemyType} at {spawnPosition}");

        RoundService.Instance?.OnEnemySpawned();
    }

    private void OnSpawnTimer()
    {
        SpawnNextGroup();
    }

    private void CompleteWave()
    {
        IsSpawning = false;
        GD.Print($"Wave {CurrentWave} completed! Spawned {EnemiesSpawned} enemies");

        if (_currentWave != null)
        {
            GameService.Instance?.AddMoney(_currentWave.BonusMoney);
        }
    }

    private int CalculateTotalEnemies(WaveModel waveConfiguration)
    {
        int total = 0;
        foreach (var group in waveConfiguration.EnemyGroups)
        {
            total += group.Count;
        }
        return total;
    }

    private bool HasMoreEnemies()
    {
        if (_currentWave == null)
            return false;

        foreach (var group in _currentWave.EnemyGroups)
        {
            if (group.Count > 0)
                return true;
        }
        return false;
    }

    public float GetWaveProgress()
    {
        if (TotalEnemiesInWave == 0)
            return 0f;

        return (float)EnemiesSpawned / TotalEnemiesInWave;
    }

    public void Reset()
    {
        StopWave();
        CurrentWave = 0;
        EnemiesSpawned = 0;
        TotalEnemiesInWave = 0;
        _currentWave = null;
    }

    public int GetTotalWaves()
    {
        return 10;
    }

    public int CurrentWaveIndex => CurrentWave;

    private WaveModel CreateWaveConfiguration(int waveNumber)
    {
        var waveConfiguration = new WaveModel
        {
            WaveNumber = waveNumber,
            WaveName = $"Wave {waveNumber}",
            Description = $"Standard wave {waveNumber} with {5 + waveNumber * 2} enemies",
            BonusMoney = 25 + (waveNumber * 5)
        };

        var enemyGroup = new EnemySpawnGroup
        {
            EnemyType = "Basic",
            Count = 5 + (waveNumber * 2),
            SpawnInterval = Math.Max(0.5f, 2.0f - (waveNumber * 0.1f)),
            HealthMultiplier = 1.0f + (waveNumber * 0.15f),
            SpeedMultiplier = 1.0f + (waveNumber * 0.05f),
            MoneyReward = 10 + (waveNumber * 2)
        };

        waveConfiguration.EnemyGroups.Add(enemyGroup);
        return waveConfiguration;
    }
}
