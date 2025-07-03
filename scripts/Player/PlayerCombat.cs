using Godot;
using System.Threading.Tasks;

public class PlayerCombat
{
	private readonly Player _player;
	private readonly AnimationPlayer _animPlayer;
	private readonly Node2D _hitbox;
	private bool _isAttacking;

	public PlayerCombat(Player player)
	{
		_player = player;
		_animPlayer = _player.GetNode<AnimationPlayer>("AnimationPlayer");
		_hitbox = _player.GetNode<Node2D>("Hitbox");
	}

	public void Update(double delta)
	{
		if (!_isAttacking && Input.IsActionJustPressed("attack"))
			_ = Attack();
	}

	private async Task Attack()
	{
		_isAttacking = true;
		_animPlayer.Play("attack");
		await _player.ToSignal(_animPlayer, "animation_finished");
		_isAttacking = false;
	}
}
