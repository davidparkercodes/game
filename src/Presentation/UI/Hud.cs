using Godot;
using Game.Infrastructure.Managers;

namespace Game.Presentation.UI;

public partial class Hud : CanvasLayer
{
	[Export] public Label MoneyLabel;
	[Export] public Label LivesLabel;
	[Export] public Label WaveLabel;
	[Export] public Button SkipButton;
	
	[Export] public Panel TowerStatsPanel;
	[Export] public Label TowerNameLabel;
	[Export] public Label CostLabel;
	[Export] public Label DamageLabel;
	[Export] public Label RangeLabel;
	[Export] public Label FireRateLabel;

	public override void _Ready()
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
		
		if (SkipButton != null)
		{
			SkipButton.Pressed += OnSkipButtonPressed;
			GD.Print("✅ SkipButton found and connected");
		}
		else
		{
			GD.PrintErr("❌ SkipButton not found in HUD");
		}
		
		CallDeferred(nameof(TestButtonVisibility));
	}
	
	private void TestButtonVisibility()
	{
		if (SkipButton != null)
		{
			GD.Print($"📍 Button found! Visible: {SkipButton.Visible}, Text: '{SkipButton.Text}'");
			SkipButton.Visible = true;
		}
		else
		{
			GD.PrintErr("📍 Button still null in deferred call");
		}
	}

	public void UpdateMoney(int amount) { if (MoneyLabel != null) MoneyLabel.Text = $"Money: ${amount}"; }
	public void UpdateLives(int lives) { if (LivesLabel != null) LivesLabel.Text = $"Lives: {lives}"; }
	public void UpdateWave(int wave) { if (WaveLabel != null) WaveLabel.Text = $"Wave: {wave}/{RoundManager.Instance?.TotalRounds ?? 5}"; }
	
	public void ShowTowerStats(string towerName, int cost, int damage, float range, float fireRate)
	{
		if (TowerStatsPanel != null)
		{
			TowerStatsPanel.Visible = true;
			if (TowerNameLabel != null) TowerNameLabel.Text = towerName;
			if (CostLabel != null) CostLabel.Text = $"Cost: ${cost}";
			if (DamageLabel != null) DamageLabel.Text = $"Damage: {damage}";
			if (RangeLabel != null) RangeLabel.Text = $"Range: {range:F0}";
			if (FireRateLabel != null) FireRateLabel.Text = $"Fire Rate: {fireRate:F1}s";
		}
	}

	public void HideTowerStats()
	{
		if (TowerStatsPanel != null)
		{
			TowerStatsPanel.Visible = false;
		}
	}
	
	public void ShowBuildingStats(string buildingName, int cost, int damage, float range, float fireRate)
	{
		if (TowerStatsPanel != null)
		{
			TowerStatsPanel.Visible = true;
			TowerNameLabel?.SetText(buildingName);
			CostLabel?.SetText($"Cost: ${cost}");
			DamageLabel?.SetText($"Damage: {damage}");
			RangeLabel?.SetText($"Range: {range:F0}");
			FireRateLabel?.SetText($"Fire Rate: {fireRate:F1}s");
		}
	}

	public void HideBuildingStats()
	{
		if (TowerStatsPanel != null)
		{
			TowerStatsPanel.Visible = false;
		}
	}
	
	public void ShowSkipButton()
	{
		if (SkipButton != null)
		{
			SkipButton.Visible = true;
			GD.Print("🔄 Skip button shown");
		}
		else
		{
			GD.PrintErr("❌ Cannot show skip button - button is null");
		}
	}
	
	public void HideSkipButton()
	{
		if (SkipButton != null)
		{
			SkipButton.Visible = false;
		}
	}
	
	private void OnSkipButtonPressed()
	{
		if (RoundManager.Instance != null)
		{
			RoundManager.Instance.ForceStartDefendPhase();
		}
	}
}
