using Godot;
using System.Collections.Generic;

[System.Serializable]
public class BuildingStatsData
{
    public int cost { get; set; }
    public int damage { get; set; }
    public float range { get; set; }
    public float fire_rate { get; set; }
    public float bullet_speed { get; set; }
    public string shoot_sound { get; set; }
    public string impact_sound { get; set; }
    public string description { get; set; }

    public BuildingStatsData()
    {
        cost = 10;
        damage = 10;
        range = 150.0f;
        fire_rate = 1.0f;
        bullet_speed = 900.0f;
        shoot_sound = "basic_turret_shoot";
        impact_sound = "basic_bullet_impact";
        description = "";
    }
}

[System.Serializable]
public class BuildingStatsConfig
{
    public Dictionary<string, BuildingStatsData> building_types { get; set; }
    public BuildingStatsData default_stats { get; set; }

    public BuildingStatsConfig()
    {
        building_types = new Dictionary<string, BuildingStatsData>();
        default_stats = new BuildingStatsData();
    }
}
