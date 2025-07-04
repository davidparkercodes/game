using Godot;

public partial class Enemy : Area2D
{
	[Export] public string EnemyType = "basic_enemy";
	public int MaxHealth { get; private set; }
	public float Speed { get; private set; }
	public int Damage { get; private set; }
	public int RewardGold { get; private set; }
	public int RewardXp { get; private set; }
	
	private int _currentHealth;
	private PathFollower _pathFollower;
	private EnemyStatsData _stats;
	
	[Signal]
	public delegate void EnemyKilledEventHandler();
	[Signal]
	public delegate void EnemyReachedEndEventHandler();

	public override void _Ready()
	{
		// Load stats from configuration
		LoadStatsFromConfig();
		
		_currentHealth = MaxHealth;
		
		// Add to enemies group for wave completion tracking
		AddToGroup("enemies");
		
		// Create and add PathFollower component
		_pathFollower = new PathFollower();
		_pathFollower.Speed = Speed;
		AddChild(_pathFollower);
		
		// Connect to PathFollower signals
		_pathFollower.PathCompleted += OnPathCompleted;
		
		GD.Print($"👾 Enemy {Name} ({EnemyType}) ready: HP={MaxHealth}, Speed={Speed}, Damage={Damage}");
	}
	
	private void LoadStatsFromConfig()
	{
		if (StatsManager.Instance != null)
		{
			_stats = StatsManager.Instance.GetEnemyStats(EnemyType);
		}
		else
		{
			// Fallback to default stats if StatsManager not available
			_stats = new EnemyStatsData();
			GD.PrintErr($"⚠️ StatsManager not available, using default stats for enemy {Name}");
		}
		
		// Apply stats from configuration
		MaxHealth = _stats.max_health;
		Speed = _stats.speed;
		Damage = _stats.damage;
		RewardGold = _stats.reward_gold;
		RewardXp = _stats.reward_xp;
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
		
		// Remove from enemies group before destruction
		RemoveFromGroup("enemies");
		
		EmitSignal(SignalName.EnemyKilled);
		QueueFree();
	}
	
	private void OnPathCompleted()
	{
		GD.Print($"{Name} reached the end!");
		
		// Remove from enemies group before destruction
		RemoveFromGroup("enemies");
		
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
	
	public void SetEnemyType(string enemyType)
	{
		EnemyType = enemyType;
		LoadStatsFromConfig();
	}
	
	public EnemyStatsData GetStats()
	{
		return _stats;
	}
	
	public void ApplyHealthMultiplier(float multiplier)
	{
		MaxHealth = Mathf.RoundToInt(MaxHealth * multiplier);
		_currentHealth = MaxHealth; // Also update current health
	}
	
	public void SetMaxHealth(int newMaxHealth)
	{
		MaxHealth = newMaxHealth;
		_currentHealth = MaxHealth; // Reset current health to max
	}
}
