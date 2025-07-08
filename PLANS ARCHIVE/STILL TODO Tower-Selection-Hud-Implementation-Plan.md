# Tower Selection HUD Implementation Plan

## Execution Instructions
**Process**: Execute phases one at a time. When a phase is complete:
1. Update this plan file to mark completed items
2. Run `dotnet clean && dotnet build && dotnet test`
3. If all pass, proceed to next phase
4. If warnings or errors occur, fix them before proceeding
5. Continue until all phases are complete

## Overview
Implement a tower selection HUD at the bottom center of the screen that displays 4 tower types (expandable to 0-9) with visual previews, numbered selection (1-4), selection highlighting, and config-driven styling. The system will integrate with the existing config-driven architecture following the established patterns in the codebase.

## Current Architecture Analysis
- **4 Tower Types**: basic_tower, sniper_tower, rapid_tower, heavy_tower
- **Config System**: Uses `data/stats/building_stats.json` for tower stats
- **Service Pattern**: `StatsService` loads config data
- **Existing HUD**: `src/Presentation/UI/Hud.cs` and `scenes/UI/Hud.tscn`
- **Tower Selection**: Currently handled in `src/Presentation/Player/Player.cs` with keyboard (1-4)
- **Building System**: Uses `PlayerBuildingBuilder` for placement

---

## Phase 1: Configuration System
**Goal**: Create config-driven HUD styling and tower display settings

### [x] 1.1 Create HUD Configuration Schema ✅ **COMPLETED**
- [x] Create `data/huds/tower_selection_hud.json` with structure **CREATED**
- [x] Added complete JSON configuration with layout, styling, and tower definitions
- [x] Configured for 4 tower types with extensible design for up to 9 towers
- [x] Includes all visual styling properties and hotkey mappings

### [x] 1.2 Create Configuration Models ✅ **COMPLETED**
- [x] Create `src/Application/UI/Configuration/TowerSelectionHudConfig.cs` **CREATED**
  - [x] TowerSelectionHudConfig class with Layout, Styling, and Towers properties
  - [x] HudLayout class with positioning/sizing properties
  - [x] HudStyling class with colors and fonts
  - [x] TowerDisplayConfig class with icon and hotkey info
- [x] Follow existing pattern in `src/Infrastructure/Stats/StatsService.cs` **FOLLOWED**
- [x] Use PascalCase naming convention following user rules **APPLIED**

### [x] 1.3 Create HUD Configuration Service ✅ **COMPLETED**
- [x] Create `src/Infrastructure/UI/Services/TowerSelectionHudConfigService.cs` **CREATED**
- [x] Implement JSON loading following `StatsService` pattern **IMPLEMENTED**
- [x] Add Godot file access with error handling **ADDED**
- [x] Create interface `src/Application/UI/Services/ITowerSelectionHudConfigService.cs` **CREATED**
- [x] Follow clean code principles with no unnecessary comments **FOLLOWED**

**✅ PHASE 1 COMPLETED:** Configuration system is fully implemented with JSON config file, data models, and service layer following established patterns.

---

## Phase 2: Tower Icon Assets
**Goal**: Create or prepare tower icon assets for the HUD display

### [x] 2.1 Create Tower Icon Assets ✅ **COMPLETED**
- [x] Create 64x64 tower icons in `assets/sprites/ui/`: **CREATED**
  - [x] `basic_tower_icon.png` - Simple balanced tower design **CREATED**
  - [x] `sniper_tower_icon.png` - Long-range tower with scope/rifle elements **CREATED**
  - [x] `rapid_tower_icon.png` - Multi-barrel rapid-fire design **CREATED**
  - [x] `heavy_tower_icon.png` - Large, heavy cannon design **CREATED**
- [x] Use consistent visual style matching game aesthetic **APPLIED**
- [x] Ensure icons are distinct and recognizable at small sizes **VERIFIED**
- [x] Updated JSON config with correct icon paths **UPDATED**

### [x] 2.2 Fallback Icon System ✅ **COMPLETED**
- [x] Create default tower icon `assets/sprites/ui/default_tower_icon.png` **CREATED**
- [x] Add icon validation in configuration service **ADDED**
- [x] Implement fallback to default icon if specific icon missing **IMPLEMENTED**
- [x] Log warnings for missing tower icons **ADDED**
- [x] Added `GetValidatedIconPath()` and `DoesIconExist()` methods **IMPLEMENTED**

**✅ PHASE 2 COMPLETED:** All tower icons created with fallback system and validation in place.

---

## Phase 3: Core HUD Component
**Goal**: Create the tower selection HUD component with proper layout

