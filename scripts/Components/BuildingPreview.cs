using Godot;

public partial class BuildingPreview : Node2D
{
	[Export] public PackedScene BuildingScene;
	[Export] public Color ValidColor = new Color(0.2f, 0.8f, 0.2f, 0.6f); // Green for valid placement
	[Export] public Color InvalidColor = new Color(0.8f, 0.2f, 0.2f, 0.6f); // Red for invalid placement
	
	private Building _previewBuilding;
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
			_previewBuilding.InitializeStats(); // Configure stats for correct range
			AddChild(_previewBuilding);
			
			// Make the building semi-transparent for preview
			_previewBuilding.Modulate = new Color(1, 1, 1, 0.7f);
			_previewBuilding.ShowRange();
			
			// Disable collision and detection for preview building
			_previewBuilding.SetCollisionLayerValue(1, false);
			_previewBuilding.SetCollisionMaskValue(1, false);
			// Disable input for preview building so it doesn't interfere with placement
			_previewBuilding.InputPickable = false;
		}
	}

	public override void _Process(double delta)
	{
		// Follow mouse position
		_mousePosition = GetGlobalMousePosition();
		GlobalPosition = _mousePosition;
		
		// Handle flash timer
		if (_isFlashing)
		{
			_flashTimer -= (float)delta;
			if (_flashTimer <= 0.0f)
			{
				_isFlashing = false;
			}
		}
		
		// Check if placement is valid
		CheckPlacementValidity();
		
		// Update visual feedback
		UpdateVisualFeedback();
	}
	
	private void CheckPlacementValidity()
	{
		// Check if we're within screen bounds
		var viewport = GetViewport();
		bool withinBounds = true;
		if (viewport != null)
		{
			var screenSize = viewport.GetVisibleRect().Size;
			withinBounds = _mousePosition.X >= 50 && _mousePosition.X <= screenSize.X - 50 &&
			               _mousePosition.Y >= 50 && _mousePosition.Y <= screenSize.Y - 50;
		}
		
		// Check if the position is valid for building using BuildingZoneValidator
		bool validBuildingZone = BuildingZoneValidator.CanBuildAt(_mousePosition);
		
		// Check for overlaps with existing buildings
		bool noOverlapWithBuildings = !IsOverlappingWithBuildings(_mousePosition);
		
		_isValidPlacement = withinBounds && validBuildingZone && noOverlapWithBuildings;
	}
	
	private void UpdateVisualFeedback()
	{
		if (_previewBuilding != null)
		{
			// Handle flash effect for insufficient funds
			if (_isFlashing)
			{
				// Flash red with pulsing effect
				float flashIntensity = Mathf.Sin(_flashTimer * 20.0f) * 0.5f + 0.5f;
				var flashColor = new Color(1.0f, 0.3f, 0.3f, 0.8f + flashIntensity * 0.2f);
				_previewBuilding.Modulate = flashColor;
				_previewBuilding.SetRangeColor(new Color(1.0f, 0.0f, 0.0f, 0.8f + flashIntensity * 0.2f));
				return;
			}
			
			// Normal visual feedback based on placement validity
			var rangeColor = _isValidPlacement ? ValidColor : InvalidColor;
			_previewBuilding.SetRangeColor(rangeColor);
			
			// Update building transparency
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
		// Get BuildingManager to check for overlaps
		var buildingManager = GetTree().GetFirstNodeInGroup("building_manager") as BuildingManager;
		if (buildingManager == null)
		{
			return false; // If no BuildingManager, allow placement
		}
		
		// Check if position is occupied (16 pixel radius to prevent overlap)
		return buildingManager.IsPositionOccupied(position, 16.0f);
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
			_previewBuilding.InitializeStats(); // Configure stats for correct range
			AddChild(_previewBuilding);
			
			// Make the building semi-transparent for preview
			_previewBuilding.Modulate = new Color(1, 1, 1, 0.7f);
			_previewBuilding.ShowRange();
			
			// Disable collision and detection for preview building
			_previewBuilding.SetCollisionLayerValue(1, false);
			_previewBuilding.SetCollisionMaskValue(1, false);
			// Disable input for preview building so it doesn't interfere with placement
			_previewBuilding.InputPickable = false;
		}
	}
}
