using Godot;
using System.Collections.Generic;

[Tool]
public partial class PathManager : Node2D
{
	private Resource _currentLevelResource;
	[Export] 
	public Resource CurrentLevel 
	{ 
		get => _currentLevelResource;
		set 
		{
			_currentLevelResource = value;
			
			if (Engine.IsEditorHint())
			{
				UpdateEditorVisualization();
			}
		}
	}
	
	public Curve2D LanePath;
	[Export] public float PathWidth = 64.0f;
	
	public static PathManager Instance { get; private set; }
	
	// Helper methods to access resource properties dynamically
	private string GetLevelName()
	{
		return _currentLevelResource?.Get("LevelName").AsString() ?? "Unknown Level";
	}
	
	private Godot.Collections.Array<Vector2> GetPathPoints()
	{
		return _currentLevelResource?.Get("PathPoints").AsGodotArray<Vector2>() ?? new Godot.Collections.Array<Vector2>();
	}
	
	private float GetPathWidthFromLevel()
	{
		return _currentLevelResource?.Get("PathWidth").AsSingle() ?? 64.0f;
	}
	
	private Color GetPathColor()
	{
		return _currentLevelResource?.Get("PathColor").AsColor() ?? Colors.Yellow;
	}
	
	private Curve2D GetPathCurve()
	{
		return _currentLevelResource?.Get("PathCurve").As<Curve2D>();
	}
	
	private Path2D _pathNode;
	private Line2D _pathLine;
	private List<Vector2> _pathPoints = new List<Vector2>();

	public override void _Ready()
	{
		if (!Engine.IsEditorHint())
		{
			Instance = this;
		}
		
		if (_currentLevelResource != null)
		{
			if (Engine.IsEditorHint())
			{
				UpdateEditorVisualization();
			}
			else
			{
				LoadLevelPath();
			}
		}
		else if (!Engine.IsEditorHint())
		{
			CreateDefaultPath();
		}
		
		if (!Engine.IsEditorHint())
		{
			GD.Print("🗺️ PathManager ready with lane path");
		}
	}
	
	private void UpdateEditorVisualization()
	{
		if (_currentLevelResource == null) return;
		
		// Clear existing editor visualization
		ClearExistingPath();
		
		// Load path data from level
		_pathPoints.Clear();
		var pathPoints = GetPathPoints();
		foreach (var point in pathPoints)
		{
			_pathPoints.Add(point);
		}
		
		// Create path curve
		LanePath = new Curve2D();
		foreach (var point in _pathPoints)
		{
			LanePath.AddPoint(point);
		}
		
		// Force redraw for editor visualization
		QueueRedraw();
	}
	
	private void CreateEditorVisualization()
	{
		// In editor mode, we rely on _Draw() method for visualization
		// This keeps the editor lightweight and responsive
	}
	
	private void LoadLevelPath()
	{
		// Clear existing path components
		ClearExistingPath();
		
		// Use the level data
		LanePath = GetPathCurve();
		PathWidth = GetPathWidthFromLevel();
		
		// Convert Godot.Collections.Array to List
		_pathPoints.Clear();
		var pathPoints = GetPathPoints();
		foreach (var point in pathPoints)
		{
			_pathPoints.Add(point);
		}
		
		// If PathCurve is null, create it from points
		if (LanePath == null && _pathPoints.Count > 0)
		{
			LanePath = new Curve2D();
			foreach (var point in _pathPoints)
			{
				LanePath.AddPoint(point);
			}
			// Note: Can't set PathCurve back to resource in this approach
		}
		
		// Create visual representation
		CreateVisualPath();
		
		GD.Print($"🛤️ Loaded level path: {GetLevelName()} with {_pathPoints.Count} points");
	}

