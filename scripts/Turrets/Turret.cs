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
	private bool _isSelected = false;
	private Color _rangeColor = new Color(0.2f, 0.8f, 1.0f, 0.3f); // Light blue with transparency
	private Color _selectedRangeColor = new Color(1.0f, 1.0f, 0.2f, 0.4f); // Yellow for selected turret
	
	public static Turret SelectedTurret { get; private set; }

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
		InputPickable = true; // Enable input detection for StaticBody2D
		InputEvent += OnTurretInput;
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;

		GD.Print($"[TURRET] {GetType().Name}: Damage={Damage}, FireRate={FireRate}, Range={Range}, InputPickable={InputPickable}");
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
	
	public void SelectTurret()
	{
		// Deselect previous turret
		if (SelectedTurret != null && SelectedTurret != this)
		{
			SelectedTurret.DeselectTurret();
		}
		
		_isSelected = true;
		SelectedTurret = this;
		ShowRange();
		GD.Print($"🎯 Selected {GetType().Name} - Range: {Range}, Damage: {Damage}, Cost: ${Cost}");
	}
	
	public void DeselectTurret()
	{
		_isSelected = false;
		if (SelectedTurret == this)
			SelectedTurret = null;
		HideRange();
	}
	
	public void ToggleSelection()
	{
		GD.Print($"🔄 Toggling selection for {GetType().Name}. Currently selected: {_isSelected}");
		if (_isSelected)
			DeselectTurret();
		else
			SelectTurret();
	}
	
	public bool IsSelected => _isSelected;

	public override void _Process(double delta)
	{
		if (_currentTarget != null && IsInstanceValid(_currentTarget))
			LookAt(_currentTarget.GlobalPosition);
	}
	
	public override void _Draw()
	{
		if (_showRange)
		{
			// Use selected color if this turret is selected
			var currentRangeColor = _isSelected ? _selectedRangeColor : _rangeColor;
			
			// Draw range circle
			DrawArc(Vector2.Zero, Range, 0, Mathf.Tau, 64, currentRangeColor, 2.0f);
			
			// Draw filled circle with transparency
			var fillColor = new Color(currentRangeColor.R, currentRangeColor.G, currentRangeColor.B, currentRangeColor.A * 0.5f);
			DrawCircle(Vector2.Zero, Range, fillColor);
		}
		
		// Draw selection indicator
		if (_isSelected)
		{
			// Draw a small circle around the turret to show it's selected
			DrawArc(Vector2.Zero, 20, 0, Mathf.Tau, 32, Colors.Yellow, 3.0f);
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
		if (@event is InputEventMouseButton button && button.Pressed)
		{
			GD.Print($"🖱️ Turret received mouse input: {button.ButtonIndex}");
			if (button.ButtonIndex == MouseButton.Left)
			{
				// Left click to select/deselect turret
				GD.Print($"🎯 Left click on {GetType().Name} turret");
				ToggleSelection();
			}
		}
	}
	
	private void OnMouseEntered()
	{
		// Show range on hover (only if not selected)
		if (!_isSelected)
		{
			ShowRange();
		}
	}
	
	private void OnMouseExited()
	{
		// Hide range when mouse leaves (only if not selected)
		if (!_isSelected)
		{
			HideRange();
		}
	}
	
}
