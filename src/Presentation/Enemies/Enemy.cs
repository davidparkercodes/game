using Godot;
using Game.Infrastructure.DI;
using Game.Domain.Enemies.ValueObjects;
using Game.Infrastructure.Managers;

namespace Game.Presentation.Enemies;

public partial class Enemy : Area2D
{
	[Export] public string EnemyType = "basic_enemy";
	public int MaxHealth { get; private set; }
	public float Speed { get; private set; }
	public int Damage { get; private set; }
	public int RewardGold { get; private set; }
	public int RewardXp { get; private set; }
	
	private int _currentHealth;
	private EnemyStatsData _stats;
	
	[Signal]
	public delegate void EnemyKilledEventHandler();
	[Signal]
	public delegate void EnemyReachedEndEventHandler();

	public override void _Ready()
	{
		LoadStatsFromConfig();
		
		_currentHealth = MaxHealth;
		
		AddToGroup("enemies");
		
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
			_stats = new EnemyStatsData();
			GD.PrintErr($"⚠️ StatsManager not available, using default stats for enemy {Name}");
		}
		
		MaxHealth = _stats.max_health;
		Speed = _stats.speed;
		Damage = _stats.damage;
		RewardGold = _stats.reward_gold;
		RewardXp = _stats.reward_xp;
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
		
		RemoveFromGroup("enemies");
		
		EmitSignal(SignalName.EnemyKilled);
		QueueFree();
	}
	
	public void OnPathCompleted()
	{
		GD.Print($"{Name} reached the end!");
		
		RemoveFromGroup("enemies");
		
		EmitSignal(SignalName.EnemyReachedEnd);
		
		if (GameManager.Instance != null)
		{
			GameManager.Instance.OnEnemyReachedEnd();
		}
		
		QueueFree();
	}
	
	public void SetSpeed(float newSpeed)
	{
		Speed = newSpeed;
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
		_currentHealth = MaxHealth;
	}
	
	public void SetMaxHealth(int newMaxHealth)
	{
		MaxHealth = newMaxHealth;
		_currentHealth = MaxHealth;
	}
}
