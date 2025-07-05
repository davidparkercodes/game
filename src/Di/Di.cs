namespace Game.Di;

public static class Di
{
    private static DiContainer? _container;

    public static DiContainer Container => _container ??= CreateContainer();

    public static DiContainer Initialize()
    {
        var container = new DiContainer();
        DiConfiguration.RegisterServices(container);
        _container = container;
        return container;
    }

    public static DiContainer InitializeForGodot()
    {
        var container = Initialize();
        DiConfiguration.RegisterSingletonsFromGodot(container);
        return container;
    }

    public static void Reset()
    {
        _container?.Clear();
        _container = null;
    }

    public static T Resolve<T>()
    {
        return Container.Resolve<T>();
    }

    public static bool IsRegistered<T>()
    {
        return Container.IsRegistered<T>();
    }

    private static DiContainer CreateContainer()
    {
        return Initialize();
    }
}
