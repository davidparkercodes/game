using Godot;

public partial class Hud : CanvasLayer
{
	[Export] public Label MoneyLabel;
	[Export] public Label LivesLabel;
	[Export] public Label WaveLabel;
	[Export] public Label SelectedTurretLabel;
	
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
		
		// Turret stats panel nodes
		TurretStatsPanel ??= GetNodeOrNull<Panel>("TurretStatsPanel");
		TurretNameLabel ??= GetNodeOrNull<Label>("TurretStatsPanel/VBoxContainer/TurretNameLabel");
		CostLabel ??= GetNodeOrNull<Label>("TurretStatsPanel/VBoxContainer/CostLabel");
		DamageLabel ??= GetNodeOrNull<Label>("TurretStatsPanel/VBoxContainer/DamageLabel");
		RangeLabel ??= GetNodeOrNull<Label>("TurretStatsPanel/VBoxContainer/RangeLabel");
		FireRateLabel ??= GetNodeOrNull<Label>("TurretStatsPanel/VBoxContainer/FireRateLabel");
	}

	public void UpdateMoney(int amount) => MoneyLabel?.SetText($"Money: ${amount}");
	public void UpdateLives(int lives)  => LivesLabel?.SetText($"Lives: {lives}");
	public void UpdateWave(int wave)    => WaveLabel?.SetText($"Wave: {wave}");
	public void UpdateSelectedTurret(string turretName) => SelectedTurretLabel?.SetText($"Selected: {turretName}");
	
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
}
