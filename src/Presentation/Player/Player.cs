using Godot;
using Game.Presentation.Buildings;
using Game.Presentation.UI;

namespace Game.Presentation.Player;

public class PlayerBuildingStats
{
	public int Cost { get; set; }
	public int Damage { get; set; }
	public float Range { get; set; }
	public float FireRate { get; set; }
}

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 200f;

	[Export] public PackedScene? BasicTowerScene;
	[Export] public PackedScene? SniperTowerScene;

	public PackedScene? CurrentBuildingScene { get; private set; } = null;

	private PlayerMovement _movement = null!;
	private PlayerBuildingBuilder _buildingBuilder = null!;

	public override void _Ready()
	{
		AddToGroup("player");
		
		if (Speed <= 0)
			Speed = 200f;
		
		CurrentBuildingScene = null!;
		// Don't update display immediately - HudManager might not be ready
		// UpdateSelectedBuildingDisplay("None");

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
					if (BasicTowerScene != null)
					{
						GD.Print($"🔍 Key1 pressed. CurrentBuildingScene: {(CurrentBuildingScene == null ? "null" : CurrentBuildingScene.ResourcePath.GetFile().GetBaseName())}");
						GD.Print($"🔍 BasicTowerScene: {BasicTowerScene.ResourcePath.GetFile().GetBaseName()}");
						GD.Print($"🔍 Are they equal? {CurrentBuildingScene == BasicTowerScene}");
						
						if (CurrentBuildingScene == BasicTowerScene)
						{
							ClearBuildingSelection();
							GD.Print("🚫 Deselected Basic Tower");
						}
						else
						{
							CurrentBuildingScene = BasicTowerScene;
							UpdateSelectedBuildingDisplay("Basic");
							_buildingBuilder.StartBuildMode(BasicTowerScene);
							GD.Print("📦 Selected Basic Tower for building");
						}
					}
					else
					{
						GD.PrintErr("❌ No BasicTowerScene assigned!");
					}
					break;

				case Key.Key2:
					if (SniperTowerScene != null)
					{
						if (CurrentBuildingScene == SniperTowerScene)
						{
							ClearBuildingSelection();
							GD.Print("🚫 Deselected Sniper Tower");
						}
						else
						{
							CurrentBuildingScene = SniperTowerScene;
							UpdateSelectedBuildingDisplay("Sniper");
							_buildingBuilder.StartBuildMode(SniperTowerScene);
							GD.Print("🎯 Selected Sniper Tower for building");
						}
					}
					else
					{
						GD.PrintErr("❌ No SniperTowerScene assigned!");
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
				CurrentBuildingScene = BasicTowerScene;
				UpdateSelectedBuildingDisplay("Basic");
				_buildingBuilder.StartBuildMode(BasicTowerScene!);
				break;
			case "Sniper":
				CurrentBuildingScene = SniperTowerScene;
				UpdateSelectedBuildingDisplay("Sniper");
				_buildingBuilder.StartBuildMode(SniperTowerScene!);
				break;
		}
	}
	
	public void ClearBuildingSelection()
	{
		CurrentBuildingScene = null!;
		UpdateSelectedBuildingDisplay("None");
		HideBuildingStats();
		_buildingBuilder.CancelBuildMode();
		GD.Print("🚫 Cleared building selection");
	}
	
	public void CancelBuildMode()
	{
		GD.Print($"🔧 CancelBuildMode called. CurrentBuildingScene before: {(CurrentBuildingScene == null ? "null" : CurrentBuildingScene.ResourcePath.GetFile().GetBaseName())}");
		
		_buildingBuilder?.CancelBuildMode();
		
		CurrentBuildingScene = null!;
		HideBuildingStats();
		
		GD.Print($"🔧 CancelBuildMode finished. CurrentBuildingScene after: {(CurrentBuildingScene == null ? "null" : "NOT NULL - ERROR!")}");
	}
	
	public void ClearPlayerSelectionState()
	{
		GD.Print($"🧹 ClearPlayerSelectionState called. CurrentBuildingScene before: {(CurrentBuildingScene == null ? "null" : CurrentBuildingScene.ResourcePath.GetFile().GetBaseName())}");
		
		CurrentBuildingScene = null!;
		HideBuildingStats();
		
		GD.Print($"🧹 ClearPlayerSelectionState finished. CurrentBuildingScene after: {(CurrentBuildingScene == null ? "null" : "NOT NULL - ERROR!")}");
	}

	private void UpdateSelectedBuildingDisplay(string buildingName)
	{
		UpdateBuildingStats(buildingName);
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
	
	private PlayerBuildingStats? GetBuildingStats(string buildingName)
	{
		PackedScene? buildingScene = buildingName switch
		{
			"Basic" => BasicTowerScene,
			"Sniper" => SniperTowerScene,
			_ => null
		};
		
		if (buildingScene == null) return null;
		
		var tempBuilding = buildingScene.Instantiate<Game.Presentation.Buildings.Building>();
		tempBuilding.InitializeStats();
		
		var stats = new PlayerBuildingStats
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
		if (HudManager.Instance != null && HudManager.Instance.IsInitialized())
		{
			HudManager.Instance.ShowBuildingStats(buildingName, cost, damage, range, fireRate);
			GD.Print($"🏗️ Building Stats: {buildingName} - Cost: ${cost}, Damage: {damage}, Range: {range:F1}, FireRate: {fireRate:F1}s");
		}
		else
		{
			GD.PrintErr("❌ Cannot show building stats - HudManager not available");
		}
	}

	private void HideBuildingStats()
	{
		if (HudManager.Instance != null && HudManager.Instance.IsInitialized())
		{
			HudManager.Instance.HideBuildingStats();
			GD.Print("🚫 Hiding building stats");
		}
		else
		{
			GD.PrintErr("❌ Cannot hide building stats - HudManager not available");
		}
	}
}
