// scripts/Buildings/Building.cs
using Godot;
using System;

public partial class Building : StaticBody2D
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
	private Color _selectedRangeColor = new Color(1.0f, 1.0f, 0.2f, 0.4f); // Yellow for selected building
	
	public static Building SelectedBuilding { get; private set; }

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
		
		InputPickable = true;
		InputEvent += OnBuildingInput;
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;

		GD.Print($"[BUILDING] {GetType().Name}: Damage={Damage}, FireRate={FireRate}, Range={Range}, InputPickable={InputPickable}");
	}

	protected virtual void ConfigureStats() { }
	
	public void InitializeStats()
	{
		ConfigureStats();
	}
	
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
	
	public void SelectBuilding()
	{
		if (SelectedBuilding != null && SelectedBuilding != this)
		{
			SelectedBuilding.DeselectBuilding();
		}
		
		_isSelected = true;
		SelectedBuilding = this;
		ShowRange();
		
		ShowBuildingStatsInHUD();
		
		GD.Print($"🎯 Selected {GetType().Name} - Range: {Range}, Damage: {Damage}, Cost: ${Cost}");
	}
	
	public void DeselectBuilding()
	{
		_isSelected = false;
		if (SelectedBuilding == this)
			SelectedBuilding = null;
		HideRange();
		
		HideBuildingStatsInHUD();
	}
	
	public void ToggleSelection()
	{
		GD.Print($"🔄 Toggling selection for {GetType().Name}. Currently selected: {_isSelected}");
		if (_isSelected)
			DeselectBuilding();
		else
			SelectBuilding();
	}
	
	public bool IsSelected => _isSelected;
	
	protected virtual void PlayShootSound()
	{
		if (SoundManager.Instance != null)
		{
			SoundManager.Instance.PlaySound("basic_turret_shoot");
		}
	}
	
	protected virtual string GetImpactSoundKey()
	{
		return "basic_bullet_impact";
	}
	
	private void ShowBuildingStatsInHUD()
	{
		if (GameManager.Instance?.Hud != null)
		{
			string buildingName = GetType().Name.Replace("Building", "");
			GameManager.Instance.Hud.ShowBuildingStats(buildingName, Cost, Damage, Range, FireRate);
		}
		
		CancelPlayerBuildMode();
	}
	
	private void CancelPlayerBuildMode()
	{
		var player = GetTree().GetFirstNodeInGroup("player") as Player;
		if (player != null)
		{
			player.CancelBuildMode();
		}
	}
	
	private void HideBuildingStatsInHUD()
	{
		if (GameManager.Instance?.Hud != null)
		{
			GameManager.Instance.Hud.HideBuildingStats();
		}
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
			var currentRangeColor = _isSelected ? _selectedRangeColor : _rangeColor;
			DrawArc(Vector2.Zero, Range, 0, Mathf.Tau, 64, currentRangeColor, 2.0f);
			var fillColor = new Color(currentRangeColor.R, currentRangeColor.G, currentRangeColor.B, currentRangeColor.A * 0.5f);
			DrawCircle(Vector2.Zero, Range, fillColor);
		}
		
		if (_isSelected)
		{
			DrawArc(Vector2.Zero, 20, 0, Mathf.Tau, 32, Colors.Yellow, 3.0f);
		}
	}

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

		if (bullet is Bullet b)
		{
			b.SetBulletVelocity(direction * bulletSpeed);
			b.SetImpactSound(GetImpactSoundKey());
		}

		GetTree().Root.AddChild(bullet);
		
		PlayShootSound();
	}
	
	private void OnBuildingInput(Node viewport, InputEvent @event, long shapeIdx)
	{
		if (@event is InputEventMouseButton button && button.Pressed)
		{
				GD.Print($"🖱️ Building received mouse input: {button.ButtonIndex}");
				if (button.ButtonIndex == MouseButton.Left)
				{
					GD.Print($"🎯 Left click on {GetType().Name} building");
					ToggleSelection();
				}
		}
	}
	
	private void OnMouseEntered()
	{
		if (!_isSelected)
		{
			ShowRange();
		}
	}
	
	private void OnMouseExited()
	{
		if (!_isSelected)
		{
			HideRange();
		}
	}
	
}
