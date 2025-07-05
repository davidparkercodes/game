namespace Game.Infrastructure.Interfaces;

public interface IWaveConfigService
{
    WaveSetConfig LoadWaveSet(string jsonPath);
    WaveSetConfig CreateDefaultWaveSet();
}
