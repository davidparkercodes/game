using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Game.Domain.Enemies.Entities;
using Game.Domain.Levels.ValueObjects;

namespace Game.Domain.Enemies.Services
{
	public partial class PathManager : Node2D
	{
		[Export] public LevelDataResource CurrentLevel { get; set; }
		private readonly List<OrderedPathPoint> _pathPoints;
		private readonly float _pathTolerance;

		public PathManager()
		{
			_pathPoints = new List<OrderedPathPoint>();
			_pathTolerance = 10.0f;
		}

		public override void _Ready()
		{
			if (CurrentLevel != null)
			{
				LoadPathFromLevel(CurrentLevel.ToLevelData());
			}
		}

		public void LoadPathFromLevel(LevelData levelData)
		{
			_pathPoints.Clear();
			for (int i = 0; i < levelData.PathPoints.Count; i++)
			{
				var levelPoint = levelData.PathPoints[i];
				_pathPoints.Add(new OrderedPathPoint(levelPoint.X, levelPoint.Y, i));
			}
		}

    public void AddPathPoint(float x, float y, int order = -1)
    {
        ValidatePosition(x, y);
        
        var pathPoint = new OrderedPathPoint(x, y, order == -1 ? _pathPoints.Count : order);
        _pathPoints.Add(pathPoint);
        
        _pathPoints.Sort((a, b) => a.Order.CompareTo(b.Order));
    }

    public void RemovePathPoint(float x, float y)
    {
        var pointToRemove = _pathPoints.FirstOrDefault(p => 
            Math.Abs(p.X - x) <= _pathTolerance && Math.Abs(p.Y - y) <= _pathTolerance);
        
        if (pointToRemove != null)
        {
            _pathPoints.Remove(pointToRemove);
            ReorderPathPoints();
        }
    }

    public OrderedPathPoint GetNextPathPoint(Game.Domain.Enemies.Entities.Enemy enemy)
    {
        if (enemy == null)
            throw new ArgumentNullException(nameof(enemy));

        if (!enemy.IsAlive)
            return null;

        var currentPoint = GetCurrentPathPoint(enemy);
        if (currentPoint == null)
            return _pathPoints.FirstOrDefault();

        var nextIndex = currentPoint.Order + 1;
        return _pathPoints.FirstOrDefault(p => p.Order == nextIndex);
    }

    public OrderedPathPoint GetCurrentPathPoint(Game.Domain.Enemies.Entities.Enemy enemy)
    {
        if (enemy == null)
            throw new ArgumentNullException(nameof(enemy));

        return _pathPoints
            .Where(p => IsEnemyAtPoint(enemy, p))
            .OrderBy(p => p.Order)
            .FirstOrDefault();
    }

    public bool IsEnemyAtPoint(Game.Domain.Enemies.Entities.Enemy enemy, OrderedPathPoint point)
    {
        if (enemy == null || point == null)
            return false;

        return enemy.CalculateDistance(point.X, point.Y) <= _pathTolerance;
    }

    public float CalculatePathDistance(Game.Domain.Enemies.Entities.Enemy enemy, OrderedPathPoint targetPoint)
    {
        if (enemy == null || targetPoint == null)
            return float.MaxValue;

        var currentPoint = GetCurrentPathPoint(enemy);
        if (currentPoint == null)
            return enemy.CalculateDistance(targetPoint.X, targetPoint.Y);

        var totalDistance = 0f;
        var currentIndex = currentPoint.Order;
        var targetIndex = targetPoint.Order;

        if (currentIndex >= targetIndex)
            return enemy.CalculateDistance(targetPoint.X, targetPoint.Y);

        for (int i = currentIndex; i < targetIndex; i++)
        {
            var from = i == currentIndex ? 
                new OrderedPathPoint(enemy.X, enemy.Y, i) : 
                _pathPoints.FirstOrDefault(p => p.Order == i);
            
            var to = _pathPoints.FirstOrDefault(p => p.Order == i + 1);
            
            if (from != null && to != null)
            {
                totalDistance += CalculateDistance(from.X, from.Y, to.X, to.Y);
            }
        }

        return totalDistance;
    }

    public bool HasReachedEnd(Game.Domain.Enemies.Entities.Enemy enemy)
    {
        if (enemy == null || !_pathPoints.Any())
            return false;

        var lastPoint = _pathPoints.OrderByDescending(p => p.Order).First();
        return IsEnemyAtPoint(enemy, lastPoint);
    }

    public void ClearPath()
    {
        _pathPoints.Clear();
    }

    public IReadOnlyList<OrderedPathPoint> GetPathPoints()
    {
        return _pathPoints.AsReadOnly();
    }

    private void ReorderPathPoints()
    {
        for (int i = 0; i < _pathPoints.Count; i++)
        {
            _pathPoints[i] = new OrderedPathPoint(_pathPoints[i].X, _pathPoints[i].Y, i);
        }
    }

    private float CalculateDistance(float x1, float y1, float x2, float y2)
    {
        var deltaX = x2 - x1;
        var deltaY = y2 - y1;
        return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }

    private void ValidatePosition(float x, float y)
    {
        if (float.IsNaN(x) || float.IsNaN(y))
            throw new ArgumentException("Position cannot be NaN");
        
        if (float.IsInfinity(x) || float.IsInfinity(y))
            throw new ArgumentException("Position cannot be infinite");
    }
}

public class OrderedPathPoint
{
    public float X { get; }
    public float Y { get; }
    public int Order { get; }

    public OrderedPathPoint(float x, float y, int order)
    {
        X = x;
        Y = y;
        Order = order;
    }

    public override string ToString()
    {
        return $"OrderedPathPoint(X:{X:F1}, Y:{Y:F1}, Order:{Order})";
    }

    public override bool Equals(object obj)
    {
        return obj is OrderedPathPoint other && 
               Math.Abs(X - other.X) < 0.001f && 
               Math.Abs(Y - other.Y) < 0.001f && 
               Order == other.Order;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Order);
    }
}
}
