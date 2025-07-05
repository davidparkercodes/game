using Godot;
using System;
using Game.Infrastructure.Enemies.Services;
namespace Game.Presentation.Components;

public partial class PathFollower : Node
{
	[Export] public float Speed = 60.0f;
	[Export] public bool AutoStart = true;
	
	[Signal] public delegate void PathCompletedEventHandler();
	[Signal] public delegate void PathProgressChangedEventHandler(float progress);
	
	private Node2D _body;
	private float _pathProgress = 0.0f;
	private bool _isFollowingPath = false;
	private PathService _pathService;

	public float PathProgress 
	{ 
		get => _pathProgress; 
		set => _pathProgress = Mathf.Clamp(value, 0.0f, 1.0f);
	}

	public bool IsFollowingPath => _isFollowingPath;

	public override void _Ready()
	{
		_body = GetParent<Node2D>();
		_pathService = PathService.Instance;
		
		if (_body == null)
		{
			GD.PrintErr("❌ PathFollower must be a child of Node2D (or derived class)");
			return;
		}

		if (_pathService == null)
		{
			GD.PrintErr("❌ PathService instance not found");
			return;
		}

		if (AutoStart)
		{
			StartFollowingPath();
		}
		
		GD.Print("🚶 PathFollower ready");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_isFollowingPath || _pathService == null || _body == null)
			return;

		float pathLength = _pathService.GetPathLength();
		if (pathLength <= 0) return;

		float distanceToMove = Speed * (float)delta;
		float progressDelta = distanceToMove / pathLength;
		
		_pathProgress += progressDelta;
		
		if (_pathProgress >= 1.0f)
		{
			_pathProgress = 1.0f;
			_isFollowingPath = false;
			
			_body.GlobalPosition = _pathService.GetPathPosition(_pathProgress);
			
			EmitSignal(SignalName.PathCompleted);
			GD.Print("🏁 Path completed");
			return;
		}
		
		Vector2 targetPosition = _pathService.GetPathPosition(_pathProgress);
		Vector2 direction = _pathService.GetPathDirection(_pathProgress);
		
		_body.GlobalPosition = targetPosition;
		
		EmitSignal(SignalName.PathProgressChanged, _pathProgress);
	}

	public void StartFollowingPath(float startProgress = 0.0f)
	{
		if (_pathService == null)
		{
			GD.PrintErr("❌ Cannot start following path: PathService not available");
			return;
		}

		_pathProgress = startProgress;
		_isFollowingPath = true;
		
		if (_body != null)
		{
			_body.GlobalPosition = _pathService.GetPathPosition(_pathProgress);
		}
		
		GD.Print($"🚀 Started following path from progress: {_pathProgress:F2}");
	}

	public void StopFollowingPath()
	{
		_isFollowingPath = false;
		GD.Print("⏹️ Stopped following path");
	}

	public void SetSpeed(float newSpeed)
	{
		Speed = Mathf.Max(0.0f, newSpeed);
	}

	public Vector2 GetCurrentPathPosition()
	{
		return _pathService?.GetPathPosition(_pathProgress) ?? Vector2.Zero;
	}

	public Vector2 GetCurrentPathDirection()
	{
		return _pathService?.GetPathDirection(_pathProgress) ?? Vector2.Down;
	}

	public float GetDistanceToEnd()
	{
		if (_pathService == null) return 0.0f;
		
		float remainingProgress = 1.0f - _pathProgress;
		return remainingProgress * _pathService.GetPathLength();
	}
}
