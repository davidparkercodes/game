using System.Threading;
using System.Threading.Tasks;
using Game.Application.Shared.Cqrs;
using Game.Application.Buildings.Queries;
using Game.Domain.Buildings.Services;

namespace Game.Application.Buildings.Handlers;

public class GetTowerStatsQueryHandler : IQueryHandler<GetTowerStatsQuery, TowerStatsResponse>
{
    private readonly IBuildingStatsProvider _buildingStatsProvider;
    private readonly IBuildingTypeRegistry _buildingTypeRegistry;

    public GetTowerStatsQueryHandler(IBuildingStatsProvider buildingStatsProvider, IBuildingTypeRegistry buildingTypeRegistry)
    {
        _buildingStatsProvider = buildingStatsProvider ?? throw new System.ArgumentNullException(nameof(buildingStatsProvider));
        _buildingTypeRegistry = buildingTypeRegistry ?? throw new System.ArgumentNullException(nameof(buildingTypeRegistry));
    }

    public Task<TowerStatsResponse> HandleAsync(GetTowerStatsQuery query, CancellationToken cancellationToken = default)
    {
        if (query == null)
            throw new System.ArgumentNullException(nameof(query));

        if (string.IsNullOrEmpty(query.TowerType))
            return Task.FromResult(TowerStatsResponse.NotFound(""));

        // Use BuildingTypeRegistry for config-driven validation instead of hardcoded checks
        if (!_buildingTypeRegistry.IsValidConfigKey(query.TowerType))
            return Task.FromResult(TowerStatsResponse.NotFound(query.TowerType));

        var buildingStats = _buildingStatsProvider.GetBuildingStats(query.TowerType);

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
