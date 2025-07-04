// scripts/Turrets/Turret.cs
using Godot;
using System;

public partial class Turret : StaticBody2D
{
	[Export] public PackedScene BulletScene;
	[Export] public int Cost     = 10;
	[Export] public int Damage   = 10;
	[Export] public float Range  = 150.0f;
	[Export] public float FireRate = 1.0f;

	private Timer  _fireTimer;
	private Area2D _detectionArea;
	private Node2D _currentTarget;
	private bool _showRange = false;
	private Color _rangeColor = new Color(0.2f, 0.8f, 1.0f, 0.3f); // Light blue with transparency

	public override void _Ready()
	{

		_fireTimer     = GetNode<Timer>("Timer");
		_detectionArea = GetNode<Area2D>("Area2D");

		_detectionArea.BodyEntered += OnBodyEntered;
		_detectionArea.BodyExited  += OnBodyExited;

		ConfigureStats(); 

		var shape = _detectionArea.GetNode<CollisionShape2D>("CollisionShape2D");
		if (shape.Shape is CircleShape2D circle) circle.Radius = Range;

		_fireTimer.WaitTime = FireRate;
		_fireTimer.Timeout += OnFireTimerTimeout;
		
		// Enable mouse input for hover detection
		InputEvent += OnTurretInput;

		GD.Print($"[TURRET] {GetType().Name}: Damage={Damage}, FireRate={FireRate}, Range={Range}");
	}

	// ---------- OVERRIDES ----------------------------------------------------
	protected virtual void ConfigureStats() { }
	
	// Public method to manually configure stats before _Ready() is called
	public void InitializeStats()
	{
		ConfigureStats();
	}
	
	// Range visualization methods
	public void ShowRange()
	{
		_showRange = true;
		QueueRedraw();
	}
	
	public void HideRange()
	{
		_showRange = false;
		QueueRedraw();
	}
	
	public void SetRangeColor(Color color)
	{
		_rangeColor = color;
		if (_showRange)
			QueueRedraw();
	}

	public override void _Process(double delta)
	{
		if (_currentTarget != null && IsInstanceValid(_currentTarget))
			LookAt(_currentTarget.GlobalPosition);
	}
	
	public override void _Draw()
	{
		if (_showRange)
		{
			// Draw range circle
			DrawArc(Vector2.Zero, Range, 0, Mathf.Tau, 64, _rangeColor, 2.0f);
			
			// Draw filled circle with transparency
			var fillColor = new Color(_rangeColor.R, _rangeColor.G, _rangeColor.B, _rangeColor.A * 0.5f);
			DrawCircle(Vector2.Zero, Range, fillColor);
		}
	}

	// ---------- SIGNAL CALLBACKS --------------------------------------------
	private void OnBodyEntered(Node2D body)
	{
		if (body.IsInGroup("enemies") && _currentTarget == null)
		{
			_currentTarget = body;
			Callable.From(() => _fireTimer.Start()).CallDeferred();
		}
	}

	private void OnBodyExited(Node2D body)
	{
		if (body == _currentTarget)
		{
			_currentTarget = null;
			_fireTimer.Stop();
		}
	}

	private void OnFireTimerTimeout()
	{
		if (_currentTarget == null || !IsInstanceValid(_currentTarget)) return;

		Vector2 toTarget       = _currentTarget.GlobalPosition - GlobalPosition;
		Vector2 targetVelocity = _currentTarget is CharacterBody2D cb ? cb.Velocity : Vector2.Zero;

		const float bulletSpeed = 900;
		float distance          = toTarget.Length();
		float timeToTarget      = distance / bulletSpeed;

		Vector2 predictedPosition = _currentTarget.GlobalPosition + targetVelocity * timeToTarget;
		Vector2 direction         = (predictedPosition - GlobalPosition).Normalized();

		var bullet = BulletScene.Instantiate<Area2D>();
		bullet.GlobalPosition = GlobalPosition;

		if (bullet is Bullet b) b.SetBulletVelocity(direction * bulletSpeed);

		GetTree().Root.AddChild(bullet);
	}
	
	private void OnTurretInput(Node viewport, InputEvent @event, long shapeIdx)
	{
		if (@event is InputEventMouseMotion)
		{
			// Show range on hover
			ShowRange();
		}
		else if (@event is InputEventMouseButton button && !button.Pressed)
		{
			// Hide range when mouse leaves
			HideRange();
		}
	}
	
}
