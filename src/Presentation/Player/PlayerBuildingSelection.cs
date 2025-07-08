using Godot;
using Game.Infrastructure.Audio.Services;
using Game.Domain.Audio.Enums;

namespace Game.Presentation.Player;

public class PlayerBuildingSelection
{
    private readonly Player _player;
    
    public PlayerBuildingSelection(Player player)
    {
        _player = player;
    }

    public void HandleBuildingSelection(Key keycode)
    {
        switch (keycode)
        {
            case Key.Key1:
                HandleBuildingToggle(_player.BasicTowerScene, "Basic", Game.Domain.Buildings.Entities.BasicTower.ConfigKey);
                break;
            case Key.Key2:
                HandleBuildingToggle(_player.SniperTowerScene, "Sniper", Game.Domain.Buildings.Entities.SniperTower.ConfigKey);
                break;
            case Key.Key3:
                HandleBuildingToggle(_player.RapidTowerScene, "Rapid", Game.Domain.Buildings.Entities.RapidTower.ConfigKey);
                break;
            case Key.Key4:
                HandleBuildingToggle(_player.HeavyTowerScene, "Heavy", Game.Domain.Buildings.Entities.HeavyTower.ConfigKey);
                break;
        }
    }

    private void HandleBuildingToggle(PackedScene? buildingScene, string buildingName, string configKey)
    {
        if (buildingScene == null)
        {
            GD.PrintErr($"❌ No {buildingName}TowerScene assigned!");
            return;
        }

        if (_player.CurrentBuildingScene == buildingScene)
        {
            DeselectCurrentBuilding();
        }
        else
        {
            SelectBuilding(buildingScene, buildingName, configKey);
        }
    }

    private void SelectBuilding(PackedScene buildingScene, string buildingName, string configKey)
    {
        PlayBuildingSelectionSound();
        _player.CurrentBuildingScene = buildingScene;
        _player.UpdateSelectedBuildingDisplay(buildingName);
        _player._buildingBuilder.StartBuildMode(buildingScene);
        _player._hudConnector.NotifyHudSelectionChange(configKey);
        GD.Print($"🏗️ Selected {buildingName} Tower for building");
    }

    private void DeselectCurrentBuilding()
    {
        PlayBuildingDeselectionSound();
        _player.ClearBuildingSelection();
        GD.Print("🚫 Deselected current building");
    }

    public void SelectBuildingByName(string buildingId)
    {
        var (buildingScene, buildingName, configKey) = GetBuildingInfo(buildingId);
        
        if (buildingScene != null)
        {
            SelectBuilding(buildingScene, buildingName, configKey);
        }
        else
        {
            GD.PrintErr($"❌ Unknown building ID: {buildingId}");
        }
    }

    private (PackedScene?, string, string) GetBuildingInfo(string buildingId)
    {
        return buildingId switch
        {
            "Basic" => (_player.BasicTowerScene, "Basic", Game.Domain.Buildings.Entities.BasicTower.ConfigKey),
            "Sniper" => (_player.SniperTowerScene, "Sniper", Game.Domain.Buildings.Entities.SniperTower.ConfigKey),
            "Rapid" => (_player.RapidTowerScene, "Rapid", Game.Domain.Buildings.Entities.RapidTower.ConfigKey),
            "Heavy" => (_player.HeavyTowerScene, "Heavy", Game.Domain.Buildings.Entities.HeavyTower.ConfigKey),
            _ => (null, "", "")
        };
    }

    private void PlayBuildingSelectionSound()
    {
        try
        {
            if (SoundManagerService.Instance != null)
            {
                SoundManagerService.Instance.PlaySound("tower_select", SoundCategory.UI);
                GD.Print("🎵 Played building selection sound");
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"❌ Error playing building selection sound: {ex.Message}");
        }
    }

    private void PlayBuildingDeselectionSound()
    {
        try
        {
            if (SoundManagerService.Instance != null)
            {
                SoundManagerService.Instance.PlaySound("tower_deselect", SoundCategory.UI);
                GD.Print("🎵 Played building deselection sound");
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"❌ Error playing building deselection sound: {ex.Message}");
        }
    }
}
