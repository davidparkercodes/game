using System.Collections.Generic;
using Game.Domain.Enemies.ValueObjects;

namespace Game.Application.Enemies.Configuration;

public class EnemyStatsConfig
{
    public EnemyStatsData default_stats { get; init; } = new();
    public Dictionary<string, EnemyStatsData> enemy_types { get; init; } = new();
}
