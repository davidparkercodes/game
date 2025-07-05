using Godot;
using Game.Domain.Buildings.Services;
using Game.Domain.Shared.ValueObjects;
using InfrastructureValidator = Game.Infrastructure.Buildings.Validators.BuildingZoneValidator;

namespace Game.Infrastructure.Buildings.Services;

public class BuildingZoneService : IBuildingZoneService
{
    public bool CanBuildAt(Position worldPosition)
    {
        var godotPosition = new Vector2(worldPosition.X, worldPosition.Y);
        return InfrastructureValidator.CanBuildAt(godotPosition);
    }

    public bool IsOnPath(Position worldPosition)
    {
        var godotPosition = new Vector2(worldPosition.X, worldPosition.Y);
        return InfrastructureValidator.IsOnPath(godotPosition);
    }

    public bool CanBuildAtWithLogging(Position worldPosition)
    {
        var godotPosition = new Vector2(worldPosition.X, worldPosition.Y);
        return InfrastructureValidator.CanBuildAtWithLogging(godotPosition);
    }
}
