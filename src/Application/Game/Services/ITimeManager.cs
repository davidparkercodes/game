namespace Game.Application.Game.Services;

public interface ITimeManager
{
    float CurrentTimeScale { get; }
    int CurrentSpeedIndex { get; }
    float[] AvailableSpeeds { get; }
    
    event SpeedChangedEventHandler? SpeedChanged;
    
    void SetGameSpeed(float multiplier);
    void SetGameSpeedByIndex(int speedIndex);
    void CycleToNextSpeed();
    void SetSpeedTo1x();
    void SetSpeedTo2x();
    void SetSpeedTo4x();
    void SetSpeedTo10x();
    void SetSpeedTo20x();
    string GetCurrentSpeedText();
}

public delegate void SpeedChangedEventHandler(float newSpeed, int speedIndex);
