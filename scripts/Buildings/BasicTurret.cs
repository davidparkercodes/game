// scripts/Buildings/BasicTurret.cs
using Godot;

public partial class BasicTurret : Building
{
	protected override void ConfigureStats()
	{
		Cost = 15;
		Damage = 10;
		Range = 120.0f;
		FireRate = 1.2f;
	}
}
