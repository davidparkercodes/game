using System;

namespace Game.Di;

public class DiAdapter : IServiceProvider
{
    private readonly DiContainer _diContainer;

    public DiAdapter(DiContainer diContainer)
    {
        _diContainer = diContainer ?? throw new ArgumentNullException(nameof(diContainer));
    }

    public object GetService(Type serviceType)
    {
        try
        {
            return _diContainer.Resolve(serviceType);
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }
}
