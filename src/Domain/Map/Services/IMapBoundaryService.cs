using Game.Domain.Common.Types;
using Game.Domain.Map.Interfaces;

namespace Game.Domain.Map.Services;

public interface IMapBoundaryService
{
    void Initialize(ITileMapLayer tileMapLayer);
    bool IsInitialized { get; }
    bool CanWalkToPosition(Vector2 worldPosition);
    bool CanBuildAtPosition(Vector2 worldPosition);
    bool IsWithinMapBounds(Vector2 worldPosition);
    bool IsInAbyssBufferZone(Vector2 worldPosition);
    Vector2 ClampToValidPosition(Vector2 worldPosition);
    Rect2 GetMapBounds();
    float GetAbyssBufferDistance();
}
