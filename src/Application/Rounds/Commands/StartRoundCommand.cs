using Game.Application.Shared.Cqrs;

namespace Game.Application.Rounds.Commands;

public class StartRoundCommand : ICommand<StartRoundResult>
{
    public int RoundNumber { get; }
    public bool ForceStart { get; }

    public StartRoundCommand(int roundNumber = 0, bool forceStart = false)
    {
        RoundNumber = roundNumber;
        ForceStart = forceStart;
    }
}

public class StartRoundResult
{
    public bool Success { get; }
    public string? ErrorMessage { get; }
    public int RoundNumber { get; }
    public string? Phase { get; }

    public StartRoundResult(bool success, int roundNumber = 0, string? phase = null, string? errorMessage = null)
    {
        Success = success;
        RoundNumber = roundNumber;
        Phase = phase;
        ErrorMessage = errorMessage;
    }

    public static StartRoundResult Successful(int roundNumber, string phase)
    {
        return new StartRoundResult(true, roundNumber, phase);
    }

    public static StartRoundResult Failed(string errorMessage)
    {
        return new StartRoundResult(false, errorMessage: errorMessage);
    }
}
