using System.Collections.Generic;
using System.Linq;
using Godot;
using Game.Domain.Audio.Services;
using Game.Domain.Audio.Enums;
using Game.Domain.Audio.ValueObjects;
using Game.Domain.Shared.ValueObjects;

namespace Game.Infrastructure.Sound;

public class SoundService : ISoundService
{
    private Dictionary<string, AudioStream> _sounds = new();
    private Dictionary<string, SoundConfigData> _soundConfigs = new();
    private List<AudioStreamPlayer> _sfxPlayers = new();
    private AudioStreamPlayer _uiPlayer = null!;
    private AudioStreamPlayer _musicPlayer = null!;
    
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
            GD.PrintErr($"❌ Sound not found: {soundKey}");
            return;
        }
        
        AudioStreamPlayer player = GetPlayerForCategory(category);
        if (player == null)
        {
            GD.PrintErr($"❌ No audio player for category: {category}");
            return;
        }
        
        float individualSoundVolume = 0.0f;
        if (_soundConfigs.ContainsKey(soundKey))
        {
            individualSoundVolume = _soundConfigs[soundKey].Volume;
        }
        
        float finalVolume = volumeDb + individualSoundVolume + GetCategoryVolumeDb(category);
        
        player.Stream = _sounds[soundKey];
        player.VolumeDb = finalVolume;
        
        // Set looping for music category
        if (category == SoundCategory.Music)
        {
            if (player.Stream is AudioStreamOggVorbis oggStream)
            {
                oggStream.Loop = true;
                GD.Print($"🎵 Set OGG loop for music: {soundKey}");
            }
            else if (player.Stream is AudioStreamWav wavStream)
            {
                wavStream.LoopMode = AudioStreamWav.LoopModeEnum.Forward;
                GD.Print($"🎵 Set WAV loop for music: {soundKey}");
            }
            else if (player.Stream is AudioStreamMP3 mp3Stream)
            {
                mp3Stream.Loop = true;
                GD.Print($"🎵 Set MP3 loop for music: {soundKey}");
            }
            else
            {
                GD.PrintErr($"⚠️ Unknown audio format for music looping: {player.Stream.GetType().Name}");
            }
        }
        
        player.Play();
    }

    public void PlaySoundAtPosition(string soundKey, Position position, Position listenerPosition, float maxDistance = 500.0f)
    {
        float distance = position.DistanceTo(listenerPosition);
        if (distance > maxDistance) return;
        
        float volumeMultiplier = 1.0f - (distance / maxDistance);
        float volumeDb = Mathf.LinearToDb(volumeMultiplier);
        
        PlaySound(soundKey, SoundCategory.SFX, volumeDb);
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
        // Get the current scene to add audio players to
        var currentScene = Engine.GetMainLoop();
        if (currentScene is SceneTree sceneTree && sceneTree.CurrentScene != null)
        {
            for (int i = 0; i < MaxSimultaneousSFX; i++)
            {
                var player = new AudioStreamPlayer();
                player.Name = $"SFXPlayer{i}";
                _sfxPlayers.Add(player);
                sceneTree.CurrentScene.AddChild(player);
            }
            
            _uiPlayer = new AudioStreamPlayer();
            _uiPlayer.Name = "UIPlayer";
            sceneTree.CurrentScene.AddChild(_uiPlayer);
            
            _musicPlayer = new AudioStreamPlayer();
            _musicPlayer.Name = "MusicPlayer";
            _musicPlayer.Autoplay = false;
            sceneTree.CurrentScene.AddChild(_musicPlayer);
            
            GD.Print($"🎵 Added {MaxSimultaneousSFX + 2} audio players to scene tree");
        }
        else
        {
            GD.PrintErr("❌ Cannot add audio players - no current scene available");
        }
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
