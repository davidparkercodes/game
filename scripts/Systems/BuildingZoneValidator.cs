using Godot;

public static class BuildingZoneValidator
{
    private static TileMapLayer _groundLayer;
    
    public static void Initialize(TileMapLayer groundLayer)
    {
        _groundLayer = groundLayer;
        GD.Print("🏗️ BuildingZoneValidator initialized");
    }
    
    public static bool CanBuildAt(Vector2 worldPosition)
    {
        if (_groundLayer == null)
        {
            GD.PrintErr("❌ GroundLayer not initialized in BuildingZoneValidator");
            return false;
        }
        
        Vector2I tileCoords = _groundLayer.LocalToMap(_groundLayer.ToLocal(worldPosition));
        
        TileData tileData = _groundLayer.GetCellTileData(tileCoords);
        if (tileData == null)
        {
            return false;
        }
        
        bool isBuildable = IsValidBuildingTile(tileCoords);
        
        if (!isBuildable)
        {
            GD.Print($"🚫 Cannot build at {worldPosition} (tile {tileCoords}) - on restricted area");
        }
        
        return isBuildable;
    }
    
    private static bool IsValidBuildingTile(Vector2I tileCoords)
    {
        if (_groundLayer == null) return false;
        
        // Get the source ID and atlas coordinates of the tile
        int sourceId = _groundLayer.GetCellSourceId(tileCoords);
        Vector2I atlasCoords = _groundLayer.GetCellAtlasCoords(tileCoords);
        
        // If there's no tile at this position, consider it non-buildable
        if (sourceId == -1)
        {
            return false;
        }
        
        // Check if this is a lane tile (black tile at top-left atlas coordinates 0,0)
        bool isLaneTile = (atlasCoords.X == 0 && atlasCoords.Y == 0);
        
        // Debug logging (can be removed later)
        if (isLaneTile)
        {
            GD.Print($"🔍 Lane tile detected at {tileCoords}: atlas({atlasCoords.X},{atlasCoords.Y})");
        }
        
        return !isLaneTile;
    }
    
    public static bool IsOnPath(Vector2 worldPosition)
    {
        if (_groundLayer == null) return false;
        
        Vector2I tileCoords = _groundLayer.LocalToMap(_groundLayer.ToLocal(worldPosition));
        return !IsValidBuildingTile(tileCoords);
    }
}
