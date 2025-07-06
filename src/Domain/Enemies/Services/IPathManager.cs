using System.Collections.Generic;
using Game.Domain.Enemies.Entities;
using Game.Domain.Levels.ValueObjects;

namespace Game.Domain.Enemies.Services;

public interface IPathManager
{
    void LoadPathFromLevel(LevelData levelData);
    void AddPathPoint(float x, float y, int order = -1);
    void RemovePathPoint(float x, float y);
    OrderedPathPoint? GetNextPathPoint(Enemy enemy);
    OrderedPathPoint? GetCurrentPathPoint(Enemy enemy);
    bool IsEnemyAtPoint(Enemy enemy, OrderedPathPoint point);
    float CalculatePathDistance(Enemy enemy, OrderedPathPoint targetPoint);
    bool HasReachedEnd(Enemy enemy);
    void ClearPath();
    IReadOnlyList<OrderedPathPoint> GetPathPoints();
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

    public override bool Equals(object? obj)
    {
        return obj is OrderedPathPoint other && 
               System.Math.Abs(X - other.X) < 0.001f && 
               System.Math.Abs(Y - other.Y) < 0.001f && 
               Order == other.Order;
    }

    public override int GetHashCode()
    {
        return System.HashCode.Combine(X, Y, Order);
    }
}
