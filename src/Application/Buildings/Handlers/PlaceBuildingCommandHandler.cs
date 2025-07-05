using System.Threading.Tasks;
using Game.Application.Shared.Cqrs;
using Game.Application.Buildings.Commands;
using Game.Domain.Buildings.Services;
using Game.Domain.Buildings.ValueObjects;
using Game.Domain.Shared.ValueObjects;

namespace Game.Application.Buildings.Handlers;

public class PlaceBuildingCommandHandler : ICommandHandler<PlaceBuildingCommand, PlaceBuildingResult>
{
    private readonly IBuildingStatsProvider _buildingStatsProvider;
    private readonly IBuildingZoneService _zoneService;
    private readonly IBuildingTypeRegistry _buildingTypeRegistry;
    private static int _nextBuildingId = 1;

    public PlaceBuildingCommandHandler(IBuildingStatsProvider buildingStatsProvider, IBuildingZoneService zoneService, IBuildingTypeRegistry buildingTypeRegistry)
    {
        _buildingStatsProvider = buildingStatsProvider ?? throw new System.ArgumentNullException(nameof(buildingStatsProvider));
        _zoneService = zoneService ?? throw new System.ArgumentNullException(nameof(zoneService));
        _buildingTypeRegistry = buildingTypeRegistry ?? throw new System.ArgumentNullException(nameof(buildingTypeRegistry));
    }

    public Task<PlaceBuildingResult> HandleAsync(PlaceBuildingCommand command, System.Threading.CancellationToken cancellationToken = default)
    {
        if (command == null)
            return Task.FromResult(PlaceBuildingResult.Failed("Command cannot be null"));

        if (string.IsNullOrEmpty(command.BuildingType))
            return Task.FromResult(PlaceBuildingResult.Failed("Building type cannot be empty"));

        // Use BuildingTypeRegistry for validation
        if (!_buildingTypeRegistry.IsValidConfigKey(command.BuildingType))
            return Task.FromResult(PlaceBuildingResult.Failed($"Unknown building type: {command.BuildingType}"));

        var buildingStats = _buildingStatsProvider.GetBuildingStats(command.BuildingType);

        if (!_zoneService.CanBuildAt(command.Position))
            return Task.FromResult(PlaceBuildingResult.Failed("Cannot build at this location - invalid zone"));

        if (!HasEnoughMoney(buildingStats.Cost))
            return Task.FromResult(PlaceBuildingResult.Failed($"Not enough money. Cost: ${buildingStats.Cost}"));

        if (IsPositionOccupied(command.Position))
            return Task.FromResult(PlaceBuildingResult.Failed("Position is already occupied by another building"));

        var buildingId = CreateBuilding(command.BuildingType, command.Position, buildingStats);
        SpendMoney(buildingStats.Cost);

        return Task.FromResult(PlaceBuildingResult.Successful(buildingId, buildingStats.Cost));
    }

    private bool HasEnoughMoney(int cost)
    {
        // TODO: Inject IGameStateService to check money
        // For now, return true for simulation purposes
        return true;
    }

    private void SpendMoney(int cost)
    {
        // TODO: Inject IGameStateService to spend money
        // For now, no-op for simulation purposes
    }

    private bool IsPositionOccupied(Position position)
    {
        // TODO: Inject IBuildingCollisionService to check occupancy
        // For now, return false for simulation purposes
        return false;
    }

    private int CreateBuilding(string buildingType, Position position, BuildingStats stats)
    {
        // TODO: Inject IBuildingCreationService to handle building creation
        // For now, just return a unique ID for simulation purposes
        return _nextBuildingId++;
    }
}
