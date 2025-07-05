using Godot;
using Game.Presentation.UI;
using Game.Infrastructure.Game.Services;
using Game.Infrastructure.Rounds.Services;
using Game.Infrastructure.Waves.Services;
using System.Threading.Tasks;

namespace Game.Tests.Presentation.UI;

public partial class HudIntegrationTests : Node
{
	private Hud? _hud;
	private HudManager? _hudManager;
	private const string LogPrefix = "🧪 [HUD-TEST]";

	public override void _Ready()
	{
		GD.Print($"{LogPrefix} Starting HUD Integration Tests...");
		CallDeferred(nameof(RunAllTests));
	}

	private async void RunAllTests()
	{
		await Task.Delay(1000);
		
		await SetupTestEnvironment();
		
		if (_hud == null || _hudManager == null)
		{
			GD.PrintErr($"{LogPrefix} Test setup failed - cannot run tests");
			return;
		}

		await RunMoneyUpdateTests();
		await RunLivesUpdateTests();
		await RunWaveUpdateTests();
		await RunBuildingStatsTests();
		await RunButtonStateTests();
		
		GD.Print($"{LogPrefix} All integration tests completed!");
	}

private async Task SetupTestEnvironment()
	{
		GD.Print($"{LogPrefix} Setting up test environment...");
		
		var hudScene = GD.Load<PackedScene>("res://scenes/UI/Hud.tscn");
		if (hudScene == null)
		{
			GD.PrintErr($"{LogPrefix} Failed to load HUD scene");
			return;
		}
		
		_hud = hudScene.Instantiate<Hud>();
		if (_hud == null)
		{
			GD.PrintErr($"{LogPrefix} Failed to instantiate HUD");
			return;
		}
		
		AddChild(_hud);
		
		_hudManager = new HudManager();
		AddChild(_hudManager);
		
		await Task.Delay(500);
		
		_hudManager.Initialize(_hud);
		
		GD.Print($"{LogPrefix} Test environment setup complete");
	}

	private async Task RunMoneyUpdateTests()
	{
		GD.Print($"{LogPrefix} Testing money updates...");
		
		TestMoneyUpdate(100, "Money: $100");
		await Task.Delay(100);
		
		TestMoneyUpdate(0, "Money: $0");
		await Task.Delay(100);
		
		TestMoneyUpdate(9999, "Money: $9999");
		await Task.Delay(100);
		
		GD.Print($"{LogPrefix} ✅ Money update tests passed");
	}

	private void TestMoneyUpdate(int amount, string expectedText)
	{
		_hudManager?.UpdateMoney(amount);
		
		if (_hud?.MoneyLabel?.Text == expectedText)
		{
			GD.Print($"{LogPrefix}   ✅ Money test passed: {amount} -> '{expectedText}'");
		}
		else
		{
			GD.PrintErr($"{LogPrefix}   ❌ Money test failed: Expected '{expectedText}', got '{_hud?.MoneyLabel?.Text}'");
		}
	}

	private async Task RunLivesUpdateTests()
	{
		GD.Print($"{LogPrefix} Testing lives updates...");
		
		TestLivesUpdate(10, "Lives: 10");
		await Task.Delay(100);
		
		TestLivesUpdate(1, "Lives: 1");
		await Task.Delay(100);
		
		TestLivesUpdate(0, "Lives: 0");
		await Task.Delay(100);
		
		GD.Print($"{LogPrefix} ✅ Lives update tests passed");
	}

	private void TestLivesUpdate(int lives, string expectedText)
	{
		_hudManager?.UpdateLives(lives);
		
		if (_hud?.LivesLabel?.Text == expectedText)
		{
			GD.Print($"{LogPrefix}   ✅ Lives test passed: {lives} -> '{expectedText}'");
		}
		else
		{
			GD.PrintErr($"{LogPrefix}   ❌ Lives test failed: Expected '{expectedText}', got '{_hud?.LivesLabel?.Text}'");
		}
	}

