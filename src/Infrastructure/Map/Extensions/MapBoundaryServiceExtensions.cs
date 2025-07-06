using Game.Domain.Map.Services;
using Game.Infrastructure.Map.Adapters;
using Game.Infrastructure.Map.Services;
using Godot;

namespace Game.Infrastructure.Map.Extensions;

public static class MapBoundaryServiceExtensions
{
    public static void Initialize(this IMapBoundaryService service, TileMapLayer godotTileMapLayer)
    {
        var adapter = new TileMapLayerAdapter(godotTileMapLayer);
        service.Initialize(adapter);
    }
}
