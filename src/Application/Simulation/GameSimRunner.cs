using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Game.Application.Simulation.ValueObjects;
using Game.Application.Simulation.Services;
using Game.Domain.Shared.ValueObjects;
using Game.Domain.Buildings.Services;
using Game.Domain.Enemies.Services;
using Game.Application.Buildings.Services;
using Game.Domain.Common.Services;

namespace Game.Application.Simulation;

public class GameSimRunner
{
    private readonly MockBuildingStatsProvider _buildingStatsProvider;
    private readonly MockEnemyStatsProvider _enemyStatsProvider;
    private readonly MockWaveService _waveService;
    private readonly IBuildingTypeRegistry _buildingTypeRegistry;
    private readonly IPlacementStrategyProvider _placementStrategyProvider;
    private readonly IWaveMetricsCollector _metricsCollector;
    private readonly ILogger _logger;
    private Random _random;

    public GameSimRunner(ILogger? logger = null)
    {
        _logger = logger ?? new ConsoleLogger("[GAMESIM]", LogLevel.Error); // Only show errors during tests
        _buildingStatsProvider = new MockBuildingStatsProvider(null, null, _logger);
        _enemyStatsProvider = new MockEnemyStatsProvider();
        _waveService = new MockWaveService(_logger);
        _buildingTypeRegistry = new BuildingTypeRegistry(_buildingStatsProvider);
        _placementStrategyProvider = new PlacementStrategyProvider(_buildingTypeRegistry);
        _metricsCollector = new WaveMetricsCollector();
        _random = new Random();
    }

    public SimulationResult RunSimulation(SimulationConfig config, IProgress<SimulationProgress>? progress = null)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            _logger.LogDebug($"Starting simulation with MaxWaves: {config.MaxWaves}, WaveSetDifficulty: {config.WaveSetDifficulty}");
            
            // Set random seed for deterministic results
            _random = new Random(config.RandomSeed);
            
            // Apply config modifiers to mock services
            _buildingStatsProvider.SetCostMultiplier(config.BuildingCostMultiplier);
            _buildingStatsProvider.SetDamageMultiplier(config.BuildingDamageMultiplier);
            _enemyStatsProvider.SetHealthMultiplier(config.EnemyHealthMultiplier);
            _enemyStatsProvider.SetSpeedMultiplier(config.EnemySpeedMultiplier);
            
            // Load wave configuration based on difficulty setting
            _waveService.LoadWaveSet(config.WaveSetDifficulty);
            _waveService.SetEnemyCountMultiplier(config.EnemyCountMultiplier);
            
            _logger.LogDebug($"Wave service loaded with {_waveService.GetTotalWaves()} total waves available");

            // Initialize game state and metrics
            var gameState = new GameState(config.StartingMoney, config.StartingLives);
            var waveResults = new List<WaveResult>();
            _metricsCollector.Reset();
            
            _logger.LogDebug($"Initialized game state - Money: {gameState.Money}, Lives: {gameState.Lives}");

            // Report initial progress
            progress?.Report(new SimulationProgress(0, gameState.Money, gameState.Lives));

            // Run simulation waves
            for (int wave = 1; wave <= config.MaxWaves && !gameState.IsGameOver; wave++)
            {
                _logger.LogDebug($"Starting wave {wave}/{config.MaxWaves}");
                var waveResult = RunWave(gameState, wave, config);
                waveResults.Add(waveResult);
                
                _logger.LogDebug($"Wave {wave} result - Completed: {waveResult.Completed}, EnemiesKilled: {waveResult.EnemiesKilled}, LivesLost: {waveResult.LivesLost}");
                _logger.LogDebug($"Game state after wave {wave} - Money: {gameState.Money}, Lives: {gameState.Lives}, IsGameOver: {gameState.IsGameOver}, IsVictory: {gameState.IsVictory}");

                // Report progress after each wave
                progress?.Report(new SimulationProgress(wave, gameState.Money, gameState.Lives));

                if (!waveResult.Completed)
                {
                    _logger.LogDebug($"Wave {wave} failed to complete, ending simulation");
                    stopwatch.Stop();
                    return SimulationResult.Failure(
                        $"Failed to complete wave {wave}",
                        gameState,
                        stopwatch.Elapsed,
                        waveResults
                    );
                }
            }

            stopwatch.Stop();
            
            _logger.LogDebug($"Simulation loop completed. Final state - CurrentWave: {gameState.CurrentWave}, IsGameOver: {gameState.IsGameOver}, IsVictory: {gameState.IsVictory}");
            _logger.LogDebug($"Wave results count: {waveResults.Count}");

