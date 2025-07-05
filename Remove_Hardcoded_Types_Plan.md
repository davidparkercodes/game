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
      "BASIC_TOWER": { "config_key": "basic_tower", "display_name": "Basic Tower", "category": "starter" },
      "SNIPER_TOWER": { "config_key": "sniper_tower", "display_name": "Sniper Tower", "category": "precision" }
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

### **Phase 4: Replace Hardcoded Enemy Logic** 👹

#### **[ ] 4.1 Fix Enemy Type Selection in GameSimRunner**
- [ ] Replace hardcoded enemy types in `GetEnemyTypeForWave()`:
  - [ ] Remove `"boss_enemy"` hardcode
  - [ ] Remove `"elite_enemy"` hardcode  
  - [ ] Remove `"tank_enemy"` hardcode
  - [ ] Remove `"fast_enemy"` hardcode
  - [ ] Remove `"basic_enemy"` hardcode
- [ ] Use `EnemyTypeRegistry.GetByTier()` or `EnemyTypeRegistry.GetByCategory()`
- [ ] Create wave progression rules based on enemy categories

#### **[ ] 4.2 Fix MockEnemyStatsProvider**
- [ ] Replace hardcoded enemy type references
- [ ] Use `EnemyTypeRegistry` for all enemy type lookups
- [ ] Remove hardcoded fallback enemy types

#### **[ ] 4.3 Create Enemy Wave Configuration**
- [ ] Add `wave_progression.json` config file:
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
      "tank_frequency": 3
    }
  }
}
```

### **Phase 5: Create Type Management System** 🔧

#### **[ ] 5.1 Build Type Registry Services**
- [ ] Implement `BuildingTypeRegistry` with methods:
  - `GetByInternalId(string id)` 
  - `GetByConfigKey(string key)`
  - `GetByCategory(string category)`
  - `GetDefaultType()` / `GetCheapestType()`
  - `IsValidType(string key)`
  - `GetAllByTier(int tier)`

#### **[ ] 5.2 Build Enemy Type Registry Services**
- [ ] Implement `EnemyTypeRegistry` with similar pattern
- [ ] Add methods for wave progression logic
- [ ] Support for enemy tier/difficulty scaling

#### **[ ] 5.3 Registry Integration**
- [ ] Register services in DI container
- [ ] Ensure all existing code uses registries
- [ ] Add startup validation for registry consistency

### **Phase 6: Configuration Validation & Error Handling** 🛡️

#### **[ ] 6.1 Startup Validation**
- [ ] Validate all building types in config exist in registry
- [ ] Validate all enemy types in config exist in registry  
- [ ] Validate placement strategies reference valid building categories
- [ ] Validate wave progression references valid enemy categories

#### **[ ] 6.2 Runtime Error Handling**
- [ ] Graceful fallbacks when types are missing
- [ ] Clear error messages for invalid type references
- [ ] Logging for type registry mismatches

#### **[ ] 6.3 Development Tools**
- [ ] Add debug command to list all registered building types
- [ ] Add debug command to list all registered enemy types
- [ ] Add validation tool for config consistency

### **Phase 7: Testing & Documentation** 📋

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
