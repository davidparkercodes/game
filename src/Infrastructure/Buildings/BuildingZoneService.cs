using Godot;
using Game.Domain.Buildings.Services;
using Game.Domain.Shared.ValueObjects;
using Game.Infrastructure.Validators;

namespace Game.Infrastructure.Buildings;

public class BuildingZoneService : IBuildingZoneService
{
    public bool CanBuildAt(Position worldPosition)
    {
        var godotPosition = new Vector2(worldPosition.X, worldPosition.Y);
        return BuildingZoneValidator.CanBuildAt(godotPosition);
    }

    public bool IsOnPath(Position worldPosition)
    {
        var godotPosition = new Vector2(worldPosition.X, worldPosition.Y);
        return BuildingZoneValidator.IsOnPath(godotPosition);
    }

    public bool CanBuildAtWithLogging(Position worldPosition)
    {
        var godotPosition = new Vector2(worldPosition.X, worldPosition.Y);
        return BuildingZoneValidator.CanBuildAtWithLogging(godotPosition);
    }
}
