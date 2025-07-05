using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Game.Application.Simulation.ValueObjects;
using Game.Application.Simulation.Services;
using Game.Domain.Shared.ValueObjects;

namespace Game.Application.Simulation;

public class GameSimRunner
{
    private readonly MockBuildingStatsProvider _buildingStatsProvider;
    private readonly MockEnemyStatsProvider _enemyStatsProvider;
    private Random _random;

    public GameSimRunner()
    {
        _buildingStatsProvider = new MockBuildingStatsProvider();
        _enemyStatsProvider = new MockEnemyStatsProvider();
        _random = new Random();
    }

    public SimulationResult RunSimulation(SimulationConfig config, IProgress<SimulationProgress>? progress = null)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            // Set random seed for deterministic results
            _random = new Random(config.RandomSeed);
            
            // Apply config modifiers to mock services
            _buildingStatsProvider.SetCostMultiplier(config.BuildingCostMultiplier);
            _enemyStatsProvider.SetHealthMultiplier(config.EnemyHealthMultiplier);
            _enemyStatsProvider.SetSpeedMultiplier(config.EnemySpeedMultiplier);

            // Initialize game state
            var gameState = new GameState(config.StartingMoney, config.StartingLives);
            var waveResults = new List<WaveResult>();

            // Report initial progress
            progress?.Report(new SimulationProgress(0, gameState.Money, gameState.Lives));

            // Run simulation waves
            for (int wave = 1; wave <= config.MaxWaves && !gameState.IsGameOver; wave++)
            {
                var waveResult = RunWave(gameState, wave, config);
                waveResults.Add(waveResult);

                // Report progress after each wave
                progress?.Report(new SimulationProgress(wave, gameState.Money, gameState.Lives));

                if (!waveResult.Completed)
                {
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

            // Check final result
            if (gameState.IsVictory)
            {
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

    private WaveResult RunWave(GameState gameState, int waveNumber, SimulationConfig config)
    {
        var waveStopwatch = Stopwatch.StartNew();
        var startingLives = gameState.Lives;
        var startingMoney = gameState.Money;
        var startingScore = gameState.Score;

        gameState.StartWave(waveNumber);

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
        var moneyEarned = enemiesKilled * 10; // Base reward per enemy
        var scoreEarned = enemiesKilled * 100 + (waveNumber * 50); // Wave bonus
        
        gameState.AddMoney(moneyEarned);
        gameState.AddScore(scoreEarned);
        gameState.CompleteWave(config.MaxWaves);

        waveStopwatch.Stop();

        var livesLost = startingLives - gameState.Lives;
        var waveCompleted = gameState.Lives > 0;

        return new WaveResult(
            waveNumber: waveNumber,
            completed: waveCompleted,
            enemiesKilled: enemiesKilled,
            livesLost: livesLost,
            moneyEarned: gameState.Money - startingMoney,
            scoreEarned: gameState.Score - startingScore,
            waveDuration: waveStopwatch.Elapsed
        );
    }

    private void PlaceInitialBuildings(GameState gameState, int waveNumber)
    {
        // Simple strategy: Place 2 basic towers at strategic positions
        var positions = new[]
        {
            new Position(200, 200),
            new Position(400, 200)
        };

        foreach (var position in positions)
        {
            if (TryPlaceBuilding(gameState, "basic_tower", position))
            {
                // Building placed successfully
            }
        }
    }

    private void PlaceAdditionalBuildings(GameState gameState, int waveNumber)
    {
        // Adaptive strategy: upgrade or place new buildings based on money
        if (gameState.Money >= 100 && waveNumber >= 3)
        {
            // Try to place a sniper tower
            TryPlaceBuilding(gameState, "sniper_tower", new Position(300, 150));
        }
        else if (gameState.Money >= 75 && waveNumber >= 2)
        {
            // Try to place a rapid tower
            TryPlaceBuilding(gameState, "rapid_tower", new Position(250, 250));
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
                stats.FireRate,
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
        var enemyCount = 5 + (waveNumber - 1) * 2; // Increase enemies per wave
        var spawnPosition = new Position(0, 250); // Start position

        for (int i = 0; i < enemyCount; i++)
        {
            string enemyType = GetEnemyTypeForWave(waveNumber, i);
            var enemyStats = _enemyStatsProvider.GetScaledStatsForWave(enemyType, waveNumber);

            var enemy = new SimulatedEnemy(
                enemyType,
                enemyStats.MaxHealth,
                enemyStats.Speed,
                enemyStats.RewardGold,
                spawnPosition
            );

            enemies.Add(enemy);
        }

        return enemies;
    }

    private string GetEnemyTypeForWave(int waveNumber, int enemyIndex)
    {
        // Progressive enemy type introduction
        if (waveNumber >= 8 && enemyIndex == 0)
            return "boss_enemy";
        else if (waveNumber >= 6 && enemyIndex % 4 == 0)
            return "elite_enemy";
        else if (waveNumber >= 4 && enemyIndex % 3 == 0)
            return "tank_enemy";
        else if (waveNumber >= 2 && enemyIndex % 2 == 0)
            return "fast_enemy";
        else
            return "basic_enemy";
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
                }
            }
        }

        return enemiesKilled;
    }

    private SimulatedEnemy FindTargetInRange(SimulatedBuilding building, List<SimulatedEnemy> enemies)
    {
        SimulatedEnemy closestEnemy = null;
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
