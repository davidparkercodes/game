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
			if (mouse.ButtonIndex == MouseButton.Right)
			{
				if (_isInBuildMode)
				{
					BuildTurret();
				}
				else
				{
					StartBuildMode();
				}
			}
			else if (mouse.ButtonIndex == MouseButton.Left && _isInBuildMode)
			{
				CancelBuildMode();
			}
		}
		
		if (@event is InputEventKey key && key.Pressed && key.Keycode == Key.Escape && _isInBuildMode)
		{
			CancelBuildMode();
		}
	}

	private void StartBuildMode()
	{
		if (_player.CurrentTurretScene == null)
		{
			GD.PrintErr("❌ No turret scene selected!");
			return;
		}
		
		if (_isInBuildMode) return;
		
		_isInBuildMode = true;
		
		// Create preview
		_currentPreview = new TurretPreview();
		_currentPreview.TurretScene = _player.CurrentTurretScene;
		_player.GetTree().Root.AddChild(_currentPreview);
		
		GD.Print("🔨 Entered turret build mode - Right click to place, Left click or ESC to cancel");
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
		var turret = _player.CurrentTurretScene.Instantiate<Turret>();
		turret.GlobalPosition = _currentPreview.GetPlacementPosition();
		_player.GetTree().Root.AddChild(turret);

		GD.Print($"🔧 Built turret at {turret.GlobalPosition} for ${cost}");
		
		// Exit build mode
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
