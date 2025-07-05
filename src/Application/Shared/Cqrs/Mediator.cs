using System;
using System.Threading;
using System.Threading.Tasks;

namespace Game.Application.Shared.Cqrs;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) 
        where TCommand : ICommand
    {
        var handler = GetHandler<ICommandHandler<TCommand>>();
        await handler.HandleAsync(command, cancellationToken);
    }

    public async Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse));
        dynamic handler = GetHandler(handlerType);
        return await handler.HandleAsync((dynamic)command, cancellationToken);
    }

    public async Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
        dynamic handler = GetHandler(handlerType);
        return await handler.HandleAsync((dynamic)query, cancellationToken);
    }

    private T GetHandler<T>()
    {
        var handler = _serviceProvider.GetService(typeof(T));
        if (handler == null)
            throw new InvalidOperationException($"Handler of type {typeof(T).Name} is not registered.");
        return (T)handler;
    }

    private object GetHandler(Type handlerType)
    {
        var handler = _serviceProvider.GetService(handlerType);
        if (handler == null)
            throw new InvalidOperationException($"Handler of type {handlerType.Name} is not registered.");
        return handler;
    }
}
