using System.Collections.Generic;
using System.Linq;
using Godot;
using Game.Infrastructure.Interfaces;

namespace Game.Infrastructure.Sound;

public class SoundService : ISoundService
{
    private Dictionary<string, AudioStream> _sounds = new();
    private Dictionary<string, SoundConfigData> _soundConfigs = new();
    private List<AudioStreamPlayer> _sfxPlayers = new();
    private AudioStreamPlayer _uiPlayer;
    private AudioStreamPlayer _musicPlayer;
    
    private int MaxSimultaneousSFX = 10;
    private float MasterVolume = 0.6f;
    private float SFXVolume = 0.7f;
    private float UIVolume = 0.5f;
    private float MusicVolume = 0.4f;

    public SoundService()
    {
        try
        {
            if (IsInGodotRuntime())
            {
                InitializeAudioPlayers();
            }
            LoadSounds();
        }
        catch
        {
            // Ignore initialization errors in test environment
        }
    }

    private static bool IsInGodotRuntime()
    {
        try
        {
            return Godot.Engine.IsEditorHint() || !Godot.Engine.IsEditorHint();
        }
        catch
        {
            return false;
        }
    }

    public void PlaySound(string soundKey, SoundCategory category = SoundCategory.SFX, float volumeDb = 0.0f)
    {
        if (!_sounds.ContainsKey(soundKey))
        {
            return;
        }
        
        AudioStreamPlayer player = GetPlayerForCategory(category);
        if (player == null)
        {
            return;
        }
        
        float individualSoundVolume = 0.0f;
        if (_soundConfigs.ContainsKey(soundKey))
        {
            individualSoundVolume = _soundConfigs[soundKey].Volume;
        }
        
        player.Stream = _sounds[soundKey];
        player.VolumeDb = volumeDb + individualSoundVolume + GetCategoryVolumeDb(category);
        player.Play();
    }

    public void PlaySoundAtPosition(string soundKey, Vector2 position, Vector2 listenerPosition, float maxDistance = 500.0f)
    {
        float distance = position.DistanceTo(listenerPosition);
        if (distance > maxDistance) return;
        
        float volumeMultiplier = 1.0f - (distance / maxDistance);
        float volumeDb = Mathf.LinearToDb(volumeMultiplier);
        
        PlaySound(soundKey, SoundCategory.SFX, volumeDb);
    }

    public void SetMasterVolume(float volume)
    {
        MasterVolume = Mathf.Clamp(volume, 0.0f, 1.0f);
    }

    public void SetCategoryVolume(SoundCategory category, float volume)
    {
        volume = Mathf.Clamp(volume, 0.0f, 1.0f);
        
        switch (category)
        {
            case SoundCategory.SFX:
                SFXVolume = volume;
                break;
            case SoundCategory.UI:
                UIVolume = volume;
                break;
            case SoundCategory.Music:
                MusicVolume = volume;
                break;
        }
    }

    public void StopAllSounds()
    {
        foreach (var player in _sfxPlayers)
        {
            player.Stop();
        }
        _uiPlayer?.Stop();
    }

    public void StopMusic()
    {
        _musicPlayer?.Stop();
    }

    private void InitializeAudioPlayers()
    {
        for (int i = 0; i < MaxSimultaneousSFX; i++)
        {
            var player = new AudioStreamPlayer();
            _sfxPlayers.Add(player);
        }
        
        _uiPlayer = new AudioStreamPlayer();
        _musicPlayer = new AudioStreamPlayer();
        _musicPlayer.Autoplay = false;
    }

    private void LoadSounds()
    {
        try
        {
            if (IsInGodotRuntime())
            {
                _sounds = SoundLoader.LoadSounds();
                
                var config = SoundLoader.LoadConfiguration();
                if (config != null)
                {
                    _soundConfigs = config.GetAllSounds();
                }
            }
        }
        catch
        {
            // Ignore in test environment
        }
    }

    private AudioStreamPlayer GetPlayerForCategory(SoundCategory category)
    {
        return category switch
        {
            SoundCategory.SFX => _sfxPlayers.FirstOrDefault(p => !p.Playing) ?? _sfxPlayers[0],
            SoundCategory.UI => _uiPlayer,
            SoundCategory.Music => _musicPlayer,
            _ => _sfxPlayers[0]
        };
    }

    private float GetCategoryVolumeDb(SoundCategory category)
    {
        float categoryVolume = category switch
        {
            SoundCategory.SFX => SFXVolume,
            SoundCategory.UI => UIVolume,
            SoundCategory.Music => MusicVolume,
            _ => 1.0f
        };
        
        return Mathf.LinearToDb(MasterVolume * categoryVolume);
    }
}
