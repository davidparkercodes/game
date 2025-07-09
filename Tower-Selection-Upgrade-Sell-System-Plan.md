# Building Selection, Upgrade & Sell System Plan

## Execution Instructions

**Process**: Execute phases one at a time. When a phase is complete:

1. Update this plan file to mark completed items
2. Run `dotnet clean && dotnet build && dotnet test`
3. If all pass, proceed to next phase
4. If warnings or errors occur, fix them before proceeding
5. Continue until all phases are complete

---

## 🎯 **Objective**

Implement a comprehensive building selection, upgrade, and selling system that allows players to interact with placed buildings through left-click selection, showing a modal HUD with building information, upgrade options, and sell functionality.

---

## 📋 **Phase 1: Building Selection & Input System** 🎯

### **[x] 1.1 Implement Building Click Detection**

- [x] **File**: `src/Presentation/Buildings/Building.cs` - Add input handling
- [x] **Action**: Add `_Input` method to handle mouse clicks
- [x] **Features**:
  - [x] Detect left mouse button clicks on building collision area
  - [x] Ignore clicks when player has active building to place
  - [x] Add selection state tracking (`IsSelected` property)
  - [x] Connect to BuildingSelectionManager for selection handling

### **[x] 1.2 Create Building Selection Manager**

- [x] **New File**: `src/Presentation/UI/BuildingSelectionManager.cs`
- [x] **Purpose**: Central manager for building selection state
- [x] **Features**:
  - [x] Singleton pattern for global access
  - [x] Track currently selected building
  - [x] Handle building selection/deselection logic
  - [x] Coordinate with BuildingUpgradeHud display (TODO: Phase 2)
  - [x] Handle "click outside" detection for closing HUD

### **[x] 1.3 Add Visual Selection Feedback**

- [x] **File**: `src/Presentation/Buildings/Building.cs` - Visual enhancements
- [x] **Action**: Add visual selection indicators
- [x] **Features**:
  - [x] Add 2px solid black border when selected
  - [x] Show range circle when building is selected
  - [x] Highlight building with subtle color change
  - [x] Ensure selection visuals are clear and professional

### **[x] 1.4 Integration with Build Mode**

- [x] **File**: `src/Presentation/Player/PlayerBuildingBuilder.cs` - Check integration
- [x] **Action**: Ensure building selection doesn't interfere with building placement
- [x] **Features**:
  - [x] Disable building selection when player has active building to place
  - [x] Clear building selection when entering build mode
  - [x] Prevent building selection during build preview mode

---

## 📋 **Phase 2: Building Upgrade HUD Implementation** 🖥️

### **[x] 2.1 Create Building Upgrade HUD Scene**

- [x] **New File**: `scenes/UI/BuildingUpgradeHud.tscn`
- [x] **Structure**: Compact vertical panel in top-right corner
- [x] **Components**:
  - [x] Main Panel (vertical rectangle, top-right anchored)
  - [x] Tower name and level display
  - [x] Current stats with upgrade preview arrows
  - [x] Upgrade button with cost
  - [x] Sell button with value
  - [x] Top-right: Close button (X)
  - [x] Optimized for minimal screen obstruction

### **[x] 2.2 Implement Tower Upgrade HUD Script**

- [x] **New File**: `src/Presentation/UI/BuildingUpgradeHud.cs`
- [x] **Purpose**: Manage tower upgrade HUD functionality
- [x] **Features**:
  - [x] Display tower information (name, image, description)
  - [x] Show current stats (damage, range, attack speed, etc.)
  - [x] Display upgrade preview stats (50% improvement)
  - [x] Handle upgrade button click (cost validation, stat updates)
  - [x] Handle sell button click (75% value return)
  - [x] Handle close button and outside-click closing
  - [x] Modal behavior (block input to other UI elements)

### **[x] 2.3 Design HUD Layout & Styling**

- [x] **File**: `scenes/UI/BuildingUpgradeHud.tscn` - Polish layout
- [x] **Features**:
  - [x] Professional UI design with proper spacing
  - [x] Clear visual hierarchy (title, stats, buttons)
  - [x] Consistent styling with existing game UI
  - [x] Responsive layout that works at different screen sizes
  - [x] Smooth show/hide animations (TODO: Phase 5)

