using System;

namespace Game.Domain.Buildings.ValueObjects;

public readonly struct BuildingType : IEquatable<BuildingType>
{
    public string InternalId { get; }
    public string ConfigKey { get; }
    public string DisplayName { get; }
    public string Category { get; }
    
    public BuildingType(string internalId, string configKey, string displayName, string category)
    {
        if (string.IsNullOrWhiteSpace(internalId))
            throw new ArgumentException("Internal ID cannot be null or empty", nameof(internalId));
        if (string.IsNullOrWhiteSpace(configKey))
            throw new ArgumentException("Config key cannot be null or empty", nameof(configKey));
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display name cannot be null or empty", nameof(displayName));
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category cannot be null or empty", nameof(category));
            
        InternalId = internalId;
        ConfigKey = configKey;
        DisplayName = displayName;
        Category = category;
    }
    
    public bool Equals(BuildingType other)
    {
        return InternalId == other.InternalId;
    }
    
    public override bool Equals(object? obj)
    {
        return obj is BuildingType other && Equals(other);
    }
    
    public override int GetHashCode()
    {
        return InternalId.GetHashCode();
    }
    
    public static bool operator ==(BuildingType left, BuildingType right)
    {
        return left.Equals(right);
    }
    
    public static bool operator !=(BuildingType left, BuildingType right)
    {
        return !left.Equals(right);
    }
    
    public override string ToString()
    {
        return $"{DisplayName} ({InternalId})";
    }
}
