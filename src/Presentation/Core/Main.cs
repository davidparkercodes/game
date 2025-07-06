using Godot;
using Game.Di;
using Game.Infrastructure.Audio.Services;
using Game.Application.Shared.Cqrs;
using Game.Infrastructure.Sound;
using Game.Presentation.Systems;
using Game.Application.Shared.Services;
using Game.Presentation.UI;
using Game.Infrastructure.Waves.Services;
using Game.Application.Game.Services;
using Game.Infrastructure.Map.Services;
using Game.Infrastructure.Map.Extensions;
using static Game.Di.DiConfiguration;

namespace Game.Presentation.Core;

public partial class Main : Node
{
	private Panel _inventoryPanel = null!;
	private VBoxContainer _inventoryList = null!;
	private DiContainer _diContainer = null!;
	private IMediator _mediator = null!;
	private HudManager _hudManager = null!;
	private Hud _hud = null!;
	private ITimeManager _timeManager = null!;
	private SpeedControl _speedControl = null!;
	private static MapBoundaryService? _mapBoundaryService = null!;
	
	public static MapBoundaryService? MapBoundaryService => _mapBoundaryService;

	public override void _Ready()
	{
		InitializeDependencyContainer();
		InitializeInfrastructure();
		InitializeUI();
		// Defer building system initialization to ensure GroundLayer is ready
		CallDeferred(nameof(InitializeBuildingSystem));
	}

	private void InitializeDependencyContainer()
	{
		// Temporary workaround - directly use DiContainer
		_diContainer = new DiContainer();
		RegisterServices(_diContainer);
		
		_mediator = _diContainer.Resolve<IMediator>();
		
		GD.Print("🔧 DI Container initialized with mediator");

		// Perform startup validation
		PerformStartupValidation();
	}

	private void InitializeInfrastructure()
	{
		// SoundManagerService is a singleton, just ensure it's initialized
		var soundManagerService = SoundManagerService.Instance;
		
		GD.Print("🔊 Sound infrastructure initialized");
	}

	private void InitializeUI()
	{
		// Try to initialize inventory - may not exist in all scenes
		try
		{
			_inventoryPanel = GetNodeOrNull<Panel>("InventoryUI/InventoryPanel");
			if (_inventoryPanel != null)
			{
				_inventoryList = _inventoryPanel.GetNodeOrNull<VBoxContainer>("MarginContainer/InventoryList");
				GD.Print("✅ Inventory UI found and initialized");
			}
			else
			{
				GD.Print("⚠️ Inventory UI not found - skipping inventory initialization");
			}
		}
		catch (System.Exception ex)
		{
			GD.PrintErr($"⚠️ Failed to initialize inventory: {ex.Message}");
		}
		
		// Initialize HUD
		InitializeHUD();
		
		// Initialize TimeManager
		InitializeTimeManager();
		
		// Initialize Speed Control
		InitializeSpeedControl();
		
		GD.Print("🎨 UI components initialized");
	}

	private void InitializeHUD()
	{
		try
		{
			// Load and instantiate HUD scene
			var hudScene = GD.Load<PackedScene>("res://scenes/UI/Hud.tscn");
			if (hudScene == null)
			{
				GD.PrintErr("❌ Failed to load HUD scene from res://scenes/UI/Hud.tscn");
				return;
			}
			
			_hud = hudScene.Instantiate<Hud>();
			if (_hud == null)
			{
				GD.PrintErr("❌ Failed to instantiate HUD from scene");
				return;
			}
			
			// Add HUD to scene tree
			AddChild(_hud);
			GD.Print("✅ HUD scene instantiated and added to tree");
			
			// Create and initialize HUD Manager
			_hudManager = new HudManager();
			AddChild(_hudManager);
			GD.Print("✅ HudManager created and added to tree");
			
		// Connect HUD Manager to HUD instance
		_hudManager.Initialize(_hud);
		GD.Print("🎯 HUD fully initialized and connected");
		
		// Initialize Wave Manager after everything is set up
		CallDeferred(nameof(InitializeWaveManager));
		}
		catch (System.Exception ex)
		{
			GD.PrintErr($"❌ Failed to initialize HUD: {ex.Message}");
			GD.PrintErr($"Stack trace: {ex.StackTrace}");
		}
	}

