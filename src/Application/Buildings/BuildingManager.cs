using System.Threading.Tasks;
using Game.Application.Shared.Cqrs;
using Game.Application.Buildings.Commands;
using Game.Domain.Shared.ValueObjects;
using Game.Infrastructure.DI;

namespace Game.Application.Buildings;

public class BuildingManager
{
    private readonly IMediator _mediator;

    public BuildingManager(IMediator mediator)
    {
        _mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
    }

    public static BuildingManager Instance { get; private set; }

    public static void Initialize()
    {
        var mediator = ServiceLocator.Instance.Resolve<IMediator>();
        Instance = new BuildingManager(mediator);
    }

    public async Task<PlaceBuildingResult> PlaceBuildingAsync(string buildingType, Position position, int playerId = 0)
    {
        var command = new PlaceBuildingCommand(buildingType, position, playerId);
        return await _mediator.SendAsync<PlaceBuildingResult>(command);
    }

    public async Task<PlaceBuildingResult> TryPlaceBuilding(string buildingType, Position position)
    {
        try
        {
            return await PlaceBuildingAsync(buildingType, position);
        }
        catch (System.Exception ex)
        {
            return PlaceBuildingResult.Failed($"Error placing building: {ex.Message}");
        }
    }
}
