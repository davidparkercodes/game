using System.Threading;
using System.Threading.Tasks;
using Game.Application.Shared.Cqrs;
using Game.Application.Buildings.Queries;
using Game.Domain.Shared.Services;

namespace Game.Application.Buildings.Handlers;

public class GetTurretStatsQueryHandler : IQueryHandler<GetTurretStatsQuery, TurretStatsResponse>
{
    private readonly IStatsService _statsService;

    public GetTurretStatsQueryHandler(IStatsService statsService)
    {
        _statsService = statsService ?? throw new System.ArgumentNullException(nameof(statsService));
    }

    public Task<TurretStatsResponse> HandleAsync(GetTurretStatsQuery query, CancellationToken cancellationToken = default)
    {
        if (query == null)
            throw new System.ArgumentNullException(nameof(query));

        if (string.IsNullOrEmpty(query.TurretType))
            return Task.FromResult(TurretStatsResponse.NotFound(""));

        var buildingStats = _statsService.GetBuildingStats(query.TurretType);
        if (buildingStats.cost == 0 && query.TurretType != "basic_tower")
            return Task.FromResult(TurretStatsResponse.NotFound(query.TurretType));

        var response = new TurretStatsResponse(
            query.TurretType,
            buildingStats.cost,
            buildingStats.damage,
            buildingStats.range,
            buildingStats.fire_rate,
            buildingStats.bullet_speed,
            buildingStats.description,
            true
        );

        return Task.FromResult(response);
    }
}