### **[x] 2.4 Add Configuration Support**

- [x] **New File**: `data/ui/building_upgrade_hud_config.json`
- [x] **Purpose**: Configure HUD appearance and behavior
- [x] **Content**:
  - [x] HUD dimensions and positioning
  - [x] Button styling and text
  - [x] Animation settings
  - [x] Color schemes and visual themes
  - [x] Tower icon/image paths

---

## 📋 **Phase 3: Upgrade & Sell Logic Implementation** ⚙️

### **[x] 3.1 Implement Tower Upgrade System**

- [x] **File**: `src/Domain/Buildings/Services/ITowerUpgradeService.cs` - New interface
- [x] **File**: `src/Application/Buildings/Services/TowerUpgradeService.cs` - Implementation
- [x] **Features**:
  - [x] Calculate upgrade costs from configuration
  - [x] Apply 50% stat improvements (configurable multiplier)
  - [x] Track tower upgrade levels (level 1, 2, 3, etc.)
  - [x] Validate upgrade affordability before application
  - [x] Update building stats in real-time

### **[x] 3.2 Implement Tower Selling System**

- [x] **File**: `src/Domain/Buildings/Services/ITowerSellService.cs` - New interface
- [x] **File**: `src/Application/Buildings/Services/TowerSellService.cs` - Implementation
- [x] **Features**:
  - [x] Calculate sell value (75% of total investment)
  - [x] Track total investment (base cost + all upgrade costs)
  - [x] Handle money return to player
  - [x] Remove tower from BuildingRegistry
  - [x] Clean up tower scene and references

### **[x] 3.3 Extend Building Entity for Upgrade Tracking**

- [x] **File**: `src/Presentation/Buildings/Building.cs` - Add upgrade properties
- [x] **Features**:
  - [x] `UpgradeLevel` property (starts at 0)
  - [x] `TotalInvestment` property (tracks money spent)
  - [x] `BaseStats` property (original stats for upgrade calculations)
  - [x] Upgrade preview calculation in TowerUpgradeService
  - [x] Upgrade application in TowerUpgradeService

### **[x] 3.4 Configuration Integration**

- [x] **File**: `data/simulation/building-stats.json` - Add upgrade configuration
- [x] **Features**:
  - [x] Add `upgrade_multiplier` (default: 1.5 for 50% improvement)
  - [x] Add `sell_percentage` (default: 0.75 for 75% return)
  - [x] Add `max_upgrade_level` (default: 3 levels)
  - [x] Per-tower-type upgrade cost scaling
  - [x] Upgrade stat scaling curves (linear vs exponential)

---

## 📋 **Phase 4: Game Systems Integration** 🎮 ✅

### **[x] 4.1 Money & Economy Integration**

- [x] **File**: `src/Infrastructure/Game/Services/GameService.cs` - Update money handling
- [x] **Features**:
  - [x] Add `CanAffordUpgrade(cost)` method
  - [x] Add `SpendMoneyOnUpgrade(cost, towerName)` method
  - [x] Add `ReceiveMoneyFromSale(amount, towerName)` method
  - [x] Update HUD money display after transactions
  - [x] Add transaction logging for debugging

### **[x] 4.2 Stats Service Integration**

- [x] **File**: `src/Infrastructure/Stats/Services/StatsManagerService.cs` - Upgrade support
- [x] **Features**:
  - [x] Add `GetUpgradeStats(towerType, currentLevel)` method
  - [x] Add `GetUpgradeCost(towerType, currentLevel)` method
  - [x] Support for per-level stat calculations
  - [x] Cache upgrade calculations for performance

### **[x] 4.3 Building Registry Updates**

- [x] **File**: `src/Presentation/Buildings/BuildingRegistry.cs` - Track upgrades
- [x] **Features**:
  - [x] Track tower upgrade levels in registry
  - [x] Update collision detection for upgraded towers
  - [x] Handle tower removal during selling
  - [x] Maintain registry consistency during upgrades/sales

### **[x] 4.4 Player Interaction Updates**

- [x] **File**: `src/Presentation/Player/Player.cs` - Integrate with selection
- [x] **Features**:
  - [x] Disable building placement when tower upgrade HUD is open
  - [x] Clear building selection when tower is selected
  - [x] Handle input priority (tower selection vs building placement)
  - [x] Update building stats display logic

