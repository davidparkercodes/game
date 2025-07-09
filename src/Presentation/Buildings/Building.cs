using Godot;
using Game.Di;
using Game.Presentation.Enemies;
using Game.Presentation.Projectiles;
using Game.Infrastructure.Audio.Services;
using Game.Infrastructure.Stats.Services;
using Game.Presentation.UI;
using Game.Domain.Buildings.ValueObjects;
using System.Collections.Generic;
using System.Linq;

namespace Game.Presentation.Buildings;

public partial class Building : StaticBody2D
{
	[Export] public int Cost { get; set; } = 10;
	[Export] public int Damage { get; set; } = 10;
	[Export] public float Range { get; set; } = 150.0f;
	[Export] public float AttackSpeed { get; set; } = 30.0f;
	[Export] public float CollisionRadius { get; set; } = BuildingRegistry.DefaultCollisionRadius;
	[Export] public PackedScene? BulletScene;
	[Export] public bool IsPreview { get; set; } = false;

	protected Godot.Timer _fireTimer = null!;
	protected Area2D _rangeArea = null!;
	protected CollisionShape2D _rangeCollision = null!;
	protected bool _showingRange = false;
	private Line2D _rangeCircle = null!;
	
	// Input detection for tower selection
	private Area2D _inputArea = null!;
	private CollisionShape2D _inputCollision = null!;
	
	protected List<Enemy> _enemiesInRange = new List<Enemy>();
	protected Enemy? _currentTarget = null;
	private const string LogPrefix = "🏢 [TOWER]";
	private bool _isActive = true;
	
	// Building selection system
	public bool IsSelected { get; private set; } = false;
	private Line2D? _selectionBorder = null;
	private Color _originalModulate = Colors.White;
	
	// Upgrade level visual indicators (removed level labels and stars)
	// Only color tinting is used for upgrade feedback
	
	// Animation components
	private Tween? _selectionTween = null;
	private Tween? _rangeTween = null;
	
	// Building upgrade tracking
	public int UpgradeLevel { get; set; } = 0;
	public int TotalInvestment { get; set; } = 0;
	public BuildingStats BaseStats { get; private set; }
	
	// Shooting system properties
	protected string _shootSoundKey = $"{Domain.Entities.BuildingConfigKeys.BasicTower}_shoot";
	protected string _bulletImpactSoundKey = $"{Domain.Entities.BuildingConfigKeys.BasicTower.Replace("_tower", "_bullet")}_impact";
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
		CreateInputArea();
		CreateAnimations();
		ConnectSignals();
		SetupInputHandling();
		
		// Register this building with the registry for collision detection (but not for preview buildings)
		if (!IsPreview)
		{
			BuildingRegistry.Instance.RegisterBuilding(this);
		}
		else
		{
			GD.Print($"{LogPrefix} {Name} is a preview building - skipping registry registration");
		}
		
		GD.Print($"{LogPrefix} {Name} ready - Range: {Range}, Damage: {Damage}, AttackSpeed: {AttackSpeed}, CollisionRadius: {CollisionRadius}");
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
		
