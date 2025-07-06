using Game.Domain.Common.Types;
using Game.Domain.Map.Interfaces;
using Godot;

namespace Game.Infrastructure.Map.Adapters;

public class TileMapLayerAdapter : ITileMapLayer
{
    private readonly TileMapLayer _tileMapLayer;
    
    public TileMapLayerAdapter(TileMapLayer tileMapLayer)
    {
        _tileMapLayer = tileMapLayer;
    }
    
    public Domain.Common.Types.Vector2 MapToLocal(Domain.Map.Interfaces.Vector2I tileCoords)
    {
        return _tileMapLayer.MapToLocal(tileCoords);
    }
    
    public Domain.Map.Interfaces.Vector2I LocalToMap(Domain.Common.Types.Vector2 localPosition)
    {
        return _tileMapLayer.LocalToMap(localPosition);
    }
    
    public Domain.Common.Types.Vector2 ToLocal(Domain.Common.Types.Vector2 globalPosition)
    {
        return _tileMapLayer.ToLocal(globalPosition);
    }
    
    public Domain.Map.Interfaces.Rect2I GetUsedRect()
    {
        return _tileMapLayer.GetUsedRect();
    }
    
    public int GetCellSourceId(Domain.Map.Interfaces.Vector2I coords)
    {
        return _tileMapLayer.GetCellSourceId(coords);
    }
    
    public Domain.Map.Interfaces.Vector2I GetCellAtlasCoords(Domain.Map.Interfaces.Vector2I coords)
    {
        return _tileMapLayer.GetCellAtlasCoords(coords);
    }
    
    public object? GetCellTileData(Domain.Map.Interfaces.Vector2I coords)
    {
        return _tileMapLayer.GetCellTileData(coords);
    }
}
