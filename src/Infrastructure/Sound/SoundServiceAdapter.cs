using Godot;
using Game.Domain.Audio.Services;
using Game.Domain.Shared.ValueObjects;
using Game.Infrastructure.Managers;
using Game.Domain.Audio.Enums;
using Game.Domain.Audio.ValueObjects;

namespace Game.Infrastructure.Sound;

public class SoundServiceAdapter : ISoundService
{
    private readonly SoundManager _soundManager;

    public SoundServiceAdapter(SoundManager soundManager)
    {
        _soundManager = soundManager ?? throw new System.ArgumentNullException(nameof(soundManager));
    }

    public void PlaySound(string soundKey, SoundCategory category = SoundCategory.SFX, float volumeDb = 0.0f)
    {
        _soundManager.PlaySound(soundKey, category, volumeDb);
    }

    public void PlaySoundAtPosition(string soundKey, Position position, Position listenerPosition, float maxDistance = 500.0f)
    {
        var godotPosition = new Vector2(position.X, position.Y);
        var godotListener = new Vector2(listenerPosition.X, listenerPosition.Y);
        _soundManager.PlaySoundAtPosition(soundKey, godotPosition, godotListener, maxDistance);
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
        _soundManager.SetMasterVolume(volume);
    }

    public void SetCategoryVolume(SoundCategory category, float volume)
    {
        _soundManager.SetCategoryVolume(category, volume);
    }

    public void StopAllSounds()
    {
        _soundManager.StopAllSounds();
    }

    public void StopMusic()
    {
        _soundManager.StopMusic();
    }
}
