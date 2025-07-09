using Godot;
using Game.Di;
using Game.Domain.Enemies.ValueObjects;
using Game.Infrastructure.Stats.Services;
using Game.Infrastructure.Game.Services;
using Game.Infrastructure.Audio.Services;

namespace Game.Presentation.Enemies;

public partial class Enemy : Area2D
{
	[Export] public string EnemyType = Domain.Entities.EnemyConfigKeys.BasicEnemy;
	public int MaxHealth { get; private set; }
	public float Speed { get; private set; }
	public int Damage { get; private set; }
	public int RewardGold { get; private set; }
	public int RewardXp { get; private set; }
	public float ScaleMultiplier { get; private set; } = 1.0f;
	
	private int _currentHealth;
	private EnemyStatsData _stats;
	private ProgressBar? _healthBar;
	
	[Signal]
	public delegate void EnemyKilledEventHandler();
	[Signal]
	public delegate void EnemyReachedEndEventHandler();

	public override void _Ready()
	{
		LoadStatsFromConfig();
		
		_currentHealth = MaxHealth;
		
		AddToGroup("enemies");
		
		ApplyVisualScale();
		
		// Update sprite region based on enemy type
		UpdateSpriteRegion();
		
		// Initialize health bar if it exists
		InitializeHealthBar();
		
		GD.Print($"👾 Enemy {Name} ({EnemyType}) ready: HP={MaxHealth}, Speed={Speed}, Damage={Damage}, Scale={ScaleMultiplier}x");
	}
	
