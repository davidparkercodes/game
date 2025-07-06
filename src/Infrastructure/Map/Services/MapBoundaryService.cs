using Godot;
using Game.Domain.Map.Services;
using Game.Domain.Common.Types;
using Game.Domain.Map.Interfaces;
using Game.Infrastructure.Map.Adapters;

namespace Game.Infrastructure.Map.Services;

public class MapBoundaryService : IMapBoundaryService
{
    private ITileMapLayer? _tileMapLayer;
    private bool _isInitialized = false;
    private Domain.Common.Types.Rect2 _mapBounds;
    private const float ABYSS_BUFFER_DISTANCE = 64.0f; // pixels beyond map edge
    private const int BLACK_TILE_SOURCE_ID = 0;
    private const int BLACK_TILE_ATLAS_X = 0;
    private const int BLACK_TILE_ATLAS_Y = 0;
    
    public bool IsInitialized => _isInitialized;
    
    public void Initialize(ITileMapLayer tileMapLayer)
    {
        _tileMapLayer = tileMapLayer;
        _mapBounds = CalculateMapBounds();
        _isInitialized = true;
        
        GD.Print($"🗺️ MapBoundaryService initialized");
        GD.Print($"🗺️ Map bounds: {_mapBounds}");
        GD.Print($"🗺️ Abyss buffer distance: {ABYSS_BUFFER_DISTANCE}px");
    }
    
    public bool CanWalkToPosition(Domain.Common.Types.Vector2 worldPosition)
    {
        if (!_isInitialized || _tileMapLayer == null)
            return false;
            
        // Only allow walking within the actual map bounds (no abyss walking)
        if (!_mapBounds.HasPoint(worldPosition))
            return false; // Cannot walk in abyss area
            
        // Within map bounds - check if there's a tile at this position
        return CanWalkOnTile(worldPosition);
    }
    
    public bool CanBuildAtPosition(Domain.Common.Types.Vector2 worldPosition)
    {
        if (!_isInitialized || _tileMapLayer == null)
            return false;
            
        // Building only allowed within map bounds, not in abyss
        if (!_mapBounds.HasPoint(worldPosition))
            return false;
            
        return CanBuildOnTile(worldPosition);
    }
    
    public bool IsWithinMapBounds(Domain.Common.Types.Vector2 worldPosition)
    {
        return _mapBounds.HasPoint(worldPosition);
    }
    
    public bool IsInAbyssBufferZone(Domain.Common.Types.Vector2 worldPosition)
    {
        if (!_isInitialized)
            return false;
            
        // Not in map bounds but within expanded bounds
        var expandedBounds = _mapBounds.Grow(ABYSS_BUFFER_DISTANCE);
        return !_mapBounds.HasPoint(worldPosition) && expandedBounds.HasPoint(worldPosition);
    }
    
    public Domain.Common.Types.Vector2 ClampToValidPosition(Domain.Common.Types.Vector2 worldPosition)
    {
        if (!_isInitialized)
            return worldPosition;
            
        // Clamp to map bounds only (no abyss buffer)
        var clampedX = Mathf.Clamp(worldPosition.X, _mapBounds.Position.X, _mapBounds.End.X);
        var clampedY = Mathf.Clamp(worldPosition.Y, _mapBounds.Position.Y, _mapBounds.End.Y);
        
        return new Domain.Common.Types.Vector2(clampedX, clampedY);
    }
    
    public Domain.Common.Types.Rect2 GetMapBounds()
    {
        return _mapBounds;
    }
    
    public float GetAbyssBufferDistance()
    {
        return ABYSS_BUFFER_DISTANCE;
    }
    
    private Domain.Common.Types.Rect2 CalculateMapBounds()
    {
        if (_tileMapLayer == null)
            return new Domain.Common.Types.Rect2();
            
        var usedRect = _tileMapLayer.GetUsedRect();
        if (usedRect.Size.X == 0 && usedRect.Size.Y == 0)
            return new Domain.Common.Types.Rect2();
            
        // Convert tile coordinates to world coordinates
        var topLeft = _tileMapLayer.MapToLocal(usedRect.Position);
        var bottomRight = _tileMapLayer.MapToLocal(usedRect.End);
        
        return new Domain.Common.Types.Rect2(topLeft, new Domain.Common.Types.Vector2(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y));
    }
    
    private bool CanWalkOnTile(Domain.Common.Types.Vector2 worldPosition)
    {
        if (_tileMapLayer == null)
            return false;
            
        var tileCoords = _tileMapLayer.LocalToMap(_tileMapLayer.ToLocal(worldPosition));
        var tileData = _tileMapLayer.GetCellTileData(tileCoords);
        
        if (tileData == null)
            return false; // No tile at this position - cannot walk in empty space
            
        // If there's a tile here, player can walk on it (both black and white tiles)
        // Black tiles = wave paths, White tiles = buildable areas
        // Both are walkable surfaces
        return true;
    }
    
    private bool CanBuildOnTile(Domain.Common.Types.Vector2 worldPosition)
    {
        if (_tileMapLayer == null)
            return false;
            
        var tileCoords = _tileMapLayer.LocalToMap(_tileMapLayer.ToLocal(worldPosition));
        var tileData = _tileMapLayer.GetCellTileData(tileCoords);
        
        if (tileData == null)
            return false;
            
        // Check if this is NOT a black tile (can build on white tiles, not black tiles)
        var sourceId = _tileMapLayer.GetCellSourceId(tileCoords);
        var atlasCoords = _tileMapLayer.GetCellAtlasCoords(tileCoords);
        
        bool isBlackTile = (sourceId == BLACK_TILE_SOURCE_ID && 
                           atlasCoords.X == BLACK_TILE_ATLAS_X && 
                           atlasCoords.Y == BLACK_TILE_ATLAS_Y);
        
        return !isBlackTile; // Can build on non-black tiles (white tiles = buildable areas)
    }
    
    private bool IsValidAbyssPosition(Domain.Common.Types.Vector2 worldPosition)
    {
        if (_tileMapLayer == null)
            return false;
            
        // For now, allow walking in abyss buffer zone
        // Could add more sophisticated logic here (e.g., only allow if adjacent to white tile edges)
        return true;
    }
}
