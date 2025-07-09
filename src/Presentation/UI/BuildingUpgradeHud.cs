using Godot;
using Game.Presentation.Buildings;
using Game.Infrastructure.Stats.Services;
using Game.Infrastructure.Audio.Services;
using Game.Domain.Audio.Enums;
using Game.Domain.Buildings.Services;
using Game.Application.Buildings.Services;

namespace Game.Presentation.UI;

public partial class BuildingUpgradeHud : Control
{
    private Building? _selectedBuilding = null;
    private const string LogPrefix = "🏗️ [UPGRADE-HUD]";
    
    // Services
    private readonly ITowerUpgradeService _upgradeService = new TowerUpgradeService();
    private readonly ITowerSellService _sellService = new TowerSellService();
    
    // UI Node References
    private Label _towerNameLabel = null!;
    private Label _towerDescriptionLabel = null!;
    private TextureRect _towerImage = null!;
    
    // Current Stats Labels (split into label and value)
    private Label _costLabel = null!;
    private RichTextLabel _costValue = null!;
    private Label _damageLabel = null!;
    private RichTextLabel _damageValue = null!;
    private Label _rangeLabel = null!;
    private RichTextLabel _rangeValue = null!;
    private Label _attackSpeedLabel = null!;
    private RichTextLabel _attackSpeedValue = null!;
    
    
    // Buttons
    private Button _upgradeButton = null!;
    private Button _sellButton = null!;
    private Button _closeButton = null!;
    
    // Modal Background
    private Panel _modalBackground = null!;
    
    public override void _Ready()
    {
        GD.Print($"{LogPrefix} Initializing BuildingUpgradeHud...");
        
        // Cache UI node references
        CacheUiNodeReferences();
        
        // Connect button signals
        ConnectButtonSignals();
        
        // Connect modal background for outside clicks
        ConnectModalBackground();
        
        // Hide by default
        Hide();
        
        GD.Print($"{LogPrefix} BuildingUpgradeHud initialized successfully");
    }
    
    private void CacheUiNodeReferences()
    {
        // Main UI elements
        _towerNameLabel = GetNode<Label>("MainPanel/VBoxContainer/TowerNameLabel");
        _towerDescriptionLabel = null!; // Removed from new layout
        _towerImage = null!; // Removed from new layout
        
        // Current stats (split labels)
        _costLabel = GetNode<Label>("MainPanel/VBoxContainer/CurrentStatsContainer/CostContainer/CostLabel");
        _costValue = GetNode<RichTextLabel>("MainPanel/VBoxContainer/CurrentStatsContainer/CostContainer/CostValue");
        _damageLabel = GetNode<Label>("MainPanel/VBoxContainer/CurrentStatsContainer/DamageContainer/DamageLabel");
        _damageValue = GetNode<RichTextLabel>("MainPanel/VBoxContainer/CurrentStatsContainer/DamageContainer/DamageValue");
        _rangeLabel = GetNode<Label>("MainPanel/VBoxContainer/CurrentStatsContainer/RangeContainer/RangeLabel");
        _rangeValue = GetNode<RichTextLabel>("MainPanel/VBoxContainer/CurrentStatsContainer/RangeContainer/RangeValue");
        _attackSpeedLabel = GetNode<Label>("MainPanel/VBoxContainer/CurrentStatsContainer/AttackSpeedContainer/AttackSpeedLabel");
        _attackSpeedValue = GetNode<RichTextLabel>("MainPanel/VBoxContainer/CurrentStatsContainer/AttackSpeedContainer/AttackSpeedValue");
        
        // Buttons
        _upgradeButton = GetNode<Button>("MainPanel/VBoxContainer/UpgradeButton");
        _sellButton = GetNode<Button>("MainPanel/VBoxContainer/SellButton");
        _closeButton = GetNode<Button>("MainPanel/CloseButton");
        
        // No modal background in new layout
        _modalBackground = null!;
    }
    
    private void ConnectButtonSignals()
    {
        _upgradeButton.Pressed += OnUpgradeButtonPressed;
        _sellButton.Pressed += OnSellButtonPressed;
        _closeButton.Pressed += OnCloseButtonPressed;
    }
    
    private void ConnectModalBackground()
    {
        // No modal background in new layout - skip this step
    }
    
    public void ShowForBuilding(Building building)
    {
        if (building == null)
        {
            GD.PrintErr($"{LogPrefix} Cannot show HUD for null building");
            return;
        }
        
        _selectedBuilding = building;
        
        // Update all UI elements with building data
        UpdateBuildingInfo();
        UpdateCurrentStats();
        UpdateUpgradeButton();
        UpdateSellInfo();
        
        // Show the HUD
        Show();
        
        // Play open sound
        PlayModalOpenSound();
        
        GD.Print($"{LogPrefix} Showing upgrade HUD for building {building.Name}");
    }
    
    public void HideHud()
    {
        if (_selectedBuilding == null) return;
        
        // Play close sound
        PlayModalCloseSound();
        
        // Clear selection
        _selectedBuilding = null;
        
        // Hide the HUD
        Hide();
        
        GD.Print($"{LogPrefix} Hiding upgrade HUD");
    }
    