            // Check final result
            if (gameState.IsVictory)
            {
                _logger.LogDebug($"Simulation succeeded - Victory achieved!");
                return SimulationResult.CreateSuccess(
                    finalMoney: gameState.Money,
                    finalLives: gameState.Lives,
                    finalScore: gameState.Score,
                    wavesCompleted: gameState.CurrentWave,
                    totalEnemiesKilled: CalculateEnemiesKilled(waveResults),
                    totalBuildingsPlaced: gameState.Buildings.Count,
                    duration: stopwatch.Elapsed,
                    waveResults: waveResults
                );
            }
            else
            {
                _logger.LogDebug($"Simulation failed - No victory condition met");
                return SimulationResult.Failure(
                    "Game ended without victory",
                    gameState,
                    stopwatch.Elapsed,
                    waveResults
                );
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return SimulationResult.Failure(
                $"Simulation error: {ex.Message}",
                new GameState(0, 0),
                stopwatch.Elapsed
            );
        }
    }

    public async Task<SimulationResult> RunSimulationAsync(SimulationConfig config, IProgress<SimulationProgress>? progress = null)
    {
        return await Task.Run(() => RunSimulation(config, progress));
    }

    public List<SimulationResult> RunMultipleScenarios(List<SimulationConfig> scenarios)
    {
        var results = new List<SimulationResult>();
        
        foreach (var scenario in scenarios)
        {
            var result = RunSimulation(scenario);
            results.Add(result);
        }

        return results;
    }
    
    public SimulationMetrics GetSimulationMetrics(string scenarioName, TimeSpan totalDuration, bool overallSuccess)
    {
        return _metricsCollector.GenerateSimulationMetrics(scenarioName, totalDuration, overallSuccess);
    }
    
    public void ExportMetrics(string filePath, SimulationMetrics metrics)
    {
        _metricsCollector.ExportMetrics(filePath, metrics);
    }

    private WaveResult RunWave(GameState gameState, int waveNumber, SimulationConfig config)
    {
        var waveStopwatch = Stopwatch.StartNew();
        var startingLives = gameState.Lives;
        var startingMoney = gameState.Money;
        var startingScore = gameState.Score;

        gameState.StartWave(waveNumber);
        
        // Start metrics tracking for this wave
        var waveName = $"Wave {waveNumber}";
        _metricsCollector.StartWaveTracking(waveNumber, waveName);

        // Phase 1: Pre-wave building placement
        if (waveNumber == 1)
        {
            PlaceInitialBuildings(gameState, waveNumber);
        }
        else
        {
            PlaceAdditionalBuildings(gameState, waveNumber);
        }

        // Phase 2: Spawn enemies for this wave
        var enemies = SpawnEnemiesForWave(gameState, waveNumber);
        var enemiesKilled = 0;
        
        // Track enemy spawns for metrics
        foreach (var enemy in enemies)
        {
            _metricsCollector.RecordEnemySpawn("simulated_enemy", waveStopwatch.Elapsed);
        }

        // Phase 3: Combat simulation
        while (enemies.Count > 0 && gameState.Lives > 0)
        {
            // Simulate one combat "tick"
            var enemiesKilledThisTick = SimulateCombatTick(gameState, enemies);
            enemiesKilled += enemiesKilledThisTick;

            // Move remaining enemies
            MoveEnemies(enemies);

            // Check if any enemies reached the end
            var enemiesReachedEnd = CheckEnemiesReachedEnd(enemies);
            foreach (var enemy in enemiesReachedEnd)
            {
                gameState.LoseLife();
                enemies.Remove(enemy);
            }

            // Remove dead enemies
            enemies.RemoveAll(e => !e.IsAlive);
        }

        // Phase 4: Wave completion rewards
        // Calculate base rewards from enemies killed
        var baseMoneyFromEnemies = enemiesKilled * 10; 
        var baseScoreFromEnemies = enemiesKilled * 100 + (waveNumber * 50);
        
        // Add wave completion bonus (simulates bonus money from wave configuration)
        var waveBonusMoney = waveNumber * 25; // Progressive bonus per wave
        
        gameState.AddMoney(baseMoneyFromEnemies + waveBonusMoney);
        gameState.AddScore(baseScoreFromEnemies);
        gameState.CompleteWave(config.MaxWaves);
        
        // Complete the wave in the wave service
        _waveService.StopCurrentWave();

        waveStopwatch.Stop();

        var livesLost = startingLives - gameState.Lives;
        var waveCompleted = gameState.Lives > 0;
        var totalEnemies = enemies.Count + enemiesKilled;
        var enemiesLeaked = totalEnemies - enemiesKilled;
        var moneyEarned = gameState.Money - startingMoney;
        
        // Complete metrics tracking for this wave
        _metricsCollector.EndWaveTracking(totalEnemies, enemiesKilled, enemiesLeaked, moneyEarned, livesLost);

        return new WaveResult(
            waveNumber: waveNumber,
            completed: waveCompleted,
            enemiesKilled: enemiesKilled,
            livesLost: livesLost,
            moneyEarned: moneyEarned,
            scoreEarned: gameState.Score - startingScore,
            waveDuration: waveStopwatch.Elapsed
        );
    }

    private void PlaceInitialBuildings(GameState gameState, int waveNumber)
    {
        _logger.LogDebug($"Placing initial buildings for wave {waveNumber}");
        
        // Fully config-driven strategy using PlacementStrategyProvider
        var positions = _placementStrategyProvider.GetInitialBuildingPositions();
        var buildingCategory = _placementStrategyProvider.GetInitialBuildingCategory();
        var maxCost = _placementStrategyProvider.GetMaxCostPerBuilding();
        
        _logger.LogDebug($"Placement config - Category: {buildingCategory}, MaxCost: {maxCost}, Positions: {positions.Count()}");

        // Get buildings from the specified category
        var categoryBuildings = _buildingTypeRegistry.GetByCategory(buildingCategory).ToList();
        var initialBuildingType = categoryBuildings.Any() ? categoryBuildings.First().ConfigKey : null;
        
        _logger.LogDebug($"Category '{buildingCategory}' has {categoryBuildings.Count} buildings available");
        
        // Use fallback strategy if no category buildings found
        if (string.IsNullOrEmpty(initialBuildingType))
        {
            _logger.LogDebug($"No buildings found in category '{buildingCategory}', using fallback");
            initialBuildingType = _placementStrategyProvider.GetFallbackBuildingType();
        }
        
        _logger.LogDebug($"Selected building type: {initialBuildingType}");

        int buildingsPlaced = 0;
        foreach (var position in positions)
        {
            // Check cost constraint from strategy
            var stats = _buildingStatsProvider.GetBuildingStats(initialBuildingType);
            _logger.LogDebug($"Trying to place {initialBuildingType} at {position} - Cost: {stats.Cost}, MaxCost: {maxCost}, GameMoney: {gameState.Money}");
            
            if (stats.Cost <= maxCost)
            {
                if (TryPlaceBuilding(gameState, initialBuildingType, position))
                {
                    buildingsPlaced++;
                    _logger.LogDebug($"Successfully placed building {buildingsPlaced} at {position}");
                }
                else
                {
                    _logger.LogDebug($"Failed to place building at {position} - insufficient money");
                }
            }
            else
            {
                _logger.LogDebug($"Building cost {stats.Cost} exceeds max cost {maxCost}");
            }
        }
        
        _logger.LogDebug($"Initial building placement complete - {buildingsPlaced} buildings placed, total buildings: {gameState.Buildings.Count}");
    }

    private void PlaceAdditionalBuildings(GameState gameState, int waveNumber)
    {
        // Fully config-driven strategy using PlacementStrategyProvider
        var upgradeBuildingCategory = _placementStrategyProvider.GetUpgradeBuildingCategory(waveNumber, gameState.Money);
        var upgradeBuildingPosition = _placementStrategyProvider.GetUpgradeBuildingPosition(waveNumber);
        
        if (!string.IsNullOrEmpty(upgradeBuildingCategory) && upgradeBuildingPosition != null)
        {
            // Get buildings from the specified upgrade category
            var categoryBuildings = _buildingTypeRegistry.GetByCategory(upgradeBuildingCategory).ToList();
            var upgradeBuildingType = categoryBuildings.Any() ? categoryBuildings.First().ConfigKey : null;
            
            // Use fallback if no category buildings found
            if (string.IsNullOrEmpty(upgradeBuildingType))
            {
                upgradeBuildingType = _placementStrategyProvider.GetFallbackBuildingType();
            }
            
            if (upgradeBuildingPosition != null)
            {
                TryPlaceBuilding(gameState, upgradeBuildingType, upgradeBuildingPosition.Value);
            }
        }
    }

    private bool TryPlaceBuilding(GameState gameState, string buildingType, Position position)
    {
        var stats = _buildingStatsProvider.GetBuildingStats(buildingType);
        
        if (gameState.Money >= stats.Cost)
        {
            var building = new SimulatedBuilding(
                buildingType,
                position,
                stats.Damage,
                stats.Range,
                stats.AttackSpeed,
                stats.Cost
            );

            gameState.SpendMoney(stats.Cost);
            gameState.AddBuilding(building);
            return true;
        }

        return false;
    }

    private List<SimulatedEnemy> SpawnEnemiesForWave(GameState gameState, int waveNumber)
    {
        var enemies = new List<SimulatedEnemy>();
        var spawnPosition = new Position(0, 250); // Start position

        _logger.LogDebug($"Spawning enemies for wave {waveNumber}");
        
        // Start the wave in the wave service to get configuration
        _waveService.StartWave(waveNumber);
        
        // Get the total number of enemies from wave configuration
        var totalEnemiesInWave = _waveService.GetRemainingEnemies();
        
        _logger.LogDebug($"Wave {waveNumber} should spawn {totalEnemiesInWave} enemies");
        
        // Create enemies based on wave configuration
        for (int i = 0; i < totalEnemiesInWave; i++)
        {
            // Get enemy stats for this enemy (wave service handles enemy type distribution)
            var enemyStats = _waveService.GetNextEnemyType();
            
            var enemy = new SimulatedEnemy(
                $"wave_{waveNumber}_enemy_{i}", // Unique identifier for simulation
                enemyStats.MaxHealth,
                enemyStats.Speed,
                enemyStats.RewardGold,
                spawnPosition
            );

            enemies.Add(enemy);
        }
        
        _logger.LogDebug($"Successfully spawned {enemies.Count} enemies for wave {waveNumber}");

        return enemies;
    }

    private string GetEnemyTypeForWave(int waveNumber, int enemyIndex)
    {
        // Use EnemyTypeRegistry's built-in wave progression logic
        var enemyType = _enemyStatsProvider.EnemyTypeRegistry.GetEnemyTypeForWaveProgression(waveNumber, enemyIndex);
        
        // Return config key, with fallback to domain constant as last resort
        return enemyType.HasValue ? enemyType.Value.ConfigKey : Domain.Entities.EnemyConfigKeys.BasicEnemy;
    }

    private int SimulateCombatTick(GameState gameState, List<SimulatedEnemy> enemies)
    {
        int enemiesKilled = 0;

        foreach (var building in gameState.Buildings)
        {
            // Find enemies in range
            var target = FindTargetInRange(building, enemies);
            if (target != null)
            {
                // Simulate shooting
                target.TakeDamage(building.Damage);
                if (!target.IsAlive)
                {
                    enemiesKilled++;
                    gameState.AddMoney(target.Reward);
                    _waveService.OnEnemyKilled(); // Notify wave service
                    _metricsCollector.RecordEnemyKill("simulated_enemy", TimeSpan.FromMilliseconds(0)); // Track kill
                }
            }
        }

        return enemiesKilled;
    }

    private SimulatedEnemy? FindTargetInRange(SimulatedBuilding building, List<SimulatedEnemy> enemies)
    {
        SimulatedEnemy? closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (var enemy in enemies)
        {
            if (!enemy.IsAlive) continue;

            var distance = building.Position.DistanceTo(enemy.Position);
            if (distance <= building.Range && distance < closestDistance)
            {
                closestEnemy = enemy;
                closestDistance = distance;
            }
        }

        return closestEnemy;
    }

    private void MoveEnemies(List<SimulatedEnemy> enemies)
    {
        // Simple movement: move enemies toward the goal
        var goalPosition = new Position(800, 250);

        foreach (var enemy in enemies)
        {
            if (!enemy.IsAlive) continue;

            // Simple linear movement toward goal
            var direction = new Position(
                goalPosition.X - enemy.Position.X,
                goalPosition.Y - enemy.Position.Y
            );

            var distance = enemy.Position.DistanceTo(goalPosition);
            if (distance > 0)
            {
                var moveDistance = Math.Min(enemy.Speed * 0.1f, distance); // 0.1 = time delta
                var normalizedX = direction.X / distance;
                var normalizedY = direction.Y / distance;

                var newPosition = new Position(
                    enemy.Position.X + normalizedX * moveDistance,
                    enemy.Position.Y + normalizedY * moveDistance
                );

                enemy.Move(newPosition);
            }
        }
    }

    private List<SimulatedEnemy> CheckEnemiesReachedEnd(List<SimulatedEnemy> enemies)
    {
        var goalPosition = new Position(800, 250);
        var reachedEnd = new List<SimulatedEnemy>();

        foreach (var enemy in enemies)
        {
            if (enemy.Position.DistanceTo(goalPosition) < 10) // Within 10 units of goal
            {
                reachedEnd.Add(enemy);
            }
        }

        return reachedEnd;
    }

    private int CalculateEnemiesKilled(List<WaveResult> waveResults)
    {
        int total = 0;
        foreach (var wave in waveResults)
        {
            total += wave.EnemiesKilled;
        }
        return total;
    }
}
