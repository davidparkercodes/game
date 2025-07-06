using Game.Domain.Buildings.ValueObjects;

namespace Game.Domain.Buildings.Entities;

public class BasicTower : Tower
{
    public const string ConfigKey = "basic_tower";

    public BasicTower(BuildingStats stats, float x, float y) : base(stats, x, y)
    {
        ValidatePosition(x, y);
    }
}
