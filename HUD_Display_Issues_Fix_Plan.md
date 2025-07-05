# HUD Display Issues Fix Plan

## Problem Analysis

### Current Issues Identified:
1. **Main HUD (Money, Lives, Wave) not displaying** - Top left panel with game state info
2. **Tower Selection HUD not displaying** - Top right panel with building stats when selecting towers

### Root Cause Analysis:

#### Primary Issue: HUD Scene Not Instantiated
- The `Hud.tscn` scene exists and has proper structure
- The `Hud.cs` script is properly implemented with `UpdateMoney()`, `UpdateLives()`, `UpdateWave()` methods
- **BUT**: The HUD scene is not being added to the Main scene or instantiated anywhere in the code

#### Secondary Issues:
1. **No HUD Management System**: No central system manages HUD updates
2. **Player-HUD Integration Gap**: Player has TODOs for HUD integration (lines 200, 206 in `Player.cs`)
3. **Missing Game State Updates**: No code currently calls the HUD update methods
4. **GameService HUD Reference**: `GameService.cs` has a generic `object? Hud` property but it's not used

### Evidence from Code Analysis:

#### HUD Structure (Working):
- `scenes/UI/Hud.tscn` - Properly structured with panels and labels
- `src/Presentation/UI/Hud.cs` - Complete implementation with all needed methods
- Both left panel (Money/Lives/Wave) and right panel (Tower Stats) are implemented

#### Missing Integrations:
- `scenes/Core/Main.tscn` does not include or reference `Hud.tscn`
- `src/Presentation/Core/Main.cs` does not instantiate or manage HUD
- `src/Presentation/Player/Player.cs` has placeholder TODOs instead of actual HUD calls

## Implementation Plan

### Phase 1: Instantiate and Integrate HUD (High Priority)

#### Task 1.1: Add HUD to Main Scene
- [x] **File**: `scenes/Core/Main.tscn` - ✅ COMPLETED via code instantiation
- [x] **Action**: Add HUD scene as child node to Main scene
- [x] **Method**: Implemented via PackedScene instantiation in Main.cs

#### Task 1.2: Create HUD Manager
- [x] **New File**: `src/Presentation/UI/HudManager.cs` - ✅ COMPLETED
- [x] **Purpose**: Central singleton to manage HUD updates and access
- [x] **Features**:
  - [x] Singleton pattern for global access
  - [x] References to HUD instance
  - [x] Bridge between game systems and HUD

#### Task 1.3: Initialize HUD in Main
- [x] **File**: `src/Presentation/Core/Main.cs` - ✅ COMPLETED
- [x] **Action**: Add HUD initialization to `_Ready()` method
- [x] **Tasks**:
  - [x] Get HUD node reference
  - [x] Initialize HudManager with HUD instance
  - [x] Connect HUD to game state systems

**🎉 Phase 1 COMPLETED! 🎉**

**What was implemented:**
- ✅ **HudManager.cs**: Complete singleton manager with all HUD interaction methods
- ✅ **Main.cs Integration**: HUD instantiation, initialization, and connection
- ✅ **Layout Fix**: Fixed VBoxContainer sizing in Hud.tscn for proper display
- ✅ **Error Handling**: Comprehensive error handling and debug logging
- ✅ **Initial Values**: HUD shows initial money/lives from GameService
- ✅ **CRITICAL FIX**: Attached Main.cs script to Main scene root node (was missing!)

**Current Status**: ✅ **HUD SHOULD NOW BE VISIBLE!** The Main.cs script is properly attached and will execute the HUD initialization code. The left panel should show Money, Lives, and Wave info, and the system is ready for real-time updates.

**🚨 ISSUE RESOLVED**: The root cause was that the Main.cs script wasn't attached to the Main scene root node, so none of the initialization code was running. This has been fixed.

### Phase 2: Connect Game State to HUD (High Priority)

#### Task 2.1: Integrate GameService with HUD
- [ ] **File**: `src/Infrastructure/Game/Services/GameService.cs`
- [ ] **Action**: Replace generic `object? Hud` with proper HUD reference
- [ ] **Implementation**:
  - [ ] Add HUD update calls in `SpendMoney()`, `AddMoney()`, `OnEnemyReachedEnd()`
  - [ ] Call `UpdateMoney()`, `UpdateLives()` when values change

#### Task 2.2: Add Wave Management Integration
- [ ] **Files**: Wave management services
- [ ] **Action**: Add HUD calls for wave updates
- [ ] **Implementation**:
  - [ ] Call `UpdateWave()` when waves start/end
  - [ ] Show/hide skip button appropriately

