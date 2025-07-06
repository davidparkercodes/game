using System;
using Game.Domain.Buildings.ValueObjects;

namespace Game.Domain.Buildings.Entities;

public abstract class Building
{
    public Guid Id { get; }
    public BuildingStats Stats { get; protected set; }
    public float X { get; }
    public float Y { get; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; }

    protected Building(BuildingStats stats, float x, float y)
    {
        Id = Guid.NewGuid();
        Stats = stats;
        X = x;
        Y = y;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public virtual bool IsInRange(float targetX, float targetY)
    {
        var distance = CalculateDistance(targetX, targetY);
        return distance <= Stats.Range;
    }

    public virtual float CalculateDistance(float targetX, float targetY)
    {
        var deltaX = targetX - X;
        var deltaY = targetY - Y;
        return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }

    public virtual void Deactivate()
    {
        IsActive = false;
    }

    public virtual void Activate()
    {
        IsActive = true;
    }

    public virtual bool CanUpgrade()
    {
        return IsActive;
    }

    protected virtual void ValidatePosition(float x, float y)
    {
        if (float.IsNaN(x) || float.IsNaN(y))
            throw new ArgumentException("Position cannot be NaN");
        
        if (float.IsInfinity(x) || float.IsInfinity(y))
            throw new ArgumentException("Position cannot be infinite");
    }
}
