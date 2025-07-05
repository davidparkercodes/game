using Godot;
using Game.Di;
using Game.Infrastructure.Audio.Services;
using Game.Application.Shared.Cqrs;
using Game.Infrastructure.Sound;
using Game.Presentation.Systems;
using Game.Application.Shared.Services;
using Game.Presentation.UI;
using Game.Infrastructure.Waves.Services;
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

	private void InitializeWaveManager()
	{
		// Initialize Wave Manager and set initial button state
		WaveManager.Initialize();
		WaveManager.Instance?.Reset(); // This will set up the initial wave button state
		GD.Print("🌊 Wave Manager initialized");
	}

	public DiContainer GetDiContainer() => _diContainer;
	public IMediator GetMediator() => _mediator;
}
