using Godot;
using Game.Infrastructure.Configuration;

namespace Game.Infrastructure.Managers;

public class WaveSpawner
{
    public static WaveSpawner Instance { get; private set; }

    public bool IsSpawning { get; private set; } = false;
    public int CurrentWave { get; private set; } = 0;
    public int EnemiesSpawned { get; private set; } = 0;
    public int TotalEnemiesInWave { get; private set; } = 0;

    private Timer _spawnTimer;
    private WaveConfig _currentWaveConfig;

    static WaveSpawner()
    {
        Instance = new WaveSpawner();
    }

    public void Initialize()
    {
        _spawnTimer = new Timer();
        _spawnTimer.OneShot = true;
        _spawnTimer.Timeout += OnSpawnTimer;
    }

    public void StartWave(WaveConfig waveConfig)
    {
        if (waveConfig == null)
        {
            GD.PrintErr("Cannot start wave: WaveConfig is null");
            return;
        }

        _currentWaveConfig = waveConfig;
        CurrentWave = waveConfig.WaveNumber;
        IsSpawning = true;
        EnemiesSpawned = 0;
        TotalEnemiesInWave = CalculateTotalEnemies(waveConfig);

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
        if (IsSpawning && _currentWaveConfig != null)
        {
            SpawnNextGroup();
            GD.Print($"Wave {CurrentWave} resumed");
        }
    }

    private void SpawnNextGroup()
    {
        if (!IsSpawning || _currentWaveConfig == null)
            return;

        // Find the next group to spawn
        foreach (var group in _currentWaveConfig.EnemyGroups)
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
        if (_currentWaveConfig != null)
        {
            GameManager.Instance?.AddMoney(_currentWaveConfig.BonusMoney);
        }
    }

    private int CalculateTotalEnemies(WaveConfig waveConfig)
    {
        int total = 0;
        foreach (var group in waveConfig.EnemyGroups)
        {
            total += group.Count;
        }
        return total;
    }

    private bool HasMoreEnemies()
    {
        if (_currentWaveConfig == null)
            return false;

        foreach (var group in _currentWaveConfig.EnemyGroups)
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
        _currentWaveConfig = null;
    }

    public int GetTotalWaves()
    {
        // TODO: Get this from configuration
        return 10;
    }

    public int CurrentWaveIndex => CurrentWave;
}
