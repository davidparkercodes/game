using Godot;
using Game.Infrastructure.Interfaces;
using Game.Infrastructure.Validators;

namespace Game.Infrastructure.Buildings;

public class BuildingZoneService : IBuildingZoneService
{
    public bool CanBuildAt(Vector2 worldPosition)
    {
        return BuildingZoneValidator.CanBuildAt(worldPosition);
    }

    public bool IsOnPath(Vector2 worldPosition)
    {
        return BuildingZoneValidator.IsOnPath(worldPosition);
    }

    public bool CanBuildAtWithLogging(Vector2 worldPosition)
    {
        return BuildingZoneValidator.CanBuildAtWithLogging(worldPosition);
    }
}
