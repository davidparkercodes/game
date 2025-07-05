using Game.Application.Shared.Cqrs;

namespace Game.Application.Queries;

public class GetGameStateQuery : IQuery<GameStateResponse>
{
}

public class GameStateResponse
{
    public int Money { get; }
    public int Lives { get; }
    public int CurrentRound { get; }
    public string CurrentPhase { get; }
    public float PhaseTimeRemaining { get; }
    public bool IsGameOver { get; }
    public bool IsGameWon { get; }
    public bool IsRoundActive { get; }
    public int EnemiesRemaining { get; }

    public GameStateResponse(
        int money,
        int lives,
        int currentRound,
        string currentPhase,
        float phaseTimeRemaining,
        bool isGameOver,
        bool isGameWon,
        bool isRoundActive,
        int enemiesRemaining)
    {
        Money = money;
        Lives = lives;
        CurrentRound = currentRound;
        CurrentPhase = currentPhase;
        PhaseTimeRemaining = phaseTimeRemaining;
        IsGameOver = isGameOver;
        IsGameWon = isGameWon;
        IsRoundActive = isRoundActive;
        EnemiesRemaining = enemiesRemaining;
    }
}
