using System;
using System.Linq;
using Xunit;
using FluentAssertions;
using Game.Application.Simulation;
using Game.Application.Simulation.ValueObjects;

namespace Game.Tests.Application.Simulation;

public class GameSimRunnerTests
{
    private readonly GameSimRunner _runner;

    public GameSimRunnerTests()
    {
        _runner = new GameSimRunner();
    }

    [Fact]
    public void RunSimulation_WithDefaultConfig_ShouldNotCrash()
    {
        // Arrange
        var config = SimulationConfig.Default();

        // Act
        var result = _runner.RunSimulation(config);

        // Assert
        result.Should().NotBeNull();
        result.SimulationDuration.Should().BeGreaterThan(TimeSpan.Zero);
    }

    [Fact]
    public void RunSimulation_WithSameSeed_ShouldProduceDeterministicResults()
    {
        // Arrange
        var config1 = new SimulationConfig(randomSeed: 12345);
        var config2 = new SimulationConfig(randomSeed: 12345);

        // Act
        var result1 = _runner.RunSimulation(config1);
        var result2 = _runner.RunSimulation(config2);

        // Assert
        result1.Success.Should().Be(result2.Success);
        result1.FinalMoney.Should().Be(result2.FinalMoney);
        result1.FinalLives.Should().Be(result2.FinalLives);
        result1.WavesCompleted.Should().Be(result2.WavesCompleted);
    }

    [Fact]
    public void RunSimulation_WithBalanceTestingConfig_ShouldAttemptAllWaves()
    {
        // Arrange
        var config = SimulationConfig.ForBalanceTesting();

        // Act
        var result = _runner.RunSimulation(config);

        // Assert
        result.WavesCompleted.Should().BeGreaterThan(0);
        result.WaveResults.Should().NotBeEmpty();
        result.WaveResults.Count.Should().BeLessOrEqualTo(config.MaxWaves);
    }

    [Fact]
    public void RunSimulation_WithImpossibleDifficulty_ShouldFailEarly()
    {
        // Arrange - Make enemies super strong
        var config = new SimulationConfig(
            startingMoney: 50, // Very little money
            enemyHealthMultiplier: 10.0f, // Very strong enemies
            maxWaves: 10
        );

        // Act
        var result = _runner.RunSimulation(config);

        // Assert
        result.Success.Should().BeFalse();
        result.FailureReason.Should().NotBeNullOrEmpty();
        result.WavesCompleted.Should().BeLessThan(config.MaxWaves);
    }

    [Fact]
    public void RunSimulation_WithEasyDifficulty_ShouldCompleteMoreWaves()
    {
        // Arrange - Make it easier
        var config = new SimulationConfig(
            startingMoney: 1000, // Lots of money
            enemyHealthMultiplier: 0.5f, // Weak enemies
            buildingCostMultiplier: 0.5f, // Cheap buildings
            maxWaves: 5 // Fewer waves for faster test
        );

        // Act
        var result = _runner.RunSimulation(config);

        // Assert
        result.Success.Should().BeTrue();
        result.IsVictory.Should().BeTrue();
        result.WavesCompleted.Should().Be(config.MaxWaves);
        result.FinalLives.Should().BeGreaterThan(0);
    }

    [Fact]
    public void RunMultipleScenarios_ShouldReturnResultsForAllScenarios()
    {
        // Arrange
        var scenarios = new[]
        {
            SimulationConfig.Default(),
            SimulationConfig.ForBalanceTesting(),
            SimulationConfig.WithDifficultyModifier(1.2f)
        };

        // Act
        var results = _runner.RunMultipleScenarios(scenarios.ToList());

        // Assert
        results.Should().HaveCount(3);
        results.Should().AllSatisfy(r => r.SimulationDuration.Should().BeGreaterThan(TimeSpan.Zero));
    }

    [Fact]
    public void SimulationConfig_Default_ShouldHaveReasonableValues()
    {
        // Act
        var config = SimulationConfig.Default();

        // Assert
        config.StartingMoney.Should().BeGreaterThan(0);
        config.StartingLives.Should().BeGreaterThan(0);
        config.MaxWaves.Should().BeGreaterThan(0);
        config.EnemyHealthMultiplier.Should().Be(1.0f);
        config.EnemySpeedMultiplier.Should().Be(1.0f);
        config.BuildingCostMultiplier.Should().Be(1.0f);
        config.FastMode.Should().BeTrue();
    }

    [Fact]
    public void SimulationResult_ToString_ShouldProvideUsefulInformation()
    {
        // Arrange
        var config = SimulationConfig.ForBalanceTesting();

        // Act
        var result = _runner.RunSimulation(config);
        var resultString = result.ToString();

        // Assert
        resultString.Should().NotBeNullOrEmpty();
        resultString.Should().Contain(result.Success ? "SUCCESS" : "FAILURE");
        if (result.Success)
        {
            resultString.Should().Contain(result.WavesCompleted.ToString());
            resultString.Should().Contain(result.FinalLives.ToString());
        }
        else
        {
            resultString.Should().Contain(result.FailureReason);
        }
    }

    [Fact]
    public async void RunSimulationAsync_ShouldProduceSameResultAsSync()
    {
        // Arrange
        var config = new SimulationConfig(randomSeed: 42, maxWaves: 3);

        // Act
        var syncResult = _runner.RunSimulation(config);
        var asyncResult = await _runner.RunSimulationAsync(config);

        // Assert
        syncResult.Success.Should().Be(asyncResult.Success);
        syncResult.WavesCompleted.Should().Be(asyncResult.WavesCompleted);
        // Note: Timing may be slightly different, so we don't compare duration
    }
}
