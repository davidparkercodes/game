using System;
using Godot;
using Game.Infrastructure.Waves;

namespace Game.Infrastructure.Managers;

public class WaveSpawner
{
    public static WaveSpawner Instance { get; private set; }

    public bool IsSpawning { get; private set; } = false;
    public int CurrentWave { get; private set; } = 0;
    public int EnemiesSpawned { get; private set; } = 0;
    public int TotalEnemiesInWave { get; private set; } = 0;

    private Godot.Timer _spawnTimer;
    private WaveConfigurationInternal _currentWaveConfiguration;

    static WaveSpawner()
    {
        Instance = new WaveSpawner();
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

        _currentWaveConfiguration = CreateWaveConfiguration(waveNumber);
        CurrentWave = waveNumber;
        IsSpawning = true;
        EnemiesSpawned = 0;
        TotalEnemiesInWave = CalculateTotalEnemies(_currentWaveConfiguration);

        GD.Print($"Starting wave {CurrentWave} with {TotalEnemiesInWave} enemies");

        // Start spawning the first group
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
        if (IsSpawning && _currentWaveConfiguration != null)
        {
            SpawnNextGroup();
            GD.Print($"Wave {CurrentWave} resumed");
        }
    }

    private void SpawnNextGroup()
    {
        if (!IsSpawning || _currentWaveConfiguration == null)
            return;

        // Find the next group to spawn
        foreach (var group in _currentWaveConfiguration.EnemyGroups)
        {
            if (group.Count > 0)
            {
                SpawnEnemyGroup(group);
                return;
            }
        }

        // No more groups to spawn
        CompleteWave();
    }

    private void SpawnEnemyGroup(EnemySpawnGroup group)
    {
        if (group.Count <= 0)
            return;

        // Spawn one enemy from this group
        SpawnEnemy(group.EnemyType);
        group.Count--;
        EnemiesSpawned++;

        // Schedule next spawn
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
        // TODO: Implement actual enemy spawning
        var spawnPosition = PathManager.Instance?.GetSpawnPosition() ?? Vector2.Zero;
        GD.Print($"Spawning {enemyType} at {spawnPosition}");

        // Notify round manager
        RoundManager.Instance?.OnEnemySpawned();
    }

    private void OnSpawnTimer()
    {
        SpawnNextGroup();
    }

    private void CompleteWave()
    {
        IsSpawning = false;
        GD.Print($"Wave {CurrentWave} completed! Spawned {EnemiesSpawned} enemies");

        // Award wave completion bonus
        if (_currentWaveConfiguration != null)
        {
            GameManager.Instance?.AddMoney(_currentWaveConfiguration.BonusMoney);
        }
    }

    private int CalculateTotalEnemies(WaveConfigurationInternal waveConfiguration)
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
        if (_currentWaveConfiguration == null)
            return false;

        foreach (var group in _currentWaveConfiguration.EnemyGroups)
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
        _currentWaveConfiguration = null;
    }

    public int GetTotalWaves()
    {
        // TODO: Get this from configuration
        return 10;
    }

    public int CurrentWaveIndex => CurrentWave;

    private WaveConfigurationInternal CreateWaveConfiguration(int waveNumber)
    {
        var waveConfiguration = new WaveConfigurationInternal
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
