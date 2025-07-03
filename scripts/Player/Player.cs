using Godot;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 200f;
	[Export] public int AttackDamage = 10;

	[Export] public PackedScene BasicTurretScene;
	[Export] public PackedScene SniperTurretScene;

	public PackedScene CurrentTurretScene { get; private set; }
	
	public int Money { get; set; } = 50;

	private PlayerMovement _movement;
	private PlayerCombat _combat;
	private PlayerTurretBuilder _turretBuilder;

	public override void _Ready()
	{
		if (BasicTurretScene != null)
		{
			CurrentTurretScene = BasicTurretScene;
		}
		else
		{
			GD.PrintErr("❌ BasicTurretScene not assigned!");
		}

		_movement = new PlayerMovement(this);
		_combat = new PlayerCombat(this);
		_turretBuilder = new PlayerTurretBuilder(this);
	}

	public override void _PhysicsProcess(double delta)
	{
		_movement.Update(delta);
	}

	public override void _Process(double delta)
	{
		_combat.Update(delta);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		_turretBuilder.HandleInput(@event);

		if (@event is InputEventKey key && key.Pressed)
		{
			switch (key.Keycode)
			{
				case Key.Key1:
					if (BasicTurretScene != null)
					{
						CurrentTurretScene = BasicTurretScene;
						GD.Print("📦 Switched to Basic Turret");
					}
					else
					{
						GD.PrintErr("❌ No BasicTurretScene assigned!");
					}
					break;

				case Key.Key2:
					if (SniperTurretScene != null)
					{
						CurrentTurretScene = SniperTurretScene;
						GD.Print("🎯 Switched to Sniper Turret");
					}
					else
					{
						GD.PrintErr("❌ No SniperTurretScene assigned!");
					}
					break;
			}
		}
	}
	
	public void SelectTurret(string turretId)
	{
		switch (turretId)
		{
			case "Basic":
				CurrentTurretScene = BasicTurretScene;   // make sure this PackedScene field exists/exported
				break;
			case "Sniper":
				CurrentTurretScene = SniperTurretScene;  // same here
				break;
		}
	}
}
