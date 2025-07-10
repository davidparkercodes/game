using System;
using System.Collections.Generic;

namespace Game.Application.Configuration.Services;

public interface IConfigurationManager
{
    T Get<T>(string configPath) where T : class, new();
    
    T Get<T>(string configPath, T defaultValue) where T : class, new();
    
    void Set<T>(string configPath, T value) where T : class;
    
    bool Exists(string configPath);
    
    void Reload(string configPath);
    
    void ReloadAll();
    
    bool IsValidConfiguration<T>(string configPath) where T : class;
    
    IEnumerable<string> GetAllConfigurationPaths();
    
    event EventHandler<ConfigurationChangedEventArgs> ConfigurationChanged;
}

public class ConfigurationChangedEventArgs : EventArgs
{
    public string ConfigPath { get; }
    public Type ConfigType { get; }
    public object? OldValue { get; }
    public object? NewValue { get; }

    public ConfigurationChangedEventArgs(string configPath, Type configType, object? oldValue, object? newValue)
    {
        ConfigPath = configPath;
        ConfigType = configType;
        OldValue = oldValue;
        NewValue = newValue;
    }
}
