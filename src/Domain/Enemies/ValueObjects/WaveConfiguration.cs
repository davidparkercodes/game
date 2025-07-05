namespace Game.Domain.Enemies.ValueObjects;

public readonly struct WaveConfiguration
{
    public string Name { get; }
    public int WaveCount { get; }
    public string JsonData { get; }

    public WaveConfiguration(string name, int waveCount, string jsonData)
    {
        Name = name;
        WaveCount = waveCount;
        JsonData = jsonData;
    }

    public static WaveConfiguration CreateDefault()
    {
        return new WaveConfiguration(
            "Default Wave Set",
            5,
            "{\"waves\": []}"
        );
    }
}
