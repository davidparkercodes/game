using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using Game.Application.Shared.Cqrs;

namespace Game.Tests.Application.Shared;

public class MediatorTests
{
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mediator _mediator;

    public MediatorTests()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _mediator = new Mediator(_serviceProviderMock.Object);
    }

    [Fact]
    public async Task SendAsync_WithCommand_ShouldCallCorrectHandler()
    {
        var command = new TestCommand { Value = "test" };
        var handlerMock = new Mock<ICommandHandler<TestCommand>>();
        
        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(ICommandHandler<TestCommand>)))
            .Returns(handlerMock.Object);

        await _mediator.SendAsync(command);

        handlerMock.Verify(h => h.HandleAsync(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendAsync_WithCommandReturningResult_ShouldReturnCorrectResult()
    {
        var command = new TestCommandWithResult { Input = "test input" };
        var expectedResult = "processed: test input";
        var handlerMock = new Mock<ICommandHandler<TestCommandWithResult, string>>();
        
        handlerMock
            .Setup(h => h.HandleAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(ICommandHandler<TestCommandWithResult, string>)))
            .Returns(handlerMock.Object);

        var result = await _mediator.SendAsync<string>(command);

        result.Should().Be(expectedResult);
        handlerMock.Verify(h => h.HandleAsync(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task QueryAsync_ShouldCallCorrectQueryHandler()
    {
        var query = new TestQuery { SearchTerm = "test search" };
        var expectedResult = "query result";
        var handlerMock = new Mock<IQueryHandler<TestQuery, string>>();
        
        handlerMock
            .Setup(h => h.HandleAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IQueryHandler<TestQuery, string>)))
            .Returns(handlerMock.Object);

        var result = await _mediator.QueryAsync(query);

        result.Should().Be(expectedResult);
        handlerMock.Verify(h => h.HandleAsync(query, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendAsync_WithUnregisteredCommandHandler_ShouldThrowException()
    {
        var command = new TestCommand { Value = "test" };
        
        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(ICommandHandler<TestCommand>)))
            .Returns(null);

        var action = async () => await _mediator.SendAsync(command);

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Handler of type ICommandHandler`1 is not registered.");
    }

    [Fact]
    public async Task QueryAsync_WithUnregisteredQueryHandler_ShouldThrowException()
    {
        var query = new TestQuery { SearchTerm = "test" };
        
        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IQueryHandler<TestQuery, string>)))
            .Returns(null);

        var action = async () => await _mediator.QueryAsync<string>(query);

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Handler of type IQueryHandler`2 is not registered.");
    }
}

public class TestCommand : ICommand
{
    public string Value { get; set; }
}

public class TestCommandWithResult : ICommand<string>
{
    public string Input { get; set; }
}

public class TestQuery : IQuery<string>
{
    public string SearchTerm { get; set; }
}
