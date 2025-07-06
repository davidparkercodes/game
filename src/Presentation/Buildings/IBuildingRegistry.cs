using Godot;
using System.Collections.Generic;

namespace Game.Presentation.Buildings;

/// <summary>
/// Registry service for tracking placed buildings and handling collision detection
/// </summary>
public interface IBuildingRegistry
{
    /// <summary>
    /// Register a building in the registry when it's placed
    /// </summary>
    /// <param name="building">The building to register</param>
    void RegisterBuilding(Building building);
    
    /// <summary>
    /// Unregister a building from the registry when it's destroyed
    /// </summary>
    /// <param name="building">The building to unregister</param>
    void UnregisterBuilding(Building building);
    
    /// <summary>
    /// Check if a position is occupied by any building within the specified radius
    /// </summary>
    /// <param name="position">The position to check</param>
    /// <param name="radius">The collision radius to check</param>
    /// <returns>True if position is occupied, false otherwise</returns>
    bool IsPositionOccupied(Vector2 position, float radius);
    
    /// <summary>
    /// Get the building at the specified position within the radius, if any
    /// </summary>
    /// <param name="position">The position to check</param>
    /// <param name="radius">The search radius</param>
    /// <returns>The building at that position, or null if none found</returns>
    Building? GetBuildingAt(Vector2 position, float radius);
    
    /// <summary>
    /// Get all registered buildings
    /// </summary>
    /// <returns>Collection of all registered buildings</returns>
    IReadOnlyCollection<Building> GetAllBuildings();
    
    /// <summary>
    /// Clear all registered buildings (useful for scene changes)
    /// </summary>
    void ClearAllBuildings();
}
