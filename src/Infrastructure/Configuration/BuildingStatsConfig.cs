using System.Collections.Generic;
using Game.Domain.Buildings.ValueObjects;

namespace Game.Infrastructure.Configuration;

public class BuildingStatsConfig
{
    // NO HARDCODED VALUES! All data must come from config files.
    public BuildingStatsData default_stats { get; set; } = BuildingStatsData.Empty;
    public Dictionary<string, BuildingStatsData> building_types { get; set; } = new();

    // Remove parameterless constructor to force explicit initialization
    // All data should be loaded from JSON files
}
