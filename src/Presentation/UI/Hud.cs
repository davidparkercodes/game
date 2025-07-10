using Godot;
using Game.Infrastructure.Rounds.Services;
using Game.Infrastructure.Waves.Services;
using System.Collections.Generic;

namespace Game.Presentation.UI;

public partial class Hud : CanvasLayer
{
	[Export] public Label? MoneyLabel;
	[Export] public Label? LivesLabel;
	[Export] public Label? WaveLabel;
	[Export] public Button? SkipButton;

	[Export] public Panel? TowerStatsPanel;
	[Export] public Label? TowerNameLabel;
	[Export] public RichTextLabel? CostLabel;
	[Export] public RichTextLabel? DamageLabel;
	[Export] public RichTextLabel? RangeLabel;
	[Export] public RichTextLabel? FireRateLabel;

	[Export] public Panel? VictoryPanel;
	[Export] public Label? VictoryLabel;
	[Export] public Label? VictorySubLabel;

	private bool _isInitialized = false;
	private const string LogPrefix = "🎨 [HUD]";

	public override void _Ready()
	{
		GD.Print($"{LogPrefix} Initializing HUD components...");

		InitializeNodeReferences();
		InitializeEventConnections();
		SetupInitialState();

		_isInitialized = true;
		GD.Print($"{LogPrefix} HUD initialization complete!");

		CallDeferred(nameof(PerformStartupValidation));
	}

	private void InitializeNodeReferences()
	{
		MoneyLabel ??= GetNodeOrNull<Label>("SidebarPanel/VBoxContainer/MoneyLabel");
		LivesLabel ??= GetNodeOrNull<Label>("SidebarPanel/VBoxContainer/LivesLabel");
		WaveLabel ??= GetNodeOrNull<Label>("SidebarPanel/VBoxContainer/WaveLabel");
		SkipButton ??= GetNodeOrNull<Button>("SidebarPanel/VBoxContainer/SkipButton");

		TowerStatsPanel ??= GetNodeOrNull<Panel>("TowerStatsPanel");
		TowerNameLabel ??= GetNodeOrNull<Label>("TowerStatsPanel/VBoxContainer/TowerNameLabel");
		CostLabel ??= GetNodeOrNull<RichTextLabel>("TowerStatsPanel/VBoxContainer/CostLabel");
		DamageLabel ??= GetNodeOrNull<RichTextLabel>("TowerStatsPanel/VBoxContainer/DamageLabel");
		RangeLabel ??= GetNodeOrNull<RichTextLabel>("TowerStatsPanel/VBoxContainer/RangeLabel");
		FireRateLabel ??= GetNodeOrNull<RichTextLabel>("TowerStatsPanel/VBoxContainer/FireRateLabel");

		VictoryPanel ??= GetNodeOrNull<Panel>("VictoryPanel");
		VictoryLabel ??= GetNodeOrNull<Label>("VictoryPanel/VictoryContainer/VictoryLabel");
		VictorySubLabel ??= GetNodeOrNull<Label>("VictoryPanel/VictoryContainer/VictorySubLabel");

		// Initialize BuildingUpgradeHud connection
		InitializeBuildingUpgradeHud();

		LogNodeStatus();
	}

	private void InitializeEventConnections()
	{
		if (SkipButton != null)
		{
			SkipButton.Pressed += OnSkipButtonPressed;
			GD.Print($"{LogPrefix} SkipButton connected successfully");
		}
		else
		{
			GD.PrintErr($"{LogPrefix} SkipButton not found - wave progression may not work");
		}
	}

	private void SetupInitialState()
	{
		if (TowerStatsPanel != null)
		{
			TowerStatsPanel.Visible = false;
		}

		if (VictoryPanel != null)
		{
			VictoryPanel.Visible = false;
		}

		if (SkipButton != null)
		{
			SkipButton.Visible = true;
			SkipButton.Text = "⏭️ Start Wave";
		}

		SetDefaultLabels();
	}

