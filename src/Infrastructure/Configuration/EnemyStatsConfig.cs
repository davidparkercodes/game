using System.Collections.Generic;
using Game.Domain.Enemies.ValueObjects;

namespace Game.Infrastructure.Configuration;

public class EnemyStatsConfig
{
    // NO HARDCODED VALUES! All data must come from config files.
    public EnemyStatsData default_stats { get; set; } = EnemyStatsData.Empty;
    public Dictionary<string, EnemyStatsData> enemy_types { get; set; } = new();

    // Remove parameterless constructor to force explicit initialization
    // All data should be loaded from JSON files
}
