using System;
using Game.Presentation.UI;
using Game.Infrastructure.Game.Services;
using Game.Infrastructure.Rounds.Services;
using Game.Infrastructure.Waves.Services;
using Game.Domain.Common.Services;
namespace Game.Application.Shared.Services;

public class HudDebugCommands
{
	private const string LogPrefix = "🐛 [HUD-DEBUG]";

	public static void ForceUpdateHudValues()
	{
		if (HudManager.Instance == null)
		{
			Console.WriteLine($"{LogPrefix} HudManager not available");
			return;
		}

		Console.WriteLine($"{LogPrefix} Force updating HUD values with test data...");
		
		HudManager.Instance.UpdateMoney(1500);
		HudManager.Instance.UpdateLives(8);
		HudManager.Instance.UpdateWave(3);
		
		Console.WriteLine($"{LogPrefix} HUD values force updated!");
	}

	public static void TestAllHudStates()
	{
		if (HudManager.Instance == null)
		{
			Console.WriteLine($"{LogPrefix} HudManager not available");
			return;
		}

		Console.WriteLine($"{LogPrefix} Testing all HUD states...");
		
		TestMoneyStates();
		TestLivesStates();
		TestWaveStates();
		TestBuildingStatsStates();
		TestButtonStates();
		
		Console.WriteLine($"{LogPrefix} All HUD state tests completed!");
	}

	private static void TestMoneyStates()
	{
		Console.WriteLine($"{LogPrefix} Testing money display states...");
		
		var testValues = new[] { 0, 50, 500, 1000, 9999 };
		foreach (var money in testValues)
		{
			HudManager.Instance?.UpdateMoney(money);
			Console.WriteLine($"{LogPrefix}   Money: ${money}");
		}
	}

	private static void TestLivesStates()
	{
		Console.WriteLine($"{LogPrefix} Testing lives display states...");
		
		var testValues = new[] { 0, 1, 5, 10, 20 };
		foreach (var lives in testValues)
		{
			HudManager.Instance?.UpdateLives(lives);
			Console.WriteLine($"{LogPrefix}   Lives: {lives}");
		}
	}

	private static void TestWaveStates()
	{
		Console.WriteLine($"{LogPrefix} Testing wave display states...");
		
		var testValues = new[] { 1, 2, 3, 4, 5 };
		foreach (var wave in testValues)
		{
			HudManager.Instance?.UpdateWave(wave);
			Console.WriteLine($"{LogPrefix}   Wave: {wave}");
		}
	}

	private static void TestBuildingStatsStates()
	{
		Console.WriteLine($"{LogPrefix} Testing building stats display states...");
		
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
			Console.WriteLine($"{LogPrefix}   Building: {name} (${cost}, {damage} dmg, {range} range, {fireRate}s)");
		}
		
