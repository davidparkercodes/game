# Tower Selection, Upgrade & Sell System Plan

## Execution Instructions

**Process**: Execute phases one at a time. When a phase is complete:

1. Update this plan file to mark completed items
2. Run `dotnet clean && dotnet build && dotnet test`
3. If all pass, proceed to next phase
4. If warnings or errors occur, fix them before proceeding
5. Continue until all phases are complete

---

## 🎯 **Objective**

Implement a comprehensive tower selection, upgrade, and selling system that allows players to interact with placed towers through left-click selection, showing a modal HUD with tower information, upgrade options, and sell functionality.

---

## 📋 **Phase 1: Tower Selection & Input System** 🎯

### **[ ] 1.1 Implement Tower Click Detection**
- [ ] **File**: `src/Presentation/Buildings/Building.cs` - Add input handling
- [ ] **Action**: Add `_on_input_event` method to handle mouse clicks
- [ ] **Features**:
  - [ ] Detect left mouse button clicks on tower collision area
  - [ ] Ignore clicks when player has active building to place
  - [ ] Add selection state tracking (`IsSelected` property)
  - [ ] Emit selection signal to TowerSelectionManager

### **[ ] 1.2 Create Tower Selection Manager**
- [ ] **New File**: `src/Presentation/UI/TowerSelectionManager.cs`
- [ ] **Purpose**: Central manager for tower selection state
- [ ] **Features**:
  - [ ] Singleton pattern for global access
  - [ ] Track currently selected tower
  - [ ] Handle tower selection/deselection logic
  - [ ] Coordinate with TowerUpgradeHud display
  - [ ] Handle "click outside" detection for closing HUD

### **[ ] 1.3 Add Visual Selection Feedback**
- [ ] **File**: `src/Presentation/Buildings/Building.cs` - Visual enhancements
- [ ] **Action**: Add visual selection indicators
- [ ] **Features**:
  - [ ] Add 2px solid black border when selected
  - [ ] Show range circle when tower is selected
  - [ ] Highlight tower with subtle color change
  - [ ] Ensure selection visuals are clear and professional

### **[ ] 1.4 Integration with Build Mode**
- [ ] **File**: `src/Presentation/Player/PlayerBuildingBuilder.cs` - Check integration
- [ ] **Action**: Ensure tower selection doesn't interfere with building placement
- [ ] **Features**:
  - [ ] Disable tower selection when player has active building to place
  - [ ] Clear tower selection when entering build mode
  - [ ] Prevent tower selection during build preview mode

---

## 📋 **Phase 2: Tower Upgrade HUD Implementation** 🖥️

### **[ ] 2.1 Create Tower Upgrade HUD Scene**
- [ ] **New File**: `scenes/UI/TowerUpgradeHud.tscn`
- [ ] **Structure**: Large horizontal rectangle centered on screen
- [ ] **Components**:
  - [ ] Main Panel (horizontal rectangle, center-anchored)
  - [ ] Left Section: Tower image/icon display
  - [ ] Center Section: Tower name, description, current stats
  - [ ] Right Section: Upgrade stats, upgrade button, sell button
  - [ ] Bottom-right: Close button (X)
  - [ ] Semi-transparent background overlay

### **[ ] 2.2 Implement Tower Upgrade HUD Script**
- [ ] **New File**: `src/Presentation/UI/TowerUpgradeHud.cs`
- [ ] **Purpose**: Manage tower upgrade HUD functionality
- [ ] **Features**:
  - [ ] Display tower information (name, image, description)
  - [ ] Show current stats (damage, range, attack speed, etc.)
  - [ ] Display upgrade preview stats (50% improvement)
  - [ ] Handle upgrade button click (cost validation, stat updates)
  - [ ] Handle sell button click (75% value return)
  - [ ] Handle close button and outside-click closing
  - [ ] Modal behavior (block input to other UI elements)

### **[ ] 2.3 Design HUD Layout & Styling**
- [ ] **File**: `scenes/UI/TowerUpgradeHud.tscn` - Polish layout
- [ ] **Features**:
  - [ ] Professional UI design with proper spacing
  - [ ] Clear visual hierarchy (title, stats, buttons)
  - [ ] Consistent styling with existing game UI
  - [ ] Responsive layout that works at different screen sizes
  - [ ] Smooth show/hide animations (optional polish)

### **[ ] 2.4 Add Configuration Support**
- [ ] **New File**: `data/ui/tower_upgrade_hud_config.json`
- [ ] **Purpose**: Configure HUD appearance and behavior
- [ ] **Content**:
  - [ ] HUD dimensions and positioning
  - [ ] Button styling and text
  - [ ] Animation settings
  - [ ] Color schemes and visual themes
  - [ ] Tower icon/image paths

---

## 📋 **Phase 3: Upgrade & Sell Logic Implementation** ⚙️

### **[ ] 3.1 Implement Tower Upgrade System**
- [ ] **File**: `src/Domain/Buildings/Services/ITowerUpgradeService.cs` - New interface
- [ ] **File**: `src/Application/Buildings/Services/TowerUpgradeService.cs` - Implementation
- [ ] **Features**:
  - [ ] Calculate upgrade costs from configuration
  - [ ] Apply 50% stat improvements (configurable multiplier)
  - [ ] Track tower upgrade levels (level 1, 2, 3, etc.)
  - [ ] Validate upgrade affordability before application
  - [ ] Update building stats in real-time

