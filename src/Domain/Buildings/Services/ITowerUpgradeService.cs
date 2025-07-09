using Game.Presentation.Buildings;

namespace Game.Domain.Buildings.Services;

public interface ITowerUpgradeService
{
    bool CanUpgrade(Building building);
    int GetUpgradeCost(Building building);
    int GetUpgradeCost(string buildingType, int currentLevel);
    (int damage, float range, float attackSpeed) GetUpgradePreview(Building building);
    (int damage, float range, float attackSpeed) GetUpgradePreview(string buildingType, int currentLevel);
    bool UpgradeBuilding(Building building);
    int GetMaxUpgradeLevel(string buildingType);
    int GetMaxUpgradeLevel(Building building);
    float GetUpgradeMultiplier(string buildingType);
    bool IsAtMaxLevel(Building building);
}
