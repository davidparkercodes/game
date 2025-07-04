using Godot;

public partial class TurretPreview : Node2D
{
	[Export] public PackedScene TurretScene;
	[Export] public Color ValidColor = new Color(0.2f, 0.8f, 0.2f, 0.6f); // Green for valid placement
	[Export] public Color InvalidColor = new Color(0.8f, 0.2f, 0.2f, 0.6f); // Red for invalid placement
	
	private Turret _previewTurret;
	private bool _isValidPlacement = true;
	private Vector2 _mousePosition;
	
	public override void _Ready()
	{
		if (TurretScene != null)
		{
			_previewTurret = TurretScene.Instantiate<Turret>();
			_previewTurret.InitializeStats(); // Configure stats for correct range
			AddChild(_previewTurret);
			
			// Make the turret semi-transparent for preview
			_previewTurret.Modulate = new Color(1, 1, 1, 0.7f);
			_previewTurret.ShowRange();
			
			// Disable collision and detection for preview turret
			_previewTurret.SetCollisionLayerValue(1, false);
			_previewTurret.SetCollisionMaskValue(1, false);
			// Disable input for preview turret so it doesn't interfere with placement
			_previewTurret.InputPickable = false;
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
		// For now, simple validity check - can be expanded later
		// Check if we're within screen bounds or other placement rules
		var viewport = GetViewport();
		if (viewport != null)
		{
			var screenSize = viewport.GetVisibleRect().Size;
			_isValidPlacement = _mousePosition.X >= 50 && _mousePosition.X <= screenSize.X - 50 &&
			                   _mousePosition.Y >= 50 && _mousePosition.Y <= screenSize.Y - 50;
		}
		
		// Could add more checks here:
		// - Check for collisions with other turrets
		// - Check for valid terrain
		// - Check minimum distance from path
		// etc.
	}
	
	private void UpdateVisualFeedback()
	{
		if (_previewTurret != null)
		{
			// Update range color based on validity
			var rangeColor = _isValidPlacement ? ValidColor : InvalidColor;
			_previewTurret.SetRangeColor(rangeColor);
			
			// Update turret transparency
			var turretColor = _isValidPlacement ? 
				new Color(1, 1, 1, 0.8f) : 
				new Color(1, 0.5f, 0.5f, 0.8f);
			_previewTurret.Modulate = turretColor;
		}
	}
	
	public bool CanPlaceTurret()
	{
		return _isValidPlacement;
	}
	
	public Vector2 GetPlacementPosition()
	{
		return _mousePosition;
	}
	
	public int GetTurretCost()
	{
		return _previewTurret?.Cost ?? 0;
	}
	
	public void UpdateTurretScene(PackedScene newTurretScene)
	{
		if (_previewTurret != null)
		{
			_previewTurret.QueueFree();
		}
		
		TurretScene = newTurretScene;
		if (TurretScene != null)
		{
			_previewTurret = TurretScene.Instantiate<Turret>();
			_previewTurret.InitializeStats();
			AddChild(_previewTurret);
			_previewTurret.Modulate = new Color(1, 1, 1, 0.7f);
			_previewTurret.ShowRange();
		}
	}
}
