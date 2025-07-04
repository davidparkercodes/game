// scripts/Buildings/BasicTurret.cs
using Godot;

public partial class BasicTurret : Building
{
	public override void _Ready()
	{
		BuildingType = "basic_turret";
		base._Ready();
	}
}
