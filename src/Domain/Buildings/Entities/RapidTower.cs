using Game.Domain.Buildings.ValueObjects;

namespace Game.Domain.Buildings.Entities;

public class RapidTower : Tower
{
    public const string ConfigKey = "rapid_tower";

    public RapidTower(BuildingStats stats, float x, float y) : base(stats, x, y)
    {
        ValidatePosition(x, y);
    }
}
