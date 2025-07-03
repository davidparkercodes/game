using Godot;

public partial class BasicTurret : Turret
{
	protected override void ConfigureStats()
	{
		Cost     = 10;
		Damage   = 50;
		FireRate = 1.0f;
		Range    = 75f;
	}
}
