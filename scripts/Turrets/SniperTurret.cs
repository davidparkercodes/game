using Godot;

public partial class SniperTurret : Turret
{
	protected override void ConfigureStats()
	{
		Cost     = 20;
		Damage   = 100;
		FireRate = 2.5f;
		Range    = 200f;
	}
}
