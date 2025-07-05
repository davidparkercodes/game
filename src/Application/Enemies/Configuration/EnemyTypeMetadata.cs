using System.Collections.Generic;
using Game.Domain.Enemies.ValueObjects;

namespace Game.Application.Enemies.Configuration;

public class EnemyTypeMetadata
{
    public string config_key { get; init; } = string.Empty;
    public string display_name { get; init; } = string.Empty;
    public string category { get; init; } = string.Empty;
    public int tier { get; init; } = 1;
    public bool is_default { get; init; } = false;
}

public class EnemyTypesMetadataConfig
{
    public Dictionary<string, EnemyTypeMetadata> registry { get; init; } = new();
    public List<string> categories { get; init; } = new();
    public string default_category { get; init; } = string.Empty;
}

public class EnhancedEnemyStatsConfig
{
    public EnemyTypesMetadataConfig enemy_types_metadata { get; init; } = new();
    public EnemyStatsData default_stats { get; init; } = new();
    public Dictionary<string, EnemyStatsData> enemy_types { get; init; } = new();
}
