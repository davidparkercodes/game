using System.Threading;
using System.Threading.Tasks;
using Game.Application.Shared.Cqrs;
using Game.Application.Buildings.Queries;
using Game.Domain.Buildings.Services;

namespace Game.Application.Buildings.Handlers;

public class GetTowerStatsQueryHandler : IQueryHandler<GetTowerStatsQuery, TowerStatsResponse>
{
    private readonly IBuildingStatsProvider _buildingStatsProvider;

    public GetTowerStatsQueryHandler(IBuildingStatsProvider buildingStatsProvider)
    {
        _buildingStatsProvider = buildingStatsProvider ?? throw new System.ArgumentNullException(nameof(buildingStatsProvider));
    }

    public Task<TowerStatsResponse> HandleAsync(GetTowerStatsQuery query, CancellationToken cancellationToken = default)
    {
        if (query == null)
            throw new System.ArgumentNullException(nameof(query));

        if (string.IsNullOrEmpty(query.TowerType))
            return Task.FromResult(TowerStatsResponse.NotFound(""));

        var buildingStats = _buildingStatsProvider.GetBuildingStats(query.TowerType);
        if (buildingStats.Cost == 0 && query.TowerType != "basic_tower")
            return Task.FromResult(TowerStatsResponse.NotFound(query.TowerType));

        var response = new TowerStatsResponse(
            query.TowerType,
            buildingStats.Cost,
            buildingStats.Damage,
            buildingStats.Range,
            buildingStats.FireRate,
            buildingStats.BulletSpeed,
            buildingStats.Description,
            true
        );

        return Task.FromResult(response);
    }
}
