using Godot;

public class PlayerBuildingBuilder
{
	private readonly Player _player;
	private BuildingPreview _currentPreview;
	private bool _isInBuildMode = false;
	
	public bool IsInBuildMode => _isInBuildMode;

	public PlayerBuildingBuilder(Player player)
	{
		_player = player;
	}

	public void HandleInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouse && mouse.Pressed)
		{
			if (mouse.ButtonIndex == MouseButton.Left && _isInBuildMode)
			{
				// Left click to place building
				BuildBuilding();
				// Mark the event as handled to prevent it from reaching buildings
				_player.GetViewport().SetInputAsHandled();
			}
			else if (mouse.ButtonIndex == MouseButton.Right && _isInBuildMode)
			{
				// Right click to deselect current building
				CancelBuildMode();
				// Mark the event as handled
				_player.GetViewport().SetInputAsHandled();
			}
		}
		
		if (@event is InputEventKey key && key.Pressed && key.Keycode == Key.Escape && _isInBuildMode)
		{
			CancelBuildMode();
		}
	}

	public void StartBuildMode(PackedScene buildingScene)
	{
		if (buildingScene == null)
		{
			GD.PrintErr("❌ No building scene provided!");
			return;
		}
		
		// If already in build mode, update the existing preview
		if (_isInBuildMode && _currentPreview != null)
		{
			_currentPreview.UpdateBuildingScene(buildingScene);
			GD.Print($"🔄 Switched to {buildingScene.ResourcePath.GetFile().GetBaseName()} building");
			return;
		}
		
		_isInBuildMode = true;
		
		// Create preview
		_currentPreview = new BuildingPreview();
		_currentPreview.BuildingScene = buildingScene;
		_player.GetTree().Root.AddChild(_currentPreview);
		
		GD.Print("🔨 Entered building build mode - Left click to place, Right click or ESC to cancel");
	}

	public void CancelBuildMode()
	{
		if (!_isInBuildMode) return;
		
		_isInBuildMode = false;
		
		if (_currentPreview != null)
		{
			_currentPreview.QueueFree();
			_currentPreview = null;
		}
		
		GD.Print("❌ Cancelled building build mode");
	}

	private void BuildBuilding()
	{
		if (!_isInBuildMode || _currentPreview == null)
			return;
		
		if (!_currentPreview.CanPlaceBuilding())
		{
			GD.PrintErr("❌ Cannot place building at this location!");
			return;
		}

		// Use GameManager's money system
		if (GameManager.Instance == null)
		{
			GD.PrintErr("❌ GameManager not available!");
			return;
		}
		
		int cost = _currentPreview.GetBuildingCost();
		if (!GameManager.Instance.SpendMoney(cost))
		{
			GD.PrintErr($"❌ Not enough money! Need ${cost}, but have ${GameManager.Instance.Money}");
			return;
		}

		// Create the actual building
		var building = _currentPreview.BuildingScene.Instantiate<Building>();
		building.GlobalPosition = _currentPreview.GetPlacementPosition();
		_player.GetTree().Root.AddChild(building);

		// Register with BuildingManager
		var buildingManager = _player.GetTree().GetFirstNodeInGroup("building_manager") as BuildingManager;
		buildingManager?.AddBuilding(building);

		GD.Print($"🔧 Built building at {building.GlobalPosition} for ${cost}");
		
		// Stay in build mode to allow building more of the same type
	}

}
