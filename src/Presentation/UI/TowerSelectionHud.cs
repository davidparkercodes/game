using Godot;
using Game.Application.UI.Configuration;
using Game.Infrastructure.UI.Services;
using Game.Infrastructure.Stats.Services;
using System.Collections.Generic;
using System.Linq;
using Game.Presentation.Player;
using Game.Presentation.UI;

namespace Game.Presentation.UI;

public partial class TowerSelectionHud : Control
{
    [Export] public HBoxContainer? HBoxContainer;
    private TowerSelectionHudConfigService _configService = null!;
    private Dictionary<string, Button> _towerButtons = new();
    private string? _selectedTowerKey = null;
    private const string LOG_PREFIX = "🏗️ [TOWER_HUD]";

    public override void _Ready()
    {
        GD.Print($"{LOG_PREFIX} Initializing TowerSelectionHud...");
        
        _configService = new TowerSelectionHudConfigService();
        HBoxContainer ??= GetNode<HBoxContainer>("HBoxContainer");

        InitializeTowerIcons();
        GD.Print($"{LOG_PREFIX} TowerSelectionHud initialized successfully");
    }

    private void InitializeTowerIcons()
    {
        var config = _configService.GetConfiguration();
        var layout = config.Layout;
        var styling = config.Styling;
        
        // Sort towers by display order
        var sortedTowers = config.Towers
            .OrderBy(kvp => kvp.Value.DisplayOrder)
            .ToList();
        
        foreach (var (towerKey, towerConfig) in sortedTowers)
        {
            var button = CreateTowerButton(towerKey, towerConfig, layout, styling);
            HBoxContainer?.AddChild(button);
            _towerButtons[towerKey] = button;
            
            GD.Print($"{LOG_PREFIX} Created button for {towerKey} (order: {towerConfig.DisplayOrder}, hotkey: {towerConfig.Hotkey})");
        }
    }
    
    private Button CreateTowerButton(string towerKey, TowerDisplayConfig towerConfig, HudLayout layout, HudStyling styling)
    {
        var button = new Button
        {
            CustomMinimumSize = new Vector2(layout.SquareSize, layout.SquareSize),
            TooltipText = $"{CapitalizeTowerName(towerKey)} (Press {towerConfig.Hotkey})",
            ExpandIcon = true
        };
        
        // Load icon based on whether region is specified
        if (!string.IsNullOrEmpty(towerConfig.IconRegion))
        {
            button.Icon = CreateTowerIconFromRegion(towerConfig.IconPath, towerConfig.IconRegion);
        }
        else
        {
            button.Icon = LoadTowerIcon(towerConfig.IconPath);
        }
        
        // Apply styling from config
        SetButtonStyling(button, styling, false);
        
        // Add hotkey number label overlay
        AddHotkeyLabel(button, towerConfig.Hotkey, styling);
        
        // Connect button press event
        button.Pressed += () => OnTowerButtonPressed(towerKey);
        
        // Connect hover events for visual feedback and tower info display
        button.MouseEntered += () => 
        {
            OnTowerButtonHover(towerKey, true);
            ShowTowerInfo(towerKey);
        };
        button.MouseExited += () => 
        {
            OnTowerButtonHover(towerKey, false);
            HideTowerInfo();
        };
        
        return button;
    }
    
    private void AddHotkeyLabel(Button button, string hotkey, HudStyling styling)
    {
        var label = new Label
        {
            Text = hotkey.ToUpper(),
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Bottom,
            AnchorLeft = 1.0f,
            AnchorTop = 1.0f,
            AnchorRight = 1.0f,
            AnchorBottom = 1.0f,
            OffsetLeft = -16,
            OffsetTop = -16,
            OffsetRight = -2,
            OffsetBottom = -2,
            MouseFilter = Control.MouseFilterEnum.Ignore // Allow button clicks through
        };
        
        // Style the hotkey label
        label.AddThemeColorOverride("font_color", Color.FromHtml(styling.NumberTextColor));
        label.AddThemeFontSizeOverride("font_size", styling.NumberFontSize);
        
        // Add subtle shadow for better readability
        label.AddThemeColorOverride("font_shadow_color", Colors.Black);
        label.AddThemeConstantOverride("shadow_offset_x", 1);
        label.AddThemeConstantOverride("shadow_offset_y", 1);
        
        button.AddChild(label);
    }
    
