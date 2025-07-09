using Godot;
using Game.Domain.Audio.Services;
using Game.Domain.Shared.ValueObjects;
using Game.Infrastructure.Audio.Services;
using Game.Domain.Audio.Enums;
using Game.Domain.Audio.ValueObjects;

namespace Game.Infrastructure.Sound;

public class SoundServiceAdapter : ISoundService
{
    private readonly SoundManagerService _soundManagerService;

    public SoundServiceAdapter(SoundManagerService soundManagerService)
    {
        _soundManagerService = soundManagerService ?? throw new System.ArgumentNullException(nameof(soundManagerService));
    }

    public void PlaySound(string soundKey, SoundCategory category = SoundCategory.SFX, float volumeDb = 0.0f)
    {
        GD.Print($"🔊 SoundServiceAdapter.PlaySound called: {soundKey}, category: {category}");
        _soundManagerService.PlaySound(soundKey, category, volumeDb);
    }

    public void PlaySoundAtPosition(string soundKey, Position position, Position listenerPosition, float maxDistance = 500.0f)
    {
        var godotPosition = new Vector2(position.X, position.Y);
        var godotListener = new Vector2(listenerPosition.X, listenerPosition.Y);
        _soundManagerService.PlaySoundAtPosition(soundKey, godotPosition, godotListener, maxDistance);
    }

    public void PlaySound(SoundRequest request)
    {
        if (request.IsPositional)
        {
            PlaySoundAtPosition(request.SoundKey, request.Position!.Value, request.ListenerPosition!.Value, request.MaxDistance);
        }
        else
        {
            PlaySound(request.SoundKey, request.Category, request.VolumeDb);
        }
    }

    public void SetMasterVolume(float volume)
    {
        _soundManagerService.SetMasterVolume(volume);
    }

    public void SetCategoryVolume(SoundCategory category, float volume)
    {
        _soundManagerService.SetCategoryVolume(category, volume);
    }

    public void StopAllSounds()
    {
        _soundManagerService.StopAllSounds();
    }

    public void StopMusic()
    {
        _soundManagerService.StopMusic();
    }
}
