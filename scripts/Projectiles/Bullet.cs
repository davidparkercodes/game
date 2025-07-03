using Godot;

public partial class Bullet : Area2D
{
	[Export] public float Speed = 900;
	[Export] public int Damage = 50;

	private Vector2 _velocity;

	public override void _Ready()
	{
		Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
	}

	public void SetBulletVelocity(Vector2 velocity)
	{
		_velocity = velocity;
	}

	public override void _Process(double delta)
	{
		Position += _velocity * (float)delta;

		if (Position.Length() > 3000)
			QueueFree();
	}
	
	private void OnBodyEntered(Node body)
	{
		GD.Print("Bullet hit: " + body.Name);

		if (body.IsInGroup("enemies"))
		{
			if (body is Enemy enemy)
			{
				enemy.TakeDamage(Damage);
			}

			QueueFree();
		}
	}
}
