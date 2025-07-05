# AI Context - Game Codebase

## 🚨 Critical Rules

### **NO HARDCODED VALUES**
- **NEVER** hardcode stats, prices, damage, ranges, etc. in C# code
- **ALL** game data comes from config files in `data/` directory
- Use `StatsManagerService.Instance.GetBuildingStats()` and similar services
- Config files: `data/stats/building_stats.json`, `data/stats/enemy_stats.json`, `data/audio/sound_config.json`

### **Config-Driven Architecture**
- Tower stats: `LoadStatsFromConfig("tower_type")` in Building classes
- Enemy stats: Load via `StatsManagerService` in Enemy classes  
- Sounds: All sound keys defined in `data/audio/sound_config.json`
- Building types: Registry system in `data/simulation/building-stats.json`

## 🏗️ Architecture

### **Clean Architecture Layers**
- `Domain/` - Core game entities (Enemy, Building, etc.)
- `Application/` - Use cases, handlers, services (feature-centric)
- `Infrastructure/` - External concerns (audio, stats loading)
- `Presentation/` - Godot-specific UI and game objects

### **Dependency Injection**
- Services registered in DI container
- Use `ServiceProvider.Instance.GetService<T>()`
- Never create service instances directly

### **Code Style (C#)**
- PascalCase for everything: `Ui`, `Dto`, `Ai` (not `UI`, `DTO`, `AI`)
- Namespaces end with `;` not `{}`
- No `///` summary comments - code should be expressive
- No abbreviations - use full descriptive names

## 🎮 Game-Specific

### **Tower Defense Mechanics**
- Towers auto-detect enemies via Area2D signals
- Shooting system: Timer-based with config-driven fire rates
- Projectiles: Instantiated from BulletScene, damage from tower stats

### **Sound System**
- JSON-based config: `data/audio/sound_config.json`
- Play via `SoundManagerService.Instance.PlaySound("sound_key")`
- Tower shooting sounds: `"basic_tower_shoot"`, `"sniper_tower_shoot"`

### **Godot Integration**
- Never run CLI commands that get stuck in game (per user rules)
- Scene files reference C# scripts in `src/Presentation/`
- Use proper Godot signal connections: `Connect("signal", new Callable(this, nameof(Method)))`

## 🔧 Development

### **File Structure**
- Config files: `data/` directory (JSON format)
- C# source: `src/` with clean architecture folders
- Godot scenes: `scenes/` directory
- Tests: `tests/` directory

### **Key Services**
- `StatsManagerService` - Loads all game stats from config
- `SoundManagerService` - Handles audio playback
- `BuildingTypeRegistry` / `EnemyTypeRegistry` - Type management

## ⚠️ Common Mistakes to Avoid
- Hardcoding any game values in C# code
- Creating services instead of using DI
- Using hardcoded type strings instead of registry
- Ignoring existing config-driven architecture
- Adding `///` XML comments to code
