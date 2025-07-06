using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Game.Domain.Enemies.Entities;
using Game.Domain.Enemies.Services;
using Game.Domain.Levels.ValueObjects;
using Game.Domain.Common.Services;
using Game.Infrastructure.Levels;

namespace Game.Infrastructure.Enemies;

[Tool]
public partial class GodotPathManager : Node2D, IPathManager
{
    private readonly ILogger _logger;
    private Resource? _currentLevel;
    
    [Export] 
    public Resource? CurrentLevel 
    { 
        get => _currentLevel;
        set
        {
            _currentLevel = value;
            if (_currentLevel != null)
            {
                try
                {
                    // Try direct cast first
                    if (_currentLevel is LevelDataResource levelResource)
                    {
                        LoadPathFromLevel(levelResource.ToLevelData());
                        QueueRedraw();
                        return;
                    }
                    
                    // Attempt conversion
                    var converted = TryConvertToLevelDataResource(_currentLevel);
                    if (converted != null)
                    {
                        LoadPathFromLevel(converted.ToLevelData());
                        QueueRedraw();
                        return;
                    }
                    
                    _logger.LogError($"Cannot convert CurrentLevel to LevelDataResource, got: {_currentLevel.GetType().Name}");
                }
                catch (System.Exception ex)
                {
                    _logger.LogError($"Error loading level data - {ex.Message}");
                }
            }
        }
    }

    private readonly List<OrderedPathPoint> _pathPoints;
    private readonly float _pathTolerance;
    private Color _pathColor = Colors.Yellow;
    private float _pathWidth = 5.0f;

    public GodotPathManager() : this(new ConsoleLogger("🗺️ [PATH]"))
    {
    }
    
    public GodotPathManager(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _pathPoints = new List<OrderedPathPoint>();
        _pathTolerance = 10.0f;
    }

