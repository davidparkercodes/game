using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using Godot;
using Game.Application.UI.Configuration;
using Game.Application.UI.Services;

namespace Game.Infrastructure.UI.Services;

public class HudLayoutConfigService : IHudLayoutConfigService
{
    private static HudLayoutConfig? _cachedConfig;
    private const string ConfigPath = "res://config/ui/hud_layouts.json";
    private const string LogPrefix = "🎨 [HUD_CONFIG]";

    public HudLayoutConfig GetConfiguration()
    {
        if (_cachedConfig != null)
        {
            return _cachedConfig;
        }

        try
        {
            var file = FileAccess.Open(ConfigPath, FileAccess.ModeFlags.Read);
            if (file == null)
            {
                GD.PrintErr($"{LogPrefix} Failed to open config file: {ConfigPath}");
                return GetDefaultConfiguration();
            }

            string content = file.GetAsText();
            file.Close();

            if (string.IsNullOrEmpty(content))
            {
                GD.PrintErr($"{LogPrefix} Config file is empty: {ConfigPath}");
                return GetDefaultConfiguration();
            }

            var wrapper = JsonSerializer.Deserialize<HudLayoutConfigWrapper>(content);
            if (wrapper == null)
            {
                GD.PrintErr($"{LogPrefix} Failed to deserialize config file: {ConfigPath}");
                return GetDefaultConfiguration();
            }

            _cachedConfig = new HudLayoutConfig
            {
                HudLayouts = wrapper.HudLayouts ?? new Dictionary<string, HudElementLayout>(),
                FontSettings = wrapper.FontSettings ?? new FontSettings(),
                Colors = wrapper.Colors ?? new HudColors()
            };

            GD.Print($"{LogPrefix} Configuration loaded successfully from {ConfigPath}");
            return _cachedConfig;
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix} Error loading configuration: {ex.Message}");
            return GetDefaultConfiguration();
        }
    }

    public string GetValidatedAssetPath(string assetPath)
    {
        if (string.IsNullOrEmpty(assetPath))
        {
            GD.PrintErr($"{LogPrefix} Asset path is null or empty");
            return "res://icon.svg";
        }

        if (!assetPath.StartsWith("res://"))
        {
            GD.PrintErr($"{LogPrefix} Invalid asset path format: {assetPath}");
            return "res://icon.svg";
        }

        if (!FileAccess.FileExists(assetPath))
        {
            GD.PrintErr($"{LogPrefix} Asset file not found: {assetPath}");
            return "res://icon.svg";
        }

        return assetPath;
    }

    private HudLayoutConfig GetDefaultConfiguration()
    {
        GD.Print($"{LogPrefix} Using default configuration");
        return new HudLayoutConfig
        {
            HudLayouts = new Dictionary<string, HudElementLayout>(),
            FontSettings = new FontSettings(),
            Colors = new HudColors()
        };
    }
}

public class HudLayoutConfigWrapper
{
    [JsonPropertyName("hud_layouts")]
    public Dictionary<string, HudElementLayout>? HudLayouts { get; set; }

    [JsonPropertyName("font_settings")]
    public FontSettings? FontSettings { get; set; }

    [JsonPropertyName("colors")]
    public HudColors? Colors { get; set; }
}