		// Connect input event for building selection to the smaller input area
		if (_inputArea != null && !IsPreview)
		{
			_inputArea.Connect("input_event", new Callable(this, nameof(OnAreaInputEvent)));
			_inputArea.InputPickable = true;
			GD.Print($"{LogPrefix} {Name} input event connected to input area");
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
			_fireTimer.WaitTime = 30.0f / AttackSpeed;
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
	
	public void UpdateRangeVisual()
	{
		if (_rangeCircle == null) return;
		
		// Clear existing points
		_rangeCircle.ClearPoints();
		
		// Recreate circle with current Range
		const int segments = 64;
		for (int i = 0; i <= segments; i++)
		{
			float angle = i * 2.0f * Mathf.Pi / segments;
			Vector2 point = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Range;
			_rangeCircle.AddPoint(point);
		}
		
		GD.Print($"{LogPrefix} {Name} range visual updated to radius {Range}");
	}
	
	private void CreateInputArea()
	{
		if (IsPreview) return; // Don't create input area for preview buildings
		
		// Create a small Area2D for input detection (only around the tower sprite)
		_inputArea = new Area2D();
		_inputArea.Name = "InputArea";
		
		// Create collision shape for input detection - smaller than the tower's collision radius
		_inputCollision = new CollisionShape2D();
		var inputShape = new CircleShape2D();
		inputShape.Radius = CollisionRadius; // Use the building's collision radius (typically 12px)
		_inputCollision.Shape = inputShape;
		
		// Add collision shape to input area
		_inputArea.AddChild(_inputCollision);
		
		// Add input area to the building
		AddChild(_inputArea);
		
		GD.Print($"{LogPrefix} {Name} input area created with radius {inputShape.Radius}");
	}
	
	private void OnEnemyEnteredRange(Area2D area)
	{
		if (!_isActive) return;
		
		if (area is Enemy enemy)
		{
			if (!_enemiesInRange.Contains(enemy))
			{
				_enemiesInRange.Add(enemy);
				
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
			return;
		}
		
		Enemy? newTarget = SelectBestTarget();
		if (newTarget != _currentTarget)
		{
			_currentTarget = newTarget;
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
		
		// Bullet fired successfully
	}
	
	protected virtual void PlayShootSound()
	{
		if (SoundManagerService.Instance != null)
		{
			SoundManagerService.Instance.PlaySound(_shootSoundKey);
		}
	}
	
	private void StartFiringIfNeeded()
	{
		if (!_isActive || !HasTarget()) return;
		
		if (_fireTimer != null && _fireTimer.IsStopped())
		{
			_fireTimer.Start();
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
			AttackSpeed = stats.attack_speed;
			CollisionRadius = stats.collision_radius;
			
			// Store base stats for upgrade calculations
			BaseStats = new BuildingStats(
				cost: stats.cost,
				damage: stats.damage,
				range: stats.range,
				attackSpeed: stats.attack_speed,
				bulletSpeed: stats.bullet_speed,
				shootSound: $"{towerType}_shoot",
				impactSound: $"{towerType.Replace("_tower", "_bullet")}_impact",
				description: stats.description
			);
			
			// Initialize total investment with base cost
			if (TotalInvestment == 0)
			{
				TotalInvestment = Cost;
			}
			
			// Set sound keys based on tower type
			SetSoundKeysForTowerType(towerType);
			
			GD.Print($"{LogPrefix} {Name} loaded config for {towerType}: Cost={Cost}, Damage={Damage}, Range={Range}, AttackSpeed={AttackSpeed}, CollisionRadius={CollisionRadius}");
		}
		else
		{
			GD.PrintErr($"{LogPrefix} {Name} StatsManagerService not available, using default stats");
			SetSoundKeysForTowerType(towerType);
		}
	}
	
	private void SetSoundKeysForTowerType(string towerType)
	{
		// Config-driven sound key generation instead of hardcoded switch statement
		string shootSoundKey = $"{towerType}_shoot";
		string impactSoundKey = $"{towerType.Replace("_tower", "_bullet")}_impact";
		
		// Fallback to basic tower sounds if the specific sound doesn't exist
		// The SoundManagerService will handle missing sounds gracefully
		SetShootSoundKey(shootSoundKey);
		SetBulletImpactSoundKey(impactSoundKey);
		
		GD.Print($"{LogPrefix} {Name} configured sounds: shoot={shootSoundKey}, impact={impactSoundKey}");
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
	
	public void SetPreviewMode(bool isPreview)
	{
		IsPreview = isPreview;
		
		if (isPreview)
		{
			// Disable all building functionality for preview mode
			_isActive = false;
			_canFire = false;
			
			// Stop and disable timer
			if (_fireTimer != null)
			{
				_fireTimer.Stop();
				_fireTimer.Paused = true;
			}
			
			// Clear enemy detection lists
			_enemiesInRange.Clear();
			_currentTarget = null;
			
			// Disable collision detection for enemies (keep layers disabled)
			SetCollisionLayerValue(1, false);
			SetCollisionMaskValue(1, false);
			
			// Make sure range area doesn't detect enemies
			if (_rangeArea != null)
			{
				_rangeArea.SetCollisionLayerValue(1, false);
				_rangeArea.SetCollisionMaskValue(2, false); // Enemy layer
			}
			
			GD.Print($"{LogPrefix} {Name} set to preview mode - all functionality disabled");
		}
		else
		{
			// Enable building functionality for placed buildings
			_isActive = true;
			_canFire = true;
			
			// Enable timer
			if (_fireTimer != null)
			{
				_fireTimer.Paused = false;
			}
			
			// Enable collision detection
			SetCollisionLayerValue(1, true);
			SetCollisionMaskValue(1, true);
			
			// Enable range area for enemy detection
			if (_rangeArea != null)
			{
				_rangeArea.SetCollisionLayerValue(1, true);
				_rangeArea.SetCollisionMaskValue(2, true); // Enemy layer
			}
			
			GD.Print($"{LogPrefix} {Name} set to active mode - all functionality enabled");
		}
	}

	public override void _ExitTree()
	{
		// Unregister this building from the registry when it's destroyed (but not for preview buildings)
		if (!IsPreview)
		{
			BuildingRegistry.Instance.UnregisterBuilding(this);
			// Notify BuildingSelectionManager about building destruction
			BuildingSelectionManager.Instance.OnBuildingDestroyed(this);
		}
		base._ExitTree();
	}
	
	// ===== BUILDING SELECTION SYSTEM =====
	
	private void SetupInputHandling()
	{
		// Enable input handling for building selection
		if (!IsPreview)
		{
			InputPickable = true;
		}
	}
	
	public override void _Input(InputEvent @event)
	{
		if (IsPreview) return;
		
		if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left)
			{
				// Check if player is not in build mode
				var player = GetTree().GetFirstNodeInGroup("player") as Player.Player;
				if (player != null && player._buildingBuilder.IsInBuildMode)
				{
					return; // Don't allow selection during build mode
				}
				
				// Check if the click is within the building's collision area
				if (IsPointInCollisionArea(mouseButton.GlobalPosition))
				{
					GD.Print($"{LogPrefix} {Name} click is within collision area - handling selection");
					HandleBuildingSelection();
					GetViewport().SetInputAsHandled();
				}
				// If click is outside collision area, don't handle it - let it propagate
			}
		}
	}
	
	private bool IsPointInCollisionArea(Vector2 globalPoint)
	{
		// Check if the point is within the building's collision radius
		var distance = GlobalPosition.DistanceTo(globalPoint);
		return distance <= CollisionRadius;
	}
	
	private void OnAreaInputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
	{
		if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left)
			{
				GD.Print($"{LogPrefix} {Name} area received left click at {mouseButton.GlobalPosition}");
				
				// Check if player is not in build mode
				var player = GetTree().GetFirstNodeInGroup("player") as Player.Player;
				if (player != null && player._buildingBuilder.IsInBuildMode)
				{
					GD.Print($"{LogPrefix} {Name} ignoring click - player is in build mode");
					return;
				}
				
				HandleBuildingSelection();
				GetViewport().SetInputAsHandled();
			}
		}
	}
	
