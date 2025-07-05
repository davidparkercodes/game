using Game.Domain.Buildings.ValueObjects;

namespace Game.Domain.Buildings.Entities;

public class BasicTurret : Building
{
    public const string TurretType = "BasicTurret";

    public BasicTurret(float x, float y) : base(CreateBasicTurretStats(), x, y)
    {
        ValidatePosition(x, y);
    }

    public BasicTurret(BuildingStats customStats, float x, float y) : base(customStats, x, y)
    {
        ValidatePosition(x, y);
    }

    public static BuildingStats CreateBasicTurretStats()
    {
        return new BuildingStats(
            cost: 10,
            damage: 10,
            range: 150.0f,
            fireRate: 1.0f,
            bulletSpeed: 900.0f,
            shootSound: "basic_turret_shoot",
            impactSound: "basic_bullet_impact",
            description: "Basic defensive turret"
        );
    }

    public override bool CanShoot(float currentTime)
    {
        return base.CanShoot(currentTime) && IsActive;
    }

    public override void Shoot(float currentTime)
    {
        base.Shoot(currentTime);
    }

    public BuildingStats GetUpgradedStats()
    {
        return new BuildingStats(
            cost: Stats.Cost + 5,
            damage: Stats.Damage + 5,
            range: Stats.Range + 25.0f,
            fireRate: Stats.FireRate + 0.2f,
            bulletSpeed: Stats.BulletSpeed,
            shootSound: Stats.ShootSound,
            impactSound: Stats.ImpactSound,
            description: "Upgraded basic turret"
        );
    }

    public void Upgrade()
    {
        if (!CanUpgrade())
            throw new System.InvalidOperationException("Cannot upgrade inactive turret");
        
        Stats = GetUpgradedStats();
    }
}
