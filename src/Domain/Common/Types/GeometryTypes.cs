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
    
    public static implicit operator Godot.Vector2(Vector2 vector)
    {
        return new Godot.Vector2(vector.X, vector.Y);
    }
    
    public static implicit operator Vector2(Godot.Vector2 vector)
    {
        return new Vector2(vector.X, vector.Y);
    }
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
    
    public static implicit operator Godot.Rect2(Rect2 rect)
    {
        return new Godot.Rect2(rect.Position, rect.Size);
    }
    
    public static implicit operator Rect2(Godot.Rect2 rect)
    {
        return new Rect2(rect.Position, rect.Size);
    }
    
    public override string ToString()
    {
        return $"({Position.X}, {Position.Y}, {Size.X}, {Size.Y})";
    }
}
