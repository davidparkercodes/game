using Godot;
using Game.Di;
using Game.Infrastructure.Audio.Services;
using Game.Application.Shared.Cqrs;
using Game.Infrastructure.Sound;
using Game.Presentation.Systems;
using Game.Application.Shared.Services;
using Game.Presentation.UI;
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
		_inventoryPanel = GetNode<Panel>("InventoryUI/InventoryPanel");
		_inventoryList = _inventoryPanel.GetNode<VBoxContainer>("MarginContainer/InventoryList");
		
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
		
		// Try to get the GroundLayer node - it should be a TileMapLayer itself
		var groundLayerNode = GetNodeOrNull("GroundLayer");
		if (groundLayerNode == null)
		{
			GD.PrintErr("❌ GroundLayer node not found in scene");
			return;
		}
		
		GD.Print($"🔍 Found GroundLayer node: {groundLayerNode.GetType().Name}");
		
		// The GroundLayer should be a TileMapLayer directly since Level01.tscn's root is TileMapLayer
		if (groundLayerNode is TileMapLayer directLayer)
		{
			foundLayer = directLayer;
			GD.Print("✅ GroundLayer is directly a TileMapLayer");
		}
		else
		{
			// Fallback: search for TileMapLayer children
			GD.Print($"🔍 GroundLayer is {groundLayerNode.GetType().Name}, searching for TileMapLayer children...");
			
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
			GD.PrintErr("💡 Make sure the GroundLayer scene contains a TileMapLayer node");
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("toggle_inventory"))
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
		foreach (Node child in _inventoryList.GetChildren())
		{
			child.QueueFree();
		}

		foreach (var item in Game.Presentation.Inventory.Inventory.GetItems())
		{
			var label = new Label();
			label.Text = $"{item.Key}: {item.Value}";
			_inventoryList.AddChild(label);
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

	public DiContainer GetDiContainer() => _diContainer;
	public IMediator GetMediator() => _mediator;
}
