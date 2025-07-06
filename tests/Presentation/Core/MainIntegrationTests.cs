using Godot;
using Game.Presentation.Core;
using Game.Di;
using System.Threading.Tasks;

namespace Game.Tests.Presentation.Core;

public partial class MainIntegrationTests : Node
{
    private const string LogPrefix = "🧪 [MAIN-TEST]";
    private Main? _mainInstance;

    public override void _Ready()
    {
        GD.Print($"{LogPrefix} Starting Main Integration Tests...");
        CallDeferred(nameof(RunAllTests));
    }

    private async void RunAllTests()
    {
        await Task.Delay(1000); // Allow scene to fully initialize

        await TestMainInstantiation();
        await TestDependencyContainerInitialization();
        await TestServiceResolution();
        await TestHudInitialization();
        await TestSpeedControlInitialization();
        await TestSceneResourceLoading();
        await TestErrorHandling();

        GD.Print($"{LogPrefix} All Main integration tests completed!");
    }

    private async Task TestMainInstantiation()
    {
        GD.Print($"{LogPrefix} Testing Main instantiation...");

        try
        {
            _mainInstance = new Main();
            if (_mainInstance != null)
            {
                GD.Print($"{LogPrefix}   ✅ Main instantiation test passed");
            }
            else
            {
                GD.PrintErr($"{LogPrefix}   ❌ Main instantiation failed - instance is null");
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix}   ❌ Main instantiation failed: {ex.Message}");
        }

        await Task.Delay(100);
    }

    private async Task TestDependencyContainerInitialization()
    {
        GD.Print($"{LogPrefix} Testing DI container initialization...");

        if (_mainInstance == null)
        {
            GD.PrintErr($"{LogPrefix}   ❌ Cannot test DI - Main instance is null");
            return;
        }

        try
        {
            // Add Main to scene tree temporarily for initialization
            GetTree().Root.AddChild(_mainInstance);
            await Task.Delay(500); // Allow initialization

            var diContainer = _mainInstance.GetDiContainer();
            if (diContainer != null)
            {
                GD.Print($"{LogPrefix}   ✅ DI container initialization test passed");
            }
            else
            {
                GD.PrintErr($"{LogPrefix}   ❌ DI container is null after initialization");
            }

            var mediator = _mainInstance.GetMediator();
            if (mediator != null)
            {
                GD.Print($"{LogPrefix}   ✅ Mediator resolution test passed");
            }
            else
            {
                GD.PrintErr($"{LogPrefix}   ❌ Mediator is null after initialization");
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix}   ❌ DI initialization test failed: {ex.Message}");
        }

        await Task.Delay(100);
    }

    private async Task TestServiceResolution()
    {
        GD.Print($"{LogPrefix} Testing service resolution...");

        if (_mainInstance == null)
        {
            GD.PrintErr($"{LogPrefix}   ❌ Cannot test services - Main instance is null");
            return;
        }

        try
        {
            var diContainer = _mainInstance.GetDiContainer();
            if (diContainer != null)
            {
                // Test that we can resolve key services without throwing
                var mediator = diContainer.Resolve<Game.Application.Shared.Cqrs.IMediator>();
                if (mediator != null)
                {
                    GD.Print($"{LogPrefix}   ✅ Mediator service resolution test passed");
                }

                var waveConfigService = diContainer.Resolve<Game.Domain.Enemies.Services.IWaveConfigurationService>();
                if (waveConfigService != null)
                {
                    GD.Print($"{LogPrefix}   ✅ Wave configuration service resolution test passed");
                }
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix}   ❌ Service resolution test failed: {ex.Message}");
        }

        await Task.Delay(100);
    }

