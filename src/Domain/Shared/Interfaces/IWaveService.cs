using Game.Domain.ValueObjects;

namespace Game.Domain.Shared.Interfaces;

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
    void LoadWaveConfiguration(LevelConfiguration levelConfig);
}
