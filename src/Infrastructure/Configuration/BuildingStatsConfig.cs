using System.Collections.Generic;
using Game.Domain.Buildings.ValueObjects;

namespace Game.Infrastructure.Configuration;

public class BuildingStatsConfig
{
    public BuildingStatsData default_stats { get; set; } = BuildingStatsData.CreateDefault();
    public Dictionary<string, BuildingStatsData> building_types { get; set; } = new();

    public BuildingStatsConfig()
    {
        InitializeDefaults();
    }

    private void InitializeDefaults()
    {
        building_types["basic_tower"] = BuildingStatsData.CreateDefault();
        building_types["cannon_tower"] = new BuildingStatsData(
            cost: 150,
            damage: 50,
            range: 120f,
            attackSpeed: 0.5f,
            upgradeCost: 75,
            description: "High damage but slow attack speed"
        );
        building_types["machine_gun_tower"] = new BuildingStatsData(
            cost: 120,
            damage: 15,
            range: 100f,
            attackSpeed: 3.0f,
            upgradeCost: 60,
            description: "Fast attack speed but low damage"
        );
    }
}
