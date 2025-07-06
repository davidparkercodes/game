using Godot;
using Game.Presentation.Buildings;
using Game.Presentation.Systems;
using Game.Infrastructure.Game.Services;
using Game.Infrastructure.Audio.Services;
using Game.Domain.Audio.Enums;
using PresentationValidator = Game.Presentation.Systems.BuildingZoneValidator;

namespace Game.Presentation.Player;

public class PlayerBuildingBuilder
{
	private readonly Player _player;
	private BuildingPreview? _currentPreview;
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
				BuildBuilding();
				_player.GetViewport().SetInputAsHandled();
			}
			else if (mouse.ButtonIndex == MouseButton.Right && _isInBuildMode)
			{
				CancelBuildMode();
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
		
		if (_isInBuildMode && _currentPreview != null)
		{
			_currentPreview.UpdateBuildingScene(buildingScene);
			GD.Print($"🔄 Switched to {buildingScene.ResourcePath.GetFile().GetBaseName()} building");
			return;
		}
		
		_isInBuildMode = true;
		
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
			_currentPreview = null!;
		}
		
		_player.ClearPlayerSelectionState();
		
		GD.Print("❌ Cancelled building build mode");
	}

	private void BuildBuilding()
	{
		if (!_isInBuildMode || _currentPreview == null)
			return;
		
	Vector2 buildPosition = _currentPreview.GetPlacementPosition();
	if (!PresentationValidator.CanBuildAtWithLogging(buildPosition))
	{
		return;
	}
		
		// Additional collision check using BuildingRegistry (already checked in preview, but double-check here)
		float collisionRadius = _currentPreview.GetBuildingCost() > 100 ? 40.0f : BuildingRegistry.DefaultCollisionRadius; // Larger buildings need more space
		if (BuildingRegistry.Instance.IsPositionOccupied(buildPosition, collisionRadius))
		{
			var existingBuilding = BuildingRegistry.Instance.GetBuildingAt(buildPosition, collisionRadius);
			GD.Print($"🏭 Cannot place building - overlapping with existing {existingBuilding?.Name ?? "building"} at {buildPosition}");
			return;
		}
		
		if (!_currentPreview.CanPlaceBuilding())
		{
			GD.Print("⚠️ Cannot place building at this location!");
			return;
		}

		if (GameService.Instance == null)
		{
			GD.PrintErr("❌ GameService not available!");
			return;
		}
		
		int cost = _currentPreview.GetBuildingCost();
		if (!GameService.Instance.SpendMoney(cost))
		{
			GD.Print($"💰 Not enough money! Need ${cost}, but have ${GameService.Instance.Money}");
			_currentPreview.FlashRed();
			return;
		}

	var building = _currentPreview.BuildingScene!.Instantiate<Building>();
		building.GlobalPosition = _currentPreview.GetPlacementPosition();
		building.SetPreviewMode(false); // Ensure the placed building is fully active
		_player.GetTree().Root.AddChild(building);

		// Play construction sound
		PlayConstructionSound(building);

		// Building is automatically registered with BuildingRegistry in Building._Ready()

		GD.Print($"🔧 Built building at {building.GlobalPosition} for ${cost}");
	}
	
	private void PlayConstructionSound(Building building)
	{
		if (SoundManagerService.Instance == null)
		{
			GD.PrintErr("⚠️ SoundManagerService not available for construction sound");
			return;
		}
		
		// Determine sound key based on building type
		string soundKey;
		if (building.Name == "BasicTower")
		{
			soundKey = "basic_tower_build";
		}
		else if (building.Name == "SniperTower")
		{
			soundKey = "sniper_tower_build";
		}
		else
		{
			soundKey = "basic_tower_build"; // fallback to basic tower sound
		}
		
		GD.Print($"🔨 Playing construction sound: {soundKey} for building {building.Name}");
		SoundManagerService.Instance.PlaySound(soundKey, SoundCategory.SFX);
	}
}
