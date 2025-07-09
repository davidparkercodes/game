using Godot;
using Game.Presentation.UI;

namespace Game.Presentation.Player;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 200f;

	[Export] public PackedScene? BasicTowerScene;
	[Export] public PackedScene? SniperTowerScene;
	[Export] public PackedScene? RapidTowerScene;
	[Export] public PackedScene? HeavyTowerScene;

	public PackedScene? CurrentBuildingScene { get; set; } = null;

	private PlayerMovement _movement = null!;
	internal PlayerBuildingBuilder _buildingBuilder = null!;
	private PlayerBuildingSelection _buildingSelection = null!;
	internal PlayerHudConnector _hudConnector = null!;

	public override void _Ready()
	{
		AddToGroup("player");
		
		if (Speed <= 0)
			Speed = 200f;
		
		InitializeComponents();
		CallDeferred(nameof(InitializeHudConnections));
	}
	
	private void InitializeComponents()
	{
		CurrentBuildingScene = null;
		_movement = new PlayerMovement(this);
		_buildingBuilder = new PlayerBuildingBuilder(this);
		_buildingSelection = new PlayerBuildingSelection(this);
		_hudConnector = new PlayerHudConnector(this);
	}

	private void InitializeHudConnections()
	{
		_hudConnector.InitializeHudConnections();
	}
	

	public override void _PhysicsProcess(double delta)
	{
		_movement.Update(delta);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		// Handle input priority between tower selection and building placement
		HandleInputPriority(@event);

		if (@event is InputEventKey key && key.Pressed)
		{
			// Only process building selection if tower upgrade HUD is not open
			if (!IsTowerUpgradeHudOpen())
			{
				_buildingSelection.HandleBuildingSelection(key.Keycode);
			}
		}
	}
	
	public void SelectBuilding(string buildingId)
	{
		_buildingSelection.SelectBuildingByName(buildingId);
	}
	
	public void ClearBuildingSelection()
	{
		CurrentBuildingScene = null!;
		UpdateSelectedBuildingDisplay("None");
		_hudConnector.HideBuildingStats();
		_buildingBuilder.CancelBuildMode();
		_hudConnector.NotifyHudSelectionChange(null);
		GD.Print("🚫 Cleared building selection");
	}
	
	public void CancelBuildMode()
	{
		_buildingBuilder?.CancelBuildMode();
		CurrentBuildingScene = null;
		_hudConnector.HideBuildingStats();
		_hudConnector.NotifyHudSelectionChange(null);
		GD.Print("🔧 Build mode cancelled");
	}
	
	public void ClearPlayerSelectionState()
	{
		// Internal method called by PlayerBuildingBuilder - don't notify HUD to avoid circular calls
		CurrentBuildingScene = null;
		_hudConnector.HideBuildingStats();
	}

	internal void UpdateSelectedBuildingDisplay(string buildingName)
	{
		UpdateBuildingStats(buildingName);
	}
	
	private void UpdateBuildingStats(string buildingName)
	{
		if (buildingName == "None")
		{
			_hudConnector.HideBuildingStats();
			return;
		}
		
		var stats = _hudConnector.GetBuildingStats(buildingName);
		if (stats != null)
		{
			_hudConnector.ShowBuildingStats(buildingName, stats.Cost, stats.Damage, stats.Range, stats.AttackSpeed);
		}
	}
	
	public bool IsBuildingPlacementActive()
	{
		return CurrentBuildingScene != null;
	}
	
	public bool IsTowerUpgradeHudOpen()
	{
		return BuildingSelectionManager.Instance.IsTowerUpgradeHudOpen();
	}
	
	public void DisableBuildingPlacementForTowerSelection()
	{
		if (IsBuildingPlacementActive())
		{
			GD.Print("🔧 Player: Disabling building placement for tower selection");
			ClearBuildingSelection();
		}
	}
	
	public void ClearBuildingSelectionForTowerSelection()
	{
		if (IsBuildingPlacementActive())
		{
			GD.Print("🏗️ Player: Clearing building selection for tower selection");
			ClearBuildingSelection();
		}
	}
	
	public bool ShouldBlockBuildingPlacement()
	{
		// Block building placement when tower upgrade HUD is open
		return IsTowerUpgradeHudOpen();
	}
	
	public void HandleInputPriority(InputEvent inputEvent)
	{
		// Handle input priority between tower selection and building placement
		if (IsTowerUpgradeHudOpen())
		{
			// Tower upgrade HUD has priority - don't process building placement input
			return;
		}
		
		// Continue with normal input processing
		_buildingBuilder.HandleInput(inputEvent);
	}
	
	public void UpdateBuildingStatsDisplay()
	{
		// Update building stats display logic for current selection
		if (CurrentBuildingScene != null)
		{
			string buildingName = GetBuildingNameFromScene(CurrentBuildingScene);
			UpdateBuildingStats(buildingName);
		}
		else
		{
			_hudConnector.HideBuildingStats();
		}
	}
	
	private string GetBuildingNameFromScene(PackedScene scene)
	{
		// Map PackedScene to building name
		if (scene == BasicTowerScene) return "basic_tower";
		if (scene == SniperTowerScene) return "sniper_tower";
		if (scene == RapidTowerScene) return "rapid_tower";
		if (scene == HeavyTowerScene) return "heavy_tower";
		return "unknown";
	}
	
}
