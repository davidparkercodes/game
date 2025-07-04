using Godot;
using System.Collections.Generic;
using System.Linq;

public enum SoundCategory
{
	SFX,
	UI,
	Music
}

public partial class SoundManager : Node
{
	public static SoundManager Instance { get; private set; }
	
	private Dictionary<string, AudioStream> _sounds = new Dictionary<string, AudioStream>();
	private List<AudioStreamPlayer> _sfxPlayers = new List<AudioStreamPlayer>();
	private AudioStreamPlayer _uiPlayer;
	private AudioStreamPlayer _musicPlayer;
	
	[Export] public int MaxSimultaneousSFX = 10;
	[Export] public float MasterVolume = 1.0f;
	[Export] public float SFXVolume = 1.0f;
	[Export] public float UIVolume = 1.0f;
	[Export] public float MusicVolume = 1.0f;
	
	public override void _Ready()
	{
		Instance = this;
		
		for (int i = 0; i < MaxSimultaneousSFX; i++)
		{
			var player = new AudioStreamPlayer();
			AddChild(player);
			_sfxPlayers.Add(player);
		}
		
		_uiPlayer = new AudioStreamPlayer();
		AddChild(_uiPlayer);
		
		_musicPlayer = new AudioStreamPlayer();
		_musicPlayer.Autoplay = false;
		AddChild(_musicPlayer);
		
		LoadSounds();
		GD.Print("🔊 SoundManager ready with {} SFX channels", MaxSimultaneousSFX);
	}
	
	private void LoadSounds()
	{
		_sounds = SoundLoader.LoadSounds();
	}
	
	public void PlaySound(string soundKey, SoundCategory category = SoundCategory.SFX, float volumeDb = 0.0f)
	{
		if (!_sounds.ContainsKey(soundKey))
		{
			GD.PrintErr($"❌ Sound key '{soundKey}' does not exist");
			return;
		}
		
		AudioStreamPlayer player = GetPlayerForCategory(category);
		if (player == null)
		{
			GD.PrintErr($"❌ No available player for category {category}");
			return;
		}
		
		player.Stream = _sounds[soundKey];
		player.VolumeDb = volumeDb + GetCategoryVolumeDb(category);
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
	
	private AudioStreamPlayer GetPlayerForCategory(SoundCategory category)
	{
		switch (category)
		{
			case SoundCategory.SFX:
				return _sfxPlayers.FirstOrDefault(p => !p.Playing) ?? _sfxPlayers[0];
			case SoundCategory.UI:
				return _uiPlayer;
			case SoundCategory.Music:
				return _musicPlayer;
			default:
				return _sfxPlayers[0];
		}
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
		_uiPlayer.Stop();
	}
	
	public void StopMusic()
	{
		_musicPlayer.Stop();
	}
}
