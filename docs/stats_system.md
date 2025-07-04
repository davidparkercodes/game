# Stats Configuration System

This document describes the JSON-based stats configuration system for enemies and buildings.

## Overview

The stats system allows game balance and configuration to be managed through JSON files instead of hardcoded values in C# scripts. This makes it easier to:
- Adjust game balance without recompiling
- Add new enemy/building types by editing JSON
- Manage multiple variants of entities
- Share configurations between team members

## File Structure

```
data/
└── stats/
    ├── enemy_stats.json
    └── building_stats.json
```

## Enemy Stats Configuration

### File: `data/stats/enemy_stats.json`

Contains configuration for all enemy types in the game.

#### Structure
```json
{
  "enemy_types": {
    "enemy_type_name": {
      "max_health": 100,
      "speed": 60.0,
      "damage": 10,
      "reward_gold": 5,
      "reward_xp": 10,
      "description": "Description of enemy"
    }
  },
  "default_stats": {
    // Fallback values if enemy type not found
  }
}
```

#### Available Enemy Types
- `basic_enemy`: Standard balanced enemy
- `fast_enemy`: Quick but fragile
- `tank_enemy`: Slow but high health
- `elite_enemy`: Balanced with higher stats

### Usage in Code

```csharp
// In Enemy.cs - set the enemy type
[Export] public string EnemyType = "basic_enemy";

// Stats are automatically loaded from JSON in _Ready()
// Access stats through properties:
int health = MaxHealth;
float moveSpeed = Speed;
int attackDamage = Damage;
```

## Building Stats Configuration

### File: `data/stats/building_stats.json`

Contains configuration for all building/turret types.

#### Structure
```json
{
  "building_types": {
    "building_type_name": {
      "cost": 15,
      "damage": 10,
      "range": 120.0,
      "fire_rate": 1.2,
      "bullet_speed": 900,
      "shoot_sound": "turret_shoot_sound",
      "impact_sound": "bullet_impact_sound",
      "description": "Description of building"
    }
  },
  "default_stats": {
    // Fallback values if building type not found
  }
}
```

#### Available Building Types
- `basic_turret`: Balanced cost and performance
- `sniper_turret`: High damage, long range, slow fire rate
- `rapid_turret`: Fast firing, lower damage
- `heavy_turret`: High damage, expensive, very slow

### Usage in Code

```csharp
// In turret classes - set the building type
public override void _Ready()
{
    BuildingType = "basic_turret";  // or sniper_turret, etc.
    base._Ready();
}

// Stats are automatically loaded from JSON
// Access through properties:
int buildCost = Cost;
int attackDamage = Damage;
float attackRange = Range;
```

## StatsManager Class

The `StatsManager` singleton handles loading and providing access to stats.

### Key Methods

```csharp
// Get enemy stats
EnemyStatsData enemyStats = StatsManager.Instance.GetEnemyStats("basic_enemy");

// Get building stats  
BuildingStatsData buildingStats = StatsManager.Instance.GetBuildingStats("sniper_turret");

// Check if types exist
bool hasEnemy = StatsManager.Instance.HasEnemyType("elite_enemy");
bool hasBuilding = StatsManager.Instance.HasBuildingType("rapid_turret");

// Reload configurations (useful for development)
StatsManager.Instance.ReloadConfigurations();
```

## Adding New Types

### New Enemy Type

1. Add entry to `enemy_stats.json`:
```json
"new_enemy_type": {
  "max_health": 150,
  "speed": 45.0,
  "damage": 25,
  "reward_gold": 10,
  "reward_xp": 20,
  "description": "New enemy variant"
}
```

2. Set the enemy type in scene or code:
```csharp
EnemyType = "new_enemy_type";
```

### New Building Type

1. Add entry to `building_stats.json`:
```json
"new_turret_type": {
  "cost": 40,
  "damage": 20,
  "range": 160.0,
  "fire_rate": 1.8,
  "bullet_speed": 1000,
  "shoot_sound": "new_turret_shoot",
  "impact_sound": "new_bullet_impact",
  "description": "New turret variant"
}
```

2. Create new turret class or set type:
```csharp
public partial class NewTurret : Building
{
    public override void _Ready()
    {
        BuildingType = "new_turret_type";
        base._Ready();
    }
}
```

## Best Practices

1. **Consistent Naming**: Use snake_case for type names
2. **Balanced Values**: Test thoroughly when changing stats
3. **Descriptions**: Always include meaningful descriptions
4. **Default Stats**: Keep default_stats updated as a safety net
5. **Version Control**: Track JSON changes carefully for balance history

## Debugging

Enable debug output to see stats loading:
```csharp
GD.Print($"Enemy {Name} ({EnemyType}) ready: HP={MaxHealth}, Speed={Speed}, Damage={Damage}");
```

The StatsManager will log warnings if:
- JSON files are missing
- Enemy/building types are not found
- JSON parsing fails

## Wave System Integration

The stats system is fully integrated with the wave spawning system:

### How It Works
1. **Wave Config**: JSON wave files specify `enemyType` (e.g., "fast_enemy", "tank_enemy")
2. **Stats Loading**: WaveSpawner sets the enemy type, Enemy loads base stats from JSON
3. **Wave Modifiers**: Additional multipliers from wave config apply on top of base stats
4. **Final Stats**: Enemy spawns with: `base_stat * wave_multiplier`

### Example Wave Configuration
```json
{
  "enemyType": "tank_enemy",
  "count": 4,
  "healthMultiplier": 1.3,
  "speedMultiplier": 1.1,
  "moneyReward": 8
}
```

This spawns tank enemies with:
- Base HP: 200 (from JSON) × 1.3 = 260 HP
- Base Speed: 30 (from JSON) × 1.1 = 33 speed
- Total Reward: 12 (base) + 8 (wave bonus) = 20 gold

### Available Enemy Types in Waves
- `basic_enemy`: Balanced stats, good for early waves
- `fast_enemy`: Quick but fragile, creates pressure
- `tank_enemy`: Slow but very high HP, requires sustained fire
- `elite_enemy`: High stats across the board, boss-like enemies

## Integration with Other Systems

The stats system integrates with:
- **Sound System**: Building stats include sound keys
- **Wave System**: Enemy types referenced in wave configs with base stats from JSON
- **UI System**: Building costs and stats display in HUD
- **Save System**: Player progress can reference these stat values
- **Economy**: Enemy rewards calculated from base + wave bonus
