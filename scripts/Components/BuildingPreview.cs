using Godot;

public partial class BuildingPreview : Node2D
{
	[Export] public PackedScene BuildingScene;
	[Export] public Color ValidColor = new Color(0.2f, 0.8f, 0.2f, 0.6f); // Green for valid placement
	[Export] public Color InvalidColor = new Color(0.8f, 0.2f, 0.2f, 0.6f); // Red for invalid placement
	
	private Building _previewBuilding;
	private bool _isValidPlacement = true;
	private Vector2 _mousePosition;
	
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
		
		_isValidPlacement = withinBounds && validBuildingZone;
		
		// Could add more checks here:
		// - Check for collisions with other buildings
		// - Check minimum distance requirements
		// etc.
	}
	
	private void UpdateVisualFeedback()
	{
		if (_previewBuilding != null)
		{
			// Update range color based on validity
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
