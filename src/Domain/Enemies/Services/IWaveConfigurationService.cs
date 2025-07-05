using Game.Domain.Enemies.ValueObjects;

namespace Game.Domain.Enemies.Services;

public interface IWaveConfigurationService
{
    WaveConfiguration LoadWaveSet(string difficulty = "default");
    WaveConfiguration LoadWaveSetFromPath(string configurationPath);
    WaveConfiguration CreateDefaultWaveSet();
    string[] GetAvailableWaveSets();
    void ClearCache();
}