	private void SetDefaultLabels()
	{
		if (MoneyLabel != null) MoneyLabel.Text = "⚡$0";
		if (LivesLabel != null) LivesLabel.Text = "Lives: 0";
		if (WaveLabel != null) WaveLabel.Text = "Wave: 0/?";
	}

	private void LogNodeStatus()
	{
		GD.Print($"{LogPrefix} Node Status:");
		GD.Print($"  📊 MoneyLabel: {(MoneyLabel != null ? "✅" : "❌")}");
		GD.Print($"  ❤️ LivesLabel: {(LivesLabel != null ? "✅" : "❌")}");
		GD.Print($"  🌊 WaveLabel: {(WaveLabel != null ? "✅" : "❌")}");
		GD.Print($"  ⏭️ SkipButton: {(SkipButton != null ? "✅" : "❌")}");
		GD.Print($"  🏗️ TowerStatsPanel: {(TowerStatsPanel != null ? "✅" : "❌")}");
		GD.Print($"  🎉 VictoryPanel: {(VictoryPanel != null ? "✅" : "❌")}");
	}

	private void PerformStartupValidation()
	{
		var missingComponents = new List<string>();

		if (MoneyLabel == null) missingComponents.Add("MoneyLabel");
		if (LivesLabel == null) missingComponents.Add("LivesLabel");
		if (WaveLabel == null) missingComponents.Add("WaveLabel");
		if (SkipButton == null) missingComponents.Add("SkipButton");
		if (TowerStatsPanel == null) missingComponents.Add("TowerStatsPanel");
		if (VictoryPanel == null) missingComponents.Add("VictoryPanel");

		if (missingComponents.Count > 0)
		{
			GD.PrintErr($"{LogPrefix} Missing components: {string.Join(", ", missingComponents)}");
			GD.PrintErr($"{LogPrefix} HUD may not function correctly!");
		}
		else
		{
			GD.Print($"{LogPrefix} All components found and validated successfully!");
		}

		if (SkipButton != null)
		{
			GD.Print($"{LogPrefix} Button validation - Visible: {SkipButton.Visible}, Text: '{SkipButton.Text}'");
		}
	}

	public void UpdateMoney(int amount)
	{
		if (!ValidateComponent(MoneyLabel, "MoneyLabel")) return;
		MoneyLabel!.Text = $"⚡${amount}";
	}

	public void UpdateLives(int lives)
	{
		if (!ValidateComponent(LivesLabel, "LivesLabel")) return;
		LivesLabel!.Text = $"Lives: {lives}";
	}

	public void UpdateWave(int wave)
	{
		if (!ValidateComponent(WaveLabel, "WaveLabel")) return;
		var totalRounds = RoundService.Instance?.TotalRounds ?? -1;
		var totalDisplay = totalRounds > 0 ? totalRounds.ToString() : "?";
		WaveLabel!.Text = $"Wave: {wave}/{totalDisplay}";
	}

	private bool ValidateComponent(Node? component, string componentName)
	{
		if (component == null)
		{
			GD.PrintErr($"{LogPrefix} Cannot update {componentName} - component is null");
			return false;
		}
		return true;
	}

	public void ShowTowerStats(string towerName, int cost, int damage, float range, float attackSpeed)
	{
		if (!ValidateComponent(TowerStatsPanel, "TowerStatsPanel")) return;

		TowerStatsPanel!.Visible = true;

		if (TowerNameLabel != null) TowerNameLabel.Text = towerName;
		if (CostLabel != null) CostLabel.Text = $"[font_size=10][color=white]Cost:[/color] [color=#4eadc7]${cost}[/color][/font_size]";
		if (DamageLabel != null) DamageLabel.Text = $"[font_size=10][color=white]Damage:[/color] [color=#4eadc7]{damage}[/color][/font_size]";
		if (RangeLabel != null) RangeLabel.Text = $"[font_size=10][color=white]Range:[/color] [color=#4eadc7]{range:F0}[/color][/font_size]";
		if (FireRateLabel != null) FireRateLabel.Text = $"[font_size=10][color=white]Attack Speed:[/color] [color=#4eadc7]{attackSpeed:F0}[/color][/font_size]";

		GD.Print($"{LogPrefix} Showing tower stats for: {towerName}");
	}

