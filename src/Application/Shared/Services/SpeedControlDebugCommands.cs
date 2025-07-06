using Godot;
using Game.Application.Game.Services;

namespace Game.Application.Shared.Services;

public class SpeedControlDebugCommands
{
    private const string LogPrefix = "⚡ [SPEED-DEBUG]";

    public static void TestAllSpeedOptions()
    {
        if (TimeManager.Instance == null)
        {
            GD.PrintErr($"{LogPrefix} TimeManager not available");
            return;
        }

        GD.Print($"{LogPrefix} Testing all speed options...");
        
        var timeManager = TimeManager.Instance;
        var availableSpeeds = timeManager.AvailableSpeeds;
        
        GD.Print($"{LogPrefix} Available speeds: {string.Join(", ", availableSpeeds)}x");
        GD.Print($"{LogPrefix} Current speed: {timeManager.GetCurrentSpeedText()}");
        GD.Print($"{LogPrefix} Current Engine.TimeScale: {Engine.TimeScale}");
    }

    public static void CycleSpeedForward()
    {
        if (TimeManager.Instance == null)
        {
            GD.PrintErr($"{LogPrefix} TimeManager not available");
            return;
        }

        GD.Print($"{LogPrefix} Cycling to next speed...");
        TimeManager.Instance.CycleToNextSpeed();
    }

    public static void TestSpeedSequence()
    {
        if (TimeManager.Instance == null)
        {
            GD.PrintErr($"{LogPrefix} TimeManager not available");
            return;
        }

        GD.Print($"{LogPrefix} Testing speed sequence: 1x -> 2x -> 4x -> 1x");
        
        var timeManager = TimeManager.Instance;
        
        // Test 1x
        timeManager.SetSpeedTo1x();
        GD.Print($"{LogPrefix} Set to 1x: Current = {timeManager.GetCurrentSpeedText()}, Engine = {Engine.TimeScale}");
        
        // Test 2x
        timeManager.SetSpeedTo2x();
        GD.Print($"{LogPrefix} Set to 2x: Current = {timeManager.GetCurrentSpeedText()}, Engine = {Engine.TimeScale}");
        
        // Test 4x
        timeManager.SetSpeedTo4x();
        GD.Print($"{LogPrefix} Set to 4x: Current = {timeManager.GetCurrentSpeedText()}, Engine = {Engine.TimeScale}");
        
        // Back to 1x
        timeManager.SetSpeedTo1x();
        GD.Print($"{LogPrefix} Back to 1x: Current = {timeManager.GetCurrentSpeedText()}, Engine = {Engine.TimeScale}");
        
        GD.Print($"{LogPrefix} Speed sequence test completed!");
    }

    public static void TestKeyboardShortcuts()
    {
        GD.Print($"{LogPrefix} Keyboard shortcuts available:");
        GD.Print($"{LogPrefix}   Press '1' key for 1x speed");
        GD.Print($"{LogPrefix}   Press '2' key for 2x speed");
        GD.Print($"{LogPrefix}   Press '4' key for 4x speed");
        GD.Print($"{LogPrefix} Test by pressing these keys during gameplay!");
    }

    public static void ForceResetSpeed()
    {
        if (TimeManager.Instance == null)
        {
            GD.PrintErr($"{LogPrefix} TimeManager not available");
            return;
        }

        GD.Print($"{LogPrefix} Force resetting speed to 1x...");
        TimeManager.Instance.SetSpeedTo1x();
        GD.Print($"{LogPrefix} Speed reset complete: {TimeManager.Instance.GetCurrentSpeedText()}");
    }

    public static void PrintCurrentSpeedStatus()
    {
        if (TimeManager.Instance == null)
        {
            GD.PrintErr($"{LogPrefix} TimeManager not available");
            return;
        }

        var timeManager = TimeManager.Instance;
        GD.Print($"{LogPrefix} Current Speed Status:");
        GD.Print($"{LogPrefix}   TimeManager Speed: {timeManager.GetCurrentSpeedText()}");
        GD.Print($"{LogPrefix}   Speed Index: {timeManager.CurrentSpeedIndex}");
        GD.Print($"{LogPrefix}   Engine.TimeScale: {Engine.TimeScale}");
        GD.Print($"{LogPrefix}   Available Options: {string.Join(", ", timeManager.AvailableSpeeds)}x");
    }

    public static void PrintSpeedControlCommands()
    {
        GD.Print($"{LogPrefix} Available Speed Control Debug Commands:");
        GD.Print($"{LogPrefix}   SpeedControlDebugCommands.TestAllSpeedOptions() - Test all available speeds");
        GD.Print($"{LogPrefix}   SpeedControlDebugCommands.CycleSpeedForward() - Cycle to next speed");
        GD.Print($"{LogPrefix}   SpeedControlDebugCommands.TestSpeedSequence() - Test 1x->2x->4x->1x sequence");
        GD.Print($"{LogPrefix}   SpeedControlDebugCommands.TestKeyboardShortcuts() - Show keyboard shortcuts");
        GD.Print($"{LogPrefix}   SpeedControlDebugCommands.ForceResetSpeed() - Reset to 1x speed");
        GD.Print($"{LogPrefix}   SpeedControlDebugCommands.PrintCurrentSpeedStatus() - Show current status");
        GD.Print($"{LogPrefix}   SpeedControlDebugCommands.PrintSpeedControlCommands() - Show this help");
    }
}
