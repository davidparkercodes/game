using Godot;

public enum RoundPhase
{
	Build,
	Defend,
	Transition
}

public partial class RoundManager : Node
{
	[Signal] public delegate void PhaseChangedEventHandler(RoundPhase newPhase);
	[Signal] public delegate void RoundStartedEventHandler(int roundNumber);
	[Signal] public delegate void RoundCompletedEventHandler(int roundNumber);
	[Signal] public delegate void DefendPhaseStartedEventHandler(int roundNumber, int enemyCount);
	[Signal] public delegate void BuildPhaseStartedEventHandler(int roundNumber, float duration);

	[Export] public float BuildPhaseDuration = 30.0f;
	[Export] public float TransitionDuration = 3.0f;
	[Export] public int BaseEnemyCount = 5;
	[Export] public float EnemyCountMultiplier = 1.2f;
	[Export] public int BonusMoneyPerRound = 25;

	public static RoundManager Instance { get; private set; }

	public RoundPhase CurrentPhase { get; private set; } = RoundPhase.Build;
	public int CurrentRound { get; private set; } = 1;
	public float PhaseTimeRemaining { get; private set; }
	public int EnemiesRemaining { get; private set; }
	public bool IsRoundActive { get; private set; } = false;

	private Timer _phaseTimer;
	private Timer _transitionTimer;
	private bool _autoStartNextRound = true;

	public override void _Ready()
	{
		Instance = this;
		
		_phaseTimer = new Timer();
		_phaseTimer.WaitTime = BuildPhaseDuration;
		_phaseTimer.OneShot = true;
		_phaseTimer.Timeout += OnPhaseTimeout;
		AddChild(_phaseTimer);

		_transitionTimer = new Timer();
		_transitionTimer.WaitTime = TransitionDuration;
		_transitionTimer.OneShot = true;
		_transitionTimer.Timeout += OnTransitionTimeout;
		AddChild(_transitionTimer);

		StartBuildPhase();
	}

	public override void _Process(double delta)
	{
		if (IsRoundActive)
		{
			PhaseTimeRemaining = (float)_phaseTimer.TimeLeft;
		}
	}

	public void StartRound()
	{
		if (IsRoundActive)
			return;

		IsRoundActive = true;
		GD.Print($"🎮 Starting Round {CurrentRound}");
		EmitSignal(SignalName.RoundStarted, CurrentRound);
		StartBuildPhase();
	}

	public void StartBuildPhase()
	{
		CurrentPhase = RoundPhase.Build;
		PhaseTimeRemaining = BuildPhaseDuration;
		
		_phaseTimer.WaitTime = BuildPhaseDuration;
		_phaseTimer.Start();

		GD.Print($"🔨 Build Phase Started - Round {CurrentRound} ({BuildPhaseDuration}s)");
		EmitSignal(SignalName.PhaseChanged, (int)CurrentPhase);
		EmitSignal(SignalName.BuildPhaseStarted, CurrentRound, BuildPhaseDuration);

		// Give player bonus money for new rounds
		if (CurrentRound > 1)
		{
			GameManager.Instance.AddMoney(BonusMoneyPerRound);
			GD.Print($"💰 Bonus money: +{BonusMoneyPerRound}");
		}
	}

	public void StartDefendPhase()
	{
		CurrentPhase = RoundPhase.Defend;
		_phaseTimer.Stop();

		// Calculate enemies for this round
		EnemiesRemaining = Mathf.RoundToInt(BaseEnemyCount * Mathf.Pow(EnemyCountMultiplier, CurrentRound - 1));
		
		GD.Print($"⚔️ Defend Phase Started - Round {CurrentRound} ({EnemiesRemaining} enemies)");
		EmitSignal(SignalName.PhaseChanged, (int)CurrentPhase);
		EmitSignal(SignalName.DefendPhaseStarted, CurrentRound, EnemiesRemaining);

		// Start enemy spawning
		if (EnemySpawner.Instance != null)
		{
			EnemySpawner.Instance.StartWave(EnemiesRemaining);
		}
	}

	public void ForceStartDefendPhase()
	{
		if (CurrentPhase == RoundPhase.Build)
		{
			_phaseTimer.Stop();
			StartDefendPhase();
		}
	}

	public void OnEnemyDefeated()
	{
		if (CurrentPhase == RoundPhase.Defend)
		{
			EnemiesRemaining--;
			GD.Print($"👾 Enemy defeated! Remaining: {EnemiesRemaining}");
			
			if (EnemiesRemaining <= 0)
			{
				CompleteRound();
			}
		}
	}

	public void CompleteRound()
	{
		if (!IsRoundActive)
			return;

		GD.Print($"✅ Round {CurrentRound} Completed!");
		EmitSignal(SignalName.RoundCompleted, CurrentRound);

		// Transition phase
		CurrentPhase = RoundPhase.Transition;
		EmitSignal(SignalName.PhaseChanged, (int)CurrentPhase);

		CurrentRound++;
		IsRoundActive = false;

		if (_autoStartNextRound)
		{
			_transitionTimer.Start();
		}
	}

	public void SetAutoStartNextRound(bool autoStart)
	{
		_autoStartNextRound = autoStart;
	}

	public float GetBuildPhaseProgress()
	{
		if (CurrentPhase != RoundPhase.Build)
			return 1.0f;
			
		return 1.0f - (PhaseTimeRemaining / BuildPhaseDuration);
	}

	private void OnPhaseTimeout()
	{
		if (CurrentPhase == RoundPhase.Build)
		{
			StartDefendPhase();
		}
	}

	private void OnTransitionTimeout()
	{
		StartRound();
	}

	// Debug methods
	public void _on_skip_build_phase_pressed()
	{
		ForceStartDefendPhase();
	}

	public void _on_next_round_pressed()
	{
		if (CurrentPhase == RoundPhase.Transition)
		{
			_transitionTimer.Stop();
			OnTransitionTimeout();
		}
	}
}
