using Game.Domain.Buildings.ValueObjects;

namespace Game.Domain.Buildings.Services;

public interface IBuildingStatsProvider
{
    BuildingStats GetBuildingStats(string buildingType);
    bool HasBuildingStats(string buildingType);
}