### [x] 3.1 Create TowerSelectionHud Scene ✅ **COMPLETED**
- [x] Create `scenes/UI/TowerSelectionHud.tscn`: **CREATED**
  - [x] Root: Control node (anchored to bottom center) **CREATED**
  - [x] HBoxContainer for tower squares layout **CREATED**

### [x] 3.2 Create TowerSelectionHud Script ✅ **COMPLETED**
- [x] Create `src/Presentation/UI/TowerSelectionHud.cs`: **CREATED**
  - [x] Inherit from Control **IMPLEMENTED**
  - [x] Load configuration from TowerSelectionHudConfigService **IMPLEMENTED**
  - [x] Dynamic tower slot creation based on config **IMPLEMENTED**
  - [x] Tower selection state management **IMPLEMENTED**
  - [x] Visual styling with config-driven colors and borders **IMPLEMENTED**
  - [x] Hover effects and selection highlighting **IMPLEMENTED**
  - [x] Hotkey input handling **IMPLEMENTED**
  - [x] Integration with Player building selection **IMPLEMENTED**

### [x] 3.3 Implement Layout System ✅ **COMPLETED**
- [x] Calculate positioning based on number of towers and config settings **IMPLEMENTED**
- [x] Center HUD horizontally regardless of tower count (1-9) **IMPLEMENTED**
- [x] Implement responsive spacing with HBoxContainer alignment **IMPLEMENTED**
- [x] Apply config-driven sizing (square_size from config) **IMPLEMENTED**
- [x] Add proper anchoring for different screen sizes **IMPLEMENTED**

### [x] 3.4 Integration with Main Scene ✅ **COMPLETED**
- [x] Add TowerSelectionHud initialization to Main.cs **ADDED**
- [x] Register TowerSelectionHudConfigService in DI configuration **REGISTERED**
- [x] Ensure proper load order and scene tree integration **IMPLEMENTED**

**✅ PHASE 3 COMPLETED:** Core HUD component is fully implemented with config-driven layout, styling, and Player integration. Build successful with no compilation errors.

### [x] 3.5 Phase 3 Improvements ✅ **COMPLETED**
- [x] **Fixed positioning**: Updated scene anchoring to bottom-center like Mac dock **FIXED**
- [x] **Use actual building sprites**: Replaced custom icons with tilemap regions from actual tower sprites **IMPLEMENTED**
- [x] **Added icon_region support**: Enhanced config model and loading to support tilemap regions **ADDED**
- [x] **Fixed HeavyTower sprite**: Corrected region to use unique sprite (48,64,16,16) **FIXED**
- [x] **Improved layout**: Better HBoxContainer positioning within control bounds **IMPROVED**
- [x] **AtlasTexture implementation**: Proper region extraction from tilemap for 16x16 sprites scaled to button size **IMPLEMENTED**

### [x] 3.6 Architectural Fix ✅ **COMPLETED**
- [x] **Fixed HUD integration**: Moved TowerSelectionHud into main HUD CanvasLayer instead of separate scene **FIXED**
- [x] **Proper viewport behavior**: Now follows camera movement like other HUD elements **FIXED**
- [x] **Removed standalone scene**: Cleaned up unnecessary TowerSelectionHud.tscn file **REMOVED**
- [x] **Updated Main.cs**: Removed separate initialization since TowerSelectionHud is now part of HUD scene **UPDATED**

---

## Phase 4: Selection Logic Integration
**Goal**: Connect the HUD to existing tower selection and building systems

### [x] 4.1 Integrate with Player Tower Selection ✅ **COMPLETED**
- [x] Modify `src/Presentation/Player/Player.cs`: **MODIFIED**
  - [x] Add TowerSelectionHud reference **ADDED**
  - [x] Update existing hotkey handlers (1-4) to notify HUD **UPDATED**
  - [x] Maintain existing tower selection functionality **MAINTAINED**
  - [x] Add `InitializeTowerSelectionHud()` method for deferred HUD connection **ADDED**
  - [x] Add `SyncHudSelectionState()` for initial state synchronization **ADDED**
  - [x] Add `NotifyHudSelectionChange()` for bidirectional communication **ADDED**
  - [x] Update all selection methods (SelectBuilding, ClearBuildingSelection, etc.) **UPDATED**
  - [x] Add mapping between BuildingScene references and tower config keys **ADDED**

### [x] 4.2 Bidirectional Communication ✅ **COMPLETED**
- [x] **Player → HUD**: All Player tower selections notify HUD visual state **IMPLEMENTED**
- [x] **HUD → Player**: TowerSelectionHud button clicks call Player.SelectBuilding **IMPLEMENTED**  
- [x] **State synchronization**: HUD reflects current Player selection on initialization **IMPLEMENTED**
- [x] **Display name mapping**: TowerSelectionHud converts config keys to Player display names **ADDED**
- [x] **Added 'hud' group**: CanvasLayer tagged for easy Player discovery **ADDED**

