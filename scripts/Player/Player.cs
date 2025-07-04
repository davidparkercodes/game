using Godot;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 200f;

	[Export] public PackedScene BasicTurretScene;
	[Export] public PackedScene SniperTurretScene;

	public PackedScene CurrentTurretScene { get; private set; } = null; // Start with no turret selected

	private PlayerMovement _movement;
	private PlayerTurretBuilder _turretBuilder;

	public override void _Ready()
	{
		// Start with no turret selected
		CurrentTurretScene = null;
		UpdateSelectedTurretDisplay("None");

		_movement = new PlayerMovement(this);
		_turretBuilder = new PlayerTurretBuilder(this);
	}

	public override void _PhysicsProcess(double delta)
	{
		_movement.Update(delta);
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
						// Start building basic turret immediately
						CurrentTurretScene = BasicTurretScene;
						UpdateSelectedTurretDisplay("Basic");
						_turretBuilder.StartBuildMode(BasicTurretScene);
						GD.Print("📦 Selected Basic Turret for building");
					}
					else
					{
						GD.PrintErr("❌ No BasicTurretScene assigned!");
					}
					break;

				case Key.Key2:
					if (SniperTurretScene != null)
					{
						// Start building sniper turret immediately
						CurrentTurretScene = SniperTurretScene;
						UpdateSelectedTurretDisplay("Sniper");
						_turretBuilder.StartBuildMode(SniperTurretScene);
						GD.Print("🎯 Selected Sniper Turret for building");
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
				CurrentTurretScene = BasicTurretScene;
				UpdateSelectedTurretDisplay("Basic");
				_turretBuilder.StartBuildMode(BasicTurretScene);
				break;
			case "Sniper":
				CurrentTurretScene = SniperTurretScene;
				UpdateSelectedTurretDisplay("Sniper");
				_turretBuilder.StartBuildMode(SniperTurretScene);
				break;
		}
	}
	
	public void ClearTurretSelection()
	{
		CurrentTurretScene = null;
		UpdateSelectedTurretDisplay("None");
		GD.Print("🚫 Cleared turret selection");
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


	private void DelayedUpdateSelectedTurret(string turretName)
	{
		if (GameManager.Instance?.Hud != null)
		{
			GameManager.Instance.Hud.UpdateSelectedTurret(turretName);
		}
	}
}
