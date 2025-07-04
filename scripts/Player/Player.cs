using Godot;

public class BuildingStats
{
	public int Cost { get; set; }
	public int Damage { get; set; }
	public float Range { get; set; }
	public float FireRate { get; set; }
}

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 200f;

	[Export] public PackedScene BasicTurretScene;
	[Export] public PackedScene SniperTurretScene;

	public PackedScene CurrentBuildingScene { get; private set; } = null; // Start with no building selected

	private PlayerMovement _movement;
	private PlayerBuildingBuilder _buildingBuilder;

	public override void _Ready()
	{
		// Add to player group for easy reference
		AddToGroup("player");
		
		// Start with no building selected
		CurrentBuildingScene = null;
		UpdateSelectedBuildingDisplay("None");

		_movement = new PlayerMovement(this);
		_buildingBuilder = new PlayerBuildingBuilder(this);
	}

	public override void _PhysicsProcess(double delta)
	{
		_movement.Update(delta);
	}


	public override void _UnhandledInput(InputEvent @event)
	{
		_buildingBuilder.HandleInput(@event);

		if (@event is InputEventKey key && key.Pressed)
		{
			switch (key.Keycode)
			{
				case Key.Key1:
					if (BasicTurretScene != null)
					{
						// Start building basic turret immediately
						CurrentBuildingScene = BasicTurretScene;
						UpdateSelectedBuildingDisplay("Basic");
						_buildingBuilder.StartBuildMode(BasicTurretScene);
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
						CurrentBuildingScene = SniperTurretScene;
						UpdateSelectedBuildingDisplay("Sniper");
						_buildingBuilder.StartBuildMode(SniperTurretScene);
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
	
	public void SelectBuilding(string buildingId)
	{
		switch (buildingId)
		{
			case "Basic":
				CurrentBuildingScene = BasicTurretScene;
				UpdateSelectedBuildingDisplay("Basic");
				_buildingBuilder.StartBuildMode(BasicTurretScene);
				break;
			case "Sniper":
				CurrentBuildingScene = SniperTurretScene;
				UpdateSelectedBuildingDisplay("Sniper");
				_buildingBuilder.StartBuildMode(SniperTurretScene);
				break;
		}
	}
	
	public void ClearBuildingSelection()
	{
		CurrentBuildingScene = null;
		UpdateSelectedBuildingDisplay("None");
		HideBuildingStats();
		GD.Print("🚫 Cleared building selection");
	}
	
	public void CancelBuildMode()
	{
		// Cancel any active build mode through the building builder
		_buildingBuilder?.CancelBuildMode();
	}

	private void UpdateSelectedBuildingDisplay(string buildingName)
	{
		if (GameManager.Instance?.Hud != null)
		{
			GameManager.Instance.Hud.UpdateSelectedBuilding(buildingName);
			UpdateBuildingStats(buildingName);
		}
		else
		{
			// Defer the call until GameManager is ready
			CallDeferred(nameof(DelayedUpdateSelectedBuilding), buildingName);
		}
	}
	
	private void UpdateBuildingStats(string buildingName)
	{
		if (buildingName == "None")
		{
			HideBuildingStats();
			return;
		}
		
		var stats = GetBuildingStats(buildingName);
		if (stats != null)
		{
			ShowBuildingStats(buildingName, stats.Cost, stats.Damage, stats.Range, stats.FireRate);
		}
	}
	
	private BuildingStats GetBuildingStats(string buildingName)
	{
		PackedScene buildingScene = buildingName switch
		{
			"Basic" => BasicTurretScene,
			"Sniper" => SniperTurretScene,
			_ => null
		};
		
		if (buildingScene == null) return null;
		
		// Create a temporary instance to get stats
		var tempBuilding = buildingScene.Instantiate<Building>();
		tempBuilding.InitializeStats(); // Configure stats before accessing them
		
		var stats = new BuildingStats
		{
			Cost = tempBuilding.Cost,
			Damage = tempBuilding.Damage,
			Range = tempBuilding.Range,
			FireRate = tempBuilding.FireRate
		};
		
		tempBuilding.QueueFree();
		return stats;
	}
	
	private void ShowBuildingStats(string buildingName, int cost, int damage, float range, float fireRate)
	{
		if (GameManager.Instance?.Hud != null)
		{
			GameManager.Instance.Hud.ShowBuildingStats(buildingName, cost, damage, range, fireRate);
		}
	}
	
	private void HideBuildingStats()
	{
		if (GameManager.Instance?.Hud != null)
		{
			GameManager.Instance.Hud.HideBuildingStats();
		}
	}


	private void DelayedUpdateSelectedBuilding(string buildingName)
	{
		if (GameManager.Instance?.Hud != null)
		{
			GameManager.Instance.Hud.UpdateSelectedBuilding(buildingName);
			UpdateBuildingStats(buildingName);
		}
	}
}
