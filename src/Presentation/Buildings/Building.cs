using Godot;
using Game.Di;
using Game.Presentation.Enemies;
using Game.Presentation.Projectiles;
using Game.Infrastructure.Audio.Services;
using Game.Infrastructure.Stats.Services;
using System.Collections.Generic;
using System.Linq;

namespace Game.Presentation.Buildings;

public partial class Building : StaticBody2D
{
	[Export] public int Cost { get; set; } = 10;
	[Export] public int Damage { get; set; } = 10;
	[Export] public float Range { get; set; } = 150.0f;
	[Export] public float FireRate { get; set; } = 1.0f;
	[Export] public PackedScene? BulletScene;

	protected Godot.Timer _fireTimer = null!;
	protected Area2D _rangeArea = null!;
	protected CollisionShape2D _rangeCollision = null!;
	protected bool _showingRange = false;
	private Line2D _rangeCircle = null!;
	
	protected List<Enemy> _enemiesInRange = new List<Enemy>();
	protected Enemy? _currentTarget = null;
	private const string LogPrefix = "🏢 [TOWER]";
	private bool _isActive = true;
	
	// Shooting system properties
	protected string _shootSoundKey = "basic_tower_shoot";
	protected string _bulletImpactSoundKey = "basic_bullet_impact";
	private bool _canFire = true;
	
	// Performance optimization - bullet pooling
	private static readonly List<Bullet> _bulletPool = new List<Bullet>();
	private const int MAX_POOLED_BULLETS = 50;

	public override void _Ready()
	{
		_fireTimer = GetNode<Godot.Timer>("Timer");
		_rangeArea = GetNode<Area2D>("Area2D");
		_rangeCollision = _rangeArea.GetNode<CollisionShape2D>("CollisionShape2D");
		
		InitializeStats();
		CreateRangeVisual();
		ConnectSignals();
		
		GD.Print($"{LogPrefix} {Name} ready - Range: {Range}, Damage: {Damage}, FireRate: {FireRate}");
	}
	
	private void ConnectSignals()
	{
		if (_rangeArea != null)
		{
			_rangeArea.Connect("area_entered", new Callable(this, nameof(OnEnemyEnteredRange)));
			_rangeArea.Connect("area_exited", new Callable(this, nameof(OnEnemyExitedRange)));
			GD.Print($"{LogPrefix} {Name} range area signals connected");
		}
		else
		{
			GD.PrintErr($"{LogPrefix} {Name} failed to connect range area signals - _rangeArea is null");
		}
		
		if (_fireTimer != null)
		{
			_fireTimer.Connect("timeout", new Callable(this, nameof(OnFireTimerTimeout)));
			_fireTimer.OneShot = false;
			GD.Print($"{LogPrefix} {Name} fire timer connected");
		}
		else
		{
			GD.PrintErr($"{LogPrefix} {Name} failed to connect fire timer - _fireTimer is null");
		}
	}

	public virtual void InitializeStats()
	{
		if (_rangeCollision?.Shape is CircleShape2D circle)
		{
			circle.Radius = Range;
		}
		
		if (_fireTimer != null)
		{
			_fireTimer.WaitTime = 1.0f / FireRate;
		}
	}

	public virtual void ShowRange()
	{
		if (_rangeCircle != null)
		{
			_rangeCircle.Visible = true;
			_showingRange = true;
		}
	}

	public virtual void HideRange()
	{
		if (_rangeCircle != null)
		{
			_rangeCircle.Visible = false;
			_showingRange = false;
		}
	}

	public virtual void SetRangeColor(Color color)
	{
		if (_rangeCircle != null)
		{
			_rangeCircle.DefaultColor = color;
		}
	}

	private void CreateRangeVisual()
	{
		_rangeCircle = new Line2D();
		_rangeCircle.Width = 2.0f;
		_rangeCircle.DefaultColor = new Color(0.2f, 0.8f, 0.2f, 0.6f);
		_rangeCircle.Visible = false;
		
		const int segments = 64;
		for (int i = 0; i <= segments; i++)
		{
			float angle = i * 2.0f * Mathf.Pi / segments;
			Vector2 point = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Range;
			_rangeCircle.AddPoint(point);
		}
		
		AddChild(_rangeCircle);
	}
	
	private void OnEnemyEnteredRange(Area2D area)
	{
		if (!_isActive) return;
		
		if (area is Enemy enemy)
		{
			if (!_enemiesInRange.Contains(enemy))
			{
				_enemiesInRange.Add(enemy);
				GD.Print($"{LogPrefix} {Name} detected enemy: {enemy.Name} (Total in range: {_enemiesInRange.Count})");
				
			if (_currentTarget == null)
				{
					UpdateTarget();
				}
				
				StartFiringIfNeeded();
			}
		}
	}
	