### [x] 4.3 Mouse Selection Support ✅ **COMPLETED** 
- [x] Implement click handlers on tower slots in TowerSelectionHud **IMPLEMENTED**
- [x] Add hover effects with config-driven styling **IMPLEMENTED**
- [x] Connect mouse clicks to existing tower selection logic **IMPLEMENTED**
- [x] Ensure mouse and keyboard selection stay synchronized **IMPLEMENTED**

### [x] 4.4 Selection State Management ✅ **COMPLETED**
- [x] Create selection state tracking in TowerSelectionHud **IMPLEMENTED**
- [x] Apply visual styling changes (border colors) on selection **IMPLEMENTED**
- [x] SetSelectedTower() and ClearSelection() methods **IMPLEMENTED**
- [x] Toggle selection behavior for mouse clicks **IMPLEMENTED**

### [x] 4.5 Fix Double Input Processing ✅ **COMPLETED**
- [x] **Issue identified**: Both Player and TowerSelectionHud were processing same hotkey inputs **IDENTIFIED**
- [x] **Circular clearing calls**: ClearBuildingSelection → CancelBuildMode → ClearPlayerSelectionState → NotifyHudSelectionChange **IDENTIFIED**
- [x] **Removed duplicate NotifyHudSelectionChange calls**: From keyboard handlers since ClearBuildingSelection already notifies **FIXED**
- [x] **Removed hotkey handling from TowerSelectionHud**: Player handles all input, HUD receives notifications **FIXED**
- [x] **Removed circular HUD notification**: From ClearPlayerSelectionState to prevent cascade **FIXED**

**✅ PHASE 4 COMPLETED:** Full bidirectional integration with clean, non-duplicate input processing.

---

## Phase 5: Visual Feedback System
**Goal**: Implement config-driven visual feedback for selection states

### [x] 5.1 Enhanced Border Styling ✅ **COMPLETED**
- [x] Apply default, selected, and hover border colors from config **IMPLEMENTED**
- [x] Use StyleBoxFlat for dynamic border styling **IMPLEMENTED**
- [x] Ensure border width matches config specifications **IMPLEMENTED**
- [x] Add subtle corner rounding for modern appearance **ADDED**
- [x] Apply styling to all button states (normal, pressed, hover, focus) **IMPLEMENTED**
- [x] Enhanced hover effects with subtle background highlights **ADDED**

### [x] 5.2 Hotkey Number Display ✅ **COMPLETED**
- [x] Add hotkey number labels overlaid on tower buttons **IMPLEMENTED**
- [x] Position labels in bottom-right corner of each square **POSITIONED**
- [x] Style labels with config-driven colors and font sizes **STYLED**
- [x] Add text shadows for better readability **ADDED**
- [x] Set mouse filter to ignore for proper button click handling **CONFIGURED**

### [x] 5.3 Tower Information Display ✅ **COMPLETED**
- [x] Show tower cost, damage, range, and attack speed on hover **IMPLEMENTED**
- [x] Display tower name and basic stats via existing HUD stats panel **INTEGRATED**
- [x] Integrate with existing StatsManagerService for accurate data **INTEGRATED**
- [x] Use HudManager.ShowTowerStats() for consistent display **IMPLEMENTED**
- [x] Hide tower info when mouse exits button **IMPLEMENTED**
- [x] Robust error handling for stats retrieval **ADDED**

**✅ PHASE 5 COMPLETED:** Enhanced visual feedback with config-driven styling, hotkey displays, and integrated tower information system.

---

## Phase 6: HUD Integration
**Goal**: Integrate the tower selection HUD into the main game scene

### [x] 6.1 Add to Main Scene ✅ **COMPLETED**
- [x] Integrate TowerSelectionHud into `scenes/UI/Hud.tscn` **INTEGRATED**
- [x] Ensure proper layering (above game, below UI panels) **POSITIONED**
- [x] Test positioning with existing HUD elements **VERIFIED**
- [x] Maintain existing HUD functionality **MAINTAINED**

### [x] 6.2 HUD Manager Integration ✅ **COMPLETED**
- [x] TowerSelectionHud uses existing HudManager.ShowTowerStats() **INTEGRATED**
- [x] No modifications needed - leveraged existing singleton pattern **OPTIMIZED**
- [x] Seamless integration with existing HUD state management **IMPLEMENTED**
- [x] Compatible with existing HUD architecture **VERIFIED**

### [x] 6.3 Service Integration ✅ **COMPLETED**
- [x] Register TowerSelectionHudConfigService in DI container **REGISTERED**
- [x] Proper service lifecycle management **IMPLEMENTED**
- [x] Comprehensive error handling for configuration loading **ADDED**
- [x] Integration with existing service patterns **FOLLOWED**

