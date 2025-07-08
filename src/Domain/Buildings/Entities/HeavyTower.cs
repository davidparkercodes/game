using Game.Domain.Buildings.ValueObjects;

namespace Game.Domain.Buildings.Entities;

public class HeavyTower : Tower
{
    public const string ConfigKey = "heavy_tower";

    public HeavyTower(BuildingStats stats, float x, float y) : base(stats, x, y)
    {
        ValidatePosition(x, y);
    }
}
