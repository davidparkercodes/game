using Game.Domain.Common.Types;

namespace Game.Domain.Map.Interfaces;

public interface ITileMapLayer
{
    Vector2 MapToLocal(Vector2I tileCoords);
    Vector2I LocalToMap(Vector2 localPosition);
    Vector2 ToLocal(Vector2 globalPosition);
    Rect2I GetUsedRect();
    int GetCellSourceId(Vector2I coords);
    Vector2I GetCellAtlasCoords(Vector2I coords);
    object? GetCellTileData(Vector2I coords);
}

public struct Vector2I
{
    public int X { get; set; }
    public int Y { get; set; }
    
    public Vector2I(int x, int y)
    {
        X = x;
        Y = y;
    }
    
    public override string ToString()
    {
        return $"Vector2I(X:{X}, Y:{Y})";
    }
    
    public override bool Equals(object? obj)
    {
        return obj is Vector2I other && X == other.X && Y == other.Y;
    }
    
    public override int GetHashCode()
    {
        return System.HashCode.Combine(X, Y);
    }
    
    public static bool operator ==(Vector2I left, Vector2I right)
    {
        return left.Equals(right);
    }
    
    public static bool operator !=(Vector2I left, Vector2I right)
    {
        return !left.Equals(right);
    }
}

public struct Rect2I
{
    public Vector2I Position { get; set; }
    public Vector2I Size { get; set; }
    
    public Rect2I(Vector2I position, Vector2I size)
    {
        Position = position;
        Size = size;
    }
    
    public Vector2I End => new Vector2I(Position.X + Size.X, Position.Y + Size.Y);
    
    public override string ToString()
    {
        return $"Rect2I(Position:{Position}, Size:{Size})";
    }
    
    public override bool Equals(object? obj)
    {
        return obj is Rect2I other && Position == other.Position && Size == other.Size;
    }
    
    public override int GetHashCode()
    {
        return System.HashCode.Combine(Position, Size);
    }
    
    public static bool operator ==(Rect2I left, Rect2I right)
    {
        return left.Equals(right);
    }
    
    public static bool operator !=(Rect2I left, Rect2I right)
    {
        return !left.Equals(right);
    }
}