**✅ PHASE 6 COMPLETED:** TowerSelectionHud fully integrated into main game scene with proper HUD and service integration.

---

## Phase 7: Testing and Polish
**Goal**: Comprehensive testing and final polish

### [ ] 7.1 Functionality Testing
- [ ] Test keyboard selection (1-4) updates HUD correctly
- [ ] Test mouse selection on HUD updates tower selection
- [ ] Test with different tower counts (1, 2, 3, 4 towers)
- [ ] Test configuration changes take effect
- [ ] Test missing icons fallback to default

### [ ] 7.2 Visual Testing
- [ ] Test all border color states (default, selected, hover)
- [ ] Test positioning with different screen resolutions
- [ ] Test HUD centering with 1-4 towers
- [ ] Verify no overlap with existing UI elements
- [ ] Test tower icon loading and display

### [ ] 7.3 Integration Testing
- [ ] Test with existing building placement system
- [ ] Test tower selection persists during build mode
- [ ] Test compatibility with existing Player movement
- [ ] Test HUD shows correct tower information
- [ ] Verify no performance regressions

### [ ] 7.4 Configuration Testing
- [ ] Test configuration file validation
- [ ] Test configuration error handling
- [ ] Test dynamic tower list loading
- [ ] Test styling changes via config modifications

---

## Success Criteria

### Core Functionality:
- [ ] HUD displays 4 tower types with distinct icons
- [ ] Keyboard shortcuts (1-4) update HUD selection visually
- [ ] Mouse clicks on HUD squares select towers
- [ ] Selected tower has distinct border color/style
- [ ] HUD is centered regardless of tower count

### Visual Design:
- [ ] 48x48 (configurable) tower squares with proper spacing
- [ ] Numbers 1-4 display in bottom-right corner of each square
- [ ] Hover effects work smoothly with config-driven colors
- [ ] Icons load correctly with fallback system
- [ ] HUD positioned properly at bottom-center of screen

### Configuration Integration:
- [ ] All styling controlled via `data/huds/tower_selection_hud.json`
- [ ] Tower display order and icons configurable per tower type
- [ ] Layout settings (size, spacing, margins) work as configured
- [ ] Configuration service follows existing patterns in codebase

### Code Quality:
- [ ] No hardcoded values - all styling from config
- [ ] Clean, expressive code following user rules
- [ ] PascalCase naming convention throughout
- [ ] Integration with existing service architecture
- [ ] Proper error handling and logging

---

## Technical Integration Points

### Existing Systems to Connect:
1. **StatsService**: Tower stat information for display
2. **Player.cs**: Existing tower selection logic (keyboard 1-4)
3. **PlayerBuildingBuilder**: Building placement system
4. **HudManager**: Centralized HUD state management
5. **Main.cs**: Service initialization and DI container

### Configuration Pattern:
Follow the established pattern from `StatsService.cs`:
- JSON config files in `data/` directory
- Service classes in `Infrastructure/` layer
- Interface contracts in `Application/` layer
- Config model classes in `Application/` layer
- Godot file loading with error handling

### Performance Considerations:
- Load configuration once at startup
- Cache icon textures to avoid repeated loading
- Minimal UI updates on selection changes
- Efficient positioning calculations

---

## File Structure After Implementation:

```
data/huds/
├── tower_selection_hud.json

assets/sprites/ui/
├── basic_tower_icon.png
├── sniper_tower_icon.png
├── rapid_tower_icon.png
├── heavy_tower_icon.png
└── default_tower_icon.png

scenes/UI/
├── Hud.tscn (updated)
└── TowerSelectionHud.tscn (new)

src/Application/UI/
├── Configuration/
│   └── TowerSelectionHudConfig.cs
└── Services/
    └── ITowerSelectionHudConfigService.cs

src/Infrastructure/UI/
└── Services/
    └── TowerSelectionHudConfigService.cs

src/Presentation/UI/
├── Hud.cs (updated)
├── HudManager.cs (updated)
└── TowerSelectionHud.cs (new)

src/Presentation/Player/
└── Player.cs (updated)

src/Presentation/Core/
└── Main.cs (updated)
```

---

## Estimated Timeline:
- **Phase 1**: Configuration System - 2 hours
- **Phase 2**: Tower Icon Assets - 1 hour  
- **Phase 3**: Core HUD Component - 3 hours
- **Phase 4**: Selection Logic Integration - 2 hours
- **Phase 5**: Visual Feedback System - 2 hours
- **Phase 6**: HUD Integration - 2 hours
- **Phase 7**: Testing and Polish - 2 hours

**Total Estimated Time**: 14 hours

This plan creates a professional, config-driven tower selection HUD that seamlessly integrates with the existing codebase architecture while providing an intuitive user interface for tower selection.
