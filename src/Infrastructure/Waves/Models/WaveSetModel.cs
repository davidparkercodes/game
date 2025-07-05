using System.Collections.Generic;

namespace Game.Infrastructure.Waves.Models;

internal class WaveSetModel
{
    public string SetName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<WaveModel> Waves { get; set; } = new List<WaveModel>();
}
