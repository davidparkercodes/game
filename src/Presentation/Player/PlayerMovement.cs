using Godot;
using Game.Presentation.Core;
using Game.Infrastructure.Common.Converters;

namespace Game.Presentation.Player;

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

		// Calculate desired velocity and position
		Vector2 desiredVelocity = input * _player.Speed;
		Vector2 currentPosition = _player.GlobalPosition;
		Vector2 desiredPosition = currentPosition + desiredVelocity * (float)delta;
		
		// Check boundary constraints if MapBoundaryService is available
		if (Main.MapBoundaryService != null && Main.MapBoundaryService.IsInitialized)
		{
			// Check if the desired position is valid for walking
			var domainPosition = GodotGeometryConverter.FromGodotVector2(desiredPosition);
			if (Main.MapBoundaryService.CanWalkToPosition(domainPosition))
			{
				// Position is valid, use normal movement
				_player.Velocity = desiredVelocity;
			}
			else
			{
				// Position is invalid, stop movement
				_player.Velocity = Vector2.Zero;
			}
		}
		else
		{
			// Fallback to original movement if boundary service isn't available
			_player.Velocity = desiredVelocity;
		}
		
		_player.MoveAndSlide();
	}
}
