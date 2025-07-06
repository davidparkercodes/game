using Godot;
using Game.Domain.Enemies.Services;
using Game.Infrastructure.Game.Services;
using Game.Infrastructure.Rounds.Services;
using Game.Infrastructure.Audio.Services;
using Game.Presentation.UI;
using Game.Presentation.Enemies;

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
            GD.PrintErr("❌ WaveService returned 0 total waves! Wave loading failed.");
        }
        else
        {
            _isInitialized = true;
            
            // Set the total rounds in RoundService from the wave configuration
            RoundService.Instance?.SetTotalRounds(totalWaves);
            GD.Print($"🌊 WaveManager: Set RoundService.TotalRounds to {totalWaves}");
        }
    }

    public void Initialize(IWaveService waveService)
    {
        // This method can be used to inject the actual wave service from DI container
    }

    public void StartNextWave()
    {
        GD.Print($"🌊 DEBUG: StartNextWave called. Current state - WaveNumber: {_currentWaveNumber}, InProgress: {_isWaveInProgress}");
        
        if (_isWaveInProgress)
        {
            GD.Print("⚠️ Cannot start new wave - wave already in progress");
            return;
        }

        _currentWaveNumber++;
        GD.Print($"🌊 DEBUG: About to start wave {_currentWaveNumber}");

        try
        {
            _waveService.StartWave(_currentWaveNumber);
            _isWaveInProgress = true;
            GD.Print($"🌊 DEBUG: Wave {_currentWaveNumber} started, _isWaveInProgress set to {_isWaveInProgress}");

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
        GD.Print($"🌊 DEBUG: OnWaveCompleted called. Current state - WaveNumber: {_currentWaveNumber}, InProgress: {_isWaveInProgress}");
        
        if (!_isWaveInProgress)
        {
            GD.Print($"⚠️ DEBUG: OnWaveCompleted called but no wave in progress!");
            return;
        }

        _isWaveInProgress = false;
        GD.Print($"🌊 DEBUG: Setting _isWaveInProgress to false for wave {_currentWaveNumber}");

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
                // Special check for wave 5 (boss wave) - ensure boss is actually dead
                if (_currentWaveNumber == 5)
                {
                    var bossesInScene = GetBossEnemiesInScene();
                    if (bossesInScene > 0)
                    {
                        GD.Print($"⚠️ WaveManager: Boss wave not complete - {bossesInScene} boss enemies still alive!");
                        return;
                    }
                    GD.Print($"🎆 WaveManager: Boss wave complete! Boss defeated!");
                }
                
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
        GD.Print($"🏃 WaveManager.OnEnemyReachedEnd() called. Wave in progress: {_isWaveInProgress}");
        
        if (_isWaveInProgress)
        {
            // Check if wave is complete after this enemy leak
            var remainingEnemies = _waveService.GetRemainingEnemies();
            GD.Print($"🔢 WaveManager: Remaining enemies after leak: {remainingEnemies - 1}");
            
            // We need to account for this enemy that just leaked
            if (remainingEnemies - 1 <= 0)
            {
                GD.Print($"🎆 WaveManager: Wave complete after enemy leak! Calling OnWaveCompleted()");
                OnWaveCompleted();
            }
        }
        else
        {
            GD.Print($"⚠️ WaveManager: Enemy reached end but no wave in progress");
        }
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
        
        // Re-sync total rounds with RoundService after reset
        var totalWaves = GetTotalWaves();
        if (totalWaves > 0)
        {
            RoundService.Instance?.SetTotalRounds(totalWaves);
            GD.Print($"🔄 WaveManager: Re-synced RoundService.TotalRounds to {totalWaves} after reset");
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
    
    // DEBUG: Wave jump methods for testing
    public void JumpToWave(int targetWave)
    {
        if (targetWave < 1 || targetWave > GetTotalWaves())
        {
            GD.PrintErr($"🚫 DEBUG: Cannot jump to wave {targetWave}. Valid range: 1-{GetTotalWaves()}");
            return;
        }
        
        if (_isWaveInProgress)
        {
            GD.Print($"⏸️ DEBUG: Stopping current wave {_currentWaveNumber} to jump to wave {targetWave}");
            _waveService.StopCurrentWave();
        }
        
        GD.Print($"🚀 DEBUG: Jumping to wave {targetWave}!");
        
        // Reset current state
        Reset();
        
        // Set wave number to target - 1 (since StartNextWave increments)
        _currentWaveNumber = targetWave - 1;
        
        // Start the target wave
        StartNextWave();
        
        GD.Print($"✅ DEBUG: Successfully jumped to wave {targetWave}");
    }
    
    public void CompleteCurrentWaveInstantly()
    {
        if (!_isWaveInProgress)
        {
            GD.Print($"⚠️ DEBUG: No wave in progress to complete. Current wave: {_currentWaveNumber}");
            return;
        }
        
        GD.Print($"⚡ DEBUG: Instantly completing wave {_currentWaveNumber}!");
        
        // Stop the current wave spawning
        _waveService.StopCurrentWave();
        
        // Force complete the wave
        OnWaveCompleted();
        
        GD.Print($"✅ DEBUG: Wave {_currentWaveNumber} completed instantly!");
    }
    
    public void JumpToNextWave()
    {
        int nextWave = _currentWaveNumber + 1;
        
        if (nextWave > GetTotalWaves())
        {
            GD.Print($"🏁 DEBUG: Already at final wave ({_currentWaveNumber}). Cannot jump to next wave.");
            return;
        }
        
        GD.Print($"⏭️ DEBUG: Jumping to next wave: {nextWave}");
        JumpToWave(nextWave);
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
        
        // Stop boss battle music if it's playing
        StopBossBattleMusic();
        
        // Play victory celebration
        PlayVictoryCelebration();

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
    
    private void StopBossBattleMusic()
    {
        if (SoundManagerService.Instance != null)
        {
            SoundManagerService.Instance.StopMusic();
            GD.Print("🎵 Stopped boss battle music");
        }
        else
        {
            GD.PrintErr("⚠️ SoundManagerService not available to stop boss music");
        }
    }
    
    private void PlayVictoryCelebration()
    {
        // Display victory message
        GD.Print("🏆 VICTORY! All enemies defeated!");
        GD.Print("🎉 The kingdom is safe! Well done, commander!");
        
        // Could add victory sound/music here if we had one
        // For now, just log the celebration
        GD.Print("🎆 Victory celebration complete!");
    }
    
    private int GetBossEnemiesInScene()
    {
        var sceneTree = Godot.Engine.GetMainLoop() as Godot.SceneTree;
        if (sceneTree?.CurrentScene == null)
            return 0;
            
        var bossCount = 0;
        var enemies = sceneTree.GetNodesInGroup("enemies");
        
        foreach (Node node in enemies)
        {
            if (node is Enemy enemy && enemy.IsBossEnemy())
            {
                bossCount++;
            }
        }
        
        GD.Print($"👑 WaveManager: Found {bossCount} boss enemies in scene");
        return bossCount;
    }
}
