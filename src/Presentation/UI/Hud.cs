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
	[Export] public Label? CostLabel;
	[Export] public Label? DamageLabel;
	[Export] public Label? RangeLabel;
	[Export] public Label? FireRateLabel;

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
		WaveLabel  ??= GetNodeOrNull<Label>("SidebarPanel/VBoxContainer/WaveLabel");
		SkipButton ??= GetNodeOrNull<Button>("SidebarPanel/VBoxContainer/SkipButton");
		
		TowerStatsPanel ??= GetNodeOrNull<Panel>("TowerStatsPanel");
		TowerNameLabel ??= GetNodeOrNull<Label>("TowerStatsPanel/VBoxContainer/TowerNameLabel");
		CostLabel ??= GetNodeOrNull<Label>("TowerStatsPanel/VBoxContainer/CostLabel");
		DamageLabel ??= GetNodeOrNull<Label>("TowerStatsPanel/VBoxContainer/DamageLabel");
		RangeLabel ??= GetNodeOrNull<Label>("TowerStatsPanel/VBoxContainer/RangeLabel");
		FireRateLabel ??= GetNodeOrNull<Label>("TowerStatsPanel/VBoxContainer/FireRateLabel");
		
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
		
		if (SkipButton != null)
		{
			SkipButton.Visible = true;
			SkipButton.Text = "⏭️ Start Wave";
		}
		
		SetDefaultLabels();
	}

	private void SetDefaultLabels()
	{
		if (MoneyLabel != null) MoneyLabel.Text = "Money: $0";
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
	}
	
	private void PerformStartupValidation()
	{
		var missingComponents = new List<string>();
		
		if (MoneyLabel == null) missingComponents.Add("MoneyLabel");
		if (LivesLabel == null) missingComponents.Add("LivesLabel");
		if (WaveLabel == null) missingComponents.Add("WaveLabel");
		if (SkipButton == null) missingComponents.Add("SkipButton");
		if (TowerStatsPanel == null) missingComponents.Add("TowerStatsPanel");
		
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
		MoneyLabel!.Text = $"Money: ${amount}";
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
	
	public void ShowTowerStats(string towerName, int cost, int damage, float range, float fireRate)
	{
		if (!ValidateComponent(TowerStatsPanel, "TowerStatsPanel")) return;
		
		TowerStatsPanel!.Visible = true;
		
		if (TowerNameLabel != null) TowerNameLabel.Text = towerName;
		if (CostLabel != null) CostLabel.Text = $"Cost: ${cost}";
		if (DamageLabel != null) DamageLabel.Text = $"Damage: {damage}";
		if (RangeLabel != null) RangeLabel.Text = $"Range: {range:F0}";
		if (FireRateLabel != null) FireRateLabel.Text = $"Fire Rate: {fireRate:F1}s";
		
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
	
	public void ShowBuildingStats(string buildingName, int cost, int damage, float range, float fireRate)
	{
		if (!ValidateComponent(TowerStatsPanel, "TowerStatsPanel")) return;
		
		TowerStatsPanel!.Visible = true;
		
		if (TowerNameLabel != null) TowerNameLabel.Text = buildingName;
		if (CostLabel != null) CostLabel.Text = $"Cost: ${cost}";
		if (DamageLabel != null) DamageLabel.Text = $"Damage: {damage}";
		if (RangeLabel != null) RangeLabel.Text = $"Range: {range:F0}";
		if (FireRateLabel != null) FireRateLabel.Text = $"Fire Rate: {fireRate:F1}s";
		
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
	
	private void OnSkipButtonPressed()
	{
		GD.Print($"{LogPrefix} Skip button pressed - starting next wave");
		WaveManager.Instance?.StartNextWave();
	}
}