		HudManager.Instance?.HideBuildingStats();
		Console.WriteLine($"{LogPrefix}   Building stats hidden");
	}

	private static void TestButtonStates()
	{
		if (HudManager.Instance?.GetHud() == null)
		{
			Console.WriteLine($"{LogPrefix} HUD instance not available for button testing");
			return;
		}

		var hud = HudManager.Instance.GetHud();
		if (hud == null) return;

		Console.WriteLine($"{LogPrefix} Testing button states...");
		
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
			Console.WriteLine($"{LogPrefix}   Button text: {text}");
		}

		hud.HideSkipButton();
		Console.WriteLine($"{LogPrefix}   Button hidden");
		
		hud.ShowSkipButton();
		Console.WriteLine($"{LogPrefix}   Button shown");
	}

	public static void ToggleHudVisibility()
	{
		if (HudManager.Instance?.GetHud() == null)
		{
			Console.WriteLine($"{LogPrefix} HUD instance not available");
			return;
		}

		var hud = HudManager.Instance.GetHud();
		if (hud == null) return;

		bool currentVisibility = hud.Visible;
		hud.Visible = !currentVisibility;
		
		Console.WriteLine($"{LogPrefix} HUD visibility toggled: {currentVisibility} -> {!currentVisibility}");
	}

	public static void ResetHudToDefaults()
	{
		if (HudManager.Instance == null)
		{
			Console.WriteLine($"{LogPrefix} HudManager not available");
			return;
		}

		Console.WriteLine($"{LogPrefix} Resetting HUD to default state...");
		
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
		
		Console.WriteLine($"{LogPrefix} HUD reset to defaults: ${defaultMoney}, {defaultLives} lives, wave {defaultWave}");
	}

	public static void DiagnoseHudComponents()
	{
		Console.WriteLine($"{LogPrefix} Diagnosing HUD component status...");
		
		if (HudManager.Instance == null)
		{
			Console.WriteLine($"{LogPrefix} ❌ HudManager.Instance is null");
			return;
		}
		
		Console.WriteLine($"{LogPrefix} ✅ HudManager.Instance is available");
		
		var hud = HudManager.Instance.GetHud();
		if (hud == null)
		{
			Console.WriteLine($"{LogPrefix} ❌ HUD instance is null");
			return;
		}
		
		Console.WriteLine($"{LogPrefix} ✅ HUD instance is available");
		Console.WriteLine($"{LogPrefix} HUD initialization status: {(hud.IsInitialized ? "✅ Initialized" : "❌ Not initialized")}");
		Console.WriteLine($"{LogPrefix} HUD visibility: {(hud.Visible ? "✅ Visible" : "❌ Hidden")}");
		
		DiagnoseHudLabels(hud);
		DiagnoseHudPanels(hud);
		DiagnoseHudButtons(hud);
	}

	private static void DiagnoseHudLabels(Hud hud)
	{
		Console.WriteLine($"{LogPrefix} Label Status:");
		Console.WriteLine($"{LogPrefix}   MoneyLabel: {(hud.MoneyLabel != null ? "✅" : "❌")} - Text: '{hud.MoneyLabel?.Text ?? "null"}'");
		Console.WriteLine($"{LogPrefix}   LivesLabel: {(hud.LivesLabel != null ? "✅" : "❌")} - Text: '{hud.LivesLabel?.Text ?? "null"}'");
		Console.WriteLine($"{LogPrefix}   WaveLabel: {(hud.WaveLabel != null ? "✅" : "❌")} - Text: '{hud.WaveLabel?.Text ?? "null"}'");
		Console.WriteLine($"{LogPrefix}   TowerNameLabel: {(hud.TowerNameLabel != null ? "✅" : "❌")} - Text: '{hud.TowerNameLabel?.Text ?? "null"}'");
	}

	private static void DiagnoseHudPanels(Hud hud)
	{
		Console.WriteLine($"{LogPrefix} Panel Status:");
		
		if (hud.TowerStatsPanel != null)
		{
			Console.WriteLine($"{LogPrefix}   TowerStatsPanel: ✅ - Visible: {(hud.TowerStatsPanel.Visible ? "✅" : "❌")}");
		}
		else
		{
			Console.WriteLine($"{LogPrefix}   TowerStatsPanel: ❌");
		}
	}

	private static void DiagnoseHudButtons(Hud hud)
	{
		Console.WriteLine($"{LogPrefix} Button Status:");
		
		if (hud.SkipButton != null)
		{
			Console.WriteLine($"{LogPrefix}   SkipButton: ✅ - Visible: {(hud.SkipButton.Visible ? "✅" : "❌")} - Text: '{hud.SkipButton.Text}'");
			Console.WriteLine($"{LogPrefix}   SkipButton Visibility Helper: {(hud.IsSkipButtonVisible ? "✅" : "❌")}");
		}
		else
		{
			Console.WriteLine($"{LogPrefix}   SkipButton: ❌");
		}
	}

	public static void PrintHudCommands()
	{
		Console.WriteLine($"{LogPrefix} Available HUD Debug Commands:");
		Console.WriteLine($"{LogPrefix}   HudDebugCommands.ForceUpdateHudValues() - Update with test values");
		Console.WriteLine($"{LogPrefix}   HudDebugCommands.TestAllHudStates() - Test all display states");
		Console.WriteLine($"{LogPrefix}   HudDebugCommands.ToggleHudVisibility() - Show/hide HUD");
		Console.WriteLine($"{LogPrefix}   HudDebugCommands.ResetHudToDefaults() - Reset to game defaults");
		Console.WriteLine($"{LogPrefix}   HudDebugCommands.DiagnoseHudComponents() - Diagnose component status");
		Console.WriteLine($"{LogPrefix}   HudDebugCommands.PrintHudCommands() - Show this help");
	}
}
