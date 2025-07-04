using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class WaveProgressTracker : Node
{
	[Signal] public delegate void WaveCompletedEventHandler(int waveNumber, int bonusMoney);
	[Signal] public delegate void EnemyGroupCompletedEventHandler(string enemyType);

	public int TotalEnemiesInWave { get; private set; }
	public int EnemiesSpawned { get; private set; }
	public int EnemiesRemaining => TotalEnemiesInWave - EnemiesSpawned;

	private Dictionary<int, int> _groupSpawnCounts = new();
	private WaveConfig _currentWave;
	private bool _waveInProgress;

	public void StartTrackingWave(WaveConfig wave)
	{
		_currentWave = wave;
		_waveInProgress = true;
		EnemiesSpawned = 0;
		_groupSpawnCounts.Clear();
		
		TotalEnemiesInWave = wave.EnemyGroups.Sum(group => group.Count);
		GD.Print($"📊 Started tracking wave: {TotalEnemiesInWave} total enemies");
	}

	public void OnEnemySpawned(int groupIndex)
	{
		if (!_waveInProgress) return;

		var currentCount = _groupSpawnCounts.GetValueOrDefault(groupIndex, 0);
		_groupSpawnCounts[groupIndex] = currentCount + 1;
		EnemiesSpawned++;

		GD.Print($"👾 Enemy spawned: Group {groupIndex} now has {currentCount + 1} spawned, total: {EnemiesSpawned}/{TotalEnemiesInWave}");

		CheckGroupCompletion(groupIndex);
	}

	private void CheckGroupCompletion(int groupIndex)
	{
		if (_currentWave == null || groupIndex >= _currentWave.EnemyGroups.Count) return;

		var group = _currentWave.EnemyGroups[groupIndex];
		var spawnedCount = _groupSpawnCounts.GetValueOrDefault(groupIndex, 0);

		if (spawnedCount >= group.Count)
		{
			GD.Print($"✅ Enemy group {groupIndex} ({group.EnemyType}) spawning completed!");
			EmitSignal(SignalName.EnemyGroupCompleted, group.EnemyType);
		}
	}

	public void CheckWaveCompletion()
	{
		if (!_waveInProgress || _currentWave == null) return;

		var enemiesInScene = GetTree().GetNodesInGroup("enemies").Count;
		
		GD.Print($"🔍 Wave completion check: Spawned={EnemiesSpawned}/{TotalEnemiesInWave}, EnemiesInScene={enemiesInScene}");
		
		if (EnemiesSpawned >= TotalEnemiesInWave && enemiesInScene == 0)
		{
			CompleteWave();
		}
	}

	private void CompleteWave()
	{
		if (!_waveInProgress) return;

		_waveInProgress = false;
		GD.Print($"✅ {_currentWave.WaveName} completed! Bonus: ${_currentWave.BonusMoney}");

		if (GameManager.Instance != null)
		{
			GameManager.Instance.AddMoney(_currentWave.BonusMoney);
		}

		EmitSignal(SignalName.WaveCompleted, _currentWave.WaveNumber, _currentWave.BonusMoney);
	}

	public void StopTracking()
	{
		_waveInProgress = false;
		GD.Print("🛑 Wave tracking stopped");
	}

	public bool IsGroupCompletelySpawned(int groupIndex)
	{
		if (_currentWave == null || groupIndex >= _currentWave.EnemyGroups.Count) return false;
		
		var group = _currentWave.EnemyGroups[groupIndex];
		var spawnedCount = _groupSpawnCounts.GetValueOrDefault(groupIndex, 0);
		return spawnedCount >= group.Count;
	}

	public int GetGroupSpawnedCount(int groupIndex)
	{
		return _groupSpawnCounts.GetValueOrDefault(groupIndex, 0);
	}
}
