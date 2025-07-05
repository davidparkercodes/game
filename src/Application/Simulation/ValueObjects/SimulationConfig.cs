namespace Game.Application.Simulation.ValueObjects;

public readonly struct SimulationConfig
{
    public int StartingMoney { get; }
    public int StartingLives { get; }
    public int MaxWaves { get; }
    public int RandomSeed { get; }
    public float EnemyHealthMultiplier { get; }
    public float EnemySpeedMultiplier { get; }
    public float BuildingCostMultiplier { get; }
    public bool FastMode { get; }

    public SimulationConfig(
        int startingMoney = 500,
        int startingLives = 20,
        int maxWaves = 10,
        int randomSeed = 12345,
        float enemyHealthMultiplier = 1.0f,
        float enemySpeedMultiplier = 1.0f,
        float buildingCostMultiplier = 1.0f,
        bool fastMode = true)
    {
        StartingMoney = startingMoney;
        StartingLives = startingLives;
        MaxWaves = maxWaves;
        RandomSeed = randomSeed;
        EnemyHealthMultiplier = enemyHealthMultiplier;
        EnemySpeedMultiplier = enemySpeedMultiplier;
        BuildingCostMultiplier = buildingCostMultiplier;
        FastMode = fastMode;
    }

    public static SimulationConfig Default()
    {
        return new SimulationConfig(
            startingMoney: 500,
            startingLives: 20,
            maxWaves: 10,
            randomSeed: 12345,
            enemyHealthMultiplier: 1.0f,
            enemySpeedMultiplier: 1.0f,
            buildingCostMultiplier: 1.0f,
            fastMode: true
        );
    }

    public static SimulationConfig ForBalanceTesting()
    {
        return new SimulationConfig(
            startingMoney: 500,
            startingLives: 20,
            maxWaves: 10,
            randomSeed: 42, // Fixed seed for deterministic testing
            fastMode: true
        );
    }

    public static SimulationConfig WithDifficultyModifier(float difficultyMultiplier)
    {
        return new SimulationConfig(
            enemyHealthMultiplier: difficultyMultiplier,
            enemySpeedMultiplier: 1.0f + (difficultyMultiplier - 1.0f) * 0.5f
        );
    }
}
