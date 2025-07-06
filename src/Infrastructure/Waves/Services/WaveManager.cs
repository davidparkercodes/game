using Godot;
using Game.Domain.Enemies.Services;
using Game.Infrastructure.Game.Services;
using Game.Infrastructure.Rounds.Services;
using Game.Infrastructure.Audio.Services;
using Game.Presentation.UI;

namespace Game.Infrastructure.Waves.Services;

public class WaveManager
{
    private static WaveManager? _instance = null;
    public static WaveManager? Instance => _instance;

    private readonly IWaveService _waveService;
    private int _currentWaveNumber = 0;
    private bool _isWaveInProgress = false;
    private bool _isInitialized = false;

    public static void Initialize()
    {
        if (_instance == null)
        {
            _instance = new WaveManager();
        }
    }

    private WaveManager()
    {
        // For now, we'll use a placeholder. In a real scenario, this would be injected via DI
        GD.Print("🌊 Creating WaveSpawnerService...");
        // We need to get the wave configuration service from DI, but for now create a simple one
        var waveConfigService = new Infrastructure.Waves.Services.WaveConfigurationService();
        _waveService = new Infrastructure.Enemies.Services.WaveSpawnerService(waveConfigService);

        // Ensure the wave service is properly initialized
        GD.Print("🌊 Initializing wave service...");
        _waveService.Initialize();

        var totalWaves = _waveService.GetTotalWaves();
        GD.Print($"🌊 WaveManager initialized with {totalWaves} total waves");

        if (totalWaves <= 0)
        {
            GD.PrintErr("❌ MockWaveService returned 0 total waves! Wave loading failed.");
        }
        else
        {
            _isInitialized = true;
        }
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

            // Play round start sound
            PlayRoundStartSound();

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
        GD.Print($"🎯 WaveManager.OnEnemyKilled() called. Wave in progress: {_isWaveInProgress}");
        
        if (_isWaveInProgress)
        {
            // Update round service
            RoundService.Instance?.OnEnemyDefeated();

            // Check if wave is complete
            var remainingEnemies = _waveService.GetRemainingEnemies();
            GD.Print($"🔢 WaveManager: Remaining enemies: {remainingEnemies}");
            
            if (remainingEnemies <= 0)
            {
                GD.Print($"🎆 WaveManager: Wave complete! Calling OnWaveCompleted()");
                OnWaveCompleted();
            }
        }
        else
        {
            GD.Print($"⚠️ WaveManager: Enemy killed but no wave in progress");
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
        if (!_isInitialized)
        {
            GD.Print("⚠️ WaveManager not initialized yet, skipping button state update");
            return;
        }

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
        else
        {
            GD.Print("⚠️ HudManager not available, skipping button state update");
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
    
    private void PlayRoundStartSound()
    {
        if (SoundManagerService.Instance != null)
        {
            SoundManagerService.Instance.PlaySound("round_start");
            GD.Print("🔊 Playing round start sound");
        }
        else
        {
            GD.PrintErr("⚠️ SoundManagerService not available for round start sound");
        }
    }
}
