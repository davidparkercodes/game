using Game.Domain.Buildings.Services;
using Game.Domain.Enemies.Services;
using Game.Domain.Shared.Services;
using Game.Application.Buildings.Services;
using Game.Application.Enemies.Services;
using Game.Application.Shared.Services;
using Game.Application.Simulation.Services;
using Game.Di;

namespace GameSimRunner;

public static class SimpleServiceConfiguration
{
    public static void RegisterServices(DiContainer diContainer)
    {
        if (diContainer == null)
            throw new System.ArgumentNullException(nameof(diContainer));

        // Register Mock Stats Providers
        diContainer.RegisterFactory<IBuildingStatsProvider>(() => new MockBuildingStatsProvider());
        diContainer.RegisterFactory<IEnemyStatsProvider>(() => new MockEnemyStatsProvider());
        
        // Register Type Registries
        diContainer.RegisterFactory<IBuildingTypeRegistry>(() => 
        {
            var tempStatsProvider = new MockBuildingStatsProvider();
            return new BuildingTypeRegistry(tempStatsProvider);
        });
        
        diContainer.RegisterFactory<IEnemyTypeRegistry>(() => 
        {
            return new EnemyTypeRegistry();
        });
        
        // Register Placement Strategy Provider
        diContainer.RegisterFactory<IPlacementStrategyProvider>(() => 
            new PlacementStrategyProvider(diContainer.Resolve<IBuildingTypeRegistry>()));
        
        // Register TypeManagementService (unified interface)
        diContainer.RegisterFactory<ITypeManagementService>(() => 
            new TypeManagementService(
                diContainer.Resolve<IBuildingTypeRegistry>(),
                diContainer.Resolve<IEnemyTypeRegistry>()));
        
        // Register StartupValidationService
        diContainer.RegisterFactory<StartupValidationService>(() => 
            new StartupValidationService(diContainer.Resolve<ITypeManagementService>()));
        
        // Register DebugCommands
        diContainer.RegisterFactory<DebugCommands>(() => 
            new DebugCommands(diContainer.Resolve<ITypeManagementService>()));
    }
}
