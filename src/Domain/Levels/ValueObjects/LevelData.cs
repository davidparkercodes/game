using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Domain.Levels.ValueObjects
{
	public readonly struct LevelData
	{
		public string LevelName { get; }
		public string Description { get; }
		public IReadOnlyList<PathPoint> PathPoints { get; }
		public float PathWidth { get; }
		public PathPoint SpawnPoint { get; }
		public PathPoint EndPoint { get; }
		public int InitialMoney { get; }
		public int InitialLives { get; }

		public LevelData(
			string levelName,
			string description,
			IEnumerable<PathPoint> pathPoints,
			float pathWidth,
			PathPoint spawnPoint,
			PathPoint endPoint,
			int initialMoney,
			int initialLives)
		{
			if (string.IsNullOrWhiteSpace(levelName)) 
				throw new ArgumentException("Level name cannot be empty", nameof(levelName));
			if (pathWidth <= 0) 
				throw new ArgumentException("Path width must be positive", nameof(pathWidth));
			if (initialMoney < 0) 
				throw new ArgumentException("Initial money cannot be negative", nameof(initialMoney));
			if (initialLives <= 0) 
				throw new ArgumentException("Initial lives must be positive", nameof(initialLives));

			var pointsList = pathPoints?.ToList() ?? new List<PathPoint>();
			if (pointsList.Count < 2)
				throw new ArgumentException("Path must have at least 2 points", nameof(pathPoints));

			LevelName = levelName;
			Description = description ?? string.Empty;
			PathPoints = pointsList.AsReadOnly();
			PathWidth = pathWidth;
			SpawnPoint = spawnPoint;
			EndPoint = endPoint;
			InitialMoney = initialMoney;
			InitialLives = initialLives;
		}

		public static LevelData CreateDefault()
		{
			var pathPoints = new[]
			{
				new PathPoint(100, -50),
				new PathPoint(100, 150),
				new PathPoint(400, 150),
				new PathPoint(400, 300),
				new PathPoint(150, 300),
				new PathPoint(150, 450),
				new PathPoint(500, 450),
				new PathPoint(500, 600),
				new PathPoint(300, 600),
				new PathPoint(300, 750)
			};

			return new LevelData(
				levelName: "Level 1",
				description: "A simple zigzag level",
				pathPoints: pathPoints,
				pathWidth: 64.0f,
				spawnPoint: pathPoints[0],
				endPoint: pathPoints[^1],
				initialMoney: 100,
				initialLives: 20
			);
		}

		public float PathLength
		{
			get
			{
				if (PathPoints.Count < 2) return 0;

				float totalLength = 0;
				for (int i = 1; i < PathPoints.Count; i++)
				{
					totalLength += PathPoints[i-1].DistanceTo(PathPoints[i]);
				}
				return totalLength;
			}
		}

		public float DifficultyRating
		{
			get
			{
				var pathComplexity = PathPoints.Count / 10.0f;
				var resourceScarcity = 1000.0f / Math.Max(InitialMoney, 1);
				var livesConstraint = 20.0f / Math.Max(InitialLives, 1);
				
				return (pathComplexity + resourceScarcity + livesConstraint) / 3.0f;
			}
		}

		public override string ToString()
		{
			return $"Level({LevelName}, Points:{PathPoints.Count}, Money:{InitialMoney}, Lives:{InitialLives}, Difficulty:{DifficultyRating:F2})";
		}

		public override bool Equals(object obj)
		{
			return obj is LevelData other && Equals(other);
		}

		public bool Equals(LevelData other)
		{
			return LevelName == other.LevelName &&
				   Description == other.Description &&
				   PathPoints.SequenceEqual(other.PathPoints) &&
				   Math.Abs(PathWidth - other.PathWidth) < 0.001f &&
				   SpawnPoint.Equals(other.SpawnPoint) &&
				   EndPoint.Equals(other.EndPoint) &&
				   InitialMoney == other.InitialMoney &&
				   InitialLives == other.InitialLives;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(LevelName, Description, PathWidth, SpawnPoint, EndPoint, InitialMoney, InitialLives);
		}

		public static bool operator ==(LevelData left, LevelData right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(LevelData left, LevelData right)
		{
			return !left.Equals(right);
		}
	}

	public readonly struct PathPoint
	{
		public float X { get; }
		public float Y { get; }

		public PathPoint(float x, float y)
		{
			X = x;
			Y = y;
		}

		public float DistanceTo(PathPoint other)
		{
			var dx = X - other.X;
			var dy = Y - other.Y;
			return (float)Math.Sqrt(dx * dx + dy * dy);
		}

		public override string ToString()
		{
			return $"({X:F1}, {Y:F1})";
		}

		public override bool Equals(object obj)
		{
			return obj is PathPoint other && Equals(other);
		}

		public bool Equals(PathPoint other)
		{
			return Math.Abs(X - other.X) < 0.001f && Math.Abs(Y - other.Y) < 0.001f;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(X, Y);
		}

		public static bool operator ==(PathPoint left, PathPoint right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(PathPoint left, PathPoint right)
		{
			return !left.Equals(right);
		}
	}
}
