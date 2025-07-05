using System.Threading;
using System.Threading.Tasks;
using Game.Application.Shared.Cqrs;
using Game.Application.Waves.Commands;
using Game.Infrastructure.Enemies.Services;

namespace Game.Application.Waves.Handlers;

public class StartWaveCommandHandler : ICommandHandler<StartWaveCommand, StartWaveResult>
{
    public Task<StartWaveResult> HandleAsync(StartWaveCommand command, CancellationToken cancellationToken = default)
    {
        if (command == null)
            return Task.FromResult(StartWaveResult.Failed("Command cannot be null"));

        if (command.WaveIndex < 0)
            return Task.FromResult(StartWaveResult.Failed("Wave index cannot be negative"));

        var waveSpawnerService = WaveSpawnerService.Instance;
        if (waveSpawnerService == null)
            return Task.FromResult(StartWaveResult.Failed("WaveSpawnerService is not available"));

        if (command.WaveIndex >= waveSpawnerService.GetTotalWaves())
            return Task.FromResult(StartWaveResult.Failed($"Wave index {command.WaveIndex} is out of range. Total waves: {waveSpawnerService.GetTotalWaves()}"));

        if (waveSpawnerService.IsSpawning)
            return Task.FromResult(StartWaveResult.Failed($"Wave {waveSpawnerService.CurrentWaveIndex} is already active"));

        try
        {
            waveSpawnerService.StartWave(command.WaveIndex + 1);
            var waveName = $"Wave {command.WaveIndex + 1}";
            var totalEnemies = waveSpawnerService.TotalEnemiesInWave;

            return Task.FromResult(StartWaveResult.Successful(command.WaveIndex, totalEnemies, waveName));
        }
        catch (System.Exception ex)
        {
            return Task.FromResult(StartWaveResult.Failed($"Failed to start wave: {ex.Message}"));
        }
    }
}