	private void InitializeBuildingSystem()
	{
		TileMapLayer? foundLayer = null;
		
		// Try to get the Level01 node - it should be a TileMapLayer itself
		var groundLayerNode = GetNodeOrNull("Level01");
		if (groundLayerNode == null)
		{
			GD.PrintErr("❌ Level01 node not found in scene");
			return;
		}
		
		GD.Print($"🔍 Found Level01 node: {groundLayerNode.GetType().Name}");
		
		// The Level01 should be a TileMapLayer directly since Level01.tscn's root is TileMapLayer
		if (groundLayerNode is TileMapLayer directLayer)
		{
			foundLayer = directLayer;
			GD.Print("✅ Level01 is directly a TileMapLayer");
		}
		else
		{
			// Fallback: search for TileMapLayer children
			GD.Print($"🔍 Level01 is {groundLayerNode.GetType().Name}, searching for TileMapLayer children...");
			
			// Look for TileMapLayer in immediate children
			foreach (Node child in groundLayerNode.GetChildren())
			{
				GD.Print($"  - Child: {child.Name} ({child.GetType().Name})");
				if (child is TileMapLayer childLayer)
				{
					foundLayer = childLayer;
					GD.Print($"✅ Found TileMapLayer child: {child.Name}");
					break;
				}
			}
			
			// Last resort: use GetNode if we know the specific path
			if (foundLayer == null)
			{
				foundLayer = groundLayerNode.GetNodeOrNull<TileMapLayer>(".");
				if (foundLayer != null)
				{
					GD.Print("✅ Found TileMapLayer using GetNodeOrNull");
				}
			}
		}
		
	if (foundLayer != null)
	{
		BuildingZoneValidator.Initialize(foundLayer);
		GD.Print("🏗️ Building system initialized successfully");
		
		// Initialize MapBoundaryService with the same TileMapLayer
		_mapBoundaryService = new MapBoundaryService();
		_mapBoundaryService.Initialize(foundLayer);
		GD.Print("🗺️ Map boundary service initialized successfully");
	}
	else
	{
		GD.PrintErr("❌ Could not find any TileMapLayer - building placement will not work");
		GD.PrintErr("💡 Make sure the Level01 scene contains a TileMapLayer node");
	}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("toggle_inventory") && _inventoryPanel != null)
		{
			_inventoryPanel.Visible = !_inventoryPanel.Visible;

			if (_inventoryPanel.Visible)
			{
				UpdateInventoryDisplay();
			}
		}
		
