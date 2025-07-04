using Godot;

public partial class Bullet : Area2D
{
	[Export] public float Speed = 900;
	[Export] public int Damage = 50;

	private Vector2 _velocity;
	private string _impactSoundKey = "basic_bullet_impact";

	public override void _Ready()
	{
		Connect("area_entered", new Callable(this, nameof(OnAreaEntered)));
	}

	public void SetBulletVelocity(Vector2 velocity)
	{
		_velocity = velocity;
	}

	public void SetImpactSound(string soundKey)
	{
		_impactSoundKey = soundKey;
	}

	public override void _Process(double delta)
	{
		Position += _velocity * (float)delta;

		if (Position.Length() > 3000)
			QueueFree();
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area.IsInGroup("enemies"))
		{
			if (area is Enemy enemy)
			{
				enemy.TakeDamage(Damage);
			}

			if (SoundManager.Instance != null)
			{
				SoundManager.Instance.PlaySound(_impactSoundKey);
			}

			QueueFree();
		}
	}
}
