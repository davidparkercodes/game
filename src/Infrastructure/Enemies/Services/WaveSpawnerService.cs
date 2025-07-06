using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Godot;
using Game.Domain.Enemies.Services;
using Game.Domain.Enemies.ValueObjects;
using Game.Domain.Levels.ValueObjects;
using Game.Infrastructure.Waves.Models;
using Game.Infrastructure.Game.Services;
using Game.Infrastructure.Rounds.Services;
using Game.Infrastructure.Waves.Services;
using Game.Presentation.Enemies;
using Game.Presentation.Components;

namespace Game.Infrastructure.Enemies.Services;

    public class WaveSpawnerService : IWaveService
    {
        public bool IsSpawning { get; private set; } = false;
        public int CurrentWave { get; private set; } = 0;
        public int EnemiesSpawned { get; private set; } = 0;
        public int TotalEnemiesInWave { get; private set; } = 0;
        public int EnemiesKilled { get; private set; } = 0;

        private Godot.Timer? _spawnTimer;
        private WaveModel? _currentWave;
        private readonly IWaveConfigurationService _waveConfigurationService;
        private WaveConfiguration _loadedWaveSet;
        private List<WaveModel>? _waves;

    public WaveSpawnerService(IWaveConfigurationService waveConfigurationService)
    {
        _waveConfigurationService = waveConfigurationService ?? throw new ArgumentNullException(nameof(waveConfigurationService));
    }

    public void Initialize()
    {
        // Create timer but don't add to scene yet - will be added when needed
        _spawnTimer = new Godot.Timer();
        _spawnTimer.OneShot = true;
        _spawnTimer.Timeout += OnSpawnTimer;
        
        // Add timer to the main scene tree so it can be used
        var mainScene = Godot.Engine.GetMainLoop() as Godot.SceneTree;
        if (mainScene?.CurrentScene != null)
        {
            mainScene.CurrentScene.AddChild(_spawnTimer);
            GD.Print("✅ WaveSpawnerService: Timer added to scene tree");
        }
        else
        {
            GD.PrintErr("❌ WaveSpawnerService: Could not add timer to scene tree - no current scene");
        }
        
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
        try
        {
            if (waveNumber <= 0)
            {
                var errorMsg = $"WaveSpawnerService: Cannot start wave: Invalid wave number {waveNumber}";
                GD.PrintErr(errorMsg);
                throw new ArgumentException(errorMsg, nameof(waveNumber));
            }

            if (_waves == null || _waves.Count == 0)
            {
                var errorMsg = $"WaveSpawnerService: No waves loaded, cannot start wave {waveNumber}";
                GD.PrintErr(errorMsg);
                throw new InvalidOperationException(errorMsg);
            }

            if (IsSpawning)
            {
                var errorMsg = $"WaveSpawnerService: Cannot start wave {waveNumber}: Wave {CurrentWave} is already active";
                GD.PrintErr(errorMsg);
                throw new InvalidOperationException(errorMsg);
            }

            var waveIndex = waveNumber - 1;
            if (waveIndex >= _waves.Count)
            {
                var errorMsg = $"WaveSpawnerService: Wave number {waveNumber} exceeds available waves (max: {_waves.Count})";
                GD.PrintErr(errorMsg);
                throw new ArgumentOutOfRangeException(nameof(waveNumber), errorMsg);
            }

            _currentWave = CloneWaveModel(_waves[waveIndex]);
            if (_currentWave == null)
            {
                var errorMsg = $"WaveSpawnerService: Failed to clone wave data for wave {waveNumber}";
                GD.PrintErr(errorMsg);
                throw new InvalidOperationException(errorMsg);
            }
            
            CurrentWave = waveNumber;
            IsSpawning = true;
            EnemiesSpawned = 0;
            EnemiesKilled = 0;
            TotalEnemiesInWave = CalculateTotalEnemies(_currentWave);

            GD.Print($"WaveSpawnerService: Starting wave {CurrentWave} '{_currentWave.WaveName}' with {TotalEnemiesInWave} enemies");

            SpawnNextGroup();
        }
        catch (Exception exception) when (!(exception is ArgumentException || exception is ArgumentOutOfRangeException || exception is InvalidOperationException))
        {
            var errorMsg = $"WaveSpawnerService: Unexpected error starting wave {waveNumber}: {exception.Message}";
            GD.PrintErr(errorMsg);
            
            // Reset state on unexpected errors
            Reset();
            throw new InvalidOperationException(errorMsg, exception);
        }
    }
    
    public void StopCurrentWave()
    {
        StopWave();
    }
    
    public bool IsWaveActive()
    {
        return IsSpawning;
    }
    
    public int GetCurrentWaveNumber()
    {
        return CurrentWave;
    }
    
    public int GetRemainingEnemies()
    {
        if (_currentWave == null || !IsSpawning)
            return 0;
            
        // Calculate remaining enemies: total spawned minus killed
        var spawnedAlive = EnemiesSpawned - EnemiesKilled;
        
        // Add enemies still waiting to be spawned
        int waitingToSpawn = 0;
        foreach (var group in _currentWave.EnemyGroups)
        {
            waitingToSpawn += group.Count;
        }
        
        return Math.Max(0, spawnedAlive + waitingToSpawn);
    }
    
    public EnemyStats GetNextEnemyType()
    {
        if (_currentWave == null || _currentWave.EnemyGroups.Count == 0)
        {
            return EnemyStats.CreateDefault();
        }
        
        var nextGroup = _currentWave.EnemyGroups.FirstOrDefault(g => g.Count > 0);
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
            description: $"Enemy of type {nextGroup.EnemyType}"
        );
    }
    
    public bool IsWaveComplete()
    {
        return !IsSpawning && CurrentWave > 0;
    }
    
    public void LoadWaveConfiguration(LevelData levelConfiguration)
    {
        try
        {
            // Use level difficulty rating to determine wave set
            var difficultyRating = levelConfiguration.DifficultyRating;
            string difficulty = difficultyRating switch
            {
                < 1.0f => "easy",
                > 2.0f => "hard",
                _ => "default"
            };
            
            LoadWaveSet(difficulty);
            GD.Print($"WaveSpawnerService: Loaded wave set '{difficulty}' based on level difficulty rating {difficultyRating:F2}");
        }
        catch (Exception exception)
        {
            GD.PrintErr($"WaveSpawnerService: Failed to load wave configuration from level data: {exception.Message}");
            LoadWaveConfiguration();
        }
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
            if (_spawnTimer != null)
            {
                _spawnTimer.WaitTime = group.SpawnInterval;
                _spawnTimer.Start();
            }
        }
        else
        {
            CompleteWave();
        }
    }

    private void SpawnEnemy(string enemyType)
    {
        try
        {
            var spawnPosition = PathService.Instance?.GetSpawnPosition() ?? Vector2.Zero;
            GD.Print($"Spawning {enemyType} at {spawnPosition}");
            
            // Load and instantiate the enemy scene
            var enemyScene = GD.Load<PackedScene>("res://scenes/Enemies/Enemy.tscn");
            if (enemyScene == null)
            {
                GD.PrintErr($"WaveSpawnerService: Failed to load enemy scene at res://scenes/Enemies/Enemy.tscn");
                return;
            }
            
            var enemyInstance = enemyScene.Instantiate<Enemy>();
            if (enemyInstance == null)
            {
                GD.PrintErr($"WaveSpawnerService: Failed to instantiate enemy from scene");
                return;
            }
            
            // Set enemy properties
            enemyInstance.SetEnemyType(enemyType);
            enemyInstance.GlobalPosition = spawnPosition;
            
            // Add PathFollower component for movement
            var pathFollower = new PathFollower();
            pathFollower.Speed = enemyInstance.Speed;
            pathFollower.PathCompleted += () => enemyInstance.OnPathCompleted();
            enemyInstance.AddChild(pathFollower);
            
            // Get the current scene tree and add the enemy to it
            var sceneTree = Godot.Engine.GetMainLoop() as Godot.SceneTree;
            if (sceneTree?.CurrentScene != null)
            {
                sceneTree.CurrentScene.AddChild(enemyInstance);
                GD.Print($"✅ Enemy {enemyType} added to scene tree at {spawnPosition}");
                
                // Connect enemy signals for cleanup
                GD.Print($"🔗 WaveSpawnerService: Connecting signals for enemy {enemyInstance.Name}");
                enemyInstance.EnemyKilled += OnEnemyKilled;
                enemyInstance.EnemyReachedEnd += OnEnemyReachedEnd;
                GD.Print($"✅ WaveSpawnerService: Signal connections established for enemy {enemyInstance.Name}");
                
                RoundService.Instance?.OnEnemySpawned();
            }
            else
            {
                GD.PrintErr($"WaveSpawnerService: Could not add enemy to scene tree - no current scene available");
                enemyInstance?.QueueFree();
            }
        }
        catch (Exception exception)
        {
            GD.PrintErr($"WaveSpawnerService: Error spawning enemy: {exception.Message}");
        }
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
        EnemiesKilled = 0;
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
    
    private void OnEnemyKilled()
    {
        if (IsSpawning)
        {
            EnemiesKilled++;
            GD.Print($"WaveSpawnerService: Enemy killed! {EnemiesKilled}/{EnemiesSpawned} enemies killed, remaining: {GetRemainingEnemies()}");
            
            // Check if wave is complete (all enemies spawned and all killed)
            if (GetRemainingEnemies() <= 0 && !HasMoreEnemies())
            {
                GD.Print($"🎉 Wave {CurrentWave} complete! All {EnemiesKilled} enemies killed.");
                CompleteWave();
            }
        }
    }
    
    private void OnEnemyReachedEnd()
    {
        GD.Print("WaveSpawnerService: Enemy reached end");
    }
}
