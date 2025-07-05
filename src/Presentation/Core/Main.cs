using Godot;
using Game.Di;
using Game.Infrastructure.Audio.Services;
using Game.Application.Shared.Cqrs;
using Game.Infrastructure.Sound;
using Game.Presentation.Systems;
using Game.Application.Shared.Services;
using static Game.Di.DiConfiguration;

namespace Game.Presentation.Core;

public partial class Main : Node
{
	private Panel _inventoryPanel;
	private VBoxContainer _inventoryList;
	private DiContainer _diContainer;
	private IMediator _mediator;

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
		
		GD.Print("🎨 UI components initialized");
	}

	private void InitializeBuildingSystem()
	{
		var groundLayerInstance = GetNode("GroundLayer");
		var groundLayer = groundLayerInstance as TileMapLayer;
		
		if (groundLayer != null)
		{
			BuildingZoneValidator.Initialize(groundLayer);
			GD.Print("🏗️ Building system initialized successfully");
		}
		else
		{
			GD.PrintErr("❌ Failed to find GroundLayer TileMapLayer");
			GD.PrintErr($"❌ GroundLayer node type: {groundLayerInstance?.GetType().Name}");
			
			var tileMapLayer = groundLayerInstance?.GetNodeOrNull<TileMapLayer>(".");
			if (tileMapLayer == null && groundLayerInstance != null)
			{
				foreach (Node child in groundLayerInstance.GetChildren())
				{
					if (child is TileMapLayer layer)
					{
						tileMapLayer = layer;
						break;
					}
				}
			}
			
			if (tileMapLayer != null)
			{
				BuildingZoneValidator.Initialize(tileMapLayer);
				GD.Print("🏗️ Building system initialized successfully with child TileMapLayer");
			}
			else
			{
				GD.PrintErr("❌ Could not find any TileMapLayer in GroundLayer or its children");
			}
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
