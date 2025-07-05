using System;
using Game.Infrastructure.Interfaces;
using Game.Infrastructure.Stats;
using Game.Infrastructure.Sound;
using Game.Infrastructure.Waves;
using Game.Application.Shared.Cqrs;
using Game.Application.Buildings.Commands;
using Game.Application.Buildings.Handlers;
using Game.Application.Game.Commands;
using Game.Application.Game.Handlers;
using Game.Application.Rounds.Commands;
using Game.Application.Rounds.Handlers;
using Game.Application.Waves.Commands;
using Game.Application.Waves.Handlers;
using Game.Application.Queries;
using Game.Application.Queries.Handlers;
using Game.Infrastructure.Buildings;

namespace Game.Infrastructure.DI;

public static class ServiceConfiguration
{
    public static void RegisterServices(ServiceLocator serviceLocator)
    {
        if (serviceLocator == null)
            throw new System.ArgumentNullException(nameof(serviceLocator));

        // Infrastructure services
        serviceLocator.RegisterFactory<IStatsService>(() => new StatsService());
        serviceLocator.RegisterFactory<ISoundService>(() => new SoundService());
        serviceLocator.RegisterFactory<IWaveConfigService>(() => new WaveConfigService());
        serviceLocator.RegisterFactory<IBuildingZoneService>(() => new BuildingZoneService());
        
        // Mediator
        serviceLocator.RegisterFactory<IServiceProvider>(() => new ServiceLocatorAdapter(serviceLocator));
        serviceLocator.RegisterFactory<IMediator>(() => new Game.Application.Shared.Cqrs.Mediator(serviceLocator.Resolve<IServiceProvider>()));
        
        // Command handlers
        serviceLocator.RegisterFactory<ICommandHandler<PlaceBuildingCommand, PlaceBuildingResult>>(() => 
            new PlaceBuildingCommandHandler(
                serviceLocator.Resolve<IStatsService>(),
                serviceLocator.Resolve<IBuildingZoneService>()));
        
        serviceLocator.RegisterFactory<ICommandHandler<SpendMoneyCommand, SpendMoneyResult>>(() => 
            new SpendMoneyCommandHandler());
        
        serviceLocator.RegisterFactory<ICommandHandler<StartRoundCommand, StartRoundResult>>(() => 
            new StartRoundCommandHandler());
        
        serviceLocator.RegisterFactory<ICommandHandler<StartWaveCommand, StartWaveResult>>(() => 
            new StartWaveCommandHandler());
        
        // Query handlers
        serviceLocator.RegisterFactory<IQueryHandler<GetTurretStatsQuery, TurretStatsResponse>>(() => 
            new GetTurretStatsQueryHandler(serviceLocator.Resolve<IStatsService>()));
        
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
            serviceLocator.RegisterSingleton<IStatsService>(new StatsServiceAdapter(statsManager));
        }

        var soundManager = SoundManager.Instance;
        if (soundManager != null)
        {
            serviceLocator.RegisterSingleton<ISoundService>(new SoundServiceAdapter(soundManager));
        }
    }
}
