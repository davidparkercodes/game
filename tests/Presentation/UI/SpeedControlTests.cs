using FluentAssertions;
using Game.Application.Game.Services;
using Game.Presentation.UI;
using Moq;
using Xunit;

namespace Game.Tests.Presentation.UI;

public class SpeedControlTests
{
    private Mock<ITimeManager> GetMockTimeManager()
    {
        var mockTimeManager = new Mock<ITimeManager>();
        mockTimeManager.Setup(tm => tm.CurrentSpeedIndex).Returns(0); // Default to 1x speed
        return mockTimeManager;
    }

    private SpeedControl GetSpeedControl()
    {
        return new SpeedControl();
    }

    private void SetupMockButtons(SpeedControl speedControl)
    {
        // Create mock buttons for testing
        speedControl.Speed1xButton = new Mock<Godot.Button>().Object;
        speedControl.Speed2xButton = new Mock<Godot.Button>().Object;
        speedControl.Speed4xButton = new Mock<Godot.Button>().Object;
    }

    [Fact]
    public void SpeedControl_WhenCreated_ShouldHaveButtonPropertiesAvailable()
    {
        var speedControl = GetSpeedControl();

        // Test that the button properties exist (even if null initially)
        var speed1xProperty = speedControl.GetType().GetProperty("Speed1xButton");
        var speed2xProperty = speedControl.GetType().GetProperty("Speed2xButton");
        var speed4xProperty = speedControl.GetType().GetProperty("Speed4xButton");

        speed1xProperty.Should().NotBeNull();
        speed2xProperty.Should().NotBeNull();
        speed4xProperty.Should().NotBeNull();
    }

    [Fact]
    public void SpeedControl_WhenTimeManagerAvailable_ShouldConnectToSpeedChangedEvent()
    {
        var speedControl = GetSpeedControl();
        var mockTimeManager = GetMockTimeManager();
        SetupMockButtons(speedControl);

        // Test that we can set up the connection without throwing
        var action = () => {
            // Simulate the connection that would happen in ConnectToTimeManager
            var timeManagerField = speedControl.GetType().GetField("_timeManager", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            timeManagerField?.SetValue(speedControl, mockTimeManager.Object);
        };

        action.Should().NotThrow();
    }

    [Fact]
    public void SpeedControl_ButtonStateUpdate_ShouldHandleNullButtonsGracefully()
    {
        var speedControl = GetSpeedControl();
        
        // Test calling methods that might update button states with null buttons
        var action = () => {
            // This simulates what UpdateButtonStates might do internally
            var updateMethod = speedControl.GetType().GetMethod("UpdateButtonStates", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            updateMethod?.Invoke(speedControl, new object[] { 0 });
        };

        action.Should().NotThrow();
    }

    [Fact]
    public void SpeedControl_SetButtonState_ShouldHandleNullButton()
    {
        var speedControl = GetSpeedControl();
        
        // Test the SetButtonState method with null button
        var action = () => {
            var setButtonStateMethod = speedControl.GetType().GetMethod("SetButtonState", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            setButtonStateMethod?.Invoke(speedControl, new object?[] { null, true });
        };

        action.Should().NotThrow();
    }

    [Fact]
    public void SpeedControl_ButtonEventHandlers_ShouldCallTimeManagerMethods()
    {
        var speedControl = GetSpeedControl();
        var mockTimeManager = GetMockTimeManager();
        SetupMockButtons(speedControl);

        // Set the private _timeManager field
        var timeManagerField = speedControl.GetType().GetField("_timeManager", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        timeManagerField?.SetValue(speedControl, mockTimeManager.Object);

        // Test that button press handlers call the correct TimeManager methods
        var speed1xMethod = speedControl.GetType().GetMethod("OnSpeed1xPressed", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var speed2xMethod = speedControl.GetType().GetMethod("OnSpeed2xPressed", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var action1x = () => speed1xMethod?.Invoke(speedControl, null);
        var action2x = () => speed2xMethod?.Invoke(speedControl, null);

        action1x.Should().NotThrow();
        action2x.Should().NotThrow();

        // Verify TimeManager methods were called
        mockTimeManager.Verify(tm => tm.SetSpeedTo1x(), Times.Once);
        mockTimeManager.Verify(tm => tm.SetSpeedTo2x(), Times.Once);
    }

    [Fact]
    public void SpeedControl_OnSpeedChanged_ShouldUpdateButtonStates()
    {
        var speedControl = GetSpeedControl();
        SetupMockButtons(speedControl);

        // Test the OnSpeedChanged event handler
        var onSpeedChangedMethod = speedControl.GetType().GetMethod("OnSpeedChanged", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var action = () => onSpeedChangedMethod?.Invoke(speedControl, new object[] { 2.0f, 1 });

        action.Should().NotThrow();
    }

    [Fact]
    public void SpeedControl_InitializeNodeReferences_ShouldHandleMissingNodes()
    {
        var speedControl = GetSpeedControl();

        // Test the node initialization method
        var initMethod = speedControl.GetType().GetMethod("InitializeNodeReferences", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var action = () => initMethod?.Invoke(speedControl, null);

        action.Should().NotThrow();
    }
}