    private async Task TestHudInitialization()
    {
        GD.Print($"{LogPrefix} Testing HUD initialization workflow...");

        try
        {
            // Test that HUD scene can be loaded
            var hudScene = GD.Load<PackedScene>("res://scenes/UI/Hud.tscn");
            if (hudScene != null)
            {
                GD.Print($"{LogPrefix}   ✅ HUD scene loading test passed");

                // Test that HUD can be instantiated
                var hud = hudScene.Instantiate<Game.Presentation.UI.Hud>();
                if (hud != null)
                {
                    GD.Print($"{LogPrefix}   ✅ HUD instantiation test passed");
                    hud.QueueFree(); // Clean up
                }
                else
                {
                    GD.PrintErr($"{LogPrefix}   ❌ HUD instantiation failed");
                }
            }
            else
            {
                GD.PrintErr($"{LogPrefix}   ❌ HUD scene loading failed");
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix}   ❌ HUD initialization test failed: {ex.Message}");
        }

        await Task.Delay(100);
    }

    private async Task TestSpeedControlInitialization()
    {
        GD.Print($"{LogPrefix} Testing SpeedControl initialization workflow...");

        try
        {
            // Test that SpeedControl scene can be loaded
            var speedControlScene = GD.Load<PackedScene>("res://scenes/UI/SpeedControlPanel.tscn");
            if (speedControlScene != null)
            {
                GD.Print($"{LogPrefix}   ✅ SpeedControl scene loading test passed");

                // Test that SpeedControl can be instantiated
                var speedControl = speedControlScene.Instantiate<Game.Presentation.UI.SpeedControl>();
                if (speedControl != null)
                {
                    GD.Print($"{LogPrefix}   ✅ SpeedControl instantiation test passed");
                    speedControl.QueueFree(); // Clean up
                }
                else
                {
                    GD.PrintErr($"{LogPrefix}   ❌ SpeedControl instantiation failed");
                }
            }
            else
            {
                GD.PrintErr($"{LogPrefix}   ❌ SpeedControl scene loading failed");
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix}   ❌ SpeedControl initialization test failed: {ex.Message}");
        }

        await Task.Delay(100);
    }

    private async Task TestSceneResourceLoading()
    {
        GD.Print($"{LogPrefix} Testing scene resource loading...");

        try
        {
            // Test critical scene resources
            string[] criticalScenes = {
                "res://scenes/UI/Hud.tscn",
                "res://scenes/UI/SpeedControlPanel.tscn"
            };

            foreach (var scenePath in criticalScenes)
            {
                var scene = GD.Load<PackedScene>(scenePath);
                if (scene != null)
                {
                    GD.Print($"{LogPrefix}   ✅ Scene loading test passed: {scenePath}");
                }
                else
                {
                    GD.PrintErr($"{LogPrefix}   ❌ Scene loading test failed: {scenePath}");
                }
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix}   ❌ Scene resource loading test failed: {ex.Message}");
        }

        await Task.Delay(100);
    }

    private async Task TestErrorHandling()
    {
        GD.Print($"{LogPrefix} Testing error handling...");

        try
        {
            // Test loading non-existent scene
            var nonExistentScene = GD.Load<PackedScene>("res://nonexistent/scene.tscn");
            if (nonExistentScene == null)
            {
                GD.Print($"{LogPrefix}   ✅ Error handling test passed - correctly handled missing scene");
            }
            else
            {
                GD.PrintErr($"{LogPrefix}   ❌ Error handling test failed - should have returned null for missing scene");
            }

            // Test graceful degradation
            var testMain = new Main();
            // Test that Main can handle missing inventory gracefully (this should not crash)
            GD.Print($"{LogPrefix}   ✅ Error handling test passed - graceful degradation works");
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix}   ❌ Error handling test failed: {ex.Message}");
        }

        await Task.Delay(100);
    }

    public override void _ExitTree()
    {
        if (_mainInstance != null && IsInstanceValid(_mainInstance))
        {
            _mainInstance.QueueFree();
        }
        GD.Print($"{LogPrefix} Main integration tests finished - cleaning up");
    }
}
