using System;
using Game.Domain.Audio.Services;
using Game.Domain.Buildings.Services;
using Game.Domain.Enemies.Services;
using Game.Infrastructure.Stats;
using Game.Infrastructure.Sound;
using Game.Infrastructure.Buildings;
using Game.Infrastructure.Managers;
using Game.Application.Shared.Cqrs;
using Game.Application.Buildings.Commands;
using Game.Application.Buildings.Handlers;
using Game.Application.Buildings.Queries;
using Game.Application.Game.Commands;
using Game.Application.Game.Handlers;
using Game.Application.Game.Queries;
using Game.Application.Rounds.Commands;
using Game.Application.Rounds.Handlers;
using Game.Application.Buildings.Services;

namespace Game.Infrastructure.DI;

public static class ServiceConfiguration
{
    public static void RegisterServices(ServiceLocator serviceLocator)
    {
        if (serviceLocator == null)
            throw new System.ArgumentNullException(nameof(serviceLocator));

        serviceLocator.RegisterFactory<IBuildingStatsProvider>(() => new StatsService());
        serviceLocator.RegisterFactory<IEnemyStatsProvider>(() => new StatsService());
        serviceLocator.RegisterFactory<ISoundService>(() => new SoundService());
        // serviceLocator.RegisterFactory<IWaveConfigService>(() => new WaveConfigService()); // Temporarily disabled
        serviceLocator.RegisterFactory<IBuildingZoneService>(() => new BuildingZoneService());
        
        // Register BuildingTypeRegistry (needs special handling since it depends on IBuildingStatsProvider)
        // We'll create a temporary MockBuildingStatsProvider for the registry, then replace the main one
        serviceLocator.RegisterFactory<IBuildingTypeRegistry>(() => 
        {
            var tempStatsProvider = new Game.Application.Simulation.Services.MockBuildingStatsProvider();
            return new BuildingTypeRegistry(tempStatsProvider);
        });
        
        serviceLocator.RegisterFactory<IServiceProvider>(() => new ServiceLocatorAdapter(serviceLocator));
        serviceLocator.RegisterFactory<IMediator>(() => new Game.Application.Shared.Cqrs.Mediator(serviceLocator.Resolve<IServiceProvider>()));
        
        serviceLocator.RegisterFactory<ICommandHandler<PlaceBuildingCommand, PlaceBuildingResult>>(() => 
            new PlaceBuildingCommandHandler(
                serviceLocator.Resolve<IBuildingStatsProvider>(),
                serviceLocator.Resolve<IBuildingZoneService>(),
                serviceLocator.Resolve<IBuildingTypeRegistry>()));
        
        serviceLocator.RegisterFactory<ICommandHandler<SpendMoneyCommand, SpendMoneyResult>>(() => 
            new SpendMoneyCommandHandler());
        
        serviceLocator.RegisterFactory<ICommandHandler<StartRoundCommand, StartRoundResult>>(() => 
            new StartRoundCommandHandler());
        
        // serviceLocator.RegisterFactory<ICommandHandler<StartWaveCommand, StartWaveResult>>(() => 
        //     new StartWaveCommandHandler()); // Temporarily disabled
        
        serviceLocator.RegisterFactory<IQueryHandler<GetTowerStatsQuery, TowerStatsResponse>>(() => 
            new GetTowerStatsQueryHandler(
                serviceLocator.Resolve<IBuildingStatsProvider>(),
                serviceLocator.Resolve<IBuildingTypeRegistry>()));
        
        serviceLocator.RegisterFactory<IQueryHandler<GetGameStateQuery, GameStateResponse>>(() => 
            new GetGameStateQueryHandler());
    }

    public static void RegisterSingletonsFromGodot(ServiceLocator serviceLocator)
    {
        if (serviceLocator == null)
            throw new System.ArgumentNullException(nameof(serviceLocator));

        var statsManager = StatsManager.Instance;
        if (statsManager != null)
        {
            var statsAdapter = new StatsServiceAdapter(statsManager);
            serviceLocator.RegisterSingleton<IBuildingStatsProvider>(statsAdapter);
            serviceLocator.RegisterSingleton<IEnemyStatsProvider>(statsAdapter);
        }

        var soundManager = SoundManager.Instance;
        if (soundManager != null)
        {
            serviceLocator.RegisterSingleton<ISoundService>(new SoundServiceAdapter(soundManager));
        }
    }
}