	public void HideTowerStats()
	{
		if (TowerStatsPanel != null)
		{
			TowerStatsPanel.Visible = false;
			GD.Print($"{LogPrefix} Tower stats panel hidden");
		}
	}

	public void ShowBuildingStats(string buildingName, int cost, int damage, float range, float attackSpeed)
	{
		if (!ValidateComponent(TowerStatsPanel, "TowerStatsPanel")) return;

		TowerStatsPanel!.Visible = true;

		if (TowerNameLabel != null) TowerNameLabel.Text = buildingName;
		if (CostLabel != null) CostLabel.Text = $"[font_size=10][color=white]Cost:[/color] [color=#4eadc7]${cost}[/color][/font_size]";
		if (DamageLabel != null) DamageLabel.Text = $"[font_size=10][color=white]Damage:[/color] [color=#4eadc7]{damage}[/color][/font_size]";
		if (RangeLabel != null) RangeLabel.Text = $"[font_size=10][color=white]Range:[/color] [color=#4eadc7]{range:F0}[/color][/font_size]";
		if (FireRateLabel != null) FireRateLabel.Text = $"[font_size=10][color=white]Attack Speed:[/color] [color=#4eadc7]{attackSpeed:F0}[/color][/font_size]";

		GD.Print($"{LogPrefix} Showing building stats for: {buildingName}");
	}

	public void HideBuildingStats()
	{
		if (TowerStatsPanel != null)
		{
			TowerStatsPanel.Visible = false;
			GD.Print($"{LogPrefix} Building stats panel hidden");
		}
	}

	public void ShowSkipButton()
	{
		if (ValidateComponent(SkipButton, "SkipButton"))
		{
			SkipButton!.Visible = true;
			GD.Print($"{LogPrefix} Skip button shown");
		}
	}

	public void HideSkipButton()
	{
		if (SkipButton != null)
		{
			SkipButton.Visible = false;
			GD.Print($"{LogPrefix} Skip button hidden");
		}
	}

	public void UpdateSkipButtonText(string newText)
	{
		if (ValidateComponent(SkipButton, "SkipButton"))
		{
			SkipButton!.Text = newText;
			GD.Print($"{LogPrefix} Skip button text updated to: {newText}");
		}
	}

	public bool IsSkipButtonVisible => SkipButton?.Visible ?? false;

	public bool IsInitialized => _isInitialized;

	public void ShowVictoryMessage()
	{
		if (!ValidateComponent(VictoryPanel, "VictoryPanel")) return;
		VictoryPanel!.Visible = true;
		GD.Print($"{LogPrefix} Victory message shown - You Win! :)");
	}

	public void HideVictoryMessage()
	{
		if (VictoryPanel != null)
		{
			VictoryPanel.Visible = false;
			GD.Print($"{LogPrefix} Victory message hidden");
		}
	}

	private void OnSkipButtonPressed()
	{
		GD.Print($"{LogPrefix} Skip button pressed - starting next wave");
		WaveManager.Instance?.StartNextWave();
	}

	private void InitializeBuildingUpgradeHud()
	{
		try
		{
			var buildingUpgradeHud = GetNodeOrNull<BuildingUpgradeHud>("BuildingUpgradeHud");
			if (buildingUpgradeHud != null)
			{
				BuildingSelectionManager.Instance.InitializeBuildingUpgradeHud(buildingUpgradeHud);
				GD.Print($"{LogPrefix} BuildingUpgradeHud connected to BuildingSelectionManager");
			}
			else
			{
				GD.PrintErr($"{LogPrefix} BuildingUpgradeHud not found in scene");
			}
		}
		catch (System.Exception ex)
		{
			GD.PrintErr($"{LogPrefix} Error initializing BuildingUpgradeHud: {ex.Message}");
		}
	}
}
