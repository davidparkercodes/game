using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Game.Domain.Levels.Services;
using Game.Domain.Levels.ValueObjects;
using Game.Domain.Common.Services;

namespace Game.Infrastructure.Levels;

public class GodotLevelDataRepository : ILevelDataRepository
{
    private readonly ILogger _logger;
    private const string LEVELS_BASE_PATH = "res://data/levels/";

    public GodotLevelDataRepository(ILogger? logger = null)
    {
        _logger = logger ?? new ConsoleLogger("🗂️ [LEVEL-REPO]");
    }

    public Task<LevelData?> LoadLevelAsync(string levelName)
    {
        try
        {
            var resourcePath = $"{LEVELS_BASE_PATH}{levelName}.tres";
            
            if (!FileAccess.FileExists(resourcePath))
            {
                _logger.LogWarning($"Level file not found: {resourcePath}");
                return Task.FromResult<LevelData?>(null);
            }

            var resource = GD.Load<LevelDataResource>(resourcePath);
            if (resource == null)
            {
                _logger.LogError($"Failed to load level resource: {resourcePath}");
                return Task.FromResult<LevelData?>(null);
            }

            _logger.LogInformation($"Successfully loaded level: {levelName}");
            return Task.FromResult<LevelData?>(resource.ToLevelData());
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error loading level '{levelName}': {ex.Message}");
            return Task.FromResult<LevelData?>(null);
        }
    }

    public Task<bool> SaveLevelAsync(string levelName, LevelData levelData)
    {
        try
        {
            var resource = LevelDataResource.FromLevelData(levelData);
            var resourcePath = $"{LEVELS_BASE_PATH}{levelName}.tres";
            
            var error = ResourceSaver.Save(resource, resourcePath);
            if (error != Error.Ok)
            {
                _logger.LogError($"Failed to save level '{levelName}': {error}");
                return Task.FromResult(false);
            }

            _logger.LogInformation($"Successfully saved level: {levelName}");
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error saving level '{levelName}': {ex.Message}");
            return Task.FromResult(false);
        }
    }

    public Task<string[]> GetAvailableLevelsAsync()
    {
        try
        {
            var dir = DirAccess.Open(LEVELS_BASE_PATH);
            if (dir == null)
            {
                _logger.LogWarning($"Levels directory not found: {LEVELS_BASE_PATH}");
                return Task.FromResult(Array.Empty<string>());
            }

            var levels = new List<string>();
            dir.ListDirBegin();
            
            string fileName = dir.GetNext();
            while (fileName != "")
            {
                if (fileName.EndsWith(".tres") && !dir.CurrentIsDir())
                {
                    var levelName = fileName.Replace(".tres", "");
                    levels.Add(levelName);
                }
                fileName = dir.GetNext();
            }

            _logger.LogInformation($"Found {levels.Count} available levels");
            return Task.FromResult(levels.ToArray());
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting available levels: {ex.Message}");
            return Task.FromResult(Array.Empty<string>());
        }
    }

    public Task<bool> LevelExistsAsync(string levelName)
    {
        var resourcePath = $"{LEVELS_BASE_PATH}{levelName}.tres";
        return Task.FromResult(FileAccess.FileExists(resourcePath));
    }

    public LevelData GetDefaultLevel()
    {
        return LevelData.CreateDefault();
    }
}
