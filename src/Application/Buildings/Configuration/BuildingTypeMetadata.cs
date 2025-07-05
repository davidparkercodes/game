using System.Collections.Generic;
using Game.Domain.Buildings.ValueObjects;

namespace Game.Application.Buildings.Configuration;

public class BuildingTypeMetadata
{
    public string config_key { get; init; } = string.Empty;
    public string display_name { get; init; } = string.Empty;
    public string category { get; init; } = string.Empty;
    public int tier { get; init; } = 1;
    public bool is_default { get; init; } = false;
}

public class BuildingTypesMetadataConfig
{
    public Dictionary<string, BuildingTypeMetadata> registry { get; init; } = new();
    public List<string> categories { get; init; } = new();
    public string default_category { get; init; } = string.Empty;
}

public class EnhancedBuildingStatsConfig
{
    public BuildingTypesMetadataConfig building_types_metadata { get; init; } = new();
    public BuildingStatsData default_stats { get; init; } = new();
    public Dictionary<string, BuildingStatsData> building_types { get; init; } = new();
}