		// DEBUG: Wave jump shortcuts
		HandleDebugInput(@event);
	}

	private void UpdateInventoryDisplay()
	{
		if (_inventoryList == null)
		{
			GD.PrintErr("⚠️ Cannot update inventory display - inventory list is null");
			return;
		}
		
		foreach (Node child in _inventoryList.GetChildren())
		{
			child.QueueFree();
		}

		try
		{
			foreach (var item in Game.Presentation.Inventory.Inventory.GetItems())
			{
				var label = new Label();
				label.Text = $"{item.Key}: {item.Value}";
				_inventoryList.AddChild(label);
			}
		}
		catch (System.Exception ex)
		{
			GD.PrintErr($"⚠️ Failed to update inventory display: {ex.Message}");
		}
	}

	private void PerformStartupValidation()
	{
		var validationService = _diContainer.Resolve<StartupValidationService>();
		if (validationService != null)
		{
			bool isValid = validationService.ValidateOnStartup();
			if (!isValid)
			{
				GD.PrintErr("⚠️  Startup validation detected issues with configuration. Check logs for detailed errors.");
			}
		}
	}

	private void InitializeTimeManager()
	{
		try
		{
			// Get TimeManager from DI container
			_timeManager = _diContainer.Resolve<ITimeManager>();
			
			// Add GodotTimeManager to scene tree if it's a Node
			if (_timeManager is Node timeManagerNode)
			{
				AddChild(timeManagerNode);
				GD.Print("⚡ GodotTimeManager added to scene tree");
			}
			
			GD.Print("⚡ TimeManager resolved from DI container");
		}
		catch (System.Exception ex)
		{
			GD.PrintErr($"❌ Failed to initialize TimeManager: {ex.Message}");
		}
	}

	private void InitializeSpeedControl()
	{
		try
		{
			// Load and instantiate SpeedControlPanel scene
			var speedControlScene = GD.Load<PackedScene>("res://scenes/UI/SpeedControlPanel.tscn");
			if (speedControlScene == null)
			{
				GD.PrintErr("❌ Failed to load SpeedControlPanel scene from res://scenes/UI/SpeedControlPanel.tscn");
				return;
			}
			
			_speedControl = speedControlScene.Instantiate<SpeedControl>();
			if (_speedControl == null)
			{
				GD.PrintErr("❌ Failed to instantiate SpeedControl from scene");
				return;
			}
			
			// Add SpeedControl to scene tree
			AddChild(_speedControl);
			GD.Print("✅ SpeedControlPanel instantiated and added to tree");
		}
		catch (System.Exception ex)
		{
			GD.PrintErr($"❌ Failed to initialize SpeedControl: {ex.Message}");
			GD.PrintErr($"Stack trace: {ex.StackTrace}");
		}
	}

	private void InitializeWaveManager()
	{
		// Initialize Wave Manager and set initial button state
		WaveManager.Initialize();
		WaveManager.Instance?.Reset(); // This will set up the initial wave button state
		GD.Print("🌊 Wave Manager initialized");
		
		// Announce debug shortcuts
		AnnounceDebugShortcuts();
	}

	// Handle keyboard shortcuts (both debug and game features)
	private void HandleDebugInput(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.ShiftPressed)
		{
			switch (keyEvent.Keycode)
			{
				// Speed Control shortcuts
				case Key.Key1:
					GD.Print("🐌 SPEED: Shift+1 pressed - Setting speed to 1x");
					Game.Infrastructure.Game.GodotTimeManager.Instance?.SetSpeedTo1x();
					break;

				case Key.Key2:
					GD.Print("🏃 SPEED: Shift+2 pressed - Setting speed to 2x");
					Game.Infrastructure.Game.GodotTimeManager.Instance?.SetSpeedTo2x();
					break;

				case Key.Key3:
					GD.Print("⚡ SPEED: Shift+3 pressed - Setting speed to 4x");
					Game.Infrastructure.Game.GodotTimeManager.Instance?.SetSpeedTo4x();
					break;

				// DEBUG: Wave control shortcuts
				case Key.Key5:
					GD.Print("🔥 DEBUG: Shift+5 pressed - Jumping to Wave 5 (Boss Wave)!");
					WaveManager.Instance?.JumpToWave(5);
					break;

				case Key.Key6:
					GD.Print("⏭️ DEBUG: Shift+6 pressed - Jumping to next wave!");
					WaveManager.Instance?.JumpToNextWave();
					break;

				case Key.Key7:
					GD.Print("⚡ DEBUG: Shift+7 pressed - Completing current wave instantly!");
					WaveManager.Instance?.CompleteCurrentWaveInstantly();
					break;
			}
		}
	}
	
	private void AnnounceDebugShortcuts()
	{
		GD.Print("🎮 ===== KEYBOARD SHORTCUTS AVAILABLE =====");
		GD.Print("🐌 Shift+1: Set speed to 1x");
		GD.Print("🏃 Shift+2: Set speed to 2x");
		GD.Print("⚡ Shift+3: Set speed to 4x");
		GD.Print("🔧 ===== DEBUG SHORTCUTS =====");
		GD.Print("🔥 Shift+5: Jump to Wave 5 (Boss Wave)");
		GD.Print("⏭️ Shift+6: Jump to next wave");
		GD.Print("⚡ Shift+7: Complete current wave instantly");
		GD.Print("🎮 ====================================");
	}

	public DiContainer GetDiContainer() => _diContainer;
	public IMediator GetMediator() => _mediator;
}
