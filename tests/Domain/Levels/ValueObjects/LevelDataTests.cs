using System;
using Xunit;
using FluentAssertions;
using Game.Domain.Levels.ValueObjects;
using DomainLevelData = Game.Domain.Levels.ValueObjects.LevelData;

namespace Game.Tests.Domain.Levels.ValueObjects;

public class LevelDataTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateLevelData()
    {
        // Arrange
        var pathPoints = new[]
        {
            new PathPoint(0, 0),
            new PathPoint(100, 100),
            new PathPoint(200, 0)
        };

        // Act
        var config = new DomainLevelData(
            "Test Level",
            "A test level",
            pathPoints,
            64.0f,
            new PathPoint(0, 0),
            new PathPoint(200, 0),
            150,
            25
        );

        // Assert
        config.LevelName.Should().Be("Test Level");
        config.Description.Should().Be("A test level");
        config.PathPoints.Should().HaveCount(3);
        config.PathWidth.Should().Be(64.0f);
        config.SpawnPoint.Should().Be(new PathPoint(0, 0));
        config.EndPoint.Should().Be(new PathPoint(200, 0));
        config.InitialMoney.Should().Be(150);
        config.InitialLives.Should().Be(25);
    }

    [Theory]
    [InlineData(null, "desc", 64.0f, 100, 20)]     // Null level name
    [InlineData("", "desc", 64.0f, 100, 20)]       // Empty level name
    [InlineData("   ", "desc", 64.0f, 100, 20)]    // Whitespace level name
    [InlineData("Level", "desc", 0.0f, 100, 20)]   // Zero path width
    [InlineData("Level", "desc", 64.0f, -1, 20)]   // Negative money
    [InlineData("Level", "desc", 64.0f, 100, 0)]   // Zero lives
    public void Constructor_WithInvalidParameters_ShouldThrowArgumentException(
        string levelName, string description, float pathWidth, int initialMoney, int initialLives)
    {
        // Arrange
        var pathPoints = new[] { new PathPoint(0, 0), new PathPoint(100, 100) };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new DomainLevelData(
            levelName,
            description,
            pathPoints,
            pathWidth,
            new PathPoint(0, 0),
            new PathPoint(100, 100),
            initialMoney,
            initialLives
        ));
    }

    [Fact]
    public void Constructor_WithInsufficientPathPoints_ShouldThrowArgumentException()
    {
        // Arrange
        var pathPoints = new[] { new PathPoint(0, 0) }; // Only one point

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new DomainLevelData(
            "Level",
            "desc",
            pathPoints,
            64.0f,
            new PathPoint(0, 0),
            new PathPoint(100, 100),
            100,
            20
        ));
    }

    [Fact]
    public void Constructor_WithNullPathPoints_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new DomainLevelData(
            "Level",
            "desc",
            null,
            64.0f,
            new PathPoint(0, 0),
            new PathPoint(100, 100),
            100,
            20
        ));
    }

    [Fact]
    public void Constructor_WithNullDescription_ShouldUseEmptyString()
    {
        // Arrange
        var pathPoints = new[] { new PathPoint(0, 0), new PathPoint(100, 100) };

        // Act
        var config = new DomainLevelData(
            "Level",
            null,
            pathPoints,
            64.0f,
            new PathPoint(0, 0),
            new PathPoint(100, 100),
            100,
            20
        );

        // Assert
        config.Description.Should().Be(string.Empty);
    }

    [Fact]
    public void CreateDefault_ShouldReturnValidDefaultConfiguration()
    {
        // Act
        var config = DomainLevelData.CreateDefault();

        // Assert
        config.LevelName.Should().Be("Level 1");
        config.Description.Should().NotBeEmpty();
        config.PathPoints.Should().HaveCountGreaterThan(2);
        config.PathWidth.Should().Be(64.0f);
        config.InitialMoney.Should().Be(100);
        config.InitialLives.Should().Be(20);
    }

    [Fact]
    public void PathLength_ShouldCalculateCorrectly()
    {
        // Arrange
        var pathPoints = new[]
        {
            new PathPoint(0, 0),     // Start
            new PathPoint(100, 0),   // 100 units right
            new PathPoint(100, 100)  // 100 units down
        };

        var config = new DomainLevelData(
            "Test",
            "",
            pathPoints,
            64.0f,
            pathPoints[0],
            pathPoints[^1],
            100,
            20
        );

        // Act
        var pathLength = config.PathLength;

        // Assert
        pathLength.Should().Be(200.0f); // 100 + 100
    }

    [Fact]
    public void PathLength_WithSinglePoint_ShouldReturnZero()
    {
        // Arrange - This shouldn't happen with valid constructor, but test the property
        var pathPoints = new[] { new PathPoint(0, 0), new PathPoint(0, 0) };

        var config = new DomainLevelData(
            "Test",
            "",
            pathPoints,
            64.0f,
            pathPoints[0],
            pathPoints[1],
            100,
            20
        );

        // Act
        var pathLength = config.PathLength;

        // Assert
        pathLength.Should().Be(0.0f);
    }

    [Fact]
    public void DifficultyRating_ShouldCalculateBasedOnComplexityAndResources()
    {
        // Arrange
        var simplePathPoints = new[] { new PathPoint(0, 0), new PathPoint(100, 100) };
        var complexPathPoints = new PathPoint[15]; // More complex path
        for (int i = 0; i < 15; i++)
        {
            complexPathPoints[i] = new PathPoint(i * 10, i * 5);
        }

        var easyLevel = new DomainLevelData(
            "Easy",
            "",
            simplePathPoints,
            64.0f,
            simplePathPoints[0],
            simplePathPoints[1],
            1000,
            50
        );

        var hardLevel = new DomainLevelData(
            "Hard",
            "",
            complexPathPoints,
            64.0f,
            complexPathPoints[0],
            complexPathPoints[^1],
            50,
            5
        );

        // Act & Assert
        hardLevel.DifficultyRating.Should().BeGreaterThan(easyLevel.DifficultyRating);
    }

    [Fact]
    public void PathPoints_ShouldBeReadOnly()
    {
        // Arrange
        var pathPoints = new[] { new PathPoint(0, 0), new PathPoint(100, 100) };
        var config = new DomainLevelData(
            "Test",
            "",
            pathPoints,
            64.0f,
            pathPoints[0],
            pathPoints[1],
            100,
            20
        );

        // Act & Assert
        config.PathPoints.Should().BeOfType<System.Collections.ObjectModel.ReadOnlyCollection<PathPoint>>();
    }

    [Fact]
    public void Equals_WithIdenticalConfigurations_ShouldReturnTrue()
    {
        // Arrange
        var pathPoints = new[] { new PathPoint(0, 0), new PathPoint(100, 100) };

        var config1 = new DomainLevelData(
            "Test",
            "desc",
            pathPoints,
            64.0f,
            pathPoints[0],
            pathPoints[1],
            100,
            20
        );

        var config2 = new DomainLevelData(
            "Test",
            "desc",
            pathPoints,
            64.0f,
            pathPoints[0],
            pathPoints[1],
            100,
            20
        );

        // Act & Assert
        config1.Equals(config2).Should().BeTrue();
        (config1 == config2).Should().BeTrue();
        (config1 != config2).Should().BeFalse();
    }

    [Fact]
    public void ToString_ShouldContainRelevantInformation()
    {
        // Arrange
        var pathPoints = new[] { new PathPoint(0, 0), new PathPoint(100, 100), new PathPoint(200, 0) };
        var config = new DomainLevelData(
            "Test Level",
            "",
            pathPoints,
            64.0f,
            pathPoints[0],
            pathPoints[^1],
            150,
            25
        );

        // Act
        var result = config.ToString();

        // Assert
        result.Should().Contain("Test Level");
        result.Should().Contain("3"); // Points count
        result.Should().Contain("150"); // Money
        result.Should().Contain("25"); // Lives
        result.Should().Contain("Difficulty");
    }
}

