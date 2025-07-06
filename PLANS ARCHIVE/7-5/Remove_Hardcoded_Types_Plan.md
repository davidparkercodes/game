# **Plan: Remove All Hardcoded Building/Enemy Type References**

## **Problem Analysis**

The codebase contains numerous hardcoded building and enemy type strings that violate configuration-driven design principles. This makes it impossible to change building/enemy names without code changes and creates tight coupling between game logic and specific type names.

### **Current Hardcoded References Found:**

#### **Building Types:**

- `basic_tower` - Found in 4 files
- `sniper_tower` - Found in GameSimRunner.cs
- `rapid_tower` - Found in GameSimRunner.cs
- `heavy_tower` - Found in config files (referenced)

#### **Enemy Types:**

- `basic_enemy` - Found in GameSimRunner.cs, MockEnemyStatsProvider.cs
- `fast_enemy` - Found in GameSimRunner.cs, MockEnemyStatsProvider.cs
- `tank_enemy` - Found in GameSimRunner.cs, MockEnemyStatsProvider.cs
- `elite_enemy` - Found in GameSimRunner.cs, MockEnemyStatsProvider.cs
- `boss_enemy` - Found in GameSimRunner.cs, MockEnemyStatsProvider.cs

---

## **Solution Approach**

Instead of changing the actual names, we'll create a **Type Registry System** that allows easy management of all building/enemy types with flexible naming and config-driven discovery.

---

## **Implementation Plan**

### **Phase 1: Create Type Registry Foundation** 🏗️ ✅ COMPLETED

#### **[x] 1.1 Create Building Type Registry**

