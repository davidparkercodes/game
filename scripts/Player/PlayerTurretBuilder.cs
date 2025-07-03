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

		if (_player.Money < turret.Cost)
		{
			GD.PrintErr($"❌ Not enough money! Need ${turret.Cost}, but have ${_player.Money}");
			return;
		}

		turret.GlobalPosition = _player.GetGlobalMousePosition();
		_player.GetTree().Root.AddChild(turret);

		_player.Money -= turret.Cost;
		GameManager.Instance.Hud.UpdateMoney(_player.Money);
		GD.Print($"🔧 Built turret at {turret.GlobalPosition} for ${turret.Cost}");
	}
}