	private void LoadStatsFromConfig()
	{
		if (StatsManagerService.Instance != null)
		{
			_stats = StatsManagerService.Instance.GetEnemyStats(EnemyType);
		}
		else
		{
			_stats = new EnemyStatsData();
			GD.PrintErr($"⚠️ StatsManagerService not available, using default stats for enemy {Name}");
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
		
		// Update health bar
		UpdateHealthBar();

		if (_currentHealth <= 0)
		{
			Die();
		}
	}

	private void Die()
	{
		GD.Print($"{Name} died!");
		
		// Do NOT stop boss music here - let WaveManager handle it when the entire wave is complete
		// This prevents music from stopping when regular enemies die in boss waves
		if (IsBossEnemy())
		{
			GD.Print($"🎵 Boss enemy {Name} died! Boss music will be stopped by WaveManager when wave completes.");
		}
		
		RemoveFromGroup("enemies");
		
		// Notify GameService about the enemy death for money/score
		if (GameService.Instance != null)
		{
			GameService.Instance.OnEnemyKilled(RewardGold);
			GD.Print($"💰 Enemy {Name} gave {RewardGold} gold reward");
		}
		
		// Emit signal for any other listeners (like WaveSpawnerService)
		GD.Print($"📡 Enemy {Name} emitting EnemyKilled signal");
		EmitSignal(SignalName.EnemyKilled);
		QueueFree();
	}
	
	public void OnPathCompleted()
	{
		GD.Print($"{Name} reached the end!");
		
		RemoveFromGroup("enemies");
		
		EmitSignal(SignalName.EnemyReachedEnd);
		
		if (GameService.Instance != null)
		{
			GameService.Instance.OnEnemyReachedEnd();
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
		UpdateSpriteRegion();
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
	
	public void SetScaleMultiplier(float scale)
	{
		ScaleMultiplier = scale;
		ApplyVisualScale();
	}
	
	private void ApplyVisualScale()
	{
		if (ScaleMultiplier != 1.0f)
		{
			Scale = new Vector2(ScaleMultiplier, ScaleMultiplier);
			GD.Print($"👑 {Name} scaled to {ScaleMultiplier}x size (boss enemy)");
		}
	}
	
	public bool IsBossEnemy()
	{
		return EnemyType == Domain.Entities.EnemyConfigKeys.BossEnemy || ScaleMultiplier > 1.5f;
	}
	
	private void InitializeHealthBar()
	{
		_healthBar = GetNodeOrNull<ProgressBar>("HealthBar");
		if (_healthBar != null)
		{
			_healthBar.MinValue = 0;
			_healthBar.MaxValue = MaxHealth;
			_healthBar.Value = _currentHealth;
			_healthBar.Visible = IsBossEnemy(); // Only show for boss enemies
			
			// Style the health bar with red fill
			if (IsBossEnemy())
			{
				_healthBar.Modulate = new Color(1.0f, 0.2f, 0.2f, 1.0f); // Red tint
			}
			
			GD.Print($"🏥 Health bar initialized for {Name}: {_currentHealth}/{MaxHealth}");
		}
		else if (IsBossEnemy())
		{
			GD.PrintErr($"⚠️ Boss enemy {Name} missing health bar!");
		}
	}
	
	private void UpdateHealthBar()
	{
		if (_healthBar != null && _healthBar.Visible)
		{
			_healthBar.Value = _currentHealth;
			GD.Print($"🏥 Updated health bar: {_currentHealth}/{MaxHealth} ({(_currentHealth / (float)MaxHealth * 100):F1}%)");
		}
	}
	
	private void StopBossMusic()
	{
		if (SoundManagerService.Instance != null)
		{
			SoundManagerService.Instance.StopMusic();
			GD.Print("🎵 Boss defeated! Stopped boss battle music.");
		}
		else
		{
		GD.Print($"⚠️ StatsManagerService not available to stop boss music");
		}
	}
	
	private void UpdateSpriteRegion()
	{
		var sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		if (sprite == null)
		{
			GD.PrintErr($"⚠️ Sprite2D not found for enemy {Name}");
			return;
		}
		
		// Set sprite region based on enemy type
		// Sprite sheet: 64x128 with 16x16 tiles
		Rect2 region = EnemyType switch
		{
			Domain.Entities.EnemyConfigKeys.BasicEnemy => new Rect2(32, 0, 16, 16),    // (2,0)
			Domain.Entities.EnemyConfigKeys.TankEnemy => new Rect2(32, 16, 16, 16),    // (2,1)
			Domain.Entities.EnemyConfigKeys.FastEnemy => new Rect2(32, 32, 16, 16),    // (2,2)
			Domain.Entities.EnemyConfigKeys.EliteEnemy => new Rect2(32, 48, 16, 16),   // (2,3)
			Domain.Entities.EnemyConfigKeys.BossEnemy => new Rect2(48, 0, 16, 16),     // (3,0)
			_ => new Rect2(32, 0, 16, 16) // Default to basic enemy
		};
		
		sprite.RegionEnabled = true;
		sprite.RegionRect = region;
		
		// Apply color modulation based on enemy type
		Color enemyColor = EnemyType switch
		{
			Domain.Entities.EnemyConfigKeys.BasicEnemy => new Color(1.0f, 1.0f, 1.0f, 1.0f),     // White (original)
			Domain.Entities.EnemyConfigKeys.TankEnemy => new Color(0.4f, 0.4f, 0.4f, 1.0f),      // Dark gray (armored)
			Domain.Entities.EnemyConfigKeys.FastEnemy => new Color(0.2f, 1.0f, 0.2f, 1.0f),      // Bright green (fast)
			Domain.Entities.EnemyConfigKeys.EliteEnemy => new Color(1.0f, 0.8f, 0.2f, 1.0f),     // Gold (elite)
			Domain.Entities.EnemyConfigKeys.BossEnemy => new Color(0.8f, 0.2f, 0.2f, 1.0f),      // Red (boss)
			_ => new Color(1.0f, 1.0f, 1.0f, 1.0f) // Default to white
		};
		
		sprite.Modulate = enemyColor;
		
		GD.Print($"🎨 Updated sprite region for {EnemyType} to {region} with color {enemyColor}");
	}
}
