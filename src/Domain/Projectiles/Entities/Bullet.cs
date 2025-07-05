using System;

namespace Game.Domain.Projectiles.Entities;

public class Bullet
{
    public Guid Id { get; }
    public float X { get; private set; }
    public float Y { get; private set; }
    public float VelocityX { get; private set; }
    public float VelocityY { get; private set; }
    public float Speed { get; }
    public int Damage { get; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; }
    public float MaxDistance { get; }
    public float DistanceTraveled { get; private set; }

    public Bullet(float x, float y, float targetX, float targetY, float speed, int damage, float maxDistance = 1000f)
    {
        ValidatePosition(x, y);
        ValidatePosition(targetX, targetY);
        
        if (speed <= 0)
            throw new ArgumentException("Speed must be positive", nameof(speed));
        
        if (damage < 0)
            throw new ArgumentException("Damage cannot be negative", nameof(damage));
        
        if (maxDistance <= 0)
            throw new ArgumentException("Max distance must be positive", nameof(maxDistance));

        Id = Guid.NewGuid();
        X = x;
        Y = y;
        Speed = speed;
        Damage = damage;
        MaxDistance = maxDistance;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        DistanceTraveled = 0f;

        CalculateVelocity(targetX, targetY);
    }

    public void Update(float deltaTime)
    {
        if (!IsActive)
            return;

        if (deltaTime <= 0)
            throw new ArgumentException("Delta time must be positive", nameof(deltaTime));

        var deltaX = VelocityX * deltaTime;
        var deltaY = VelocityY * deltaTime;
        
        X += deltaX;
        Y += deltaY;
        
        var distanceThisFrame = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        DistanceTraveled += distanceThisFrame;

        if (DistanceTraveled >= MaxDistance)
        {
            Deactivate();
        }
    }

    public float CalculateDistance(float targetX, float targetY)
    {
        var deltaX = targetX - X;
        var deltaY = targetY - Y;
        return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }

    public bool IsNearTarget(float targetX, float targetY, float tolerance = 5.0f)
    {
        return CalculateDistance(targetX, targetY) <= tolerance;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public bool HasExpired()
    {
        return !IsActive || DistanceTraveled >= MaxDistance;
    }

    public float GetRemainingDistance()
    {
        return Math.Max(0f, MaxDistance - DistanceTraveled);
    }

    public float GetTimeToTarget(float targetX, float targetY)
    {
        if (Speed <= 0)
            return float.MaxValue;

        var distance = CalculateDistance(targetX, targetY);
        return distance / Speed;
    }

    public void SetPosition(float x, float y)
    {
        ValidatePosition(x, y);
        X = x;
        Y = y;
    }

    private void CalculateVelocity(float targetX, float targetY)
    {
        var deltaX = targetX - X;
        var deltaY = targetY - Y;
        var distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

        if (distance <= 0.001f)
        {
            VelocityX = 0f;
            VelocityY = 0f;
            return;
        }

        var directionX = deltaX / distance;
        var directionY = deltaY / distance;

        VelocityX = directionX * Speed;
        VelocityY = directionY * Speed;
    }

    private void ValidatePosition(float x, float y)
    {
        if (float.IsNaN(x) || float.IsNaN(y))
            throw new ArgumentException("Position cannot be NaN");
        
        if (float.IsInfinity(x) || float.IsInfinity(y))
            throw new ArgumentException("Position cannot be infinite");
    }

    public override string ToString()
    {
        return $"Bullet(Id:{Id:N}, Pos:({X:F1},{Y:F1}), Damage:{Damage}, Active:{IsActive}, Distance:{DistanceTraveled:F1}/{MaxDistance:F1})";
    }
}
