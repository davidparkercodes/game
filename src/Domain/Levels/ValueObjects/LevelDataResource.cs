using Godot;
using System;

namespace Game.Domain.Levels.ValueObjects
{
	[GlobalClass]
	public partial class LevelDataResource : Resource
	{
		[Export] public string LevelName { get; set; } = "";
		[Export] public string Description { get; set; } = "";
		[Export] public Vector2[] PathPoints { get; set; } = Array.Empty<Vector2>();
		[Export] public float PathWidth { get; set; } = 64.0f;
		[Export] public Vector2 SpawnPoint { get; set; }
		[Export] public Vector2 EndPoint { get; set; }
		[Export] public int InitialMoney { get; set; } = 100;
		[Export] public int InitialLives { get; set; } = 20;
		[Export] public Color PathColor { get; set; } = Colors.Yellow;

		public LevelData ToLevelData()
		{
			var pathPoints = new PathPoint[PathPoints.Length];
			for (int i = 0; i < PathPoints.Length; i++)
			{
				pathPoints[i] = new PathPoint(PathPoints[i].X, PathPoints[i].Y);
			}

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
