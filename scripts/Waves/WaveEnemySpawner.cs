using Godot;
using System;
using System.Collections.Generic;

public partial class WaveEnemySpawner : Node
{
	[Signal] public delegate void EnemySpawnedEventHandler(Enemy enemy);
	
	private Dictionary<string, PackedScene> _enemyScenes = new();
	private PackedScene _basicEnemyScene;

	public void Initialize(PackedScene basicEnemyScene)
	{
		_basicEnemyScene = basicEnemyScene;
		RegisterEnemyTypes();
	}

	private void RegisterEnemyTypes()
	{
		_enemyScenes["Basic"] = _basicEnemyScene;
		_enemyScenes["basic_enemy"] = _basicEnemyScene;
		_enemyScenes["fast_enemy"] = _basicEnemyScene;
		_enemyScenes["tank_enemy"] = _basicEnemyScene;
		_enemyScenes["elite_enemy"] = _basicEnemyScene;
		
		GD.Print($"📋 Registered {_enemyScenes.Count} enemy types for spawning");
	}

	public Enemy SpawnEnemy(EnemySpawnGroup group, Vector2 spawnPosition)
	{
		if (!_enemyScenes.ContainsKey(group.EnemyType))
		{
			GD.PrintErr($"❌ Unknown enemy type: {group.EnemyType}");
			return null;
		}

		var enemyScene = _enemyScenes[group.EnemyType];
		var enemy = enemyScene.Instantiate<Enemy>();
		
		enemy.EnemyType = group.EnemyType;
		enemy.GlobalPosition = spawnPosition;
		
		CallDeferred(MethodName.ApplyWaveModifiers, enemy, group);
		
		GetTree().Root.AddChild(enemy);
		EmitSignal(SignalName.EnemySpawned, enemy);
		
		GD.Print($"👾 Spawned {group.EnemyType} enemy at {spawnPosition}");
		return enemy;
	}

	private void ApplyWaveModifiers(Enemy enemy, EnemySpawnGroup group)
	{
		var baseHealth = enemy.MaxHealth;
		var baseSpeed = enemy.Speed;
		
		enemy.ApplyHealthMultiplier(group.HealthMultiplier);
		enemy.SetSpeed(baseSpeed * group.SpeedMultiplier);
		
		GD.Print($"🔧 Applied wave modifiers to {group.EnemyType}: HP {baseHealth}→{enemy.MaxHealth}, Speed {baseSpeed:F1}→{enemy.Speed:F1}");
	}

	public int CalculateEnemyReward(EnemySpawnGroup group, Enemy enemy)
	{
		var baseReward = enemy.RewardGold;
		var waveBonus = group.MoneyReward;
		var totalReward = baseReward + waveBonus;
		
		GD.Print($"💰 Enemy reward: {baseReward} (base) + {waveBonus} (wave bonus) = {totalReward}");
		return totalReward;
	}

	public Vector2 GetSpawnPosition()
	{
		if (PathManager.Instance != null)
		{
			return PathManager.Instance.GetSpawnPosition();
		}
		
		return Vector2.Zero;
	}
}
