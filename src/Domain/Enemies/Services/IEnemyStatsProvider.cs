using Game.Domain.Enemies.ValueObjects;

namespace Game.Domain.Enemies.Services;

public interface IEnemyStatsProvider
{
    EnemyStats GetEnemyStats(string enemyType);
    bool HasEnemyStats(string enemyType);
}