### **[ ] 3.2 Implement Tower Selling System**
- [ ] **File**: `src/Domain/Buildings/Services/ITowerSellService.cs` - New interface
- [ ] **File**: `src/Application/Buildings/Services/TowerSellService.cs` - Implementation
- [ ] **Features**:
  - [ ] Calculate sell value (75% of total investment)
  - [ ] Track total investment (base cost + all upgrade costs)
  - [ ] Handle money return to player
  - [ ] Remove tower from BuildingRegistry
  - [ ] Clean up tower scene and references

### **[ ] 3.3 Extend Building Entity for Upgrade Tracking**
- [ ] **File**: `src/Presentation/Buildings/Building.cs` - Add upgrade properties
- [ ] **Features**:
  - [ ] `UpgradeLevel` property (starts at 0)
  - [ ] `TotalInvestment` property (tracks money spent)
  - [ ] `BaseStats` property (original stats for upgrade calculations)
  - [ ] `GetUpgradePreview()` method (shows next level stats)
  - [ ] `ApplyUpgrade()` method (increases stats and level)

### **[ ] 3.4 Configuration Integration**
- [ ] **File**: `data/simulation/building-stats.json` - Add upgrade configuration
- [ ] **Features**:
  - [ ] Add `upgrade_multiplier` (default: 1.5 for 50% improvement)
  - [ ] Add `sell_percentage` (default: 0.75 for 75% return)
  - [ ] Add `max_upgrade_level` (default: 3 levels)
  - [ ] Per-tower-type upgrade cost scaling
  - [ ] Upgrade stat scaling curves (linear vs exponential)

---

## 📋 **Phase 4: Game Systems Integration** 🎮

### **[ ] 4.1 Money & Economy Integration**
- [ ] **File**: `src/Infrastructure/Game/Services/GameService.cs` - Update money handling
- [ ] **Features**:
  - [ ] Add `CanAffordUpgrade(cost)` method
  - [ ] Add `SpendMoneyOnUpgrade(cost, towerName)` method
  - [ ] Add `ReceiveMoneyFromSale(amount, towerName)` method
  - [ ] Update HUD money display after transactions
  - [ ] Add transaction logging for debugging

### **[ ] 4.2 Stats Service Integration**
- [ ] **File**: `src/Infrastructure/Stats/Services/StatsManagerService.cs` - Upgrade support
- [ ] **Features**:
  - [ ] Add `GetUpgradeStats(towerType, currentLevel)` method
  - [ ] Add `GetUpgradeCost(towerType, currentLevel)` method
  - [ ] Support for per-level stat calculations
  - [ ] Cache upgrade calculations for performance

### **[ ] 4.3 Building Registry Updates**
- [ ] **File**: `src/Presentation/Buildings/BuildingRegistry.cs` - Track upgrades
- [ ] **Features**:
  - [ ] Track tower upgrade levels in registry
  - [ ] Update collision detection for upgraded towers
  - [ ] Handle tower removal during selling
  - [ ] Maintain registry consistency during upgrades/sales

### **[ ] 4.4 Player Interaction Updates**
- [ ] **File**: `src/Presentation/Player/Player.cs` - Integrate with selection
- [ ] **Features**:
  - [ ] Disable building placement when tower upgrade HUD is open
  - [ ] Clear building selection when tower is selected
  - [ ] Handle input priority (tower selection vs building placement)
  - [ ] Update building stats display logic

---

## 📋 **Phase 5: Visual & Audio Enhancements** ✨

### **[ ] 5.1 Visual Feedback System**
- [ ] **File**: `src/Presentation/Buildings/Building.cs` - Visual improvements
- [ ] **Features**:
  - [ ] Upgrade level visual indicators (level badges, stars, etc.)
  - [ ] Upgraded tower visual changes (color tints, particle effects)
  - [ ] Selection animation (smooth border appearance)
  - [ ] Range circle improvements (show upgrade range preview)

### **[ ] 5.2 Sound Effects Integration**
- [ ] **File**: `data/audio/sound_config.json` - Add upgrade/sell sounds
- [ ] **Features**:
  - [ ] `tower_upgrade` sound effect
  - [ ] `tower_sell` sound effect  
  - [ ] `tower_select` sound effect
  - [ ] `ui_button_click` for HUD interactions
  - [ ] `ui_modal_open/close` for HUD show/hide

### **[ ] 5.3 Animation & Polish**
- [ ] **File**: `src/Presentation/UI/TowerUpgradeHud.cs` - Add animations
- [ ] **Features**:
  - [ ] Smooth HUD show/hide animations (fade in/out, scale)
  - [ ] Button hover effects and visual feedback
  - [ ] Stat number animations (counter-up effects)
  - [ ] Upgrade success visual feedback (particles, flash)

### **[ ] 5.4 Accessibility & UX**
- [ ] **Files**: Multiple UI files - Improve accessibility
- [ ] **Features**:
  - [ ] Keyboard shortcuts (ESC to close HUD, U for upgrade, S for sell)
  - [ ] Tooltips with detailed information
  - [ ] Clear visual hierarchy and readable fonts
  - [ ] Confirmation dialogs for expensive operations
  - [ ] Color-blind friendly visual indicators

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

- [ ] Players can left-click any placed tower to select it
- [ ] Selected towers show clear visual feedback (border, range circle)
- [ ] Tower upgrade HUD appears centered on screen with all required information
- [ ] Upgrade button correctly improves tower stats by 50% (configurable)
- [ ] Sell button returns 75% of total investment (configurable)
- [ ] HUD closes when clicking outside, pressing ESC, or clicking X button
- [ ] Money transactions are accurate and update HUD immediately
- [ ] Visual upgrades are visible on towers (level indicators)
- [ ] System integrates seamlessly with existing building/wave systems
- [ ] No performance degradation with multiple upgraded towers
- [ ] All interactions provide appropriate audio/visual feedback

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
