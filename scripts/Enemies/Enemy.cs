using Godot;

public partial class Enemy : CharacterBody2D
{
	[Export] public int MaxHealth = 100;
	[Export] public float Speed = 60.0f;
	
	private int _currentHealth;
	private PathFollower _pathFollower;
	
	[Signal]
	public delegate void EnemyKilledEventHandler();
	[Signal]
	public delegate void EnemyReachedEndEventHandler();

	public override void _Ready()
	{
		_currentHealth = MaxHealth;
		
		// Create and add PathFollower component
		_pathFollower = new PathFollower();
		_pathFollower.Speed = Speed;
		AddChild(_pathFollower);
		
		// Connect to PathFollower signals
		_pathFollower.PathCompleted += OnPathCompleted;
		
		GD.Print($"👾 Enemy {Name} ready with path following");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		// Movement is now handled by PathFollower component
		// This method can be used for other enemy logic if needed
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
		EmitSignal(SignalName.EnemyKilled);
		QueueFree();
	}
	
	private void OnPathCompleted()
	{
		GD.Print($"{Name} reached the end!");
		EmitSignal(SignalName.EnemyReachedEnd);
		
		// Notify GameManager that enemy reached the end
		if (GameManager.Instance != null)
		{
			GameManager.Instance.OnEnemyReachedEnd();
		}
		
		QueueFree();
	}
	
	public float GetPathProgress()
	{
		return _pathFollower?.PathProgress ?? 0.0f;
	}
	
	public void SetSpeed(float newSpeed)
	{
		Speed = newSpeed;
		_pathFollower?.SetSpeed(newSpeed);
	}
}
