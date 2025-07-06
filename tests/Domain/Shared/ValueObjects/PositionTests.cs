using System;
using FluentAssertions;
using Game.Domain.Shared.ValueObjects;
using Xunit;

namespace Game.Tests.Domain.Shared.ValueObjects;

public class PositionTests
{
    [Fact]
    public void Constructor_WithValidCoordinates_ShouldCreatePosition()
    {
        var x = 10.5f;
        var y = 20.3f;

        var position = new Position(x, y);

        position.X.Should().Be(x);
        position.Y.Should().Be(y);
    }

    [Theory]
    [InlineData(0.0f, 0.0f)]
    [InlineData(-10.5f, 15.2f)]
    [InlineData(100.0f, -50.0f)]
    [InlineData(float.MaxValue, float.MinValue)]
    public void Constructor_WithVariousCoordinates_ShouldCreatePosition(float x, float y)
    {
        var position = new Position(x, y);

        position.X.Should().Be(x);
        position.Y.Should().Be(y);
    }

    [Theory]
    [InlineData(0.0f, 0.0f, 0.0f, 0.0f, 0.0f)]
    [InlineData(0.0f, 0.0f, 3.0f, 4.0f, 5.0f)]  // 3-4-5 triangle
    [InlineData(0.0f, 0.0f, 1.0f, 1.0f, 1.414f)]  // √2 ≈ 1.414
    [InlineData(5.0f, 5.0f, 5.0f, 5.0f, 0.0f)]
    [InlineData(-3.0f, -4.0f, 0.0f, 0.0f, 5.0f)]
    public void DistanceTo_ShouldCalculateCorrectDistance(float x1, float y1, float x2, float y2, float expectedDistance)
    {
        var position1 = new Position(x1, y1);
        var position2 = new Position(x2, y2);

        var distance = position1.DistanceTo(position2);

        distance.Should().BeApproximately(expectedDistance, 0.01f);
    }

    [Fact]
    public void DistanceTo_IsSymmetric_ShouldReturnSameDistance()
    {
        var position1 = new Position(10.0f, 20.0f);
        var position2 = new Position(30.0f, 40.0f);

        var distance1To2 = position1.DistanceTo(position2);
        var distance2To1 = position2.DistanceTo(position1);

        distance1To2.Should().BeApproximately(distance2To1, 0.001f);
    }

    [Fact]
    public void ToString_ShouldFormatCorrectly()
    {
        var position = new Position(10.5f, -20.75f);

        var result = position.ToString();

        result.Should().Be("(10.5, -20.8)");  // Rounded to 1 decimal place
    }

    [Theory]
    [InlineData(0.0f, 0.0f)]
    [InlineData(100.5f, -50.25f)]
    [InlineData(-999.9f, 999.9f)]
    public void ToString_WithVariousValues_ShouldFormatCorrectly(float x, float y)
    {
        var position = new Position(x, y);

        var result = position.ToString();

        result.Should().StartWith("(").And.EndWith(")").And.Contain(", ");
    }

    [Fact]
    public void Equals_WithIdenticalPositions_ShouldReturnTrue()
    {
        var position1 = new Position(10.0f, 20.0f);
        var position2 = new Position(10.0f, 20.0f);

        position1.Equals(position2).Should().BeTrue();
        (position1 == position2).Should().BeTrue();
        (position1 != position2).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithSlightlyDifferentPositions_ShouldReturnTrue()
    {
        var position1 = new Position(10.0f, 20.0f);
        var position2 = new Position(10.0005f, 20.0005f);  // Within tolerance

        position1.Equals(position2).Should().BeTrue();
        (position1 == position2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithSignificantlyDifferentPositions_ShouldReturnFalse()
    {
        var position1 = new Position(10.0f, 20.0f);
        var position2 = new Position(10.1f, 20.0f);  // Outside tolerance

        position1.Equals(position2).Should().BeFalse();
        (position1 == position2).Should().BeFalse();
        (position1 != position2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentXCoordinate_ShouldReturnFalse()
    {
        var position1 = new Position(10.0f, 20.0f);
        var position2 = new Position(15.0f, 20.0f);

        position1.Equals(position2).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentYCoordinate_ShouldReturnFalse()
    {
        var position1 = new Position(10.0f, 20.0f);
        var position2 = new Position(10.0f, 25.0f);

        position1.Equals(position2).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_WithIdenticalPositions_ShouldReturnSameHashCode()
    {
        var position1 = new Position(10.0f, 20.0f);
        var position2 = new Position(10.0f, 20.0f);

        position1.GetHashCode().Should().Be(position2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentPositions_ShouldReturnDifferentHashCodes()
    {
        var position1 = new Position(10.0f, 20.0f);
        var position2 = new Position(30.0f, 40.0f);

        position1.GetHashCode().Should().NotBe(position2.GetHashCode());
    }

    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        var position = new Position(10.0f, 20.0f);

        position.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentType_ShouldReturnFalse()
    {
        var position = new Position(10.0f, 20.0f);

        position.Equals("not a position").Should().BeFalse();
    }

    [Theory]
    [InlineData(float.NaN, 0.0f)]
    [InlineData(0.0f, float.NaN)]
    [InlineData(float.NaN, float.NaN)]
    public void Constructor_WithNaNValues_ShouldAcceptValues(float x, float y)
    {
        var position = new Position(x, y);

        position.X.Should().Be(x);
        position.Y.Should().Be(y);
    }

    [Theory]
    [InlineData(float.PositiveInfinity, 0.0f)]
    [InlineData(0.0f, float.NegativeInfinity)]
    [InlineData(float.PositiveInfinity, float.NegativeInfinity)]
    public void Constructor_WithInfinityValues_ShouldAcceptValues(float x, float y)
    {
        var position = new Position(x, y);

        position.X.Should().Be(x);
        position.Y.Should().Be(y);
    }

    [Fact]
    public void DistanceTo_WithNaNCoordinates_ShouldReturnNaN()
    {
        var position1 = new Position(float.NaN, 0.0f);
        var position2 = new Position(0.0f, 0.0f);

        var distance = position1.DistanceTo(position2);

        distance.Should().Be(float.NaN);
    }

    [Fact]
    public void DistanceTo_WithInfinityCoordinates_ShouldReturnInfinity()
    {
        var position1 = new Position(float.PositiveInfinity, 0.0f);
        var position2 = new Position(0.0f, 0.0f);

        var distance = position1.DistanceTo(position2);

        distance.Should().Be(float.PositiveInfinity);
    }
}
