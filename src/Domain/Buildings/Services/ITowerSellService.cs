using Game.Presentation.Buildings;

namespace Game.Domain.Buildings.Services;

public interface ITowerSellService
{
    int GetSellValue(Building building);
    int GetSellValue(string buildingType, int upgradeLevel, int totalInvestment);
    bool SellBuilding(Building building);
    float GetSellPercentage(string buildingType);
    int CalculateTotalInvestment(Building building);
    int CalculateTotalInvestment(string buildingType, int upgradeLevel);
}
