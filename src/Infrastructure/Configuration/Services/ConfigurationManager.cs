using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text.Json;
using System.IO;
using Godot;
using Game.Application.Configuration.Services;
using Game.Application.Configuration.Models;

namespace Game.Infrastructure.Configuration.Services;

public class ConfigurationManager : IConfigurationManager
{
    private static ConfigurationManager? _instance;
    public static ConfigurationManager Instance => _instance ??= new ConfigurationManager();

    private readonly ConcurrentDictionary<string, object> _configCache = new();
    private readonly ConcurrentDictionary<string, DateTime> _lastModified = new();
    private ConfigurationRegistry? _registry;
    private const string RegistryPath = "res://config/config_registry.json";
    private const string LogPrefix = "⚙️ [CONFIG_MANAGER]";

    public event EventHandler<ConfigurationChangedEventArgs>? ConfigurationChanged;

    private ConfigurationManager()
    {
        LoadRegistry();
        GD.Print($"{LogPrefix} Configuration manager initialized");
    }

    public T Get<T>(string configPath) where T : class, new()
    {
        return Get(configPath, new T());
    }

    public T Get<T>(string configPath, T defaultValue) where T : class, new()
    {
        try
        {
            if (_configCache.TryGetValue(configPath, out var cachedValue) && cachedValue is T cached)
            {
                if (!ShouldReload(configPath))
                {
                    return cached;
                }
            }

            var filePath = GetFilePath(configPath);
            if (string.IsNullOrEmpty(filePath))
            {
                GD.PrintErr($"{LogPrefix} Configuration path not found in registry: {configPath}");
                return defaultValue;
            }

            if (!FileAccess.FileExists(filePath))
            {
                GD.PrintErr($"{LogPrefix} Configuration file not found: {filePath}");
                return defaultValue;
            }

            var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
            if (file == null)
            {
                GD.PrintErr($"{LogPrefix} Failed to open configuration file: {filePath}");
                return defaultValue;
            }

            var content = file.GetAsText();
            file.Close();

            if (string.IsNullOrEmpty(content))
            {
                GD.PrintErr($"{LogPrefix} Configuration file is empty: {filePath}");
                return defaultValue;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };

            var config = JsonSerializer.Deserialize<T>(content, options);
            if (config == null)
            {
                GD.PrintErr($"{LogPrefix} Failed to deserialize configuration: {filePath}");
                return defaultValue;
            }

            var oldValue = _configCache.TryGetValue(configPath, out var old) ? old : null;
            _configCache[configPath] = config;
            _lastModified[configPath] = DateTime.UtcNow;

            if (oldValue != null && !ReferenceEquals(oldValue, config))
            {
                ConfigurationChanged?.Invoke(this, new ConfigurationChangedEventArgs(
                    configPath, typeof(T), oldValue, config));
            }

            GD.Print($"{LogPrefix} Loaded configuration: {configPath}");
            return config;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"{LogPrefix} Error loading configuration {configPath}: {ex.Message}");
            return defaultValue;
        }
    }

    public void Set<T>(string configPath, T value) where T : class
    {
        try
        {
            var filePath = GetFilePath(configPath);
            if (string.IsNullOrEmpty(filePath))
            {
                GD.PrintErr($"{LogPrefix} Configuration path not found in registry: {configPath}");
                return;
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(value, options);
            
            var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write);
            if (file == null)
            {
                GD.PrintErr($"{LogPrefix} Failed to open configuration file for writing: {filePath}");
                return;
            }

            file.StoreString(json);
            file.Close();

            var oldValue = _configCache.TryGetValue(configPath, out var old) ? old : null;
            _configCache[configPath] = value;
            _lastModified[configPath] = DateTime.UtcNow;

            ConfigurationChanged?.Invoke(this, new ConfigurationChangedEventArgs(
                configPath, typeof(T), oldValue, value));

            GD.Print($"{LogPrefix} Saved configuration: {configPath}");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"{LogPrefix} Error saving configuration {configPath}: {ex.Message}");
        }
    }

    public bool Exists(string configPath)
    {
        var filePath = GetFilePath(configPath);
        return !string.IsNullOrEmpty(filePath) && FileAccess.FileExists(filePath);
    }

    public void Reload(string configPath)
    {
        _configCache.TryRemove(configPath, out _);
        _lastModified.TryRemove(configPath, out _);
        GD.Print($"{LogPrefix} Cleared cache for configuration: {configPath}");
    }

    public void ReloadAll()
    {
        _configCache.Clear();
        _lastModified.Clear();
        LoadRegistry();
        GD.Print($"{LogPrefix} Cleared all configuration cache");
    }

    public bool IsValidConfiguration<T>(string configPath) where T : class
    {
        try
        {
            var config = Get<T>(configPath);
            return config != null;
        }
        catch
        {
            return false;
        }
    }

    public IEnumerable<string> GetAllConfigurationPaths()
    {
        return _registry?.Registry.Configurations.Keys ?? Array.Empty<string>();
    }

    private void LoadRegistry()
    {
        try
        {
            if (!FileAccess.FileExists(RegistryPath))
            {
                GD.PrintErr($"{LogPrefix} Configuration registry not found: {RegistryPath}");
                CreateDefaultRegistry();
                return;
            }

            var file = FileAccess.Open(RegistryPath, FileAccess.ModeFlags.Read);
            if (file == null)
            {
                GD.PrintErr($"{LogPrefix} Failed to open registry file: {RegistryPath}");
                return;
            }

            var content = file.GetAsText();
            file.Close();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            _registry = JsonSerializer.Deserialize<ConfigurationRegistry>(content, options);
            GD.Print($"{LogPrefix} Configuration registry loaded with {_registry?.Registry.Configurations.Count ?? 0} entries");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"{LogPrefix} Error loading configuration registry: {ex.Message}");
            CreateDefaultRegistry();
        }
    }

    private void CreateDefaultRegistry()
    {
        _registry = new ConfigurationRegistry();
        GD.Print($"{LogPrefix} Created default configuration registry");
    }

    private string GetFilePath(string configPath)
    {
        if (_registry?.Registry.Configurations.TryGetValue(configPath, out var entry) == true)
        {
            return entry.FilePath;
        }

        // Fallback: try to construct path from configPath
        return $"res://config/{configPath.Replace('.', '/')}.json";
    }

    private bool ShouldReload(string configPath)
    {
        var entry = _registry?.Registry.Configurations.GetValueOrDefault(configPath);
        if (entry?.HotReload != true)
        {
            return false;
        }

        var filePath = GetFilePath(configPath);
        if (string.IsNullOrEmpty(filePath) || !FileAccess.FileExists(filePath))
        {
            return false;
        }

        if (!_lastModified.TryGetValue(configPath, out var lastCheck))
        {
            return true;
        }

        // For simplicity, we'll reload if it's been more than 1 second since last check
        // In a production system, you'd want to check actual file modification times
        return DateTime.UtcNow - lastCheck > TimeSpan.FromSeconds(1);
    }
}
