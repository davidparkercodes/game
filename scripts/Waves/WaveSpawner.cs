using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class WaveSpawner : Node2D
{
	[Export] public PackedScene BasicEnemyScene;
	[Export] public WaveSetConfig WaveSet;
	[Export] public string WaveConfigPath = "res://data/waves/default_waves.json";

	[Signal] public delegate void WaveStartedEventHandler(int waveNumber, string waveName);
	[Signal] public delegate void WaveCompletedEventHandler(int waveNumber, int bonusMoney);
	[Signal] public delegate void EnemyGroupStartedEventHandler(string enemyType, int count);
	[Signal] public delegate void EnemyGroupCompletedEventHandler(string enemyType);
	[Signal] public delegate void AllWavesCompletedEventHandler();

	public static WaveSpawner Instance { get; private set; }

	public int CurrentWaveIndex { get; private set; } = 0;
	public WaveConfig CurrentWave => IsValidWaveIndex(CurrentWaveIndex) ? WaveSet.Waves[CurrentWaveIndex] : null;
	
	// Progress tracking properties
	public bool IsSpawning => _progressTracker?.EnemiesSpawned > 0 && _progressTracker?.EnemiesSpawned < _progressTracker?.TotalEnemiesInWave;
	public int TotalEnemiesInWave => _progressTracker?.TotalEnemiesInWave ?? 0;
	public int EnemiesSpawned => _progressTracker?.EnemiesSpawned ?? 0;
	public int EnemiesRemaining => _progressTracker?.EnemiesRemaining ?? 0;
	
	private WaveTimerManager _timerManager;
	private WaveEnemySpawner _enemySpawner;
	private WaveProgressTracker _progressTracker;

	public override void _Ready()
	{
		Instance = this;
		_timerManager = new WaveTimerManager(this);
		_enemySpawner = new WaveEnemySpawner();
		_progressTracker = new WaveProgressTracker();

		// Add child nodes
		AddChild(_enemySpawner);
		AddChild(_progressTracker);

		_enemySpawner.Initialize(BasicEnemyScene);
		
		// Connect signals
		_enemySpawner.EnemySpawned += OnEnemySpawned;
		_progressTracker.WaveCompleted += OnWaveCompleted;
		_progressTracker.EnemyGroupCompleted += OnEnemyGroupCompleted;

		LoadWaveConfiguration();

		GD.Print("🌊 WaveSpawner ready");
	}

	private void LoadWaveConfiguration()
	{
		WaveSet = WaveConfigLoader.LoadWaveSet(WaveConfigPath);
	}

	public void SetWaveIndex(int waveIndex)
	{
		CurrentWaveIndex = waveIndex;
		GD.Print($"🌊 Set wave index to {waveIndex}");
	}

	public void StartWave(int waveIndex = -1)
	{
		if (waveIndex >= 0)
		{
			SetWaveIndex(waveIndex);
		}

		if (!IsValidWaveIndex(CurrentWaveIndex))
		{
			GD.Print("🏆 All waves completed!");
			EmitSignal(SignalName.AllWavesCompleted);
			return;
		}

		var wave = CurrentWave;
		_progressTracker.StartTrackingWave(wave);

		GD.Print($"🌊 Starting {wave.WaveName} (Wave {wave.WaveNumber})");
		GD.Print($"📊 Wave details: {wave.EnemyGroups.Count} groups, PreWaveDelay: {wave.PreWaveDelay}s");
		EmitSignal(SignalName.WaveStarted, wave.WaveNumber, wave.WaveName);

		if (wave.PreWaveDelay > 0)
		{
			GD.Print($"⏰ Creating pre-wave delay timer for {wave.PreWaveDelay}s");
			_timerManager.CreateTimer(wave.PreWaveDelay, StartSpawningGroups);
		}
		else
		{
			StartSpawningGroups();
		}
	}

	private void StartSpawningGroups()
	{
		var wave = CurrentWave;
		if (wave == null)
		{
			GD.PrintErr("❌ StartSpawningGroups: CurrentWave is null!");
			return;
		}

		GD.Print($"🚀 StartSpawningGroups: Starting {wave.EnemyGroups.Count} enemy groups for {wave.WaveName}");
		foreach (int i in Enumerable.Range(0, wave.EnemyGroups.Count))
		{
			var group = wave.EnemyGroups[i];
			GD.Print($"📋 Group {i}: {group.Count}x {group.EnemyType}, StartDelay: {group.StartDelay}s");

			if (group.StartDelay > 0)
			{
				_timerManager.CreateTimer(group.StartDelay, () => StartEnemyGroupAtIndex(i));
			}
			else
			{
				StartEnemyGroupAtIndex(i);
			}
		}
	}

	private void StartEnemyGroupAtIndex(int groupIndex)
	{
		var wave = CurrentWave;
		if (wave == null) return;

		var group = wave.EnemyGroups[groupIndex];
		GD.Print($"👥 Starting enemy group {groupIndex}: {group.Count}x {group.EnemyType}");
		
		SpawnEnemiesInGroup(group, groupIndex);
	}

	private void SpawnEnemiesInGroup(EnemySpawnGroup group, int groupIndex)
	{
		GD.Print($"🚀 Initializing spawn for group {groupIndex}: {group.Count}x {group.EnemyType} with {group.SpawnInterval}s interval");
		EmitSignal(SignalName.EnemyGroupStarted, group.EnemyType, group.Count);

		// Spawn first enemy immediately
		SpawnEnemyInGroup(group, groupIndex);

		// Create timer for remaining enemies if there are more than 1
		if (group.Count > 1)
		{
			_timerManager.CreateRepeatingTimer(group.SpawnInterval, () =>
			{
				if (!_progressTracker.IsGroupCompletelySpawned(groupIndex))
				{
					SpawnEnemyInGroup(group, groupIndex);
				}
			});
		}
	}

	private void SpawnEnemyInGroup(EnemySpawnGroup group, int groupIndex)
	{
		Vector2 spawnPosition = _enemySpawner.GetSpawnPosition();
		var enemy = _enemySpawner.SpawnEnemy(group, spawnPosition);
		
		if (enemy != null)
		{
			// Connect enemy events
			enemy.EnemyKilled += () => OnEnemyKilled(_enemySpawner.CalculateEnemyReward(group, enemy));
			enemy.EnemyReachedEnd += OnEnemyReachedEnd;
			
			_progressTracker.OnEnemySpawned(groupIndex);
		}
	}

	private void OnEnemySpawned(Enemy enemy)
	{
		// Additional handling for spawned enemies if needed
	}

	private void OnEnemyKilled(int moneyReward)
	{
		GD.Print($"💀 WaveSpawner received enemy killed signal, reward: ${moneyReward}");
		
		if (GameManager.Instance != null)
		{
			GameManager.Instance.AddMoney(moneyReward);
		}
		
		CallDeferred(nameof(CheckWaveCompletion));
	}

	private void OnEnemyReachedEnd()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.OnEnemyReachedEnd();
		}
		
		CallDeferred(nameof(CheckWaveCompletion));
	}

	private void CheckWaveCompletion()
	{
		_progressTracker.CheckWaveCompletion();
	}

	private void OnWaveCompleted(int waveNumber, int bonusMoney)
	{
		GD.Print($"✅ Wave {waveNumber} completed! Bonus: ${bonusMoney}");
		EmitSignal(SignalName.WaveCompleted, waveNumber, bonusMoney);
		_timerManager.CleanupTimers();
	}

	private void OnEnemyGroupCompleted(string enemyType)
	{
		GD.Print($"✅ Enemy group {enemyType} completed!");
		EmitSignal(SignalName.EnemyGroupCompleted, enemyType);
	}

	public void StopCurrentWave()
	{
		GD.Print("🛑 Wave stopped");
		_progressTracker.StopTracking();
		_timerManager.CleanupTimers();
	}

	private bool IsValidWaveIndex(int index)
	{
		return WaveSet != null && index >= 0 && index < WaveSet.Waves.Count;
	}

	// Public getters for UI/HUD
	public int GetTotalWaves() => WaveSet?.Waves.Count ?? 0;
	public string GetCurrentWaveName() => CurrentWave?.WaveName ?? "Unknown";
	public string GetWaveSetName() => WaveSet?.SetName ?? "No Wave Set";
}
