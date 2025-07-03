using Godot;

public partial class Hud : CanvasLayer
{
	[Export] public Label MoneyLabel;
	[Export] public Label LivesLabel;
	[Export] public Label WaveLabel;
	[Export] public Label SelectedTurretLabel;

	public override void _Ready()
	{
		// Fallback if the export slots were left empty in the Inspector
		MoneyLabel ??= GetNodeOrNull<Label>("SidebarPanel/VBoxContainer/MoneyLabel");
		LivesLabel ??= GetNodeOrNull<Label>("SidebarPanel/VBoxContainer/LivesLabel");
		WaveLabel  ??= GetNodeOrNull<Label>("SidebarPanel/VBoxContainer/WaveLabel");
		SelectedTurretLabel ??= GetNodeOrNull<Label>("SidebarPanel/VBoxContainer/SelectedTurretLabel");
	}

	public void UpdateMoney(int amount) => MoneyLabel?.SetText($"Money: ${amount}");
	public void UpdateLives(int lives)  => LivesLabel?.SetText($"Lives: {lives}");
	public void UpdateWave(int wave)    => WaveLabel?.SetText($"Wave: {wave}");
	public void UpdateSelectedTurret(string turretName) => SelectedTurretLabel?.SetText($"Selected: {turretName}");
}
