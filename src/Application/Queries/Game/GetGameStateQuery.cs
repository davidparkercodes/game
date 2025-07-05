using Game.Application.Shared.Cqrs;

namespace Game.Application.Queries.Game;

public class GetGameStateQuery : IQuery<GameStateResponse>
{
    public GetGameStateQuery()
    {
    }
}

public class GameStateResponse
{
    public int Money { get; }
    public int Lives { get; }
    public int Score { get; }
    public bool IsGameActive { get; }
    public int CurrentRound { get; }
    public string CurrentPhase { get; }
    public int EnemiesRemaining { get; }

    public GameStateResponse(
        int money,
        int lives,
        int score,
        bool isGameActive,
        int currentRound,
        string currentPhase,
        int enemiesRemaining)
    {
        Money = money;
        Lives = lives;
        Score = score;
        IsGameActive = isGameActive;
        CurrentRound = currentRound;
        CurrentPhase = currentPhase;
        EnemiesRemaining = enemiesRemaining;
    }

    public static GameStateResponse CreateDefault()
    {
        return new GameStateResponse(
            money: 500,
            lives: 20,
            score: 0,
            isGameActive: false,
            currentRound: 1,
            currentPhase: "Preparation",
            enemiesRemaining: 0
        );
    }
}
