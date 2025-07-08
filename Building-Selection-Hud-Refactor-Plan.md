# Building Selection HUD Refactor Plan

**Goal**: Refactor TowerSelectionHud to BuildingSelectionHud for general building types, and add audio feedback for selection/deselection.

## Execution Instructions

**Process**: Execute phases one at a time. When a phase is complete:

1. Update this plan file to mark completed items
2. Run `dotnet clean && dotnet build && dotnet test`
3. If all pass, proceed to next phase
4. If warnings or errors occur, fix them before proceeding
5. Continue until all phases are complete

---

## Phase 1: Rename Core Components ✅ **COMPLETED**

**Goal**: Rename all TowerSelection-related components to BuildingSelection

### [x] 1.1 Rename Configuration Classes ✅ **COMPLETED**

- [x] Rename `TowerSelectionHudConfig` → `BuildingSelectionHudConfig`
- [x] Rename `TowerDisplayConfig` → `BuildingDisplayConfig`
- [x] Update all property references and usages
- [x] Maintain existing functionality and structure

### [x] 1.2 Rename Service Classes ✅ **COMPLETED**

- [x] Rename `ITowerSelectionHudConfigService` → `IBuildingSelectionHudConfigService`
- [x] Rename `TowerSelectionHudConfigService` → `BuildingSelectionHudConfigService`
- [x] Update all method names and references
- [x] Update DI registration in `DiConfiguration.cs`

### [x] 1.3 Rename UI Component ✅ **COMPLETED**

- [x] Rename `TowerSelectionHud.cs` → `BuildingSelectionHud.cs`
- [x] Update class name and all internal references
- [x] Update logging prefixes from `[TOWER_HUD]` → `[BUILDING_HUD]`
- [x] Update all method names and variables

### [x] 1.4 Update Configuration File ✅ **COMPLETED**

- [x] Rename `data/huds/tower_selection_hud.json` → `data/huds/building_selection_hud.json`
- [x] Update JSON property names from `towers` → `buildings`
- [x] Update tower type keys to building type keys
- [x] Update file path constants in service classes

---

## Phase 2: Update Terminology and References ✅ **COMPLETED**

**Goal**: Replace tower terminology with building terminology throughout

### [x] 2.1 Update Variable and Method Names ✅ **COMPLETED**

- [x] Replace all `tower` variables with `building`
- [x] Replace all `Tower` method names with `Building`
- [x] Update method signatures and parameters
- [x] Update internal documentation and comments

### [x] 2.2 Update Configuration Properties ✅ **COMPLETED**

- [x] Update JSON structure: `towers` → `buildings`
- [x] Update building type identifiers (keep `basic_tower`, etc. for compatibility)
- [x] Update display names and tooltips
- [x] Update service method names

### [x] 2.3 Update Integration Points ✅ **COMPLETED**

- [x] Update Player.cs integration calls
- [x] Update HUD scene references
- [x] Update all logging and debug messages
- [x] Ensure backward compatibility where needed

### [x] 2.4 Remove Hardcoded Tower Type Strings ✅ **COMPLETED**

- [x] Replace hardcoded strings in `BuildingSelectionHud.cs` with config-driven approach
- [x] Replace hardcoded strings in `Player.cs` with Domain ConfigKey constants
- [x] Update all method mappings to use Domain constants
- [x] Ensure config-driven architecture compliance per AI_CONTEXT.md

---

## Phase 3: Audio Feedback System ✅ **COMPLETED**

**Goal**: Implement audio feedback for building selection and deselection

### [x] 3.1 Audio Configuration ✅ **COMPLETED**

- [x] Add audio properties to `BuildingSelectionHudConfig`
- [x] Add `select_sound_path` and `deselect_sound_path` to JSON config
- [x] Update configuration models to include audio settings
- [x] Set default paths to new audio files

### [x] 3.2 Audio Service Integration ✅ **COMPLETED**

- [x] Import audio files into Godot project with proper settings
- [x] Verify audio files are properly imported (OGG Vorbis format)
- [x] Test audio file accessibility via resource paths
- [x] Add audio validation to config service

### [x] 3.3 Audio Playback Implementation ✅ **COMPLETED**

- [x] Add SoundManagerService integration to BuildingSelectionHud
- [x] Implement `PlaySelectionSound()` method
- [x] Implement `PlayDeselectionSound()` method
- [x] Add audio playback to selection change events

### [x] 3.4 Audio Event Integration ✅ **COMPLETED**

- [x] Add audio to mouse click selection/deselection
- [x] Add audio to keyboard hotkey selection/deselection
- [x] Add audio to programmatic selection changes
- [x] Ensure audio plays only once per state change

### [x] 3.5 Sound Configuration Integration ✅ **COMPLETED**

- [x] Add `tower_select` and `tower_deselect` sounds to `sound_config.json`
- [x] Configure proper UI audio category and volume levels
- [x] Integrate with existing SoundManagerService architecture
- [x] Test audio files exist and are accessible

---

## Phase 4: Configuration and Service Updates

**Goal**: Update all configuration and service references

### [ ] 4.1 Update JSON Configuration Structure

