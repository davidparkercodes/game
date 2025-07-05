using Godot;

namespace Game.Infrastructure.Interfaces;

public interface IBuildingZoneService
{
    bool CanBuildAt(Vector2 worldPosition);
    bool IsOnPath(Vector2 worldPosition);
    bool CanBuildAtWithLogging(Vector2 worldPosition);
}
