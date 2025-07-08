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

### [ ] 1.1 Create HUD Configuration Schema
- [ ] Create `data/huds/tower_selection_hud.json` with structure:
```json
{
  "layout": {
    "max_towers": 9,
    "square_size": 48,
    "spacing_between_squares": 8,
    "bottom_margin": 20,
    "border_width": 2
  },
  "styling": {
    "default_border_color": "#444444",
    "selected_border_color": "#00FF00",
    "hover_border_color": "#FFFF00",
    "background_color": "#000000AA",
    "number_text_color": "#FFFFFF",
    "number_font_size": 14
  },
  "towers": {
    "basic_tower": {
      "display_order": 1,
      "icon_path": "res://assets/sprites/basic_tower_icon.png",
      "hotkey": "1"
    },
    "sniper_tower": {
      "display_order": 2, 
      "icon_path": "res://assets/sprites/sniper_tower_icon.png",
      "hotkey": "2"
    },
    "rapid_tower": {
      "display_order": 3,
      "icon_path": "res://assets/sprites/rapid_tower_icon.png", 
      "hotkey": "3"
    },
    "heavy_tower": {
      "display_order": 4,
      "icon_path": "res://assets/sprites/heavy_tower_icon.png",
      "hotkey": "4"
    }
  }
}
```

### [ ] 1.2 Create Configuration Models
- [ ] Create `src/Application/UI/Configuration/TowerSelectionHudConfig.cs`:
  - TowerSelectionHudConfig class
  - HudLayout class with positioning/sizing properties
  - HudStyling class with colors and fonts
  - TowerDisplayConfig class with icon and hotkey info
- [ ] Follow existing pattern in `src/Infrastructure/Stats/StatsService.cs`
- [ ] Use PascalCase naming convention following user rules

### [ ] 1.3 Create HUD Configuration Service
- [ ] Create `src/Infrastructure/UI/Services/TowerSelectionHudConfigService.cs`
- [ ] Implement JSON loading following `StatsService` pattern
- [ ] Add Godot file access with error handling
- [ ] Create interface `src/Application/UI/Services/ITowerSelectionHudConfigService.cs`
- [ ] Follow clean code principles with no unnecessary comments

---

## Phase 2: Tower Icon Assets
**Goal**: Create or prepare tower icon assets for the HUD display

### [ ] 2.1 Create Tower Icon Assets
- [ ] Create 64x64 tower icons in `assets/sprites/ui/`:
  - `basic_tower_icon.png` - Simple balanced tower design
  - `sniper_tower_icon.png` - Long-range tower with scope/rifle elements
  - `rapid_tower_icon.png` - Multi-barrel rapid-fire design
  - `heavy_tower_icon.png` - Large, heavy cannon design
- [ ] Use consistent visual style matching game aesthetic
- [ ] Ensure icons are distinct and recognizable at small sizes

### [ ] 2.2 Fallback Icon System
- [ ] Create default tower icon `assets/sprites/ui/default_tower_icon.png`
- [ ] Add icon validation in configuration service
- [ ] Implement fallback to default icon if specific icon missing
- [ ] Log warnings for missing tower icons

---

## Phase 3: Core HUD Component
**Goal**: Create the tower selection HUD component with proper layout

### [ ] 3.1 Create TowerSelectionHud Scene
- [ ] Create `scenes/UI/TowerSelectionHud.tscn`:
  - Root: Control node (anchored to bottom center)
  - HBoxContainer for tower squares layout
  - Panel nodes for each tower slot with proper styling
  - TextureRect for tower icons
  - Label for hotkey numbers (bottom-right corner)

### [ ] 3.2 Create TowerSelectionHud Script
- [ ] Create `src/Presentation/UI/TowerSelectionHud.cs`:
  - Inherit from Control
  - Load configuration from TowerSelectionHudConfigService
  - Dynamic tower slot creation based on config
  - Tower selection state management
  - Visual feedback for selection/hover states
  - Follow existing HUD patterns from `Hud.cs`

### [ ] 3.3 Implement Layout System
- [ ] Calculate positioning based on number of towers and config settings
- [ ] Center HUD horizontally regardless of tower count (1-9)
- [ ] Implement responsive spacing: 1 tower = center, 2+ = evenly spaced
- [ ] Apply config-driven sizing (32x32 to 64x64 based on config)
- [ ] Add proper anchoring for different screen sizes

---

## Phase 4: Selection Logic Integration
**Goal**: Connect the HUD to existing tower selection and building systems

### [ ] 4.1 Integrate with Player Tower Selection
- [ ] Modify `src/Presentation/Player/Player.cs`:
  - Add TowerSelectionHud reference
  - Update existing hotkey handlers (1-4) to notify HUD
  - Maintain existing tower selection functionality
  - Add visual feedback when tower type changes

### [ ] 4.2 Add Mouse Selection Support
- [ ] Implement click handlers on tower slots in TowerSelectionHud
- [ ] Add hover effects with config-driven styling
- [ ] Connect mouse clicks to existing tower selection logic
- [ ] Ensure mouse and keyboard selection stay synchronized

### [ ] 4.3 Selection State Management
- [ ] Create selection state tracking in TowerSelectionHud
- [ ] Apply visual styling changes (border colors) on selection
- [ ] Reset selection visual states appropriately
- [ ] Handle edge cases (invalid tower types, disabled towers)

---

## Phase 5: Visual Feedback System
**Goal**: Implement config-driven visual feedback for selection states

### [ ] 5.1 Implement Border Styling
- [ ] Apply default, selected, and hover border colors from config
- [ ] Use StyleBoxFlat for dynamic border styling
- [ ] Ensure border width matches config specifications
- [ ] Handle border color transitions smoothly

### [ ] 5.2 Add Selection Animations
- [ ] Implement subtle selection animation (scale/glow effect)
- [ ] Add hover animation feedback
- [ ] Keep animations minimal and performance-friendly
- [ ] Make animation timing configurable if needed

### [ ] 5.3 Tower Information Display
- [ ] Show tower cost on hover/selection
- [ ] Display tower name and basic stats
- [ ] Integrate with existing building stats from StatsService
- [ ] Position information display appropriately (tooltip-style)

---

## Phase 6: HUD Integration
**Goal**: Integrate the tower selection HUD into the main game scene

### [ ] 6.1 Add to Main Scene
- [ ] Integrate TowerSelectionHud into `scenes/UI/Hud.tscn`
- [ ] Ensure proper layering (above game, below UI panels)
- [ ] Test positioning with existing HUD elements
- [ ] Maintain existing HUD functionality

### [ ] 6.2 Update HUD Manager
- [ ] Extend `src/Presentation/UI/HudManager.cs`:
  - Add TowerSelectionHud reference
  - Add methods for tower selection updates
  - Integrate with existing HUD state management
  - Follow singleton pattern for access

### [ ] 6.3 Service Integration
- [ ] Register TowerSelectionHudConfigService in DI container
- [ ] Update `src/Presentation/Core/Main.cs` initialization
- [ ] Ensure proper service lifecycle management
- [ ] Add error handling for configuration loading

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
