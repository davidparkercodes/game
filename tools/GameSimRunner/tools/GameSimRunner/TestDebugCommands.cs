using System;
using Game.Application.Shared.Services;
using Game.Infrastructure.DI;

namespace GameSimRunner;

public static class TestDebugCommands
{
    public static void RunDebugTests()
    {
        Console.WriteLine("=== Debug Commands Test ===");
        
        var serviceLocator = new ServiceLocator();
        SimpleServiceConfiguration.RegisterServices(serviceLocator);
        
        var debugCommands = serviceLocator.Resolve<DebugCommands>();
        
        if (debugCommands != null)
        {
            Console.WriteLine("Testing debug commands...\n");
            
            // Test building types listing
            debugCommands.ListAllBuildingTypes();
            Console.WriteLine();
            
            // Test enemy types listing  
            debugCommands.ListAllEnemyTypes();
            Console.WriteLine();
            
            // Test wave progression
            debugCommands.ShowWaveProgression(5);
            Console.WriteLine();
            
            // Test validation
            debugCommands.ValidateConfigConsistency();
        }
        else
        {
            Console.WriteLine("❌ Failed to resolve DebugCommands from service locator");
        }
        
        Console.WriteLine("=== Debug Commands Test Complete ===");
    }
}
