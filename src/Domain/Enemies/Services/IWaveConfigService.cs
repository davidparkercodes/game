using Game.Domain.Enemies.ValueObjects;

namespace Game.Domain.Enemies.Services;

public interface IWaveConfigService
{
    WaveConfiguration LoadWaveSet(string configPath);
    WaveConfiguration CreateDefaultWaveSet();
}
