using System;
using FluentAssertions;
using Game.Domain.Items.Entities;
using Xunit;

namespace Game.Tests.Domain.Items.Entities;

public class LootablePickupTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateLootablePickup()
    {
        var type = ItemType.Gold;
        var value = 50;
        var x = 100.0f;
        var y = 200.0f;
        var pickupRadius = 25.0f;
        var expiration = TimeSpan.FromSeconds(30);

        var pickup = new LootablePickup(type, value, x, y, pickupRadius, expiration);

        pickup.Id.Should().NotBeEmpty();
        pickup.Type.Should().Be(type);
        pickup.Value.Should().Be(value);
        pickup.X.Should().Be(x);
        pickup.Y.Should().Be(y);
        pickup.PickupRadius.Should().Be(pickupRadius);
        pickup.IsActive.Should().BeTrue();
        pickup.IsCollected.Should().BeFalse();
        pickup.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        pickup.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.Add(expiration), TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_WithDefaultParameters_ShouldUseDefaults()
    {
        var pickup = new LootablePickup(ItemType.Experience, 25, 50, 75);

        pickup.PickupRadius.Should().Be(20.0f);
        pickup.ExpiresAt.Should().BeNull();
    }

    [Fact]
    public void Constructor_ShouldAssignUniqueIds()
    {
        var pickup1 = new LootablePickup(ItemType.Gold, 10, 0, 0);
        var pickup2 = new LootablePickup(ItemType.Gold, 10, 0, 0);

        pickup1.Id.Should().NotBe(pickup2.Id);
    }

    [Theory]
    [InlineData(float.NaN, 0.0f)]
    [InlineData(0.0f, float.NaN)]
    [InlineData(float.PositiveInfinity, 0.0f)]
    [InlineData(0.0f, float.NegativeInfinity)]
    public void Constructor_WithInvalidPosition_ShouldThrowArgumentException(float x, float y)
    {
        var action = () => new LootablePickup(ItemType.Gold, 10, x, y);

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    [InlineData(-100)]
    public void Constructor_WithNegativeValue_ShouldThrowArgumentException(int value)
    {
        var action = () => new LootablePickup(ItemType.Gold, value, 0, 0);

        action.Should().Throw<ArgumentException>()
            .WithMessage("Value cannot be negative*")
            .And.ParamName.Should().Be("value");
    }

    [Theory]
    [InlineData(0.0f)]
    [InlineData(-1.0f)]
    [InlineData(-10.0f)]
    public void Constructor_WithNonPositivePickupRadius_ShouldThrowArgumentException(float pickupRadius)
    {
        var action = () => new LootablePickup(ItemType.Gold, 10, 0, 0, pickupRadius);

        action.Should().Throw<ArgumentException>()
            .WithMessage("Pickup radius must be positive*")
            .And.ParamName.Should().Be("pickupRadius");
    }

    [Fact]
    public void Constructor_WithZeroValue_ShouldCreatePickup()
    {
        var pickup = new LootablePickup(ItemType.Experience, 0, 0, 0);

        pickup.Value.Should().Be(0);
    }

    [Theory]
    [InlineData(ItemType.Gold)]
    [InlineData(ItemType.Experience)]
    [InlineData(ItemType.HealthPotion)]
    [InlineData(ItemType.PowerUp)]
    [InlineData(ItemType.Upgrade)]
    public void Constructor_WithDifferentItemTypes_ShouldCreatePickup(ItemType itemType)
    {
        var pickup = new LootablePickup(itemType, 10, 0, 0);

        pickup.Type.Should().Be(itemType);
    }

    [Theory]
    [InlineData(10.0f, 0.0f, 15.0f, true)]   // Within radius - distance is 10
    [InlineData(15.0f, 0.0f, 15.0f, true)]   // Exactly at radius - distance is 15
    [InlineData(20.0f, 0.0f, 25.0f, true)]   // Within radius - distance is 20
    [InlineData(30.0f, 0.0f, 25.0f, false)]  // Outside radius - distance is 30
    public void CanBePickedUp_WithinRadius_ShouldReturnCorrectResult(float playerX, float playerY, float pickupRadius, bool expectedResult)
    {
        var pickup = new LootablePickup(ItemType.Gold, 10, 0, 0, pickupRadius);

        var result = pickup.CanBePickedUp(playerX, playerY);

        result.Should().Be(expectedResult);
    }

    [Fact]
    public void CanBePickedUp_WhenInactive_ShouldReturnFalse()
    {
        var pickup = new LootablePickup(ItemType.Gold, 10, 0, 0, 50);
        pickup.Deactivate();

        var result = pickup.CanBePickedUp(0, 0);

        result.Should().BeFalse();
    }

    [Fact]
    public void CanBePickedUp_WhenCollected_ShouldReturnFalse()
    {
        var pickup = new LootablePickup(ItemType.Gold, 10, 0, 0, 50);
        pickup.Collect();

        var result = pickup.CanBePickedUp(0, 0);

        result.Should().BeFalse();
    }

    [Fact]
    public void CanBePickedUp_WhenExpired_ShouldReturnFalse()
    {
        var pickup = new LootablePickup(ItemType.Gold, 10, 0, 0, 50, TimeSpan.FromMilliseconds(1));
        System.Threading.Thread.Sleep(10); // Wait for expiration

        var result = pickup.CanBePickedUp(0, 0);

        result.Should().BeFalse();
    }

    [Fact]
    public void Collect_WhenActive_ShouldSetCollectedAndInactive()
    {
        var pickup = new LootablePickup(ItemType.Gold, 10, 0, 0);

        pickup.Collect();

        pickup.IsCollected.Should().BeTrue();
        pickup.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Collect_WhenInactive_ShouldThrowInvalidOperationException()
    {
        var pickup = new LootablePickup(ItemType.Gold, 10, 0, 0);
        pickup.Deactivate();

        var action = () => pickup.Collect();

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot collect inactive pickup");
    }

    [Fact]
    public void Collect_WhenAlreadyCollected_ShouldThrowInvalidOperationException()
    {
        var pickup = new LootablePickup(ItemType.Gold, 10, 0, 0);
        pickup.Collect();

        var action = () => pickup.Collect();

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot collect inactive pickup");
    }

    [Fact]
    public void Collect_WhenExpired_ShouldThrowInvalidOperationException()
    {
        var pickup = new LootablePickup(ItemType.Gold, 10, 0, 0, 20, TimeSpan.FromMilliseconds(1));
        System.Threading.Thread.Sleep(10); // Wait for expiration

        var action = () => pickup.Collect();

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot collect expired pickup");
    }

    [Fact]
    public void HasExpired_WithoutExpirationTime_ShouldReturnFalse()
    {
        var pickup = new LootablePickup(ItemType.Gold, 10, 0, 0);

        pickup.HasExpired().Should().BeFalse();
    }

    [Fact]
    public void HasExpired_BeforeExpirationTime_ShouldReturnFalse()
    {
        var pickup = new LootablePickup(ItemType.Gold, 10, 0, 0, 20, TimeSpan.FromMinutes(1));

        pickup.HasExpired().Should().BeFalse();
    }

    [Fact]
    public void HasExpired_AfterExpirationTime_ShouldReturnTrue()
    {
        var pickup = new LootablePickup(ItemType.Gold, 10, 0, 0, 20, TimeSpan.FromMilliseconds(1));
        System.Threading.Thread.Sleep(10); // Wait for expiration

        pickup.HasExpired().Should().BeTrue();
    }

    [Fact]
    public void GetTimeUntilExpiration_WithoutExpirationTime_ShouldReturnNull()
    {
        var pickup = new LootablePickup(ItemType.Gold, 10, 0, 0);

        pickup.GetTimeUntilExpiration().Should().BeNull();
    }

    [Fact]
    public void GetTimeUntilExpiration_BeforeExpiration_ShouldReturnTimeLeft()
    {
        var expiration = TimeSpan.FromMinutes(5);
        var pickup = new LootablePickup(ItemType.Gold, 10, 0, 0, 20, expiration);

        var timeLeft = pickup.GetTimeUntilExpiration();

        timeLeft.Should().NotBeNull();
        timeLeft!.Value.Should().BeCloseTo(expiration, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void GetTimeUntilExpiration_AfterExpiration_ShouldReturnZero()
    {
        var pickup = new LootablePickup(ItemType.Gold, 10, 0, 0, 20, TimeSpan.FromMilliseconds(1));
        System.Threading.Thread.Sleep(10); // Wait for expiration

        var timeLeft = pickup.GetTimeUntilExpiration();

        timeLeft.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void CalculateDistance_ShouldReturnCorrectDistance()
    {
        var pickup = new LootablePickup(ItemType.Gold, 10, 0, 0);
        var targetX = 30.0f;
        var targetY = 40.0f;

        var distance = pickup.CalculateDistance(targetX, targetY);

        distance.Should().BeApproximately(50.0f, 0.001f); // 3-4-5 triangle
    }

    [Fact]
    public void CalculateDistance_WithSamePosition_ShouldReturnZero()
    {
        var pickup = new LootablePickup(ItemType.Gold, 10, 10, 20);

        var distance = pickup.CalculateDistance(10, 20);

        distance.Should().Be(0f);
    }

    [Fact]
    public void MoveTo_WhenActive_ShouldUpdatePosition()
    {
        var pickup = new LootablePickup(ItemType.Gold, 10, 0, 0);
        var newX = 50.0f;
        var newY = 75.0f;

        pickup.MoveTo(newX, newY);

        pickup.X.Should().Be(newX);
        pickup.Y.Should().Be(newY);
    }

    [Fact]
    public void MoveTo_WhenInactive_ShouldThrowInvalidOperationException()
    {
        var pickup = new LootablePickup(ItemType.Gold, 10, 0, 0);
        pickup.Deactivate();

        var action = () => pickup.MoveTo(10, 20);

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot move inactive pickup");
    }

    [Theory]
    [InlineData(float.NaN, 0.0f)]
    [InlineData(0.0f, float.NaN)]
    [InlineData(float.PositiveInfinity, 0.0f)]
    [InlineData(0.0f, float.NegativeInfinity)]
    public void MoveTo_WithInvalidPosition_ShouldThrowArgumentException(float x, float y)
    {
        var pickup = new LootablePickup(ItemType.Gold, 10, 0, 0);

        var action = () => pickup.MoveTo(x, y);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        var pickup = new LootablePickup(ItemType.Gold, 10, 0, 0);

        pickup.Deactivate();

        pickup.IsActive.Should().BeFalse();
    }

    [Fact]
    public void CreateGold_ShouldCreateGoldPickupWithCorrectProperties()
    {
        var amount = 100;
        var x = 50.0f;
        var y = 75.0f;

        var pickup = LootablePickup.CreateGold(amount, x, y);

        pickup.Type.Should().Be(ItemType.Gold);
        pickup.Value.Should().Be(amount);
        pickup.X.Should().Be(x);
        pickup.Y.Should().Be(y);
        pickup.PickupRadius.Should().Be(15.0f);
        pickup.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddSeconds(30), TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CreateExperience_ShouldCreateExperiencePickupWithCorrectProperties()
    {
        var amount = 50;
        var x = 25.0f;
        var y = 35.0f;

        var pickup = LootablePickup.CreateExperience(amount, x, y);

        pickup.Type.Should().Be(ItemType.Experience);
        pickup.Value.Should().Be(amount);
        pickup.X.Should().Be(x);
        pickup.Y.Should().Be(y);
        pickup.PickupRadius.Should().Be(20.0f);
        pickup.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddSeconds(45), TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CreateHealthPotion_ShouldCreateHealthPotionWithCorrectProperties()
    {
        var healAmount = 75;
        var x = 30.0f;
        var y = 40.0f;

        var pickup = LootablePickup.CreateHealthPotion(healAmount, x, y);

        pickup.Type.Should().Be(ItemType.HealthPotion);
        pickup.Value.Should().Be(healAmount);
        pickup.X.Should().Be(x);
        pickup.Y.Should().Be(y);
        pickup.PickupRadius.Should().Be(18.0f);
        pickup.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(2), TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CreatePowerUp_ShouldCreatePowerUpWithCorrectProperties()
    {
        var level = 3;
        var x = 60.0f;
        var y = 80.0f;

        var pickup = LootablePickup.CreatePowerUp(level, x, y);

        pickup.Type.Should().Be(ItemType.PowerUp);
        pickup.Value.Should().Be(level);
        pickup.X.Should().Be(x);
        pickup.Y.Should().Be(y);
        pickup.PickupRadius.Should().Be(25.0f);
        pickup.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(1), TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CreateUpgrade_ShouldCreateUpgradeWithCorrectProperties()
    {
        var tier = 2;
        var x = 90.0f;
        var y = 110.0f;

        var pickup = LootablePickup.CreateUpgrade(tier, x, y);

        pickup.Type.Should().Be(ItemType.Upgrade);
        pickup.Value.Should().Be(tier);
        pickup.X.Should().Be(x);
        pickup.Y.Should().Be(y);
        pickup.PickupRadius.Should().Be(30.0f);
        pickup.ExpiresAt.Should().BeNull(); // Upgrades don't expire
    }

    [Fact]
    public void ToString_ShouldFormatCorrectly()
    {
        var pickup = new LootablePickup(ItemType.Gold, 100, 50.5f, 75.7f);

        var result = pickup.ToString();

        result.Should().Contain("Type:Gold")
            .And.Contain("Value:100")
            .And.Contain("Pos:(50.5,75.7)")
            .And.Contain("Active:True")
            .And.Contain("Collected:False");
    }

    [Fact]
    public void ToString_WithExpiration_ShouldShowExpirationTime()
    {
        var pickup = new LootablePickup(ItemType.Experience, 25, 0, 0, 20, TimeSpan.FromMinutes(1));

        var result = pickup.ToString();

        result.Should().Contain("Expires:");
    }

    [Fact]
    public void ToString_WithoutExpiration_ShouldNotShowExpirationTime()
    {
        var pickup = new LootablePickup(ItemType.Upgrade, 1, 0, 0);

        var result = pickup.ToString();

        result.Should().NotContain("Expires:");
    }

    [Fact]
    public void ComplexScenario_PickupLifecycle_ShouldWorkCorrectly()
    {
        // Create a gold pickup
        var pickup = LootablePickup.CreateGold(50, 100, 200);

        // Initial state
        pickup.IsActive.Should().BeTrue();
        pickup.IsCollected.Should().BeFalse();
        pickup.HasExpired().Should().BeFalse();

        // Player approaches but is too far
        pickup.CanBePickedUp(130, 230).Should().BeFalse();

        // Player gets closer - within pickup radius
        pickup.CanBePickedUp(110, 210).Should().BeTrue();

        // Move pickup to new location
        pickup.MoveTo(150, 250);
        pickup.X.Should().Be(150);
        pickup.Y.Should().Be(250);

        // Collect the pickup
        pickup.Collect();
        pickup.IsCollected.Should().BeTrue();
        pickup.IsActive.Should().BeFalse();

        // Try to collect again - should fail
        var action = () => pickup.Collect();
        action.Should().Throw<InvalidOperationException>();

        // Can no longer be picked up
        pickup.CanBePickedUp(150, 250).Should().BeFalse();
    }

    [Fact]
    public void ExpirationScenario_ShouldExpireCorrectly()
    {
        // Create a pickup that expires quickly
        var pickup = new LootablePickup(ItemType.Experience, 25, 0, 0, 20, TimeSpan.FromMilliseconds(10));

        // Initially not expired
        pickup.HasExpired().Should().BeFalse();
        pickup.CanBePickedUp(0, 0).Should().BeTrue();

        // Wait for expiration
        System.Threading.Thread.Sleep(20);

        // Now expired
        pickup.HasExpired().Should().BeTrue();
        pickup.CanBePickedUp(0, 0).Should().BeFalse();
        pickup.GetTimeUntilExpiration().Should().Be(TimeSpan.Zero);

        // Cannot collect expired pickup
        var action = () => pickup.Collect();
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot collect expired pickup");
    }
}
