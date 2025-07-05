using System;

namespace Game.Domain.Buildings.ValueObjects;

public readonly struct BuildingStatsData
{
    public int cost { get; }
    public int damage { get; }
    public float range { get; }
    public float attack_speed { get; }
    public int upgrade_cost { get; }
    public string description { get; }

    public BuildingStatsData(
        int cost = 0,
        int damage = 0,
        float range = 0f,
        float attackSpeed = 0f,
        int upgradeCost = 0,
        string description = "")
    {
        if (cost < 0) throw new ArgumentException("Cost cannot be negative", nameof(cost));
        if (damage < 0) throw new ArgumentException("Damage cannot be negative", nameof(damage));
        if (range < 0) throw new ArgumentException("Range cannot be negative", nameof(range));
        if (attackSpeed < 0) throw new ArgumentException("Attack speed cannot be negative", nameof(attackSpeed));
        if (upgradeCost < 0) throw new ArgumentException("Upgrade cost cannot be negative", nameof(upgradeCost));

        this.cost = cost;
        this.damage = damage;
        this.range = range;
        attack_speed = attackSpeed;
        upgrade_cost = upgradeCost;
        this.description = description ?? string.Empty;
    }

    public static BuildingStatsData CreateDefault() => new(
        cost: 100,
        damage: 25,
        range: 150f,
        attackSpeed: 1.0f,
        upgradeCost: 50,
        description: "Basic tower"
    );

    public override string ToString()
    {
        return $"BuildingStats(Cost:{cost}, Damage:{damage}, Range:{range:F1}, AttackSpeed:{attack_speed:F1})";
    }

    public override bool Equals(object obj)
    {
        return obj is BuildingStatsData other && Equals(other);
    }

    public bool Equals(BuildingStatsData other)
    {
        return cost == other.cost &&
               damage == other.damage &&
               Math.Abs(range - other.range) < 0.001f &&
               Math.Abs(attack_speed - other.attack_speed) < 0.001f &&
               upgrade_cost == other.upgrade_cost &&
               description == other.description;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(cost, damage, range, attack_speed, upgrade_cost, description);
    }

    public static bool operator ==(BuildingStatsData left, BuildingStatsData right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BuildingStatsData left, BuildingStatsData right)
    {
        return !left.Equals(right);
    }
}
