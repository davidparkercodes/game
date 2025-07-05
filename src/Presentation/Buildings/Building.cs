using Godot;
using Game.Infrastructure.DI;

namespace Game.Presentation.Buildings;

public partial class Building : StaticBody2D
{
	[Export] public int Cost { get; set; } = 10;
	[Export] public int Damage { get; set; } = 10;
	[Export] public float Range { get; set; } = 150.0f;
	[Export] public float FireRate { get; set; } = 1.0f;
	[Export] public PackedScene BulletScene;

	protected Timer _fireTimer;
	protected Area2D _rangeArea;
	protected CollisionShape2D _rangeCollision;
	protected bool _showingRange = false;
	private Line2D _rangeCircle;

	public override void _Ready()
	{
		_fireTimer = GetNode<Timer>("Timer");
		_rangeArea = GetNode<Area2D>("Area2D");
		_rangeCollision = _rangeArea.GetNode<CollisionShape2D>("CollisionShape2D");
		
		InitializeStats();
		CreateRangeVisual();
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
}
