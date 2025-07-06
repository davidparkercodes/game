using System.Collections.Generic;
using Game.Domain.Shared.ValueObjects;

namespace Game.Application.Simulation.ValueObjects;

public class GameState
{
    public int Money { get; private set; }
    public int Lives { get; private set; }
    public int Score { get; private set; }
    public int CurrentWave { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsVictory { get; private set; }
    public List<SimulatedBuilding> Buildings { get; private set; }
    public List<SimulatedEnemy> Enemies { get; private set; }

    public GameState(int startingMoney, int startingLives)
    {
        Money = startingMoney;
        Lives = startingLives;
        Score = 0;
        CurrentWave = 0;
        IsGameOver = false;
        IsVictory = false;
        Buildings = new List<SimulatedBuilding>();
        Enemies = new List<SimulatedEnemy>();
    }

    public void SpendMoney(int amount)
    {
        if (amount > Money)
            throw new System.InvalidOperationException($"Cannot spend {amount} money, only have {Money}");
        
        Money -= amount;
    }

    public void AddMoney(int amount)
    {
        Money += amount;
    }

    public void LoseLife()
    {
        Lives--;
        if (Lives <= 0)
        {
            IsGameOver = true;
        }
    }

    public void AddScore(int points)
    {
        Score += points;
    }

    public void StartWave(int waveNumber)
    {
        CurrentWave = waveNumber;
        Enemies.Clear(); // Clear previous wave enemies
    }

    public void CompleteWave(int maxWaves)
    {
        if (CurrentWave >= maxWaves)
        {
            IsGameOver = true;
            IsVictory = true;
        }
    }

    public void AddBuilding(SimulatedBuilding building)
    {
        Buildings.Add(building);
    }

    public void AddEnemy(SimulatedEnemy enemy)
    {
        Enemies.Add(enemy);
    }

    public void RemoveDeadEnemies()
    {
        Enemies.RemoveAll(e => !e.IsAlive);
    }
}

public class SimulatedBuilding
{
    public string BuildingType { get; }
    public Position Position { get; }
    public float Damage { get; }
    public float Range { get; }
    public float AttackSpeed { get; }
    public int Cost { get; }

    public SimulatedBuilding(string buildingType, Position position, float damage, float range, float attackSpeed, int cost)
    {
        BuildingType = buildingType;
        Position = position;
        Damage = damage;
        Range = range;
        AttackSpeed = attackSpeed;
        Cost = cost;
    }
}

public class SimulatedEnemy
{
    public string EnemyType { get; }
    public float Health { get; private set; }
    public float MaxHealth { get; }
    public float Speed { get; }
    public int Reward { get; }
    public Position Position { get; set; }
    public bool IsAlive => Health > 0;

    public SimulatedEnemy(string enemyType, float health, float speed, int reward, Position startPosition)
    {
        EnemyType = enemyType;
        Health = health;
        MaxHealth = health;
        Speed = speed;
        Reward = reward;
        Position = startPosition;
    }

    public void TakeDamage(float damage)
    {
        Health = System.Math.Max(0, Health - damage);
    }

    public void Move(Position newPosition)
    {
        Position = newPosition;
    }
}
