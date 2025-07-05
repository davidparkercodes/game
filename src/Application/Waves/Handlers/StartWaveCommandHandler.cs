using System.Threading;
using System.Threading.Tasks;
using Game.Application.Shared.Cqrs;
using Game.Application.Waves.Commands;
using Game.Infrastructure.Managers;

namespace Game.Application.Waves.Handlers;

public class StartWaveCommandHandler : ICommandHandler<StartWaveCommand, StartWaveResult>
{
    public Task<StartWaveResult> HandleAsync(StartWaveCommand command, CancellationToken cancellationToken = default)
    {
        if (command == null)
            return Task.FromResult(StartWaveResult.Failed("Command cannot be null"));

        if (command.WaveIndex < 0)
            return Task.FromResult(StartWaveResult.Failed("Wave index cannot be negative"));

        var waveSpawner = WaveSpawner.Instance;
        if (waveSpawner == null)
            return Task.FromResult(StartWaveResult.Failed("WaveSpawner is not available"));

        if (command.WaveIndex >= waveSpawner.GetTotalWaves())
            return Task.FromResult(StartWaveResult.Failed($"Wave index {command.WaveIndex} is out of range. Total waves: {waveSpawner.GetTotalWaves()}"));

        if (waveSpawner.IsSpawning)
            return Task.FromResult(StartWaveResult.Failed($"Wave {waveSpawner.CurrentWaveIndex} is already active"));

        try
        {
            waveSpawner.StartWave(command.WaveIndex + 1);
            var waveName = $"Wave {command.WaveIndex + 1}";
            var totalEnemies = waveSpawner.TotalEnemiesInWave;

            return Task.FromResult(StartWaveResult.Successful(command.WaveIndex, totalEnemies, waveName));
        }
        catch (System.Exception ex)
        {
            return Task.FromResult(StartWaveResult.Failed($"Failed to start wave: {ex.Message}"));
        }
    }
}
