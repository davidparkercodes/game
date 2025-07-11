using Godot;
using Game.Presentation.Systems;

namespace Game.Presentation.Buildings;

public partial class BuildingPreview : Node2D
{
	[Export] public PackedScene? BuildingScene;
	[Export] public Color ValidColor = new Color(0.2f, 0.8f, 0.2f, 0.6f);
	[Export] public Color InvalidColor = new Color(0.8f, 0.2f, 0.2f, 0.6f);
	
	private Building? _previewBuilding;
	private bool _isValidPlacement = true;
	private Vector2 _mousePosition;
	private bool _isFlashing = false;
	private float _flashTimer = 0.0f;
	private const float FlashDuration = 0.5f;
	
	public override void _Ready()
	{
		if (BuildingScene != null)
		{
			_previewBuilding = BuildingScene.Instantiate<Building>();
			_previewBuilding.InitializeStatsFromConfig();
			_previewBuilding.SetPreviewMode(true); // Properly disable all functionality
			AddChild(_previewBuilding);
			
			_previewBuilding.Modulate = new Color(1, 1, 1, 0.7f);
			_previewBuilding.ShowRange();
			_previewBuilding.InputPickable = false;
		}
	}

	public override void _Process(double delta)
	{
		_mousePosition = GetGlobalMousePosition();
		GlobalPosition = _mousePosition;
		
		if (_isFlashing)
		{
			_flashTimer -= (float)delta;
			if (_flashTimer <= 0.0f)
			{
				_isFlashing = false;
			}
		}
		
		CheckPlacementValidity();
		UpdateVisualFeedback();
	}
	
	private void CheckPlacementValidity()
	{
		// Use world coordinates for validation instead of viewport bounds
		// The BuildingZoneValidator handles map bounds internally
		bool validBuildingZone = BuildingZoneValidator.CanBuildAt(_mousePosition);
		bool noOverlapWithBuildings = !IsOverlappingWithBuildings(_mousePosition);
		
		_isValidPlacement = validBuildingZone && noOverlapWithBuildings;
	}
	
	private void UpdateVisualFeedback()
	{
		if (_previewBuilding != null)
		{
			if (_isFlashing)
			{
				float flashIntensity = Mathf.Sin(_flashTimer * 20.0f) * 0.5f + 0.5f;
				var flashColor = new Color(1.0f, 0.3f, 0.3f, 0.8f + flashIntensity * 0.2f);
				_previewBuilding.Modulate = flashColor;
				_previewBuilding.SetRangeColor(new Color(1.0f, 0.0f, 0.0f, 0.8f + flashIntensity * 0.2f));
				return;
			}
			
			var rangeColor = _isValidPlacement ? ValidColor : InvalidColor;
			_previewBuilding.SetRangeColor(rangeColor);
			
			var buildingColor = _isValidPlacement ? 
				new Color(1, 1, 1, 0.8f) : 
				new Color(1, 0.5f, 0.5f, 0.8f);
			_previewBuilding.Modulate = buildingColor;
		}
	}
	
	public bool CanPlaceBuilding()
	{
		return _isValidPlacement;
	}
	
	public Vector2 GetPlacementPosition()
	{
		return _mousePosition;
	}
	
	public int GetBuildingCost()
	{
		return _previewBuilding?.Cost ?? 0;
	}
	
	public void FlashRed()
	{
		_isFlashing = true;
		_flashTimer = FlashDuration;
		GD.Print("💰 Insufficient funds - flashing red indicator");
	}
	
    private bool IsOverlappingWithBuildings(Vector2 position)
    {
        // Use BuildingRegistry to check for collisions with existing buildings
        float collisionRadius = _previewBuilding?.CollisionRadius ?? BuildingRegistry.DefaultCollisionRadius;
        
        bool isOccupied = BuildingRegistry.Instance.IsPositionOccupied(position, collisionRadius);
        
        // Only log on significant changes to avoid spam
        // The visual feedback is enough for the player
        
        return isOccupied;
    }
	
	public void UpdateBuildingScene(PackedScene newBuildingScene)
	{
		if (_previewBuilding != null)
		{
			_previewBuilding.QueueFree();
		}
		
		BuildingScene = newBuildingScene;
		if (BuildingScene != null)
		{
			_previewBuilding = BuildingScene.Instantiate<Building>();
			_previewBuilding.InitializeStatsFromConfig();
			_previewBuilding.SetPreviewMode(true); // Properly disable all functionality
			AddChild(_previewBuilding);
			
			_previewBuilding.Modulate = new Color(1, 1, 1, 0.7f);
			_previewBuilding.ShowRange();
			_previewBuilding.InputPickable = false;
		}
	}
}
