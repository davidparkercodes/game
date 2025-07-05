using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Game.Domain.Enemies.Services;
using Game.Domain.Enemies.ValueObjects;
using Game.Domain.Levels.ValueObjects;
using SimulationWaveConfiguration = Game.Application.Simulation.ValueObjects.WaveConfiguration;
using SimulationWaveSetConfiguration = Game.Application.Simulation.ValueObjects.WaveSetConfiguration;
using SimulationEnemyGroupConfiguration = Game.Application.Simulation.ValueObjects.EnemyGroupConfiguration;

namespace Game.Application.Simulation.Services;

public class MockWaveService : IWaveService
{
    private SimulationWaveSetConfiguration? _currentWaveSet;
    private List<SimulationWaveConfiguration>? _waves;
    private int _currentWaveNumber = 0;
    private bool _isWaveActive = false;
    private bool _isWaveComplete = false;
    private int _remainingEnemies = 0;
    private int _totalWaves = 0;
    private string _currentWaveSetName = "Unknown";

    public MockWaveService()
    {
        LoadWaveConfiguration();
    }

    public void StartWave(int waveNumber)
    {
        if (_waves == null || waveNumber <= 0 || waveNumber > _waves.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(waveNumber), $"Invalid wave number: {waveNumber}");
        }

        _currentWaveNumber = waveNumber;
        _isWaveActive = true;
        _isWaveComplete = false;
        
        var currentWave = _waves[waveNumber - 1];
        _remainingEnemies = currentWave.EnemyGroups.Sum(group => group.Count);
    }

    public void StopCurrentWave()
    {
        _isWaveActive = false;
        _isWaveComplete = true;
        _remainingEnemies = 0;
    }

    public bool IsWaveActive()
    {
        return _isWaveActive;
    }

    public int GetCurrentWaveNumber()
    {
        return _currentWaveNumber;
    }

    public int GetRemainingEnemies()
    {
        return _remainingEnemies;
    }

    public float GetWaveProgress()
    {
        if (_waves == null || _currentWaveNumber <= 0 || _currentWaveNumber > _waves.Count)
            return 0f;

        var currentWave = _waves[_currentWaveNumber - 1];
        var totalEnemies = currentWave.EnemyGroups.Sum(group => group.Count);
        
        if (totalEnemies == 0)
            return 1f;

        return 1f - ((float)_remainingEnemies / totalEnemies);
    }

    public EnemyStats GetNextEnemyType()
    {
        if (_waves == null || _currentWaveNumber <= 0 || _currentWaveNumber > _waves.Count)
        {
            return EnemyStats.CreateDefault();
        }

        var currentWave = _waves[_currentWaveNumber - 1];
        var nextGroup = currentWave.EnemyGroups.FirstOrDefault(g => g.Count > 0);
        
        if (nextGroup == null)
        {
            return EnemyStats.CreateDefault();
        }

        return new EnemyStats(
            maxHealth: (int)(100 * nextGroup.HealthMultiplier),
            speed: 50 * nextGroup.SpeedMultiplier,
            damage: 10,
            rewardGold: nextGroup.MoneyReward,
            rewardXp: 5,
            description: $"Simulated enemy of type {nextGroup.EnemyType}"
        );
    }

    public bool IsWaveComplete()
    {
        return _isWaveComplete;
    }

    public void LoadWaveConfiguration(LevelData levelConfiguration)
    {
        var difficulty = levelConfiguration.DifficultyRating switch
        {
            < 1.0f => "easy",
            > 2.0f => "hard", 
            _ => "default"
        };
        
        LoadWaveSet(difficulty);
    }

    public void PauseWave()
    {
        // No-op for simulation
    }

    public void ResumeWave()
    {
        // No-op for simulation
    }

    public void Reset()
    {
        _currentWaveNumber = 0;
        _isWaveActive = false;
        _isWaveComplete = false;
        _remainingEnemies = 0;
    }

    public int GetTotalWaves()
    {
        return _totalWaves;
    }

    public bool LoadWaveSet(string difficulty)
    {
        try
        {
            LoadWaveConfiguration(difficulty);
            return _waves != null && _waves.Count > 0;
        }
        catch
        {
            return false;
        }
    }

    public string[] GetAvailableWaveSets()
    {
        return new[] { "default", "easy", "hard", "balance-testing" };
    }

    public string GetCurrentWaveSetName()
    {
        return _currentWaveSetName;
    }

    public void Initialize()
    {
        LoadWaveConfiguration();
    }

    public void OnEnemyKilled()
    {
        if (_remainingEnemies > 0)
        {
            _remainingEnemies--;
            
            if (_remainingEnemies == 0)
            {
                _isWaveActive = false;
                _isWaveComplete = true;
            }
        }
    }

    private void LoadWaveConfiguration(string difficulty = "default")
    {
        try
        {
            var configPath = GetWaveConfigPath(difficulty);
            var configContent = System.IO.File.ReadAllText(configPath);
            
            _currentWaveSet = JsonSerializer.Deserialize<SimulationWaveSetConfiguration>(configContent);
            
            if (_currentWaveSet?.Waves != null)
            {
                _waves = _currentWaveSet.Waves;
                _totalWaves = _waves.Count;
                _currentWaveSetName = _currentWaveSet.SetName ?? difficulty;
            }
            else
            {
                CreateFallbackConfiguration();
            }
        }
        catch (Exception)
        {
            CreateFallbackConfiguration();
        }
    }

    private void CreateFallbackConfiguration()
    {
        _waves = new List<SimulationWaveConfiguration>
        {
            new SimulationWaveConfiguration
            {
                WaveNumber = 1,
                WaveName = "Basic Wave",
                Description = "Fallback wave configuration",
                EnemyGroups = new List<SimulationEnemyGroupConfiguration>
                {
                    new SimulationEnemyGroupConfiguration
                    {
                        EnemyType = "basic_enemy",
                        Count = 5,
                        SpawnInterval = 1.0f,
                        HealthMultiplier = 1.0f,
                        SpeedMultiplier = 1.0f,
                        MoneyReward = 10
                    }
                }
            }
        };
        _totalWaves = _waves.Count;
        _currentWaveSetName = "Fallback";
    }

    private static string GetWaveConfigPath(string difficulty)
    {
        var basePath = "data/simulation";
        return difficulty switch
        {
            "balance-testing" => $"{basePath}/wave-configs-balance.json",
            "easy" => $"{basePath}/wave-configs-easy.json", 
            "hard" => $"{basePath}/wave-configs-hard.json",
            _ => $"{basePath}/wave-configs.json"
        };
    }
}
