using Game.Application.Game.Services;
using Game.Domain.Common.Services;

namespace Game.Application.Shared.Services;

public class SpeedControlDebugCommands
{
    private static readonly ILogger _logger = new ConsoleLogger("⚡ [SPEED-DEBUG]");

    public static void TestAllSpeedOptions()
    {
        if (TimeManager.Instance == null)
        {
            _logger.LogError("TimeManager not available");
            return;
        }

        _logger.LogInformation("Testing all speed options...");
        
        var timeManager = TimeManager.Instance;
        var availableSpeeds = timeManager.AvailableSpeeds;
        
        _logger.LogInformation($"Available speeds: {string.Join(", ", availableSpeeds)}x");
        _logger.LogInformation($"Current speed: {timeManager.GetCurrentSpeedText()}");
    }

    public static void CycleSpeedForward()
    {
        if (TimeManager.Instance == null)
        {
            _logger.LogError("TimeManager not available");
            return;
        }

        _logger.LogInformation("Cycling to next speed...");
        TimeManager.Instance.CycleToNextSpeed();
    }

    public static void TestSpeedSequence()
    {
        if (TimeManager.Instance == null)
        {
            _logger.LogError("TimeManager not available");
            return;
        }

        _logger.LogInformation("Testing speed sequence: 1x -> 2x -> 4x -> 1x");
        
        var timeManager = TimeManager.Instance;
        
        // Test 1x
        timeManager.SetSpeedTo1x();
        _logger.LogInformation($"Set to 1x: Current = {timeManager.GetCurrentSpeedText()}");
        
        // Test 2x
        timeManager.SetSpeedTo2x();
        _logger.LogInformation($"Set to 2x: Current = {timeManager.GetCurrentSpeedText()}");
        
        // Test 4x
        timeManager.SetSpeedTo4x();
        _logger.LogInformation($"Set to 4x: Current = {timeManager.GetCurrentSpeedText()}");
        
        // Back to 1x
        timeManager.SetSpeedTo1x();
        _logger.LogInformation($"Back to 1x: Current = {timeManager.GetCurrentSpeedText()}");
        
        _logger.LogInformation("Speed sequence test completed!");
    }

    public static void TestKeyboardShortcuts()
    {
        _logger.LogInformation("Keyboard shortcuts available:");
        _logger.LogInformation("  Press '1' key for 1x speed");
        _logger.LogInformation("  Press '2' key for 2x speed");
        _logger.LogInformation("  Press '4' key for 4x speed");
        _logger.LogInformation("Test by pressing these keys during gameplay!");
    }

    public static void ForceResetSpeed()
    {
        if (TimeManager.Instance == null)
        {
            _logger.LogError("TimeManager not available");
            return;
        }

        _logger.LogInformation("Force resetting speed to 1x...");
        TimeManager.Instance.SetSpeedTo1x();
        _logger.LogInformation($"Speed reset complete: {TimeManager.Instance.GetCurrentSpeedText()}");
    }

    public static void PrintCurrentSpeedStatus()
    {
        if (TimeManager.Instance == null)
        {
            _logger.LogError("TimeManager not available");
            return;
        }

        var timeManager = TimeManager.Instance;
        _logger.LogInformation("Current Speed Status:");
        _logger.LogInformation($"  TimeManager Speed: {timeManager.GetCurrentSpeedText()}");
        _logger.LogInformation($"  Speed Index: {timeManager.CurrentSpeedIndex}");
        _logger.LogInformation($"  Available Options: {string.Join(", ", timeManager.AvailableSpeeds)}x");
    }

    public static void PrintSpeedControlCommands()
    {
        _logger.LogInformation("Available Speed Control Debug Commands:");
        _logger.LogInformation("  SpeedControlDebugCommands.TestAllSpeedOptions() - Test all available speeds");
        _logger.LogInformation("  SpeedControlDebugCommands.CycleSpeedForward() - Cycle to next speed");
        _logger.LogInformation("  SpeedControlDebugCommands.TestSpeedSequence() - Test 1x->2x->4x->1x sequence");
        _logger.LogInformation("  SpeedControlDebugCommands.TestKeyboardShortcuts() - Show keyboard shortcuts");
        _logger.LogInformation("  SpeedControlDebugCommands.ForceResetSpeed() - Reset to 1x speed");
        _logger.LogInformation("  SpeedControlDebugCommands.PrintCurrentSpeedStatus() - Show current status");
        _logger.LogInformation("  SpeedControlDebugCommands.PrintSpeedControlCommands() - Show this help");
    }
}
