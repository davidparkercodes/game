using System.Threading;
using System.Threading.Tasks;
using Game.Application.Shared.Cqrs;
using Game.Application.Game.Queries;
using Game.Infrastructure.Game.Services;
using Game.Infrastructure.Rounds.Services;

namespace Game.Application.Game.Handlers;

public class GetGameStateQueryHandler : IQueryHandler<GetGameStateQuery, GameStateResponse>
{
    public Task<GameStateResponse> HandleAsync(GetGameStateQuery query, CancellationToken cancellationToken = default)
    {
        var gameService = GameService.Instance;
        var roundService = RoundService.Instance;

        var money = gameService?.Money ?? 0;
        var lives = gameService?.Lives ?? 0;
        var isGameOver = gameService?.IsGameOver() ?? false;
        var isGameWon = gameService?.IsGameWon() ?? false;

        var currentRound = roundService?.CurrentRound ?? 1;
        var currentPhase = roundService?.CurrentPhase.ToString() ?? "Unknown";
        var phaseTimeRemaining = roundService?.PhaseTimeRemaining ?? 0;
        var isRoundActive = roundService?.IsRoundActive() ?? false;
        var enemiesRemaining = roundService?.EnemiesRemaining ?? 0;

        var response = new GameStateResponse(
            money,
            lives,
            gameService?.Score ?? 0,
            gameService?.IsGameActive ?? false,
            currentRound,
            currentPhase,
            enemiesRemaining
        );

        return Task.FromResult(response);
    }
}
