using Godot;
using Game.Presentation.UI;
using Game.Infrastructure.Game.Services;
using Game.Infrastructure.Rounds.Services;
using Game.Infrastructure.Waves.Services;

namespace Game.Application.Shared.Services;

public class HudDebugCommands
{
	private const string LogPrefix = "🐛 [HUD-DEBUG]";

	public static void ForceUpdateHudValues()
	{
		if (HudManager.Instance == null)
		{
			GD.PrintErr($"{LogPrefix} HudManager not available");
			return;
		}

		GD.Print($"{LogPrefix} Force updating HUD values with test data...");
		
		HudManager.Instance.UpdateMoney(1500);
		HudManager.Instance.UpdateLives(8);
		HudManager.Instance.UpdateWave(3);
		
		GD.Print($"{LogPrefix} HUD values force updated!");
	}

	public static void TestAllHudStates()
	{
		if (HudManager.Instance == null)
		{
			GD.PrintErr($"{LogPrefix} HudManager not available");
			return;
		}

		GD.Print($"{LogPrefix} Testing all HUD states...");
		
		TestMoneyStates();
		TestLivesStates();
		TestWaveStates();
		TestBuildingStatsStates();
		TestButtonStates();
		
		GD.Print($"{LogPrefix} All HUD state tests completed!");
	}

	private static void TestMoneyStates()
	{
		GD.Print($"{LogPrefix} Testing money display states...");
		
		var testValues = new[] { 0, 50, 500, 1000, 9999 };
		foreach (var money in testValues)
		{
			HudManager.Instance?.UpdateMoney(money);
			GD.Print($"{LogPrefix}   Money: ${money}");
		}
	}

	private static void TestLivesStates()
	{
		GD.Print($"{LogPrefix} Testing lives display states...");
		
		var testValues = new[] { 0, 1, 5, 10, 20 };
		foreach (var lives in testValues)
		{
			HudManager.Instance?.UpdateLives(lives);
			GD.Print($"{LogPrefix}   Lives: {lives}");
		}
	}

	private static void TestWaveStates()
	{
		GD.Print($"{LogPrefix} Testing wave display states...");
		
		var testValues = new[] { 1, 2, 3, 4, 5 };
		foreach (var wave in testValues)
		{
			HudManager.Instance?.UpdateWave(wave);
			GD.Print($"{LogPrefix}   Wave: {wave}");
		}
	}

	private static void TestBuildingStatsStates()
	{
		GD.Print($"{LogPrefix} Testing building stats display states...");
		
		var testBuildings = new[]
		{
			("Basic Tower", 50, 25, 75f, 1.0f),
			("Advanced Tower", 100, 50, 100f, 0.8f),
			("Sniper Tower", 200, 100, 150f, 2.0f),
			("Machine Gun", 150, 15, 60f, 0.2f)
		};

		foreach (var (name, cost, damage, range, fireRate) in testBuildings)
		{
			HudManager.Instance?.ShowBuildingStats(name, cost, damage, range, fireRate);
			GD.Print($"{LogPrefix}   Building: {name} (${cost}, {damage} dmg, {range} range, {fireRate}s)");
		}
		
		HudManager.Instance?.HideBuildingStats();
		GD.Print($"{LogPrefix}   Building stats hidden");
	}

	private static void TestButtonStates()
	{
		if (HudManager.Instance?.GetHud() == null)
		{
			GD.PrintErr($"{LogPrefix} HUD instance not available for button testing");
			return;
		}

		var hud = HudManager.Instance.GetHud();
		if (hud == null) return;

		GD.Print($"{LogPrefix} Testing button states...");
		
		var testTexts = new[]
		{
			"⏭️ Start Wave",
			"🔄 Wave in Progress",
			"✅ Wave Complete",
			"🎉 All Waves Complete",
			"💥 Emergency Stop"
		};

		foreach (var text in testTexts)
		{
			hud.UpdateSkipButtonText(text);
			GD.Print($"{LogPrefix}   Button text: {text}");
		}

		hud.HideSkipButton();
		GD.Print($"{LogPrefix}   Button hidden");
		
		hud.ShowSkipButton();
		GD.Print($"{LogPrefix}   Button shown");
	}

	public static void ToggleHudVisibility()
	{
		if (HudManager.Instance?.GetHud() == null)
		{
			GD.PrintErr($"{LogPrefix} HUD instance not available");
			return;
		}

		var hud = HudManager.Instance.GetHud();
		if (hud == null) return;

		bool currentVisibility = hud.Visible;
		hud.Visible = !currentVisibility;
		
		GD.Print($"{LogPrefix} HUD visibility toggled: {currentVisibility} -> {!currentVisibility}");
	}