	private void OnEnemyExitedRange(Area2D area)
	{
		if (area is Enemy enemy)
		{
			if (_enemiesInRange.Contains(enemy))
			{
				_enemiesInRange.Remove(enemy);
				GD.Print($"{LogPrefix} {Name} lost enemy: {enemy.Name} (Total in range: {_enemiesInRange.Count})");
				
				if (_currentTarget == enemy)
				{
					_currentTarget = null;
					UpdateTarget();
				}
				
				StopFiringIfNeeded();
			}
		}
	}
	
	protected virtual void UpdateTarget()
	{
		CleanupEnemiesList();
		
		if (_enemiesInRange.Count == 0)
		{
			_currentTarget = null;
			GD.Print($"{LogPrefix} {Name} no targets available");
			return;
		}
		
		Enemy? newTarget = SelectBestTarget();
		if (newTarget != _currentTarget)
		{
			_currentTarget = newTarget;
			if (_currentTarget != null)
				{
					GD.Print($"{LogPrefix} {Name} targeting: {_currentTarget.Name}");
				}
		}
	}
	
	private void CleanupEnemiesList()
	{
		for (int i = _enemiesInRange.Count - 1; i >= 0; i--)
		{
			if (_enemiesInRange[i] == null || !IsInstanceValid(_enemiesInRange[i]))
			{
				_enemiesInRange.RemoveAt(i);
			}
		}
	}
	
	protected virtual Enemy? SelectBestTarget()
	{
		if (_enemiesInRange.Count == 0) return null;
		
		// Optimized target selection - prioritize based on distance and health
		Enemy? bestTarget = null;
		float bestScore = float.MinValue;
		
		foreach (Enemy enemy in _enemiesInRange)
		{
			if (enemy == null || !IsInstanceValid(enemy)) continue;
			
			float distance = GlobalPosition.DistanceTo(enemy.GlobalPosition);
			
			// Score calculation: closer enemies and lower health get higher priority
			// This makes towers focus fire and finish off wounded enemies
			float distanceScore = (Range - distance) / Range; // 0-1, higher = closer
			float healthScore = 1.0f - (enemy.GetStats().max_health > 0 ? 
				(float)enemy.GetStats().max_health / enemy.GetStats().max_health : 0); // Prefer low health
			
			float totalScore = (distanceScore * 0.7f) + (healthScore * 0.3f);
			
			if (totalScore > bestScore)
			{
				bestScore = totalScore;
				bestTarget = enemy;
			}
		}
		
		return bestTarget;
	}
	
	public bool HasTarget()
	{
		return _currentTarget != null && IsInstanceValid(_currentTarget);
	}
	
	public Enemy? GetCurrentTarget()
	{
		return _currentTarget;
	}
	
	public int GetEnemyCount()
	{
		return _enemiesInRange.Count;
	}
	
	public void SetActive(bool active)
	{
		_isActive = active;
		if (!_isActive)
		{
			_currentTarget = null;
			_enemiesInRange.Clear();
			StopFiring();
		}
	}
	
	// ===== SHOOTING SYSTEM =====
	
	private void OnFireTimerTimeout()
	{
		if (!_isActive || !_canFire) return;
		
		if (HasTarget())
		{
			FireAtTarget();
		}
		else
		{
			StopFiring();
		}
	}
	
	protected virtual void FireAtTarget()
	{
		if (_currentTarget == null || !IsInstanceValid(_currentTarget)) return;
		
		if (BulletScene == null)
		{
			GD.PrintErr($"{LogPrefix} {Name} cannot fire - BulletScene is null");
			return;
		}
		
		// Calculate direction to target
		Vector2 direction = (_currentTarget.GlobalPosition - GlobalPosition).Normalized();
		
		// Rotate tower to face target
		RotateTowardsTarget(direction);
		
		// Get bullet from pool or create new one
		Bullet? bullet = GetPooledBullet();
		if (bullet == null)
		{
			GD.PrintErr($"{LogPrefix} {Name} failed to get bullet from pool");
			return;
		}
		
		// Configure bullet
		bullet.GlobalPosition = GlobalPosition;
		bullet.SetBulletVelocity(direction * bullet.Speed);
		bullet.Damage = Damage;
		bullet.SetImpactSound(_bulletImpactSoundKey);
		
		// Reset bullet for reuse
		bullet.Visible = true;
		
		// Add bullet to scene
		GetTree().CurrentScene.AddChild(bullet);
		
		// Play shooting sound
		PlayShootSound();
		
		GD.Print($"{LogPrefix} {Name} fired at {_currentTarget.Name} (Damage: {Damage})");
	}
	
