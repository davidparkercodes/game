using System;

namespace Game.Domain.Buildings.ValueObjects;

public readonly struct BuildingStats
{
    public int Cost { get; }
    public int Damage { get; }
    public float Range { get; }
    public float FireRate { get; }
    public float BulletSpeed { get; }
    public string ShootSound { get; }
    public string ImpactSound { get; }
    public string Description { get; }

    public BuildingStats(
        int cost,
        int damage,
        float range,
        float fireRate,
        float bulletSpeed,
        string shootSound,
        string impactSound,
        string description)
    {
        if (cost < 0) throw new ArgumentException("Cost cannot be negative", nameof(cost));
        if (damage < 0) throw new ArgumentException("Damage cannot be negative", nameof(damage));
        if (range <= 0) throw new ArgumentException("Range must be positive", nameof(range));
        if (fireRate <= 0) throw new ArgumentException("Fire rate must be positive", nameof(fireRate));
        if (bulletSpeed <= 0) throw new ArgumentException("Bullet speed must be positive", nameof(bulletSpeed));

        Cost = cost;
        Damage = damage;
        Range = range;
        FireRate = fireRate;
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
            fireRate: 1.0f,
            bulletSpeed: 900.0f,
            shootSound: "basic_turret_shoot",
            impactSound: "basic_bullet_impact",
            description: "Basic turret configuration"
        );
    }

    public float DamagePerSecond => Damage / FireRate;

    public float CostEffectiveness => Cost > 0 ? DamagePerSecond / Cost : 0;

    public override string ToString()
    {
        return $"BuildingStats(Cost:{Cost}, Damage:{Damage}, Range:{Range:F1}, FireRate:{FireRate:F2}, DPS:{DamagePerSecond:F1})";
    }

    public override bool Equals(object obj)
    {
        return obj is BuildingStats other && Equals(other);
    }

    public bool Equals(BuildingStats other)
    {
        return Cost == other.Cost &&
               Damage == other.Damage &&
               Math.Abs(Range - other.Range) < 0.001f &&
               Math.Abs(FireRate - other.FireRate) < 0.001f &&
               Math.Abs(BulletSpeed - other.BulletSpeed) < 0.001f &&
               ShootSound == other.ShootSound &&
               ImpactSound == other.ImpactSound &&
               Description == other.Description;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Cost, Damage, Range, FireRate, BulletSpeed, ShootSound, ImpactSound, Description);
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