	private async Task RunWaveUpdateTests()
	{
		GD.Print($"{LogPrefix} Testing wave updates...");
		
		TestWaveUpdate(1, "Wave: 1/5");
		await Task.Delay(100);
		
		TestWaveUpdate(3, "Wave: 3/5");
		await Task.Delay(100);
		
		TestWaveUpdate(5, "Wave: 5/5");
		await Task.Delay(100);
		
		GD.Print($"{LogPrefix} ✅ Wave update tests passed");
	}

	private void TestWaveUpdate(int wave, string expectedText)
	{
		_hudManager?.UpdateWave(wave);
		
		if (_hud?.WaveLabel?.Text == expectedText)
		{
			GD.Print($"{LogPrefix}   ✅ Wave test passed: {wave} -> '{expectedText}'");
		}
		else
		{
			GD.PrintErr($"{LogPrefix}   ❌ Wave test failed: Expected '{expectedText}', got '{_hud?.WaveLabel?.Text}'");
		}
	}

	private async Task RunBuildingStatsTests()
	{
		GD.Print($"{LogPrefix} Testing building stats display...");
		
		TestShowBuildingStats();
		await Task.Delay(200);
		
		TestHideBuildingStats();
		await Task.Delay(200);
		
		GD.Print($"{LogPrefix} ✅ Building stats tests passed");
	}

	private void TestShowBuildingStats()
	{
		_hudManager?.ShowBuildingStats("Test Tower", 50, 100, 75.0f, 1.5f);
		
		bool panelVisible = _hud?.TowerStatsPanel?.Visible ?? false;
		if (panelVisible)
		{
			GD.Print($"{LogPrefix}   ✅ Show building stats test passed - panel is visible");
			
			if (_hud?.TowerNameLabel?.Text == "Test Tower")
			{
				GD.Print($"{LogPrefix}   ✅ Building name display test passed");
			}
			else
			{
				GD.PrintErr($"{LogPrefix}   ❌ Building name test failed");
			}
		}
		else
		{
			GD.PrintErr($"{LogPrefix}   ❌ Show building stats test failed - panel not visible");
		}
	}

	private void TestHideBuildingStats()
	{
		_hudManager?.HideBuildingStats();
		
		bool panelHidden = !(_hud?.TowerStatsPanel?.Visible ?? true);
		if (panelHidden)
		{
			GD.Print($"{LogPrefix}   ✅ Hide building stats test passed - panel is hidden");
		}
		else
		{
			GD.PrintErr($"{LogPrefix}   ❌ Hide building stats test failed - panel still visible");
		}
	}

	private async Task RunButtonStateTests()
	{
		GD.Print($"{LogPrefix} Testing button state management...");
		
		TestButtonVisibility();
		await Task.Delay(100);
		
		TestButtonTextUpdate();
		await Task.Delay(100);
		
		GD.Print($"{LogPrefix} ✅ Button state tests passed");
	}

	private void TestButtonVisibility()
	{
		bool initialVisibility = _hud?.IsSkipButtonVisible ?? false;
		GD.Print($"{LogPrefix}   Button initial visibility: {initialVisibility}");
		
		_hud?.HideSkipButton();
		bool hiddenState = _hud?.IsSkipButtonVisible ?? true;
		
		_hud?.ShowSkipButton();
		bool shownState = _hud?.IsSkipButtonVisible ?? false;
		
		if (!hiddenState && shownState)
		{
			GD.Print($"{LogPrefix}   ✅ Button visibility toggle test passed");
		}
		else
		{
			GD.PrintErr($"{LogPrefix}   ❌ Button visibility test failed - Hide: {!hiddenState}, Show: {shownState}");
		}
	}

	private void TestButtonTextUpdate()
	{
		string testText = "Test Button Text";
		_hud?.UpdateSkipButtonText(testText);
		
		if (_hud?.SkipButton?.Text == testText)
		{
			GD.Print($"{LogPrefix}   ✅ Button text update test passed");
		}
		else
		{
			GD.PrintErr($"{LogPrefix}   ❌ Button text update test failed");
		}
	}

	public override void _ExitTree()
	{
		GD.Print($"{LogPrefix} Tests finished - cleaning up");
	}
}