    private void UpdateBuildingInfo()
    {
        if (_selectedBuilding == null) return;
        
        // Get building type name (extract from class name)
        string buildingTypeName = GetBuildingTypeName(_selectedBuilding);
        
        // Update tower name on separate lines
        _towerNameLabel.Text = $"{buildingTypeName} Tower\n(Level {_selectedBuilding.UpgradeLevel})";
        
        // Skip description and image updates since they're not in the new layout
    }
    
    private void UpdateCurrentStats()
    {
        if (_selectedBuilding == null) return;
        
        // Get upgrade preview for cost and stats
        var upgradeStats = _upgradeService.GetUpgradePreview(_selectedBuilding);
        bool canUpgrade = !_upgradeService.IsAtMaxLevel(_selectedBuilding);
        
        // Update cost with upgrade cost preview (keep lightning bolts for cost)
        if (canUpgrade)
        {
            int upgradeCost = _upgradeService.GetUpgradeCost(_selectedBuilding);
            _costValue.Text = $"[color=teal]⚡{_selectedBuilding.Cost}[/color] [color=white]+[/color] [color=#ff66cc]⚡{upgradeCost}[/color]";
        }
        else
        {
            _costValue.Text = $"[color=teal]⚡{_selectedBuilding.Cost}[/color] [color=white](MAX)[/color]";
        }
        
        // Update stats without lightning bolts
        if (canUpgrade)
        {
            _damageValue.Text = $"[color=teal]{_selectedBuilding.Damage}[/color] [color=white]→[/color] [color=#ff66cc]{upgradeStats.damage}[/color]";
            _rangeValue.Text = $"[color=teal]{_selectedBuilding.Range:F0}[/color] [color=white]→[/color] [color=#ff66cc]{upgradeStats.range:F0}[/color]";
            _attackSpeedValue.Text = $"[color=teal]{_selectedBuilding.AttackSpeed:F0}[/color] [color=white]→[/color] [color=#ff66cc]{upgradeStats.attackSpeed:F0}[/color]";
        }
        else
        {
            // No arrows at max level
            _damageValue.Text = $"[color=teal]{_selectedBuilding.Damage}[/color] [color=white](MAX)[/color]";
            _rangeValue.Text = $"[color=teal]{_selectedBuilding.Range:F0}[/color] [color=white](MAX)[/color]";
            _attackSpeedValue.Text = $"[color=teal]{_selectedBuilding.AttackSpeed:F0}[/color] [color=white](MAX)[/color]";
        }
    }
    
    private void UpdateUpgradeButton()
    {
        if (_selectedBuilding == null) return;
        
        // Check if building can be upgraded
        if (_upgradeService.IsAtMaxLevel(_selectedBuilding))
        {
            _upgradeButton.Text = "Max Level";
            _upgradeButton.Disabled = true;
            return;
        }
        
        // Get upgrade cost and update button
        int upgradeCost = _upgradeService.GetUpgradeCost(_selectedBuilding);
        _upgradeButton.Text = $"Upgrade (⚡{upgradeCost})";
        
        // Disable button if player can't afford upgrade
        _upgradeButton.Disabled = !_upgradeService.CanUpgrade(_selectedBuilding);
    }
    
    private void UpdateSellInfo()
    {
        if (_selectedBuilding == null) return;
        
        // Get sell value from service
        int sellValue = _sellService.GetSellValue(_selectedBuilding);
        
        // Update sell button text (no sell value label anymore)
        _sellButton.Text = $"Sell Tower (⚡{sellValue})";
    }
    
    private void UpdateTowerImage(string buildingTypeName)
    {
        // TODO: Load appropriate tower image based on building type
        // For now, we'll leave it as placeholder
        // This will be implemented in Phase 2.4 when we add configuration support
    }
    
    // ===== BUTTON EVENT HANDLERS =====
    
    private void OnUpgradeButtonPressed()
    {
        if (_selectedBuilding == null) return;
        
        GD.Print($"{LogPrefix} Upgrade button pressed for {_selectedBuilding.Name}");
        
        // Attempt to upgrade the building
        if (_upgradeService.UpgradeBuilding(_selectedBuilding))
        {
            // Upgrade successful
            PlayUpgradeSound();
            
            // Update the HUD to reflect new stats - keep it open to show the changes
            UpdateCurrentStats();
            UpdateUpgradeButton();
            UpdateSellInfo();
            
            GD.Print($"{LogPrefix} Successfully upgraded {_selectedBuilding.Name} to level {_selectedBuilding.UpgradeLevel}");
            
            // Don't close HUD - let player see the stat changes
        }
        else
        {
            // Upgrade failed (not enough money or at max level)
            GD.Print($"{LogPrefix} Failed to upgrade {_selectedBuilding.Name}");
        }
    }
    
