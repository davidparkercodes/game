using System.IO;
using System.Text.Json;
using Game.Application.Game.Configuration;
using Game.Application.Game.Services;

namespace Game.Infrastructure.Game;

public class TimeManagementConfigService : ITimeManagementConfigService
{
    private readonly TimeManagementConfig _config;

    public TimeManagementConfigService()
    {
        var jsonString = File.ReadAllText("config/gameplay/time_management.json");
        _config = JsonSerializer.Deserialize<TimeManagementConfig>(jsonString) ?? throw new FileNotFoundException("Configuration file 'config/gameplay/time_management.json' not found.");
    }

    public TimeManagementConfig GetTimeManagementConfig() => _config;

    public float[] GetSpeedOptions() => _config.SpeedOptions;

    public int GetDefaultSpeedIndex() => _config.DefaultSpeedIndex;

    public float GetSpeedTolerance() => _config.SpeedTolerance;
}