- [x] Create `BuildingType` value object/enum with:
  - Internal ID (for code reference)
  - Display name (configurable)
  - Config key (what's used in JSON)
- [x] Create `BuildingTypeRegistry` service to manage all building types
- [x] Load building types from config file metadata
- [x] Support for aliases/alternate names

#### **[x] 1.2 Create Enemy Type Registry**

- [x] Create `EnemyType` value object/enum with same pattern
- [x] Create `EnemyTypeRegistry` service
- [x] Load enemy types from config file metadata
- [x] Support for enemy tier/category system

#### **[x] 1.3 Config Schema Enhancement**

- [x] Add `building_types_metadata` section to building-stats.json:

```json
{
  "building_types_metadata": {
    "registry": {
      "BASIC_TOWER": {
        "config_key": "basic_tower",
        "display_name": "Basic Tower",
        "category": "starter"
      },
      "SNIPER_TOWER": {
        "config_key": "sniper_tower",
        "display_name": "Sniper Tower",
        "category": "precision"
      }
    },
    "categories": ["starter", "precision", "rapid", "heavy"]
  }
}
```

- [x] Add similar `enemy_types_metadata` to enemy-stats.json

#### **[x] 1.4 Architectural Cleanup (CRITICAL)** ✅ COMPLETED

- [x] Move configuration classes from Infrastructure to Application layer (feature-centric)
- [x] Move `BuildingTypeMetadata.cs` to `src/Application/Buildings/Configuration/`
- [x] Move `EnemyTypeMetadata.cs` to `src/Application/Enemies/Configuration/`
- [x] Delete incorrectly placed Infrastructure configuration files
- [x] Update registry services to use correct import paths
- [x] Verify system still works after restructuring
- [x] Follow Domain/Application layer focus (no Infrastructure yet)

**Architectural Issue Identified:** Configuration models were incorrectly placed in Infrastructure layer instead of Application layer with feature-centric organization. This violates clean architecture principles and feature-centric design.

**Achievement Note:** Type Registry Foundation is fully complete with proper architecture! Created BuildingType and EnemyType value objects, registry services, enhanced config schema with metadata, performed architectural cleanup to move configs to feature-centric Application layer organization, and verified the system works perfectly with the existing GameSimRunner. All types are now managed through config-driven registries with flexible categorization and clean architecture.

#### **[x] 1.5 Terminology Cleanup - Replace "Turret" with "Tower"** ✅ COMPLETED

- [x] Find all occurrences of 'turret'/'Turret'/'TURRET' in codebase
- [x] Replace class names: `GetTurretStatsQuery` → `GetTowerStatsQuery`
- [x] Replace handler names: `GetTurretStatsQueryHandler` → `GetTowerStatsQueryHandler`
- [x] Replace response names: `TurretStatsResponse` → `TowerStatsResponse`
- [x] Replace property names: `TurretType` → `TowerType`
- [x] Replace variable names and comments containing "turret"
- [x] Update any file names containing "turret"
- [x] Verify system still builds and works after terminology cleanup

**Terminology Issue:** The codebase uses inconsistent terminology mixing "turret" and "tower" concepts. The game uses "tower" so all code should use "tower" for consistency.

**Achievement Note:** Terminology cleanup is complete! Successfully renamed all classes, files, properties, and references from "turret" to "tower" across Domain, Application, and Infrastructure layers. The codebase now uses consistent "tower" terminology throughout, including `GetTowerStatsQuery`, `GetTowerStatsQueryHandler`, `TowerStatsResponse`, `BasicTower`, `SniperTower`, and all related properties. System builds and runs perfectly after the terminology standardization.

#### **[x] 1.75 Complete Turret Reference Cleanup** ✅ COMPLETED

- [x] Update `project.godot` - replace turret references
- [x] Update `README.md` - replace turret references
- [x] Update `sound_config.json` - replace turret sound names
- [x] Update `data/simulation/building-stats.json` - replace turret references in descriptions (already correct)
- [x] Update `stats_system.md` - replace turret references
- [x] Update `wave_progression.md` - replace turret references
- [x] Rename `BasicTurret.tscn` → `BasicTower.tscn`
- [x] Rename `SniperTurret.tscn` → `SniperTower.tscn`
- [x] Rename `scenes/Turrets/` → `scenes/Towers/`
- [x] Update scene files to reference correct .cs files
- [x] Update `src/Presentation/Player/Player.cs` - replace remaining turret references
- [x] Update `src/Presentation/UI/Hud.cs` - replace remaining turret references
- [x] Verify all turret references eliminated from entire codebase

**Achievement Note:** Complete turret reference cleanup accomplished! Successfully updated all remaining files including project configuration, documentation (README, stats_system.md, wave_progression.md), config files (sound_config.json), Godot scene files, and C# presentation layer files. Renamed scene files and directories, updated all property names, method names, and UI references. The entire codebase now uses consistent "tower" terminology with zero "turret" references remaining.

### **Phase 2: Replace Hardcoded Validation Logic** ✅ COMPLETED

#### **[x] 2.1 Fix GetTowerStatsQueryHandler**

- [x] Remove hardcoded `"basic_tower"` exception
- [x] Use `BuildingTypeRegistry.IsValidConfigKey(query.TowerType)`
- [x] Use config-driven validation instead of cost checks

#### **[x] 2.2 Fix PlaceBuildingCommandHandler**

- [x] Remove hardcoded `"basic_tower"` exception
- [x] Use consistent `BuildingTypeRegistry` validation
- [x] Ensure all building type references go through registry

#### **[x] 2.3 Fix MockBuildingStatsProvider**

- [x] Remove hardcoded `"basic_tower"` fallback logic
- [x] Use `BuildingTypeRegistry.GetDefaultType()` for fallbacks
- [x] Use `BuildingTypeRegistry.GetCheapestType()` as alternative fallback

**Achievement Note:** Phase 2 is fully complete! Successfully removed all hardcoded validation logic and replaced it with config-driven BuildingTypeRegistry validation. Updated GetTowerStatsQueryHandler and PlaceBuildingCommandHandler to use `IsValidConfigKey()` instead of cost-based checks with hardcoded exceptions. Enhanced MockBuildingStatsProvider with intelligent fallback strategy using registry methods while maintaining backward compatibility. Updated dependency injection to provide BuildingTypeRegistry to all handlers. Zero hardcoded type validation remains in the system.

### **Phase 3: Replace Hardcoded Placement Strategy** 🎯 ✅ COMPLETED

#### **[x] 3.1 Make GameSimRunner Placement Config-Driven**

- [x] Replace hardcoded `"basic_tower"` in `PlaceInitialBuildings()`
- [x] Replace hardcoded `"sniper_tower"` in `PlaceAdditionalBuildings()`
- [x] Replace hardcoded `"rapid_tower"` in `PlaceAdditionalBuildings()`
- [x] Use `BuildingTypeRegistry.GetByCategory("starter")` for initial placement
- [x] Use `BuildingTypeRegistry.GetByCategory("precision")` for sniper-like towers
- [x] Use `BuildingTypeRegistry.GetByCategory("rapid")` for rapid-fire towers

#### **[x] 3.2 Create Placement Strategy Configuration**

- [x] Add `placement_strategies.json` config file with comprehensive strategy definitions
- [x] Create `IPlacementStrategyProvider` interface with methods for all placement scenarios
- [x] Implement `PlacementStrategyProvider` with config-driven placement logic
- [x] Add configuration models (`PlacementStrategyConfig`, `StrategiesConfig`, etc.)
- [x] Integrate PlacementStrategyProvider into GameSimRunner
- [x] Replace all hardcoded placement logic with config-driven strategy calls

**Achievement Note:** Phase 3 is fully complete! Successfully eliminated all hardcoded placement strategies from GameSimRunner. Created comprehensive placement strategy system with `placement_strategies.json` config file, `IPlacementStrategyProvider` interface, and `PlacementStrategyProvider` implementation. Updated both `PlaceInitialBuildings()` and `PlaceAdditionalBuildings()` methods to use config-driven category-based building selection with intelligent fallbacks. The placement system now supports configurable positions, cost thresholds, wave-specific upgrades, and multiple fallback strategies. Zero hardcoded building types remain in placement logic.

### **Phase 4: Replace Hardcoded Enemy Logic** 👹 ✅ COMPLETED

#### **[x] 4.1 Fix Enemy Type Selection in GameSimRunner**

- [x] Replace hardcoded enemy types in `GetEnemyTypeForWave()`:
  - [x] Remove `"boss_enemy"` hardcode
  - [x] Remove `"elite_enemy"` hardcode
  - [x] Remove `"tank_enemy"` hardcode
  - [x] Remove `"fast_enemy"` hardcode
  - [x] Remove `"basic_enemy"` hardcode
- [x] Use `EnemyTypeRegistry.GetByCategory()` for category-based enemy selection
- [x] Create wave progression rules based on enemy categories
- [x] Add comprehensive fallback logic using `GetDefaultType()` and `GetBasicType()`

#### **[x] 4.2 Fix MockEnemyStatsProvider**

- [x] Replace hardcoded enemy type references with EnemyTypeRegistry calls
- [x] Add `EnemyTypeRegistry` property to MockEnemyStatsProvider
- [x] Use `EnemyTypeRegistry` for all enemy type lookups and fallbacks
- [x] Remove hardcoded `"basic_enemy"` fallback logic
- [x] Implement intelligent fallback strategy using registry methods

#### **[x] 4.3 Create Enemy Wave Configuration**

- [x] Add `wave_progression.json` config file with comprehensive wave rules:

```json
{
  "wave_rules": {
    "enemy_introduction": {
      "wave_1": ["basic"],
      "wave_2": ["basic", "fast"],
      "wave_4": ["basic", "fast", "tank"],
      "wave_6": ["basic", "fast", "tank", "elite"],
      "wave_8": ["boss"]
    },
    "spawn_patterns": {
      "boss_waves": [8, 16, 24],
      "elite_frequency": 4,
      "tank_frequency": 3,
      "fast_frequency": 2
    },
    "category_progression": {
      "basic": { "min_wave": 1, "spawn_probability": 0.6 },
      "fast": { "min_wave": 2, "spawn_probability": 0.3 },
      "tank": { "min_wave": 4, "spawn_probability": 0.2 },
      "elite": { "min_wave": 6, "spawn_probability": 0.1 },
      "boss": { "min_wave": 8, "spawn_probability": 0.05 }
    }
  },
  "scaling": {
    "enemy_count_per_wave": { "base": 5, "increment_per_wave": 2 },
    "difficulty_scaling": {
      "health_multiplier_per_wave": 0.15,
      "speed_multiplier_per_wave": 0.05,
      "damage_increment_per_wave": 0.33,
      "reward_multiplier_per_wave": 0.1
    }
  }
}
```

**Achievement Note:** Phase 4 is fully complete! Successfully eliminated all hardcoded enemy type strings from GameSimRunner's `GetEnemyTypeForWave()` method. Replaced hardcoded logic with config-driven category-based enemy selection using `EnemyTypeRegistry.GetByCategory()`. Enhanced MockEnemyStatsProvider with EnemyTypeRegistry integration and intelligent fallback strategies. Created comprehensive `wave_progression.json` configuration file with enemy introduction rules, spawn patterns, category progression, and difficulty scaling. The enemy selection system now uses categories ("basic", "fast", "tank", "elite", "boss") instead of hardcoded strings, with robust fallback logic when categories are not found. Zero hardcoded enemy types remain in the enemy spawning logic.

### **Phase 5: Create Type Management System** 🔧 ✅ COMPLETED

#### **[x] 5.1 Build Type Registry Services**

- [x] Implement `BuildingTypeRegistry` with all required methods:
  - [x] `GetByInternalId(string id)`
  - [x] `GetByConfigKey(string key)`
  - [x] `GetByCategory(string category)`
  - [x] `GetDefaultType()` / `GetCheapestType()`
  - [x] `IsValidConfigKey(string key)`
  - [x] `GetAllByTier(int tier)` - Added with cost-based tier logic
  - [x] `GetAllTypes()` and `GetAllCategories()`
- [x] Enhanced `IBuildingTypeRegistry` interface with missing methods

#### **[x] 5.2 Build Enemy Type Registry Services**

- [x] Implement `EnemyTypeRegistry` with comprehensive wave progression methods:
  - [x] `GetEnemiesForWave(int waveNumber)` - Tier-based enemy availability
  - [x] `GetEnemyTypeForWaveProgression(int waveNumber, int enemyIndex)` - Smart enemy selection
  - [x] `IsEnemyAvailableForWave(string configKey, int waveNumber)` - Wave availability checking
- [x] Enhanced `IEnemyTypeRegistry` interface with wave progression methods
- [x] Support for enemy tier/difficulty scaling with progressive unlocking

#### **[x] 5.3 Registry Integration**

- [x] Create `ITypeManagementService` unified interface for both registries
- [x] Implement `TypeManagementService` with comprehensive type management features:
  - [x] Unified building and enemy type access
  - [x] Wave progression support
  - [x] Configuration validation and error reporting
  - [x] Registry status logging and diagnostics
- [x] Create `StartupValidationService` for registry consistency checking:
  - [x] Startup validation with detailed error reporting
  - [x] Placement strategy validation
  - [x] Wave progression validation
  - [x] Registry integrity validation
- [x] Updated GameSimRunner to use EnemyTypeRegistry's built-in wave progression

**Achievement Note:** Phase 5 is fully complete! Created a comprehensive Type Management System that provides unified access to both building and enemy type registries. Enhanced both registries with all required methods, including tier-based access and wave progression logic. Built TypeManagementService as a unified interface with validation, error reporting, and diagnostics capabilities. Created StartupValidationService for thorough configuration validation at application startup. The system now provides centralized type management with robust validation, intelligent fallbacks, and comprehensive error handling. All existing code uses registries instead of hardcoded types, with startup validation ensuring configuration consistency.

### **Phase 6: Configuration Validation & Error Handling** 🛡️ ✅ COMPLETED

#### **[x] 6.1 Startup Validation**
- [x] Validate all building types in config exist in registry
- [x] Validate all enemy types in config exist in registry  
- [x] Validate placement strategies reference valid building categories
- [x] Validate wave progression references valid enemy categories

#### **[x] 6.2 Runtime Error Handling**
- [x] Graceful fallbacks when types are missing
- [x] Clear error messages for invalid type references
- [x] Logging for type registry mismatches

#### **[x] 6.3 Development Tools**
- [x] Add debug command to list all registered building types
- [x] Add debug command to list all registered enemy types
- [x] Add validation tool for config consistency

**Achievement Note:** Phase 6 is fully complete! Successfully implemented comprehensive configuration validation and error handling system. Created StartupValidationService that validates all type registries, placement strategies, and wave progression rules at application startup. Enhanced MockBuildingStatsProvider and MockEnemyStatsProvider with intelligent fallback strategies using registry methods. Built comprehensive DebugCommands system with tools to list all registered types, validate configuration consistency, and inspect wave progression. Integrated startup validation into both Godot game and GameSimRunner tool with clear error reporting and graceful degradation. Added runtime error handling with clear messages for invalid type references and robust logging for type registry mismatches. The system now provides bulletproof configuration validation with developer-friendly debugging tools.

### **Phase 7: Testing & Documentation** 📋 **\_ MAYBE DO NOW \_\_\_**

#### **[ ] 7.1 Unit Tests**

- [ ] Test BuildingTypeRegistry with various config scenarios
- [ ] Test EnemyTypeRegistry with various config scenarios
- [ ] Test placement strategies with different building sets
- [ ] Test wave progression with different enemy sets

#### **[ ] 7.2 Integration Tests**

- [ ] Test full simulation with custom building names
- [ ] Test simulation with missing building types
- [ ] Test simulation with custom enemy progression

#### **[ ] 7.3 Documentation**

- [ ] Document how to add new building types
- [ ] Document how to add new enemy types
- [ ] Document placement strategy configuration
- [ ] Document wave progression configuration

---

## **Benefits After Implementation**

### **For Development:**

- [ ] **Zero Hardcoded Type Names** - All types managed through registries
- [ ] **Easy Name Changes** - Change display names without code changes
- [ ] **Safe Refactoring** - Find/replace operations work on internal IDs
- [ ] **Type Safety** - Enum-like behavior with string flexibility

### **For Game Design:**

- [ ] **Easy Type Addition** - Add new building/enemy types via config
- [ ] **Flexible Categorization** - Group types for game logic (starter, advanced, etc.)
- [ ] **Configurable Progression** - Change wave/placement logic via config files
- [ ] **A/B Testing** - Test different type names and progressions

### **For Maintenance:**

- [ ] **Config Validation** - Catch type reference errors at startup
- [ ] **Clear Error Messages** - Know exactly which types are missing/invalid
- [ ] **Debug Tools** - Inspect type registries during development

---

## **Migration Strategy**

### **[ ] Option A: Internal ID Approach** (Recommended)

- Keep existing names like "basic_tower" as internal IDs
- Add display names and categories via config
- Code references internal IDs, UI shows display names
- Easy migration, backward compatible

### **[ ] Option B: Full String Replacement**

- Create enums for all building/enemy types
- Global find/replace hardcoded strings with enum values
- Config maps enum values to display names
- More type-safe but requires more code changes

---

## **Implementation Timeline**

- **Phase 1-2**: Foundation & Validation (2-3 hours)
- **Phase 3**: Building Placement (2 hours)
- **Phase 4**: Enemy Logic (2 hours)
- **Phase 5**: Registry Services (3 hours)
- **Phase 6**: Validation & Error Handling (1-2 hours)
- **Phase 7**: Testing & Documentation (2-3 hours)

**Total Estimated Time**: 12-15 hours

---

## **Success Criteria**

- [ ] No hardcoded building/enemy type strings in application code
- [ ] All type references go through type registries
- [ ] Easy to add new types via config files only
- [ ] Easy to change display names without code changes
- [ ] Robust error handling for missing/invalid types
- [ ] Full test coverage for type registry functionality
