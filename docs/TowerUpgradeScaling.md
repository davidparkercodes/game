# Tower Upgrade Scaling Feature

## Overview
This feature adds visual scaling to towers when they are upgraded, making them grow 10% bigger with each upgrade level. The scaling percentage is configurable through a JSON configuration file.

## Configuration

### Configuration File
The tower upgrade visuals are configured in `data/ui/tower_upgrade_visuals_config.json`:

```json
{
  "upgrade_visuals": {
    "size_scaling": {
      "scale_increase_per_level": 0.10,
      "description": "Tower size increases by 10% per upgrade level",
      "min_scale": 0.8,
      "max_scale": 2.0,
      "enable_scaling": true
    },
    "color_tinting": {
      "enable_color_tinting": true,
      "upgrade_colors": {
        "level_1": { "r": 1.1, "g": 1.0, "b": 1.0, "a": 1.0 },
        "level_2": { "r": 1.0, "g": 1.1, "b": 1.0, "a": 1.0 },
        "level_3": { "r": 1.0, "g": 1.0, "b": 1.1, "a": 1.0 },
        "level_4_plus": { "r": 1.2, "g": 1.2, "b": 1.0, "a": 1.0 }
      }
    },
    "animation": {
      "scale_animation_duration": 0.3,
      "scale_animation_enabled": true,
      "scale_animation_ease": "ease_out"
    },
    "collision": {
      "scale_collision_with_size": true,
      "collision_scale_multiplier": 1.0
    }
  }
}
```

### Key Configuration Values

- **`scale_increase_per_level`**: The percentage increase per upgrade level (0.10 = 10%)
- **`min_scale`**: Minimum allowed scale factor (0.8 = 80%)
- **`max_scale`**: Maximum allowed scale factor (2.0 = 200%)
- **`enable_scaling`**: Whether to enable size scaling (true/false)

## Implementation

### Core Components

1. **TowerUpgradeVisualsConfig** (`src/Infrastructure/Configuration/Services/TowerUpgradeVisualsConfig.cs`)
   - Singleton service that loads and manages the configuration
   - Provides `GetScaleForLevel(int level)` method
   - Provides `GetColorForLevel(int level)` method
   - Handles JSON deserialization with proper property mapping

2. **Building.cs Updates**
   - Added `ApplyUpgradeScaling()` method
   - Updated `UpdateUpgradeVisuals()` to call scaling method
   - Updated `ApplyUpgradeColorTint()` to use configuration

3. **TowerUpgradeService.cs**
   - Calls `UpdateUpgradeVisuals()` when towers are upgraded
   - Integrates with existing upgrade system

### Scale Calculation
The scale factor is calculated as:
```
scale = 1.0 + (scale_increase_per_level * upgrade_level)
```

For example, with default 10% increase per level:
- Level 0: 1.0x (100%)
- Level 1: 1.1x (110%)  
- Level 2: 1.2x (120%)
- Level 3: 1.3x (130%)

## Usage

### Changing Scale Percentage
To change the scale percentage, modify the `scale_increase_per_level` value in the configuration file:

```json
{
  "upgrade_visuals": {
    "size_scaling": {
      "scale_increase_per_level": 0.15,  // 15% increase per level
      ...
    }
  }
}
```

### Disabling Scaling
To disable scaling entirely:

```json
{
  "upgrade_visuals": {
    "size_scaling": {
      "enable_scaling": false,
      ...
    }
  }
}
```

### Setting Scale Limits
Adjust the minimum and maximum scale limits:

```json
{
  "upgrade_visuals": {
    "size_scaling": {
      "min_scale": 0.5,   // Minimum 50% size
      "max_scale": 3.0,   // Maximum 300% size
      ...
    }
  }
}
```

## Visual Effects

### Size Scaling
- Towers grow larger with each upgrade level
- Scaling is applied uniformly (both X and Y axes)
- Scaling affects the entire tower sprite

### Color Tinting
- Each upgrade level has a distinct color tint
- Level 1: Slight red tint
- Level 2: Slight green tint  
- Level 3: Slight blue tint
- Level 4+: Golden tint

### Animation (Future Enhancement)
The configuration includes animation settings for future implementation:
- Scale animation duration
- Animation easing
- Enable/disable animation

## Testing

The feature includes unit tests in `tests/Infrastructure/Configuration/Services/TowerUpgradeVisualsConfigTests.cs` that verify:
- Scale calculation for different levels
- Color tinting for different levels
- Default configuration values
- Edge cases (level 0, high levels)

## Integration with Existing Systems

The feature integrates seamlessly with:
- **Tower Upgrade System**: Automatically applies visual changes when towers are upgraded
- **Building Selection**: Scaling works with existing selection visuals
- **Building Registry**: Collision detection still works with scaled towers
- **Range Visualization**: Range circles are not affected by scaling

## Performance Considerations

- Configuration is loaded once on startup and cached
- Scaling calculations are minimal (simple multiplication)
- No additional memory overhead per tower
- Visual scaling uses Godot's built-in Scale property

## Future Enhancements

Potential future improvements:
- Animated scaling transitions when upgrading
- Per-tower-type scaling configurations
- Scaling collision radius with visual size
- Particle effects on upgrade
- Sound effects for scaling

## Files Modified

- `data/ui/tower_upgrade_visuals_config.json` (new)
- `src/Infrastructure/Configuration/Services/TowerUpgradeVisualsConfig.cs` (new)
- `src/Presentation/Buildings/Building.cs` (modified)
- `tests/Infrastructure/Configuration/Services/TowerUpgradeVisualsConfigTests.cs` (new)
- `docs/TowerUpgradeScaling.md` (new)
