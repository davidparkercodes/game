using Godot;
using Game.Domain.Audio.Enums;

namespace Game.Infrastructure.Audio.Services;

public class SoundManagerService
{
    public static SoundManagerService Instance { get; private set; }

    static SoundManagerService()
    {
        Instance = new SoundManagerService();
    }

    public void PlaySound(string soundKey, SoundCategory category = SoundCategory.SFX, float volumeDb = 0.0f)
    {
        GD.Print($"Playing sound: {soundKey} (Category: {category}, Volume: {volumeDb})");
    }

    public void PlaySoundAtPosition(string soundKey, Vector2 position, Vector2 listenerPosition, float maxDistance = 500.0f)
    {
        GD.Print($"Playing sound at position: {soundKey} at {position}");
    }

    public void SetMasterVolume(float volume)
    {
        GD.Print($"Setting master volume to: {volume}");
    }

    public void SetCategoryVolume(SoundCategory category, float volume)
    {
        GD.Print($"Setting {category} volume to: {volume}");
    }

    public void StopAllSounds()
    {
        GD.Print("Stopping all sounds");
    }

    public void StopMusic()
    {
        GD.Print("Stopping music");
    }
}