	protected virtual void PlayShootSound()
	{
		if (SoundManagerService.Instance != null)
		{
			GD.Print($"{LogPrefix} {Name} playing shoot sound: {_shootSoundKey}");
			SoundManagerService.Instance.PlaySound(_shootSoundKey);
		}
		else
		{
			GD.PrintErr($"{LogPrefix} {Name} SoundManagerService not available for shoot sound");
		}
	}
	
	private void StartFiringIfNeeded()
	{
		if (!_isActive || !HasTarget()) return;
		
		if (_fireTimer != null && _fireTimer.IsStopped())
		{
			_fireTimer.Start();
			GD.Print($"{LogPrefix} {Name} started firing");
		}
	}
	
	private void StopFiringIfNeeded()
	{
		if (_enemiesInRange.Count == 0 || !HasTarget())
		{
			StopFiring();
		}
	}
	
	private void StopFiring()
	{
		if (_fireTimer != null && !_fireTimer.IsStopped())
		{
			_fireTimer.Stop();
			GD.Print($"{LogPrefix} {Name} stopped firing");
		}
	}
	
	protected virtual void SetShootSoundKey(string soundKey)
	{
		_shootSoundKey = soundKey;
	}
	
	protected virtual void SetBulletImpactSoundKey(string soundKey)
	{
		_bulletImpactSoundKey = soundKey;
	}
	
	protected virtual void LoadStatsFromConfig(string towerType)
	{
		if (StatsManagerService.Instance != null)
		{
			var stats = StatsManagerService.Instance.GetBuildingStats(towerType);
			
			Cost = stats.cost;
			Damage = stats.damage;
			Range = stats.range;
			FireRate = stats.fire_rate;
			
			// Set sound keys based on tower type
			SetSoundKeysForTowerType(towerType);
			
			GD.Print($"{LogPrefix} {Name} loaded config for {towerType}: Cost={Cost}, Damage={Damage}, Range={Range}, FireRate={FireRate}");
		}
		else
		{
			GD.PrintErr($"{LogPrefix} {Name} StatsManagerService not available, using default stats");
			SetSoundKeysForTowerType(towerType);
		}
	}
	
	private void SetSoundKeysForTowerType(string towerType)
	{
		switch (towerType)
		{
			case "basic_tower":
				SetShootSoundKey("basic_tower_shoot");
				SetBulletImpactSoundKey("basic_bullet_impact");
				break;
			case "sniper_tower":
				SetShootSoundKey("sniper_tower_shoot");
				SetBulletImpactSoundKey("sniper_bullet_impact");
				break;
			default:
				SetShootSoundKey("basic_tower_shoot");
				SetBulletImpactSoundKey("basic_bullet_impact");
				break;
		}
	}
	
	// ===== VISUAL ENHANCEMENTS =====
	
	protected virtual void RotateTowardsTarget(Vector2 direction)
	{
		// Calculate the angle to face the target
		float targetAngle = direction.Angle();
		
		// Smoothly rotate towards target (optional: can be instant)
		float rotationSpeed = 5.0f; // Radians per second
		float currentAngle = Rotation;
		float angleDifference = Mathf.AngleDifference(currentAngle, targetAngle);
		
		// Use smooth rotation for more polished look
		if (Mathf.Abs(angleDifference) > 0.1f)
		{
			float deltaRotation = Mathf.Sign(angleDifference) * rotationSpeed * (float)GetProcessDeltaTime();
			if (Mathf.Abs(deltaRotation) > Mathf.Abs(angleDifference))
			{
				Rotation = targetAngle;
			}
			else
			{
				Rotation += deltaRotation;
			}
		}
		else
		{
			Rotation = targetAngle;
		}
	}
	
	// ===== PERFORMANCE OPTIMIZATION =====
	
	private Bullet? GetPooledBullet()
	{
		// Try to get a bullet from the pool
		for (int i = _bulletPool.Count - 1; i >= 0; i--)
		{
			Bullet pooledBullet = _bulletPool[i];
			if (pooledBullet != null && !pooledBullet.IsInsideTree())
			{
				_bulletPool.RemoveAt(i);
				return pooledBullet;
			}
		}
		
		// No available bullets in pool, create a new one
		if (BulletScene != null)
		{
			Bullet newBullet = BulletScene.Instantiate<Bullet>();
			if (newBullet != null)
			{
				return newBullet;
			}
		}
		
		return null;
	}
	
	// Simple pooling without complex signal handling for now
	// In a full implementation, bullets would mark themselves as available for pooling
	
	public static void ClearBulletPool()
	{
		// Utility method to clear the bullet pool (useful for scene changes)
		_bulletPool.Clear();
	}
}
