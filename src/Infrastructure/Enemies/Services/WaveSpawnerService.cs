using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;
using Game.Domain.Enemies.Services;
using Game.Domain.Enemies.ValueObjects;
using Game.Infrastructure.Waves.Models;
using Game.Infrastructure.Game.Services;
using Game.Infrastructure.Rounds.Services;
using Game.Infrastructure.Waves.Services;

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
    private readonly IWaveConfigurationService _waveConfigurationService;
    private WaveConfiguration _loadedWaveSet;
    private List<WaveModel>? _waves;

    private WaveSpawnerService()
    {
        _waveConfigurationService = new WaveConfigurationService();
    }
    
    static WaveSpawnerService()
    {
        Instance = new WaveSpawnerService();
    }

    public void Initialize()
    {
        _spawnTimer = new Godot.Timer();
        _spawnTimer.OneShot = true;
        _spawnTimer.Timeout += OnSpawnTimer;
        
        LoadWaveConfiguration();
    }
    
    private void LoadWaveConfiguration()
    {
        try
        {
            _loadedWaveSet = _waveConfigurationService.LoadWaveSetFromPath("res://data/waves/default_waves.json");
            
            if (!string.IsNullOrEmpty(_loadedWaveSet.JsonData))
            {
                var waveSetModel = JsonSerializer.Deserialize<WaveSetModel>(_loadedWaveSet.JsonData);
                _waves = waveSetModel?.Waves;
                
                if (_waves != null && _waves.Count > 0)
                {
                    GD.Print($"WaveSpawnerService: Successfully loaded {_waves.Count} waves from configuration");
                }
                else
                {
                    GD.PrintErr("WaveSpawnerService: No waves found in loaded configuration");
                    _waves = new List<WaveModel>();
                }
            }
            else
            {
                GD.PrintErr("WaveSpawnerService: Failed to load wave configuration, using empty wave list");
                _waves = new List<WaveModel>();
            }
        }
        catch (Exception exception)
        {
            GD.PrintErr($"WaveSpawnerService: Exception loading wave configuration: {exception.Message}");
            _waves = new List<WaveModel>();
        }
    }

    public void StartWave(int waveNumber)
    {
        if (waveNumber <= 0)
        {
            GD.PrintErr($"WaveSpawnerService: Cannot start wave: Invalid wave number {waveNumber}");
            return;
        }

        if (_waves == null || _waves.Count == 0)
        {
            GD.PrintErr($"WaveSpawnerService: No waves loaded, cannot start wave {waveNumber}");
            return;
        }

        var waveIndex = waveNumber - 1;
        if (waveIndex >= _waves.Count)
        {
            GD.PrintErr($"WaveSpawnerService: Wave number {waveNumber} exceeds available waves (max: {_waves.Count})");
            return;
        }

        _currentWave = CloneWaveModel(_waves[waveIndex]);
        if (_currentWave == null)
        {
            GD.PrintErr($"WaveSpawnerService: Failed to clone wave data for wave {waveNumber}");
            return;
        }
        
        CurrentWave = waveNumber;
        IsSpawning = true;
        EnemiesSpawned = 0;
        TotalEnemiesInWave = CalculateTotalEnemies(_currentWave);

        GD.Print($"WaveSpawnerService: Starting wave {CurrentWave} '{_currentWave.WaveName}' with {TotalEnemiesInWave} enemies");

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
        return _waves?.Count ?? 0;
    }
    
    public bool LoadWaveSet(string difficulty)
    {
        try
        {
            var newWaveSet = _waveConfigurationService.LoadWaveSet(difficulty);
            
            if (!string.IsNullOrEmpty(newWaveSet.JsonData))
            {
                var waveSetModel = JsonSerializer.Deserialize<WaveSetModel>(newWaveSet.JsonData);
                if (waveSetModel?.Waves != null && waveSetModel.Waves.Count > 0)
                {
                    _loadedWaveSet = newWaveSet;
                    _waves = waveSetModel.Waves;
                    
                    Reset();
                    
                    GD.Print($"WaveSpawnerService: Successfully switched to wave set '{waveSetModel.SetName}' with {_waves.Count} waves");
                    return true;
                }
            }
            
            GD.PrintErr($"WaveSpawnerService: Failed to load wave set for difficulty '{difficulty}'");
            return false;
        }
        catch (Exception exception)
        {
            GD.PrintErr($"WaveSpawnerService: Exception loading wave set '{difficulty}': {exception.Message}");
            return false;
        }
    }
    
    public string[] GetAvailableWaveSets()
    {
        return _waveConfigurationService.GetAvailableWaveSets();
    }
    
    public string GetCurrentWaveSetName()
    {
        return string.IsNullOrEmpty(_loadedWaveSet.Name) ? "Unknown" : _loadedWaveSet.Name;
    }
    
    private static WaveModel? CloneWaveModel(WaveModel original)
    {
        try
        {
            var clonedWave = new WaveModel
            {
                WaveNumber = original.WaveNumber,
                WaveName = original.WaveName,
                Description = original.Description,
                PreWaveDelay = original.PreWaveDelay,
                PostWaveDelay = original.PostWaveDelay,
                BonusMoney = original.BonusMoney,
                EnemyGroups = new List<EnemySpawnGroup>()
            };

            foreach (var group in original.EnemyGroups)
            {
                var clonedGroup = new EnemySpawnGroup
                {
                    EnemyType = group.EnemyType,
                    Count = group.Count,
                    SpawnInterval = group.SpawnInterval,
                    StartDelay = group.StartDelay,
                    HealthMultiplier = group.HealthMultiplier,
                    SpeedMultiplier = group.SpeedMultiplier,
                    MoneyReward = group.MoneyReward
                };
                clonedWave.EnemyGroups.Add(clonedGroup);
            }

            return clonedWave;
        }
        catch (Exception exception)
        {
            GD.PrintErr($"WaveSpawnerService: Failed to clone wave model: {exception.Message}");
            return null;
        }
    }

    public int CurrentWaveIndex => CurrentWave;
}
