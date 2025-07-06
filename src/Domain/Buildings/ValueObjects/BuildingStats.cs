using System;

namespace Game.Domain.Buildings.ValueObjects;

public readonly struct BuildingStats
{
    public int Cost { get; }
    public int Damage { get; }
    public float Range { get; }
    public float AttackSpeed { get; }
    public float BulletSpeed { get; }
    public string ShootSound { get; }
    public string ImpactSound { get; }
    public string Description { get; }

    public BuildingStats(
        int cost,
        int damage,
        float range,
        float attackSpeed,
        float bulletSpeed,
        string shootSound,
        string impactSound,
        string description)
    {
        if (cost < 0) throw new ArgumentException("Cost cannot be negative", nameof(cost));
        if (damage < 0) throw new ArgumentException("Damage cannot be negative", nameof(damage));
        if (range <= 0) throw new ArgumentException("Range must be positive", nameof(range));
        if (attackSpeed <= 0) throw new ArgumentException("Attack speed must be positive", nameof(attackSpeed));
        if (bulletSpeed <= 0) throw new ArgumentException("Bullet speed must be positive", nameof(bulletSpeed));

        Cost = cost;
        Damage = damage;
        Range = range;
        AttackSpeed = attackSpeed;
        BulletSpeed = bulletSpeed;
        ShootSound = shootSound ?? string.Empty;
        ImpactSound = impactSound ?? string.Empty;
        Description = description ?? string.Empty;
    }

    public static BuildingStats CreateDefault()
    {
        return new BuildingStats(
            cost: 10,
            damage: 10,
            range: 150.0f,
            attackSpeed: 30.0f,
            bulletSpeed: 900.0f,
            shootSound: "basic_tower_shoot",
            impactSound: "basic_bullet_impact",
            description: "Basic tower configuration"
        );
    }

    public float DamagePerSecond => Damage * (AttackSpeed / 30.0f);

    public float CostEffectiveness => Cost > 0 ? DamagePerSecond / Cost : 0;

    public override string ToString()
    {
        return $"BuildingStats(Cost:{Cost}, Damage:{Damage}, Range:{Range:F1}, AttackSpeed:{AttackSpeed:F1}, DPS:{DamagePerSecond:F1})";
    }

    public override bool Equals(object? obj)
    {
        return obj is BuildingStats other && Equals(other);
    }

    public bool Equals(BuildingStats other)
    {
        return Cost == other.Cost &&
               Damage == other.Damage &&
               Math.Abs(Range - other.Range) < 0.001f &&
               Math.Abs(AttackSpeed - other.AttackSpeed) < 0.001f &&
               Math.Abs(BulletSpeed - other.BulletSpeed) < 0.001f &&
               ShootSound == other.ShootSound &&
               ImpactSound == other.ImpactSound &&
               Description == other.Description;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Cost, Damage, Range, AttackSpeed, BulletSpeed, ShootSound, ImpactSound, Description);
    }

    public static bool operator ==(BuildingStats left, BuildingStats right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BuildingStats left, BuildingStats right)
    {
        return !left.Equals(right);
    }
}
