using Godot;
using System.Collections.Generic;

public partial class BuildingManager : Node
{
	private List<Building> _buildings = new List<Building>();
	
	public override void _Ready()
	{
		
	}
	
	public void AddBuilding(Building building)
	{
		if (building != null && !_buildings.Contains(building))
		{
			_buildings.Add(building);
			GD.Print($"🏗️ Added {building.GetType().Name} to BuildingManager. Total buildings: {_buildings.Count}");
		}
	}
	
	public void RemoveBuilding(Building building)
	{
		if (building != null && _buildings.Contains(building))
		{
			_buildings.Remove(building);
			GD.Print($"🗑️ Removed {building.GetType().Name} from BuildingManager. Total buildings: {_buildings.Count}");
		}
	}
	
	public List<Building> GetAllBuildings()
	{
		return new List<Building>(_buildings);
	}
	
	public List<T> GetBuildingsOfType<T>() where T : Building
	{
		var result = new List<T>();
		foreach (var building in _buildings)
		{
			if (building is T typedBuilding)
				result.Add(typedBuilding);
		}
		return result;
	}
	
	public int GetBuildingCount()
	{
		return _buildings.Count;
	}
	
	public int GetBuildingCountOfType<T>() where T : Building
	{
		int count = 0;
		foreach (var building in _buildings)
		{
			if (building is T)
				count++;
		}
		return count;
	}
}
