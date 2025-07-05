using System;

namespace Game.Domain.Enemies.ValueObjects;

public readonly struct EnemyType : IEquatable<EnemyType>
{
    public string InternalId { get; }
    public string ConfigKey { get; }
    public string DisplayName { get; }
    public string Category { get; }
    public int Tier { get; }
    
    public EnemyType(string internalId, string configKey, string displayName, string category, int tier = 1)
    {
        if (string.IsNullOrWhiteSpace(internalId))
            throw new ArgumentException("Internal ID cannot be null or empty", nameof(internalId));
        if (string.IsNullOrWhiteSpace(configKey))
            throw new ArgumentException("Config key cannot be null or empty", nameof(configKey));
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display name cannot be null or empty", nameof(displayName));
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category cannot be null or empty", nameof(category));
        if (tier < 1)
            throw new ArgumentException("Tier must be at least 1", nameof(tier));
            
        InternalId = internalId;
        ConfigKey = configKey;
        DisplayName = displayName;
        Category = category;
        Tier = tier;
    }
    
    public bool Equals(EnemyType other)
    {
        return InternalId == other.InternalId;
    }
    
    public override bool Equals(object? obj)
    {
        return obj is EnemyType other && Equals(other);
    }
    
    public override int GetHashCode()
    {
        return InternalId.GetHashCode();
    }
    
    public static bool operator ==(EnemyType left, EnemyType right)
    {
        return left.Equals(right);
    }
    
    public static bool operator !=(EnemyType left, EnemyType right)
    {
        return !left.Equals(right);
    }
    
    public override string ToString()
    {
        return $"{DisplayName} ({InternalId}, Tier {Tier})";
    }
}
