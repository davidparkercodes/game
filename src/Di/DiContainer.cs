using System;
using System.Collections.Generic;

namespace Game.Di;

public class DiContainer
{
    private static DiContainer? _instance;
    private readonly Dictionary<Type, object> _services = new();
    private readonly Dictionary<Type, Func<object>> _serviceFactories = new();

    public static DiContainer Instance => _instance ??= new DiContainer();

    public void RegisterSingleton<TInterface, TImplementation>(TImplementation implementation)
        where TImplementation : class, TInterface
    {
        if (implementation == null)
            throw new ArgumentNullException(nameof(implementation));
            
        _services[typeof(TInterface)] = implementation;
    }

    public void RegisterSingleton<T>(T implementation) where T : class
    {
        if (implementation == null)
            throw new ArgumentNullException(nameof(implementation));
            
        _services[typeof(T)] = implementation;
    }

    public void RegisterFactory<TInterface>(Func<TInterface> factory)
    {
        if (factory == null)
            throw new ArgumentNullException(nameof(factory));
            
        _serviceFactories[typeof(TInterface)] = () => factory()!;
    }

    public T Resolve<T>()
    {
        return (T)Resolve(typeof(T));
    }

    public object Resolve(Type serviceType)
    {
        if (_services.TryGetValue(serviceType, out var service))
        {
            return service;
        }

        if (_serviceFactories.TryGetValue(serviceType, out var factory))
        {
            var instance = factory();
            _services[serviceType] = instance;
            return instance;
        }

        throw new InvalidOperationException($"Service of type {serviceType.Name} is not registered");
    }

    public bool IsRegistered<T>()
    {
        return IsRegistered(typeof(T));
    }

    public bool IsRegistered(Type serviceType)
    {
        return _services.ContainsKey(serviceType) || _serviceFactories.ContainsKey(serviceType);
    }

    public void Clear()
    {
        _services.Clear();
        _serviceFactories.Clear();
    }
}