	private void CreateDefaultPath()
	{
		// Clear existing path components
		ClearExistingPath();
		
		// Create a zigzag path that moves across the screen
		LanePath = new Curve2D();
		_pathPoints.Clear();
		
		// Define zigzag path points (adjust these based on your screen size)
		_pathPoints.Add(new Vector2(100, -50));   // Start above screen (left side)
		_pathPoints.Add(new Vector2(100, 150));   // Entry point
		_pathPoints.Add(new Vector2(400, 150));   // Move right
		_pathPoints.Add(new Vector2(400, 300));   // Move down
		_pathPoints.Add(new Vector2(150, 300));   // Move left
		_pathPoints.Add(new Vector2(150, 450));   // Move down
		_pathPoints.Add(new Vector2(500, 450));   // Move right
		_pathPoints.Add(new Vector2(500, 600));   // Move down
		_pathPoints.Add(new Vector2(300, 600));   // Move left
		_pathPoints.Add(new Vector2(300, 750));   // Exit below screen
		
		// Add points to the curve
		foreach (var point in _pathPoints)
		{
			LanePath.AddPoint(point);
		}
		
		// Create visual representation
		CreateVisualPath();
		
		GD.Print($"🛤️ Created default zigzag lane path with {_pathPoints.Count} points");
	}
	
	private void CreateVisualPath()
	{
		// Create visual Path2D node
		_pathNode = new Path2D();
		_pathNode.Curve = LanePath;
		_pathNode.Name = "EnemyPath";
		AddChild(_pathNode);
		
		// Add a Line2D for better visibility
		_pathLine = new Line2D();
		_pathLine.Name = "PathLine";
		_pathLine.Width = 4.0f;
		_pathLine.DefaultColor = GetPathColor();
		foreach (var point in _pathPoints)
		{
			_pathLine.AddPoint(point);
		}
		AddChild(_pathLine);
	}
	
	private void ClearExistingPath()
	{
		// Remove existing path nodes
		if (_pathNode != null && IsInstanceValid(_pathNode))
		{
			_pathNode.QueueFree();
			_pathNode = null;
		}
		
		if (_pathLine != null && IsInstanceValid(_pathLine))
		{
			_pathLine.QueueFree();
			_pathLine = null;
		}
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
		// Draw the path for visualization (both editor and runtime)
		if (_pathPoints.Count > 1)
		{
			Color pathColor = GetPathColor();
			float lineWidth = Engine.IsEditorHint() ? 3.0f : 2.0f;
			
			// Draw path lines
			for (int i = 0; i < _pathPoints.Count - 1; i++)
			{
				DrawLine(_pathPoints[i], _pathPoints[i + 1], pathColor, lineWidth);
			}
			
			// Draw path width indicators in editor
			if (Engine.IsEditorHint())
			{
				// Draw path width as semi-transparent area
				for (int i = 0; i < _pathPoints.Count - 1; i++)
				{
					Vector2 direction = (_pathPoints[i + 1] - _pathPoints[i]).Normalized();
					Vector2 perpendicular = new Vector2(-direction.Y, direction.X) * (PathWidth / 2);
					
					Vector2[] widthQuad = {
						_pathPoints[i] + perpendicular,
						_pathPoints[i] - perpendicular,
						_pathPoints[i + 1] - perpendicular,
						_pathPoints[i + 1] + perpendicular
					};
					
					Color widthColor = pathColor;
					widthColor.A = 0.3f; // Semi-transparent
					DrawColoredPolygon(widthQuad, widthColor);
				}
				
				// Draw control points
				for (int i = 0; i < _pathPoints.Count; i++)
				{
					Color pointColor = i == 0 ? Colors.Green : (i == _pathPoints.Count - 1 ? Colors.Red : Colors.White);
					DrawCircle(_pathPoints[i], 6.0f, pointColor);
					DrawCircle(_pathPoints[i], 4.0f, Colors.Black);
				}
				
				// Draw spawn and end labels
				if (_pathPoints.Count > 0)
				{
					// Note: DrawString requires a font, so we'll use simple shapes instead
					// Draw larger circles for spawn (green) and end (red) points
					DrawCircle(_pathPoints[0], 10.0f, Colors.Green.Lerp(Colors.White, 0.3f));
					DrawCircle(_pathPoints[_pathPoints.Count - 1], 10.0f, Colors.Red.Lerp(Colors.White, 0.3f));
				}
			}
			else
			{
				// Runtime: Just draw simple point indicators
				foreach (var point in _pathPoints)
				{
					DrawCircle(point, 4.0f, Colors.Red);
				}
			}
		}
	}
}