	private void HandleBuildingSelection()
	{
		GD.Print($"{LogPrefix} Building {Name} clicked at {GlobalPosition}");
		
		// Use BuildingSelectionManager to handle the selection
		BuildingSelectionManager.Instance.SelectBuilding(this);
	}
	
	public void SetSelected(bool selected)
	{
		if (IsSelected == selected) return;
		
		IsSelected = selected;
		
		if (selected)
		{
			// Show selection visual feedback
			ShowSelectionVisuals();
			GD.Print($"{LogPrefix} Building {Name} selected");
		}
		else
		{
			// Hide selection visual feedback
			HideSelectionVisuals();
			GD.Print($"{LogPrefix} Building {Name} deselected");
		}
	}
	
	public void ToggleSelection()
	{
		SetSelected(!IsSelected);
	}
	
	private void ShowSelectionVisuals()
	{
		// Create selection border if it doesn't exist
		if (_selectionBorder == null)
		{
			CreateSelectionBorder();
		}
		
		// Animate selection border appearance
		if (_selectionBorder != null && _selectionTween != null)
		{
			_selectionBorder.Visible = true;
			_selectionBorder.DefaultColor = new Color(Colors.Black.R, Colors.Black.G, Colors.Black.B, 0.0f);
			
			_selectionTween.Kill();
			_selectionTween = CreateTween();
			_selectionTween.TweenProperty(_selectionBorder, "default_color", Colors.Black, 0.2f);
			_selectionTween.Parallel().TweenProperty(this, "modulate", new Color(1.1f, 1.1f, 1.1f, 1.0f), 0.2f);
		}
		
		// Show range circle with animation
		ShowRangeAnimated();
	}
	