    public override void _Ready()
    {
        try
        {
            if (CurrentLevel != null)
            {
                // Try direct cast first
                if (CurrentLevel is LevelDataResource levelResource)
                {
                    LoadPathFromLevel(levelResource.ToLevelData());
                    QueueRedraw();
                    return;
                }
                
                // Try conversion
                var converted = TryConvertToLevelDataResource(CurrentLevel);
                if (converted != null)
                {
                    LoadPathFromLevel(converted.ToLevelData());
                    QueueRedraw();
                    return;
                }
                
                _logger.LogError($"_Ready: Cannot load CurrentLevel, got: {CurrentLevel.GetType().Name}");
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError($"_Ready: Error loading path - {ex.Message}");
        }
    }

    public void LoadPathFromLevel(LevelData levelData)
    {
        _pathPoints.Clear();
        for (int i = 0; i < levelData.PathPoints.Count; i++)
        {
            var levelPoint = levelData.PathPoints[i];
            _pathPoints.Add(new OrderedPathPoint(levelPoint.X, levelPoint.Y, i));
        }
        
        // Use default yellow path color
        _pathColor = Colors.Yellow;
        
        QueueRedraw(); // Trigger redraw when path is loaded
    }

    public void AddPathPoint(float x, float y, int order = -1)
    {
        ValidatePosition(x, y);
        
        var pathPoint = new OrderedPathPoint(x, y, order == -1 ? _pathPoints.Count : order);
        _pathPoints.Add(pathPoint);
        
        _pathPoints.Sort((a, b) => a.Order.CompareTo(b.Order));
        QueueRedraw();
    }

    public void RemovePathPoint(float x, float y)
    {
        var pointToRemove = _pathPoints.FirstOrDefault(p => 
            Math.Abs(p.X - x) <= _pathTolerance && Math.Abs(p.Y - y) <= _pathTolerance);
        
        if (pointToRemove != null)
        {
            _pathPoints.Remove(pointToRemove);
            ReorderPathPoints();
            QueueRedraw();
        }
    }

    public OrderedPathPoint? GetNextPathPoint(Enemy enemy)
    {
        if (enemy == null)
            throw new ArgumentNullException(nameof(enemy));

        if (!enemy.IsAlive)
            return null;

        var currentPoint = GetCurrentPathPoint(enemy);
        if (currentPoint == null)
            return _pathPoints.FirstOrDefault();

        var nextIndex = currentPoint.Order + 1;
        return _pathPoints.FirstOrDefault(p => p.Order == nextIndex);
    }

    public OrderedPathPoint? GetCurrentPathPoint(Enemy enemy)
    {
        if (enemy == null)
            throw new ArgumentNullException(nameof(enemy));

        return _pathPoints
            .Where(p => IsEnemyAtPoint(enemy, p))
            .OrderBy(p => p.Order)
            .FirstOrDefault();
    }

    public bool IsEnemyAtPoint(Enemy enemy, OrderedPathPoint point)
    {
        if (enemy == null || point == null)
            return false;

        return enemy.CalculateDistance(point.X, point.Y) <= _pathTolerance;
    }

    public float CalculatePathDistance(Enemy enemy, OrderedPathPoint targetPoint)
    {
        if (enemy == null || targetPoint == null)
            return float.MaxValue;

        var currentPoint = GetCurrentPathPoint(enemy);
        if (currentPoint == null)
            return enemy.CalculateDistance(targetPoint.X, targetPoint.Y);

        var totalDistance = 0f;
        var currentIndex = currentPoint.Order;
        var targetIndex = targetPoint.Order;

        if (currentIndex >= targetIndex)
            return enemy.CalculateDistance(targetPoint.X, targetPoint.Y);

        for (int i = currentIndex; i < targetIndex; i++)
        {
            var from = i == currentIndex ? 
                new OrderedPathPoint(enemy.X, enemy.Y, i) : 
                _pathPoints.FirstOrDefault(p => p.Order == i);
            
            var to = _pathPoints.FirstOrDefault(p => p.Order == i + 1);
            
            if (from != null && to != null)
            {
                totalDistance += CalculateDistance(from.X, from.Y, to.X, to.Y);
            }
        }

        return totalDistance;
    }

    public bool HasReachedEnd(Enemy enemy)
    {
        if (enemy == null || !_pathPoints.Any())
            return false;

        var lastPoint = _pathPoints.OrderByDescending(p => p.Order).First();
        return IsEnemyAtPoint(enemy, lastPoint);
    }

    public void ClearPath()
    {
        _pathPoints.Clear();
        QueueRedraw();
    }

    public IReadOnlyList<OrderedPathPoint> GetPathPoints()
    {
        return _pathPoints.AsReadOnly();
    }

    public override void _Draw()
    {
        if (_pathPoints.Count < 2)
            return;
            
        // Draw lines between consecutive path points
        for (int i = 0; i < _pathPoints.Count - 1; i++)
        {
            var fromPoint = _pathPoints[i];
            var toPoint = _pathPoints[i + 1];
            
            var from = new Vector2(fromPoint.X, fromPoint.Y);
            var to = new Vector2(toPoint.X, toPoint.Y);
            
            // Draw the path line
            DrawLine(from, to, _pathColor, _pathWidth);
        }
        
        // Draw circles at each path point for visibility
        foreach (var point in _pathPoints)
        {
            var position = new Vector2(point.X, point.Y);
            DrawCircle(position, 8.0f, _pathColor);
        }
    }

    private LevelDataResource? TryConvertToLevelDataResource(Resource resource)
    {
        if (resource is LevelDataResource)
            return (LevelDataResource)resource;
        
        // Handle Godot deserialization issues by reconstructing from properties
        try
        {
            // Debug: Try different ways to access properties
            _logger.LogDebug($"Resource type: {resource.GetType().Name}");
            _logger.LogDebug($"Resource script: {resource.GetScript()}");
            
            // Try getting specific properties directly
            _logger.LogDebug($"LevelName: {resource.Get("LevelName")}");
            _logger.LogDebug($"Description: {resource.Get("Description")}");
            
            // Check if resource has the expected PathPoints property
            var pathPointsVariant = resource.Get("PathPoints");
            _logger.LogDebug($"PathPoints variant type: {pathPointsVariant.VariantType}");
            _logger.LogDebug($"PathPoints raw value: {pathPointsVariant}");
            
            Vector2[]? pathPointsArray = null;
            
            if (pathPointsVariant.VariantType == Variant.Type.PackedVector2Array)
            {
                pathPointsArray = pathPointsVariant.AsVector2Array();
                _logger.LogDebug($"Found {pathPointsArray.Length} path points in PackedVector2Array");
            }
            else if (pathPointsVariant.VariantType == Variant.Type.Array)
            {
                _logger.LogDebug("Trying to convert from Array to Vector2[]");
                var array = pathPointsVariant.AsGodotArray();
                pathPointsArray = new Vector2[array.Count];
                for (int i = 0; i < array.Count; i++)
                {
                    pathPointsArray[i] = array[i].AsVector2();
                }
                _logger.LogDebug($"Converted {pathPointsArray.Length} points from Array");
            }
            
            if (pathPointsArray != null && pathPointsArray.Length > 0)
            {
                // Debug: Print first few points if any
                for (int i = 0; i < Math.Min(3, pathPointsArray.Length); i++)
                {
                    _logger.LogDebug($"  Point {i}: {pathPointsArray[i]}");
                }
                
                // Create new LevelDataResource instance and copy all properties
                var levelResource = new LevelDataResource();
                levelResource.LevelName = resource.Get("LevelName").AsString();
                levelResource.Description = resource.Get("Description").AsString();
                levelResource.PathPoints = pathPointsArray;
                levelResource.PathWidth = resource.Get("PathWidth").AsSingle();
                levelResource.SpawnPoint = resource.Get("SpawnPoint").AsVector2();
                levelResource.EndPoint = resource.Get("EndPoint").AsVector2();
                levelResource.InitialMoney = resource.Get("InitialMoney").AsInt32();
                levelResource.InitialLives = resource.Get("InitialLives").AsInt32();
                levelResource.PathColor = resource.Get("PathColor").AsColor();
                
                _logger.LogInformation($"Created LevelDataResource with {levelResource.PathPoints.Length} path points");
                return levelResource;
            }
            else
            {
                _logger.LogError($"PathPoints conversion failed - variant type: {pathPointsVariant.VariantType}, array length: {pathPointsArray?.Length ?? -1}");
                
                // Try to use default level data as fallback
                _logger.LogInformation("Falling back to default level data");
                var defaultLevelData = LevelData.CreateDefault();
                return LevelDataResource.FromLevelData(defaultLevelData);
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError($"Error during resource conversion - {ex.Message}");
        }
        
        _logger.LogError($"Failed to convert resource type: {resource.GetType().Name}");
        return null;
    }

    private void ReorderPathPoints()
    {
        for (int i = 0; i < _pathPoints.Count; i++)
        {
            _pathPoints[i] = new OrderedPathPoint(_pathPoints[i].X, _pathPoints[i].Y, i);
        }
    }

    private float CalculateDistance(float x1, float y1, float x2, float y2)
    {
        var deltaX = x2 - x1;
        var deltaY = y2 - y1;
        return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }

    private void ValidatePosition(float x, float y)
    {
        if (float.IsNaN(x) || float.IsNaN(y))
            throw new ArgumentException("Position cannot be NaN");
        
        if (float.IsInfinity(x) || float.IsInfinity(y))
            throw new ArgumentException("Position cannot be infinite");
    }
}
