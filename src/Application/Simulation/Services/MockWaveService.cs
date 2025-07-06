using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Game.Domain.Enemies.Services;
using Game.Domain.Enemies.ValueObjects;
using Game.Domain.Levels.ValueObjects;
using Game.Domain.Common.Services;
using SimulationWaveConfiguration = Game.Application.Simulation.ValueObjects.WaveConfiguration;
using SimulationWaveSetConfiguration = Game.Application.Simulation.ValueObjects.WaveSetConfiguration;
using SimulationEnemyGroupConfiguration = Game.Application.Simulation.ValueObjects.EnemyGroupConfiguration;

namespace Game.Application.Simulation.Services;

public class MockWaveService : IWaveService
{
    private readonly ILogger _logger;
    private SimulationWaveSetConfiguration? _currentWaveSet;
    private List<SimulationWaveConfiguration>? _waves;
    private int _currentWaveNumber = 0;
    private bool _isWaveActive = false;
    private bool _isWaveComplete = false;
    private int _remainingEnemies = 0;
    private int _totalWaves = 0;
    private string _currentWaveSetName = "Unknown";
    private float _enemyCountMultiplier = 1.0f;
    private int _currentEnemyGroupIndex = 0;

    public MockWaveService(ILogger? logger = null)
    {
        _logger = logger ?? new ConsoleLogger("🌊 [WAVE-SVC]");
        _logger.LogInformation("MockWaveService constructor starting");

        // Initialize fields
        _waves = null;
        _totalWaves = 0;
        _currentWaveSetName = "Unknown";

        _logger.LogInformation("About to load wave configuration");
        LoadWaveConfiguration();

        _logger.LogInformation($"MockWaveService constructor finished: waves={_waves?.Count ?? 0}, totalWaves={_totalWaves}");

        // Double-check that waves were loaded
        if (_waves == null || _waves.Count == 0 || _totalWaves == 0)
        {
            _logger.LogError("Wave loading failed completely! Forcing fallback creation...");
            CreateFallbackConfiguration();
            _logger.LogInformation($"After forced fallback: waves={_waves?.Count ?? 0}, totalWaves={_totalWaves}");
        }
    }

