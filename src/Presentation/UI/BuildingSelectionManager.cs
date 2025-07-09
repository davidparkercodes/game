using Godot;
using Game.Presentation.Buildings;
using Game.Presentation.UI;

namespace Game.Presentation.UI;

public class BuildingSelectionManager
{
    private static BuildingSelectionManager? _instance;
    public static BuildingSelectionManager Instance => _instance ??= new BuildingSelectionManager();
    
    private Building? _currentlySelectedBuilding = null;
    private BuildingUpgradeHud? _buildingUpgradeHud = null;
    private const string LogPrefix = "🎯 [BUILDING-SELECTION]";
    
    private BuildingSelectionManager()
    {
        GD.Print($"{LogPrefix} Building selection manager initialized");
    }
    
    public void InitializeBuildingUpgradeHud(BuildingUpgradeHud upgradeHud)
    {
        _buildingUpgradeHud = upgradeHud;
        GD.Print($"{LogPrefix} BuildingUpgradeHud connected to BuildingSelectionManager");
    }
    
    public Building? CurrentlySelectedBuilding => _currentlySelectedBuilding;
    
    public void SelectBuilding(Building building)
    {
        if (building == null)
        {
            GD.PrintErr($"{LogPrefix} Cannot select null building");
            return;
        }
        
        GD.Print($"{LogPrefix} SelectBuilding called for {building.Name} at {building.GlobalPosition}");
        
        // If the same building is already selected, deselect it
        if (_currentlySelectedBuilding == building)
        {
            GD.Print($"{LogPrefix} Building {building.Name} is already selected - deselecting");
            DeselectCurrentBuilding();
            return;
        }
        
        // Deselect previous building
        if (_currentlySelectedBuilding != null)
        {
            GD.Print($"{LogPrefix} Deselecting previous building {_currentlySelectedBuilding.Name}");
            _currentlySelectedBuilding.SetSelected(false);
        }
        
        // Select new building
        _currentlySelectedBuilding = building;
        building.SetSelected(true);
        
        // Show BuildingUpgradeHud if available
        if (_buildingUpgradeHud != null)
        {
            GD.Print($"{LogPrefix} Showing BuildingUpgradeHud for {building.Name}");
            _buildingUpgradeHud.ShowForBuilding(building);
        }
        else
        {
            GD.PrintErr($"{LogPrefix} BuildingUpgradeHud is null - cannot show HUD");
        }
        
        GD.Print($"{LogPrefix} Selected building {building.Name} at {building.GlobalPosition}");
    }
    
    public void DeselectCurrentBuilding()
    {
        if (_currentlySelectedBuilding != null)
        {
            _currentlySelectedBuilding.SetSelected(false);
            GD.Print($"{LogPrefix} Deselected building {_currentlySelectedBuilding.Name}");
            _currentlySelectedBuilding = null;
        }
        
        // Hide BuildingUpgradeHud if available
        if (_buildingUpgradeHud != null)
        {
            _buildingUpgradeHud.HideHud();
        }
    }
    
    public void DeselectIfSelected(Building building)
    {
        if (_currentlySelectedBuilding == building)
        {
            DeselectCurrentBuilding();
        }
    }
    
    public bool IsBuildingSelected(Building building)
    {
        return _currentlySelectedBuilding == building;
    }
    
    public bool HasSelectedBuilding()
    {
        return _currentlySelectedBuilding != null;
    }
    
    public bool IsTowerUpgradeHudOpen()
    {
        return _buildingUpgradeHud != null && _buildingUpgradeHud.IsVisible();
    }
    
	public void HandleClickOutside(Vector2 globalPosition)
	{
		GD.Print($"{LogPrefix} HandleClickOutside called at {globalPosition}");
		
		// Check if the click is outside all registered buildings
		var buildings = BuildingRegistry.Instance.GetAllBuildings();
		bool clickedOnBuilding = false;
		
		GD.Print($"{LogPrefix} Checking {buildings.Count} buildings for collision");
		foreach (var building in buildings)
		{
			float distance = building.GlobalPosition.DistanceTo(globalPosition);
			GD.Print($"{LogPrefix} Building {building.Name} at {building.GlobalPosition}, distance: {distance}, radius: {building.CollisionRadius}");
			
			if (distance <= building.CollisionRadius)
			{
				GD.Print($"{LogPrefix} Click is within building {building.Name} - not deselecting");
				clickedOnBuilding = true;
				break;
			}
		}
		
		// If clicked outside any building, deselect current selection
		if (!clickedOnBuilding)
		{
			GD.Print($"{LogPrefix} Click is outside all buildings - deselecting");
			DeselectCurrentBuilding();
		}
		else
		{
			GD.Print($"{LogPrefix} Click is on a building - keeping selection");
		}
	}
    
    public void ClearSelection()
    {
        DeselectCurrentBuilding();
    }
    
    public void OnBuildingDestroyed(Building building)
    {
        // Clean up selection if the destroyed building was selected
        if (_currentlySelectedBuilding == building)
        {
            _currentlySelectedBuilding = null;
        }
    }
    
    /// <summary>
    /// Reset the singleton instance (useful for testing)
    /// </summary>
    public static void ResetInstance()
    {
        _instance?.ClearSelection();
        _instance = null;
    }
}
