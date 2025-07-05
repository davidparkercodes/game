using Godot;
using Game.Infrastructure.Game.Services;

namespace Game.Presentation.UI;

public partial class HudManager : Node
{
    public static HudManager? Instance { get; private set; }
    private Hud? _hud;

    public override void _Ready()
    {
        Instance = this;
        GD.Print("🎯 HudManager singleton initialized");
    }

    public void Initialize(Hud hud)
    {
        _hud = hud;
        GD.Print("🎯 HudManager connected to HUD instance");
        UpdateInitialValues();
    }

    private void UpdateInitialValues()
    {
        if (_hud == null)
        {
            GD.PrintErr("❌ Cannot update initial values - HUD is null");
            return;
        }

        // Get initial values from GameService
        var gameService = GameService.Instance;
        if (gameService != null)
        {
            _hud.UpdateMoney(gameService.Money);
            _hud.UpdateLives(gameService.Lives);
            _hud.UpdateWave(1); // Default to wave 1
            GD.Print($"🎯 HUD initialized with Money: {gameService.Money}, Lives: {gameService.Lives}");
        }
        else
        {
            // Fallback values
            _hud.UpdateMoney(500);
            _hud.UpdateLives(20);
            _hud.UpdateWave(1);
            GD.Print("🎯 HUD initialized with fallback values");
        }

        // Hide tower stats panel initially
        _hud.HideTowerStats();
        
        // Initialize wave button state
        SetWaveButtonState("Start Wave 1", true);
    }

    // Money Management
    public void UpdateMoney(int amount)
    {
        if (_hud != null)
        {
            _hud.UpdateMoney(amount);
            GD.Print($"💰 HUD Money updated: {amount}");
        }
        else
        {
            GD.PrintErr("❌ Cannot update money - HUD is null");
        }
    }

    // Lives Management
    public void UpdateLives(int lives)
    {
        if (_hud != null)
        {
            _hud.UpdateLives(lives);
            GD.Print($"❤️ HUD Lives updated: {lives}");
        }
        else
        {
            GD.PrintErr("❌ Cannot update lives - HUD is null");
        }
    }

    // Wave Management
    public void UpdateWave(int wave)
    {
        if (_hud != null)
        {
            _hud.UpdateWave(wave);
            GD.Print($"🌊 HUD Wave updated: {wave}");
        }
        else
        {
            GD.PrintErr("❌ Cannot update wave - HUD is null");
        }
    }

    // Building Stats Management
    public void ShowBuildingStats(string buildingName, int cost, int damage, float range, float fireRate)
    {
        if (_hud != null)
        {
            _hud.ShowBuildingStats(buildingName, cost, damage, range, fireRate);
            GD.Print($"🏗️ Showing building stats for: {buildingName}");
        }
        else
        {
            GD.PrintErr("❌ Cannot show building stats - HUD is null");
        }
    }

    public void HideBuildingStats()
    {
        if (_hud != null)
        {
            _hud.HideBuildingStats();
            GD.Print("🚫 Hiding building stats");
        }
        else
        {
            GD.PrintErr("❌ Cannot hide building stats - HUD is null");
        }
    }

    // Tower Stats Management (for compatibility)
    public void ShowTowerStats(string towerName, int cost, int damage, float range, float fireRate)
    {
        ShowBuildingStats(towerName, cost, damage, range, fireRate);
    }

    public void HideTowerStats()
    {
        HideBuildingStats();
    }

    // Skip Button Management
    public void ShowSkipButton()
    {
        if (_hud != null)
        {
            _hud.ShowSkipButton();
            GD.Print("⏭️ Skip button shown");
        }
        else
        {
            GD.PrintErr("❌ Cannot show skip button - HUD is null");
        }
    }

    public void HideSkipButton()
    {
        if (_hud != null)
        {
            _hud.HideSkipButton();
            GD.Print("⏸️ Skip button hidden");
        }
        else
        {
            GD.PrintErr("❌ Cannot hide skip button - HUD is null");
        }
    }

    public void SetWaveButtonState(string text, bool enabled)
    {
        if (_hud?.SkipButton != null)
        {
            _hud.SkipButton.Text = text;
            _hud.SkipButton.Disabled = !enabled;
            GD.Print($"🌊 Wave button updated: '{text}', enabled: {enabled}");
        }
        else
        {
            GD.PrintErr("❌ Cannot update wave button - button is null");
        }
    }

    // Utility Methods
    public bool IsInitialized()
    {
        return _hud != null;
    }

    public Hud? GetHud()
    {
        return _hud;
    }

    public override void _ExitTree()
    {
        if (Instance == this)
        {
            Instance = null;
            GD.Print("🎯 HudManager singleton cleaned up");
        }
    }
}
