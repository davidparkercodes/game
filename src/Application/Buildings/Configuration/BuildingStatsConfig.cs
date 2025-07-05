using System.Collections.Generic;
using Game.Domain.Buildings.ValueObjects;

namespace Game.Application.Buildings.Configuration;

public class BuildingStatsConfig
{
    public BuildingStatsData default_stats { get; init; } = new();
    public Dictionary<string, BuildingStatsData> building_types { get; init; } = new();
}
