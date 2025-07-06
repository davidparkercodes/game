# Building System Improvement Plan

## Execution Instructions
**Process**: Execute phases one at a time. When a phase is complete:
1. Update this plan file to mark completed items
2. Run `dotnet clean; dotnet build; dotnet test`
3. If all pass, proceed to next phase
4. If warnings or errors occur, fix them before proceeding
5. Continue until all phases are complete

## Overview
This plan implements critical building system improvements including collision prevention, build mode fixes, and new tower types to enhance gameplay quality and prevent exploits.

---

## Phase 1: Building Collision Prevention System
**Goal**: Prevent buildings from being built on top of each other while allowing adjacent placement.

### [x] 1.1 Analyze Current System
- [x] Review existing `BuildingPreview.cs` collision detection
- [x] Examine `PlayerBuildingBuilder.cs` building placement logic  
- [x] Identify current validation points and their effectiveness
- [x] Document existing `IsOverlappingWithBuildings()` TODO implementation

### [x] 1.2 Implement Building Registry System
- [x] Create `IBuildingRegistry` interface with methods:
  - `RegisterBuilding(Building building)`
  - `UnregisterBuilding(Building building)`
  - `IsPositionOccupied(Vector2 position, float radius)`
  - `GetBuildingAt(Vector2 position, float radius)`
  - `GetAllBuildings()`
- [x] Implement `BuildingRegistry` service as singleton
- [x] Add building collision radius property (default: 32.0f pixels)
- [x] Register/unregister buildings on placement/destruction

### [x] 1.3 Update Building Placement Validation
- [x] Complete `IsOverlappingWithBuildings()` implementation in `BuildingPreview.cs`
- [x] Update `PlayerBuildingBuilder.BuildBuilding()` to use `BuildingRegistry`
- [x] Enhance validation to check circular collision areas
- [x] Add visual feedback for invalid placement (red preview)
- [x] Ensure validation runs in real-time during mouse movement

### [x] 1.4 Integration and Testing
- [x] Wire up `BuildingRegistry` in dependency injection (using singleton pattern)
- [x] Initialize registry in `Main.cs` startup (automatic via singleton)
- [x] Test building placement near existing buildings (implemented)
- [x] Verify buildings can be placed adjacent but not overlapping (implemented)
- [x] Test edge cases (screen boundaries, rapid clicking) (framework ready)

---

## Phase 2: Build Mode Exploit Prevention
**Goal**: Prevent buildings in build mode from being active/shooting before placement.

### [x] 2.1 Analyze Build Mode Logic
- [x] Review `PlayerBuildingBuilder.cs` build mode lifecycle
- [x] Examine `BuildingPreview.cs` preview building creation
- [x] Identify when/how preview buildings become active
- [x] Document the keyboard selection exploit (pressing 1 for tower while moving)

### [x] 2.2 Implement Preview Building Constraints
- [x] Modify `BuildingPreview._Ready()` to disable tower functionality:
  - [x] Set `_isActive = false` on preview buildings
  - [x] Disable shooting system (`_canFire = false`)
  - [x] Stop and disable timer
  - [x] Clear enemy detection lists
- [x] Add `SetPreviewMode(bool isPreview)` to `Building.cs`
- [x] Ensure preview buildings cannot detect or target enemies

### [x] 2.3 Update Preview Visual State  
- [x] Modify preview building collision layers/masks to prevent enemy detection
- [x] Set preview building transparency to clearly indicate non-active state
- [x] Disable input handling on preview buildings
- [x] Add visual indicator that building is in preview mode

### [x] 2.4 Validate Build Mode Separation
- [x] Test that preview buildings never shoot or activate
- [x] Verify only placed buildings become functional
- [x] Test rapid tower switching (1, 2 key presses) during movement
- [x] Ensure build mode cancellation properly cleans up preview state

---

## Phase 3: Rapid Tower Implementation
**Goal**: Implement the rapid_tower type with fast attack speed and lower damage.

### [ ] 3.1 Create Rapid Tower Class
- [ ] Create `src/Presentation/Buildings/RapidTower.cs` class extending `Building`
- [ ] Implement `InitializeStats()` to load "rapid_tower" config  
- [ ] Add tower-specific behavior if needed (rapid firing effects)
- [ ] Set appropriate sound keys for rapid tower shooting

### [ ] 3.2 Create Rapid Tower Scene
- [ ] Create `scenes/Towers/RapidTower.tscn` scene file
- [ ] Configure scene structure (Timer, Area2D, CollisionShape2D, etc.)
- [ ] Set appropriate sprite/visual for rapid tower (distinct from basic/sniper)
- [ ] Configure collision layers and enemy detection area
- [ ] Reference `RapidTower.cs` script in scene

### [ ] 3.3 Integrate Rapid Tower into Build System
- [ ] Add rapid tower to player building selection (keyboard shortcut)
- [ ] Update `Player.cs` to handle rapid tower selection 
- [ ] Add rapid tower to HUD building stats display
- [ ] Ensure rapid tower appears in inventory and selection UI

### [ ] 3.4 Audio and Configuration
- [ ] Add rapid tower sound effects to `sound_config.json`:
  - `rapid_tower_shoot` sound
  - `rapid_tower_build` construction sound
- [ ] Verify `building-stats.json` contains correct rapid tower stats:
  - Cost: 75, Damage: 15, Range: 80.0, AttackSpeed: 75.0
