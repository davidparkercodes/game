using Godot;
using Game.Domain.Audio.Enums;

namespace Game.Infrastructure.Interfaces;

public interface ISoundService
{
    void PlaySound(string soundKey, SoundCategory category = SoundCategory.SFX, float volumeDb = 0.0f);
    void PlaySoundAtPosition(string soundKey, Vector2 position, Vector2 listenerPosition, float maxDistance = 500.0f);
    void SetMasterVolume(float volume);
    void SetCategoryVolume(SoundCategory category, float volume);
    void StopAllSounds();
    void StopMusic();
}
