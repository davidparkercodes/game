using Godot;
using Game.Presentation.UI;
using Game.Infrastructure.Waves.Services;

namespace Game.Infrastructure.Game.Services;

public class GameService
{
    public static GameService Instance { get; private set; } = null!;

    public int Money { get; private set; } = 500;
    public int Lives { get; private set; } = 20;
    public int Score { get; private set; } = 0;
    public bool IsGameActive { get; private set; } = false;

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
        
        // Initialize wave system
        WaveManager.Instance?.Reset();
        
        // Update HUD with initial values
        UpdateHudMoney();
        UpdateHudLives();
        
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
            UpdateHudMoney();
            GD.Print($"Spent {amount} money. Remaining: {Money}");
            return true;
        }
        return false;
    }

    public void AddMoney(int amount)
    {
        Money += amount;
        UpdateHudMoney();
        GD.Print($"Added {amount} money. Total: {Money}");
    }

    public void OnEnemyReachedEnd()
    {
        Lives--;
        UpdateHudLives();
        
        // Notify wave manager
        WaveManager.Instance?.OnEnemyReachedEnd();
        
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
        
        GD.Print($"💰 GameService: Enemy killed! Reward: {reward}, New score: {Score}");
        
        // Notify wave manager
        GD.Print($"📡 GameService: Notifying WaveManager about enemy kill");
        WaveManager.Instance?.OnEnemyKilled();
    }

    public void Reset()
    {
        Money = 500;
        Lives = 20;
        Score = 0;
        IsGameActive = false;
        
        // Reset wave manager
        WaveManager.Instance?.Reset();
        
        // Update HUD with reset values
        UpdateHudMoney();
        UpdateHudLives();
    }

    public bool IsGameOver()
    {
        return Lives <= 0;
    }

    public bool IsGameWon()
    {
        return false;
    }
    
    // HUD Update Helper Methods
    private void UpdateHudMoney()
    {
        if (HudManager.Instance != null && HudManager.Instance.IsInitialized())
        {
            HudManager.Instance.UpdateMoney(Money);
        }
    }
    
    private void UpdateHudLives()
    {
        if (HudManager.Instance != null && HudManager.Instance.IsInitialized())
        {
            HudManager.Instance.UpdateLives(Lives);
        }
    }
    
    public void UpdateHudWave(int wave)
    {
        if (HudManager.Instance != null && HudManager.Instance.IsInitialized())
        {
            HudManager.Instance.UpdateWave(wave);
        }
    }
}
