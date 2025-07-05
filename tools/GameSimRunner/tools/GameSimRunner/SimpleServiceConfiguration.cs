using Game.Domain.Buildings.Services;
using Game.Domain.Enemies.Services;
using Game.Domain.Shared.Services;
using Game.Application.Buildings.Services;
using Game.Application.Enemies.Services;
using Game.Application.Shared.Services;
using Game.Application.Simulation.Services;
using Game.Infrastructure.DI;

namespace GameSimRunner;

public static class SimpleServiceConfiguration
{
    public static void RegisterServices(ServiceLocator serviceLocator)
    {
        if (serviceLocator == null)
            throw new System.ArgumentNullException(nameof(serviceLocator));

        // Register Mock Stats Providers
        serviceLocator.RegisterFactory<IBuildingStatsProvider>(() => new MockBuildingStatsProvider());
        serviceLocator.RegisterFactory<IEnemyStatsProvider>(() => new MockEnemyStatsProvider());
        
        // Register Type Registries
        serviceLocator.RegisterFactory<IBuildingTypeRegistry>(() => 
        {
            var tempStatsProvider = new MockBuildingStatsProvider();
            return new BuildingTypeRegistry(tempStatsProvider);
        });
        
        serviceLocator.RegisterFactory<IEnemyTypeRegistry>(() => 
        {
            return new EnemyTypeRegistry();
        });
        
        // Register Placement Strategy Provider
        serviceLocator.RegisterFactory<IPlacementStrategyProvider>(() => 
            new PlacementStrategyProvider(serviceLocator.Resolve<IBuildingTypeRegistry>()));
        
        // Register TypeManagementService (unified interface)
        serviceLocator.RegisterFactory<ITypeManagementService>(() => 
            new TypeManagementService(
                serviceLocator.Resolve<IBuildingTypeRegistry>(),
                serviceLocator.Resolve<IEnemyTypeRegistry>()));
        
        // Register StartupValidationService
        serviceLocator.RegisterFactory<StartupValidationService>(() => 
            new StartupValidationService(serviceLocator.Resolve<ITypeManagementService>()));
        
        // Register DebugCommands
        serviceLocator.RegisterFactory<DebugCommands>(() => 
            new DebugCommands(serviceLocator.Resolve<ITypeManagementService>()));
    }
}
