using Game.Application.Shared.Cqrs;

namespace Game.Application.Buildings.Queries;

public class GetTurretStatsQuery : IQuery<TurretStatsResponse>
{
    public string TurretType { get; }

    public GetTurretStatsQuery(string turretType)
    {
        TurretType = turretType ?? throw new System.ArgumentNullException(nameof(turretType));
    }
}

public class TurretStatsResponse
{
    public string TurretType { get; }
    public int Cost { get; }
    public int Damage { get; }
    public float Range { get; }
    public float FireRate { get; }
    public float BulletSpeed { get; }
    public string Description { get; }
    public bool IsAvailable { get; }

    public TurretStatsResponse(
        string turretType, 
        int cost, 
        int damage, 
        float range, 
        float fireRate, 
        float bulletSpeed, 
        string description, 
        bool isAvailable = true)
    {
        TurretType = turretType;
        Cost = cost;
        Damage = damage;
        Range = range;
        FireRate = fireRate;
        BulletSpeed = bulletSpeed;
        Description = description;
        IsAvailable = isAvailable;
    }

    public static TurretStatsResponse NotFound(string turretType)
    {
        return new TurretStatsResponse(turretType, 0, 0, 0, 0, 0, "Not found", false);
    }
}
