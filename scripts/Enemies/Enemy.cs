using Godot;

public partial class Enemy : CharacterBody2D
{
	[Export] public int MaxHealth = 100;
	[Export] public float Speed = 60.0f;
	
	private int _currentHealth;

	public override void _Ready()
	{
		_currentHealth = MaxHealth;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Velocity = new Vector2(0, Speed);
		MoveAndSlide();
	}

	public void TakeDamage(int amount)
	{
		_currentHealth -= amount;
		GD.Print($"{Name} took {amount} damage, remaining: {_currentHealth}");

		if (_currentHealth <= 0)
		{
			Die();
		}
	}

	private void Die()
	{
		GD.Print($"{Name} died!");
		QueueFree();
	}
}
