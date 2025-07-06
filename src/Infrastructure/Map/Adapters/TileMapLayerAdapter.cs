using Game.Domain.Common.Types;
using Game.Domain.Map.Interfaces;
using Game.Infrastructure.Common.Converters;
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
        var godotResult = _tileMapLayer.MapToLocal(tileCoords);
        return GodotGeometryConverter.FromGodotVector2(godotResult);
    }
    
    public Domain.Map.Interfaces.Vector2I LocalToMap(Domain.Common.Types.Vector2 localPosition)
    {
        var godotPosition = GodotGeometryConverter.ToGodotVector2(localPosition);
        return _tileMapLayer.LocalToMap(godotPosition);
    }
    
    public Domain.Common.Types.Vector2 ToLocal(Domain.Common.Types.Vector2 globalPosition)
    {
        var godotPosition = GodotGeometryConverter.ToGodotVector2(globalPosition);
        var godotResult = _tileMapLayer.ToLocal(godotPosition);
        return GodotGeometryConverter.FromGodotVector2(godotResult);
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
