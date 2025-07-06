using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Game.Presentation.Buildings;

/// <summary>
/// Singleton service for tracking placed buildings and handling collision detection
/// </summary>
public class BuildingRegistry : IBuildingRegistry
{
    private static BuildingRegistry? _instance;
    public static BuildingRegistry Instance => _instance ??= new BuildingRegistry();
    
    private readonly List<Building> _registeredBuildings = new List<Building>();
    private const string LogPrefix = "🏗️ [BUILDING-REGISTRY]";
    
    /// <summary>
    /// Default collision radius for buildings (12 pixels = good for 16x16 buildings with small buffer)
    /// </summary>
    public const float DefaultCollisionRadius = 12.0f;
    
    private BuildingRegistry()
    {
        GD.Print($"{LogPrefix} Building registry initialized");
    }
    
    public void RegisterBuilding(Building building)
    {
        if (building == null)
        {
            GD.PrintErr($"{LogPrefix} Cannot register null building");
            return;
        }
        
        if (_registeredBuildings.Contains(building))
        {
            GD.PrintErr($"{LogPrefix} Building {building.Name} already registered at {building.GlobalPosition}");
            return;
        }
        
        _registeredBuildings.Add(building);
        GD.Print($"{LogPrefix} Registered building {building.Name} at {building.GlobalPosition} (Total: {_registeredBuildings.Count})");
    }
    
    public void UnregisterBuilding(Building building)
    {
        if (building == null)
        {
            GD.PrintErr($"{LogPrefix} Cannot unregister null building");
            return;
        }
        
        if (_registeredBuildings.Remove(building))
        {
            GD.Print($"{LogPrefix} Unregistered building {building.Name} from {building.GlobalPosition} (Total: {_registeredBuildings.Count})");
        }
        else
        {
            GD.PrintErr($"{LogPrefix} Building {building.Name} was not registered");
        }
    }
    
    public bool IsPositionOccupied(Vector2 position, float radius)
    {
        return GetBuildingAt(position, radius) != null;
    }
    
    public Building? GetBuildingAt(Vector2 position, float radius)
    {
        // Clean up any invalid buildings first
        CleanupInvalidBuildings();
        
        foreach (var building in _registeredBuildings)
        {
            if (building == null || !Godot.GodotObject.IsInstanceValid(building))
                continue;
                
            float distance = building.GlobalPosition.DistanceTo(position);
            float combinedRadius = radius + building.CollisionRadius;
            
            if (distance < combinedRadius)
            {
                return building;
            }
        }
        
        return null;
    }
    
    public IReadOnlyCollection<Building> GetAllBuildings()
    {
        CleanupInvalidBuildings();
        return _registeredBuildings.AsReadOnly();
    }
    
    public void ClearAllBuildings()
    {
        int count = _registeredBuildings.Count;
        _registeredBuildings.Clear();
        GD.Print($"{LogPrefix} Cleared all buildings from registry (Removed: {count})");
    }
    
    /// <summary>
    /// Remove any buildings that have been freed or are no longer valid
    /// </summary>
    private void CleanupInvalidBuildings()
    {
        for (int i = _registeredBuildings.Count - 1; i >= 0; i--)
        {
            var building = _registeredBuildings[i];
            if (building == null || !Godot.GodotObject.IsInstanceValid(building))
            {
                _registeredBuildings.RemoveAt(i);
            }
        }
    }
    
    /// <summary>
    /// Get debugging information about the registry state
    /// </summary>
    public string GetDebugInfo()
    {
        CleanupInvalidBuildings();
        return $"BuildingRegistry: {_registeredBuildings.Count} buildings registered";
    }
    
    /// <summary>
    /// Reset the singleton instance (useful for testing)
    /// </summary>
    public static void ResetInstance()
    {
        _instance?.ClearAllBuildings();
        _instance = null;
    }
    
    /// <summary>
    /// Force cleanup of all registered buildings - useful for debugging
    /// </summary>
    public void ForceCleanupAll()
    {
        GD.Print($"{LogPrefix} Force cleanup - Before: {_registeredBuildings.Count} buildings");
        CleanupInvalidBuildings();
        GD.Print($"{LogPrefix} Force cleanup - After: {_registeredBuildings.Count} buildings");
        
        // List all remaining buildings
        foreach (var building in _registeredBuildings)
        {
            GD.Print($"{LogPrefix} Remaining building: {building.Name} at {building.GlobalPosition}");
        }
    }
}
