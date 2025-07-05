using Godot;
using Game.Di;
using Game.Presentation.Enemies;
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
			GD.Print($"{LogPrefix} {Name} signals connected");
		}
		else
		{
			GD.PrintErr($"{LogPrefix} {Name} failed to connect signals - _rangeArea is null");
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
		
		Enemy? closestEnemy = null;
		float closestDistance = float.MaxValue;
		
		foreach (Enemy enemy in _enemiesInRange)
		{
			if (enemy == null || !IsInstanceValid(enemy)) continue;
			
			float distance = GlobalPosition.DistanceTo(enemy.GlobalPosition);
			if (distance < closestDistance)
			{
				closestDistance = distance;
				closestEnemy = enemy;
			}
		}
		
		return closestEnemy;
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
		}
	}
}
