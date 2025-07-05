using Game.Domain.Enemies.ValueObjects;
using Game.Domain.Levels.ValueObjects;

namespace Game.Domain.Enemies.Services;

public interface IWaveService
{
    void StartWave(int waveNumber);
    void StopCurrentWave();
    bool IsWaveActive();
    int GetCurrentWaveNumber();
    int GetRemainingEnemies();
    float GetWaveProgress();
    EnemyStats GetNextEnemyType();
    bool IsWaveComplete();
    void LoadWaveConfiguration(LevelData levelConfiguration);
}
