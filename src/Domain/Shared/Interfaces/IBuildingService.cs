using Game.Domain.ValueObjects;

namespace Game.Domain.Shared.Interfaces;

public interface IBuildingService
{
    bool CanPlaceBuilding(string buildingType, float x, float y);
    bool PlaceBuilding(string buildingType, float x, float y);
    bool RemoveBuilding(float x, float y);
    BuildingStats GetBuildingStatsAt(float x, float y);
    bool HasBuildingAt(float x, float y);
    int GetTotalBuildingCost();
    int GetBuildingCount(string buildingType);
}
