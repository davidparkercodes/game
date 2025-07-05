namespace GameSimRunner.Standalone.ValueObjects;

public readonly struct SimulationResult
{
    public bool Success { get; }
    public bool IsVictory { get; }
    public int FinalMoney { get; }
    public int FinalLives { get; }
    public TimeSpan SimulationDuration { get; }

    public static SimulationResult CreateSuccess(int finalMoney, int finalLives, TimeSpan duration)
    {
        return new SimulationResult(true, true, finalMoney, finalLives, duration);
    }

    public static SimulationResult Failure()
    {
        return new SimulationResult(false, false, 0, 0, TimeSpan.Zero);
    }

    private SimulationResult(bool success, bool isVictory, int finalMoney, int finalLives, TimeSpan duration)
    {
        Success = success;
        IsVictory = isVictory;
        FinalMoney = finalMoney;
        FinalLives = finalLives;
        SimulationDuration = duration;
    }
}
