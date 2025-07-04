using Godot;

public partial class PathFollower : Node
{
	[Export] public float Speed = 60.0f;
	[Export] public bool AutoStart = true;
	
	[Signal] public delegate void PathCompletedEventHandler();
	[Signal] public delegate void PathProgressChangedEventHandler(float progress);
	
	private Node2D _body;
	private float _pathProgress = 0.0f;
	private bool _isFollowingPath = false;
	private PathManager _pathManager;

	public float PathProgress 
	{ 
		get => _pathProgress; 
		set => _pathProgress = Mathf.Clamp(value, 0.0f, 1.0f);
	}

	public bool IsFollowingPath => _isFollowingPath;

	public override void _Ready()
	{
		_body = GetParent<Node2D>();
		_pathManager = PathManager.Instance;
		
		if (_body == null)
		{
			GD.PrintErr("❌ PathFollower must be a child of Node2D (or derived class)");
			return;
		}

		if (_pathManager == null)
		{
			GD.PrintErr("❌ PathManager instance not found");
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
		if (!_isFollowingPath || _pathManager == null || _body == null)
			return;

		// Calculate distance to move this frame
		float pathLength = _pathManager.GetPathLength();
		if (pathLength <= 0) return;

		float distanceToMove = Speed * (float)delta;
		float progressDelta = distanceToMove / pathLength;
		
		// Update progress
		_pathProgress += progressDelta;
		
		// Check if we've reached the end
		if (_pathProgress >= 1.0f)
		{
			_pathProgress = 1.0f;
			_isFollowingPath = false;
			
			// Move to final position
			_body.GlobalPosition = _pathManager.GetPathPosition(_pathProgress);
			
			EmitSignal(SignalName.PathCompleted);
			GD.Print("🏁 Path completed");
			return;
		}
		
		// Get current position and direction from path
		Vector2 targetPosition = _pathManager.GetPathPosition(_pathProgress);
		Vector2 direction = _pathManager.GetPathDirection(_pathProgress);
		
		// Move directly to target position without collision detection
		// This allows enemies to pass through each other in tower defense style
		_body.GlobalPosition = targetPosition;
		
		// Emit progress signal
		EmitSignal(SignalName.PathProgressChanged, _pathProgress);
	}

	public void StartFollowingPath(float startProgress = 0.0f)
	{
		if (_pathManager == null)
		{
			GD.PrintErr("❌ Cannot start following path: PathManager not available");
			return;
		}

		_pathProgress = startProgress;
		_isFollowingPath = true;
		
		// Set initial position
		if (_body != null)
		{
			_body.GlobalPosition = _pathManager.GetPathPosition(_pathProgress);
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
		return _pathManager?.GetPathPosition(_pathProgress) ?? Vector2.Zero;
	}

	public Vector2 GetCurrentPathDirection()
	{
		return _pathManager?.GetPathDirection(_pathProgress) ?? Vector2.Down;
	}

	public float GetDistanceToEnd()
	{
		if (_pathManager == null) return 0.0f;
		
		float remainingProgress = 1.0f - _pathProgress;
		return remainingProgress * _pathManager.GetPathLength();
	}
}
