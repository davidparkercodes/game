using Game.Application.Shared.Cqrs;

namespace Game.Application.Waves.Commands;

public class StartWaveCommand : ICommand<StartWaveResult>
{
    public int WaveIndex { get; }
    public bool IsRoundBased { get; }

    public StartWaveCommand(int waveIndex, bool isRoundBased = true)
    {
        WaveIndex = waveIndex;
        IsRoundBased = isRoundBased;
    }
}

public class StartWaveResult
{
    public bool Success { get; }
    public string? ErrorMessage { get; }
    public int WaveIndex { get; }
    public int TotalEnemies { get; }
    public string? WaveName { get; }

    public StartWaveResult(bool success, int waveIndex = 0, int totalEnemies = 0, string? waveName = null, string? errorMessage = null)
    {
        Success = success;
        WaveIndex = waveIndex;
        TotalEnemies = totalEnemies;
        WaveName = waveName;
        ErrorMessage = errorMessage;
    }

    public static StartWaveResult Successful(int waveIndex, int totalEnemies, string waveName)
    {
        return new StartWaveResult(true, waveIndex, totalEnemies, waveName);
    }

    public static StartWaveResult Failed(string errorMessage)
    {
        return new StartWaveResult(false, errorMessage: errorMessage);
    }
}
