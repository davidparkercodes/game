using Game.Application.UI.Configuration;

namespace Game.Application.UI.Services;

public interface IBuildingSelectionHudConfigService
{
    BuildingSelectionHudConfig GetConfiguration();
    bool IsConfigurationValid();
    string GetDefaultIconPath();
    string GetValidatedIconPath(string iconPath);
    bool DoesIconExist(string iconPath);
}
