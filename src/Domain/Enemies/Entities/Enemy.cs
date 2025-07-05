using System;
using Game.Domain.Enemies.ValueObjects;

namespace Game.Domain.Enemies.Entities;

public class Enemy
{
    public Guid Id { get; }
    public EnemyStats Stats { get; }
    public int CurrentHealth { get; private set; }
    public float X { get; private set; }
    public float Y { get; private set; }
    public bool IsAlive { get; private set; }
    public DateTime SpawnedAt { get; }
    public float LastDamageTime { get; private set; }

    public Enemy(EnemyStats stats, float x, float y)
    {
        ValidatePosition(x, y);
        
        Id = Guid.NewGuid();
        Stats = stats;
        CurrentHealth = stats.MaxHealth;
        X = x;
        Y = y;
        IsAlive = true;
        SpawnedAt = DateTime.UtcNow;
        LastDamageTime = 0f;
    }

    public virtual void TakeDamage(int damage, float currentTime)
    {
        if (!IsAlive)
            throw new InvalidOperationException("Cannot damage dead enemy");
        
        if (damage < 0)
            throw new ArgumentException("Damage cannot be negative", nameof(damage));

        CurrentHealth = Math.Max(0, CurrentHealth - damage);
        LastDamageTime = currentTime;

        if (CurrentHealth <= 0)
        {
            IsAlive = false;
        }
    }

    public virtual void MoveTo(float x, float y)
    {
        if (!IsAlive)
            throw new InvalidOperationException("Cannot move dead enemy");
        
        ValidatePosition(x, y);
        X = x;
        Y = y;
    }

    public virtual void MoveBy(float deltaX, float deltaY)
    {
        MoveTo(X + deltaX, Y + deltaY);
    }

    public virtual float CalculateDistance(float targetX, float targetY)
    {
        var deltaX = targetX - X;
        var deltaY = targetY - Y;
        return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }

    public virtual float HealthPercentage => Stats.MaxHealth > 0 ? (float)CurrentHealth / Stats.MaxHealth : 0f;

    public virtual bool IsNearDeath => HealthPercentage <= 0.2f;

    public virtual bool CanReachTarget(float targetX, float targetY, float maxDistance)
    {
        return CalculateDistance(targetX, targetY) <= maxDistance;
    }

    public virtual void Heal(int amount)
    {
        if (!IsAlive)
            throw new InvalidOperationException("Cannot heal dead enemy");
        
        if (amount < 0)
            throw new ArgumentException("Heal amount cannot be negative", nameof(amount));

        CurrentHealth = Math.Min(Stats.MaxHealth, CurrentHealth + amount);
    }

    public virtual void Kill()
    {
        CurrentHealth = 0;
        IsAlive = false;
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
        return $"Enemy(Id:{Id:N}, HP:{CurrentHealth}/{Stats.MaxHealth}, Pos:({X:F1},{Y:F1}), Alive:{IsAlive})";
    }
}
