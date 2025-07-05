using Godot;
using Game.Domain.Enemies.Services;
using Game.Infrastructure.Game.Services;
using Game.Infrastructure.Rounds.Services;
using Game.Presentation.UI;

namespace Game.Infrastructure.Waves.Services;

public class WaveManager
{
    public static WaveManager Instance { get; private set; } = null!;
    
    private readonly IWaveService _waveService;
    private int _currentWaveNumber = 0;
    private bool _isWaveInProgress = false;
    
    static WaveManager()
    {
        Instance = new WaveManager();
    }
    
    private WaveManager()
    {
        // For now, we'll use a placeholder. In a real scenario, this would be injected via DI
        _waveService = new Application.Simulation.Services.MockWaveService();
        
        // Ensure the wave service is properly initialized
        _waveService.Initialize();
        
        GD.Print($"🌊 WaveManager initialized with {_waveService.GetTotalWaves()} total waves");
    }
    
    public void Initialize(IWaveService waveService)
    {
        // This method can be used to inject the actual wave service from DI container
    }
    
    public void StartNextWave()
    {
        if (_isWaveInProgress)
        {
            GD.Print("⚠️ Cannot start new wave - wave already in progress");
            return;
        }
        
        _currentWaveNumber++;
        
        try
        {
            _waveService.StartWave(_currentWaveNumber);
            _isWaveInProgress = true;
            
            // Update round service
            RoundService.Instance?.StartRound(_currentWaveNumber);
            
            // Update game service HUD
            GameService.Instance?.UpdateHudWave(_currentWaveNumber);
            
            // Update HUD button state
            UpdateWaveButtonState();
            
            // Update wave display
            UpdateWaveDisplay();
            
            GD.Print($"🌊 Started wave {_currentWaveNumber}");
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"❌ Failed to start wave {_currentWaveNumber}: {ex.Message}");
            _currentWaveNumber--; // Revert on failure
        }
    }
    
    public void OnWaveCompleted()
    {
        if (!_isWaveInProgress)
        {
            return;
        }
        
        _isWaveInProgress = false;
        
        // Complete the wave in the wave service
        _waveService.StopCurrentWave();
        
        // Complete the round
        RoundService.Instance?.CompleteRound();
        
        // Update HUD button state
        UpdateWaveButtonState();
        
        // Update wave display
        UpdateWaveDisplay();
        
        GD.Print($"✅ Wave {_currentWaveNumber} completed");
        
        // Check if this was the last wave
        if (_currentWaveNumber >= GetTotalWaves())
        {
            OnAllWavesCompleted();
        }
    }
    
    public void OnEnemyKilled()
    {
        if (_isWaveInProgress)
        {
            // Update round service
            RoundService.Instance?.OnEnemyDefeated();
            
            // Check if wave is complete
            if (_waveService.GetRemainingEnemies() <= 0)
            {
                OnWaveCompleted();
            }
        }
    }
    
    public void OnEnemyReachedEnd()
    {
        // This is handled by GameService already, but we could add wave-specific logic here
    }
    
    public void Reset()
    {
        _currentWaveNumber = 0;
        _isWaveInProgress = false;
        _waveService.Reset();
        RoundService.Instance?.Reset();
        
        // Update HUD
        GameService.Instance?.UpdateHudWave(0);
        
        // Ensure wave service is initialized before updating button state
        if (GetTotalWaves() <= 0)
        {
            // Re-initialize the wave service if needed
            _waveService.Initialize();
        }
        
        UpdateWaveButtonState();
        
        // Update wave display
        UpdateWaveDisplay();
        
        GD.Print("🔄 Wave manager reset");
    }
    
    public int GetCurrentWaveNumber()
    {
        return _currentWaveNumber;
    }
    
    public int GetTotalWaves()
    {
        return _waveService.GetTotalWaves();
    }
    
    public bool IsWaveInProgress()
    {
        return _isWaveInProgress;
    }
    
    public int GetRemainingEnemies()
    {
        return _waveService.GetRemainingEnemies();
    }
    
    public float GetWaveProgress()
    {
        return _waveService.GetWaveProgress();
    }
    
    private void UpdateWaveButtonState()
    {
        if (HudManager.Instance != null && HudManager.Instance.IsInitialized())
        {
            var totalWaves = GetTotalWaves();
            
            // Debug logging
            GD.Print($"🌊 UpdateWaveButtonState: currentWave={_currentWaveNumber}, totalWaves={totalWaves}, inProgress={_isWaveInProgress}");
            
            if (_isWaveInProgress)
            {
                HudManager.Instance.SetWaveButtonState("Wave in Progress", false);
            }
            else if (totalWaves <= 0)
            {
                // Wave service not ready yet, show loading state
                HudManager.Instance.SetWaveButtonState("Loading Waves...", false);
            }
            else if (_currentWaveNumber >= totalWaves)
            {
                HudManager.Instance.SetWaveButtonState("All Waves Complete", false);
            }
            else
            {
                HudManager.Instance.SetWaveButtonState($"Start Wave {_currentWaveNumber + 1}", true);
            }
        }
    }
    
    private void UpdateWaveDisplay()
    {
        if (HudManager.Instance != null && HudManager.Instance.IsInitialized())
        {
            var displayWave = _currentWaveNumber == 0 ? 1 : _currentWaveNumber;
            HudManager.Instance.UpdateWave(displayWave);
        }
    }
    
    private void OnAllWavesCompleted()
    {
        GD.Print("🎉 All waves completed! Player wins!");
        
        // Mark game as won in GameService
        // GameService would need a method for this
        
        UpdateWaveButtonState();
    }
}