    private void OnSellButtonPressed()
    {
        if (_selectedBuilding == null) return;
        
        GD.Print($"{LogPrefix} Sell button pressed for {_selectedBuilding.Name}");
        
        // Store building reference before selling (since it will be destroyed)
        var buildingName = _selectedBuilding.Name;
        var sellValue = _sellService.GetSellValue(_selectedBuilding);
        
        // Attempt to sell the building
        if (_sellService.SellBuilding(_selectedBuilding))
        {
            // Sell successful
            PlaySellSound();
            
            GD.Print($"{LogPrefix} Successfully sold {buildingName} for ${sellValue}");
            
            // Close HUD after sell (building is destroyed)
            HideHud();
        }
        else
        {
            // Sell failed (shouldn't happen, but handle gracefully)
            GD.PrintErr($"{LogPrefix} Failed to sell {buildingName}");
        }
    }
    
    private void OnCloseButtonPressed()
    {
        GD.Print($"{LogPrefix} Close button pressed");
        HideHud();
    }
    
    private void OnModalBackgroundInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
        {
            if (mouseButton.ButtonIndex == MouseButton.Left)
            {
                // Click outside - close the HUD
                GD.Print($"{LogPrefix} Click outside detected - closing HUD");
                HideHud();
            }
        }
    }
    
    // ===== KEYBOARD INPUT HANDLING =====
    
    public override void _Input(InputEvent @event)
    {
        if (!Visible) return;
        
        // Handle mouse clicks outside the HUD panel to close it
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
        {
            // Check if click is outside the main panel
            var mainPanel = GetNode<Panel>("MainPanel");
            var panelRect = mainPanel.GetGlobalRect();
            
            if (!panelRect.HasPoint(mouseEvent.GlobalPosition))
            {
                GD.Print($"{LogPrefix} Click outside HUD panel at {mouseEvent.GlobalPosition} - closing HUD and deselecting building");
                
                // Deselect the currently selected building
                BuildingSelectionManager.Instance.DeselectCurrentBuilding();
                
                GetViewport().SetInputAsHandled();
            }
        }
        
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            switch (keyEvent.Keycode)
            {
                case Key.Escape:
                    HideHud();
                    GetViewport().SetInputAsHandled();
                    break;
                    
                case Key.U:
                    if (!_upgradeButton.Disabled)
                    {
                        OnUpgradeButtonPressed();
                        GetViewport().SetInputAsHandled();
                    }
                    break;
                    
                case Key.S:
                    OnSellButtonPressed();
                    GetViewport().SetInputAsHandled();
                    break;
            }
        }
    }
    
    // ===== HELPER METHODS =====
    
    private string GetBuildingTypeName(Building building)
    {
        // Extract building type from class name
        string className = building.GetType().Name;
        
        // Remove "Tower" suffix if present
        if (className.EndsWith("Tower"))
        {
            className = className[..^5]; // Remove last 5 characters ("Tower")
        }
        
        return className;
    }
    
    private string GetBuildingDescription(string buildingTypeName)
    {
        // TODO: Load descriptions from configuration
        // For now, use simple descriptions
        return buildingTypeName.ToLower() switch
        {
            "basic" => "A reliable all-around tower with balanced stats",
            "sniper" => "Long-range tower with high damage but slow attack speed",
            "rapid" => "Fast-firing tower with lower damage per shot",
            "heavy" => "Powerful tower with high damage and strong impact",
            _ => "A powerful defensive tower"
        };
    }
    
    private string GetBuildingConfigKey(Building building)
    {
        // Extract building type from class name and convert to config key
        string className = building.GetType().Name;
        
        // Convert from PascalCase to snake_case
        return className.ToLower() switch
        {
            "basictower" => "basic_tower",
            "snipertower" => "sniper_tower",
            "rapidtower" => "rapid_tower",
            "heavytower" => "heavy_tower",
            _ => "basic_tower" // fallback
        };
    }
    
    // ===== AUDIO METHODS =====
    
    private void PlayModalOpenSound()
    {
        try
        {
            if (SoundManagerService.Instance != null)
            {
                SoundManagerService.Instance.PlaySound("ui_modal_open", SoundCategory.UI);
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix} Error playing modal open sound: {ex.Message}");
        }
    }
    
    private void PlayModalCloseSound()
    {
        try
        {
            if (SoundManagerService.Instance != null)
            {
                SoundManagerService.Instance.PlaySound("ui_modal_close", SoundCategory.UI);
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix} Error playing modal close sound: {ex.Message}");
        }
    }
    
    private void PlayUpgradeSound()
    {
        try
        {
            if (SoundManagerService.Instance != null)
            {
                SoundManagerService.Instance.PlaySound("tower_upgrade", SoundCategory.SFX);
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix} Error playing upgrade sound: {ex.Message}");
        }
    }
    
    private void PlaySellSound()
    {
        try
        {
            if (SoundManagerService.Instance != null)
            {
                SoundManagerService.Instance.PlaySound("tower_sell", SoundCategory.SFX);
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"{LogPrefix} Error playing sell sound: {ex.Message}");
        }
    }
}
