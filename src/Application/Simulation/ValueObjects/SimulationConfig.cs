namespace Game.Application.Simulation.ValueObjects;

public readonly struct SimulationConfig
{
    public int StartingMoney { get; }
    public int StartingLives { get; }
    public int MaxWaves { get; }
    public int RandomSeed { get; }
    public float EnemyHealthMultiplier { get; }
    public float EnemySpeedMultiplier { get; }
    public float EnemyCountMultiplier { get; }
    public float BuildingCostMultiplier { get; }
    public float BuildingDamageMultiplier { get; }
    public string WaveSetDifficulty { get; }
    public bool FastMode { get; }
    public bool VerboseOutput { get; }

    public SimulationConfig(
        int startingMoney = 500,
        int startingLives = 20,
        int maxWaves = 10,
        int randomSeed = 12345,
        float enemyHealthMultiplier = 1.0f,
        float enemySpeedMultiplier = 1.0f,
        float enemyCountMultiplier = 1.0f,
        float buildingCostMultiplier = 1.0f,
        float buildingDamageMultiplier = 1.0f,
        string waveSetDifficulty = "default",
        bool fastMode = true,
        bool verboseOutput = false)
    {
        StartingMoney = startingMoney;
        StartingLives = startingLives;
        MaxWaves = maxWaves;
        RandomSeed = randomSeed;
        EnemyHealthMultiplier = enemyHealthMultiplier;
        EnemySpeedMultiplier = enemySpeedMultiplier;
        EnemyCountMultiplier = enemyCountMultiplier;
        BuildingCostMultiplier = buildingCostMultiplier;
        BuildingDamageMultiplier = buildingDamageMultiplier;
        WaveSetDifficulty = waveSetDifficulty;
        FastMode = fastMode;
        VerboseOutput = verboseOutput;
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
            enemyCountMultiplier: 1.0f,
            buildingCostMultiplier: 1.0f,
            buildingDamageMultiplier: 1.0f,
            waveSetDifficulty: "default",
            fastMode: true,
            verboseOutput: false
        );
    }

    public static SimulationConfig ForBalanceTesting()
    {
        return new SimulationConfig(
            startingMoney: 500,
            startingLives: 20,
            maxWaves: 5,
            randomSeed: 42, // Fixed seed for deterministic testing
            enemyHealthMultiplier: 1.0f,
            enemySpeedMultiplier: 1.0f,
            enemyCountMultiplier: 1.0f,
            buildingCostMultiplier: 1.0f,
            buildingDamageMultiplier: 1.0f,
            waveSetDifficulty: "balance-testing",
            fastMode: true,
            verboseOutput: false
        );
    }

    public static SimulationConfig WithDifficultyModifier(float difficultyMultiplier)
    {
        return new SimulationConfig(
            enemyHealthMultiplier: difficultyMultiplier,
            enemySpeedMultiplier: 1.0f + (difficultyMultiplier - 1.0f) * 0.5f,
            enemyCountMultiplier: 1.0f + (difficultyMultiplier - 1.0f) * 0.3f
        );
    }

    public static SimulationConfig QuickBalance() => new SimulationConfig(
        maxWaves: 5, 
        waveSetDifficulty: "balance-testing", 
        fastMode: true
    );

    public static SimulationConfig EnemyHealthTest(float multiplier) => new SimulationConfig(
        enemyHealthMultiplier: multiplier, 
        waveSetDifficulty: "balance-testing"
    );

    public static SimulationConfig DifficultyTest(float difficulty) => new SimulationConfig(
        enemyHealthMultiplier: difficulty,
        enemyCountMultiplier: 1.0f + (difficulty - 1.0f) * 0.5f,
        waveSetDifficulty: "balance-testing"
    );
}
