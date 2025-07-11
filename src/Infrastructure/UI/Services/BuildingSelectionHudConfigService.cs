using System;
using System.Text.Json;
using Game.Application.UI.Configuration;
using Game.Application.UI.Services;
using Godot;

namespace Game.Infrastructure.UI.Services;

public class BuildingSelectionHudConfigService : IBuildingSelectionHudConfigService
{
    private const string CONFIG_PATH = "res://config/ui/building_selection_hud.json";
    private const string DEFAULT_ICON_PATH = "res://assets/sprites/ui/default_tower_icon.png";
    
    private BuildingSelectionHudConfig? _cachedConfig;
    private bool _configLoaded = false;

    public BuildingSelectionHudConfig GetConfiguration()
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
                   config.Layout.MaxBuildings > 0 && 
                   config.Buildings.Count > 0 &&
                   IsAudioConfigurationValid(config.Audio);
        }
        catch
        {
            return false;
        }
    }

    private bool IsAudioConfigurationValid(HudAudio audio)
    {
        if (audio == null) return false;
        
        // If audio is disabled, we don't need to validate paths
        if (!audio.Enabled) return true;
        
        // If enabled, validate that audio files exist
        return DoesAudioFileExist(audio.SelectSoundPath) &&
               DoesAudioFileExist(audio.DeselectSoundPath);
    }

    private bool DoesAudioFileExist(string audioPath)
    {
        try
        {
            if (string.IsNullOrEmpty(audioPath)) return false;
            
            if (IsInGodotRuntime())
            {
                return Godot.FileAccess.FileExists(audioPath);
            }
            return false;
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
        
        GD.PrintErr($"⚠️ BuildingSelectionHudConfigService: Icon not found at {iconPath}, using default");
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
                    
                    _cachedConfig = JsonSerializer.Deserialize<BuildingSelectionHudConfig>(jsonContent, options);
                    GD.Print($"✅ BuildingSelectionHudConfigService: Configuration loaded from {CONFIG_PATH}");
                }
            }
            else
            {
                GD.PrintErr($"⚠️ BuildingSelectionHudConfigService: Config file not found at {CONFIG_PATH}, using defaults");
                _cachedConfig = CreateDefaultConfiguration();
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"❌ BuildingSelectionHudConfigService: Error loading config: {ex.Message}");
            _cachedConfig = CreateDefaultConfiguration();
        }
        finally
        {
            _configLoaded = true;
        }
    }

    private BuildingSelectionHudConfig CreateDefaultConfiguration()
    {
        return new BuildingSelectionHudConfig
        {
            Layout = new HudLayout
            {
                MaxBuildings = 4,
                SquareSize = 24,
                SpacingBetweenSquares = 4,
                BottomMargin = 10,
                BorderWidth = 1
            },
            Styling = new HudStyling
            {
                DefaultBorderColor = "#444444",
                SelectedBorderColor = "#00FF00",
                HoverBorderColor = "#FFFF00",
                BackgroundColor = "#000000AA",
                NumberTextColor = "#FFFFFF",
                NumberFontSize = 10
            },
            Audio = new HudAudio
            {
                SelectSoundPath = "res://assets/audio/towers/tower_select.mp3",
                DeselectSoundPath = "res://assets/audio/towers/tower_deselect.mp3",
                Enabled = true
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
