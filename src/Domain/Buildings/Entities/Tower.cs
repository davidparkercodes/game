using Game.Domain.Buildings.ValueObjects;

namespace Game.Domain.Buildings.Entities;

public abstract class Tower : Building
{
    public float LastShotTime { get; private set; }

    protected Tower(BuildingStats stats, float x, float y) : base(stats, x, y)
    {
        LastShotTime = 0f;
    }

    public virtual bool CanShoot(float currentTime)
    {
        if (!IsActive) return false;
        return currentTime - LastShotTime >= (30f / Stats.AttackSpeed);
    }

    public virtual void Shoot(float currentTime)
    {
        if (!CanShoot(currentTime))
            throw new System.InvalidOperationException("Tower cannot shoot at this time");
        
        LastShotTime = currentTime;
    }
}
