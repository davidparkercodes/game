using System.Collections.Generic;
using Game.Domain.Shared.ValueObjects;

namespace Game.Domain.Buildings.Services;

public interface IPlacementStrategyProvider
{
    /// <summary>
    /// Gets the building category to use for initial wave placement
    /// </summary>
    string GetInitialBuildingCategory();
    
    /// <summary>
    /// Gets the positions where initial buildings should be placed
    /// </summary>
    IEnumerable<Position> GetInitialBuildingPositions();
    
    /// <summary>
    /// Gets the maximum cost per building for initial placement
    /// </summary>
    int GetMaxCostPerBuilding();
    
    /// <summary>
    /// Gets the building category to use for a specific wave upgrade
    /// </summary>
    /// <param name="waveNumber">The current wave number</param>
    /// <param name="availableMoney">The amount of money available</param>
    /// <returns>Building category if upgrade should be placed, null otherwise</returns>
    string? GetUpgradeBuildingCategory(int waveNumber, int availableMoney);
    
    /// <summary>
    /// Gets the position where an upgrade building should be placed
    /// </summary>
    /// <param name="waveNumber">The current wave number</param>
    /// <returns>Position for the upgrade building, null if no upgrade</returns>
    Position? GetUpgradeBuildingPosition(int waveNumber);
    
    /// <summary>
    /// Gets a fallback building type when preferred types are unavailable
    /// </summary>
    /// <returns>Fallback building type config key</returns>
    string GetFallbackBuildingType();
}
