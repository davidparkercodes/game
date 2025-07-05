namespace Game.Domain.Shared.ValueObjects;

public readonly struct Position
{
    public float X { get; }
    public float Y { get; }

    public Position(float x, float y)
    {
        X = x;
        Y = y;
    }

    public float DistanceTo(Position other)
    {
        var dx = X - other.X;
        var dy = Y - other.Y;
        return (float)System.Math.Sqrt(dx * dx + dy * dy);
    }

    public override string ToString()
    {
        return $"({X:F1}, {Y:F1})";
    }

    public override bool Equals(object? obj)
    {
        return obj is Position other && Equals(other);
    }

    public bool Equals(Position other)
    {
        return System.Math.Abs(X - other.X) < 0.001f && System.Math.Abs(Y - other.Y) < 0.001f;
    }

    public override int GetHashCode()
    {
        return System.HashCode.Combine(X, Y);
    }

    public static bool operator ==(Position left, Position right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Position left, Position right)
    {
        return !left.Equals(right);
    }
}
