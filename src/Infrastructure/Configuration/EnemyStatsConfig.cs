using System.Collections.Generic;
using Game.Domain.Enemies.ValueObjects;

namespace Game.Infrastructure.Configuration;

public class EnemyStatsConfig
{
    public EnemyStatsData default_stats { get; set; } = EnemyStatsData.CreateDefault();
    public Dictionary<string, EnemyStatsData> enemy_types { get; set; } = new();

    public EnemyStatsConfig()
    {
        InitializeDefaults();
    }

    private void InitializeDefaults()
    {
        enemy_types["basic_enemy"] = EnemyStatsData.CreateDefault();
        enemy_types["fast_enemy"] = new EnemyStatsData(
            maxHealth: 50,
            speed: 100f,
            damage: 5,
            rewardGold: 8,
            rewardXp: 3,
            description: "Fast but weak enemy"
        );
        enemy_types["tank_enemy"] = new EnemyStatsData(
            maxHealth: 200,
            speed: 25f,
            damage: 20,
            rewardGold: 20,
            rewardXp: 10,
            description: "Slow but tough enemy"
        );
    }
}
