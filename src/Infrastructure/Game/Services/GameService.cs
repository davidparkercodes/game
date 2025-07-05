using Godot;

namespace Game.Infrastructure.Game.Services;

public class GameService
{
    public static GameService Instance { get; private set; }

    public int Money { get; private set; } = 500;
    public int Lives { get; private set; } = 20;
    public int Score { get; private set; } = 0;
    public bool IsGameActive { get; private set; } = false;
    public object Hud { get; set; }

    static GameService()
    {
        Instance = new GameService();
    }

    public void StartGame()
    {
        IsGameActive = true;
        Money = 500;
        Lives = 20;
        Score = 0;
        GD.Print("Game started!");
    }

    public void EndGame()
    {
        IsGameActive = false;
        GD.Print("Game ended!");
    }

    public bool SpendMoney(int amount)
    {
        if (Money >= amount)
        {
            Money -= amount;
            GD.Print($"Spent {amount} money. Remaining: {Money}");
            return true;
        }
        return false;
    }

    public void AddMoney(int amount)
    {
        Money += amount;
        GD.Print($"Added {amount} money. Total: {Money}");
    }

    public void OnEnemyReachedEnd()
    {
        Lives--;
        GD.Print($"Enemy reached end! Lives remaining: {Lives}");
        
        if (Lives <= 0)
        {
            EndGame();
        }
    }

    public void OnEnemyKilled(int reward)
    {
        AddMoney(reward);
        Score += reward * 10;
        GD.Print($"Enemy killed! Score: {Score}");
    }

    public void Reset()
    {
        Money = 500;
        Lives = 20;
        Score = 0;
        IsGameActive = false;
    }

    public bool IsGameOver()
    {
        return Lives <= 0;
    }

    public bool IsGameWon()
    {
        return false;
    }
}
