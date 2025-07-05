using Game.Domain.Buildings.ValueObjects;

namespace Game.Domain.Buildings.Entities;

public class SniperTower : Building
{
    public const string TowerType = "SniperTower";

    public SniperTower(float x, float y) : base(CreateSniperTowerStats(), x, y)
    {
        ValidatePosition(x, y);
    }

    public SniperTower(BuildingStats customStats, float x, float y) : base(customStats, x, y)
    {
        ValidatePosition(x, y);
    }

    public static BuildingStats CreateSniperTowerStats()
    {
        return new BuildingStats(
            cost: 25,
            damage: 40,
            range: 300.0f,
            fireRate: 0.5f,
            bulletSpeed: 1200.0f,
            shootSound: "sniper_tower_shoot",
            impactSound: "sniper_bullet_impact",
            description: "High damage, long range tower"
        );
    }

    public override bool CanShoot(float currentTime)
    {
        return base.CanShoot(currentTime) && IsActive && HasClearLineOfSight();
    }

    public override void Shoot(float currentTime)
    {
        if (!HasClearLineOfSight())
            throw new System.InvalidOperationException("No clear line of sight for sniper shot");
        
        base.Shoot(currentTime);
    }

    public virtual bool HasClearLineOfSight()
    {
        return true;
    }

    public BuildingStats GetUpgradedStats()
    {
        return new BuildingStats(
            cost: Stats.Cost + 15,
            damage: Stats.Damage + 20,
            range: Stats.Range + 50.0f,
            fireRate: Stats.FireRate + 0.1f,
            bulletSpeed: Stats.BulletSpeed + 200.0f,
            shootSound: Stats.ShootSound,
            impactSound: Stats.ImpactSound,
            description: "Upgraded sniper tower"
        );
    }

    public void Upgrade()
    {
        if (!CanUpgrade())
            throw new System.InvalidOperationException("Cannot upgrade inactive tower");
        
        Stats = GetUpgradedStats();
    }

    public override bool CanUpgrade()
    {
        return base.CanUpgrade() && Stats.Damage < 100;
    }
}
