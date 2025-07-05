using Godot;

namespace Game.Presentation.Buildings;

public partial class SniperTurret : Building
{
	public override void _Ready()
	{
		Cost = 25;
		Damage = 30;
		Range = 250.0f;
		FireRate = 0.5f;
		
		base._Ready();
	}
}
