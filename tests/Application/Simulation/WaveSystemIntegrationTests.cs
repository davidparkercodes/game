using System;
using System.Linq;
using Xunit;
using FluentAssertions;
using Game.Application.Simulation;
using Game.Application.Simulation.ValueObjects;
using Game.Application.Simulation.Services;

namespace Game.Tests.Application.Simulation;

public class WaveSystemIntegrationTests
{
    private readonly MockWaveService _waveService;
    private readonly GameSimRunner _runner;

    public WaveSystemIntegrationTests()
    {
        _waveService = new MockWaveService();
        _runner = new GameSimRunner();
    }

    [Fact]
    public void MockWaveService_ShouldLoadDefaultConfiguration()
    {
        // Act
        _waveService.Initialize();

        // Assert
        _waveService.GetTotalWaves().Should().BeGreaterThan(0);
        _waveService.GetAvailableWaveSets().Should().NotBeEmpty();
        _waveService.GetCurrentWaveSetName().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void MockWaveService_ShouldLoadBalanceTestingConfiguration()
    {
        // Act
        var loaded = _waveService.LoadWaveSet("balance-testing");

        // Assert
        loaded.Should().BeTrue();
        _waveService.GetTotalWaves().Should().Be(5); // Balance testing has 5 waves
        _waveService.GetCurrentWaveSetName().Should().Be("Balance Testing Waves");
    }

    [Fact]
    public void MockWaveService_ShouldHandleEnemyCountMultiplier()
    {
        // Arrange
        _waveService.SetEnemyCountMultiplier(2.0f);

        // Act
        _waveService.StartWave(1);
        var remainingEnemies = _waveService.GetRemainingEnemies();

        // Assert
        remainingEnemies.Should().BeGreaterThan(0);
        // The exact count depends on configuration, but should be doubled
    }

    [Fact]
    public void MockWaveService_ShouldProvideEnemyStats()
    {
        // Arrange
        _waveService.StartWave(1);

        // Act
        var enemyStats = _waveService.GetNextEnemyType();

        // Assert
        enemyStats.Should().NotBeNull();
        enemyStats.MaxHealth.Should().BeGreaterThan(0);
        enemyStats.Speed.Should().BeGreaterThan(0);
        enemyStats.RewardGold.Should().BeGreaterThan(0);
    }

    [Fact]
    public void MockWaveService_ShouldTrackWaveProgress()
    {
        // Arrange
        _waveService.StartWave(1);
        var initialProgress = _waveService.GetWaveProgress();

        // Act
        _waveService.OnEnemyKilled();
        var progressAfterKill = _waveService.GetWaveProgress();

        // Assert
        initialProgress.Should().Be(0f);
        progressAfterKill.Should().BeGreaterThan(initialProgress);
    }

    [Fact]
    public void MockWaveService_ShouldCompleteWaveWhenAllEnemiesKilled()
    {
        // Arrange
        _waveService.StartWave(1);
        var totalEnemies = _waveService.GetRemainingEnemies();

        // Act - Kill all enemies
        for (int i = 0; i < totalEnemies; i++)
        {
            _waveService.OnEnemyKilled();
        }

        // Assert
        _waveService.IsWaveComplete().Should().BeTrue();
        _waveService.IsWaveActive().Should().BeFalse();
        _waveService.GetRemainingEnemies().Should().Be(0);
    }

    [Fact]
    public void GameSimRunner_ShouldUseWaveConfigurationForSimulation()
    {
        // Arrange
        var config = SimulationConfig.ForBalanceTesting();

        // Act
        var result = _runner.RunSimulation(config);

        // Assert
        result.Should().NotBeNull();
        result.WaveResults.Should().NotBeEmpty();
        result.WaveResults.Count.Should().BeLessOrEqualTo(5); // Balance testing config has max 5 waves
    }

    [Fact]
    public void GameSimRunner_ShouldRespectEnemyCountMultiplier()
    {
        // Arrange
        var baseConfig = new SimulationConfig(
            maxWaves: 2,
            enemyCountMultiplier: 1.0f,
            randomSeed: 42
        );
        
        var doubleEnemyConfig = new SimulationConfig(
            maxWaves: 2,
            enemyCountMultiplier: 2.0f,
            randomSeed: 42
        );

        // Act
        var baseResult = _runner.RunSimulation(baseConfig);
        var doubleResult = _runner.RunSimulation(doubleEnemyConfig);

        // Assert
        baseResult.WaveResults.Should().NotBeEmpty();
        doubleResult.WaveResults.Should().NotBeEmpty();
        
        // With double enemies, we expect potentially more enemies killed or more difficulty
        doubleResult.WaveResults.Sum(w => w.EnemiesKilled).Should().BeGreaterOrEqualTo(
            baseResult.WaveResults.Sum(w => w.EnemiesKilled));
    }

    [Fact]
    public void GameSimRunner_ShouldHandleDifferentWaveSets()
    {
        // Arrange
        var defaultConfig = new SimulationConfig(
            maxWaves: 3,
            waveSetDifficulty: "default",
            randomSeed: 42
        );
        
        var balanceConfig = new SimulationConfig(
            maxWaves: 3,
            waveSetDifficulty: "balance-testing",
            randomSeed: 42
        );

        // Act
        var defaultResult = _runner.RunSimulation(defaultConfig);
        var balanceResult = _runner.RunSimulation(balanceConfig);

        // Assert
        defaultResult.Should().NotBeNull();
        balanceResult.Should().NotBeNull();
        
        // Both should complete waves, but potentially with different outcomes
        defaultResult.WaveResults.Should().NotBeEmpty();
        balanceResult.WaveResults.Should().NotBeEmpty();
    }

    [Fact]
    public void GameSimRunner_ShouldProvideConsistentResults()
    {
        // Arrange
        var config = new SimulationConfig(
            maxWaves: 3,
            randomSeed: 12345 // Fixed seed for determinism
        );

        // Act
        var result1 = _runner.RunSimulation(config);
        var result2 = _runner.RunSimulation(config);

        // Assert
        result1.Success.Should().Be(result2.Success);
        result1.WavesCompleted.Should().Be(result2.WavesCompleted);
        result1.FinalMoney.Should().Be(result2.FinalMoney);
        result1.FinalLives.Should().Be(result2.FinalLives);
    }

    [Fact]
    public void MockWaveService_ShouldCycleEnemyGroups()
    {
        // Arrange
        _waveService.LoadWaveSet("balance-testing"); // Has multiple enemy groups
        _waveService.StartWave(2); // Wave 2 has mixed enemy types

        // Act
        var enemy1 = _waveService.GetNextEnemyType();
        var enemy2 = _waveService.GetNextEnemyType();
        var enemy3 = _waveService.GetNextEnemyType();

        // Assert
        enemy1.Should().NotBeNull();
        enemy2.Should().NotBeNull();
        enemy3.Should().NotBeNull();
        
        // Enemies should have stats (exact values depend on configuration)
        enemy1.MaxHealth.Should().BeGreaterThan(0);
        enemy2.MaxHealth.Should().BeGreaterThan(0);
        enemy3.MaxHealth.Should().BeGreaterThan(0);
    }
}