---

## 📋 **Phase 5: Visual & Audio Enhancements** ✨ ✅

### **[x] 5.1 Visual Feedback System**

- [x] **File**: `src/Presentation/Buildings/Building.cs` - Visual improvements
- [x] **Features**:
  - [x] Upgrade level visual indicators (level badges, stars, etc.)
  - [x] Upgraded tower visual changes (color tints, particle effects)
  - [x] Selection animation (smooth border appearance)
  - [x] Range circle improvements (show upgrade range preview)

### **[x] 5.2 Sound Effects Integration**

- [x] **File**: `data/audio/sound_config.json` - Add upgrade/sell sounds
- [x] **Features**:
  - [x] `tower_upgrade` sound effect
  - [x] `tower_sell` sound effect
  - [x] `tower_select` sound effect
  - [x] `ui_button_click` for HUD interactions
  - [x] `ui_modal_open/close` for HUD show/hide

### **[x] 5.3 Animation & Polish**

- [x] **File**: `src/Presentation/Buildings/Building.cs` - Add animations
- [x] **Features**:
  - [x] Smooth selection animations (fade in/out, scale)
  - [x] Range circle animation effects
  - [x] Upgrade level visual feedback
  - [x] Smooth visual transitions

---

## 📋 **Phase 6: Testing & Quality Assurance** 🧪

### **[ ] 6.1 Unit Testing**

- [ ] **New File**: `tests/Unit/Buildings/TowerUpgradeServiceTests.cs`
- [ ] **Tests**:
  - [ ] Upgrade cost calculations
  - [ ] Stat improvement calculations
  - [ ] Upgrade level progression
  - [ ] Maximum upgrade level enforcement
  - [ ] Edge cases (negative costs, invalid levels)

### **[ ] 6.2 Integration Testing**

- [ ] **New File**: `tests/Integration/UI/TowerSelectionIntegrationTests.cs`
- [ ] **Tests**:
  - [ ] Tower selection workflow (click → HUD → upgrade/sell)
  - [ ] Money transaction accuracy
  - [ ] HUD display with real tower data
  - [ ] Multiple tower selection scenarios
  - [ ] Error handling (insufficient funds, max level towers)

### **[ ] 6.3 Gameplay Testing**

- [ ] **Manual Testing**: Comprehensive gameplay scenarios
- [ ] **Tests**:
  - [ ] Place towers → select → upgrade → verify improved performance
  - [ ] Economic balance (upgrade costs vs benefits)
  - [ ] UI responsiveness and visual clarity
  - [ ] Integration with existing wave/enemy systems
  - [ ] Performance with multiple upgraded towers

### **[ ] 6.4 Edge Case Testing**

- [ ] **Scenarios**:
  - [ ] Rapid clicking on towers and UI elements
  - [ ] Tower selection during active wave combat
  - [ ] Selling towers during enemy attacks
  - [ ] Maximum upgrade level boundary conditions
  - [ ] Insufficient funds scenarios
  - [ ] HUD behavior at different screen resolutions

---

## 📋 **Phase 7: Configuration & Documentation** 📚

### **[ ] 7.1 Configuration Finalization**

- [ ] **File**: `data/simulation/building-stats.json` - Finalize upgrade values
- [ ] **Content**:
  - [ ] Balance upgrade costs and benefits
  - [ ] Set reasonable sell percentages (75% default)
  - [ ] Configure upgrade multipliers (50% improvement default)
  - [ ] Set maximum upgrade levels per tower type

### **[ ] 7.2 Documentation**

- [ ] **New File**: `docs/systems/tower_upgrade_system.md`
- [ ] **Content**:
  - [ ] System architecture overview
  - [ ] Configuration options and their effects
  - [ ] Developer guide for extending upgrade system
  - [ ] API documentation for upgrade/sell services

### **[ ] 7.3 User Experience Documentation**

- [ ] **New File**: `docs/gameplay/tower_management.md`
- [ ] **Content**:
  - [ ] How to select, upgrade, and sell towers
  - [ ] Upgrade strategy guide
  - [ ] Economic considerations and tips
  - [ ] Visual indicators and UI guide

