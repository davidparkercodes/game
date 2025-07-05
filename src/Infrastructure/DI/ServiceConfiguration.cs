using Game.Infrastructure.Interfaces;
using Game.Infrastructure.Stats;
using Game.Infrastructure.Sound;
using Game.Infrastructure.Waves;

namespace Game.Infrastructure.DI;

public static class ServiceConfiguration
{
    public static void RegisterServices(ServiceLocator serviceLocator)
    {
        if (serviceLocator == null)
            throw new System.ArgumentNullException(nameof(serviceLocator));

        serviceLocator.RegisterFactory<IStatsService>(() => new StatsService());
        serviceLocator.RegisterFactory<ISoundService>(() => new SoundService());
        serviceLocator.RegisterFactory<IWaveConfigService>(() => new WaveConfigService());
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
