using Game.Application.Game.Configuration;

namespace Game.Application.Game.Services;

public interface ITimeManagementConfigService
{
    TimeManagementConfig GetTimeManagementConfig();
    float[] GetSpeedOptions();
    int GetDefaultSpeedIndex();
    float GetSpeedTolerance();
}
