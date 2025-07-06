using FluentAssertions;
using Game.Presentation.Buildings;
using Godot;
using Xunit;

namespace Game.Tests.Presentation.Buildings;

public class BuildingRegistryTests
{
    [Fact] 
    public void Instance_ShouldReturnSameInstance()
    {
        var instance1 = BuildingRegistry.Instance;
        var instance2 = BuildingRegistry.Instance;
        
        instance1.Should().BeSameAs(instance2);
    }

    [Fact]
    public void DefaultCollisionRadius_ShouldBe32()
    {
        BuildingRegistry.DefaultCollisionRadius.Should().Be(32.0f);
    }

    [Fact]
    public void GetAllBuildings_WhenEmpty_ShouldReturnEmptyCollection()
    {
        BuildingRegistry.ResetInstance();
        var registry = BuildingRegistry.Instance;
        
        var buildings = registry.GetAllBuildings();
        
        buildings.Should().BeEmpty();
    }

    [Fact]
    public void IsPositionOccupied_WhenNoBuildingsRegistered_ShouldReturnFalse()
    {
        BuildingRegistry.ResetInstance();
        var registry = BuildingRegistry.Instance;
        
        var isOccupied = registry.IsPositionOccupied(new Vector2(100, 100), 32.0f);
        
        isOccupied.Should().BeFalse();
    }

    [Fact]
    public void GetBuildingAt_WhenNoBuildingsRegistered_ShouldReturnNull()
    {
        BuildingRegistry.ResetInstance();
        var registry = BuildingRegistry.Instance;
        
        var building = registry.GetBuildingAt(new Vector2(100, 100), 32.0f);
        
        building.Should().BeNull();
    }

    [Fact]
    public void ClearAllBuildings_ShouldRemoveAllBuildings()
    {
        BuildingRegistry.ResetInstance();
        var registry = BuildingRegistry.Instance;
        
        registry.ClearAllBuildings();
        
        registry.GetAllBuildings().Should().BeEmpty();
    }

    [Fact]
    public void GetDebugInfo_ShouldReturnCorrectFormat()
    {
        BuildingRegistry.ResetInstance();
        var registry = BuildingRegistry.Instance;
        
        var debugInfo = registry.GetDebugInfo();
        
        debugInfo.Should().Be("BuildingRegistry: 0 buildings registered");
    }

    [Fact]
    public void RegisterBuilding_WithNullBuilding_ShouldNotThrow()
    {
        BuildingRegistry.ResetInstance();
        var registry = BuildingRegistry.Instance;
        
        var action = () => registry.RegisterBuilding(null);
        
        action.Should().NotThrow();
    }

    [Fact]
    public void UnregisterBuilding_WithNullBuilding_ShouldNotThrow()
    {
        BuildingRegistry.ResetInstance();
        var registry = BuildingRegistry.Instance;
        
        var action = () => registry.UnregisterBuilding(null);
        
        action.Should().NotThrow();
    }
}
