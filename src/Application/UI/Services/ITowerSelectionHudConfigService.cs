using Game.Application.UI.Configuration;

namespace Game.Application.UI.Services;

public interface ITowerSelectionHudConfigService
{
    TowerSelectionHudConfig GetConfiguration();
    bool IsConfigurationValid();
    string GetDefaultIconPath();
    string GetValidatedIconPath(string iconPath);
    bool DoesIconExist(string iconPath);
}
