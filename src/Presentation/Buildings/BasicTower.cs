using Godot;

namespace Game.Presentation.Buildings;

public partial class BasicTower : Building
{
	public override void _Ready()
	{
		Cost = 10;
		Damage = 10;
		Range = 150.0f;
		FireRate = 1.0f;
		
		base._Ready();
	}
}
