using System;

namespace Game.Domain.Items.Entities;

public enum ItemType
{
    Gold,
    Experience,
    HealthPotion,
    PowerUp,
    Upgrade
}

public class LootablePickup
{
    public Guid Id { get; }
    public ItemType Type { get; }
    public int Value { get; }
    public float X { get; private set; }
    public float Y { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? ExpiresAt { get; private set; }
    public float PickupRadius { get; }
    public bool IsCollected { get; private set; }

    public LootablePickup(ItemType type, int value, float x, float y, float pickupRadius = 20.0f, TimeSpan? expiration = null)
    {
        ValidatePosition(x, y);
        
        if (value < 0)
            throw new ArgumentException("Value cannot be negative", nameof(value));
        
        if (pickupRadius <= 0)
            throw new ArgumentException("Pickup radius must be positive", nameof(pickupRadius));

        Id = Guid.NewGuid();
        Type = type;
        Value = value;
        X = x;
        Y = y;
        PickupRadius = pickupRadius;
        IsActive = true;
        IsCollected = false;
        CreatedAt = DateTime.UtcNow;
        
        if (expiration.HasValue)
        {
            ExpiresAt = CreatedAt.Add(expiration.Value);
        }
    }

    public bool CanBePickedUp(float playerX, float playerY)
    {
        if (!IsActive || IsCollected)
            return false;

        if (HasExpired())
            return false;

        return CalculateDistance(playerX, playerY) <= PickupRadius;
    }

    public void Collect()
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot collect inactive pickup");
        
        if (IsCollected)
            throw new InvalidOperationException("Pickup already collected");
        
        if (HasExpired())
            throw new InvalidOperationException("Cannot collect expired pickup");

        IsCollected = true;
        IsActive = false;
    }

    public bool HasExpired()
    {
        return ExpiresAt.HasValue && DateTime.UtcNow >= ExpiresAt.Value;
    }

    public TimeSpan? GetTimeUntilExpiration()
    {
        if (!ExpiresAt.HasValue)
            return null;

        var timeLeft = ExpiresAt.Value - DateTime.UtcNow;
        return timeLeft > TimeSpan.Zero ? timeLeft : TimeSpan.Zero;
    }

    public float CalculateDistance(float targetX, float targetY)
    {
        var deltaX = targetX - X;
        var deltaY = targetY - Y;
        return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }

    public void MoveTo(float x, float y)
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot move inactive pickup");
        
        ValidatePosition(x, y);
        X = x;
        Y = y;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public static LootablePickup CreateGold(int amount, float x, float y)
    {
        return new LootablePickup(ItemType.Gold, amount, x, y, 15.0f, TimeSpan.FromSeconds(30));
    }

    public static LootablePickup CreateExperience(int amount, float x, float y)
    {
        return new LootablePickup(ItemType.Experience, amount, x, y, 20.0f, TimeSpan.FromSeconds(45));
    }

    public static LootablePickup CreateHealthPotion(int healAmount, float x, float y)
    {
        return new LootablePickup(ItemType.HealthPotion, healAmount, x, y, 18.0f, TimeSpan.FromMinutes(2));
    }

    public static LootablePickup CreatePowerUp(int level, float x, float y)
    {
        return new LootablePickup(ItemType.PowerUp, level, x, y, 25.0f, TimeSpan.FromMinutes(1));
    }

    public static LootablePickup CreateUpgrade(int tier, float x, float y)
    {
        return new LootablePickup(ItemType.Upgrade, tier, x, y, 30.0f);
    }

    private void ValidatePosition(float x, float y)
    {
        if (float.IsNaN(x) || float.IsNaN(y))
            throw new ArgumentException("Position cannot be NaN");
        
        if (float.IsInfinity(x) || float.IsInfinity(y))
            throw new ArgumentException("Position cannot be infinite");
    }

    public override string ToString()
    {
        var expiration = ExpiresAt.HasValue ? $", Expires:{ExpiresAt:HH:mm:ss}" : "";
        return $"LootablePickup(Type:{Type}, Value:{Value}, Pos:({X:F1},{Y:F1}), Active:{IsActive}, Collected:{IsCollected}{expiration})";
    }
}
