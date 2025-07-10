using Godot;
using Game.Application.UI.Configuration;
using Game.Infrastructure.UI.Services;
using Game.Infrastructure.Stats.Services;
using System.Collections.Generic;
using System.Linq;
using Game.Presentation.Player;
using Game.Presentation.UI;
using Game.Application.UI.Services;
using Game.Infrastructure.Audio.Services;
using Game.Domain.Audio.Enums;

namespace Game.Presentation.UI;

public partial class BuildingSelectionHud : Control
{
    [Export] public HBoxContainer? HBoxContainer;
    private BuildingSelectionHudConfigService _configService = null!;
    private Dictionary<string, Button> _buildingButtons = new();
    private string? _selectedBuildingKey = null;
    private const string LOG_PREFIX = "🏗️ [BUILDING_HUD]";

    public override void _Ready()
    {
        GD.Print($"{LOG_PREFIX} Initializing BuildingSelectionHud...");
        
        _configService = new BuildingSelectionHudConfigService();
        HBoxContainer ??= GetNode<HBoxContainer>("HBoxContainer");

        InitializeBuildingIcons();
        GD.Print($"{LOG_PREFIX} BuildingSelectionHud initialized successfully");
    }

    private void InitializeBuildingIcons()
    {
        var config = _configService.GetConfiguration();
        var layout = config.Layout;
        var styling = config.Styling;
        
        // Sort buildings by display order
        var sortedBuildings = config.Buildings
            .OrderBy(kvp => kvp.Value.DisplayOrder)
            .ToList();
        
        foreach (var (buildingKey, buildingConfig) in sortedBuildings)
        {
            var button = CreateBuildingButton(buildingKey, buildingConfig, layout, styling);
            HBoxContainer?.AddChild(button);
            _buildingButtons[buildingKey] = button;
            
            GD.Print($"{LOG_PREFIX} Created button for {buildingKey} (order: {buildingConfig.DisplayOrder}, hotkey: {buildingConfig.Hotkey})");
        }
    }
    
    private Button CreateBuildingButton(string buildingKey, BuildingDisplayConfig buildingConfig, HudLayout layout, HudStyling styling)
    {
        var button = new Button
        {
            CustomMinimumSize = new Vector2(layout.SquareSize, layout.SquareSize),
            TooltipText = $"{CapitalizeBuildingName(buildingKey)} (Press {buildingConfig.Hotkey})",
            ExpandIcon = true
        };
        
        // Load icon based on whether region is specified
        if (!string.IsNullOrEmpty(buildingConfig.IconRegion))
        {
            button.Icon = CreateBuildingIconFromRegion(buildingConfig.IconPath, buildingConfig.IconRegion);
        }
        else
        {
            button.Icon = LoadBuildingIcon(buildingConfig.IconPath);
        }
        
        // Apply styling from config
        SetButtonStyling(button, styling, false);
        
        // Add hotkey number label overlay
        AddHotkeyLabel(button, buildingConfig.Hotkey, styling);
        
        // Connect button press event
        button.Pressed += () => OnBuildingButtonPressed(buildingKey);
        
        // Connect hover events for visual feedback and building info display
        button.MouseEntered += () => 
        {
            OnBuildingButtonHover(buildingKey, true);
            ShowBuildingInfo(buildingKey);
        };
        button.MouseExited += () => 
        {
            OnBuildingButtonHover(buildingKey, false);
            HideBuildingInfo();
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
            OffsetLeft = styling.HotkeyPositioning.OffsetLeft,
            OffsetTop = styling.HotkeyPositioning.OffsetTop,
            OffsetRight = styling.HotkeyPositioning.OffsetRight,
            OffsetBottom = styling.HotkeyPositioning.OffsetBottom,
            MouseFilter = Control.MouseFilterEnum.Ignore // Allow button clicks through
        };
        
        // Style the hotkey label
        label.AddThemeColorOverride("font_color", Color.FromHtml(styling.NumberTextColor));
        label.AddThemeFontSizeOverride("font_size", styling.NumberFontSize);
        
        // Add subtle shadow for better readability
        label.AddThemeColorOverride("font_shadow_color", Colors.Black);
        label.AddThemeConstantOverride("shadow_offset_x", styling.TextShadow.OffsetX);
        label.AddThemeConstantOverride("shadow_offset_y", styling.TextShadow.OffsetY);
        
        button.AddChild(label);
    }
    
    private Texture2D? LoadBuildingIcon(string iconPath)
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
    
    private AtlasTexture? CreateBuildingIconFromRegion(string iconPath, string iconRegion)
    {
        if (string.IsNullOrEmpty(iconRegion))
        {
            // Fallback to full texture if no region specified
            var fullTexture = LoadBuildingIcon(iconPath);
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
        
        // Add corner rounding based on configuration
        styleBox.CornerRadiusTopLeft = styling.ButtonCornerRadius;
        styleBox.CornerRadiusTopRight = styling.ButtonCornerRadius;
        styleBox.CornerRadiusBottomLeft = styling.ButtonCornerRadius;
        styleBox.CornerRadiusBottomRight = styling.ButtonCornerRadius;
        
        // Apply to all button states for consistency
        button.AddThemeStyleboxOverride("normal", styleBox);
        button.AddThemeStyleboxOverride("pressed", styleBox);
        button.AddThemeStyleboxOverride("hover", styleBox);
        button.AddThemeStyleboxOverride("focus", styleBox);
    }
    
    private string CapitalizeBuildingName(string buildingKey)
    {
        return buildingKey.Replace("_", " ")
            .Split(' ')
            .Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower())
            .Aggregate((a, b) => a + " " + b);
    }
    
