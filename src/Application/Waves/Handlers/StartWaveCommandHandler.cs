using System.Threading;
using System.Threading.Tasks;
using Game.Application.Shared.Cqrs;
using Game.Application.Waves.Commands;
using Game.Domain.Enemies.Services;

namespace Game.Application.Waves.Handlers;

public class StartWaveCommandHandler : ICommandHandler<StartWaveCommand, StartWaveResult>
{
    private readonly IWaveService _waveService;
    
    public StartWaveCommandHandler(IWaveService waveService)
    {
        _waveService = waveService ?? throw new System.ArgumentNullException(nameof(waveService));
    }
    
    public Task<StartWaveResult> HandleAsync(StartWaveCommand command, CancellationToken cancellationToken = default)
    {
        if (command == null)
            return Task.FromResult(StartWaveResult.Failed("Command cannot be null"));

        if (command.WaveIndex < 0)
            return Task.FromResult(StartWaveResult.Failed("Wave index cannot be negative"));

        if (_waveService.IsWaveActive())
            return Task.FromResult(StartWaveResult.Failed($"Wave {_waveService.GetCurrentWaveNumber()} is already active"));

        try
        {
            _waveService.StartWave(command.WaveIndex + 1);
            var waveName = $"Wave {command.WaveIndex + 1}";
            var totalEnemies = _waveService.GetRemainingEnemies();

            return Task.FromResult(StartWaveResult.Successful(command.WaveIndex, totalEnemies, waveName));
        }
        catch (System.Exception ex)
        {
            return Task.FromResult(StartWaveResult.Failed($"Failed to start wave: {ex.Message}"));
        }
    }
}
