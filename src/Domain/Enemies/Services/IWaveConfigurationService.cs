using Game.Domain.Enemies.ValueObjects;

namespace Game.Domain.Enemies.Services;

public interface IWaveConfigurationService
{
    WaveConfiguration LoadWaveSet(string configPath);
    WaveConfiguration CreateDefaultWaveSet();
}
