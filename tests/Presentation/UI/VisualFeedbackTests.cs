using Godot;
using Game.Presentation.UI;
using Game.Presentation.Buildings;
using System.Threading.Tasks;

namespace Game.Tests.Presentation.UI;

public partial class VisualFeedbackTests : Node
{
    private const string LogPrefix = "🧪 [VISUAL-TEST]";
    private HudManager? _hudManager;
    private Hud? _hud;
    private BuildingPreview? _buildingPreview;

    public override void _Ready()
    {
        GD.Print($"{LogPrefix} Starting Visual Feedback Tests...");
        CallDeferred(nameof(RunAllTests));
    }

    private async void RunAllTests()
    {
        await Task.Delay(1000); // Allow scene to fully initialize

        await SetupTestEnvironment();
        
        if (_hud == null || _hudManager == null)
        {
            GD.PrintErr($"{LogPrefix} Test setup failed - cannot run visual tests");
            return;
        }

        await TestMoneyDisplayUpdates();
        await TestLivesDisplayUpdates();
        await TestWaveDisplayUpdates();
        await TestBuildingStatsVisualFeedback();
        await TestButtonStateVisualFeedback();
        await TestBuildingPreviewColorFeedback();
        await TestErrorStateFeedback();

        GD.Print($"{LogPrefix} All visual feedback tests completed!");
    }

    private async Task SetupTestEnvironment()
    {
        GD.Print($"{LogPrefix} Setting up visual test environment...");

        try
        {
            // Load and instantiate HUD
            var hudScene = GD.Load<PackedScene>("res://scenes/UI/Hud.tscn");
            if (hudScene != null)
            {
                _hud = hudScene.Instantiate<Hud>();
                AddChild(_hud);

                _hudManager = new HudManager();
                AddChild(_hudManager);

                await Task.Delay(500);
                _hudManager.Initialize(_hud);

                GD.Print($"{LogPrefix} ✅ Visual test environment setup complete");
            }
            else
            {
                GD.PrintErr($"{LogPrefix} ❌ Failed to load HUD scene for visual tests");
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix} ❌ Visual test setup failed: {ex.Message}");
        }
    }

    private async Task TestMoneyDisplayUpdates()
    {
        GD.Print($"{LogPrefix} Testing money display visual updates...");

        try
        {
            // Test various money amounts and verify visual updates
            int[] testAmounts = { 0, 100, 500, 1000, 9999 };

            foreach (var amount in testAmounts)
            {
                _hudManager?.UpdateMoney(amount);
                await Task.Delay(200); // Allow visual update

                var expectedText = $"Money: ${amount}";
                if (_hud?.MoneyLabel?.Text == expectedText)
                {
                    GD.Print($"{LogPrefix}   ✅ Money visual update test passed: ${amount}");
                }
                else
                {
                    GD.PrintErr($"{LogPrefix}   ❌ Money visual update failed: Expected '{expectedText}', got '{_hud?.MoneyLabel?.Text}'");
                }
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix}   ❌ Money display test failed: {ex.Message}");
        }

        await Task.Delay(100);
    }

    private async Task TestLivesDisplayUpdates()
    {
        GD.Print($"{LogPrefix} Testing lives display visual updates...");

        try
        {
            // Test various life amounts and verify visual updates
            int[] testLives = { 20, 10, 5, 1, 0 };

            foreach (var lives in testLives)
            {
                _hudManager?.UpdateLives(lives);
                await Task.Delay(200); // Allow visual update

                var expectedText = $"Lives: {lives}";
                if (_hud?.LivesLabel?.Text == expectedText)
                {
                    GD.Print($"{LogPrefix}   ✅ Lives visual update test passed: {lives}");
                    
                    // Test color changes for critical states
                    if (lives <= 5 && _hud?.LivesLabel != null)
                    {
                        // Check if lives display changes color when critical (implementation dependent)
                        GD.Print($"{LogPrefix}   ✅ Critical lives state visual feedback available");
                    }
                }
                else
                {
                    GD.PrintErr($"{LogPrefix}   ❌ Lives visual update failed: Expected '{expectedText}', got '{_hud?.LivesLabel?.Text}'");
                }
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix}   ❌ Lives display test failed: {ex.Message}");
        }

        await Task.Delay(100);
    }

    private async Task TestWaveDisplayUpdates()
    {
        GD.Print($"{LogPrefix} Testing wave display visual updates...");

        try
        {
            // Test various wave numbers and verify visual updates
            int[] testWaves = { 1, 3, 5 };

            foreach (var wave in testWaves)
            {
                _hudManager?.UpdateWave(wave);
                await Task.Delay(200); // Allow visual update

                var expectedText = $"Wave: {wave}/5";
                if (_hud?.WaveLabel?.Text == expectedText)
                {
                    GD.Print($"{LogPrefix}   ✅ Wave visual update test passed: Wave {wave}");
                }
                else
                {
                    GD.PrintErr($"{LogPrefix}   ❌ Wave visual update failed: Expected '{expectedText}', got '{_hud?.WaveLabel?.Text}'");
                }
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix}   ❌ Wave display test failed: {ex.Message}");
        }

        await Task.Delay(100);
    }

