using Game.Domain.Shared.ValueObjects;

namespace Game.Domain.Buildings.Services;

public interface IBuildingZoneService
{
    bool CanBuildAt(Position worldPosition);
    bool IsOnPath(Position worldPosition);
    bool CanBuildAtWithLogging(Position worldPosition);
}
