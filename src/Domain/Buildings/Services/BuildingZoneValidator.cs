using System;
using System.Collections.Generic;
using System.Linq;
using Game.Domain.Buildings.Entities;

namespace Game.Domain.Buildings.Services;

public class BuildingZoneValidator
{
    private readonly List<Game.Domain.Buildings.Entities.Building> _existingBuildings;
    private readonly float _minimumBuildingDistance;
    private readonly HashSet<(float X, float Y)> _blockedZones;

    public BuildingZoneValidator(float minimumBuildingDistance = 50.0f)
    {
        _existingBuildings = new List<Game.Domain.Buildings.Entities.Building>();
        _minimumBuildingDistance = minimumBuildingDistance;
        _blockedZones = new HashSet<(float X, float Y)>();
    }

    public bool CanPlaceBuilding(float x, float y, string buildingType = null)
    {
        if (IsInBlockedZone(x, y))
            return false;

        if (IsTooCloseToExistingBuilding(x, y))
            return false;

        if (IsOutOfBounds(x, y))
            return false;

        return true;
    }

    public bool CanBuildAt(Godot.Vector2 position)
    {
        return CanPlaceBuilding(position.X, position.Y);
    }

    public void AddBuilding(Game.Domain.Buildings.Entities.Building building)
    {
        if (building == null)
            throw new ArgumentNullException(nameof(building));

        if (!CanPlaceBuilding(building.X, building.Y))
            throw new InvalidOperationException($"Cannot place building at ({building.X}, {building.Y})");

        _existingBuildings.Add(building);
    }

    public void RemoveBuilding(Game.Domain.Buildings.Entities.Building building)
    {
        if (building == null)
            throw new ArgumentNullException(nameof(building));

        _existingBuildings.Remove(building);
    }

    public void AddBlockedZone(float x, float y)
    {
        _blockedZones.Add((x, y));
    }

    public void RemoveBlockedZone(float x, float y)
    {
        _blockedZones.Remove((x, y));
    }

    public bool IsInBlockedZone(float x, float y)
    {
        return _blockedZones.Any(zone => 
            Math.Abs(zone.X - x) < _minimumBuildingDistance && 
            Math.Abs(zone.Y - y) < _minimumBuildingDistance);
    }

    public bool IsTooCloseToExistingBuilding(float x, float y)
    {
        return _existingBuildings.Any(building => 
            building.CalculateDistance(x, y) < _minimumBuildingDistance);
    }

    public bool IsOutOfBounds(float x, float y)
    {
        return x < 0 || y < 0 || x > 1920 || y > 1080;
    }

    public IEnumerable<Game.Domain.Buildings.Entities.Building> GetNearbyBuildings(float x, float y, float radius)
    {
        return _existingBuildings.Where(building => 
            building.CalculateDistance(x, y) <= radius);
    }

    public Game.Domain.Buildings.Entities.Building GetClosestBuilding(float x, float y)
    {
        return _existingBuildings.OrderBy(building => 
            building.CalculateDistance(x, y)).FirstOrDefault();
    }

    public void ClearAll()
    {
        _existingBuildings.Clear();
        _blockedZones.Clear();
    }
}
