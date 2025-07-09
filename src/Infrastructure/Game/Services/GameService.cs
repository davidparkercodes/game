using Godot;
using Game.Presentation.UI;
using Game.Infrastructure.Waves.Services;
using Game.Infrastructure.Economy.Services;

namespace Game.Infrastructure.Game.Services;

public class GameService
{
    public static GameService Instance { get; private set; } = null!;

    public int Money { get; private set; }
    public int Lives { get; private set; }
    public int Score { get; private set; }
    public bool IsGameActive { get; private set; } = false;

    static GameService()
    {
        Instance = new GameService();
        // Initialize economy config service first
        GameEconomyConfigService.Instance.Initialize();
        GD.Print($"💰 GameService: Economy config initialized. Starting money: {GameEconomyConfigService.Instance.GetStartingMoney()}");
    }

    public void StartGame()
    {
        IsGameActive = true;
        
        // Ensure economy config is initialized
        if (GameEconomyConfigService.Instance == null)
        {
            GD.PrintErr("💰 GameService: Economy config service not initialized!");
            Money = 500; // Fallback
            Lives = 20;  // Fallback
            Score = 0;   // Fallback
        }
        else
        {
            Money = GameEconomyConfigService.Instance.GetStartingMoney();
            Lives = GameEconomyConfigService.Instance.GetStartingLives();
            Score = GameEconomyConfigService.Instance.GetStartingScore();
            GD.Print($"💰 GameService: Starting game with Money: {Money}, Lives: {Lives}, Score: {Score}");
        }
        
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
    
    public bool CanAffordUpgrade(int cost)
    {
        return Money >= cost;
    }
    
    public bool SpendMoneyOnUpgrade(int cost, string towerName)
    {
        if (Money >= cost)
        {
            Money -= cost;
            UpdateHudMoney();
            GD.Print($"💰 [UPGRADE] Spent ${cost} to upgrade {towerName}. Remaining: ${Money}");
            return true;
        }
        else
        {
            GD.Print($"💰 [UPGRADE] Cannot afford upgrade for {towerName}. Need ${cost}, have ${Money}");
            return false;
        }
    }
    
    public void ReceiveMoneyFromSale(int amount, string towerName)
    {
        Money += amount;
        UpdateHudMoney();
        GD.Print($"💰 [SELL] Received ${amount} from selling {towerName}. Total: ${Money}");
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
        Score += reward * GameEconomyConfigService.Instance.GetKillScoreMultiplier();
        
        GD.Print($"💰 GameService: Enemy killed! Reward: {reward}, New score: {Score}");
        
        // Notify wave manager
        GD.Print($"📡 GameService: Notifying WaveManager about enemy kill");
        WaveManager.Instance?.OnEnemyKilled();
    }

    public void Reset()
    {
        // Ensure economy config is initialized
        if (GameEconomyConfigService.Instance == null)
        {
            GD.PrintErr("💰 GameService: Economy config service not initialized during reset!");
            Money = 500; // Fallback
            Lives = 20;  // Fallback
            Score = 0;   // Fallback
        }
        else
        {
            Money = GameEconomyConfigService.Instance.GetStartingMoney();
            Lives = GameEconomyConfigService.Instance.GetStartingLives();
            Score = GameEconomyConfigService.Instance.GetStartingScore();
            GD.Print($"💰 GameService: Reset game with Money: {Money}, Lives: {Lives}, Score: {Score}");
        }
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