	private void HideSelectionVisuals()
	{
		// Animate selection border disappearance
		if (_selectionBorder != null && _selectionTween != null)
		{
			_selectionTween.Kill();
			_selectionTween = CreateTween();
			_selectionTween.TweenProperty(_selectionBorder, "default_color", new Color(Colors.Black.R, Colors.Black.G, Colors.Black.B, 0.0f), 0.15f);
			_selectionTween.TweenCallback(Callable.From(() => _selectionBorder.Visible = false));
			_selectionTween.Parallel().TweenProperty(this, "modulate", _originalModulate, 0.15f);
		}
		
		// Hide range circle with animation
		HideRangeAnimated();
	}
	
	private void CreateSelectionBorder()
	{
		_selectionBorder = new Line2D();
		_selectionBorder.Width = 2.0f;
		_selectionBorder.DefaultColor = Colors.Black;
		_selectionBorder.Visible = false;
		
		// Create a square border around the building
		float borderSize = CollisionRadius + 5.0f;
		_selectionBorder.AddPoint(new Vector2(-borderSize, -borderSize));
		_selectionBorder.AddPoint(new Vector2(borderSize, -borderSize));
		_selectionBorder.AddPoint(new Vector2(borderSize, borderSize));
		_selectionBorder.AddPoint(new Vector2(-borderSize, borderSize));
		_selectionBorder.AddPoint(new Vector2(-borderSize, -borderSize)); // Close the square
		
		AddChild(_selectionBorder);
	}
	
	// ===== UPGRADE LEVEL VISUALS =====
	// Visual upgrade indicators removed - only color tinting is used
	
	private void CreateAnimations()
	{
		if (IsPreview) return; // Don't create animations for preview buildings
		
		// Create tweens for smooth animations
		_selectionTween = CreateTween();
		_rangeTween = CreateTween();
		
		GD.Print($"{LogPrefix} {Name} animations created");
	}
	
	private void ShowRangeAnimated()
	{
		if (_rangeCircle != null && _rangeTween != null)
		{
			_rangeCircle.Visible = true;
			_rangeCircle.DefaultColor = new Color(0.2f, 0.8f, 0.2f, 0.0f);
			
			_rangeTween.Kill();
			_rangeTween = CreateTween();
			_rangeTween.TweenProperty(_rangeCircle, "default_color", new Color(0.2f, 0.8f, 0.2f, 0.6f), 0.3f);
			_showingRange = true;
		}
	}
	
	private void HideRangeAnimated()
	{
		if (_rangeCircle != null && _rangeTween != null)
		{
			_rangeTween.Kill();
			_rangeTween = CreateTween();
			_rangeTween.TweenProperty(_rangeCircle, "default_color", new Color(0.2f, 0.8f, 0.2f, 0.0f), 0.2f);
			_rangeTween.TweenCallback(Callable.From(() => { _rangeCircle.Visible = false; _showingRange = false; }));
		}
	}
	
	public void UpdateUpgradeVisuals()
	{
		if (IsPreview) return;
		
		if (UpgradeLevel > 0)
		{
			// Apply upgrade color tint
			ApplyUpgradeColorTint();
		}
		else
		{
			// Remove upgrade color tint
			Modulate = _originalModulate;
		}
		
		GD.Print($"{LogPrefix} {Name} upgrade visuals updated for level {UpgradeLevel} (color tint only)");
	}
	
	private void ApplyUpgradeColorTint()
	{
		// Apply subtle color tint based on upgrade level
		Color upgradeTint = UpgradeLevel switch
		{
			1 => new Color(1.1f, 1.0f, 1.0f, 1.0f), // Slightly more red
			2 => new Color(1.0f, 1.1f, 1.0f, 1.0f), // Slightly more green
			3 => new Color(1.0f, 1.0f, 1.1f, 1.0f), // Slightly more blue
			_ => new Color(1.2f, 1.2f, 1.0f, 1.0f)  // Golden tint for higher levels
		};
		
		// Only apply if not selected (selection has its own modulation)
		if (!IsSelected)
		{
			Modulate = upgradeTint;
		}
	}
}
