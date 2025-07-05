using Godot;

namespace Game.Infrastructure.Validators;

public static class BuildingZoneValidator
{
    public static bool CanBuildAt(Vector2 position)
    {
        // TODO: Implement proper building zone validation
        // For now, allow building anywhere except on the path
        
        // Check if position is on the path
        if (IsOnPath(position))
        {
            return false;
        }

        // Check if position is within game bounds
        if (!IsWithinBounds(position))
        {
            return false;
        }

        // Check if position is too close to other buildings
        if (IsTooCloseToOtherBuildings(position))
        {
            return false;
        }

        return true;
    }

    public static bool IsValidZone(Vector2 position)
    {
        return CanBuildAt(position);
    }

    public static bool IsInBuildableArea(Vector2 position)
    {
        return CanBuildAt(position);
    }

    public static bool CanBuildAtWithLogging(Vector2 position)
    {
        bool canBuild = CanBuildAt(position);
        if (!canBuild)
        {
            Godot.GD.Print($"❌ Cannot build at {position}: Invalid zone");
        }
        return canBuild;
    }

    public static bool IsOnPath(Vector2 position)
    {
        // TODO: Implement actual path checking
        // For now, assume a simple path area
        var pathPoints = new Vector2[]
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

        const float pathWidth = 64f;

        for (int i = 1; i < pathPoints.Length; i++)
        {
            var start = pathPoints[i - 1];
            var end = pathPoints[i];
            
            // Check distance from position to line segment
            var distance = DistanceToLineSegment(position, start, end);
            if (distance < pathWidth / 2)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsWithinBounds(Vector2 position)
    {
        // TODO: Get actual game bounds
        // For now, use some reasonable bounds
        return position.X >= 0 && position.X <= 1024 && 
               position.Y >= 0 && position.Y <= 768;
    }

    private static bool IsTooCloseToOtherBuildings(Vector2 position)
    {
        // TODO: Implement proper building collision detection
        // For now, return false to allow building
        return false;
    }

    private static float DistanceToLineSegment(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
    {
        var A = point.X - lineStart.X;
        var B = point.Y - lineStart.Y;
        var C = lineEnd.X - lineStart.X;
        var D = lineEnd.Y - lineStart.Y;

        var dot = A * C + B * D;
        var lenSq = C * C + D * D;
        
        if (lenSq == 0)
            return point.DistanceTo(lineStart);

        var param = dot / lenSq;

        Vector2 projection;
        if (param < 0)
        {
            projection = lineStart;
        }
        else if (param > 1)
        {
            projection = lineEnd;
        }
        else
        {
            projection = new Vector2(lineStart.X + param * C, lineStart.Y + param * D);
        }

        return point.DistanceTo(projection);
    }
}
