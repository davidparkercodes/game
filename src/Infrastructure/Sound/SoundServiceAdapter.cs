using Godot;
using Game.Infrastructure.Interfaces;
using Game.Domain.Audio.Enums;
using Game.Infrastructure.Managers;

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

    public void PlaySoundAtPosition(string soundKey, Vector2 position, Vector2 listenerPosition, float maxDistance = 500.0f)
    {
        _soundManager.PlaySoundAtPosition(soundKey, position, listenerPosition, maxDistance);
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
