using System.Collections.Generic;

namespace Game.Application.Simulation.Configuration;

public class PlacementStrategyConfig
{
    public StrategiesConfig strategies { get; set; } = new();
    public FallbackStrategyConfig fallback_strategy { get; set; } = new();
    public Dictionary<string, int> cost_thresholds { get; set; } = new();
}

public class StrategiesConfig
{
    public InitialWaveConfig initial_wave { get; set; } = new();
    public Dictionary<string, WaveUpgradeConfig> wave_upgrades { get; set; } = new();
}

public class InitialWaveConfig
{
    public string building_category { get; set; } = "";
    public List<List<int>> positions { get; set; } = new();
    public int max_cost_per_building { get; set; }
    public string description { get; set; } = "";
}

public class WaveUpgradeConfig
{
    public string category { get; set; } = "";
    public int cost_threshold { get; set; }
    public List<int> position { get; set; } = new();
    public string description { get; set; } = "";
}

public class FallbackStrategyConfig
{
    public bool use_default_type { get; set; }
    public bool use_cheapest_type { get; set; }
    public string emergency_fallback { get; set; } = "";
}
