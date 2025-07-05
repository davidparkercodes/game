using Godot;

namespace Game.Presentation.Systems;

public static class BuildingZoneValidator
{
    private static TileMapLayer? _groundLayer;
    
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
        
        return IsValidBuildingTile(tileCoords);
    }
    
    private static bool IsValidBuildingTile(Vector2I tileCoords)
    {
        if (_groundLayer == null) return false;
        
        int sourceId = _groundLayer.GetCellSourceId(tileCoords);
        Vector2I atlasCoords = _groundLayer.GetCellAtlasCoords(tileCoords);
        
        if (sourceId == -1)
        {
            return false;
        }
        
        bool isLaneTile = (atlasCoords.X == 0 && atlasCoords.Y == 0);
        
        return !isLaneTile;
    }
    
    public static bool IsOnPath(Vector2 worldPosition)
    {
        if (_groundLayer == null) return false;
        
        Vector2I tileCoords = _groundLayer.LocalToMap(_groundLayer.ToLocal(worldPosition));
        return !IsValidBuildingTile(tileCoords);
    }
    
    public static bool CanBuildAtWithLogging(Vector2 worldPosition)
    {
        bool canBuild = CanBuildAt(worldPosition);
        
        if (!canBuild && _groundLayer != null)
        {
            Vector2I tileCoords = _groundLayer.LocalToMap(_groundLayer.ToLocal(worldPosition));
            GD.Print($"🚫 Cannot place building at {worldPosition} (tile {tileCoords}) - on restricted area");
        }
        
        return canBuild;
    }
}
