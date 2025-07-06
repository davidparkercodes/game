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
    
    public static implicit operator Godot.Vector2I(Vector2I vector)
    {
        return new Godot.Vector2I(vector.X, vector.Y);
    }
    
    public static implicit operator Vector2I(Godot.Vector2I vector)
    {
        return new Vector2I(vector.X, vector.Y);
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
    
    public static implicit operator Godot.Rect2I(Rect2I rect)
    {
        return new Godot.Rect2I(rect.Position, rect.Size);
    }
    
    public static implicit operator Rect2I(Godot.Rect2I rect)
    {
        return new Rect2I(rect.Position, rect.Size);
    }
}
