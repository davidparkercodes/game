using Game.Domain.Audio.Enums;
using Game.Domain.Audio.ValueObjects;
using Game.Domain.Shared.ValueObjects;

namespace Game.Domain.Audio.Services;

public interface ISoundService
{
    void PlaySound(string soundKey, SoundCategory category = SoundCategory.SFX, float volumeDb = 0.0f);
    void PlaySoundAtPosition(string soundKey, Position position, Position listenerPosition, float maxDistance = 500.0f);
    void PlaySound(SoundRequest request);
    void SetMasterVolume(float volume);
    void SetCategoryVolume(SoundCategory category, float volume);
    void StopAllSounds();
    void StopMusic();
}
