using Game.Domain.Buildings.ValueObjects;

namespace Game.Domain.Buildings.Entities;

public class SniperTower : Tower
{
    public const string ConfigKey = "sniper_tower";

    public SniperTower(BuildingStats stats, float x, float y) : base(stats, x, y)
    {
        ValidatePosition(x, y);
    }

    public override bool CanShoot(float currentTime)
    {
        return base.CanShoot(currentTime) && HasClearLineOfSight();
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
}
