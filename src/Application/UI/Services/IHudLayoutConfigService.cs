using Game.Application.UI.Configuration;

namespace Game.Application.UI.Services;

public interface IHudLayoutConfigService
{
    HudLayoutConfig GetConfiguration();
    string GetValidatedAssetPath(string assetPath);
}
