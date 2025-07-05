using System.Threading;
using System.Threading.Tasks;
using Game.Application.Shared.Cqrs;
using Game.Application.Rounds.Commands;
using Game.Infrastructure.Rounds.Services;

namespace Game.Application.Rounds.Handlers;

public class StartRoundCommandHandler : ICommandHandler<StartRoundCommand, StartRoundResult>
{
    public Task<StartRoundResult> HandleAsync(StartRoundCommand command, CancellationToken cancellationToken = default)
    {
        if (command == null)
            return Task.FromResult(StartRoundResult.Failed("Command cannot be null"));

        var roundService = RoundService.Instance;
        if (roundService == null)
            return Task.FromResult(StartRoundResult.Failed("RoundService is not available"));

        if (command.ForceStart && roundService.CurrentPhase == RoundPhase.Build)
        {
            roundService.ForceStartDefendPhase();
            return Task.FromResult(StartRoundResult.Successful(roundService.CurrentRound, "Defend"));
        }

        if (command.RoundNumber > 0 && command.RoundNumber != roundService.CurrentRound)
        {
            return Task.FromResult(StartRoundResult.Failed($"Cannot start round {command.RoundNumber}. Current round is {roundService.CurrentRound}"));
        }

        if (roundService.IsRoundActive())
        {
            return Task.FromResult(StartRoundResult.Failed($"Round {roundService.CurrentRound} is already active"));
        }

        roundService.StartRound(command.RoundNumber > 0 ? command.RoundNumber : roundService.CurrentRound);
        return Task.FromResult(StartRoundResult.Successful(roundService.CurrentRound, roundService.CurrentPhase.ToString()));
    }
}
