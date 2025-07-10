using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Game.Application.Configuration.Models;

public class ConfigurationRegistry
{
    [JsonPropertyName("registry")]
    public ConfigurationRegistryData Registry { get; set; } = new();
}

public class ConfigurationRegistryData
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0";

    [JsonPropertyName("base_path")]
    public string BasePath { get; set; } = "res://config";

    [JsonPropertyName("configurations")]
    public Dictionary<string, ConfigurationEntry> Configurations { get; set; } = new();

    [JsonPropertyName("categories")]
    public Dictionary<string, CategoryInfo> Categories { get; set; } = new();
}

public class ConfigurationEntry
{
    [JsonPropertyName("file_path")]
    public string FilePath { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("hot_reload")]
    public bool HotReload { get; set; } = true;

    [JsonPropertyName("required")]
    public bool Required { get; set; } = true;

    [JsonPropertyName("schema_version")]
    public string SchemaVersion { get; set; } = "1.0";
}

public class CategoryInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;

    [JsonPropertyName("order")]
    public int Order { get; set; } = 0;
}