    private void OnBuildingButtonPressed(string buildingKey)
    {
        GD.Print($"{LOG_PREFIX} Building button {buildingKey} pressed");
        
        // Toggle selection if same building is clicked
        if (_selectedBuildingKey == buildingKey)
        {
            PlayDeselectionSound();
            ClearSelection();
            var player = GetTree().GetFirstNodeInGroup("player") as Game.Presentation.Player.Player;
            player?.ClearBuildingSelection();
        }
        else
        {
            PlaySelectionSound();
            SetSelectedBuilding(buildingKey);
            var player = GetTree().GetFirstNodeInGroup("player") as Game.Presentation.Player.Player;
            var displayName = GetDisplayNameFromBuildingKey(buildingKey);
            player?.SelectBuilding(displayName);
        }
    }
    
    private string GetDisplayNameFromBuildingKey(string buildingKey)
    {
        // Use config-driven approach - get display name from configuration
        var config = _configService.GetConfiguration();
        if (config.Buildings.TryGetValue(buildingKey, out var buildingConfig))
        {
            // Could add display_name field to config, for now capitalize the key
            return CapitalizeBuildingName(buildingKey);
        }
        
        return CapitalizeBuildingName(buildingKey);
    }
    
    private void OnBuildingButtonHover(string buildingKey, bool isHovering)
    {
        if (!_buildingButtons.TryGetValue(buildingKey, out var button) || buildingKey == _selectedBuildingKey)
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
        styleBox.CornerRadiusTopLeft = styling.ButtonCornerRadius;
        styleBox.CornerRadiusTopRight = styling.ButtonCornerRadius;
        styleBox.CornerRadiusBottomLeft = styling.ButtonCornerRadius;
        styleBox.CornerRadiusBottomRight = styling.ButtonCornerRadius;
        
        if (isHovering)
        {
            styleBox.BorderColor = Color.FromHtml(styling.HoverBorderColor);
            // Subtle background highlight on hover
            var hoverBgColor = Color.FromHtml(styling.BackgroundColor);
            hoverBgColor.A *= styling.HoverTransparencyMultiplier; // Configurable transparency multiplier
            styleBox.BgColor = hoverBgColor;
        }
        else
        {
            styleBox.BorderColor = Color.FromHtml(styling.DefaultBorderColor);
        }
        
        button.AddThemeStyleboxOverride("normal", styleBox);
        button.AddThemeStyleboxOverride("hover", styleBox);
    }
    
    public void SetSelectedBuilding(string? buildingKey)
    {
        // Clear previous selection
        if (_selectedBuildingKey != null && _buildingButtons.TryGetValue(_selectedBuildingKey, out var oldButton))
        {
            var config = _configService.GetConfiguration();
            SetButtonStyling(oldButton, config.Styling, false);
        }
        
        _selectedBuildingKey = buildingKey;
        
        // Apply selection styling to new button
        if (buildingKey != null && _buildingButtons.TryGetValue(buildingKey, out var newButton))
        {
            var config = _configService.GetConfiguration();
            SetButtonStyling(newButton, config.Styling, true);
            GD.Print($"{LOG_PREFIX} Selected building: {buildingKey}");
        }
    }
    
    public void ClearSelection()
    {
        SetSelectedBuilding(null);
        GD.Print($"{LOG_PREFIX} Cleared building selection");
    }
    
    private void ShowBuildingInfo(string buildingKey)
    {
        try
        {
            var displayName = GetDisplayNameFromBuildingKey(buildingKey);
            var buildingStats = GetBuildingStats(buildingKey);
            
            if (buildingStats != null && HudManager.Instance != null && HudManager.Instance.IsInitialized())
            {
                HudManager.Instance.ShowTowerStats(
                    $"{displayName} Tower",
                    buildingStats.Value.Cost,
                    buildingStats.Value.Damage,
                    buildingStats.Value.Range,
                    buildingStats.Value.AttackSpeed
                );
                GD.Print($"{LOG_PREFIX} Showing stats for {displayName} building");
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LOG_PREFIX} Error showing building info: {ex.Message}");
        }
    }
    
    private void HideBuildingInfo()
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
            GD.PrintErr($"{LOG_PREFIX} Error hiding building info: {ex.Message}");
        }
    }
    
    private (int Cost, int Damage, float Range, float AttackSpeed)? GetBuildingStats(string buildingKey)
    {
        try
        {
            // Use the buildingKey directly - it should match the config key in building stats
            if (StatsManagerService.Instance != null)
            {
                var configStats = StatsManagerService.Instance.GetBuildingStats(buildingKey);
                return (configStats.cost, configStats.damage, configStats.range, configStats.attack_speed);
            }
            
            return null;
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LOG_PREFIX} Error getting building stats for {buildingKey}: {ex.Message}");
            return null;
        }
    }
    
    // Hotkey input is handled by Player class and communicated via NotifyHudSelectionChange
    // This prevents double processing of the same input events
    
    private void PlaySelectionSound()
    {
        try
        {
            var config = _configService.GetConfiguration();
            if (config.Audio.Enabled && SoundManagerService.Instance != null)
            {
                SoundManagerService.Instance.PlaySound("tower_select", SoundCategory.UI);
                GD.Print($"{LOG_PREFIX} Played selection sound");
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LOG_PREFIX} Error playing selection sound: {ex.Message}");
        }
    }
    
    private void PlayDeselectionSound()
    {
        try
        {
            var config = _configService.GetConfiguration();
            if (config.Audio.Enabled && SoundManagerService.Instance != null)
            {
                SoundManagerService.Instance.PlaySound("tower_deselect", SoundCategory.UI);
                GD.Print($"{LOG_PREFIX} Played deselection sound");
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LOG_PREFIX} Error playing deselection sound: {ex.Message}");
        }
    }
}

