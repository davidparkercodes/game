using Godot;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 200f;
	[Export] public int AttackDamage = 10;

	[Export] public PackedScene BasicTurretScene;
	[Export] public PackedScene SniperTurretScene;

	public PackedScene CurrentTurretScene { get; private set; }

	private PlayerMovement _movement;
	private PlayerCombat _combat;
	private PlayerTurretBuilder _turretBuilder;

	public override void _Ready()
	{
		if (BasicTurretScene != null)
		{
			CurrentTurretScene = BasicTurretScene;
			UpdateSelectedTurretDisplay("Basic");
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
						UpdateSelectedTurretDisplay("Basic");
						_turretBuilder.UpdateTurretSelection();
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
						UpdateSelectedTurretDisplay("Sniper");
						_turretBuilder.UpdateTurretSelection();
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
				UpdateSelectedTurretDisplay("Basic");
				_turretBuilder.UpdateTurretSelection();
				break;
			case "Sniper":
				CurrentTurretScene = SniperTurretScene;  // same here
				UpdateSelectedTurretDisplay("Sniper");
				_turretBuilder.UpdateTurretSelection();
				break;
		}
	}

	private void UpdateSelectedTurretDisplay(string turretName)
	{
		if (GameManager.Instance?.Hud != null)
		{
			GameManager.Instance.Hud.UpdateSelectedTurret(turretName);
		}
		else
		{
			// Defer the call until GameManager is ready
			CallDeferred(nameof(DelayedUpdateSelectedTurret), turretName);
		}
	}

	private void TriggerHitbox()
	{
		var hitbox = GetNode<Area2D>("Hitbox");
		if (hitbox != null)
		{
			hitbox.Call("ResetHits");
			var collisionShape = hitbox.GetNode<CollisionShape2D>("CollisionShape2D");
			if (collisionShape != null)
			{
				collisionShape.Disabled = false;
				// Use a timer to disable it after a brief moment
				GetTree().CreateTimer(0.1).Timeout += () => collisionShape.Disabled = true;
			}
		}
	}

	private void DelayedUpdateSelectedTurret(string turretName)
	{
		if (GameManager.Instance?.Hud != null)
		{
			GameManager.Instance.Hud.UpdateSelectedTurret(turretName);
		}
	}
}
