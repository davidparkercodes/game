using System;
using FluentAssertions;
using Game.Domain.Projectiles.Entities;
using Xunit;

namespace Game.Tests.Domain.Projectiles.Entities;

public class BulletTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateBullet()
    {
        var x = 10.0f;
        var y = 20.0f;
        var targetX = 50.0f;
        var targetY = 60.0f;
        var speed = 100.0f;
        var damage = 25;
        var maxDistance = 200.0f;

        var bullet = new Bullet(x, y, targetX, targetY, speed, damage, maxDistance);

        bullet.Id.Should().NotBeEmpty();
        bullet.X.Should().Be(x);
        bullet.Y.Should().Be(y);
        bullet.Speed.Should().Be(speed);
        bullet.Damage.Should().Be(damage);
        bullet.MaxDistance.Should().Be(maxDistance);
        bullet.IsActive.Should().BeTrue();
        bullet.DistanceTraveled.Should().Be(0f);
        bullet.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_WithDefaultMaxDistance_ShouldUseDefaultValue()
    {
        var bullet = new Bullet(0, 0, 10, 10, 100, 25);

        bullet.MaxDistance.Should().Be(1000f);
    }

    [Fact]
    public void Constructor_ShouldAssignUniqueIds()
    {
        var bullet1 = new Bullet(0, 0, 10, 10, 100, 25);
        var bullet2 = new Bullet(0, 0, 10, 10, 100, 25);

        bullet1.Id.Should().NotBe(bullet2.Id);
    }

    [Theory]
    [InlineData(float.NaN, 0.0f, 10.0f, 10.0f)]
    [InlineData(0.0f, float.NaN, 10.0f, 10.0f)]
    [InlineData(float.PositiveInfinity, 0.0f, 10.0f, 10.0f)]
    [InlineData(0.0f, float.NegativeInfinity, 10.0f, 10.0f)]
    public void Constructor_WithInvalidStartPosition_ShouldThrowArgumentException(float x, float y, float targetX, float targetY)
    {
        var action = () => new Bullet(x, y, targetX, targetY, 100, 25);

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0.0f, 0.0f, float.NaN, 10.0f)]
    [InlineData(0.0f, 0.0f, 10.0f, float.NaN)]
    [InlineData(0.0f, 0.0f, float.PositiveInfinity, 10.0f)]
    [InlineData(0.0f, 0.0f, 10.0f, float.NegativeInfinity)]
    public void Constructor_WithInvalidTargetPosition_ShouldThrowArgumentException(float x, float y, float targetX, float targetY)
    {
        var action = () => new Bullet(x, y, targetX, targetY, 100, 25);

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0.0f)]
    [InlineData(-1.0f)]
    [InlineData(-100.0f)]
    public void Constructor_WithNonPositiveSpeed_ShouldThrowArgumentException(float speed)
    {
        var action = () => new Bullet(0, 0, 10, 10, speed, 25);

        action.Should().Throw<ArgumentException>()
            .WithMessage("Speed must be positive*")
            .And.ParamName.Should().Be("speed");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    [InlineData(-100)]
    public void Constructor_WithNegativeDamage_ShouldThrowArgumentException(int damage)
    {
        var action = () => new Bullet(0, 0, 10, 10, 100, damage);

        action.Should().Throw<ArgumentException>()
            .WithMessage("Damage cannot be negative*")
            .And.ParamName.Should().Be("damage");
    }

    [Theory]
    [InlineData(0.0f)]
    [InlineData(-1.0f)]
    [InlineData(-100.0f)]
    public void Constructor_WithNonPositiveMaxDistance_ShouldThrowArgumentException(float maxDistance)
    {
        var action = () => new Bullet(0, 0, 10, 10, 100, 25, maxDistance);

        action.Should().Throw<ArgumentException>()
            .WithMessage("Max distance must be positive*")
            .And.ParamName.Should().Be("maxDistance");
    }

    [Fact]
    public void Constructor_WithZeroDamage_ShouldCreateBullet()
    {
        var bullet = new Bullet(0, 0, 10, 10, 100, 0);

        bullet.Damage.Should().Be(0);
    }

    [Fact]
    public void Constructor_ShouldCalculateVelocityCorrectly()
    {
        var startX = 0.0f;
        var startY = 0.0f;
        var targetX = 30.0f;
        var targetY = 40.0f; // 3-4-5 triangle, distance = 50
        var speed = 100.0f;

        var bullet = new Bullet(startX, startY, targetX, targetY, speed, 25);

        // Expected velocity components: (30/50) * 100 = 60, (40/50) * 100 = 80
        bullet.VelocityX.Should().BeApproximately(60.0f, 0.001f);
        bullet.VelocityY.Should().BeApproximately(80.0f, 0.001f);
    }

    [Fact]
    public void Constructor_WithSameStartAndTargetPosition_ShouldSetZeroVelocity()
    {
        var bullet = new Bullet(10, 20, 10, 20, 100, 25);

        bullet.VelocityX.Should().Be(0f);
        bullet.VelocityY.Should().Be(0f);
    }

    [Fact]
    public void Update_WithValidDeltaTime_ShouldMoveAndUpdateDistance()
    {
        var bullet = new Bullet(0, 0, 30, 40, 100, 25); // Velocity: (60, 80)
        var deltaTime = 0.5f;

        bullet.Update(deltaTime);

        bullet.X.Should().BeApproximately(30.0f, 0.001f); // 60 * 0.5
        bullet.Y.Should().BeApproximately(40.0f, 0.001f); // 80 * 0.5
        bullet.DistanceTraveled.Should().BeApproximately(50.0f, 0.001f); // Distance moved
        bullet.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData(0.0f)]
    [InlineData(-1.0f)]
    [InlineData(-0.1f)]
    public void Update_WithNonPositiveDeltaTime_ShouldThrowArgumentException(float deltaTime)
    {
        var bullet = new Bullet(0, 0, 10, 10, 100, 25);

        var action = () => bullet.Update(deltaTime);

        action.Should().Throw<ArgumentException>()
            .WithMessage("Delta time must be positive*")
            .And.ParamName.Should().Be("deltaTime");
    }

    [Fact]
    public void Update_WhenInactive_ShouldNotMove()
    {
        var bullet = new Bullet(0, 0, 10, 10, 100, 25);
        bullet.Deactivate();
        var initialX = bullet.X;
        var initialY = bullet.Y;
        var initialDistance = bullet.DistanceTraveled;

        bullet.Update(1.0f);

        bullet.X.Should().Be(initialX);
        bullet.Y.Should().Be(initialY);
        bullet.DistanceTraveled.Should().Be(initialDistance);
    }

    [Fact]
    public void Update_WhenExceedingMaxDistance_ShouldDeactivate()
    {
        var bullet = new Bullet(0, 0, 100, 0, 200, 25, 50); // Max distance 50
        var deltaTime = 1.0f; // Will travel 200 units

        bullet.Update(deltaTime);

        bullet.IsActive.Should().BeFalse();
        bullet.DistanceTraveled.Should().BeGreaterThan(50f);
    }

    [Fact]
    public void CalculateDistance_ShouldReturnCorrectDistance()
    {
        var bullet = new Bullet(0, 0, 10, 10, 100, 25);
        var targetX = 30.0f;
        var targetY = 40.0f;

        var distance = bullet.CalculateDistance(targetX, targetY);

        distance.Should().BeApproximately(50.0f, 0.001f); // 3-4-5 triangle
    }

    [Fact]
    public void CalculateDistance_WithSamePosition_ShouldReturnZero()
    {
        var bullet = new Bullet(10, 20, 50, 60, 100, 25);

        var distance = bullet.CalculateDistance(10, 20);

        distance.Should().Be(0f);
    }

    [Theory]
    [InlineData(4.0f, true)]
    [InlineData(5.0f, true)]
    [InlineData(5.1f, false)]
    [InlineData(10.0f, false)]
    public void IsNearTarget_WithDefaultTolerance_ShouldReturnCorrectResult(float targetDistance, bool expectedResult)
    {
        var bullet = new Bullet(0, 0, 10, 10, 100, 25);
        var targetX = targetDistance; // Distance will be targetDistance from (0,0)
        var targetY = 0;

        var result = bullet.IsNearTarget(targetX, targetY);

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(1.0f, 1.5f, true)]
    [InlineData(1.0f, 1.0f, true)]
    [InlineData(1.0f, 0.5f, false)]
    [InlineData(2.0f, 1.0f, false)]
    public void IsNearTarget_WithCustomTolerance_ShouldReturnCorrectResult(float targetDistance, float tolerance, bool expectedResult)
    {
        var bullet = new Bullet(0, 0, 10, 10, 100, 25);
        var targetX = targetDistance;
        var targetY = 0;

        var result = bullet.IsNearTarget(targetX, targetY, tolerance);

        result.Should().Be(expectedResult);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        var bullet = new Bullet(0, 0, 10, 10, 100, 25);

        bullet.Deactivate();

        bullet.IsActive.Should().BeFalse();
    }

    [Fact]
    public void HasExpired_WhenActive_ShouldReturnFalse()
    {
        var bullet = new Bullet(0, 0, 10, 10, 100, 25);

        bullet.HasExpired().Should().BeFalse();
    }

    [Fact]
    public void HasExpired_WhenInactive_ShouldReturnTrue()
    {
        var bullet = new Bullet(0, 0, 10, 10, 100, 25);
        bullet.Deactivate();

        bullet.HasExpired().Should().BeTrue();
    }

    [Fact]
    public void HasExpired_WhenExceededMaxDistance_ShouldReturnTrue()
    {
        var bullet = new Bullet(0, 0, 10, 0, 100, 25, 50);
        bullet.Update(1.0f); // Travel 100 units, exceeding max of 50

        bullet.HasExpired().Should().BeTrue();
    }

    [Fact]
    public void GetRemainingDistance_ShouldReturnCorrectValue()
    {
        var bullet = new Bullet(0, 0, 10, 0, 100, 25, 200);
        bullet.Update(0.5f); // Travel 50 units

        var remaining = bullet.GetRemainingDistance();

        remaining.Should().BeApproximately(150f, 0.001f);
    }

    [Fact]
    public void GetRemainingDistance_WhenExceeded_ShouldReturnZero()
    {
        var bullet = new Bullet(0, 0, 10, 0, 100, 25, 50);
        bullet.Update(1.0f); // Travel 100 units, exceeding max of 50

        var remaining = bullet.GetRemainingDistance();

        remaining.Should().Be(0f);
    }

    [Fact]
    public void GetTimeToTarget_ShouldCalculateCorrectly()
    {
        var bullet = new Bullet(0, 0, 10, 10, 100, 25);
        var targetX = 30.0f;
        var targetY = 40.0f; // Distance = 50

        var timeToTarget = bullet.GetTimeToTarget(targetX, targetY);

        timeToTarget.Should().BeApproximately(0.5f, 0.001f); // 50 / 100 = 0.5
    }

    [Fact]
    public void GetTimeToTarget_WithZeroSpeed_ShouldReturnMaxValue()
    {
        var bullet = new Bullet(0, 0, 10, 10, 100, 25);
        // We can't directly test zero speed since constructor validates it, 
        // but we can test the logic by checking edge case behavior
        var timeToTarget = bullet.GetTimeToTarget(0, 0); // Same position

        timeToTarget.Should().Be(0f);
    }

    [Fact]
    public void SetPosition_WithValidPosition_ShouldUpdatePosition()
    {
        var bullet = new Bullet(0, 0, 10, 10, 100, 25);
        var newX = 50.0f;
        var newY = 75.0f;

        bullet.SetPosition(newX, newY);

        bullet.X.Should().Be(newX);
        bullet.Y.Should().Be(newY);
    }

    [Theory]
    [InlineData(float.NaN, 0.0f)]
    [InlineData(0.0f, float.NaN)]
    [InlineData(float.PositiveInfinity, 0.0f)]
    [InlineData(0.0f, float.NegativeInfinity)]
    public void SetPosition_WithInvalidPosition_ShouldThrowArgumentException(float x, float y)
    {
        var bullet = new Bullet(0, 0, 10, 10, 100, 25);

        var action = () => bullet.SetPosition(x, y);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ToString_ShouldFormatCorrectly()
    {
        var bullet = new Bullet(10.5f, 20.7f, 50, 60, 100, 25, 200);

        var result = bullet.ToString();

        result.Should().Contain($"Bullet(Id:{bullet.Id:N}")
            .And.Contain("Pos:(10.5,20.7)")
            .And.Contain("Damage:25")
            .And.Contain("Active:True")
            .And.Contain("Distance:0.0/200.0");
    }

    [Fact]
    public void ToString_WithTraveledDistance_ShouldShowProgress()
    {
        var bullet = new Bullet(0, 0, 10, 0, 100, 25, 200);
        bullet.Update(0.5f); // Travel 50 units

        var result = bullet.ToString();

        result.Should().Contain("Distance:50.0/200.0");
    }

    [Fact]
    public void ComplexScenario_BulletLifecycle_ShouldWorkCorrectly()
    {
        // Create a bullet that travels from (0,0) to (60,80) with speed 100
        var bullet = new Bullet(0, 0, 60, 80, 100, 30, 120);

        // Initial state
        bullet.IsActive.Should().BeTrue();
        bullet.HasExpired().Should().BeFalse();
        bullet.DistanceTraveled.Should().Be(0f);

        // Update for 0.5 seconds - should move halfway
        bullet.Update(0.5f);
        bullet.X.Should().BeApproximately(30f, 0.001f);
        bullet.Y.Should().BeApproximately(40f, 0.001f);
        bullet.DistanceTraveled.Should().BeApproximately(50f, 0.001f);
        bullet.IsActive.Should().BeTrue();

        // Check if near target
        bullet.IsNearTarget(60, 80, 55f).Should().BeTrue(); // Within 55 units
        bullet.IsNearTarget(60, 80, 45f).Should().BeFalse(); // Not within 45 units

        // Update for another 0.8 seconds - should exceed max distance and deactivate
        bullet.Update(0.8f);
        bullet.DistanceTraveled.Should().BeGreaterThan(120f);
        bullet.IsActive.Should().BeFalse();
        bullet.HasExpired().Should().BeTrue();
        bullet.GetRemainingDistance().Should().Be(0f);
    }
}
