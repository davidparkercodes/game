using FluentAssertions;
using Game.Presentation.Systems;
using System.Reflection;
using Xunit;

namespace Game.Tests.Presentation.Systems;

public class BuildingZoneValidatorTests
{
    [Fact]
    public void BuildingZoneValidator_ShouldBeStaticClass()
    {
        var validatorType = typeof(BuildingZoneValidator);

        validatorType.Should().NotBeNull();
        validatorType.IsClass.Should().BeTrue();
        validatorType.IsAbstract.Should().BeTrue();
        validatorType.IsSealed.Should().BeTrue();
    }

    [Fact]
    public void BuildingZoneValidator_ShouldHaveRequiredStaticMethods()
    {
        var validatorType = typeof(BuildingZoneValidator);

        var initializeMethod = validatorType.GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static);
        var canBuildAtMethod = validatorType.GetMethod("CanBuildAt", BindingFlags.Public | BindingFlags.Static);
        var isOnPathMethod = validatorType.GetMethod("IsOnPath", BindingFlags.Public | BindingFlags.Static);
        var canBuildAtWithLoggingMethod = validatorType.GetMethod("CanBuildAtWithLogging", BindingFlags.Public | BindingFlags.Static);

        initializeMethod.Should().NotBeNull();
        canBuildAtMethod.Should().NotBeNull();
        isOnPathMethod.Should().NotBeNull();
        canBuildAtWithLoggingMethod.Should().NotBeNull();
    }

    [Fact]
    public void BuildingZoneValidator_ShouldHaveIsInitializedProperty()
    {
        var validatorType = typeof(BuildingZoneValidator);

        var isInitializedProperty = validatorType.GetProperty("IsInitialized", BindingFlags.Public | BindingFlags.Static);

        isInitializedProperty.Should().NotBeNull();
        isInitializedProperty!.PropertyType.Should().Be(typeof(bool));
        isInitializedProperty.CanRead.Should().BeTrue();
    }

    [Fact]
    public void BuildingZoneValidator_InitializeMethod_ShouldAcceptTileMapLayer()
    {
        var validatorType = typeof(BuildingZoneValidator);
        var initializeMethod = validatorType.GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static);

        var parameters = initializeMethod!.GetParameters();
        parameters.Should().HaveCount(1);
        parameters[0].ParameterType.Name.Should().Be("TileMapLayer");
        parameters[0].Name.Should().Be("groundLayer");
    }

    [Fact]
    public void BuildingZoneValidator_CanBuildAtMethod_ShouldReturnBoolean()
    {
        var validatorType = typeof(BuildingZoneValidator);
        var canBuildAtMethod = validatorType.GetMethod("CanBuildAt", BindingFlags.Public | BindingFlags.Static);

        canBuildAtMethod!.ReturnType.Should().Be(typeof(bool));

        var parameters = canBuildAtMethod.GetParameters();
        parameters.Should().HaveCount(1);
        parameters[0].ParameterType.Name.Should().Be("Vector2");
        parameters[0].Name.Should().Be("worldPosition");
    }

    [Fact]
    public void BuildingZoneValidator_IsOnPathMethod_ShouldReturnBoolean()
    {
        var validatorType = typeof(BuildingZoneValidator);
        var isOnPathMethod = validatorType.GetMethod("IsOnPath", BindingFlags.Public | BindingFlags.Static);

        isOnPathMethod!.ReturnType.Should().Be(typeof(bool));

        var parameters = isOnPathMethod.GetParameters();
        parameters.Should().HaveCount(1);
        parameters[0].ParameterType.Name.Should().Be("Vector2");
        parameters[0].Name.Should().Be("worldPosition");
    }

    [Fact]
    public void BuildingZoneValidator_CanBuildAtWithLoggingMethod_ShouldReturnBoolean()
    {
        var validatorType = typeof(BuildingZoneValidator);
        var canBuildAtWithLoggingMethod = validatorType.GetMethod("CanBuildAtWithLogging", BindingFlags.Public | BindingFlags.Static);

        canBuildAtWithLoggingMethod!.ReturnType.Should().Be(typeof(bool));

        var parameters = canBuildAtWithLoggingMethod.GetParameters();
        parameters.Should().HaveCount(1);
        parameters[0].ParameterType.Name.Should().Be("Vector2");
        parameters[0].Name.Should().Be("worldPosition");
    }

    [Fact]
    public void BuildingZoneValidator_ShouldHavePrivateStaticFields()
    {
        var validatorType = typeof(BuildingZoneValidator);

        var groundLayerField = validatorType.GetField("_groundLayer", BindingFlags.NonPublic | BindingFlags.Static);
        var isInitializedField = validatorType.GetField("_isInitialized", BindingFlags.NonPublic | BindingFlags.Static);

        groundLayerField.Should().NotBeNull();
        isInitializedField.Should().NotBeNull();
        
        groundLayerField!.FieldType.Name.Should().Be("TileMapLayer");
        isInitializedField!.FieldType.Should().Be(typeof(bool));
    }

    [Fact]
    public void BuildingZoneValidator_ShouldHaveCorrectNamespace()
    {
        var validatorType = typeof(BuildingZoneValidator);

        validatorType.Namespace.Should().Be("Game.Presentation.Systems");
    }
}