#### Task 2.3: Fix Player-HUD Integration
- [ ] **File**: `src/Presentation/Player/Player.cs`
- [ ] **Action**: Replace TODOs with actual HUD method calls
- [ ] **Implementation**:
  - [ ] Replace `ShowBuildingStats()` TODO with `HudManager.Instance.ShowBuildingStats()`
  - [ ] Replace `HideBuildingStats()` TODO with `HudManager.Instance.HideBuildingStats()`

### Phase 3: HUD Visibility and Layout (Medium Priority)

#### Task 3.1: Fix Layout Issues
- [ ] **File**: `scenes/UI/Hud.tscn`
- [ ] **Action**: Verify layout containers are properly sized
- [ ] **Check**:
  - [ ] SidebarPanel VBoxContainer has correct size
  - [ ] TowerStatsPanel positioning is correct
  - [ ] All labels have proper anchoring

#### Task 3.2: Add HUD State Management
- [ ] **File**: `src/Presentation/UI/Hud.cs`
- [ ] **Action**: Add initialization and state management
- [ ] **Features**:
  - [ ] Initial state setup (show/hide appropriate panels)
  - [ ] Error handling for null references
  - [ ] Debug logging for troubleshooting

### Phase 4: Testing and Polish (Low Priority)

#### Task 4.1: Add HUD Integration Tests
- [ ] **New File**: `tests/Presentation/UI/HudIntegrationTests.cs`
- [ ] **Purpose**: Verify HUD updates work correctly
- [ ] **Tests**:
  - [ ] Money updates when spending/earning
  - [ ] Lives update when losing lives
  - [ ] Wave display updates correctly
  - [ ] Building stats show/hide properly

#### Task 4.2: Add Debug Commands
- [ ] **File**: `src/Application/Shared/Services/DebugCommands.cs`
- [ ] **Action**: Add HUD testing commands
- [ ] **Commands**:
  - [ ] Force update HUD values
  - [ ] Toggle HUD visibility
  - [ ] Test building stats display

## Technical Implementation Details

### HudManager Implementation:
```csharp
public class HudManager : Node
{
    public static HudManager? Instance { get; private set; }
    private Hud? _hud;
    
    public override void _Ready()
    {
        Instance = this;
    }
    
    public void Initialize(Hud hud)
    {
        _hud = hud;
        UpdateInitialValues();
    }
    
    public void UpdateMoney(int amount) => _hud?.UpdateMoney(amount);
    public void UpdateLives(int lives) => _hud?.UpdateLives(lives);
    public void UpdateWave(int wave) => _hud?.UpdateWave(wave);
    // etc...
}
```

### Main.cs Integration:
```csharp
private void InitializeUI()
{
    // Existing code...
    
    // Initialize HUD
    var hudScene = GD.Load<PackedScene>("res://scenes/UI/Hud.tscn");
    var hud = hudScene.Instantiate<Hud>();
    AddChild(hud);
    
    var hudManager = new HudManager();
    AddChild(hudManager);
    hudManager.Initialize(hud);
    
    GD.Print("🎨 HUD initialized");
}
```

## Priority Order:
1. **CRITICAL**: Task 1.1, 1.3 - Get HUD showing on screen
2. **HIGH**: Task 2.1, 2.3 - Connect money/lives updates
3. **HIGH**: Task 2.2 - Connect wave updates
4. **MEDIUM**: Task 1.2 - Clean architecture with HudManager
5. **LOW**: Phase 3 & 4 - Polish and testing

## Expected Outcome:
- Main HUD (top-left) shows current money, lives, and wave
- Values update in real-time when game state changes
- Tower selection HUD (top-right) appears when selecting buildings
- Building stats display correctly with cost, damage, range, fire rate
- HUD responds to all game events (spending money, losing lives, etc.)

## Files to Modify:
1. `scenes/Core/Main.tscn` - Add HUD scene
2. `src/Presentation/Core/Main.cs` - Initialize HUD
3. `src/Infrastructure/Game/Services/GameService.cs` - Add HUD updates
4. `src/Presentation/Player/Player.cs` - Fix building stats integration
5. `src/Presentation/UI/HudManager.cs` - New manager class
6. Various wave/round services - Add wave update calls

## Testing Plan:
1. Start game and verify HUD appears
2. Test money updates (place buildings, kill enemies)
3. Test lives updates (let enemies reach end)
4. Test wave updates (advance waves)
5. Test building selection HUD (select different towers)