public class PathPointTests
{
    [Fact]
    public void Constructor_ShouldSetCoordinates()
    {
        // Act
        var point = new PathPoint(123.45f, 678.90f);

        // Assert
        point.X.Should().Be(123.45f);
        point.Y.Should().Be(678.90f);
    }

    [Fact]
    public void DistanceTo_ShouldCalculateCorrectly()
    {
        // Arrange
        var point1 = new PathPoint(0, 0);
        var point2 = new PathPoint(3, 4); // 3-4-5 triangle

        // Act
        var distance = point1.DistanceTo(point2);

        // Assert
        distance.Should().Be(5.0f);
    }

    [Fact]
    public void DistanceTo_WithSamePoint_ShouldReturnZero()
    {
        // Arrange
        var point = new PathPoint(100, 200);

        // Act
        var distance = point.DistanceTo(point);

        // Assert
        distance.Should().Be(0.0f);
    }

    [Fact]
    public void Equals_WithIdenticalPoints_ShouldReturnTrue()
    {
        // Arrange
        var point1 = new PathPoint(123.45f, 678.90f);
        var point2 = new PathPoint(123.45f, 678.90f);

        // Act & Assert
        point1.Equals(point2).Should().BeTrue();
        (point1 == point2).Should().BeTrue();
        (point1 != point2).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithSlightlyDifferentPoints_ShouldReturnFalse()
    {
        // Arrange
        var point1 = new PathPoint(123.45f, 678.90f);
        var point2 = new PathPoint(123.46f, 678.90f); // Slightly different

        // Act & Assert
        point1.Equals(point2).Should().BeFalse();
        (point1 == point2).Should().BeFalse();
        (point1 != point2).Should().BeTrue();
    }

    [Fact]
    public void ToString_ShouldFormatCoordinates()
    {
        // Arrange
        var point = new PathPoint(123.456f, 789.012f);

        // Act
        var result = point.ToString();

        // Assert
        result.Should().Contain("123.5"); // Rounded to 1 decimal
        result.Should().Contain("789.0"); // Rounded to 1 decimal
    }
}