    public void StartWave(int waveNumber)
    {
        // Ensure we have waves loaded, create fallback if needed
        if (_waves == null || _waves.Count == 0)
        {
            Console.WriteLine("DEBUG: No waves loaded, creating fallback configuration");
            CreateFallbackConfiguration();
        }

        if (_waves == null || waveNumber <= 0 || waveNumber > _waves.Count)
        {
            Console.WriteLine($"DEBUG: Invalid wave setup - waves count: {_waves?.Count ?? 0}, requested wave: {waveNumber}");
            throw new ArgumentOutOfRangeException(nameof(waveNumber), $"Invalid wave number: {waveNumber}. Available waves: {_waves?.Count ?? 0}");
        }

        _currentWaveNumber = waveNumber;
        _isWaveActive = true;
        _isWaveComplete = false;
        _currentEnemyGroupIndex = 0;

        var currentWave = _waves[waveNumber - 1];
        var baseEnemyCount = currentWave.EnemyGroups.Sum(group => group.Count);
        _remainingEnemies = (int)(baseEnemyCount * _enemyCountMultiplier);
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

        // Cycle through enemy groups to provide variety
        if (currentWave.EnemyGroups.Count == 0)
        {
            return EnemyStats.CreateDefault();
        }

        var groupIndex = _currentEnemyGroupIndex % currentWave.EnemyGroups.Count;
        var selectedGroup = currentWave.EnemyGroups[groupIndex];

        // Move to next group for next enemy
        _currentEnemyGroupIndex++;

        return new EnemyStats(
            maxHealth: (int)(100 * selectedGroup.HealthMultiplier),
            speed: 50 * selectedGroup.SpeedMultiplier,
            damage: 10,
            rewardGold: selectedGroup.MoneyReward,
            rewardXp: 5,
            description: $"Simulated enemy of type {selectedGroup.EnemyType}"
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
        _currentEnemyGroupIndex = 0;
    }

    public void SetEnemyCountMultiplier(float multiplier)
    {
        _enemyCountMultiplier = multiplier;
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
        // Only reload if we don't already have waves loaded
        if (_waves == null || _waves.Count == 0 || _totalWaves == 0)
        {
            Console.WriteLine("🌊 MockWaveService.Initialize(): No waves loaded, reloading...");
            LoadWaveConfiguration();
        }
        else
        {
            Console.WriteLine($"🌊 MockWaveService.Initialize(): Already have {_totalWaves} waves loaded, skipping reload");
        }
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
            Console.WriteLine($"🌊 MockWaveService: Attempting to load config from: {configPath}");
            
            // Try to find the config file with multiple search paths
            var actualConfigPath = FindWaveConfigFile(configPath);
            
            if (!System.IO.File.Exists(actualConfigPath))
            {
                Console.WriteLine($"⚠️ MockWaveService: Wave config file not found at {actualConfigPath}, creating fallback");
                CreateFallbackConfiguration();
                return;
            }
            
            var configContent = System.IO.File.ReadAllText(actualConfigPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };

            _currentWaveSet = JsonSerializer.Deserialize<SimulationWaveSetConfiguration>(configContent, options);

            if (_currentWaveSet?.Waves != null)
            {
                _waves = _currentWaveSet.Waves;
                _totalWaves = _waves.Count;
                _currentWaveSetName = _currentWaveSet.SetName ?? difficulty;
                Console.WriteLine($"✅ MockWaveService: Loaded {_totalWaves} waves from config");
            }
            else
            {
                Console.WriteLine("⚠️ MockWaveService: Wave set loaded but no waves found, creating fallback");
                CreateFallbackConfiguration();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ MockWaveService: Failed to load wave configuration: {ex.Message}");
            CreateFallbackConfiguration();
        }
    }
    
    private static string FindWaveConfigFile(string relativePath)
    {
        // First try the relative path as-is
        if (System.IO.File.Exists(relativePath))
        {
            return relativePath;
        }
        
        // Try looking in common directories relative to current directory
        var searchPaths = new[]
        {
            relativePath,
            System.IO.Path.Combine("..", relativePath),
            System.IO.Path.Combine("..", "..", relativePath),
            System.IO.Path.Combine("..", "..", "..", relativePath),
            System.IO.Path.Combine("..", "..", "..", "..", relativePath),
            System.IO.Path.Combine("..", "..", "..", "..", "..", relativePath),
            System.IO.Path.Combine(Environment.CurrentDirectory, relativePath),
        };
        
        foreach (var searchPath in searchPaths)
        {
            if (System.IO.File.Exists(searchPath))
            {
                return searchPath;
            }
        }
        
        // Return the original path if nothing found (will trigger fallback)
        return relativePath;
    }

    private void CreateFallbackConfiguration()
    {
        Console.WriteLine("🔧 MockWaveService: Creating fallback wave configuration");
        _waves = new List<SimulationWaveConfiguration>();

        // Create 15 fallback waves to support typical simulation scenarios
        for (int waveNum = 1; waveNum <= 15; waveNum++)
        {
            var waveConfig = new SimulationWaveConfiguration
            {
                WaveNumber = waveNum,
                WaveName = $"Fallback Wave {waveNum}",
                Description = $"Auto-generated fallback wave {waveNum}",
                EnemyGroups = new List<SimulationEnemyGroupConfiguration>
                {
                    new SimulationEnemyGroupConfiguration
                    {
                        EnemyType = "basic_enemy",
                        Count = 5 + (waveNum * 2), // Scale enemy count with wave number
                        SpawnInterval = 1.0f,
                        HealthMultiplier = 1.0f + (waveNum * 0.1f), // Increase health each wave
                        SpeedMultiplier = 1.0f + (waveNum * 0.05f), // Slightly increase speed
                        MoneyReward = 10 + waveNum // Increase reward per wave
                    }
                }
            };
            _waves.Add(waveConfig);
        }
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
