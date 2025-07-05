using System.Collections.Generic;

namespace Game.Infrastructure.Waves;

internal class WaveSetConfigurationInternal
{
    public string SetName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<WaveConfigurationInternal> Waves { get; set; } = new List<WaveConfigurationInternal>();
}
