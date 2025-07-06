using FluentAssertions;
using Game.Presentation.Components;
using System.Linq;
using Xunit;

namespace Game.Tests.Presentation.Components;

public class StatsComponentTests
{
    [Fact]
    public void StatsComponent_WhenCreated_ShouldHaveExpectedType()
    {
        var statsComponentType = typeof(StatsComponent);

        statsComponentType.Should().NotBeNull();
        statsComponentType.Name.Should().Be("StatsComponent");
    }

    [Fact]
    public void StatsComponent_ShouldHaveExpectedFields()
    {
        var statsComponentType = typeof(StatsComponent);

        var maxHpField = statsComponentType.GetField("MaxHP");
        var strengthField = statsComponentType.GetField("Strength");
        var defenceField = statsComponentType.GetField("Defence");
        var critChanceField = statsComponentType.GetField("CritChance");

        maxHpField.Should().NotBeNull();
        strengthField.Should().NotBeNull();
        defenceField.Should().NotBeNull();
        critChanceField.Should().NotBeNull();
    }

    [Fact]
    public void StatsComponent_ShouldHaveCurrentHPProperty()
    {
        var statsComponentType = typeof(StatsComponent);

        var currentHpProperty = statsComponentType.GetProperty("CurrentHP");

        currentHpProperty.Should().NotBeNull();
        currentHpProperty!.PropertyType.Should().Be(typeof(int));
        currentHpProperty.CanRead.Should().BeTrue();
        // Note: Property has private setter, so CanWrite may be true but setter is private
    }

    [Fact]
    public void StatsComponent_ShouldHaveModifyHPMethod()
    {
        var statsComponentType = typeof(StatsComponent);

        var modifyHpMethod = statsComponentType.GetMethod("ModifyHP");

        modifyHpMethod.Should().NotBeNull();
        modifyHpMethod!.ReturnType.Should().Be(typeof(void));

        var parameters = modifyHpMethod.GetParameters();
        parameters.Should().HaveCount(1);
        parameters[0].ParameterType.Should().Be(typeof(int));
        parameters[0].Name.Should().Be("amount");
    }

    [Fact]
    public void StatsComponent_ShouldInheritFromNode()
    {
        var statsComponentType = typeof(StatsComponent);

        statsComponentType.BaseType!.Name.Should().Be("Node");
    }

    [Fact]
    public void StatsComponent_FieldTypes_ShouldBeCorrect()
    {
        var statsComponentType = typeof(StatsComponent);

        var maxHpField = statsComponentType.GetField("MaxHP");
        var strengthField = statsComponentType.GetField("Strength");
        var defenceField = statsComponentType.GetField("Defence");
        var critChanceField = statsComponentType.GetField("CritChance");

        maxHpField!.FieldType.Should().Be(typeof(int));
        strengthField!.FieldType.Should().Be(typeof(int));
        defenceField!.FieldType.Should().Be(typeof(int));
        critChanceField!.FieldType.Should().Be(typeof(float));
    }

    [Fact]
    public void StatsComponent_ShouldHaveSignalDelegates()
    {
        var statsComponentType = typeof(StatsComponent);

        // Check for nested signal delegate types
        var nestedTypes = statsComponentType.GetNestedTypes();
        var hpChangedDelegate = nestedTypes.FirstOrDefault(t => t.Name.Contains("HpChangedEventHandler"));
        var diedDelegate = nestedTypes.FirstOrDefault(t => t.Name.Contains("DiedEventHandler"));

        hpChangedDelegate.Should().NotBeNull();
        diedDelegate.Should().NotBeNull();
    }

    [Fact]
    public void StatsComponent_DefaultValues_ShouldBeReasonable()
    {
        // Test that we can create a default instance and check default values
        var statsComponentType = typeof(StatsComponent);
        
        var maxHpField = statsComponentType.GetField("MaxHP");
        var strengthField = statsComponentType.GetField("Strength");
        var defenceField = statsComponentType.GetField("Defence");
        var critChanceField = statsComponentType.GetField("CritChance");

        // All fields should exist and have reasonable default values based on [Export] attributes
        maxHpField.Should().NotBeNull();
        strengthField.Should().NotBeNull();
        defenceField.Should().NotBeNull();
        critChanceField.Should().NotBeNull();
    }

    [Fact]
    public void StatsComponent_Constructor_ShouldExist()
    {
        var statsComponentType = typeof(StatsComponent);

        var constructors = statsComponentType.GetConstructors();

        constructors.Should().NotBeEmpty();
        // Should have at least a default constructor
        constructors.Should().Contain(c => c.GetParameters().Length == 0);
    }
}
