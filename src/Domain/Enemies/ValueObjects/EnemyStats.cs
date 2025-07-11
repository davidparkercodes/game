using System;

namespace Game.Domain.Enemies.ValueObjects;

public readonly struct EnemyStats
{
    public int MaxHealth { get; }
    public float Speed { get; }
    public int Damage { get; }
    public int RewardGold { get; }
    public int RewardXp { get; }
    public string Description { get; }

    public EnemyStats(
        int maxHealth,
        float speed,
        int damage,
        int rewardGold,
        int rewardXp,
        string description)
    {
        if (maxHealth <= 0) throw new ArgumentException("Max health must be positive", nameof(maxHealth));
        if (speed <= 0) throw new ArgumentException("Speed must be positive", nameof(speed));
        if (damage < 0) throw new ArgumentException("Damage cannot be negative", nameof(damage));
        if (rewardGold < 0) throw new ArgumentException("Reward gold cannot be negative", nameof(rewardGold));
        if (rewardXp < 0) throw new ArgumentException("Reward XP cannot be negative", nameof(rewardXp));

        MaxHealth = maxHealth;
        Speed = speed;
        Damage = damage;
        RewardGold = rewardGold;
        RewardXp = rewardXp;
        Description = description ?? string.Empty;
    }

    // REMOVED: CreateDefault() method with hardcoded values
    // All EnemyStats must now be created from config data via StatsManagerService
    // Use StatsManagerService.GetDefaultEnemyStats() for fallback scenarios

    public EnemyStats WithMultipliers(float healthMultiplier, float speedMultiplier)
    {
        if (healthMultiplier <= 0) throw new ArgumentException("Health multiplier must be positive", nameof(healthMultiplier));
        if (speedMultiplier <= 0) throw new ArgumentException("Speed multiplier must be positive", nameof(speedMultiplier));

        return new EnemyStats(
            maxHealth: (int)(MaxHealth * healthMultiplier),
            speed: Speed * speedMultiplier,
            damage: Damage,
            rewardGold: RewardGold,
            rewardXp: RewardXp,
            description: Description
        );
    }

    public float ThreatLevel => (MaxHealth * Damage * Speed) / 1000.0f;

    public float RewardEfficiency => ThreatLevel > 0 ? (RewardGold + RewardXp) / ThreatLevel : 0;

    public override string ToString()
    {
        return $"EnemyStats(HP:{MaxHealth}, Speed:{Speed:F1}, Damage:{Damage}, Gold:{RewardGold}, XP:{RewardXp}, Threat:{ThreatLevel:F2})";
    }

    public override bool Equals(object? obj)
    {
        return obj is EnemyStats other && Equals(other);
    }

    public bool Equals(EnemyStats other)
    {
        return MaxHealth == other.MaxHealth &&
               Math.Abs(Speed - other.Speed) < 0.001f &&
               Damage == other.Damage &&
               RewardGold == other.RewardGold &&
               RewardXp == other.RewardXp &&
               Description == other.Description;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(MaxHealth, Speed, Damage, RewardGold, RewardXp, Description);
    }

    public static bool operator ==(EnemyStats left, EnemyStats right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(EnemyStats left, EnemyStats right)
    {
        return !left.Equals(right);
    }
}
