using Godot;

public class PlayerTurretBuilder
{
	private readonly Player _player;
	private TurretPreview _currentPreview;
	private bool _isInBuildMode = false;

	public PlayerTurretBuilder(Player player)
	{
		_player = player;
	}

	public void HandleInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouse && mouse.Pressed)
		{
			if (mouse.ButtonIndex == MouseButton.Left && _isInBuildMode)
			{
				// Left click to place turret
				BuildTurret();
			}
			else if (mouse.ButtonIndex == MouseButton.Right && _isInBuildMode)
			{
				// Right click to cancel build mode
				CancelBuildMode();
			}
		}
		
		if (@event is InputEventKey key && key.Pressed && key.Keycode == Key.Escape && _isInBuildMode)
		{
			CancelBuildMode();
		}
	}

	public void StartBuildMode(PackedScene turretScene)
	{
		if (turretScene == null)
		{
			GD.PrintErr("❌ No turret scene provided!");
			return;
		}
		
		if (_isInBuildMode) return;
		
		_isInBuildMode = true;
		
		// Create preview
		_currentPreview = new TurretPreview();
		_currentPreview.TurretScene = turretScene;
		_player.GetTree().Root.AddChild(_currentPreview);
		
		GD.Print("🔨 Entered turret build mode - Left click to place, Right click or ESC to cancel");
	}
	
	private void CancelBuildMode()
	{
		if (!_isInBuildMode) return;
		
		_isInBuildMode = false;
		
		if (_currentPreview != null)
		{
			_currentPreview.QueueFree();
			_currentPreview = null;
		}
		
		GD.Print("❌ Cancelled turret build mode");
		
		// Clear the current turret selection in the player
		_player.ClearTurretSelection();
	}
	
	private void BuildTurret()
	{
		if (!_isInBuildMode || _currentPreview == null)
			return;
		
		if (!_currentPreview.CanPlaceTurret())
		{
			GD.PrintErr("❌ Cannot place turret at this location!");
			return;
		}

		// Use GameManager's money system
		if (GameManager.Instance == null)
		{
			GD.PrintErr("❌ GameManager not available!");
			return;
		}
		
		int cost = _currentPreview.GetTurretCost();
		if (!GameManager.Instance.SpendMoney(cost))
		{
			GD.PrintErr($"❌ Not enough money! Need ${cost}, but have ${GameManager.Instance.Money}");
			return;
		}

		// Create the actual turret
		var turret = _currentPreview.TurretScene.Instantiate<Turret>();
		turret.GlobalPosition = _currentPreview.GetPlacementPosition();
		_player.GetTree().Root.AddChild(turret);

		GD.Print($"🔧 Built turret at {turret.GlobalPosition} for ${cost}");
		
		// Exit build mode and clear selection
		CancelBuildMode();
	}
	
	public void UpdateTurretSelection()
	{
		// Update preview if in build mode
		if (_isInBuildMode && _currentPreview != null)
		{
			_currentPreview.UpdateTurretScene(_player.CurrentTurretScene);
		}
	}
}
