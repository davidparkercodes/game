using System;
using Xunit;
using FluentAssertions;
using Game.Infrastructure.DI;

namespace Game.Tests.Infrastructure.DI;

public class ServiceLocatorTests : IDisposable
{
    private readonly ServiceLocator _serviceLocator;

    public ServiceLocatorTests()
    {
        _serviceLocator = new ServiceLocator();
    }

    public void Dispose()
    {
        _serviceLocator.Clear();
    }

    [Fact]
    public void RegisterSingleton_WithValidImplementation_ShouldRegisterService()
    {
        // Arrange
        var testService = new TestService();

        // Act
        _serviceLocator.RegisterSingleton<ITestService>(testService);

        // Assert
        _serviceLocator.IsRegistered<ITestService>().Should().BeTrue();
        var resolved = _serviceLocator.Resolve<ITestService>();
        resolved.Should().BeSameAs(testService);
    }

    [Fact]
    public void RegisterFactory_WithValidFactory_ShouldRegisterAndCreateService()
    {
        // Act
        _serviceLocator.RegisterFactory<ITestService>(() => new TestService());

        // Assert
        _serviceLocator.IsRegistered<ITestService>().Should().BeTrue();
        var resolved1 = _serviceLocator.Resolve<ITestService>();
        var resolved2 = _serviceLocator.Resolve<ITestService>();
        
        resolved1.Should().NotBeNull();
        resolved1.Should().BeSameAs(resolved2); // Should be singleton after first resolve
    }

    [Fact]
    public void Resolve_WithUnregisteredService_ShouldThrowException()
    {
        // Act & Assert
        var action = () => _serviceLocator.Resolve<ITestService>();
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Service of type ITestService is not registered");
    }

    [Fact]
    public void RegisterSingleton_WithNullImplementation_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => _serviceLocator.RegisterSingleton<ITestService>(null);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void RegisterFactory_WithNullFactory_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => _serviceLocator.RegisterFactory<ITestService>(null);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Clear_ShouldRemoveAllRegistrations()
    {
        // Arrange
        _serviceLocator.RegisterSingleton<ITestService>(new TestService());
        _serviceLocator.RegisterFactory<IAnotherTestService>(() => new AnotherTestService());

        // Act
        _serviceLocator.Clear();

        // Assert
        _serviceLocator.IsRegistered<ITestService>().Should().BeFalse();
        _serviceLocator.IsRegistered<IAnotherTestService>().Should().BeFalse();
    }

    [Fact]
    public void IsRegistered_WithRegisteredService_ShouldReturnTrue()
    {
        // Arrange
        _serviceLocator.RegisterSingleton<ITestService>(new TestService());

        // Act & Assert
        _serviceLocator.IsRegistered<ITestService>().Should().BeTrue();
        _serviceLocator.IsRegistered<IAnotherTestService>().Should().BeFalse();
    }
}

public interface ITestService
{
    string GetValue();
}

public interface IAnotherTestService
{
    int GetNumber();
}

public class TestService : ITestService
{
    public string GetValue() => "Test";
}

public class AnotherTestService : IAnotherTestService
{
    public int GetNumber() => 42;
}
