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
	public bool IsSpawning { get; private set; } = false;
	public int TotalEnemiesInWave { get; private set; } = 0;
	public int EnemiesSpawned { get; private set; } = 0;
	public int EnemiesRemaining => TotalEnemiesInWave - EnemiesSpawned;
	
	private Dictionary<string, PackedScene> _enemyScenes = new();
	private List<Timer> _activeTimers = new();
	private int _currentGroupIndex = 0;
	private bool _waveInProgress = false;
	private Dictionary<int, int> _groupSpawnCounts = new(); // Track spawned count per group

	public override void _Ready()
	{
		Instance = this;
		LoadWaveConfiguration();
		RegisterEnemyTypes();
		GD.Print("🌊 WaveSpawner ready");
	}

	private void LoadWaveConfiguration()
	{
		if (WaveSet == null)
		{
			if (FileAccess.FileExists(WaveConfigPath))
			{
				LoadWaveSetFromJson(WaveConfigPath);
				GD.Print($"📋 Loaded wave set from JSON: {WaveSet.SetName}");
			}
			else
			{
				CreateDefaultWaveSet();
				GD.Print("⚠️ No wave config found, created default waves");
			}
		}
	}

	private void LoadWaveSetFromJson(string jsonPath)
	{
		try
		{
			var file = FileAccess.Open(jsonPath, FileAccess.ModeFlags.Read);
			if (file == null)
			{
				GD.PrintErr($"❌ Failed to open wave config file: {jsonPath}");
				return;
			}

			string jsonText = file.GetAsText();
			file.Close();

			var json = new Json();
			var parseResult = json.Parse(jsonText);
			if (parseResult != Error.Ok)
			{
				GD.PrintErr($"❌ Failed to parse JSON: {parseResult}");
				return;
			}

			var jsonData = json.Data.AsGodotDictionary();
			WaveSet = ParseWaveSetFromJson(jsonData);
		}
		catch (System.Exception e)
		{
			GD.PrintErr($"❌ Error loading wave config: {e.Message}");
			CreateDefaultWaveSet();
		}
	}

	private WaveSetConfig ParseWaveSetFromJson(Godot.Collections.Dictionary jsonData)
	{
		var waveSet = new WaveSetConfig();
		waveSet.SetName = jsonData.GetValueOrDefault("setName", "Unknown Wave Set").AsString();
		waveSet.Description = jsonData.GetValueOrDefault("description", "").AsString();

		if (jsonData.ContainsKey("waves"))
		{
			var wavesArray = jsonData["waves"].AsGodotArray();
			foreach (var waveData in wavesArray)
			{
				var waveDict = waveData.AsGodotDictionary();
				var wave = ParseWaveFromJson(waveDict);
				waveSet.Waves.Add(wave);
			}
		}

		return waveSet;
	}

	private WaveConfig ParseWaveFromJson(Godot.Collections.Dictionary waveData)
	{
		var wave = new WaveConfig();
		wave.WaveNumber = waveData.GetValueOrDefault("waveNumber", 1).AsInt32();
		wave.WaveName = waveData.GetValueOrDefault("waveName", "Wave").AsString();
		wave.Description = waveData.GetValueOrDefault("description", "").AsString();
		wave.PreWaveDelay = waveData.GetValueOrDefault("preWaveDelay", 0.0f).AsSingle();
		wave.PostWaveDelay = waveData.GetValueOrDefault("postWaveDelay", 2.0f).AsSingle();
		wave.BonusMoney = waveData.GetValueOrDefault("bonusMoney", 25).AsInt32();

		if (waveData.ContainsKey("enemyGroups"))
		{
			var groupsArray = waveData["enemyGroups"].AsGodotArray();
			foreach (var groupData in groupsArray)
			{
				var groupDict = groupData.AsGodotDictionary();
				var group = ParseEnemyGroupFromJson(groupDict);
				wave.EnemyGroups.Add(group);
			}
		}

		return wave;
	}

	private EnemySpawnGroup ParseEnemyGroupFromJson(Godot.Collections.Dictionary groupData)
	{
		var group = new EnemySpawnGroup();
		group.EnemyType = groupData.GetValueOrDefault("enemyType", "Basic").AsString();
		group.Count = groupData.GetValueOrDefault("count", 5).AsInt32();
		group.SpawnInterval = groupData.GetValueOrDefault("spawnInterval", 1.0f).AsSingle();
		group.StartDelay = groupData.GetValueOrDefault("startDelay", 0.0f).AsSingle();
		group.HealthMultiplier = groupData.GetValueOrDefault("healthMultiplier", 1.0f).AsSingle();
		group.SpeedMultiplier = groupData.GetValueOrDefault("speedMultiplier", 1.0f).AsSingle();
		group.MoneyReward = groupData.GetValueOrDefault("moneyReward", 10).AsInt32();
		return group;
	}

	private void RegisterEnemyTypes()
	{
		// Register enemy scene mappings
		_enemyScenes["Basic"] = BasicEnemyScene;
		// Add more enemy types here as they're created
		// _enemyScenes["Fast"] = FastEnemyScene;
		// _enemyScenes["Heavy"] = HeavyEnemyScene;
	}

	private void CreateDefaultWaveSet()
	{
		WaveSet = new WaveSetConfig();
		WaveSet.SetName = "Default Waves";
		WaveSet.Description = "Auto-generated default wave progression";
		
		// Create 10 default waves with increasing difficulty
		for (int i = 1; i <= 10; i++)
		{
			var wave = new WaveConfig();
			wave.WaveNumber = i;
			wave.WaveName = $"Wave {i}";
			wave.BonusMoney = 25 + (i * 5);
			wave.Description = $"Standard wave {i} with {5 + i * 2} enemies";
			
			var enemyGroup = new EnemySpawnGroup();
			enemyGroup.EnemyType = "Basic";
			enemyGroup.Count = 5 + (i * 2); // Increase enemy count each wave
			enemyGroup.SpawnInterval = Mathf.Max(0.5f, 2.0f - (i * 0.1f)); // Faster spawning
			enemyGroup.HealthMultiplier = 1.0f + (i * 0.15f); // More HP
			enemyGroup.SpeedMultiplier = 1.0f + (i * 0.05f); // Slightly faster
			enemyGroup.MoneyReward = 10 + (i * 2); // More money per enemy
			
			wave.EnemyGroups.Add(enemyGroup);
			WaveSet.Waves.Add(wave);
		}
	}

	public void SetWaveIndex(int waveIndex)
	{
		CurrentWaveIndex = waveIndex;
		GD.Print($"🌊 Set wave index to {waveIndex}");
	}
	
	public void StartWave(int waveIndex = -1)
	{
		if (_waveInProgress)
		{
			GD.PrintErr("❌ Cannot start wave: another wave is in progress");
			return;
		}

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
		_waveInProgress = true;
		_currentGroupIndex = 0;
		EnemiesSpawned = 0;
		_groupSpawnCounts.Clear(); // Reset group spawn tracking
		
		// Calculate total enemies in this wave
		TotalEnemiesInWave = wave.EnemyGroups.Sum(group => group.Count);
		
		GD.Print($"🌊 Starting {wave.WaveName} (Wave {wave.WaveNumber}) - {TotalEnemiesInWave} enemies");
		GD.Print($"📊 Wave details: {wave.EnemyGroups.Count} groups, PreWaveDelay: {wave.PreWaveDelay}s");
		EmitSignal(SignalName.WaveStarted, wave.WaveNumber, wave.WaveName);
		
		// Start with pre-wave delay if configured
		if (wave.PreWaveDelay > 0)
		{
			GD.Print($"⏰ Creating pre-wave delay timer for {wave.PreWaveDelay}s");
			var preDelayTimer = CreateTimer(wave.PreWaveDelay);
			preDelayTimer.Timeout += () => {
				GD.Print("⏰ Pre-wave delay timer fired, starting spawning groups");
				StartSpawningGroups();
			};
		}
		else
		{
			GD.Print("▶️ No pre-wave delay, starting spawning groups immediately");
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
		IsSpawning = true;
		
		// Start all enemy groups with their respective delays
		for (int i = 0; i < wave.EnemyGroups.Count; i++)
		{
			var group = wave.EnemyGroups[i];
			GD.Print($"📋 Group {i}: {group.Count}x {group.EnemyType}, StartDelay: {group.StartDelay}s");
			
			if (group.StartDelay > 0)
			{
				GD.Print($"⏰ Delaying group {i} start by {group.StartDelay}s");
				var groupDelayTimer = CreateTimer(group.StartDelay);
				var groupIndex = i; // Capture for closure
				groupDelayTimer.Timeout += () => {
					GD.Print($"⏰ Group {groupIndex} delay timer fired after {group.StartDelay}s");
					StartEnemyGroupAtIndex(groupIndex);
				};
			}
			else
			{
				GD.Print($"▶️ Starting group {i} immediately");
				StartEnemyGroupAtIndex(i);
			}
		}
	}

	private void StartEnemyGroupAtIndex(int groupIndex)
	{
		var wave = CurrentWave;
		if (wave == null || groupIndex >= wave.EnemyGroups.Count)
		{
			GD.PrintErr($"❌ Invalid group index {groupIndex} for wave with {wave?.EnemyGroups.Count ?? 0} groups");
			return;
		}
		
		var group = wave.EnemyGroups[groupIndex];
		
		GD.Print($"👥 Starting enemy group {groupIndex}: {group.Count}x {group.EnemyType}");
		EmitSignal(SignalName.EnemyGroupStarted, group.EnemyType, group.Count);
		
		// Spawn enemies in this group with intervals
		SpawnEnemyGroupAtIndex(group, groupIndex);
	}

	private void SpawnEnemyGroupAtIndex(EnemySpawnGroup group, int groupIndex)
	{
		if (!_enemyScenes.ContainsKey(group.EnemyType))
		{
			GD.PrintErr($"❌ Unknown enemy type: {group.EnemyType}");
			return;
		}

		var enemyScene = _enemyScenes[group.EnemyType];
		_groupSpawnCounts[groupIndex] = 0; // Initialize spawn count for this group

		GD.Print($"🚀 Initializing spawn for group {groupIndex}: {group.Count}x {group.EnemyType} with {group.SpawnInterval}s interval");

		// Create a timer for spawning enemies in this group
		var spawnTimer = CreateTimer(group.SpawnInterval);
		spawnTimer.Timeout += () => SpawnEnemyFromGroup(enemyScene, group, groupIndex, spawnTimer);
		
		// Start spawning immediately for the first enemy
		SpawnEnemyFromGroup(enemyScene, group, groupIndex, spawnTimer);
	}
	
	private void SpawnEnemyFromGroup(PackedScene enemyScene, EnemySpawnGroup group, int groupIndex, Timer spawnTimer)
	{
		var enemiesSpawnedInGroup = _groupSpawnCounts.GetValueOrDefault(groupIndex, 0);
		
		GD.Print($"⏰ Spawn timer fired: Group {groupIndex} has {enemiesSpawnedInGroup}/{group.Count}, Wave in progress: {_waveInProgress}");
		
		if (enemiesSpawnedInGroup < group.Count && _waveInProgress)
		{
			SpawnEnemy(enemyScene, group);
			enemiesSpawnedInGroup++;
			_groupSpawnCounts[groupIndex] = enemiesSpawnedInGroup;
			EnemiesSpawned++;
			GD.Print($"👾 Spawned enemy {enemiesSpawnedInGroup}/{group.Count} in group {groupIndex}, total spawned: {EnemiesSpawned}/{TotalEnemiesInWave}");
			
			// Continue spawning if more enemies in this group
				if (enemiesSpawnedInGroup < group.Count)
				{
					GD.Print($"⏰ Restarting spawn timer for next enemy in {group.SpawnInterval}s");
					spawnTimer.Start(); // This one needs to stay since it's restarting an existing timer
				}
			else
			{
				// This group is complete
				GD.Print($"✅ Enemy group {groupIndex} ({group.EnemyType}) completed!");
				EmitSignal(SignalName.EnemyGroupCompleted, group.EnemyType);
				CallDeferred(MethodName.CheckWaveCompletion);
			}
		}
		else
		{
			GD.Print($"❌ Spawn timer fired but conditions not met: enemiesSpawned={enemiesSpawnedInGroup}/{group.Count}, waveInProgress={_waveInProgress}");
		}
	}

	private void SpawnEnemy(PackedScene enemyScene, EnemySpawnGroup group)
	{
		var enemy = enemyScene.Instantiate<Enemy>();
		
		// Apply group modifiers
		enemy.MaxHealth = Mathf.RoundToInt(enemy.MaxHealth * group.HealthMultiplier);
		enemy.Speed *= group.SpeedMultiplier;
		
		// Set spawn position using PathManager if available
		if (PathManager.Instance != null)
		{
			enemy.GlobalPosition = PathManager.Instance.GetSpawnPosition();
		}
		else
		{
			enemy.GlobalPosition = GlobalPosition;
		}
		
		// Connect to enemy events
		enemy.EnemyKilled += () => OnEnemyKilled(group.MoneyReward);
		enemy.EnemyReachedEnd += OnEnemyReachedEnd;
		
		GetTree().Root.AddChild(enemy);
		GD.Print($"👾 Spawned {group.EnemyType} enemy ({EnemiesSpawned}/{TotalEnemiesInWave})");
	}

	private void OnEnemyKilled(int moneyReward)
	{
		GD.Print($"💀 WaveSpawner received enemy killed signal, reward: ${moneyReward}");
		
		// Award money through GameManager
		if (GameManager.Instance != null)
		{
			GameManager.Instance.AddMoney(moneyReward);
		}
		
		// Use CallDeferred to check wave completion after QueueFree has processed
		CallDeferred(MethodName.CheckWaveCompletion);
	}

	private void OnEnemyReachedEnd()
	{
		// Handle enemy reaching end through GameManager
		if (GameManager.Instance != null)
		{
			GameManager.Instance.OnEnemyReachedEnd();
		}
		
		// Use CallDeferred to check wave completion after QueueFree has processed
		CallDeferred(MethodName.CheckWaveCompletion);
	}

	private void CheckWaveCompletion()
	{
		// Check if all enemies have been spawned and defeated/reached end
		var enemiesInScene = GetTree().GetNodesInGroup("enemies").Count;
		
		GD.Print($"🔍 Wave completion check: Spawned={EnemiesSpawned}/{TotalEnemiesInWave}, EnemiesInScene={enemiesInScene}, WaveInProgress={_waveInProgress}");
		
		if (EnemiesSpawned >= TotalEnemiesInWave && enemiesInScene == 0)
		{
			GD.Print("✅ Wave completion conditions met - calling CompleteWave()");
			CompleteWave();
		}
		else
		{
			GD.Print($"❌ Wave not complete yet - need {TotalEnemiesInWave - EnemiesSpawned} more spawned or {enemiesInScene} enemies still alive");
		}
	}

	private void CompleteWave()
	{
		if (!_waveInProgress)
		{
			GD.Print("⚠️ CompleteWave called but wave not in progress - skipping");
			return;
		}
		
		var wave = CurrentWave;
		GD.Print($"🏁 CompleteWave called: {EnemiesSpawned}/{TotalEnemiesInWave} spawned, stopping wave");
		_waveInProgress = false;
		IsSpawning = false;
		
		GD.Print($"✅ {wave.WaveName} completed! Bonus: ${wave.BonusMoney}");
		
		// Award bonus money
		if (GameManager.Instance != null)
		{
			GameManager.Instance.AddMoney(wave.BonusMoney);
		}
		
		EmitSignal(SignalName.WaveCompleted, wave.WaveNumber, wave.BonusMoney);
		
		// Clean up timers
		CleanupTimers();
		
		// Don't auto-advance wave index - let RoundManager control progression
		// Post-wave delay is handled by RoundManager transition
	}

	public void StopCurrentWave()
	{
		_waveInProgress = false;
		IsSpawning = false;
		CleanupTimers();
		GD.Print("🛑 Wave stopped");
	}

	private Timer CreateTimer(float waitTime)
	{
		var timer = new Timer();
		timer.WaitTime = waitTime;
		timer.OneShot = true;
		AddChild(timer);
		_activeTimers.Add(timer);
		GD.Print($"⏱️ Created timer with {waitTime}s wait time. Active timers: {_activeTimers.Count}");
		// Auto-start the timer
		timer.Start();
		GD.Print($"▶️ Timer started automatically");
		return timer;
	}

	private void CleanupTimers()
	{
		GD.Print($"🧹 Cleaning up {_activeTimers.Count} timers");
		foreach (var timer in _activeTimers)
		{
			if (IsInstanceValid(timer))
			{
				timer.QueueFree();
			}
		}
		_activeTimers.Clear();
		GD.Print("🧹 Timer cleanup complete");
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