- [ ] Finalize `building_selection_hud.json` structure
- [ ] Add audio configuration section
- [ ] Update building type definitions
- [ ] Validate JSON schema and structure

### [ ] 4.2 Service Architecture Updates

- [ ] Update all service interfaces and implementations
- [ ] Update DI container registrations
- [ ] Update service discovery and initialization
- [ ] Add audio service dependencies

### [ ] 4.3 Error Handling and Validation

- [ ] Update error messages and logging
- [ ] Add audio file validation
- [ ] Add graceful fallbacks for missing audio
- [ ] Update configuration validation logic

---

## Phase 5: Integration Testing and Polish

**Goal**: Ensure all integrations work correctly with new naming

### [ ] 5.1 Player Integration Updates

- [ ] Update Player.cs method calls (or similar classes or new -- I don't wnat Player.cs to get too big)
- [ ] Update building selection logic references
- [ ] Test keyboard input integration (1-4 keys)
- [ ] Test mouse click integration

### [ ] 5.2 HUD Integration Updates

- [ ] Update main HUD scene integration
- [ ] Update HudManager integration calls
- [ ] Test visual feedback systems
- [ ] Test tower information display

### [ ] 5.3 Audio System Testing

- [ ] Test selection audio on mouse clicks
- [ ] Test deselection audio on toggle clicks
- [ ] Test audio on keyboard selection (1-4)
- [ ] Test audio volume and quality
- [ ] Test audio fallbacks for missing files

---

## Phase 6: Documentation and Cleanup

**Goal**: Update documentation and clean up old references

### [ ] 6.1 Update Documentation

- [ ] Update all code comments and documentation
- [ ] Update method and class summaries
- [ ] Update configuration documentation
- [ ] Update integration notes

### [ ] 6.2 Code Cleanup

- [ ] Remove any unused tower-specific references
- [ ] Clean up temporary files and old configs
- [ ] Verify no hardcoded tower references remain
- [ ] Update variable names for consistency

### [ ] 6.3 Final Validation

- [ ] Run full build and test suite
- [ ] Test all building selection functionality
- [ ] Test audio feedback in all scenarios
- [ ] Verify configuration loading works correctly

---

## Technical Specifications

### Audio Requirements:

- **Selection Sound**: `res://assets/audio/towers/tower_select.mp3`
- **Deselection Sound**: `res://assets/audio/towers/tower_deselect.mp3`
- **Format**: MP3 (will be converted to OGG Vorbis by Godot)
- **Category**: SFX
- **Volume**: Configurable via existing sound system

### Configuration Updates:

```json
{
  "layout": {
    /* existing layout config */
  },
  "styling": {
    /* existing styling config */
  },
  "audio": {
    "select_sound_path": "res://assets/audio/towers/tower_select.mp3",
    "deselect_sound_path": "res://assets/audio/towers/tower_deselect.mp3",
    "enabled": true
  },
  "buildings": {
    "basic_tower": {
      /* existing tower config */
    },
    "sniper_tower": {
      /* existing tower config */
    },
    "rapid_tower": {
      /* existing tower config */
    },
    "heavy_tower": {
      /* existing tower config */
    }
  }
}
```

### File Renames:

- `src/Application/UI/Configuration/TowerSelectionHudConfig.cs` → `BuildingSelectionHudConfig.cs`
- `src/Application/UI/Services/ITowerSelectionHudConfigService.cs` → `IBuildingSelectionHudConfigService.cs`
- `src/Infrastructure/UI/Services/TowerSelectionHudConfigService.cs` → `BuildingSelectionHudConfigService.cs`
- `src/Presentation/UI/TowerSelectionHud.cs` → `BuildingSelectionHud.cs`
- `data/huds/tower_selection_hud.json` → `building_selection_hud.json`

### Compatibility Notes:

- Maintain existing building type keys (`basic_tower`, `sniper_tower`, etc.) for compatibility
- Keep existing Player.cs method signatures where possible
- Preserve all existing functionality while adding new audio features
- Ensure smooth transition with no breaking changes to existing game systems

---

## Success Criteria

### Core Functionality:

- [ ] All building selection functionality works with new naming
- [ ] Keyboard shortcuts (1-4) work correctly
- [ ] Mouse selection works correctly
- [ ] Visual feedback systems work unchanged
- [ ] Configuration system loads new JSON structure

### Audio Functionality:

- [ ] Selection sound plays when selecting a building
- [ ] Deselection sound plays when deselecting a building
- [ ] Audio integrates properly with existing sound system
- [ ] Audio can be disabled via configuration
- [ ] No audio errors or exceptions occur

### Code Quality:

- [ ] All code follows clean code principles
- [ ] No hardcoded tower references remain
- [ ] Consistent naming throughout codebase
- [ ] Proper error handling and logging
- [ ] All tests pass without warnings or errors

---

## Notes

This refactor maintains all existing functionality while:

1. **Generalizing the system** for future building types beyond towers
2. **Adding audio feedback** for better user experience
3. **Maintaining compatibility** with existing game systems
4. **Following established patterns** in the codebase
5. **Preserving configuration-driven architecture**

The refactor is designed to be safe and incremental, with each phase building on the previous one while maintaining a working system throughout the process.
