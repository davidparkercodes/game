using Godot;

public partial class Hud : CanvasLayer
{
	[Export] public Label MoneyLabel;
	[Export] public Label LivesLabel;
	[Export] public Label WaveLabel;
	[Export] public Label SelectedTurretLabel;
	[Export] public Button SkipButton;
	
	// Turret stats panel
	[Export] public Panel TurretStatsPanel;
	[Export] public Label TurretNameLabel;
	[Export] public Label CostLabel;
	[Export] public Label DamageLabel;
	[Export] public Label RangeLabel;
	[Export] public Label FireRateLabel;

	public override void _Ready()
	{
		// Fallback if the export slots were left empty in the Inspector
		MoneyLabel ??= GetNodeOrNull<Label>("SidebarPanel/VBoxContainer/MoneyLabel");
		LivesLabel ??= GetNodeOrNull<Label>("SidebarPanel/VBoxContainer/LivesLabel");
		WaveLabel  ??= GetNodeOrNull<Label>("SidebarPanel/VBoxContainer/WaveLabel");
		SelectedTurretLabel ??= GetNodeOrNull<Label>("SidebarPanel/VBoxContainer/SelectedTurretLabel");
		SkipButton ??= GetNodeOrNull<Button>("SidebarPanel/VBoxContainer/SkipButton");
		
		// Turret stats panel nodes
		TurretStatsPanel ??= GetNodeOrNull<Panel>("TurretStatsPanel");
		TurretNameLabel ??= GetNodeOrNull<Label>("TurretStatsPanel/VBoxContainer/TurretNameLabel");
		CostLabel ??= GetNodeOrNull<Label>("TurretStatsPanel/VBoxContainer/CostLabel");
		DamageLabel ??= GetNodeOrNull<Label>("TurretStatsPanel/VBoxContainer/DamageLabel");
		RangeLabel ??= GetNodeOrNull<Label>("TurretStatsPanel/VBoxContainer/RangeLabel");
		FireRateLabel ??= GetNodeOrNull<Label>("TurretStatsPanel/VBoxContainer/FireRateLabel");
		
		// Connect Skip button
		if (SkipButton != null)
		{
			SkipButton.Pressed += OnSkipButtonPressed;
			GD.Print("✅ SkipButton found and connected");
		}
		else
		{
			GD.PrintErr("❌ SkipButton not found in HUD");
		}
		
		// Force show button for initial testing
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

	public void UpdateMoney(int amount) => MoneyLabel?.SetText($"Money: ${amount}");
	public void UpdateLives(int lives)  => LivesLabel?.SetText($"Lives: {lives}");
	public void UpdateWave(int wave)    => WaveLabel?.SetText($"Wave: {wave}/{RoundManager.Instance?.TotalRounds ?? 5}");
	public void UpdateSelectedTurret(string turretName) => SelectedTurretLabel?.SetText($"Selected: {turretName}");
	public void UpdateSelectedBuilding(string buildingName) => SelectedTurretLabel?.SetText($"Selected: {buildingName}");
	
	public void ShowTurretStats(string turretName, int cost, int damage, float range, float fireRate)
	{
		if (TurretStatsPanel != null)
		{
			TurretStatsPanel.Visible = true;
			TurretNameLabel?.SetText(turretName);
			CostLabel?.SetText($"Cost: ${cost}");
			DamageLabel?.SetText($"Damage: {damage}");
			RangeLabel?.SetText($"Range: {range:F0}");
			FireRateLabel?.SetText($"Fire Rate: {fireRate:F1}s");
		}
	}
	
	public void HideTurretStats()
	{
		if (TurretStatsPanel != null)
		{
			TurretStatsPanel.Visible = false;
		}
	}
	
	public void ShowBuildingStats(string buildingName, int cost, int damage, float range, float fireRate)
	{
		if (TurretStatsPanel != null)
		{
			TurretStatsPanel.Visible = true;
			TurretNameLabel?.SetText(buildingName);
			CostLabel?.SetText($"Cost: ${cost}");
			DamageLabel?.SetText($"Damage: {damage}");
			RangeLabel?.SetText($"Range: {range:F0}");
			FireRateLabel?.SetText($"Fire Rate: {fireRate:F1}s");
		}
	}
	
	public void HideBuildingStats()
	{
		if (TurretStatsPanel != null)
		{
			TurretStatsPanel.Visible = false;
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
