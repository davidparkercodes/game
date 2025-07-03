using Godot;

public class PlayerMovement
{
	private readonly Player _player;

	public Vector2 LastDirection = Vector2.Up;

	public PlayerMovement(Player player)
	{
		_player = player;
	}

	public void Update(double delta)
	{
		Vector2 input = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		if (input != Vector2.Zero)
			LastDirection = input.Normalized();

		_player.Velocity = input * _player.Speed;
		_player.MoveAndSlide();
	}
}