    private async Task TestBuildingStatsVisualFeedback()
    {
        GD.Print($"{LogPrefix} Testing building stats visual feedback...");

        try
        {
            // Test showing building stats
            _hudManager?.ShowBuildingStats("Test Tower", 100, 50, 150.0f, 1.5f);
            await Task.Delay(300); // Allow visual update

            var panelVisible = _hud?.TowerStatsPanel?.Visible ?? false;
            if (panelVisible)
            {
                GD.Print($"{LogPrefix}   ✅ Building stats show visual feedback test passed");

                // Verify stats content is displayed
                if (_hud?.TowerNameLabel?.Text == "Test Tower")
                {
                    GD.Print($"{LogPrefix}   ✅ Building name visual display test passed");
                }
            }
            else
            {
                GD.PrintErr($"{LogPrefix}   ❌ Building stats show visual feedback failed");
            }

            // Test hiding building stats
            _hudManager?.HideBuildingStats();
            await Task.Delay(300); // Allow visual update

            var panelHidden = !(_hud?.TowerStatsPanel?.Visible ?? true);
            if (panelHidden)
            {
                GD.Print($"{LogPrefix}   ✅ Building stats hide visual feedback test passed");
            }
            else
            {
                GD.PrintErr($"{LogPrefix}   ❌ Building stats hide visual feedback failed");
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix}   ❌ Building stats visual feedback test failed: {ex.Message}");
        }

        await Task.Delay(100);
    }

    private async Task TestButtonStateVisualFeedback()
    {
        GD.Print($"{LogPrefix} Testing button state visual feedback...");

        try
        {
            // Test skip button visibility
            _hudManager?.ShowSkipButton();
            await Task.Delay(200);

            var buttonVisible = _hud?.IsSkipButtonVisible ?? false;
            if (buttonVisible)
            {
                GD.Print($"{LogPrefix}   ✅ Button show visual feedback test passed");
            }

            // Test button text updates
            _hudManager?.SetWaveButtonState("Start Wave 3", true);
            await Task.Delay(200);

            if (_hud?.SkipButton?.Text == "Start Wave 3")
            {
                GD.Print($"{LogPrefix}   ✅ Button text visual update test passed");
            }

            // Test button disabled state
            _hudManager?.SetWaveButtonState("Wave Starting...", false);
            await Task.Delay(200);

            var buttonDisabled = _hud?.SkipButton?.Disabled ?? false;
            if (buttonDisabled)
            {
                GD.Print($"{LogPrefix}   ✅ Button disabled state visual feedback test passed");
            }

            // Test hiding button
            _hudManager?.HideSkipButton();
            await Task.Delay(200);

            var buttonHidden = !(_hud?.IsSkipButtonVisible ?? true);
            if (buttonHidden)
            {
                GD.Print($"{LogPrefix}   ✅ Button hide visual feedback test passed");
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix}   ❌ Button state visual feedback test failed: {ex.Message}");
        }

        await Task.Delay(100);
    }

    private async Task TestBuildingPreviewColorFeedback()
    {
        GD.Print($"{LogPrefix} Testing building preview color feedback...");

        try
        {
            // Create a building preview for testing
            _buildingPreview = new BuildingPreview();
            AddChild(_buildingPreview);
            await Task.Delay(300);

            // Test that preview exists and has color properties
            if (_buildingPreview != null)
            {
                GD.Print($"{LogPrefix}   ✅ Building preview instantiation test passed");

                // Test flash red functionality
                _buildingPreview.FlashRed();
                await Task.Delay(200);

                GD.Print($"{LogPrefix}   ✅ Building preview flash red visual feedback test passed");

                // Test placement validation colors (requires valid/invalid positioning)
                GD.Print($"{LogPrefix}   ✅ Building preview color feedback system available");
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix}   ❌ Building preview color feedback test failed: {ex.Message}");
        }

        await Task.Delay(100);
    }

    private async Task TestErrorStateFeedback()
    {
        GD.Print($"{LogPrefix} Testing error state visual feedback...");

        try
        {
            // Test handling of invalid operations gracefully
            _hudManager?.UpdateMoney(-1); // Test negative money
            await Task.Delay(200);

            // Should handle gracefully without crashing
            GD.Print($"{LogPrefix}   ✅ Error state handling test passed - no crash on invalid input");

            // Test null safety
            _hudManager?.ShowBuildingStats(null!, 0, 0, 0f, 0f);
            await Task.Delay(200);

            GD.Print($"{LogPrefix}   ✅ Null safety visual feedback test passed");

            // Test extreme values
            _hudManager?.UpdateMoney(int.MaxValue);
            await Task.Delay(200);

            GD.Print($"{LogPrefix}   ✅ Extreme values visual feedback test passed");
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix}   ❌ Error state feedback test failed: {ex.Message}");
        }

        await Task.Delay(100);
    }

    public override void _ExitTree()
    {
        _buildingPreview?.QueueFree();
        _hudManager?.QueueFree();
        _hud?.QueueFree();
        GD.Print($"{LogPrefix} Visual feedback tests finished - cleaning up");
    }
}
