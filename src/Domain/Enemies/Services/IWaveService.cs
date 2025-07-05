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
    
    // Additional wave management methods
    void PauseWave();
    void ResumeWave();
    void Reset();
    int GetTotalWaves();
    bool LoadWaveSet(string difficulty);
    string[] GetAvailableWaveSets();
    string GetCurrentWaveSetName();
    void Initialize();
}
