using Godot;

namespace Game.Application.Game.Services;

public partial class TimeManager : Node
{
    public static TimeManager? Instance { get; private set; }
    
    private float _currentTimeScale = 1.0f;
    private int _currentSpeedIndex = 0;
    private readonly float[] _speedOptions = { 1.0f, 2.0f, 4.0f, 10.0f, 20.0f };
    
    public delegate void SpeedChangedEventHandler(float newSpeed, int speedIndex);
    public event SpeedChangedEventHandler? SpeedChanged;
    
    public float CurrentTimeScale => _currentTimeScale;
    public int CurrentSpeedIndex => _currentSpeedIndex;
    public float[] AvailableSpeeds => _speedOptions;

    public override void _Ready()
    {
        Instance = this;
        SetGameSpeed(1.0f); // Start at normal speed
        GD.Print("⚡ TimeManager initialized with 1x speed");
    }

    public void SetGameSpeed(float multiplier)
    {
        // Find the closest speed option
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
            GD.PrintErr($"⚠️ TimeManager: Invalid speed index {speedIndex}, using index 0");
            speedIndex = 0;
        }

        _currentSpeedIndex = speedIndex;
        _currentTimeScale = _speedOptions[speedIndex];
        
        // Apply the time scale to Godot's engine
        Engine.TimeScale = _currentTimeScale;
        
        // Notify listeners about the speed change
        SpeedChanged?.Invoke(_currentTimeScale, _currentSpeedIndex);
        
        var speedText = _currentTimeScale == 1.0f ? "1x" : 
                       _currentTimeScale == 2.0f ? "2x" : 
                       _currentTimeScale == 4.0f ? "4x" : 
                       _currentTimeScale == 10.0f ? "10x" : 
                       _currentTimeScale == 20.0f ? "20x" : $"{_currentTimeScale}x";
        
        GD.Print($"⚡ TimeManager: Game speed set to {speedText} (Engine.TimeScale = {Engine.TimeScale})");
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

    // Note: Keyboard shortcuts removed to avoid conflict with building selection
    // Speed control is handled through the UI buttons only

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
        if (Instance == this)
        {
            // Reset time scale when exiting
            Engine.TimeScale = 1.0f;
            Instance = null;
            GD.Print("⚡ TimeManager cleaned up, time scale reset to 1x");
        }
    }
}
