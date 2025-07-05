using System;
using Xunit;
using FluentAssertions;
using Game.Infrastructure.DI;
using Game.Infrastructure.Interfaces;

namespace Game.Tests.Infrastructure;

public class ServiceConfigurationTests : IDisposable
{
    private readonly ServiceLocator _serviceLocator;

    public ServiceConfigurationTests()
    {
        _serviceLocator = new ServiceLocator();
    }

    public void Dispose()
    {
        _serviceLocator.Clear();
    }

    [Fact]
    public void RegisterServices_ShouldRegisterAllRequiredServices()
    {
        // Act
        ServiceConfiguration.RegisterServices(_serviceLocator);

        // Assert
        _serviceLocator.IsRegistered<IStatsService>().Should().BeTrue();
        _serviceLocator.IsRegistered<ISoundService>().Should().BeTrue();
        _serviceLocator.IsRegistered<IWaveConfigService>().Should().BeTrue();
    }

    [Fact]
    public void ServiceConfiguration_WithNullServiceLocator_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => ServiceConfiguration.RegisterServices(null);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void RegisterSingletonsFromGodot_WithNullServiceLocator_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => ServiceConfiguration.RegisterSingletonsFromGodot(null);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void RegisterSingletonsFromGodot_WithoutGodotInstances_ShouldNotCrash()
    {
        // Act & Assert - Should not throw since no Godot singletons exist
        var action = () => ServiceConfiguration.RegisterSingletonsFromGodot(_serviceLocator);
        action.Should().NotThrow();
    }
}
