using Godot;
using Game.Presentation.Buildings;
using Game.Presentation.UI;
using Game.Infrastructure.Stats.Services;

namespace Game.Presentation.Player;

public class PlayerBuildingStats
{
    public int Cost { get; set; }
    public int Damage { get; set; }
    public float Range { get; set; }
    public float AttackSpeed { get; set; }
}

public class PlayerHudConnector
{
    private readonly Player _player;
    private BuildingSelectionHud? _buildingSelectionHud = null;

    public PlayerHudConnector(Player player)
    {
        _player = player;
    }

    public void InitializeHudConnections()
    {
        try
        {
            // Find BuildingSelectionHud in the HUD CanvasLayer
            var hudLayer = _player.GetTree().GetFirstNodeInGroup("hud") as CanvasLayer;
            if (hudLayer == null)
            {
                // Fallback: search by path if group not found
                hudLayer = _player.GetNodeOrNull<CanvasLayer>("/root/Main/CanvasLayer");
            }
            
            if (hudLayer != null)
            {
                _buildingSelectionHud = hudLayer.GetNodeOrNull<BuildingSelectionHud>("BuildingSelectionHud");
                if (_buildingSelectionHud != null)
                {
                    GD.Print("🎯 Player connected to BuildingSelectionHud successfully");
                    // Sync initial state
                    SyncHudSelectionState();
                }
                else
                {
                    GD.PrintErr("⚠️ BuildingSelectionHud not found in HUD layer");
                }
            }
            else
            {
                GD.PrintErr("⚠️ HUD CanvasLayer not found");
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"❌ Error initializing BuildingSelectionHud connection: {ex.Message}");
        }
    }

    public void SyncHudSelectionState()
    {
        if (_buildingSelectionHud == null) return;
        
        // Get current building key based on CurrentBuildingScene
        string? currentBuildingKey = GetBuildingKeyFromBuildingScene();
        
        if (currentBuildingKey != null)
        {
            _buildingSelectionHud.SetSelectedBuilding(currentBuildingKey);
            GD.Print($"🎯 Synced HUD selection to: {currentBuildingKey}");
        }
        else
        {
            _buildingSelectionHud.ClearSelection();
            GD.Print("🎯 Synced HUD selection: cleared");
        }
    }

    public void NotifyHudSelectionChange(string? buildingKey)
    {
        if (_buildingSelectionHud == null) return;
        
        if (buildingKey != null)
        {
            _buildingSelectionHud.SetSelectedBuilding(buildingKey);
        }
        else
        {
            _buildingSelectionHud.ClearSelection();
        }
    }

    public void ShowBuildingStats(string buildingName, int cost, int damage, float range, float attackSpeed)
    {
        if (HudManager.Instance != null && HudManager.Instance.IsInitialized())
        {
            HudManager.Instance.ShowBuildingStats(buildingName, cost, damage, range, attackSpeed);
            GD.Print($"🏗️ Building Stats: {buildingName} - Cost: ${cost}, Damage: {damage}, Range: {range:F1}, Attack Speed: {attackSpeed:F0}");
        }
        else
        {
            GD.PrintErr("❌ Cannot show building stats - HudManager not available");
        }
    }

    public void HideBuildingStats()
    {
        if (HudManager.Instance != null && HudManager.Instance.IsInitialized())
        {
            HudManager.Instance.HideBuildingStats();
            GD.Print("🚫 Hiding building stats");
        }
        else
        {
            GD.PrintErr("❌ Cannot hide building stats - HudManager not available");
        }
    }

    public PlayerBuildingStats? GetBuildingStats(string buildingName)
    {
        // Map display names to config keys using domain entity ConfigKey constants
        // This ensures consistency with domain layer and eliminates hardcoded strings
        string? configKey = buildingName.ToLower() switch
        {
            "basic" => Game.Domain.Buildings.Entities.BasicTower.ConfigKey,
            "sniper" => Game.Domain.Buildings.Entities.SniperTower.ConfigKey,
            "rapid" => Game.Domain.Buildings.Entities.RapidTower.ConfigKey,
            "heavy" => Game.Domain.Buildings.Entities.HeavyTower.ConfigKey,
            _ => null
        };
        
        if (configKey == null) return null;
        
        // Get stats directly from the configuration service
        if (StatsManagerService.Instance != null)
        {
            var configStats = StatsManagerService.Instance.GetBuildingStats(configKey);
            
            GD.Print($"🔧 Loading stats for {buildingName} ({configKey}): Cost=${configStats.cost}, Damage={configStats.damage}, Range={configStats.range}, AttackSpeed={configStats.attack_speed}");
            
            return new PlayerBuildingStats
            {
                Cost = configStats.cost,
                Damage = configStats.damage,
                Range = configStats.range,
                AttackSpeed = configStats.attack_speed
            };
        }
        else
        {
            GD.PrintErr($"❌ StatsManagerService not available for {buildingName} stats");
            return null;
        }
    }

    private string? GetBuildingKeyFromBuildingScene()
    {
        if (_player.CurrentBuildingScene == null) return null;
        
        // Use Domain ConfigKey constants instead of hardcoded strings
        if (_player.CurrentBuildingScene == _player.BasicTowerScene) return Game.Domain.Buildings.Entities.BasicTower.ConfigKey;
        if (_player.CurrentBuildingScene == _player.SniperTowerScene) return Game.Domain.Buildings.Entities.SniperTower.ConfigKey;
        if (_player.CurrentBuildingScene == _player.RapidTowerScene) return Game.Domain.Buildings.Entities.RapidTower.ConfigKey;
        if (_player.CurrentBuildingScene == _player.HeavyTowerScene) return Game.Domain.Buildings.Entities.HeavyTower.ConfigKey;
        
        return null;
    }
}
