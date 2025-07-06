using Godot;
using Game.Domain.Audio.Enums;
using Game.Domain.Shared.ValueObjects;
using Game.Infrastructure.Sound;

namespace Game.Infrastructure.Audio.Services;

public class SoundManagerService
{
    public static SoundManagerService Instance { get; private set; }
    private readonly SoundService _soundService;

    static SoundManagerService()
    {
        Instance = new SoundManagerService();
    }
    
    private SoundManagerService()
    {
        _soundService = new SoundService();
        GD.Print("🔊 SoundManagerService initialized with SoundService");
    }

    public void PlaySound(string soundKey, SoundCategory category = SoundCategory.SFX, float volumeDb = 0.0f)
    {
        _soundService.PlaySound(soundKey, category, volumeDb);
    }

    public void PlaySoundAtPosition(string soundKey, Vector2 position, Vector2 listenerPosition, float maxDistance = 500.0f)
    {
        GD.Print($"🔊 Playing sound at position: {soundKey} at {position}");
        var domainPosition = new Position(position.X, position.Y);
        var domainListener = new Position(listenerPosition.X, listenerPosition.Y);
        _soundService.PlaySoundAtPosition(soundKey, domainPosition, domainListener, maxDistance);
    }

    public void SetMasterVolume(float volume)
    {
        GD.Print($"🔊 Setting master volume to: {volume}");
        _soundService.SetMasterVolume(volume);
    }

    public void SetCategoryVolume(SoundCategory category, float volume)
    {
        GD.Print($"🔊 Setting {category} volume to: {volume}");
        _soundService.SetCategoryVolume(category, volume);
    }

    public void StopAllSounds()
    {
        GD.Print("🔊 Stopping all sounds");
        _soundService.StopAllSounds();
    }

    public void StopMusic()
    {
        GD.Print("🔊 Stopping music");
        _soundService.StopMusic();
    }
}
