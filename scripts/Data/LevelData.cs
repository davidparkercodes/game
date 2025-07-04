using Godot;
using System.Collections.Generic;

public partial class LevelData : Resource
{
	[Export] public string LevelName = "Level 1";
	[Export] public string Description = "A simple level";
	[Export] public Curve2D PathCurve;
	[Export] public Godot.Collections.Array<Vector2> PathPoints = new();
	[Export] public float PathWidth = 64.0f;
	[Export] public Vector2 SpawnPoint = new Vector2(100, -50);
	[Export] public Vector2 EndPoint = new Vector2(300, 750);
	
	// Level settings
	[Export] public int InitialMoney = 100;
	[Export] public int InitialLives = 20;
	[Export] public Color PathColor = Colors.Yellow;
	
	public LevelData()
	{
		// Default constructor for Godot
	}
	
	public void CreatePathFromPoints()
	{
		if (PathPoints.Count == 0) return;
		
		PathCurve = new Curve2D();
		foreach (var point in PathPoints)
		{
			PathCurve.AddPoint(point);
		}
	}
	
	public void AddPathPoint(Vector2 point)
	{
		PathPoints.Add(point);
		CreatePathFromPoints();
	}
	
	public void SetDefaultZigzagPath()
	{
		PathPoints.Clear();
		
		// Create the zigzag path
		PathPoints.Add(new Vector2(100, -50));   // Start above screen
		PathPoints.Add(new Vector2(100, 150));   // Entry point
		PathPoints.Add(new Vector2(400, 150));   // Move right
		PathPoints.Add(new Vector2(400, 300));   // Move down
		PathPoints.Add(new Vector2(150, 300));   // Move left
		PathPoints.Add(new Vector2(150, 450));   // Move down
		PathPoints.Add(new Vector2(500, 450));   // Move right
		PathPoints.Add(new Vector2(500, 600));   // Move down
		PathPoints.Add(new Vector2(300, 600));   // Move left
		PathPoints.Add(new Vector2(300, 750));   // Exit below screen
		
		SpawnPoint = PathPoints[0];
		EndPoint = PathPoints[PathPoints.Count - 1];
		
		CreatePathFromPoints();
	}
}
