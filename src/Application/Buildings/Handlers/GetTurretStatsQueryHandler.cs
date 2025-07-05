using System.Threading;
using System.Threading.Tasks;
using Game.Application.Shared.Cqrs;
using Game.Application.Buildings.Queries;
using Game.Domain.Buildings.Services;

namespace Game.Application.Buildings.Handlers;

public class GetTurretStatsQueryHandler : IQueryHandler<GetTurretStatsQuery, TurretStatsResponse>
{
    private readonly IBuildingStatsProvider _buildingStatsProvider;

    public GetTurretStatsQueryHandler(IBuildingStatsProvider buildingStatsProvider)
    {
        _buildingStatsProvider = buildingStatsProvider ?? throw new System.ArgumentNullException(nameof(buildingStatsProvider));
    }

    public Task<TurretStatsResponse> HandleAsync(GetTurretStatsQuery query, CancellationToken cancellationToken = default)
    {
        if (query == null)
            throw new System.ArgumentNullException(nameof(query));

        if (string.IsNullOrEmpty(query.TurretType))
            return Task.FromResult(TurretStatsResponse.NotFound(""));

        var buildingStats = _buildingStatsProvider.GetBuildingStats(query.TurretType);
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
