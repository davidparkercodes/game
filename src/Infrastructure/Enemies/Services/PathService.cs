using Godot;

namespace Game.Infrastructure.Enemies.Services;

public class PathService
{
    public static PathService Instance { get; private set; }

    static PathService()
    {
        Instance = new PathService();
    }

    public Vector2 GetSpawnPosition()
    {
        return new Vector2(100, -50);
    }

    public Vector2 GetEndPosition()
    {
        return new Vector2(300, 750);
    }

    public Vector2[] GetPathPoints()
    {
        return new Vector2[]
        {
            new Vector2(100, -50),
            new Vector2(100, 150),
            new Vector2(400, 150),
            new Vector2(400, 300),
            new Vector2(150, 300),
            new Vector2(150, 450),
            new Vector2(500, 450),
            new Vector2(500, 600),
            new Vector2(300, 600),
            new Vector2(300, 750)
        };
    }

    public float GetPathLength()
    {
        var points = GetPathPoints();
        float totalLength = 0f;
        for (int i = 1; i < points.Length; i++)
        {
            totalLength += points[i-1].DistanceTo(points[i]);
        }
        return totalLength;
    }

    public Vector2 GetPathPosition(float progress)
    {
        var points = GetPathPoints();
        if (points.Length < 2) return Vector2.Zero;
        
        float targetDistance = progress * GetPathLength();
        float currentDistance = 0f;
        
        for (int i = 1; i < points.Length; i++)
        {
            float segmentLength = points[i-1].DistanceTo(points[i]);
            if (currentDistance + segmentLength >= targetDistance)
            {
                float segmentProgress = (targetDistance - currentDistance) / segmentLength;
                return points[i-1].Lerp(points[i], segmentProgress);
            }
            currentDistance += segmentLength;
        }
        
        return points[points.Length - 1];
    }

    public Vector2 GetPathDirection(float progress)
    {
        var points = GetPathPoints();
        if (points.Length < 2) return Vector2.Right;
        
        float targetDistance = progress * GetPathLength();
        float currentDistance = 0f;
        
        for (int i = 1; i < points.Length; i++)
        {
            float segmentLength = points[i-1].DistanceTo(points[i]);
            if (currentDistance + segmentLength >= targetDistance)
            {
                return (points[i] - points[i-1]).Normalized();
            }
            currentDistance += segmentLength;
        }
        
        return (points[points.Length - 1] - points[points.Length - 2]).Normalized();
    }
}
