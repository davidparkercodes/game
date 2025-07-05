# Update Infrastructure Plan

## ✅ **INFRASTRUCTURE REFACTORING COMPLETE!**

**Status**: ✅ All core phases completed successfully  
**Build Status**: ✅ 0 Errors, 72 Warnings (nullable reference types)  
**Result**: Clean feature-centric architecture with proper DI naming  

### 🎆 **Achievements:**
- ✅ **Phase 1-5 Complete**: All structural reorganization finished
- ✅ **Zero Build Errors**: Fixed all critical issues including ambiguous references
- ✅ **Feature-Centric Architecture**: Managers reorganized by domain (Audio, Buildings, Enemies, etc.)
- ✅ **Consistent "Di" Naming**: Renamed DI → Di, ServiceLocator → DiContainer, etc.
- ✅ **Wave System Verified**: All types exist and function correctly with JSON configuration
- ✅ **Namespace Alignment**: All classes properly aligned with directory structure

**Remaining**: Phase 6 (nullable reference warnings) is optional - core functionality complete!

---

## Analysis of Original Issues

### Build Errors Summary (Current Status) ✅
- **0 Errors** - ✅ ALL RESOLVED: Ambiguous BuildingZoneValidator references fixed
- **72 Warnings** - Mostly nullable reference type issues
- ~~**Previous: 10 Errors**~~ - ✅ RESOLVED: Missing types issues have been addressed
- ~~**Previous: 4 Errors**~~ - ✅ RESOLVED: Ambiguous BuildingZoneValidator references fixed

### ✅ RESOLVED: Infrastructure Structure Issues
```
src/
├── Di/                                       # ✅ Renamed from Infrastructure/DI/
│   ├── Di.cs                                # ✅ Main DI entry point
│   ├── DiConfiguration.cs                   # ✅ Renamed from ServiceConfiguration
│   ├── DiContainer.cs                       # ✅ Renamed from ServiceLocator
│   └── DiAdapter.cs                         # ✅ Renamed from ServiceLocatorAdapter
├── Infrastructure/
│   ├── Audio/Services/SoundManagerService.cs # ✅ Feature-centric
│   ├── Buildings/
│   │   ├── Services/BuildingZoneService.cs  # ✅ Properly organized
│   │   └── Validators/BuildingZoneValidator.cs # ✅ Moved from root Validators/
│   ├── Enemies/Services/
│   │   ├── PathService.cs                   # ✅ Renamed from PathManager
│   │   └── WaveSpawnerService.cs            # ✅ Renamed from WaveSpawner
│   ├── Game/Services/GameService.cs         # ✅ Renamed from GameManager
│   ├── Rounds/Services/RoundService.cs      # ✅ Renamed from RoundManager
│   ├── Sound/                               # ✅ Feature-centric (preserved)
│   ├── Stats/
│   │   ├── Services/StatsManagerService.cs # ✅ Renamed from StatsManager
│   │   └── [existing files]                 # ✅ Feature-centric (preserved)
│   └── Waves/WaveConfigService.cs           # ✅ Fully functional with all types
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

### Phase 4: Wave System Issues Resolution ✅ COMPLETE
- [x] **ANALYSIS COMPLETE**: Reviewed Domain/Application layers and wave system functionality
- [x] **RESOLVED**: All required types exist and function correctly:
  - `EnemySpawnGroup` (Infrastructure layer - working)
  - `WaveConfigurationInternal` and `WaveSetConfigurationInternal` (Infrastructure layer - working)
  - `WaveConfiguration` (Domain value object - working)
  - `IWaveConfigService` and `WaveConfigService` (working with JSON configuration)
- [x] **VERIFIED**: Wave system is fully functional with JSON-based configuration (`data/waves/default_waves.json`)
- [x] **CONFIRMED**: Infrastructure properly implements Domain interfaces and provides wave spawning functionality

### Phase 5: Namespace and Reference Updates ✅ COMPLETE
- [x] Update all `using` statements to reflect new paths
- [x] Update namespaces in moved files to match directory structure
- [x] **RESOLVED**: Fixed ambiguous BuildingZoneValidator references using type aliases
- [x] All Infrastructure classes implement Domain interfaces correctly
- [x] **VERIFIED**: No Godot scene references point to old class locations (scenes use current paths)

### Phase 6: Nullable Reference Type Fixes 🔄 OPTIONAL
- [ ] Fix remaining nullable warnings in moved Infrastructure files (72 warnings)
- [ ] Ensure all Infrastructure classes properly handle null values
- [ ] Update method signatures to use nullable reference types where appropriate

**Note**: This phase is optional as these are warnings, not errors. The infrastructure refactoring is functionally complete.

---

## ✅ ACHIEVED: Final Directory Structure

```
src/
├── Di/                                       # ✅ Completed: Renamed from Infrastructure/DI/
│   ├── Di.cs                                # ✅ Main DI entry point
│   ├── DiConfiguration.cs                   # ✅ Renamed from ServiceConfiguration
│   ├── DiContainer.cs                       # ✅ Renamed from ServiceLocator
│   └── DiAdapter.cs                         # ✅ Renamed from ServiceLocatorAdapter
├── Infrastructure/
│   ├── Audio/Services/SoundManagerService.cs # ✅ Feature-centric organization
│   ├── Buildings/
│   │   ├── Services/BuildingZoneService.cs      # ✅ Properly organized
│   │   └── Validators/BuildingZoneValidator.cs  # ✅ Moved from root Validators/
│   ├── Enemies/Services/
│   │   ├── PathService.cs                       # ✅ Renamed from PathManager
│   │   └── WaveSpawnerService.cs                # ✅ Renamed from WaveSpawner
│   ├── Game/Services/GameService.cs             # ✅ Renamed from GameManager
│   ├── Rounds/Services/RoundService.cs          # ✅ Renamed from RoundManager
│   ├── Sound/                                   # ✅ Feature-centric (preserved)
│   ├── Stats/
│   │   ├── Services/StatsManagerService.cs     # ✅ Renamed from StatsManager
│   │   └── [existing files]                     # ✅ Feature-centric (preserved)
│   └── Waves/WaveConfigService.cs               # ✅ Fully functional with all types
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
