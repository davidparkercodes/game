using System.Threading;
using System.Threading.Tasks;
using Game.Application.Shared.Cqrs;
using Game.Application.Rounds.Commands;

namespace Game.Application.Rounds.Handlers;

public class StartRoundCommandHandler : ICommandHandler<StartRoundCommand, StartRoundResult>
{
    public Task<StartRoundResult> HandleAsync(StartRoundCommand command, CancellationToken cancellationToken = default)
    {
        if (command == null)
            return Task.FromResult(StartRoundResult.Failed("Command cannot be null"));

        var roundManager = RoundManager.Instance;
        if (roundManager == null)
            return Task.FromResult(StartRoundResult.Failed("RoundManager is not available"));

        if (command.ForceStart && roundManager.CurrentPhase == RoundPhase.Build)
        {
            roundManager.ForceStartDefendPhase();
            return Task.FromResult(StartRoundResult.Successful(roundManager.CurrentRound, "Defend"));
        }

        if (command.RoundNumber > 0 && command.RoundNumber != roundManager.CurrentRound)
        {
            return Task.FromResult(StartRoundResult.Failed($"Cannot start round {command.RoundNumber}. Current round is {roundManager.CurrentRound}"));
        }

        if (roundManager.IsRoundActive)
        {
            return Task.FromResult(StartRoundResult.Failed($"Round {roundManager.CurrentRound} is already active"));
        }

        roundManager.StartRound();
        return Task.FromResult(StartRoundResult.Successful(roundManager.CurrentRound, roundManager.CurrentPhase.ToString()));
    }
}
