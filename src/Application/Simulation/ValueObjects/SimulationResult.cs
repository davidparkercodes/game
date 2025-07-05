using System;
using System.Collections.Generic;

namespace Game.Application.Simulation.ValueObjects;

public readonly struct SimulationResult
{
    public bool Success { get; }
    public bool IsVictory { get; }
    public int FinalMoney { get; }
    public int FinalLives { get; }
    public int FinalScore { get; }
    public int WavesCompleted { get; }
    public int TotalEnemiesKilled { get; }
    public int TotalBuildingsPlaced { get; }
    public TimeSpan SimulationDuration { get; }
    public string FailureReason { get; }
    public List<WaveResult> WaveResults { get; }

    public SimulationResult(
        bool success,
        bool isVictory,
        int finalMoney,
        int finalLives,
        int finalScore,
        int wavesCompleted,
        int totalEnemiesKilled,
        int totalBuildingsPlaced,
        TimeSpan simulationDuration,
        string failureReason = null,
        List<WaveResult> waveResults = null)
    {
        Success = success;
        IsVictory = isVictory;
        FinalMoney = finalMoney;
        FinalLives = finalLives;
        FinalScore = finalScore;
        WavesCompleted = wavesCompleted;
        TotalEnemiesKilled = totalEnemiesKilled;
        TotalBuildingsPlaced = totalBuildingsPlaced;
        SimulationDuration = simulationDuration;
        FailureReason = failureReason;
        WaveResults = waveResults ?? new List<WaveResult>();
    }

    public static SimulationResult CreateSuccess(
        int finalMoney,
        int finalLives,
        int finalScore,
        int wavesCompleted,
        int totalEnemiesKilled,
        int totalBuildingsPlaced,
        TimeSpan duration,
        List<WaveResult> waveResults = null)
    {
        return new SimulationResult(
            success: true,
            isVictory: true,
            finalMoney: finalMoney,
            finalLives: finalLives,
            finalScore: finalScore,
            wavesCompleted: wavesCompleted,
            totalEnemiesKilled: totalEnemiesKilled,
            totalBuildingsPlaced: totalBuildingsPlaced,
            simulationDuration: duration,
            waveResults: waveResults
        );
    }

    public static SimulationResult Failure(string reason, GameState finalState, TimeSpan duration, List<WaveResult> waveResults = null)
    {
        return new SimulationResult(
            success: false,
            isVictory: false,
            finalMoney: finalState.Money,
            finalLives: finalState.Lives,
            finalScore: finalState.Score,
            wavesCompleted: finalState.CurrentWave,
            totalEnemiesKilled: 0, // TODO: Track this in GameState
            totalBuildingsPlaced: finalState.Buildings.Count,
            simulationDuration: duration,
            failureReason: reason,
            waveResults: waveResults
        );
    }

    public override string ToString()
    {
        if (Success)
        {
            return $"SUCCESS: Completed {WavesCompleted} waves, {FinalLives} lives remaining, {FinalMoney} money, Score: {FinalScore}";
        }
        else
        {
            return $"FAILURE: {FailureReason} (Wave {WavesCompleted}, {FinalLives} lives, {FinalMoney} money)";
        }
    }
}

public readonly struct WaveResult
{
    public int WaveNumber { get; }
    public bool Completed { get; }
    public int EnemiesKilled { get; }
    public int LivesLost { get; }
    public int MoneyEarned { get; }
    public int ScoreEarned { get; }
    public TimeSpan WaveDuration { get; }

    public WaveResult(
        int waveNumber,
        bool completed,
        int enemiesKilled,
        int livesLost,
        int moneyEarned,
        int scoreEarned,
        TimeSpan waveDuration)
    {
        WaveNumber = waveNumber;
        Completed = completed;
        EnemiesKilled = enemiesKilled;
        LivesLost = livesLost;
        MoneyEarned = moneyEarned;
        ScoreEarned = scoreEarned;
        WaveDuration = waveDuration;
    }
}
