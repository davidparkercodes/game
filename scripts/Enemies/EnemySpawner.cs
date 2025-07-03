using Godot;
using System;

public partial class EnemySpawner : Node2D
{
	[Export] public PackedScene EnemyScene;
	[Export] public float SpawnInterval = 2.0f;
	[Export] public Vector2 SpawnXRange = new Vector2(64, 256);
	[Export] public float SpawnY = -32;

	private Timer _timer;

	public override void _Ready()
	{
		SpawnEnemy();
		StartTimer();
	}

	private void StartTimer()
	{
		_timer = new Timer
		{
			WaitTime = SpawnInterval,
			Autostart = true,
			OneShot = false
		};

		_timer.Timeout += () => SpawnEnemy();
		AddChild(_timer);
	}

	private void SpawnEnemy()
	{
		if (EnemyScene == null)
		{
			GD.PrintErr("❌ EnemyScene not assigned!");
			return;
		}

		var enemy = EnemyScene.Instantiate<Node2D>();
		enemy.GlobalPosition = GlobalPosition;
		GetTree().Root.CallDeferred("add_child", enemy);
	}
}
