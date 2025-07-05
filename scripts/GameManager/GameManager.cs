using Godot;
using Game.Presentation.UI;

public partial class GameManager : Node
{
	[Signal] public delegate void MoneyChangedEventHandler(int newAmount);
	[Signal] public delegate void LivesChangedEventHandler(int newAmount);
	[Signal] public delegate void GameOverEventHandler();
	[Signal] public delegate void GameWonEventHandler();

	[Export] public PackedScene HudScene;
	[Export] public int StartingMoney = 100;
	[Export] public int StartingLives = 20;
	[Export] public int MoneyPerEnemyKilled = 10;

	public static GameManager Instance { get; private set; }

	public int Money { get; private set; }
	public int Lives { get; private set; }
	public bool IsGameOver { get; private set; } = false;
	public bool IsGameWon { get; private set; } = false;
	
	[Export] public int TotalRounds = 5;

	public Hud Hud { get; private set; }
	public RoundManager RoundManager { get; private set; }

	public override void _Ready()
	{
		Instance = this;

		// Initialize game state
		Money = StartingMoney;
		Lives = StartingLives;
		
		// Initialize BuildingZoneValidator
		CallDeferred(nameof(InitializeBuildingSystem));

		// Get or create RoundManager
		RoundManager = RoundManager.Instance;
		if (RoundManager == null)
		{
			RoundManager = new RoundManager();
			AddChild(RoundManager);
			GD.Print("🎮 Created new RoundManager instance");
		}
		else
		{
			GD.Print("🎮 Using existing RoundManager instance");
		}

		// Create HUD
		Hud = HudScene?.Instantiate<Hud>();
		if (Hud == null)
		{
			GD.PrintErr("❌ HudScene export is empty or invalid.");
			return;
		}

		AddChild(Hud);

		// Connect RoundManager signals
		RoundManager.PhaseChanged += OnPhaseChanged;
		RoundManager.RoundStarted += OnRoundStarted;
		RoundManager.RoundCompleted += OnRoundCompleted;
		
		// Connect WaveSpawner signals if available (deferred since WaveSpawner might not be ready yet)
		CallDeferred(nameof(ConnectToWaveSpawner));

		// Update HUD
		Callable.From(() =>
		{
			UpdateHUD();
		}).CallDeferred();
	}

	public bool SpendMoney(int amount)
	{
		if (Money >= amount)
		{
			Money -= amount;
			EmitSignal(SignalName.MoneyChanged, Money);
			UpdateHUD();
			GD.Print($"💰 Spent {amount} money. Remaining: {Money}");
			return true;
		}
		return false;
	}

	public void AddMoney(int amount)
	{
		Money += amount;
		EmitSignal(SignalName.MoneyChanged, Money);
		UpdateHUD();
		GD.Print($"💰 Earned {amount} money. Total: {Money}");
	}

	public void LoseLife()
	{
		Lives--;
		EmitSignal(SignalName.LivesChanged, Lives);
		UpdateHUD();
		GD.Print($"💔 Lost a life. Remaining: {Lives}");

		if (Lives <= 0)
		{
			TriggerGameOver();
		}
	}

	public void OnEnemyKilled()
	{
		// Only add money if WaveSpawner is not handling it
		if (WaveSpawner.Instance == null)
		{
			AddMoney(MoneyPerEnemyKilled);
		}
		RoundManager.OnEnemyDefeated();
	}

	public void OnEnemyReachedEnd()
	{
		LoseLife();
	}

	private void TriggerGameOver()
	{
		IsGameOver = true;
		EmitSignal(SignalName.GameOver);
		GD.Print("💀 GAME OVER!");
		// You can add game over screen logic here
	}

	private void TriggerGameWon()
	{
		IsGameWon = true;
		EmitSignal(SignalName.GameWon);
		GD.Print("🎉 GAME WON! Congratulations!");
		// You can add victory screen logic here
	}

	private void UpdateHUD()
	{
		if (Hud != null)
		{
			Hud.UpdateMoney(Money);
			Hud.UpdateLives(Lives);
			Hud.UpdateWave(RoundManager?.CurrentRound ?? 1);
		}
	}

	private void OnPhaseChanged(RoundPhase newPhase)
	{
		GD.Print($"📋 Phase changed to: {newPhase}");
		
		if (Hud != null)
		{
			if (newPhase == RoundPhase.Build)
			{
				GD.Print("🔄 Showing skip button for build phase");
				Hud.ShowSkipButton();
			}
			else
			{
				GD.Print($"🎯 Hiding skip button for {newPhase} phase");
				Hud.HideSkipButton();
			}
		}
		else
		{
			GD.PrintErr("❌ HUD is null, cannot update button visibility");
		}
	}

	private void OnRoundStarted(int roundNumber)
	{
		GD.Print($"🎯 Round {roundNumber} started!");
		UpdateHUD();
	}

	private void OnRoundCompleted(int roundNumber)
	{
		GD.Print($"🏆 Round {roundNumber} completed!");
		
		// Check if player has completed all rounds
		if (roundNumber >= TotalRounds)
		{
			TriggerGameWon();
		}
	}
	
	private void ConnectToWaveSpawner()
	{
		if (WaveSpawner.Instance != null)
		{
			WaveSpawner.Instance.WaveCompleted += OnWaveCompleted;
			WaveSpawner.Instance.AllWavesCompleted += OnAllWavesCompleted;
			GD.Print("🔗 Connected to WaveSpawner signals successfully");
			GD.Print($"🌊 WaveSpawner has {WaveSpawner.Instance.GetTotalWaves()} total waves");
		}
		else
		{
			GD.PrintErr("❌ WaveSpawner.Instance is null! Cannot connect signals.");
		}
	}
	
	private void OnWaveCompleted(int waveNumber, int bonusMoney)
	{
		GD.Print($"✅ Wave {waveNumber} completed! Bonus: ${bonusMoney}");
		GD.Print($"📞 Telling RoundManager to complete round {RoundManager?.CurrentRound}");
		// Tell RoundManager to complete the round
		RoundManager.CompleteRound();
	}
	
	private void OnAllWavesCompleted()
	{
		GD.Print("🏆 All waves completed! Player wins!");
		TriggerGameWon();
	}
	
	private void InitializeBuildingSystem()
	{
		// Find the GroundLayer node which is an instance of Level01.tscn containing a TileMapLayer
		var groundLayerInstance = GetNode("../GroundLayer");
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
			
			// Try to find TileMapLayer as a child if it's not the direct node
			var tileMapLayer = groundLayerInstance?.GetNodeOrNull<TileMapLayer>(".");
			if (tileMapLayer == null && groundLayerInstance != null)
			{
				// Search for any TileMapLayer child
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
}
