using Godot;
using System.Collections.Generic;

[System.Serializable]
public class EnemyStatsData
{
    public int max_health { get; set; }
    public float speed { get; set; }
    public int damage { get; set; }
    public int reward_gold { get; set; }
    public int reward_xp { get; set; }
    public string description { get; set; }

    public EnemyStatsData()
    {
        max_health = 100;
        speed = 60.0f;
        damage = 10;
        reward_gold = 5;
        reward_xp = 10;
        description = "";
    }
}

[System.Serializable]
public class EnemyStatsConfig
{
    public Dictionary<string, EnemyStatsData> enemy_types { get; set; }
    public EnemyStatsData default_stats { get; set; }

    public EnemyStatsConfig()
    {
        enemy_types = new Dictionary<string, EnemyStatsData>();
        default_stats = new EnemyStatsData();
    }
}
