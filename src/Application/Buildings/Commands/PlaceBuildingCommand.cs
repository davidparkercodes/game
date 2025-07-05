using Game.Application.Shared.Cqrs;
using Game.Domain.Shared.ValueObjects;

namespace Game.Application.Buildings.Commands;

public class PlaceBuildingCommand : ICommand<PlaceBuildingResult>
{
    public string BuildingType { get; }
    public Position Position { get; }
    public int PlayerId { get; }

    public PlaceBuildingCommand(string buildingType, Position position, int playerId = 0)
    {
        BuildingType = buildingType ?? throw new System.ArgumentNullException(nameof(buildingType));
        Position = position;
        PlayerId = playerId;
    }
}

public class PlaceBuildingResult
{
    public bool Success { get; }
    public string ErrorMessage { get; }
    public int BuildingId { get; }
    public int CostPaid { get; }

    public PlaceBuildingResult(bool success, string errorMessage = null, int buildingId = 0, int costPaid = 0)
    {
        Success = success;
        ErrorMessage = errorMessage;
        BuildingId = buildingId;
        CostPaid = costPaid;
    }

    public static PlaceBuildingResult Successful(int buildingId, int costPaid)
    {
        return new PlaceBuildingResult(true, buildingId: buildingId, costPaid: costPaid);
    }

    public static PlaceBuildingResult Failed(string errorMessage)
    {
        return new PlaceBuildingResult(false, errorMessage);
    }
}
