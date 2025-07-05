using System;

namespace Game.Infrastructure.DI;

public class ServiceLocatorAdapter : IServiceProvider
{
    private readonly ServiceLocator _serviceLocator;

    public ServiceLocatorAdapter(ServiceLocator serviceLocator)
    {
        _serviceLocator = serviceLocator ?? throw new ArgumentNullException(nameof(serviceLocator));
    }

    public object GetService(Type serviceType)
    {
        try
        {
            return _serviceLocator.Resolve(serviceType);
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }
}
