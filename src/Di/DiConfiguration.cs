using System;
using Game.Domain.Audio.Services;
using Game.Domain.Buildings.Services;
using Game.Domain.Enemies.Services;
using Game.Infrastructure.Stats;
using Game.Infrastructure.Sound;
using Game.Infrastructure.Buildings.Services;
using Game.Infrastructure.Stats.Services;
using Game.Infrastructure.Audio.Services;
using Game.Infrastructure.Waves.Services;
using Game.Infrastructure.Enemies.Services;
using Game.Application.Shared.Cqrs;
using Game.Application.Buildings.Commands;
using Game.Application.Buildings.Handlers;
using Game.Application.Buildings.Queries;
using Game.Application.Game.Commands;
using Game.Application.Game.Handlers;
using Game.Application.Game.Queries;
using Game.Application.Rounds.Commands;
using Game.Application.Rounds.Handlers;
using Game.Application.Waves.Commands;
using Game.Application.Waves.Handlers;
using Game.Application.Buildings.Services;
using Game.Application.Enemies.Services;
using Game.Application.Shared.Services;
using Game.Domain.Shared.Services;
using Game.Application.Game.Services;
using Game.Infrastructure.Game;
using Game.Domain.Common.Services;
using Game.Application.UI.Services;
using Game.Infrastructure.UI.Services;
using Game.Infrastructure.Buildings;

namespace Game.Di;

public static class DiConfiguration
{
    public static void RegisterServices(DiContainer diContainer)
    {
        if (diContainer == null)
            throw new System.ArgumentNullException(nameof(diContainer));

        diContainer.RegisterFactory<IBuildingStatsProvider>(() => new StatsService());
        diContainer.RegisterFactory<IEnemyStatsProvider>(() => new StatsService());
        diContainer.RegisterFactory<ISoundService>(() => new SoundService());
        diContainer.RegisterFactory<IWaveConfigurationService>(() => new WaveConfigurationService());
        diContainer.RegisterFactory<IWaveService>(() => new WaveSpawnerService(
            diContainer.Resolve<IWaveConfigurationService>()));
        diContainer.RegisterFactory<IBuildingZoneService>(() => new BuildingZoneService());
        
        // Register Type Registries (need special handling since they depend on stats providers)
        diContainer.RegisterFactory<IBuildingTypeRegistry>(() => 
        {
            var tempStatsProvider = new Game.Application.Simulation.Services.MockBuildingStatsProvider();
            return new BuildingTypeRegistry(tempStatsProvider);
        });
        
        diContainer.RegisterFactory<IEnemyTypeRegistry>(() => 
        {
            return new EnemyTypeRegistry();
        });
        
        // Register TypeManagementService (unified interface)
        diContainer.RegisterFactory<ITypeManagementService>(() => 
            new TypeManagementService(
                diContainer.Resolve<IBuildingTypeRegistry>(),
                diContainer.Resolve<IEnemyTypeRegistry>()));
        
        // Register StartupValidationService
        diContainer.RegisterFactory<StartupValidationService>(() => 
            new StartupValidationService(diContainer.Resolve<ITypeManagementService>()));
        
        diContainer.RegisterFactory<DebugCommands>(() => 
            new DebugCommands(diContainer.Resolve<ITypeManagementService>()));
        
        diContainer.RegisterFactory<IServiceProvider>(() => new DiAdapter(diContainer));
        diContainer.RegisterFactory<IMediator>(() => new Game.Application.Shared.Cqrs.Mediator(diContainer.Resolve<IServiceProvider>()));
        
        diContainer.RegisterFactory<ICommandHandler<PlaceBuildingCommand, PlaceBuildingResult>>(() => 
            new PlaceBuildingCommandHandler(
                diContainer.Resolve<IBuildingStatsProvider>(),
                diContainer.Resolve<IBuildingZoneService>(),
                diContainer.Resolve<IBuildingTypeRegistry>()));
        
        diContainer.RegisterFactory<ICommandHandler<SpendMoneyCommand, SpendMoneyResult>>(() => 
            new SpendMoneyCommandHandler());
        
        diContainer.RegisterFactory<ICommandHandler<StartRoundCommand, StartRoundResult>>(() => 
            new StartRoundCommandHandler());
        
        diContainer.RegisterFactory<ICommandHandler<StartWaveCommand, StartWaveResult>>(() => 
            new StartWaveCommandHandler(diContainer.Resolve<IWaveService>()));
        
        diContainer.RegisterFactory<IQueryHandler<GetTowerStatsQuery, TowerStatsResponse>>(() => 
            new GetTowerStatsQueryHandler(
                diContainer.Resolve<IBuildingStatsProvider>(),
                diContainer.Resolve<IBuildingTypeRegistry>()));
        
        diContainer.RegisterFactory<IQueryHandler<GetGameStateQuery, GameStateResponse>>(() => 
            new GetGameStateQueryHandler());
        
        // Register TimeManagementConfigService
        diContainer.RegisterFactory<ITimeManagementConfigService>(() => 
            new TimeManagementConfigService());
        
        // Register ITimeManager with factory using logger and config service
        diContainer.RegisterFactory<ITimeManager>(() => 
        {
            var logger = new ConsoleLogger("⚡ [TIMEMANAGER]");
            var timeConfigService = diContainer.Resolve<ITimeManagementConfigService>();
            return new Game.Infrastructure.Game.GodotTimeManager(logger, timeConfigService);
        });
        
        // Register BuildingDefaultsConfigService
        diContainer.RegisterFactory<IBuildingDefaultsConfigService>(() => 
            new BuildingDefaultsConfigService());
        
        // Register BuildingSelectionHudConfigService
        diContainer.RegisterFactory<IBuildingSelectionHudConfigService>(() => 
            new BuildingSelectionHudConfigService());
        
        // Register HudLayoutConfigService
        diContainer.RegisterFactory<IHudLayoutConfigService>(() => 
            new HudLayoutConfigService());
    }

    public static void RegisterSingletonsFromGodot(DiContainer diContainer)
    {
        if (diContainer == null)
            throw new System.ArgumentNullException(nameof(diContainer));

        var statsManagerService = StatsManagerService.Instance;
        if (statsManagerService != null)
        {
            var statsAdapter = new StatsServiceAdapter(statsManagerService);
            diContainer.RegisterSingleton<IBuildingStatsProvider>(statsAdapter);
            diContainer.RegisterSingleton<IEnemyStatsProvider>(statsAdapter);
        }

        var soundManagerService = SoundManagerService.Instance;
        if (soundManagerService != null)
        {
            diContainer.RegisterSingleton<ISoundService>(new SoundServiceAdapter(soundManagerService));
        }
    }
}
