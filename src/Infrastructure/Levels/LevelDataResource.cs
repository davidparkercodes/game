using Godot;
using System;
using Game.Domain.Common.Services;
using Game.Domain.Levels.ValueObjects;

namespace Game.Infrastructure.Levels
{
	[GlobalClass]
	public partial class LevelDataResource : Resource
	{
		private static readonly ILogger _logger = new ConsoleLogger("📝 [LEVEL]");
		
		[Export] public string LevelName { get; set; } = "";
		[Export] public string Description { get; set; } = "";
		[Export] public Vector2[] PathPoints { get; set; } = Array.Empty<Vector2>();
		[Export] public float PathWidth { get; set; } = 64.0f;
		[Export] public Vector2 SpawnPoint { get; set; }
		[Export] public Vector2 EndPoint { get; set; }
		[Export] public int InitialMoney { get; set; } = 100;
		[Export] public int InitialLives { get; set; } = 20;
		[Export] public Color PathColor { get; set; } = Colors.Yellow;
		
		public LevelDataResource()
		{
			// Ensure PathPoints is never null
			PathPoints ??= Array.Empty<Vector2>();
		}

	public LevelData ToLevelData()
	{
		// Ensure PathPoints is not null
		PathPoints ??= Array.Empty<Vector2>();
		
		_logger.LogInformation($"Converting LevelDataResource to LevelData: {PathPoints.Length} path points");
		
		if (PathPoints.Length < 2)
		{
			_logger.LogError($"ToLevelData: PathPoints has only {PathPoints.Length} points, need at least 2");
			throw new InvalidOperationException($"PathPoints must have at least 2 points, but has {PathPoints.Length}");
		}
		
		var pathPoints = new PathPoint[PathPoints.Length];
		for (int i = 0; i < PathPoints.Length; i++)
		{
			pathPoints[i] = new PathPoint(PathPoints[i].X, PathPoints[i].Y);
		}

		_logger.LogDebug($"About to create LevelData with {pathPoints.Length} path points");
		
		return new LevelData(
			LevelName,
			Description,
			pathPoints,
			PathWidth,
			new PathPoint(SpawnPoint.X, SpawnPoint.Y),
			new PathPoint(EndPoint.X, EndPoint.Y),
			InitialMoney,
			InitialLives
		);
	}

		public static LevelDataResource FromLevelData(LevelData levelData)
		{
			var resource = new LevelDataResource
			{
				LevelName = levelData.LevelName,
				Description = levelData.Description,
				PathWidth = levelData.PathWidth,
				SpawnPoint = new Vector2(levelData.SpawnPoint.X, levelData.SpawnPoint.Y),
				EndPoint = new Vector2(levelData.EndPoint.X, levelData.EndPoint.Y),
				InitialMoney = levelData.InitialMoney,
				InitialLives = levelData.InitialLives
			};

			resource.PathPoints = new Vector2[levelData.PathPoints.Count];
			for (int i = 0; i < levelData.PathPoints.Count; i++)
			{
				resource.PathPoints[i] = new Vector2(levelData.PathPoints[i].X, levelData.PathPoints[i].Y);
			}

			return resource;
		}
	}
}
