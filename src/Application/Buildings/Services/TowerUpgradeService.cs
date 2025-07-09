using Game.Domain.Buildings.Services;
using Game.Presentation.Buildings;
using Game.Infrastructure.Stats.Services;
using Game.Infrastructure.Game.Services;
using Godot;

namespace Game.Application.Buildings.Services;

public class TowerUpgradeService : ITowerUpgradeService
{
    private const string LogPrefix = "🔧 [TOWER-UPGRADE]";
    private const float DefaultUpgradeMultiplier = 1.5f;
    private const int DefaultMaxUpgradeLevel = 3;
    
    public bool CanUpgrade(Building building)
    {
        if (building == null) return false;
        
        // Check if building is at max level
        if (IsAtMaxLevel(building)) return false;
        
        // Check if player has enough money
        int upgradeCost = GetUpgradeCost(building);
        return GameService.Instance?.CanAffordUpgrade(upgradeCost) ?? false;
    }
    
    public int GetUpgradeCost(Building building)
    {
        if (building == null) return 0;
        
        string buildingType = GetBuildingConfigKey(building);
        return GetUpgradeCost(buildingType, building.UpgradeLevel);
    }
    
    public int GetUpgradeCost(string buildingType, int currentLevel)
    {
        if (StatsManagerService.Instance == null) return 0;
        
        try
        {
            var baseStats = StatsManagerService.Instance.GetBuildingStats(buildingType);
            
            // Calculate upgrade cost based on base upgrade cost and current level
            // Each level increases cost by 50% (configurable)
            int baseCost = baseStats.upgrade_cost;
            float multiplier = 1.0f + (currentLevel * 0.5f);
            
            return (int)(baseCost * multiplier);
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix} Error getting upgrade cost for {buildingType}: {ex.Message}");
            return 0;
        }
    }
    
    public (int damage, float range, float attackSpeed) GetUpgradePreview(Building building)
    {
        if (building == null) return (0, 0, 0);
        
        string buildingType = GetBuildingConfigKey(building);
        return GetUpgradePreview(buildingType, building.UpgradeLevel);
    }
    
    public (int damage, float range, float attackSpeed) GetUpgradePreview(string buildingType, int currentLevel)
    {
        if (IsAtMaxLevel(buildingType, currentLevel)) return (0, 0, 0);
        
        try
        {
            var baseStats = StatsManagerService.Instance?.GetBuildingStats(buildingType);
            if (baseStats == null) return (0, 0, 0);
            
            float multiplier = GetUpgradeMultiplier(buildingType);
            float totalMultiplier = 1.0f + (multiplier - 1.0f) * (currentLevel + 1);
            
            return (
                damage: (int)(baseStats.Value.damage * totalMultiplier),
                range: baseStats.Value.range * totalMultiplier,
                attackSpeed: baseStats.Value.attack_speed * totalMultiplier
            );
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix} Error getting upgrade preview for {buildingType}: {ex.Message}");
            return (0, 0, 0);
        }
    }
    
    public bool UpgradeBuilding(Building building)
    {
        if (building == null)
        {
            GD.PrintErr($"{LogPrefix} Cannot upgrade null building");
            return false;
        }
        
        if (!CanUpgrade(building))
        {
            GD.Print($"{LogPrefix} Cannot upgrade building {building.Name} - requirements not met");
            return false;
        }
        
        // Get upgrade cost and deduct money
        int upgradeCost = GetUpgradeCost(building);
        string buildingName = building.Name;
        if (GameService.Instance?.SpendMoneyOnUpgrade(upgradeCost, buildingName) != true)
        {
            GD.PrintErr($"{LogPrefix} Failed to spend money for upgrade");
            return false;
        }
        
        // Apply upgrade to building
        try
        {
            ApplyUpgradeToBuilding(building);
            
            // Update building's total investment
            building.TotalInvestment += upgradeCost;
            
            GD.Print($"{LogPrefix} Successfully upgraded {building.Name} to level {building.UpgradeLevel}");
            return true;
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix} Error applying upgrade to building: {ex.Message}");
            
            // Refund money if upgrade failed
            GameService.Instance?.AddMoney(upgradeCost);
            return false;
        }
    }
    
    private void ApplyUpgradeToBuilding(Building building)
    {
        // Increase upgrade level
        building.UpgradeLevel++;
        
        // Calculate new stats
        string buildingType = GetBuildingConfigKey(building);
        float multiplier = GetUpgradeMultiplier(buildingType);
        float totalMultiplier = 1.0f + (multiplier - 1.0f) * building.UpgradeLevel;
        
        // Apply new stats to building
        building.Damage = (int)(building.BaseStats.Damage * totalMultiplier);
        building.Range = building.BaseStats.Range * totalMultiplier;
        building.AttackSpeed = building.BaseStats.AttackSpeed * totalMultiplier;
        
        // Update building's visual appearance for upgrades
        UpdateBuildingVisuals(building);
        
		// Reinitialize stats to update internal components
		building.InitializeStats();
		
		// Update range visual circle to reflect new range
		building.UpdateRangeVisual();
    }
    
    private void UpdateBuildingVisuals(Building building)
    {
        // Update upgrade level visual indicators
        building.UpdateUpgradeVisuals();
        
        GD.Print($"{LogPrefix} Updated visuals for {building.Name} at upgrade level {building.UpgradeLevel}");
    }
    
    public int GetMaxUpgradeLevel(string buildingType)
    {
        // TODO: Load from configuration
        // For now, use default max level
        return DefaultMaxUpgradeLevel;
    }
    
    public int GetMaxUpgradeLevel(Building building)
    {
        if (building == null) return 0;
        
        string buildingType = GetBuildingConfigKey(building);
        return GetMaxUpgradeLevel(buildingType);
    }
    
    public float GetUpgradeMultiplier(string buildingType)
    {
        // TODO: Load from configuration per building type
        // For now, use default multiplier
        return DefaultUpgradeMultiplier;
    }
    
    public bool IsAtMaxLevel(Building building)
    {
        if (building == null) return true;
        
        return building.UpgradeLevel >= GetMaxUpgradeLevel(building);
    }
    
    private bool IsAtMaxLevel(string buildingType, int currentLevel)
    {
        return currentLevel >= GetMaxUpgradeLevel(buildingType);
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
