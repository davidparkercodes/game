// scripts/Buildings/SniperTurret.cs
using Godot;

public partial class SniperTurret : Building
{
	public override void _Ready()
	{
		BuildingType = "sniper_turret";
		base._Ready();
	}
}
