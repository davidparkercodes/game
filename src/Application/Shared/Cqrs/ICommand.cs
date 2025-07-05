namespace Game.Application.Shared.Cqrs;

public interface ICommand;

public interface ICommand<out TResponse> : ICommand;
