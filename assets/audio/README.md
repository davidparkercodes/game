# Audio System Documentation

## Folder Structure

```
assets/audio/
├── towers/           # Tower-related sounds
│   ├── basic_tower_shoot.mp3
│   └── sniper_tower_shoot.mp3
├── projectiles/      # Bullet and projectile sounds
│   ├── basic_bullet_impact.mp3
│   └── sniper_bullet_impact.mp3
├── game/             # General game sounds
│   └── round_start.mp3
├── ui/               # User interface sounds
├── enemies/          # Enemy-related sounds
└── music/            # Background music
```

## Adding New Sounds

### 1. Place the audio file in the appropriate folder
- **Towers**: `assets/audio/towers/`
- **Projectiles**: `assets/audio/projectiles/`
- **UI**: `assets/audio/ui/`
- **Enemies**: `assets/audio/enemies/`
- **Game Events**: `assets/audio/game/`
- **Music**: `assets/audio/music/`

### 2. Add the sound to the JSON configuration
Edit `data/audio/sound_config.json` and add your sound:

```json
{
  "sounds": {
    "category_name": {
      "your_sound_key": {
        "file": "res://assets/audio/category/your_sound.mp3",
        "category": "SFX",
        "volume": 0.0,
        "description": "Description of your sound"
      }
    }
  }
}
```

### 3. Play the sound in your code
```csharp
if (SoundManager.Instance != null)
{
    SoundManager.Instance.PlaySound("your_sound_key");
}
```

### 4. Reload the game
The sounds are loaded automatically from the JSON configuration when the game starts.

### Alternative: Using the SoundConfigUtility (for developers)
You can also add sounds programmatically:

```csharp
// Add a new sound
SoundConfigUtility.AddSound(
    "enemies", 
    "enemy_death", 
    "res://assets/audio/enemies/enemy_death.mp3", 
    "SFX", 
    0.0f, 
    "Enemy death sound"
);

// List all sounds
SoundConfigUtility.ListAllSounds();

// Validate all sound files exist
SoundConfigUtility.ValidateAllSounds();
```

## Sound Categories

- **SFX**: General sound effects (default)
- **UI**: User interface sounds
- **Music**: Background music

## JSON Configuration System

The audio system uses a JSON configuration file (`data/audio/sound_config.json`) for easy sound management:

### Benefits:
- **No code changes needed** when adding new sounds
- **Centralized configuration** for all audio assets
- **Per-sound volume control** and categorization
- **Automatic loading** and error handling
- **Easy to maintain** and organize

### Configuration Structure:
```json
{
  "sounds": {
    "category": {
      "sound_key": {
        "file": "path/to/sound.mp3",
        "category": "SFX|UI|Music",
        "volume": 0.0,
        "description": "Sound description"
      }
    }
  },
  "categories": {
    "SFX": {
      "defaultVolume": 1.0,
      "description": "Sound effects"
    }
  }
}
```

## Features

- **JSON-based configuration**: Easy sound management without code changes
- **Multiple simultaneous sounds**: Up to 10 SFX can play at once
- **Volume control**: Master, SFX, UI, and Music volume controls
- **Positional audio**: `PlaySoundAtPosition()` for distance-based volume
- **Easy integration**: Simple `PlaySound("key")` calls
- **Error handling**: Graceful fallbacks and detailed logging

## Current Sounds

| Sound Key | File | Usage |
|-----------|------|-------|
| `basic_tower_shoot` | `towers/basic_tower_shoot.mp3` | Basic tower firing |
| `sniper_tower_shoot` | `towers/sniper_tower_shoot.mp3` | Sniper tower firing |
| `basic_bullet_impact` | `projectiles/basic_bullet_impact.mp3` | Basic bullet hitting enemies |
| `sniper_bullet_impact` | `projectiles/sniper_bullet_impact.mp3` | Sniper bullet hitting enemies |
| `round_start` | `game/round_start.mp3` | When defend phase begins |
| `error` | `game/error.mp3` | Error sound when user cannot afford to upgrade building or place a building in a spot |
