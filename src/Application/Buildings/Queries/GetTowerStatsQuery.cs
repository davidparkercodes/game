using Game.Application.Shared.Cqrs;

namespace Game.Application.Buildings.Queries;

public class GetTowerStatsQuery : IQuery<TowerStatsResponse>
{
    public string TowerType { get; }

    public GetTowerStatsQuery(string towerType)
    {
        TowerType = towerType ?? throw new System.ArgumentNullException(nameof(towerType));
    }
}

public class TowerStatsResponse
{
    public string TowerType { get; }
    public int Cost { get; }
    public int Damage { get; }
    public float Range { get; }
    public float FireRate { get; }
    public float BulletSpeed { get; }
    public string Description { get; }
    public bool IsAvailable { get; }

    public TowerStatsResponse(
        string towerType, 
        int cost, 
        int damage, 
        float range, 
        float fireRate, 
        float bulletSpeed, 
        string description, 
        bool isAvailable = true)
    {
        TowerType = towerType;
        Cost = cost;
        Damage = damage;
        Range = range;
        FireRate = fireRate;
        BulletSpeed = bulletSpeed;
        Description = description;
        IsAvailable = isAvailable;
    }

    public static TowerStatsResponse NotFound(string towerType)
    {
        return new TowerStatsResponse(towerType, 0, 0, 0, 0, 0, "Not found", false);
    }
}
