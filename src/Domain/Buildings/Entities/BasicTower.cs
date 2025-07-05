using Game.Domain.Buildings.ValueObjects;

namespace Game.Domain.Buildings.Entities;

public class BasicTower : Building
{
    public const string TowerType = "BasicTower";

    public BasicTower(float x, float y) : base(CreateBasicTowerStats(), x, y)
    {
        ValidatePosition(x, y);
    }

    public BasicTower(BuildingStats customStats, float x, float y) : base(customStats, x, y)
    {
        ValidatePosition(x, y);
    }

    public static BuildingStats CreateBasicTowerStats()
    {
        return new BuildingStats(
            cost: 10,
            damage: 10,
            range: 150.0f,
            fireRate: 1.0f,
            bulletSpeed: 900.0f,
            shootSound: "basic_tower_shoot",
            impactSound: "basic_bullet_impact",
            description: "Basic defensive tower"
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
            description: "Upgraded basic tower"
        );
    }

    public void Upgrade()
    {
        if (!CanUpgrade())
            throw new System.InvalidOperationException("Cannot upgrade inactive tower");
        
        Stats = GetUpgradedStats();
    }
}
