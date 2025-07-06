using Godot;
using Game.Application.Game.Services;
using Game.Domain.Common.Services;

namespace Game.Infrastructure.Game;

public partial class GodotTimeManager : Node, ITimeManager
{
    private readonly ILogger _logger;
    
    private float _currentTimeScale = 1.0f;
    private int _currentSpeedIndex = 0;
    private readonly float[] _speedOptions = { 1.0f, 2.0f, 4.0f, 10.0f, 20.0f };
    
    public event SpeedChangedEventHandler? SpeedChanged;
    
    public float CurrentTimeScale => _currentTimeScale;
    public int CurrentSpeedIndex => _currentSpeedIndex;
    public float[] AvailableSpeeds => _speedOptions;

    public GodotTimeManager(ILogger logger)
    {
        _logger = logger;
    }

    public override void _Ready()
    {
        SetGameSpeed(1.0f);
        _logger.LogInformation("TimeManager initialized with 1x speed");
    }

    public void SetGameSpeed(float multiplier)
    {
        int speedIndex = 0;
        for (int i = 0; i < _speedOptions.Length; i++)
        {
            if (Mathf.Abs(_speedOptions[i] - multiplier) < 0.01f)
            {
                speedIndex = i;
                break;
            }
        }
        
        SetGameSpeedByIndex(speedIndex);
    }

    public void SetGameSpeedByIndex(int speedIndex)
    {
        if (speedIndex < 0 || speedIndex >= _speedOptions.Length)
        {
            _logger.LogWarning($"Invalid speed index {speedIndex}, using index 0");
            speedIndex = 0;
        }

        _currentSpeedIndex = speedIndex;
        _currentTimeScale = _speedOptions[speedIndex];
        
        Engine.TimeScale = _currentTimeScale;
        
        SpeedChanged?.Invoke(_currentTimeScale, _currentSpeedIndex);
        
        var speedText = GetCurrentSpeedText();
        _logger.LogInformation($"Game speed set to {speedText} (Engine.TimeScale = {Engine.TimeScale})");
    }

    public void CycleToNextSpeed()
    {
        int nextIndex = (_currentSpeedIndex + 1) % _speedOptions.Length;
        SetGameSpeedByIndex(nextIndex);
    }

    public void SetSpeedTo1x()
    {
        SetGameSpeedByIndex(0);
    }

    public void SetSpeedTo2x()
    {
        SetGameSpeedByIndex(1);
    }

    public void SetSpeedTo4x()
    {
        SetGameSpeedByIndex(2);
    }

    public void SetSpeedTo10x()
    {
        SetGameSpeedByIndex(3);
    }

    public void SetSpeedTo20x()
    {
        SetGameSpeedByIndex(4);
    }

    public string GetCurrentSpeedText()
    {
        return _currentTimeScale switch
        {
            1.0f => "1x",
            2.0f => "2x", 
            4.0f => "4x",
            10.0f => "10x",
            20.0f => "20x",
            _ => $"{_currentTimeScale:F1}x"
        };
    }

    public override void _ExitTree()
    {
        Engine.TimeScale = 1.0f;
        _logger.LogInformation("TimeManager cleaned up, time scale reset to 1x");
    }
}
