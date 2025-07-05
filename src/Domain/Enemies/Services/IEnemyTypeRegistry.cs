using System.Collections.Generic;
using Game.Domain.Enemies.ValueObjects;

namespace Game.Domain.Enemies.Services;

public interface IEnemyTypeRegistry
{
    EnemyType? GetByInternalId(string internalId);
    EnemyType? GetByConfigKey(string configKey);
    IEnumerable<EnemyType> GetByCategory(string category);
    IEnumerable<EnemyType> GetByTier(int tier);
    EnemyType? GetDefaultType();
    EnemyType? GetBasicType();
    bool IsValidConfigKey(string configKey);
    bool IsValidInternalId(string internalId);
    IEnumerable<EnemyType> GetAllTypes();
    IEnumerable<string> GetAllCategories();
    IEnumerable<int> GetAllTiers();
    IEnumerable<EnemyType> GetEnemiesForWave(int waveNumber);
    EnemyType? GetEnemyTypeForWaveProgression(int waveNumber, int enemyIndex);
    bool IsEnemyAvailableForWave(string configKey, int waveNumber);
}
