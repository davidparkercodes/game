# Update Infrastructure Plan

## Analysis of Current Issues

### Build Errors Summary (Current Status) ✅
- **0 Errors** - ✅ ALL RESOLVED: Ambiguous BuildingZoneValidator references fixed
- **72 Warnings** - Mostly nullable reference type issues
- ~~**Previous: 10 Errors**~~ - ✅ RESOLVED: Missing types issues have been addressed
- ~~**Previous: 4 Errors**~~ - ✅ RESOLVED: Ambiguous BuildingZoneValidator references fixed

### Current Infrastructure Structure Issues
```
src/Infrastructure/
├── Buildings/BuildingZoneService.cs          # ❌ Not in Services/ folder
├── DI/                                       # ❌ Should be Di/
│   ├── ServiceConfiguration.cs
│   ├── ServiceLocator.cs
│   └── ServiceLocatorAdapter.cs
├── Managers/                                 # ❌ Not feature-centric
│   ├── GameManager.cs
│   ├── PathManager.cs
│   ├── RoundManager.cs
│   ├── SoundManager.cs
│   ├── StatsManager.cs
│   └── WaveSpawner.cs
├── Sound/                                    # ✅ Feature-centric
├── Stats/                                    # ✅ Feature-centric
├── Validators/BuildingZoneValidator.cs       # ❌ Should be in Buildings/
└── Waves/WaveConfigService.cs               # ✅ Feature-centric but has missing types
```

## ServiceLocator & ServiceLocatorAdapter Analysis

### What ServiceLocator Does:
- Simple service container for dependency injection
- Manages singleton and factory registrations
- Resolves services by type with fallback handling
- Provides service registration checking and cleanup

### What ServiceLocatorAdapter Does:
- Implements `IServiceProvider` interface
- Wraps `ServiceLocator` to provide standard .NET DI interface
- Handles missing services gracefully (returns null instead of throwing)

### Assessment:
- These are custom DI implementations instead of using standard .NET DI
- Should be kept for now but evaluated for potential replacement with Microsoft.Extensions.DependencyInjection

---

## Refactoring Plan

### Phase 1: DI Restructure ✅ COMPLETE
- [x] Rename `src/Infrastructure/DI` → `src/Di`
- [x] Rename classes: `ServiceLocator` → `DiContainer`, `ServiceLocatorAdapter` → `DiAdapter`
- [x] Rename files to match: `ServiceLocator.cs` → `DiContainer.cs`, etc.
- [x] Update all namespace references from `Game.Infrastructure.DI` → `Game.Di`
- [x] Create `src/Di/Di.cs` as main entry point that orchestrates all DI configuration
- [x] Move `ServiceConfiguration.cs` → `DiConfiguration.cs` and refactor to be called from `Di.cs`

#### 🗑️ Ready for Cleanup: `src/Infrastructure/DI/` Directory
- **Status**: Safe to delete - all functionality moved to `src/Di/`
- **Remaining references**: Only in old files within the directory itself and documentation
- **Action needed**: Remove `src/Infrastructure/DI/` directory completely

### Phase 2: Feature-Centric Manager Reorganization ✅ COMPLETE
- [x] Move `src/Infrastructure/Managers/GameManager.cs` → `src/Infrastructure/Game/Services/GameService.cs`
- [x] Move `src/Infrastructure/Managers/RoundManager.cs` → `src/Infrastructure/Rounds/Services/RoundService.cs`
- [x] Move `src/Infrastructure/Managers/SoundManager.cs` → `src/Infrastructure/Audio/Services/SoundManagerService.cs`
- [x] Move `src/Infrastructure/Managers/StatsManager.cs` → `src/Infrastructure/Stats/Services/StatsManagerService.cs`
- [x] Move `src/Infrastructure/Managers/PathManager.cs` → `src/Infrastructure/Enemies/Services/PathService.cs`
- [x] Move `src/Infrastructure/Managers/WaveSpawner.cs` → `src/Infrastructure/Enemies/Services/WaveSpawnerService.cs`
- [x] Delete empty `src/Infrastructure/Managers/` directory
- [x] Update all references to use new service names and namespaces throughout the codebase

