using Game.Domain.Audio.Enums;
using Game.Domain.Shared.ValueObjects;

namespace Game.Domain.Audio.ValueObjects;

public readonly struct SoundRequest
{
    public string SoundKey { get; }
    public SoundCategory Category { get; }
    public float VolumeDb { get; }
    public Position? Position { get; }
    public Position? ListenerPosition { get; }
    public float MaxDistance { get; }

    public SoundRequest(
        string soundKey, 
        SoundCategory category = SoundCategory.SFX, 
        float volumeDb = 0.0f,
        Position? position = null,
        Position? listenerPosition = null,
        float maxDistance = 500.0f)
    {
        SoundKey = soundKey;
        Category = category;
        VolumeDb = volumeDb;
        Position = position;
        ListenerPosition = listenerPosition;
        MaxDistance = maxDistance;
    }

    public bool IsPositional => Position.HasValue && ListenerPosition.HasValue;
}
