namespace Game.Application.Simulation.ValueObjects;

public readonly record struct SimulationProgress(int CurrentWave, int CurrentGold, int RemainingLives);
