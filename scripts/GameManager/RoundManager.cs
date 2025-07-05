using Godot;
using Game.Presentation.Player;

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
	[Export] public float TransitionDuration = 1.0f;
	[Export] public int BaseEnemyCount = 5;
	[Export] public float EnemyCountMultiplier = 1.2f;
	[Export] public int BonusMoneyPerRound = 25;
	[Export] public int MaxRounds = 5;

	public static RoundManager Instance { get; private set; }

	public RoundPhase CurrentPhase { get; private set; } = RoundPhase.Build;
	public int CurrentRound { get; private set; } = 1;
	public float PhaseTimeRemaining { get; private set; }
	public int EnemiesRemaining { get; private set; }
	public bool IsRoundActive { get; private set; } = false;
	public int TotalRounds => MaxRounds;

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
		GD.Print($"⏱️ Created phase timer with {BuildPhaseDuration}s duration");

		_transitionTimer = new Timer();
		_transitionTimer.WaitTime = TransitionDuration;
		_transitionTimer.OneShot = true;
		_transitionTimer.Timeout += OnTransitionTimeout;
		AddChild(_transitionTimer);
		GD.Print($"⏱️ Created transition timer with {TransitionDuration}s duration");

		// Start the first round properly
		StartRound();
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
			
		// Check if we've completed all rounds
		if (CurrentRound > MaxRounds)
		{
			GD.Print($"🏆 All {MaxRounds} rounds completed! Game should end.");
			return;
		}

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
		
		// Cancel any active build mode before starting defend phase
		var player = GetTree().GetFirstNodeInGroup("player") as Player;
		if (player != null)
		{
			player.CancelBuildMode();
		}
		
		if (SoundManager.Instance != null)
		{
			SoundManager.Instance.PlaySound("round_start");
		}

		GD.Print($"🎯 Starting defend phase for round {CurrentRound}");
		GD.Print($"🌊 WaveSpawner.Instance: {(WaveSpawner.Instance != null ? "Available" : "NULL")}");
		GD.Print($"👾 EnemySpawner.Instance: {(EnemySpawner.Instance != null ? "Available" : "NULL")}");

		if (WaveSpawner.Instance != null)
		{
			GD.Print($"🌊 Using WaveSpawner for round {CurrentRound}, starting wave {CurrentRound - 1}");
			WaveSpawner.Instance.StartWave(CurrentRound - 1);
			EnemiesRemaining = WaveSpawner.Instance.TotalEnemiesInWave;
			GD.Print($"⚔️ Defend Phase Started - Round {CurrentRound} using WaveSpawner with {EnemiesRemaining} enemies");
		}
		else
		{
			GD.Print($"👾 Using EnemySpawner fallback for round {CurrentRound}");
			EnemiesRemaining = Mathf.RoundToInt(BaseEnemyCount * Mathf.Pow(EnemyCountMultiplier, CurrentRound - 1));
			GD.Print($"⚔️ Defend Phase Started - Round {CurrentRound} ({EnemiesRemaining} enemies)");
			
			if (EnemySpawner.Instance != null)
			{
				EnemySpawner.Instance.StartWave(EnemiesRemaining);
			}
		}
		
		EmitSignal(SignalName.PhaseChanged, (int)CurrentPhase);
		EmitSignal(SignalName.DefendPhaseStarted, CurrentRound, EnemiesRemaining);
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
			if (WaveSpawner.Instance != null)
			{
				return;
			}
			
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
		{
			GD.Print($"⚠️ CompleteRound called but round not active! Current round: {CurrentRound}");
			return;
		}

		GD.Print($"✅ Round {CurrentRound} Completed!");
		EmitSignal(SignalName.RoundCompleted, CurrentRound);

		CurrentPhase = RoundPhase.Transition;
		EmitSignal(SignalName.PhaseChanged, (int)CurrentPhase);

		var completedRound = CurrentRound;
		CurrentRound++;
		IsRoundActive = false;

		GD.Print($"🔄 Transitioning from round {completedRound} to {CurrentRound}. Auto-start: {_autoStartNextRound}");

		if (_autoStartNextRound)
		{
			GD.Print($"⏰ Starting transition timer ({TransitionDuration}s)");
			_transitionTimer.Start();
		}
		else
		{
			GD.Print("⚠️ Auto-start disabled, waiting for manual trigger");
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
		GD.Print($"⏰ Transition timer expired, starting round {CurrentRound}");
		StartRound();
	}

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
