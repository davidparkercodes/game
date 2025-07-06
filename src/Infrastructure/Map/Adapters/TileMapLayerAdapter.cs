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
        var godotTileCoords = GodotGeometryConverter.ToGodotVector2I(tileCoords);
        var godotResult = _tileMapLayer.MapToLocal(godotTileCoords);
        return GodotGeometryConverter.FromGodotVector2(godotResult);
    }
    
    public Domain.Map.Interfaces.Vector2I LocalToMap(Domain.Common.Types.Vector2 localPosition)
    {
        var godotPosition = GodotGeometryConverter.ToGodotVector2(localPosition);
        var godotResult = _tileMapLayer.LocalToMap(godotPosition);
        return GodotGeometryConverter.FromGodotVector2I(godotResult);
    }
    
    public Domain.Common.Types.Vector2 ToLocal(Domain.Common.Types.Vector2 globalPosition)
    {
        var godotPosition = GodotGeometryConverter.ToGodotVector2(globalPosition);
        var godotResult = _tileMapLayer.ToLocal(godotPosition);
        return GodotGeometryConverter.FromGodotVector2(godotResult);
    }
    
    public Domain.Map.Interfaces.Rect2I GetUsedRect()
    {
        var godotResult = _tileMapLayer.GetUsedRect();
        return GodotGeometryConverter.FromGodotRect2I(godotResult);
    }
    
    public int GetCellSourceId(Domain.Map.Interfaces.Vector2I coords)
    {
        var godotCoords = GodotGeometryConverter.ToGodotVector2I(coords);
        return _tileMapLayer.GetCellSourceId(godotCoords);
    }
    
    public Domain.Map.Interfaces.Vector2I GetCellAtlasCoords(Domain.Map.Interfaces.Vector2I coords)
    {
        var godotCoords = GodotGeometryConverter.ToGodotVector2I(coords);
        var godotResult = _tileMapLayer.GetCellAtlasCoords(godotCoords);
        return GodotGeometryConverter.FromGodotVector2I(godotResult);
    }
    
    public object? GetCellTileData(Domain.Map.Interfaces.Vector2I coords)
    {
        var godotCoords = GodotGeometryConverter.ToGodotVector2I(coords);
        return _tileMapLayer.GetCellTileData(godotCoords);
    }
}