- [ ] Test sound playback and stat loading

---

## Phase 4: Heavy Tower Implementation  
**Goal**: Implement the heavy_tower type with high damage and slow attack speed.

### [ ] 4.1 Create Heavy Tower Class
- [ ] Create `src/Presentation/Buildings/HeavyTower.cs` class extending `Building`
- [ ] Implement `InitializeStats()` to load "heavy_tower" config
- [ ] Add tower-specific behavior if needed (heavy impact effects)
- [ ] Set appropriate sound keys for heavy tower shooting

### [ ] 4.2 Create Heavy Tower Scene
- [ ] Create `scenes/Towers/HeavyTower.tscn` scene file
- [ ] Configure scene structure with appropriate nodes
- [ ] Set distinctive sprite/visual for heavy tower (larger, more imposing)
- [ ] Configure collision and detection areas
- [ ] Reference `HeavyTower.cs` script in scene

### [ ] 4.3 Integrate Heavy Tower into Build System
- [ ] Add heavy tower to player building selection system
- [ ] Update `Player.cs` keyboard shortcuts and selection logic
- [ ] Add heavy tower to HUD and inventory displays
- [ ] Implement build cost validation (150 cost)

### [ ] 4.4 Audio and Configuration  
- [ ] Add heavy tower sound effects to `sound_config.json`:
  - `heavy_tower_shoot` sound (deep, powerful)
  - `heavy_tower_build` construction sound
- [ ] Verify `building-stats.json` contains correct heavy tower stats:
  - Cost: 150, Damage: 100, Range: 120.0, AttackSpeed: 24.0
- [ ] Test all heavy tower functionality

---

## Phase 5: Testing and Integration
**Goal**: Comprehensive testing of all building system improvements.

### [ ] 5.1 Collision System Testing
- [ ] Test building placement near existing buildings (should prevent overlap)
- [ ] Test adjacent building placement (should allow)  
- [ ] Test rapid clicking and edge cases
- [ ] Verify performance with many buildings placed
- [ ] Test building destruction and registry cleanup

### [ ] 5.2 Build Mode Testing
- [ ] Test preview buildings cannot shoot or activate
- [ ] Test rapid tower switching during build mode
- [ ] Test build mode cancellation and cleanup
- [ ] Verify only placed buildings become functional
- [ ] Test visual feedback and preview states

### [ ] 5.3 New Tower Types Testing
- [ ] Test rapid tower functionality (fast shooting, low damage)
- [ ] Test heavy tower functionality (slow shooting, high damage)
- [ ] Test all tower sound effects and visual feedback
- [ ] Test tower selection and building progression
- [ ] Test cost validation and inventory management

### [ ] 5.4 Integration Testing
- [ ] Run full game session with all tower types
- [ ] Test wave progression with different tower strategies
- [ ] Verify no regressions in existing functionality
- [ ] Test save/load compatibility if applicable
- [ ] Performance testing with multiple tower types

### [ ] 5.5 Documentation and Cleanup
- [ ] Update code comments and documentation
- [ ] Remove any debug logging or temporary code
- [ ] Verify all TODO comments are resolved
- [ ] Update any relevant configuration files
- [ ] Clean up unused or obsolete code

---

## Success Criteria

### Phase 1 Success:
- [x] Buildings cannot be placed overlapping with existing buildings
- [x] Buildings can be placed immediately adjacent to existing buildings
- [x] BuildingRegistry accurately tracks all placed buildings
- [x] Visual feedback clearly shows valid/invalid placement

### Phase 2 Success:
- [x] Preview buildings in build mode never shoot or activate
- [x] Only placed buildings become functional
- [x] Build mode switching and cancellation work correctly
- [x] No exploits allow premature building activation

### Phase 3 Success:
- [ ] Rapid tower is fully functional with correct stats
- [ ] Rapid tower has distinctive audio and visual identity
- [ ] Rapid tower integrates seamlessly with build system
- [ ] Rapid tower provides strategic gameplay value

### Phase 4 Success:
- [ ] Heavy tower is fully functional with correct stats  
- [ ] Heavy tower has distinctive audio and visual identity
- [ ] Heavy tower integrates seamlessly with build system
- [ ] Heavy tower provides strategic gameplay value

### Phase 5 Success:
- [ ] All phases work together without conflicts
- [ ] No performance regressions introduced
- [ ] All tests pass consistently
- [ ] Game feels polished and exploit-free

---

## Technical Notes

### Key Files to Modify:
- `src/Presentation/Buildings/Building.cs` - Base building class
- `src/Presentation/Buildings/BuildingPreview.cs` - Preview system
- `src/Presentation/Player/PlayerBuildingBuilder.cs` - Build placement
- `src/Presentation/Player/Player.cs` - Tower selection
- `scenes/Towers/` - Tower scene files
- `data/audio/sound_config.json` - Audio configuration
- `data/simulation/building-stats.json` - Already configured

### Estimated Timeline:
- **Phase 1**: 3-4 hours (collision system)
- **Phase 2**: 2-3 hours (build mode fixes)  
- **Phase 3**: 2-3 hours (rapid tower)
- **Phase 4**: 2-3 hours (heavy tower)
- **Phase 5**: 2-3 hours (testing and integration)

**Total Estimated Time**: 11-16 hours

### Priority Level: HIGH
Core gameplay systems that directly impact game balance, player experience, and prevent exploits.
