using Godot;
using Game.Presentation.Buildings;
using Game.Presentation.UI;
using Game.Infrastructure.Stats.Services;
using Game.Infrastructure.Audio.Services;
using Game.Domain.Audio.Enums;

namespace Game.Presentation.Player;

public class PlayerBuildingStats
{
	public int Cost { get; set; }
	public int Damage { get; set; }
	public float Range { get; set; }
	public float AttackSpeed { get; set; }
}

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 200f;

	[Export] public PackedScene? BasicTowerScene;
	[Export] public PackedScene? SniperTowerScene;
	[Export] public PackedScene? RapidTowerScene;
	[Export] public PackedScene? HeavyTowerScene;

	public PackedScene? CurrentBuildingScene { get; private set; } = null;

	private PlayerMovement _movement = null!;
	private PlayerBuildingBuilder _buildingBuilder = null!;
	private BuildingSelectionHud? _buildingSelectionHud = null;

	public override void _Ready()
	{
		AddToGroup("player");
		
		if (Speed <= 0)
			Speed = 200f;
		
		CurrentBuildingScene = null!;
		// Don't update display immediately - HudManager might not be ready
		// UpdateSelectedBuildingDisplay("None");

		_movement = new PlayerMovement(this);
		_buildingBuilder = new PlayerBuildingBuilder(this);
		
		// Find and connect to BuildingSelectionHud
		CallDeferred(nameof(InitializeBuildingSelectionHud));
	}

	private void InitializeBuildingSelectionHud()
	{
		try
		{
			// Find BuildingSelectionHud in the HUD CanvasLayer
			var hudLayer = GetTree().GetFirstNodeInGroup("hud") as CanvasLayer;
			if (hudLayer == null)
			{
				// Fallback: search by path if group not found
				hudLayer = GetNodeOrNull<CanvasLayer>("/root/Main/CanvasLayer");
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
	
	private void SyncHudSelectionState()
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
	
	private string? GetBuildingKeyFromBuildingScene()
	{
		if (CurrentBuildingScene == null) return null;
		
		// Use Domain ConfigKey constants instead of hardcoded strings
		if (CurrentBuildingScene == BasicTowerScene) return Game.Domain.Buildings.Entities.BasicTower.ConfigKey;
		if (CurrentBuildingScene == SniperTowerScene) return Game.Domain.Buildings.Entities.SniperTower.ConfigKey;
		if (CurrentBuildingScene == RapidTowerScene) return Game.Domain.Buildings.Entities.RapidTower.ConfigKey;
		if (CurrentBuildingScene == HeavyTowerScene) return Game.Domain.Buildings.Entities.HeavyTower.ConfigKey;
		
		return null;
	}
	
	private void NotifyHudSelectionChange(string? buildingKey)
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

	public override void _PhysicsProcess(double delta)
	{
		_movement.Update(delta);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		_buildingBuilder.HandleInput(@event);

		if (@event is InputEventKey key && key.Pressed)
		{
			switch (key.Keycode)
			{
				case Key.Key1:
					if (BasicTowerScene != null)
					{
						GD.Print($"🔍 Key1 pressed. CurrentBuildingScene: {(CurrentBuildingScene == null ? "null" : CurrentBuildingScene.ResourcePath.GetFile().GetBaseName())}");
						GD.Print($"🔍 BasicTowerScene: {BasicTowerScene.ResourcePath.GetFile().GetBaseName()}");
						GD.Print($"🔍 Are they equal? {CurrentBuildingScene == BasicTowerScene}");
						
					if (CurrentBuildingScene == BasicTowerScene)
						{
							PlayBuildingDeselectionSound();
							ClearBuildingSelection();
							GD.Print("🚫 Deselected Basic Tower");
						}
						else
						{
							PlayBuildingSelectionSound();
							CurrentBuildingScene = BasicTowerScene;
							UpdateSelectedBuildingDisplay("Basic");
							_buildingBuilder.StartBuildMode(BasicTowerScene);
							NotifyHudSelectionChange(Game.Domain.Buildings.Entities.BasicTower.ConfigKey);
							GD.Print("📦 Selected Basic Tower for building");
						}
					}
					else
					{
						GD.PrintErr("❌ No BasicTowerScene assigned!");
					}
					break;

				case Key.Key2:
					if (SniperTowerScene != null)
					{
					if (CurrentBuildingScene == SniperTowerScene)
						{
							PlayBuildingDeselectionSound();
							ClearBuildingSelection();
							GD.Print("🚫 Deselected Sniper Tower");
						}
						else
						{
							PlayBuildingSelectionSound();
							CurrentBuildingScene = SniperTowerScene;
							UpdateSelectedBuildingDisplay("Sniper");
							_buildingBuilder.StartBuildMode(SniperTowerScene);
							NotifyHudSelectionChange(Game.Domain.Buildings.Entities.SniperTower.ConfigKey);
							GD.Print("🎯 Selected Sniper Tower for building");
						}
					}
					else
					{
						GD.PrintErr("❌ No SniperTowerScene assigned!");
					}
					break;

				case Key.Key3:
					if (RapidTowerScene != null)
					{
					if (CurrentBuildingScene == RapidTowerScene)
						{
							PlayBuildingDeselectionSound();
							ClearBuildingSelection();
							GD.Print("🚫 Deselected Rapid Tower");
						}
						else
						{
							PlayBuildingSelectionSound();
							CurrentBuildingScene = RapidTowerScene;
							UpdateSelectedBuildingDisplay("Rapid");
							_buildingBuilder.StartBuildMode(RapidTowerScene);
							NotifyHudSelectionChange(Game.Domain.Buildings.Entities.RapidTower.ConfigKey);
							GD.Print("⚡ Selected Rapid Tower for building");
						}
					}
					else
					{
						GD.PrintErr("❌ No RapidTowerScene assigned!");
					}
					break;

				case Key.Key4:
					if (HeavyTowerScene != null)
					{
					if (CurrentBuildingScene == HeavyTowerScene)
						{
							PlayBuildingDeselectionSound();
							ClearBuildingSelection();
							GD.Print("🚫 Deselected Heavy Tower");
						}
						else
						{
							PlayBuildingSelectionSound();
							CurrentBuildingScene = HeavyTowerScene;
							UpdateSelectedBuildingDisplay("Heavy");
							_buildingBuilder.StartBuildMode(HeavyTowerScene);
							NotifyHudSelectionChange(Game.Domain.Buildings.Entities.HeavyTower.ConfigKey);
							GD.Print("💪 Selected Heavy Tower for building");
						}
					}
					else
					{
						GD.PrintErr("❌ No HeavyTowerScene assigned!");
					}
					break;
			}
		}
	}
	
	public void SelectBuilding(string buildingId)
	{
		switch (buildingId)
		{
			case "Basic":
				CurrentBuildingScene = BasicTowerScene;
				UpdateSelectedBuildingDisplay("Basic");
				_buildingBuilder.StartBuildMode(BasicTowerScene!);
				NotifyHudSelectionChange(Game.Domain.Buildings.Entities.BasicTower.ConfigKey);
				break;
			case "Sniper":
				CurrentBuildingScene = SniperTowerScene;
				UpdateSelectedBuildingDisplay("Sniper");
				_buildingBuilder.StartBuildMode(SniperTowerScene!);
				NotifyHudSelectionChange(Game.Domain.Buildings.Entities.SniperTower.ConfigKey);
				break;
			case "Rapid":
				CurrentBuildingScene = RapidTowerScene;
				UpdateSelectedBuildingDisplay("Rapid");
				_buildingBuilder.StartBuildMode(RapidTowerScene!);
				NotifyHudSelectionChange(Game.Domain.Buildings.Entities.RapidTower.ConfigKey);
				break;
			case "Heavy":
				CurrentBuildingScene = HeavyTowerScene;
				UpdateSelectedBuildingDisplay("Heavy");
				_buildingBuilder.StartBuildMode(HeavyTowerScene!);
				NotifyHudSelectionChange(Game.Domain.Buildings.Entities.HeavyTower.ConfigKey);
				break;
		}
	}
	
	public void ClearBuildingSelection()
	{
		CurrentBuildingScene = null!;
		UpdateSelectedBuildingDisplay("None");
		HideBuildingStats();
		_buildingBuilder.CancelBuildMode();
		NotifyHudSelectionChange(null);
		GD.Print("🚫 Cleared building selection");
	}
	
	public void CancelBuildMode()
	{
		GD.Print($"🔧 CancelBuildMode called. CurrentBuildingScene before: {(CurrentBuildingScene == null ? "null" : CurrentBuildingScene.ResourcePath.GetFile().GetBaseName())}");
		
		_buildingBuilder?.CancelBuildMode();
		
		CurrentBuildingScene = null!;
		HideBuildingStats();
		NotifyHudSelectionChange(null);
		
		GD.Print($"🔧 CancelBuildMode finished. CurrentBuildingScene after: {(CurrentBuildingScene == null ? "null" : "NOT NULL - ERROR!")}");
	}
	
	public void ClearPlayerSelectionState()
	{
		GD.Print($"🧹 ClearPlayerSelectionState called. CurrentBuildingScene before: {(CurrentBuildingScene == null ? "null" : CurrentBuildingScene.ResourcePath.GetFile().GetBaseName())}");
		
		CurrentBuildingScene = null!;
		HideBuildingStats();
		// Note: Don't call NotifyHudSelectionChange here as this is called by PlayerBuildingBuilder
		// and would create circular calls when the main selection methods already handle HUD notification
		
		GD.Print($"🧹 ClearPlayerSelectionState finished. CurrentBuildingScene after: {(CurrentBuildingScene == null ? "null" : "NOT NULL - ERROR!")}");
	}

	private void UpdateSelectedBuildingDisplay(string buildingName)
	{
		UpdateBuildingStats(buildingName);
	}
	
	private void UpdateBuildingStats(string buildingName)
	{
		if (buildingName == "None")
		{
			HideBuildingStats();
			return;
		}
		
		var stats = GetBuildingStats(buildingName);
		if (stats != null)
		{
			ShowBuildingStats(buildingName, stats.Cost, stats.Damage, stats.Range, stats.AttackSpeed);
		}
	}
	
	private PlayerBuildingStats? GetBuildingStats(string buildingName)
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
	
	private void ShowBuildingStats(string buildingName, int cost, int damage, float range, float attackSpeed)
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

	private void HideBuildingStats()
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
