using FluentAssertions;
using Game.Application.Game.Services;
using Game.Domain.Common.Services;
using Moq;
using Xunit;

namespace Game.Application.Game.Services.Tests;

public class TimeManagerTests
{
    private readonly Mock<ILogger> _mockLogger;
    private readonly TimeManager _timeManager;

    public TimeManagerTests()
    {
        _mockLogger = new Mock<ILogger>();
        _timeManager = new TimeManager(_mockLogger.Object);
    }

    [Fact]
    public void Constructor_ShouldInitializeWith1xSpeed()
    {
        _timeManager.CurrentTimeScale.Should().Be(1.0f);
        _timeManager.CurrentSpeedIndex.Should().Be(0);
        _timeManager.GetCurrentSpeedText().Should().Be("1x");
    }

    [Theory]
    [InlineData(0, 1.0f, "1x")]
    [InlineData(1, 2.0f, "2x")]
    [InlineData(2, 4.0f, "4x")]
    [InlineData(3, 10.0f, "10x")]
    [InlineData(4, 20.0f, "20x")]
    public void SetGameSpeedByIndex_ShouldSetCorrectSpeed(int speedIndex, float expectedScale, string expectedText)
    {
        _timeManager.SetGameSpeedByIndex(speedIndex);

        _timeManager.CurrentTimeScale.Should().Be(expectedScale);
        _timeManager.CurrentSpeedIndex.Should().Be(speedIndex);
        _timeManager.GetCurrentSpeedText().Should().Be(expectedText);
    }

    [Fact]
    public void SetGameSpeedByIndex_WithInvalidIndex_ShouldDefaultToSpeed1x()
    {
        _timeManager.SetGameSpeedByIndex(-1);

        _timeManager.CurrentTimeScale.Should().Be(1.0f);
        _timeManager.CurrentSpeedIndex.Should().Be(0);
    }

    [Fact]
    public void CycleToNextSpeed_ShouldAdvanceThroughSpeeds()
    {
        _timeManager.CycleToNextSpeed();
        _timeManager.CurrentSpeedIndex.Should().Be(1);
        _timeManager.CurrentTimeScale.Should().Be(2.0f);

        _timeManager.CycleToNextSpeed();
        _timeManager.CurrentSpeedIndex.Should().Be(2);
        _timeManager.CurrentTimeScale.Should().Be(4.0f);
    }

    [Fact]
    public void CycleToNextSpeed_AtMaxSpeed_ShouldWrapToFirstSpeed()
    {
        _timeManager.SetGameSpeedByIndex(4); // Set to 20x (max)
        
        _timeManager.CycleToNextSpeed();
        
        _timeManager.CurrentSpeedIndex.Should().Be(0);
        _timeManager.CurrentTimeScale.Should().Be(1.0f);
    }

    [Fact]
    public void SpeedChanged_WhenSpeedChanges_ShouldTriggerEvent()
    {
        bool eventTriggered = false;
        float eventSpeed = 0;
        int eventIndex = -1;

        _timeManager.SpeedChanged += (speed, index) =>
        {
            eventTriggered = true;
            eventSpeed = speed;
            eventIndex = index;
        };

        _timeManager.SetGameSpeedByIndex(2);

        eventTriggered.Should().BeTrue();
        eventSpeed.Should().Be(4.0f);
        eventIndex.Should().Be(2);
    }
}
