using System.Threading;
using System.Threading.Tasks;
using Game.Application.Shared.Cqrs;
using Game.Application.Game.Commands;
using Game.Infrastructure.Game.Services;

namespace Game.Application.Game.Handlers;

public class SpendMoneyCommandHandler : ICommandHandler<SpendMoneyCommand, SpendMoneyResult>
{
    public Task<SpendMoneyResult> HandleAsync(SpendMoneyCommand command, CancellationToken cancellationToken = default)
    {
        if (command == null)
            return Task.FromResult(SpendMoneyResult.Failed("Command cannot be null", 0));

        if (command.Amount < 0)
            return Task.FromResult(SpendMoneyResult.Failed("Amount cannot be negative", GetCurrentMoney()));

        var currentMoney = GetCurrentMoney();
        if (currentMoney < command.Amount)
            return Task.FromResult(SpendMoneyResult.Failed($"Insufficient funds. Need: {command.Amount}, Have: {currentMoney}", currentMoney));

        var success = GameService.Instance?.SpendMoney(command.Amount) ?? false;
        if (!success)
            return Task.FromResult(SpendMoneyResult.Failed("Failed to spend money", currentMoney));

        var remainingMoney = GetCurrentMoney();
        return Task.FromResult(SpendMoneyResult.Successful(remainingMoney));
    }

    private int GetCurrentMoney()
    {
        return GameService.Instance?.Money ?? 0;
    }
}
