using Godot;
using System;

public partial class EnemySpawner : Node2D
{
	[Export] public PackedScene EnemyScene;
	[Export] public float BaseSpawnInterval = 2.0f;
	[Export] public Vector2 SpawnXRange = new Vector2(64, 256);
	[Export] public float SpawnY = -32;

	public static EnemySpawner Instance { get; private set; }

	private Timer _spawnTimer;
	private int _enemiesToSpawn = 0;
	private bool _isSpawning = false;
	private float _currentSpawnInterval;

	public override void _Ready()
	{
		Instance = this;
		_currentSpawnInterval = BaseSpawnInterval;
		
		_spawnTimer = new Timer();
		_spawnTimer.WaitTime = _currentSpawnInterval;
		_spawnTimer.OneShot = true;
		_spawnTimer.Timeout += OnSpawnTimerTimeout;
		AddChild(_spawnTimer);

		GD.Print("🏭 EnemySpawner ready");
	}

	public void StartWave(int enemyCount)
	{
		if (_isSpawning)
		{
			GD.PrintErr("⚠️ Already spawning enemies!");
			return;
		}

		_enemiesToSpawn = enemyCount;
		_isSpawning = true;
		
		GD.Print($"🌊 Starting wave with {enemyCount} enemies");
		
		// Start spawning immediately
		SpawnNextEnemy();
	}

	public void StopWave()
	{
		_isSpawning = false;
		_enemiesToSpawn = 0;
		_spawnTimer.Stop();
		GD.Print("🛑 Wave stopped");
	}

	private void SpawnNextEnemy()
	{
		if (!_isSpawning || _enemiesToSpawn <= 0)
		{
			_isSpawning = false;
			GD.Print("✅ Wave spawning completed");
			return;
		}

		SpawnEnemy();
		_enemiesToSpawn--;

		// Schedule next spawn if there are more enemies
		if (_enemiesToSpawn > 0)
		{
			_spawnTimer.Start();
		}
		else
		{
			_isSpawning = false;
			GD.Print("✅ All enemies spawned for this wave");
		}
	}

	private void SpawnEnemy()
	{
		if (EnemyScene == null)
		{
			GD.PrintErr("❌ EnemyScene not assigned!");
			return;
		}

		var enemy = EnemyScene.Instantiate<Enemy>();
		
		// Use PathManager for spawn position if available
		if (PathManager.Instance != null)
		{
			enemy.GlobalPosition = PathManager.Instance.GetSpawnPosition();
		}
		else
		{
			// Fallback to old method
			var randomX = GD.Randf() * (SpawnXRange.Y - SpawnXRange.X) + SpawnXRange.X;
			enemy.GlobalPosition = new Vector2(randomX, GlobalPosition.Y + SpawnY);
		}
		
		// Connect enemy signals to GameManager
		if (GameManager.Instance != null)
		{
			enemy.EnemyKilled += GameManager.Instance.OnEnemyKilled;
		}
		
		GetTree().Root.CallDeferred("add_child", enemy);
		GD.Print($"👾 Enemy spawned at {enemy.GlobalPosition}");
	}

	private void OnSpawnTimerTimeout()
	{
		SpawnNextEnemy();
	}

	public void SetSpawnInterval(float interval)
	{
		_currentSpawnInterval = interval;
		_spawnTimer.WaitTime = interval;
	}

	public bool IsCurrentlySpawning()
	{
		return _isSpawning;
	}

	public int GetRemainingEnemies()
	{
		return _enemiesToSpawn;
	}
}