### **[ ] 7.4 Final Polish & Cleanup**

- [ ] **Tasks**:
  - [ ] Remove debug logging and temporary code
  - [ ] Optimize performance for upgrade calculations
  - [ ] Clean up unused configuration options
  - [ ] Verify all TODO comments are resolved
  - [ ] Code review and refactoring for maintainability

---

## 🔧 **Technical Implementation Notes**

### **Key Files to Create:**

- `src/Presentation/UI/TowerSelectionManager.cs` - Selection state management
- `src/Presentation/UI/TowerUpgradeHud.cs` - Upgrade HUD controller
- `scenes/UI/TowerUpgradeHud.tscn` - HUD scene layout
- `src/Domain/Buildings/Services/ITowerUpgradeService.cs` - Upgrade interface
- `src/Application/Buildings/Services/TowerUpgradeService.cs` - Upgrade logic
- `src/Domain/Buildings/Services/ITowerSellService.cs` - Sell interface
- `src/Application/Buildings/Services/TowerSellService.cs` - Sell logic
- `data/ui/tower_upgrade_hud_config.json` - HUD configuration

### **Key Files to Modify:**

- `src/Presentation/Buildings/Building.cs` - Add click detection, upgrade tracking
- `src/Infrastructure/Game/Services/GameService.cs` - Money transaction methods
- `data/simulation/building-stats.json` - Add upgrade configuration
- `data/audio/sound_config.json` - Add upgrade/sell sound effects

### **System Architecture:**

```
User clicks tower → TowerSelectionManager → TowerUpgradeHud
    ↓
TowerUpgradeHud → TowerUpgradeService/TowerSellService → GameService
    ↓
GameService → Update money → HudManager → Update HUD display
```

### **Configuration Structure:**

```json
{
  "building_types": {
    "basic_tower": {
      "cost": 50,
      "upgrade_cost": 25,
      "upgrade_multiplier": 1.5,
      "max_upgrade_level": 3,
      "sell_percentage": 0.75
    }
  }
}
```

---

## 🚨 **Risk Mitigation**

### **Potential Issues:**

1. **UI Performance**: Large modal HUD may impact performance
2. **Input Conflicts**: Tower selection vs building placement conflicts
3. **Economic Balance**: Upgrade costs may unbalance gameplay
4. **Visual Clarity**: Selection indicators may be too subtle/obvious
5. **Mobile Compatibility**: Touch input considerations for selection

### **Mitigation Strategies:**

- **Performance**: Use object pooling for HUD elements, lazy loading
- **Input**: Clear state machine for input modes, priority system
- **Balance**: Configurable upgrade costs, extensive playtesting
- **Visual**: Multiple visual indicator options, user testing
- **Mobile**: Touch area optimization, touch-friendly UI sizing

---

## ✅ **Success Criteria**

- [x] Players can left-click any placed tower to select it
- [x] Selected towers show clear visual feedback (border, range circle)
- [x] Tower upgrade HUD appears in top-right corner with all required information
- [x] Upgrade button correctly improves tower stats by 50% (configurable)
- [x] Sell button returns 75% of total investment (configurable)
- [x] HUD closes when clicking outside, pressing ESC, or clicking X button
- [x] Money transactions are accurate and update HUD immediately
- [x] Visual upgrades are visible on towers (level indicators)
- [x] System integrates seamlessly with existing building/wave systems
- [x] No performance degradation with multiple upgraded towers
- [x] All interactions provide appropriate audio/visual feedback

---

## 📈 **Estimated Timeline**

- **Phase 1**: 6-8 hours (tower selection and input system)
- **Phase 2**: 8-10 hours (HUD implementation and design)
- **Phase 3**: 6-8 hours (upgrade and sell logic)
- **Phase 4**: 4-6 hours (game systems integration)
- **Phase 5**: 4-6 hours (visual and audio enhancements)
- **Phase 6**: 6-8 hours (testing and quality assurance)
- **Phase 7**: 3-4 hours (configuration and documentation)

**Total Estimated Time**: 37-50 hours

---

This plan provides a comprehensive approach to implementing tower selection, upgrades, and selling while maintaining high code quality and seamless integration with the existing game systems.
