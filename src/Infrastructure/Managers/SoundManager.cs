using Godot;
using Game.Domain.Audio.Enums;

namespace Game.Infrastructure.Managers;

public class SoundManager
{
    public static SoundManager Instance { get; private set; }

    static SoundManager()
    {
        Instance = new SoundManager();
    }

    public void PlaySound(string soundKey, SoundCategory category = SoundCategory.SFX, float volumeDb = 0.0f)
    {
        // TODO: Implement sound playing logic
        GD.Print($"Playing sound: {soundKey} (Category: {category}, Volume: {volumeDb})");
    }

    public void PlaySoundAtPosition(string soundKey, Vector2 position, Vector2 listenerPosition, float maxDistance = 500.0f)
    {
        // TODO: Implement positional sound playing logic
        GD.Print($"Playing sound at position: {soundKey} at {position}");
    }

    public void SetMasterVolume(float volume)
    {
        // TODO: Implement master volume setting
        GD.Print($"Setting master volume to: {volume}");
    }

    public void SetCategoryVolume(SoundCategory category, float volume)
    {
        // TODO: Implement category volume setting
        GD.Print($"Setting {category} volume to: {volume}");
    }

    public void StopAllSounds()
    {
        // TODO: Implement stop all sounds
        GD.Print("Stopping all sounds");
    }

    public void StopMusic()
    {
        // TODO: Implement stop music
        GD.Print("Stopping music");
    }
}
