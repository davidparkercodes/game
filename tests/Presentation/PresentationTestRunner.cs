using Godot;
using System.Threading.Tasks;

namespace Game.Tests.Presentation;

public partial class PresentationTestRunner : Node
{
    private const string LogPrefix = "🧪 [TEST-RUNNER]";

    public override void _Ready()
    {
        GD.Print($"{LogPrefix} Starting Presentation Test Suite...");
        CallDeferred(nameof(RunAllTestSuites));
    }

    private async void RunAllTestSuites()
    {
        await Task.Delay(500); // Allow scene to initialize

        GD.Print($"{LogPrefix} ========================================");
        GD.Print($"{LogPrefix} 🎯 PRESENTATION LAYER TEST SUITE");
        GD.Print($"{LogPrefix} ========================================");

        // Phase 3: Scene Management Testing
        await RunMainIntegrationTests();
        await Task.Delay(1000);

        // Phase 4: Visual Feedback Testing
        await RunVisualFeedbackTests();
        await Task.Delay(1000);

        // Existing HUD Integration Tests
        await RunHudIntegrationTests();

        GD.Print($"{LogPrefix} ========================================");
        GD.Print($"{LogPrefix} 🎉 ALL PRESENTATION TESTS COMPLETED!");
        GD.Print($"{LogPrefix} ========================================");
    }

    private async Task RunMainIntegrationTests()
    {
        GD.Print($"{LogPrefix} 🔧 Running Phase 3: Main Integration Tests...");
        
        try
        {
            var mainTests = new Core.MainIntegrationTests();
            AddChild(mainTests);
            await Task.Delay(5000); // Allow tests to complete
            mainTests.QueueFree();
            
            GD.Print($"{LogPrefix} ✅ Main Integration Tests completed");
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix} ❌ Main Integration Tests failed: {ex.Message}");
        }
    }

    private async Task RunVisualFeedbackTests()
    {
        GD.Print($"{LogPrefix} 🎨 Running Phase 4: Visual Feedback Tests...");
        
        try
        {
            var visualTests = new UI.VisualFeedbackTests();
            AddChild(visualTests);
            await Task.Delay(8000); // Allow tests to complete (visual tests take longer)
            visualTests.QueueFree();
            
            GD.Print($"{LogPrefix} ✅ Visual Feedback Tests completed");
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix} ❌ Visual Feedback Tests failed: {ex.Message}");
        }
    }

    private async Task RunHudIntegrationTests()
    {
        GD.Print($"{LogPrefix} 📊 Running Existing: HUD Integration Tests...");
        
        try
        {
            var hudTests = new UI.HudIntegrationTests();
            AddChild(hudTests);
            await Task.Delay(6000); // Allow tests to complete
            hudTests.QueueFree();
            
            GD.Print($"{LogPrefix} ✅ HUD Integration Tests completed");
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix} ❌ HUD Integration Tests failed: {ex.Message}");
        }
    }

    public override void _Input(InputEvent @event)
    {
        // Allow ESC to exit test runner
        if (@event is InputEventKey key && key.Pressed && key.Keycode == Key.Escape)
        {
            GD.Print($"{LogPrefix} Test runner terminated by user");
            GetTree().Quit();
        }
    }

    public override void _ExitTree()
    {
        GD.Print($"{LogPrefix} Presentation test runner finished");
    }
}
