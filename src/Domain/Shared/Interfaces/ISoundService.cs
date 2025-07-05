namespace Game.Domain.Shared.Interfaces;

public interface ISoundService
{
    void PlaySound(string soundName);
    void PlaySoundAtPosition(string soundName, float x, float y);
    void StopSound(string soundName);
    void SetMasterVolume(float volume);
    void SetSoundVolume(string soundName, float volume);
    bool IsSoundPlaying(string soundName);
}
