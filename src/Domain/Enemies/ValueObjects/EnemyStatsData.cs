using System;

namespace Game.Domain.Enemies.ValueObjects;

public readonly struct EnemyStatsData
{
    public int max_health { get; }
    public float speed { get; }
    public int damage { get; }
    public int reward_gold { get; }
    public int reward_xp { get; }
    public string description { get; }

    public EnemyStatsData(
        int maxHealth = 0,
        float speed = 0f,
        int damage = 0,
        int rewardGold = 0,
        int rewardXp = 0,
        string description = "")
    {
        if (maxHealth < 0) throw new ArgumentException("Max health cannot be negative", nameof(maxHealth));
        if (speed < 0) throw new ArgumentException("Speed cannot be negative", nameof(speed));
        if (damage < 0) throw new ArgumentException("Damage cannot be negative", nameof(damage));
        if (rewardGold < 0) throw new ArgumentException("Reward gold cannot be negative", nameof(rewardGold));
        if (rewardXp < 0) throw new ArgumentException("Reward XP cannot be negative", nameof(rewardXp));

        max_health = maxHealth;
        this.speed = speed;
        this.damage = damage;
        reward_gold = rewardGold;
        reward_xp = rewardXp;
        this.description = description ?? string.Empty;
    }

    // NO HARDCODED VALUES! All data must come from config files.
    // Use empty/zero defaults only.
    public static EnemyStatsData Empty => new();

    public override string ToString()
    {
        return $"EnemyStats(HP:{max_health}, Speed:{speed:F1}, Damage:{damage}, Gold:{reward_gold}, XP:{reward_xp})";
    }

    public override bool Equals(object obj)
    {
        return obj is EnemyStatsData other && Equals(other);
    }

    public bool Equals(EnemyStatsData other)
    {
        return max_health == other.max_health &&
               Math.Abs(speed - other.speed) < 0.001f &&
               damage == other.damage &&
               reward_gold == other.reward_gold &&
               reward_xp == other.reward_xp &&
               description == other.description;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(max_health, speed, damage, reward_gold, reward_xp, description);
    }

    public static bool operator ==(EnemyStatsData left, EnemyStatsData right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(EnemyStatsData left, EnemyStatsData right)
    {
        return !left.Equals(right);
    }
}
