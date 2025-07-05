using System.Diagnostics;
using GameSimRunner.Standalone.ValueObjects;

namespace GameSimRunner.Standalone;

public class GameSimRunner
{
    public async Task<SimulationResult> RunSimulationAsync(SimulationConfig config, IProgress<SimulationProgress>? progress = null)
    {
        var stopwatch = Stopwatch.StartNew();
        
        // Simulate game state
        int money = config.StartingMoney;
        int lives = config.StartingLives;
        
        // Report initial progress
        progress?.Report(new SimulationProgress(0, money, lives));
        
        // Simulate waves
        for (int wave = 1; wave <= config.MaxWaves; wave++)
        {
            // Simulate wave processing time
            await Task.Delay(100); // Simulate wave processing
            
            // Simulate wave outcome
            var random = new Random(config.RandomSeed + wave);
            
            // Simulate money gain/loss
            money += random.Next(10, 50);
            
            // Simulate potential life loss
            if (random.NextDouble() < 0.3) // 30% chance to lose a life
            {
                lives--;
            }
            
            // Report progress
            progress?.Report(new SimulationProgress(wave, money, lives));
            
            // Check if game over
            if (lives <= 0)
            {
                stopwatch.Stop();
                return SimulationResult.Failure();
            }
        }
        
        stopwatch.Stop();
        return SimulationResult.CreateSuccess(money, lives, stopwatch.Elapsed);
    }
}
