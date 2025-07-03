using Godot;

public partial class GameManager : Node
{
	[Signal] public delegate void MoneyChangedEventHandler(int newAmount);
	[Signal] public delegate void LivesChangedEventHandler(int newAmount);
	[Signal] public delegate void GameOverEventHandler();

	[Export] public PackedScene HudScene;
	[Export] public int StartingMoney = 100;
	[Export] public int StartingLives = 20;
	[Export] public int MoneyPerEnemyKilled = 10;

	public static GameManager Instance { get; private set; }

	public int Money { get; private set; }
	public int Lives { get; private set; }
	public bool IsGameOver { get; private set; } = false;

	public Hud Hud { get; private set; }
	public RoundManager RoundManager { get; private set; }

	public override void _Ready()
	{
		Instance = this;

		// Initialize game state
		Money = StartingMoney;
		Lives = StartingLives;

		// Create RoundManager
		RoundManager = new RoundManager();
		AddChild(RoundManager);

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
		// Update HUD phase indicator if needed
	}

	private void OnRoundStarted(int roundNumber)
	{
		GD.Print($"🎯 Round {roundNumber} started!");
		UpdateHUD();
	}

	private void OnRoundCompleted(int roundNumber)
	{
		GD.Print($"🏆 Round {roundNumber} completed!");
		// Maybe give bonus money or other rewards
	}
	
	private void ConnectToWaveSpawner()
	{
		if (WaveSpawner.Instance != null)
		{
			WaveSpawner.Instance.WaveCompleted += OnWaveCompleted;
			GD.Print("🔗 Connected to WaveSpawner signals");
		}
	}
	
	private void OnWaveCompleted(int waveNumber, int bonusMoney)
	{
		GD.Print($"✅ Wave {waveNumber} completed! Bonus: ${bonusMoney}");
		// Tell RoundManager to complete the round
		RoundManager.CompleteRound();
	}
}
