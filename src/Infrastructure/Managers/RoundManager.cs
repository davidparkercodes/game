using Godot;

namespace Game.Infrastructure.Managers;

public enum RoundPhase
{
    Preparation,
    Build,
    Active,
    Complete,
    Paused
}

public class RoundManager
{
    public static RoundManager Instance { get; private set; }

    public int CurrentRound { get; private set; } = 1;
    public RoundPhase CurrentPhase { get; private set; } = RoundPhase.Preparation;
    public int EnemiesRemaining { get; private set; } = 0;
    public int TotalEnemies { get; private set; } = 0;
    public float TimeRemaining { get; private set; } = 0f;

    static RoundManager()
    {
        Instance = new RoundManager();
    }

    public void StartRound(int roundNumber)
    {
        CurrentRound = roundNumber;
        CurrentPhase = RoundPhase.Active;
        TotalEnemies = CalculateEnemiesForRound(roundNumber);
        EnemiesRemaining = TotalEnemies;
        TimeRemaining = CalculateTimeForRound(roundNumber);
        
        GD.Print($"Starting round {CurrentRound} with {TotalEnemies} enemies");
    }

    public void CompleteRound()
    {
        CurrentPhase = RoundPhase.Complete;
        GD.Print($"Round {CurrentRound} completed!");
        
        // Award completion bonus
        int bonus = CurrentRound * 25;
        GameManager.Instance?.AddMoney(bonus);
    }

    public void PauseRound()
    {
        CurrentPhase = RoundPhase.Paused;
        GD.Print($"Round {CurrentRound} paused");
    }

    public void ResumeRound()
    {
        CurrentPhase = RoundPhase.Active;
        GD.Print($"Round {CurrentRound} resumed");
    }

    public void OnEnemySpawned()
    {
        // Enemy spawned, no change to remaining count yet
        GD.Print($"Enemy spawned. {EnemiesRemaining} remaining in round {CurrentRound}");
    }

    public void OnEnemyDefeated()
    {
        EnemiesRemaining--;
        GD.Print($"Enemy defeated. {EnemiesRemaining} remaining in round {CurrentRound}");
        
        if (EnemiesRemaining <= 0)
        {
            CompleteRound();
        }
    }

    public void Update(float deltaTime)
    {
        if (CurrentPhase == RoundPhase.Active && TimeRemaining > 0)
        {
            TimeRemaining -= deltaTime;
            if (TimeRemaining <= 0)
            {
                TimeRemaining = 0;
                // Could trigger time-based events here
            }
        }
    }

    public void Reset()
    {
        CurrentRound = 1;
        CurrentPhase = RoundPhase.Preparation;
        EnemiesRemaining = 0;
        TotalEnemies = 0;
        TimeRemaining = 0f;
    }

    private int CalculateEnemiesForRound(int roundNumber)
    {
        return 5 + (roundNumber * 3); // Base 5 enemies + 3 per round
    }

    private float CalculateTimeForRound(int roundNumber)
    {
        return 60f + (roundNumber * 10f); // Base 60 seconds + 10 per round
    }

    public bool IsRoundActive()
    {
        return CurrentPhase == RoundPhase.Active;
    }

    public bool IsRoundComplete()
    {
        return CurrentPhase == RoundPhase.Complete;
    }

    public float GetRoundProgress()
    {
        if (TotalEnemies == 0) return 0f;
        return (float)(TotalEnemies - EnemiesRemaining) / TotalEnemies;
    }

    public int TotalRounds { get; set; } = 10;

    public void ForceStartDefendPhase()
    {
        if (CurrentPhase == RoundPhase.Preparation)
        {
            StartRound(CurrentRound);
        }
    }

    public float PhaseTimeRemaining { get; private set; } = 0f;

    public int CurrentWaveIndex { get; private set; } = 0;
}
