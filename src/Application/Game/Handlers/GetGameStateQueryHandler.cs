using System.Threading;
using System.Threading.Tasks;
using Game.Application.Shared.Cqrs;
using Game.Application.Game.Queries;
using Game.Infrastructure.Managers;

namespace Game.Application.Game.Handlers;

public class GetGameStateQueryHandler : IQueryHandler<GetGameStateQuery, GameStateResponse>
{
    public Task<GameStateResponse> HandleAsync(GetGameStateQuery query, CancellationToken cancellationToken = default)
    {
        var gameManager = GameManager.Instance;
        var roundManager = RoundManager.Instance;

        var money = gameManager?.Money ?? 0;
        var lives = gameManager?.Lives ?? 0;
        var isGameOver = gameManager?.IsGameOver() ?? false;
        var isGameWon = gameManager?.IsGameWon() ?? false;

        var currentRound = roundManager?.CurrentRound ?? 1;
        var currentPhase = roundManager?.CurrentPhase.ToString() ?? "Unknown";
        var phaseTimeRemaining = roundManager?.PhaseTimeRemaining ?? 0;
        var isRoundActive = roundManager?.IsRoundActive() ?? false;
        var enemiesRemaining = roundManager?.EnemiesRemaining ?? 0;

        var response = new GameStateResponse(
            money,
            lives,
            gameManager?.Score ?? 0,
            gameManager?.IsGameActive ?? false,
            currentRound,
            currentPhase,
            enemiesRemaining
        );

        return Task.FromResult(response);
    }
}
