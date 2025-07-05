using System.Threading.Tasks;
using Godot;
using Game.Application.Shared.Cqrs;
using Game.Application.Buildings.Commands;
using Game.Infrastructure.Interfaces;
using Game.Domain.Buildings.ValueObjects;

namespace Game.Application.Buildings.Handlers;

public class PlaceBuildingCommandHandler : ICommandHandler<PlaceBuildingCommand, PlaceBuildingResult>
{
    private readonly IStatsService _statsService;
    private readonly IBuildingZoneService _zoneService;
    private static int _nextBuildingId = 1;

    public PlaceBuildingCommandHandler(IStatsService statsService, IBuildingZoneService zoneService)
    {
        _statsService = statsService ?? throw new System.ArgumentNullException(nameof(statsService));
        _zoneService = zoneService ?? throw new System.ArgumentNullException(nameof(zoneService));
    }

    public Task<PlaceBuildingResult> HandleAsync(PlaceBuildingCommand command, System.Threading.CancellationToken cancellationToken = default)
    {
        if (command == null)
            return Task.FromResult(PlaceBuildingResult.Failed("Command cannot be null"));

        if (string.IsNullOrEmpty(command.BuildingType))
            return Task.FromResult(PlaceBuildingResult.Failed("Building type cannot be empty"));

        var buildingStats = _statsService.GetBuildingStats(command.BuildingType);
        if (buildingStats == null)
            return Task.FromResult(PlaceBuildingResult.Failed($"Unknown building type: {command.BuildingType}"));

        if (!_zoneService.CanBuildAt(command.Position))
            return Task.FromResult(PlaceBuildingResult.Failed("Cannot build at this location - invalid zone"));

        if (!HasEnoughMoney(buildingStats.cost))
            return Task.FromResult(PlaceBuildingResult.Failed($"Not enough money. Cost: ${buildingStats.cost}"));

        if (IsPositionOccupied(command.Position))
            return Task.FromResult(PlaceBuildingResult.Failed("Position is already occupied by another building"));

        var buildingId = CreateBuilding(command.BuildingType, command.Position, buildingStats);
        SpendMoney(buildingStats.cost);

        return Task.FromResult(PlaceBuildingResult.Successful(buildingId, buildingStats.cost));
    }

    private bool HasEnoughMoney(int cost)
    {
        return GameManager.Instance?.Money >= cost;
    }

    private void SpendMoney(int cost)
    {
        GameManager.Instance?.SpendMoney(cost);
    }

    private bool IsPositionOccupied(Vector2 position)
    {
        // TODO: Implement proper building collision detection
        return false;
    }

    private int CreateBuilding(string buildingType, Vector2 position, BuildingStatsData stats)
    {
        var buildingScene = LoadBuildingScene(buildingType);
        if (buildingScene == null)
            return 0;

        var building = buildingScene.Instantiate<Building>();
        building.GlobalPosition = position;
        
        GetSceneTree()?.Root.AddChild(building);
        // TODO: Register building with building manager

        return _nextBuildingId++;
    }

    private PackedScene LoadBuildingScene(string buildingType)
    {
        var scenePath = $"res://scenes/Buildings/{buildingType}.tscn";
        return GD.Load<PackedScene>(scenePath);
    }

    private BuildingManager GetBuildingManager()
    {
        return BuildingManager.Instance;
    }

    private SceneTree GetSceneTree()
    {
        return Engine.GetMainLoop() as SceneTree;
    }
}
