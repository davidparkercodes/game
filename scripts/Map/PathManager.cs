using Godot;
using System.Collections.Generic;

public partial class PathManager : Node2D
{
	[Export] public Curve2D LanePath;
	[Export] public float PathWidth = 64.0f;
	
	public static PathManager Instance { get; private set; }
	
	private Path2D _pathNode;
	private List<Vector2> _pathPoints = new List<Vector2>();

	public override void _Ready()
	{
		Instance = this;
		CreateDefaultPath();
		GD.Print("🗺️ PathManager ready with lane path");
	}

	private void CreateDefaultPath()
	{
		// Create a simple straight lane from top to bottom of the screen
		LanePath = new Curve2D();
		
		// Define path points for a single lane (adjust these based on your screen size)
		_pathPoints.Add(new Vector2(200, -50));  // Start above screen
		_pathPoints.Add(new Vector2(200, 100));  // Entry point
		_pathPoints.Add(new Vector2(200, 300));  // Middle
		_pathPoints.Add(new Vector2(200, 500));  // End point
		_pathPoints.Add(new Vector2(200, 650));  // Exit below screen
		
		// Add points to the curve
		foreach (var point in _pathPoints)
		{
			LanePath.AddPoint(point);
		}
		
		// Create visual Path2D node for debugging
		_pathNode = new Path2D();
		_pathNode.Curve = LanePath;
		AddChild(_pathNode);
		
		GD.Print($"🛤️ Created lane path with {_pathPoints.Count} points");
	}

	public Vector2 GetPathPosition(float progress)
	{
		if (LanePath == null) return Vector2.Zero;
		
		// Clamp progress between 0 and 1
		progress = Mathf.Clamp(progress, 0.0f, 1.0f);
		
		// Sample the curve at the given progress
		return LanePath.SampleBaked(progress * LanePath.GetBakedLength());
	}

	public Vector2 GetPathDirection(float progress)
	{
		if (LanePath == null) return Vector2.Down;
		
		progress = Mathf.Clamp(progress, 0.0f, 1.0f);
		
		// Get a slightly ahead position to calculate direction
		float deltaProgress = 0.01f;
		float nextProgress = Mathf.Clamp(progress + deltaProgress, 0.0f, 1.0f);
		
		Vector2 currentPos = GetPathPosition(progress);
		Vector2 nextPos = GetPathPosition(nextProgress);
		
		return (nextPos - currentPos).Normalized();
	}

	public float GetPathLength()
	{
		return LanePath?.GetBakedLength() ?? 0.0f;
	}

	public Vector2 GetSpawnPosition()
	{
		// Return the first point with some randomization within lane width
		if (_pathPoints.Count > 0)
		{
			Vector2 spawnPoint = _pathPoints[0];
			float randomOffset = (GD.Randf() - 0.5f) * PathWidth;
			return new Vector2(spawnPoint.X + randomOffset, spawnPoint.Y);
		}
		return new Vector2(200, -50);
	}

	public Vector2 GetEndPosition()
	{
		// Return the last point
		if (_pathPoints.Count > 0)
		{
			return _pathPoints[_pathPoints.Count - 1];
		}
		return new Vector2(200, 650);
	}

	public override void _Draw()
	{
		// Draw the path for visualization
		if (LanePath != null && _pathPoints.Count > 1)
		{
			for (int i = 0; i < _pathPoints.Count - 1; i++)
			{
				DrawLine(_pathPoints[i], _pathPoints[i + 1], Colors.Yellow, 2.0f);
			}
			
			// Draw path width indicators
			foreach (var point in _pathPoints)
			{
				DrawCircle(point, 4.0f, Colors.Red);
			}
		}
	}
}
