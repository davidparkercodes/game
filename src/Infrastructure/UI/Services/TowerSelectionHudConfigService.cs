using System;
using System.Text.Json;
using Game.Application.UI.Configuration;
using Game.Application.UI.Services;
using Godot;

namespace Game.Infrastructure.UI.Services;

public class TowerSelectionHudConfigService : ITowerSelectionHudConfigService
{
    private const string CONFIG_PATH = "res://data/huds/tower_selection_hud.json";
    private const string DEFAULT_ICON_PATH = "res://assets/sprites/ui/default_tower_icon.png";
    
    private TowerSelectionHudConfig? _cachedConfig;
    private bool _configLoaded = false;

    public TowerSelectionHudConfig GetConfiguration()
    {
        if (!_configLoaded)
        {
            LoadConfiguration();
        }
        
        return _cachedConfig ?? CreateDefaultConfiguration();
    }

    public bool IsConfigurationValid()
    {
        try
        {
            var config = GetConfiguration();
            return config.Layout.SquareSize > 0 && 
                   config.Layout.MaxTowers > 0 && 
                   config.Towers.Count > 0;
        }
        catch
        {
            return false;
        }
    }

    public string GetDefaultIconPath()
    {
        return DEFAULT_ICON_PATH;
    }

    public string GetValidatedIconPath(string iconPath)
    {
        if (DoesIconExist(iconPath))
        {
            return iconPath;
        }
        
        GD.PrintErr($"⚠️ TowerSelectionHudConfigService: Icon not found at {iconPath}, using default");
        return DEFAULT_ICON_PATH;
    }

    public bool DoesIconExist(string iconPath)
    {
        try
        {
            if (IsInGodotRuntime())
            {
                return Godot.FileAccess.FileExists(iconPath);
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    private void LoadConfiguration()
    {
        try
        {
            if (IsInGodotRuntime() && Godot.FileAccess.FileExists(CONFIG_PATH))
            {
                using var file = Godot.FileAccess.Open(CONFIG_PATH, Godot.FileAccess.ModeFlags.Read);
                if (file != null)
                {
                    string jsonContent = file.GetAsText();
                    
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        ReadCommentHandling = JsonCommentHandling.Skip,
                        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                    };
                    
                    _cachedConfig = JsonSerializer.Deserialize<TowerSelectionHudConfig>(jsonContent, options);
                    GD.Print($"✅ TowerSelectionHudConfigService: Configuration loaded from {CONFIG_PATH}");
                }
            }
            else
            {
                GD.PrintErr($"⚠️ TowerSelectionHudConfigService: Config file not found at {CONFIG_PATH}, using defaults");
                _cachedConfig = CreateDefaultConfiguration();
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"❌ TowerSelectionHudConfigService: Error loading config: {ex.Message}");
            _cachedConfig = CreateDefaultConfiguration();
        }
        finally
        {
            _configLoaded = true;
        }
    }

    private TowerSelectionHudConfig CreateDefaultConfiguration()
    {
        return new TowerSelectionHudConfig
        {
            Layout = new HudLayout
            {
                MaxTowers = 4,
                SquareSize = 48,
                SpacingBetweenSquares = 8,
                BottomMargin = 20,
                BorderWidth = 2
            },
            Styling = new HudStyling
            {
                DefaultBorderColor = "#444444",
                SelectedBorderColor = "#00FF00",
                HoverBorderColor = "#FFFF00",
                BackgroundColor = "#000000AA",
                NumberTextColor = "#FFFFFF",
                NumberFontSize = 14
            }
        };
    }

    private static bool IsInGodotRuntime()
    {
        try
        {
            return Godot.Engine.IsEditorHint() || !Godot.Engine.IsEditorHint();
        }
        catch
        {
            return false;
        }
    }
}
