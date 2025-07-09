using Game.Domain.Buildings.Services;
using Game.Presentation.Buildings;
using Game.Infrastructure.Stats.Services;
using Game.Infrastructure.Game.Services;
using Game.Infrastructure.Economy.Services;
using Game.Presentation.UI;
using Godot;

namespace Game.Application.Buildings.Services;

public class TowerSellService : ITowerSellService
{
    private const string LogPrefix = "💰 [TOWER-SELL]";
    
    public int GetSellValue(Building building)
    {
        if (building == null) return 0;
        
        string buildingType = GetBuildingConfigKey(building);
        return GetSellValue(buildingType, building.UpgradeLevel, building.TotalInvestment);
    }
    
    public int GetSellValue(string buildingType, int upgradeLevel, int totalInvestment)
    {
        float sellPercentage = GetSellPercentage(buildingType);
        
        // If total investment is not tracked, calculate it
        if (totalInvestment <= 0)
        {
            totalInvestment = CalculateTotalInvestment(buildingType, upgradeLevel);
        }
        
        return (int)(totalInvestment * sellPercentage);
    }
    
    public bool SellBuilding(Building building)
    {
        if (building == null)
        {
            GD.PrintErr($"{LogPrefix} Cannot sell null building");
            return false;
        }
        
        try
        {
            // Calculate sell value
            int sellValue = GetSellValue(building);
            string buildingName = building.Name;
            
            // Add money to player
            GameService.Instance?.ReceiveMoneyFromSale(sellValue, buildingName);
            
            // Remove building from BuildingRegistry
            BuildingRegistry.Instance.UnregisterBuilding(building);
            
            // Notify BuildingSelectionManager that building is being destroyed
            BuildingSelectionManager.Instance.OnBuildingDestroyed(building);
            
            // Remove building from scene
            building.QueueFree();
            
            GD.Print($"{LogPrefix} Successfully sold {building.Name} for ${sellValue}");
            return true;
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix} Error selling building: {ex.Message}");
            return false;
        }
    }
    
    public float GetSellPercentage(string buildingType)
    {
        // Use economy config service for sell percentage
        return GameEconomyConfigService.Instance.GetSellPercentage();
    }
    
    public int CalculateTotalInvestment(Building building)
    {
        if (building == null) return 0;
        
        // If building tracks total investment, use it
        if (building.TotalInvestment > 0)
        {
            return building.TotalInvestment;
        }
        
        // Otherwise, calculate based on building type and upgrade level
        string buildingType = GetBuildingConfigKey(building);
        return CalculateTotalInvestment(buildingType, building.UpgradeLevel);
    }
    
    public int CalculateTotalInvestment(string buildingType, int upgradeLevel)
    {
        if (StatsManagerService.Instance == null) return 0;
        
        try
        {
            var baseStats = StatsManagerService.Instance.GetBuildingStats(buildingType);
            
            // Start with base cost
            int totalInvestment = baseStats.cost;
            
            // Add upgrade costs for each level
            for (int level = 0; level < upgradeLevel; level++)
            {
                totalInvestment += CalculateUpgradeCostForLevel(buildingType, level);
            }
            
            return totalInvestment;
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix} Error calculating total investment for {buildingType}: {ex.Message}");
            return 0;
        }
    }
    
    private int CalculateUpgradeCostForLevel(string buildingType, int level)
    {
        if (StatsManagerService.Instance == null) return 0;
        
        try
        {
            var baseStats = StatsManagerService.Instance.GetBuildingStats(buildingType);
            
            // Calculate upgrade cost based on base upgrade cost and level
            // Use economy config for upgrade cost multiplier
            int baseCost = baseStats.upgrade_cost;
            float upgradeMultiplier = GameEconomyConfigService.Instance.GetUpgradeCostMultiplier();
            float multiplier = 1.0f + (level * (upgradeMultiplier - 1.0f));
            
            return (int)(baseCost * multiplier);
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix} Error calculating upgrade cost for level {level}: {ex.Message}");
            return 0;
        }
    }
    
    private string GetBuildingConfigKey(Building building)
    {
        // Extract building type from class name and convert to config key
        string className = building.GetType().Name;
        
        // Convert from PascalCase to snake_case
        return className.ToLower() switch
        {
            "basictower" => "basic_tower",
            "snipertower" => "sniper_tower",
            "rapidtower" => "rapid_tower",
            "heavytower" => "heavy_tower",
            _ => "basic_tower" // fallback
        };
    }
}