	public static void ResetHudToDefaults()
	{
		if (HudManager.Instance == null)
		{
			GD.PrintErr($"{LogPrefix} HudManager not available");
			return;
		}

		GD.Print($"{LogPrefix} Resetting HUD to default state...");
		
		var gameService = GameService.Instance;
		var roundService = RoundService.Instance;
		
		var defaultMoney = gameService?.Money ?? 100;
		var defaultLives = gameService?.Lives ?? 10;
		var defaultWave = roundService?.CurrentRound ?? 1;
		
		HudManager.Instance.UpdateMoney(defaultMoney);
		HudManager.Instance.UpdateLives(defaultLives);
		HudManager.Instance.UpdateWave(defaultWave);
		HudManager.Instance.HideBuildingStats();
		
		var hud = HudManager.Instance.GetHud();
		if (hud != null)
		{
			hud.ShowSkipButton();
			hud.UpdateSkipButtonText("⏭️ Start Wave");
		}
		
		GD.Print($"{LogPrefix} HUD reset to defaults: ${defaultMoney}, {defaultLives} lives, wave {defaultWave}");
	}

	public static void DiagnoseHudComponents()
	{
		GD.Print($"{LogPrefix} Diagnosing HUD component status...");
		
		if (HudManager.Instance == null)
		{
			GD.PrintErr($"{LogPrefix} ❌ HudManager.Instance is null");
			return;
		}
		
		GD.Print($"{LogPrefix} ✅ HudManager.Instance is available");
		
		var hud = HudManager.Instance.GetHud();
		if (hud == null)
		{
			GD.PrintErr($"{LogPrefix} ❌ HUD instance is null");
			return;
		}
		
		GD.Print($"{LogPrefix} ✅ HUD instance is available");
		GD.Print($"{LogPrefix} HUD initialization status: {(hud.IsInitialized ? "✅ Initialized" : "❌ Not initialized")}");
		GD.Print($"{LogPrefix} HUD visibility: {(hud.Visible ? "✅ Visible" : "❌ Hidden")}");
		
		DiagnoseHudLabels(hud);
		DiagnoseHudPanels(hud);
		DiagnoseHudButtons(hud);
	}

	private static void DiagnoseHudLabels(Hud hud)
	{
		GD.Print($"{LogPrefix} Label Status:");
		GD.Print($"{LogPrefix}   MoneyLabel: {(hud.MoneyLabel != null ? "✅" : "❌")} - Text: '{hud.MoneyLabel?.Text ?? "null"}'");
		GD.Print($"{LogPrefix}   LivesLabel: {(hud.LivesLabel != null ? "✅" : "❌")} - Text: '{hud.LivesLabel?.Text ?? "null"}'");
		GD.Print($"{LogPrefix}   WaveLabel: {(hud.WaveLabel != null ? "✅" : "❌")} - Text: '{hud.WaveLabel?.Text ?? "null"}'");
		GD.Print($"{LogPrefix}   TowerNameLabel: {(hud.TowerNameLabel != null ? "✅" : "❌")} - Text: '{hud.TowerNameLabel?.Text ?? "null"}'");
	}

	private static void DiagnoseHudPanels(Hud hud)
	{
		GD.Print($"{LogPrefix} Panel Status:");
		
		if (hud.TowerStatsPanel != null)
		{
			GD.Print($"{LogPrefix}   TowerStatsPanel: ✅ - Visible: {(hud.TowerStatsPanel.Visible ? "✅" : "❌")}");
		}
		else
		{
			GD.Print($"{LogPrefix}   TowerStatsPanel: ❌");
		}
	}

	private static void DiagnoseHudButtons(Hud hud)
	{
		GD.Print($"{LogPrefix} Button Status:");
		
		if (hud.SkipButton != null)
		{
			GD.Print($"{LogPrefix}   SkipButton: ✅ - Visible: {(hud.SkipButton.Visible ? "✅" : "❌")} - Text: '{hud.SkipButton.Text}'");
			GD.Print($"{LogPrefix}   SkipButton Visibility Helper: {(hud.IsSkipButtonVisible ? "✅" : "❌")}");
		}
		else
		{
			GD.Print($"{LogPrefix}   SkipButton: ❌");
		}
	}

	public static void PrintHudCommands()
	{
		GD.Print($"{LogPrefix} Available HUD Debug Commands:");
		GD.Print($"{LogPrefix}   HudDebugCommands.ForceUpdateHudValues() - Update with test values");
		GD.Print($"{LogPrefix}   HudDebugCommands.TestAllHudStates() - Test all display states");
		GD.Print($"{LogPrefix}   HudDebugCommands.ToggleHudVisibility() - Show/hide HUD");
		GD.Print($"{LogPrefix}   HudDebugCommands.ResetHudToDefaults() - Reset to game defaults");
		GD.Print($"{LogPrefix}   HudDebugCommands.DiagnoseHudComponents() - Diagnose component status");
		GD.Print($"{LogPrefix}   HudDebugCommands.PrintHudCommands() - Show this help");
	}
}
