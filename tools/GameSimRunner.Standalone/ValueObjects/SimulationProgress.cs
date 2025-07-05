namespace GameSimRunner.Standalone.ValueObjects;

public readonly record struct SimulationProgress(int CurrentWave, int CurrentGold, int RemainingLives);
