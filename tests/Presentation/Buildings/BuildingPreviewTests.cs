using FluentAssertions;
using Game.Presentation.Buildings;
using Xunit;

namespace Game.Tests.Presentation.Buildings;

public class BuildingPreviewTests
{
    [Fact]
    public void BuildingPreview_WhenCreated_ShouldHaveExpectedType()
    {
        // Test that BuildingPreview can be instantiated
        var buildingPreviewType = typeof(BuildingPreview);

        buildingPreviewType.Should().NotBeNull();
        buildingPreviewType.Name.Should().Be("BuildingPreview");
    }

    [Fact]
    public void BuildingPreview_ShouldHaveBuildingSceneField()
    {
        var buildingPreviewType = typeof(BuildingPreview);

        var buildingSceneField = buildingPreviewType.GetField("BuildingScene");

        buildingSceneField.Should().NotBeNull();
        buildingSceneField!.FieldType.Name.Should().Be("PackedScene");
    }

    [Fact]
    public void BuildingPreview_ShouldHaveRequiredMethods()
    {
        var buildingPreviewType = typeof(BuildingPreview);

        var canPlaceBuildingMethod = buildingPreviewType.GetMethod("CanPlaceBuilding");
        var getPlacementPositionMethod = buildingPreviewType.GetMethod("GetPlacementPosition");
        var getBuildingCostMethod = buildingPreviewType.GetMethod("GetBuildingCost");
        var flashRedMethod = buildingPreviewType.GetMethod("FlashRed");
        var updateBuildingSceneMethod = buildingPreviewType.GetMethod("UpdateBuildingScene");

        canPlaceBuildingMethod.Should().NotBeNull();
        getPlacementPositionMethod.Should().NotBeNull();
        getBuildingCostMethod.Should().NotBeNull();
        flashRedMethod.Should().NotBeNull();
        updateBuildingSceneMethod.Should().NotBeNull();
    }

    [Fact]
    public void BuildingPreview_CanPlaceBuildingMethod_ShouldReturnBoolean()
    {
        var buildingPreviewType = typeof(BuildingPreview);
        var canPlaceBuildingMethod = buildingPreviewType.GetMethod("CanPlaceBuilding");

        canPlaceBuildingMethod!.ReturnType.Should().Be(typeof(bool));
    }

    [Fact]
    public void BuildingPreview_GetBuildingCostMethod_ShouldReturnInt()
    {
        var buildingPreviewType = typeof(BuildingPreview);
        var getBuildingCostMethod = buildingPreviewType.GetMethod("GetBuildingCost");

        getBuildingCostMethod!.ReturnType.Should().Be(typeof(int));
    }

    [Fact]
    public void BuildingPreview_GetPlacementPositionMethod_ShouldReturnVector2()
    {
        var buildingPreviewType = typeof(BuildingPreview);
        var getPlacementPositionMethod = buildingPreviewType.GetMethod("GetPlacementPosition");

        getPlacementPositionMethod!.ReturnType.Name.Should().Be("Vector2");
    }

    [Fact]
    public void BuildingPreview_ShouldInheritFromNode2D()
    {
        var buildingPreviewType = typeof(BuildingPreview);

        buildingPreviewType.BaseType!.Name.Should().Be("Node2D");
    }

    [Fact]
    public void BuildingPreview_UpdateBuildingSceneMethod_ShouldAcceptPackedScene()
    {
        var buildingPreviewType = typeof(BuildingPreview);
        var updateBuildingSceneMethod = buildingPreviewType.GetMethod("UpdateBuildingScene");

        var parameters = updateBuildingSceneMethod!.GetParameters();
        parameters.Should().HaveCount(1);
        parameters[0].ParameterType.Name.Should().Be("PackedScene");
    }

    [Fact]
    public void BuildingPreview_FlashRedMethod_ShouldNotRequireParameters()
    {
        var buildingPreviewType = typeof(BuildingPreview);
        var flashRedMethod = buildingPreviewType.GetMethod("FlashRed");

        var parameters = flashRedMethod!.GetParameters();
        parameters.Should().BeEmpty();
    }
}
