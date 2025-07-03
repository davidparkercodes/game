using Godot;

public class PlayerTurretBuilder
{
	private readonly Player _player;

	public PlayerTurretBuilder(Player player)
	{
		_player = player;
	}

	public void HandleInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouse && mouse.Pressed && mouse.ButtonIndex == MouseButton.Right)
		{
			BuildTurret();
		}
	}

	private void BuildTurret()
	{
		if (_player.CurrentTurretScene == null)
		{
			GD.PrintErr("❌ No turret scene selected!");
			return;
		}

		var turret = _player.CurrentTurretScene.Instantiate<Turret>();

		// Use GameManager's money system instead of Player's separate money
		if (GameManager.Instance == null)
		{
			GD.PrintErr("❌ GameManager not available!");
			return;
		}

		if (!GameManager.Instance.SpendMoney(turret.Cost))
		{
			GD.PrintErr($"❌ Not enough money! Need ${turret.Cost}, but have ${GameManager.Instance.Money}");
			return;
		}

		turret.GlobalPosition = _player.GetGlobalMousePosition();
		_player.GetTree().Root.AddChild(turret);

		GD.Print($"🔧 Built turret at {turret.GlobalPosition} for ${turret.Cost}");
	}
}