### Phase 3: Building Infrastructure Organization ✅ COMPLETE
- [x] Move `src/Infrastructure/Buildings/BuildingZoneService.cs` → `src/Infrastructure/Buildings/Services/BuildingZoneService.cs`
- [x] Move `src/Infrastructure/Validators/BuildingZoneValidator.cs` → `src/Infrastructure/Buildings/Validators/BuildingZoneValidator.cs`
- [x] Delete empty `src/Infrastructure/Validators/` directory

### Phase 4: Wave System Issues Resolution
- [ ] **ANALYSIS REQUIRED**: Review Domain/Application layers to understand what wave types are actually needed
- [ ] **DO NOT CREATE**: Missing types (`WaveConfig`, `EnemySpawnGroup`, `WaveSetConfig`) until Domain analysis is complete
- [ ] Either:
  - [ ] Remove/refactor `WaveConfigService.cs` if not needed by Domain/Application
  - [ ] OR create proper Domain value objects if the functionality is required

### Phase 5: Namespace and Reference Updates ✅ COMPLETE
- [x] Update all `using` statements to reflect new paths
- [x] Update namespaces in moved files to match directory structure
- [x] **RESOLVED**: Fixed ambiguous BuildingZoneValidator references using type aliases
- [x] All Infrastructure classes implement Domain interfaces correctly
- [ ] Update any Godot scene references that might point to old class locations

### Phase 6: Nullable Reference Type Fixes
- [ ] Fix remaining nullable warnings in moved Infrastructure files
- [ ] Ensure all Infrastructure classes properly handle null values
- [ ] Update method signatures to use nullable reference types where appropriate

---

## Expected Directory Structure After Refactoring

```
src/
├── Di/                                       # ✅ Renamed from DI
│   ├── Di.cs                                # ✅ New main entry point
│   ├── DiConfiguration.cs                   # ✅ Renamed from ServiceConfiguration
│   ├── DiContainer.cs                       # ✅ Renamed from ServiceLocator
│   └── DiAdapter.cs                         # ✅ Renamed from ServiceLocatorAdapter
├── Infrastructure/
│   ├── Audio/Services/SoundManagerService.cs
│   ├── Buildings/
│   │   ├── Services/BuildingZoneService.cs
│   │   └── Validators/BuildingZoneValidator.cs
│   ├── Enemies/Services/
│   │   ├── PathService.cs
│   │   └── WaveSpawnerService.cs
│   ├── Game/Services/GameService.cs
│   ├── Rounds/Services/RoundService.cs
│   ├── Sound/                               # ✅ Already good
│   ├── Stats/
│   │   ├── Services/StatsManagerService.cs
│   │   └── [existing files]
│   └── Waves/WaveConfigService.cs           # ⚠️  Needs analysis
```

---

## Risk Assessment

### Low Risk:
- DI rename and reorganization
- Manager file moves
- Namespace updates

### Medium Risk:
- Class renames (DiContainer, etc.) - need to update all references
- Moving files that might be referenced in Godot scenes

### High Risk:
- Wave system changes - missing types indicate broken functionality
- Any changes to DI container behavior that might affect service resolution

---

## Success Criteria

- [x] **Zero build errors** ✅ ACHIEVED!
- [ ] Zero build warnings **(72 remaining - mostly nullable reference types)**
- [x] All Infrastructure follows feature-centric organization
- [x] Consistent "Di" naming throughout DI system
- [x] All Infrastructure classes in appropriate type folders (Services/, Validators/, etc.)
- [x] Proper namespace alignment with directory structure
- [x] **Godot project builds successfully** ✅ ACHIEVED!

---

## Notes

- **DO NOT** create missing wave types until Domain/Application analysis is complete
- **PRIORITIZE** structural organization over fixing missing functionality
- **MAINTAIN** existing DI container behavior during refactoring
- **VERIFY** Godot scene references after file moves