    private Texture2D? LoadTowerIcon(string iconPath)
    {
        var validatedPath = _configService.GetValidatedIconPath(iconPath);
        try
        {
            var texture = GD.Load<Texture2D>(validatedPath);
            if (texture == null)
            {
                GD.PrintErr($"{LOG_PREFIX} Failed to load icon texture from {validatedPath}");
            }
            return texture;
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LOG_PREFIX} Error loading icon from {validatedPath}: {ex.Message}");
            return null;
        }
    }
    
    private AtlasTexture? CreateTowerIconFromRegion(string iconPath, string iconRegion)
    {
        if (string.IsNullOrEmpty(iconRegion))
        {
            // Fallback to full texture if no region specified
            var fullTexture = LoadTowerIcon(iconPath);
            if (fullTexture == null) return null;
            
            var atlasTexture = new AtlasTexture();
            atlasTexture.Atlas = fullTexture;
            atlasTexture.Region = new Rect2(0, 0, fullTexture.GetWidth(), fullTexture.GetHeight());
            return atlasTexture;
        }
        
        try
        {
            var validatedPath = _configService.GetValidatedIconPath(iconPath);
            var baseTexture = GD.Load<Texture2D>(validatedPath);
            if (baseTexture == null)
            {
                GD.PrintErr($"{LOG_PREFIX} Failed to load base texture from {validatedPath}");
                return null;
            }
            
            // Parse region string "x,y,width,height"
            var regionParts = iconRegion.Split(',');
            if (regionParts.Length != 4)
            {
                GD.PrintErr($"{LOG_PREFIX} Invalid region format: {iconRegion}. Expected 'x,y,width,height'");
                return null;
            }
            
            if (!int.TryParse(regionParts[0], out var x) ||
                !int.TryParse(regionParts[1], out var y) ||
                !int.TryParse(regionParts[2], out var width) ||
                !int.TryParse(regionParts[3], out var height))
            {
                GD.PrintErr($"{LOG_PREFIX} Failed to parse region coordinates: {iconRegion}");
                return null;
            }
            
            var atlasTexture = new AtlasTexture();
            atlasTexture.Atlas = baseTexture;
            atlasTexture.Region = new Rect2(x, y, width, height);
            
            return atlasTexture;
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LOG_PREFIX} Error creating atlas texture: {ex.Message}");
            return null;
        }
    }
    
    private void SetButtonStyling(Button button, HudStyling styling, bool isSelected)
    {
        var config = _configService.GetConfiguration();
        var borderWidth = config.Layout.BorderWidth;
        
        // Create enhanced StyleBoxFlat for the button
        var styleBox = new StyleBoxFlat();
        
        // Set border color based on state
        if (isSelected)
        {
            styleBox.BorderColor = Color.FromHtml(styling.SelectedBorderColor);
        }
        else
        {
            styleBox.BorderColor = Color.FromHtml(styling.DefaultBorderColor);
        }
        
        // Enhanced styling
        styleBox.BgColor = Color.FromHtml(styling.BackgroundColor);
        styleBox.BorderWidthTop = borderWidth;
        styleBox.BorderWidthBottom = borderWidth;
        styleBox.BorderWidthLeft = borderWidth;
        styleBox.BorderWidthRight = borderWidth;
        
        // Add subtle corner rounding for better appearance
        styleBox.CornerRadiusTopLeft = 2;
        styleBox.CornerRadiusTopRight = 2;
        styleBox.CornerRadiusBottomLeft = 2;
        styleBox.CornerRadiusBottomRight = 2;
        
        // Apply to all button states for consistency
        button.AddThemeStyleboxOverride("normal", styleBox);
        button.AddThemeStyleboxOverride("pressed", styleBox);
        button.AddThemeStyleboxOverride("hover", styleBox);
        button.AddThemeStyleboxOverride("focus", styleBox);
    }
    
    private string CapitalizeTowerName(string towerKey)
    {
        return towerKey.Replace("_", " ")
            .Split(' ')
            .Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower())
            .Aggregate((a, b) => a + " " + b);
    }
    
    private void OnTowerButtonPressed(string towerKey)
    {
        GD.Print($"{LOG_PREFIX} Tower button {towerKey} pressed");
        
        // Toggle selection if same tower is clicked
        if (_selectedTowerKey == towerKey)
        {
            ClearSelection();
            var player = GetTree().GetFirstNodeInGroup("player") as Game.Presentation.Player.Player;
            player?.ClearBuildingSelection();
        }
        else
        {
            SetSelectedTower(towerKey);
            var player = GetTree().GetFirstNodeInGroup("player") as Game.Presentation.Player.Player;
            var displayName = GetDisplayNameFromTowerKey(towerKey);
            player?.SelectBuilding(displayName);
        }
    }
    
    private string GetDisplayNameFromTowerKey(string towerKey)
    {
        return towerKey switch
        {
            "basic_tower" => "Basic",
            "sniper_tower" => "Sniper",
            "rapid_tower" => "Rapid",
            "heavy_tower" => "Heavy",
            _ => CapitalizeTowerName(towerKey)
        };
    }
    
    private void OnTowerButtonHover(string towerKey, bool isHovering)
    {
        if (!_towerButtons.TryGetValue(towerKey, out var button) || towerKey == _selectedTowerKey)
            return;
            
        var config = _configService.GetConfiguration();
        var styling = config.Styling;
        var borderWidth = config.Layout.BorderWidth;
        
        // Create enhanced hover styling
        var styleBox = new StyleBoxFlat();
        styleBox.BgColor = Color.FromHtml(styling.BackgroundColor);
        styleBox.BorderWidthTop = borderWidth;
        styleBox.BorderWidthBottom = borderWidth;
        styleBox.BorderWidthLeft = borderWidth;
        styleBox.BorderWidthRight = borderWidth;
        
        // Add corner rounding for consistency
        styleBox.CornerRadiusTopLeft = 2;
        styleBox.CornerRadiusTopRight = 2;
        styleBox.CornerRadiusBottomLeft = 2;
        styleBox.CornerRadiusBottomRight = 2;
        
        if (isHovering)
        {
            styleBox.BorderColor = Color.FromHtml(styling.HoverBorderColor);
            // Subtle background highlight on hover
            var hoverBgColor = Color.FromHtml(styling.BackgroundColor);
            hoverBgColor.A *= 1.2f; // Slight transparency increase
            styleBox.BgColor = hoverBgColor;
        }
        else
        {
            styleBox.BorderColor = Color.FromHtml(styling.DefaultBorderColor);
        }
        
        button.AddThemeStyleboxOverride("normal", styleBox);
        button.AddThemeStyleboxOverride("hover", styleBox);
    }
    
    public void SetSelectedTower(string? towerKey)
    {
        // Clear previous selection
        if (_selectedTowerKey != null && _towerButtons.TryGetValue(_selectedTowerKey, out var oldButton))
        {
            var config = _configService.GetConfiguration();
            SetButtonStyling(oldButton, config.Styling, false);
        }
        
        _selectedTowerKey = towerKey;
        
        // Apply selection styling to new button
        if (towerKey != null && _towerButtons.TryGetValue(towerKey, out var newButton))
        {
            var config = _configService.GetConfiguration();
            SetButtonStyling(newButton, config.Styling, true);
            GD.Print($"{LOG_PREFIX} Selected tower: {towerKey}");
        }
    }
    
    public void ClearSelection()
    {
        SetSelectedTower(null);
        GD.Print($"{LOG_PREFIX} Cleared tower selection");
    }
    
    private void ShowTowerInfo(string towerKey)
    {
        try
        {
            var displayName = GetDisplayNameFromTowerKey(towerKey);
            var towerStats = GetTowerStats(displayName);
            
            if (towerStats != null && HudManager.Instance != null && HudManager.Instance.IsInitialized())
            {
                HudManager.Instance.ShowTowerStats(
                    $"{displayName} Tower",
                    towerStats.Value.Cost,
                    towerStats.Value.Damage,
                    towerStats.Value.Range,
                    towerStats.Value.AttackSpeed
                );
                GD.Print($"{LOG_PREFIX} Showing stats for {displayName} tower");
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LOG_PREFIX} Error showing tower info: {ex.Message}");
        }
    }
    
    private void HideTowerInfo()
    {
        try
        {
            if (HudManager.Instance != null && HudManager.Instance.IsInitialized())
            {
                HudManager.Instance.HideTowerStats();
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LOG_PREFIX} Error hiding tower info: {ex.Message}");
        }
    }
    
    private (int Cost, int Damage, float Range, float AttackSpeed)? GetTowerStats(string displayName)
    {
        try
        {
            // Map display names to config keys using domain entity ConfigKey constants
            string? configKey = displayName.ToLower() switch
            {
                "basic" => Game.Domain.Buildings.Entities.BasicTower.ConfigKey,
                "sniper" => Game.Domain.Buildings.Entities.SniperTower.ConfigKey,
                "rapid" => Game.Domain.Buildings.Entities.RapidTower.ConfigKey,
                "heavy" => Game.Domain.Buildings.Entities.HeavyTower.ConfigKey,
                _ => null
            };
            
            if (configKey == null) return null;
            
            // Get stats from the configuration service
            if (StatsManagerService.Instance != null)
            {
                var configStats = StatsManagerService.Instance.GetBuildingStats(configKey);
                return (configStats.cost, configStats.damage, configStats.range, configStats.attack_speed);
            }
            
            return null;
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LOG_PREFIX} Error getting tower stats for {displayName}: {ex.Message}");
            return null;
        }
    }
    
    // Hotkey input is handled by Player class and communicated via NotifyHudSelectionChange
    // This prevents double processing of the same input events
}

