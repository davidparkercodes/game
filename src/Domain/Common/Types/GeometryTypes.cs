using System;

namespace Game.Domain.Common.Types;

public struct Vector2
{
    public float X { get; set; }
    public float Y { get; set; }
    
    public Vector2(float x, float y)
    {
        X = x;
        Y = y;
    }
    
    public float Length => (float)Math.Sqrt(X * X + Y * Y);
    
    public Vector2 Normalized()
    {
        var length = Length;
        return length > 0 ? new Vector2(X / length, Y / length) : new Vector2(0, 0);
    }
    
    public float DistanceTo(Vector2 other)
    {
        var dx = X - other.X;
        var dy = Y - other.Y;
        return (float)Math.Sqrt(dx * dx + dy * dy);
    }
    
    public static Vector2 operator +(Vector2 a, Vector2 b)
    {
        return new Vector2(a.X + b.X, a.Y + b.Y);
    }
    
    public static Vector2 operator -(Vector2 a, Vector2 b)
    {
        return new Vector2(a.X - b.X, a.Y - b.Y);
    }
    
    public static Vector2 operator *(Vector2 vector, float scalar)
    {
        return new Vector2(vector.X * scalar, vector.Y * scalar);
    }
    
    public override string ToString()
    {
        return $"({X:F2}, {Y:F2})";
    }
    
    // Implicit conversions to Godot types removed to maintain framework independence
    // Use converter utilities in Infrastructure layer for Godot interop
}

public struct Rect2
{
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    
    public Rect2(Vector2 position, Vector2 size)
    {
        Position = position;
        Size = size;
    }
    
    public Rect2(float x, float y, float width, float height)
    {
        Position = new Vector2(x, y);
        Size = new Vector2(width, height);
    }
    
    public Vector2 End => new Vector2(Position.X + Size.X, Position.Y + Size.Y);
    
    public bool HasPoint(Vector2 point)
    {
        return point.X >= Position.X && 
               point.Y >= Position.Y && 
               point.X <= Position.X + Size.X && 
               point.Y <= Position.Y + Size.Y;
    }
    
    public Rect2 Grow(float margin)
    {
        return new Rect2(
            Position.X - margin, 
            Position.Y - margin, 
            Size.X + (margin * 2), 
            Size.Y + (margin * 2)
        );
    }
    
    // Implicit conversions to Godot types removed to maintain framework independence
    // Use converter utilities in Infrastructure layer for Godot interop
    
    public override string ToString()
    {
        return $"({Position.X}, {Position.Y}, {Size.X}, {Size.Y})";
    }
}
