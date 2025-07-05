using System;

namespace Game.Domain.Audio.ValueObjects;

public readonly struct SoundConfigData
{
    public string SoundKey { get; }
    public float Volume { get; }
    public string FilePath { get; }
    public bool Loop { get; }

    public SoundConfigData(string soundKey, float volume = 1.0f, string filePath = "", bool loop = false)
    {
        if (string.IsNullOrWhiteSpace(soundKey))
            throw new ArgumentException("Sound key cannot be empty", nameof(soundKey));
        if (volume < 0.0f)
            throw new ArgumentException("Volume cannot be negative", nameof(volume));

        SoundKey = soundKey;
        Volume = volume;
        FilePath = filePath ?? string.Empty;
        Loop = loop;
    }

    public override string ToString()
    {
        return $"SoundConfig({SoundKey}, Volume:{Volume:F2}, Loop:{Loop})";
    }

    public override bool Equals(object obj)
    {
        return obj is SoundConfigData other && Equals(other);
    }

    public bool Equals(SoundConfigData other)
    {
        return SoundKey == other.SoundKey &&
               Math.Abs(Volume - other.Volume) < 0.001f &&
               FilePath == other.FilePath &&
               Loop == other.Loop;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(SoundKey, Volume, FilePath, Loop);
    }

    public static bool operator ==(SoundConfigData left, SoundConfigData right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SoundConfigData left, SoundConfigData right)
    {
        return !left.Equals(right);
    }
}
