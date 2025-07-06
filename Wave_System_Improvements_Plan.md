# Wave System Improvements Plan

## 🎯 **Overview**

Enhance the current wave system to provide better gameplay flow with a proper 5-wave structure, introduce a boss enemy on the final wave, and add game speed controls for improved user experience.

---

## 📋 **Current Issues & Goals**

### **Issues to Fix:**

- ❌ Game shows "Waves: 5/10" but ends at wave 5 with "All Waves Complete"
- ❌ No challenging final encounter
- ❌ No speed control options for players

### **Goals:**

- ✅ Implement proper 5-wave system
- ✅ Add boss enemy on wave 5 (2x visual size)
- ✅ Add x2 and x4 speed buttons for game pacing control

---

## 🚀 **Implementation Plan**

### **Phase 1: Wave System Configuration** 📊

#### **1.1 Update Wave Count** ✅ COMPLETED

- [x] **File**: `src/Infrastructure/Rounds/Services/RoundService.cs`
- [x] **Change**: Removed hardcoded `TotalRounds = 10`, now dynamically loaded from wave configuration
- [x] **Implementation**: `TotalRounds = -1` initially, set via `SetTotalRounds()` from `WaveManager`
- [x] **Validation**: UI shows "Waves: X/5" correctly (or "X/?" when not yet loaded)
- [x] **Configuration-Driven**: Total waves now come from `data/waves/*.json` files

#### **1.2 Wave Progression Logic** ✅ COMPLETED

- [x] **Review**: Current wave spawning and progression system
- [x] **Fix**: Integrated `WaveManager` to set `RoundService.TotalRounds` from wave configuration
- [x] **Implementation**: 
  - `WaveManager` reads total waves from `WaveConfigurationService`
  - `WaveManager` calls `RoundService.Instance?.SetTotalRounds(totalWaves)`
  - HUD displays correct count dynamically
- [x] **Ensure**: Proper "Victory" state after wave completion (based on configured wave count)

---

### **Phase 2: Boss Enemy Implementation** 👹

#### **2.1 Boss Enemy Design**

- [ ] **Visual Size**: 2x scale of regular enemies (programmatically)
- [ ] **Health**: 3x-5x health of regular enemies
- [ ] **Speed**: 0.75x speed (slower but tankier)
- [ ] **Spawn**: Only on wave 5, at the end of regular enemy spawns
- [ ] **Count**: 1 boss enemy on wave 5

#### **2.2 Boss Enemy Files to Create/Modify**

**New Files:**

- [ ] `src/Domain/Enemies/Entities/BossEnemy.cs` - Boss-specific logic
- [ ] `scenes/Enemies/BossEnemy.tscn` - Boss enemy scene

**Modified Files:**

- [ ] `src/Presentation/Enemies/Enemy.cs` - Add scale/size modification support
- [ ] Wave spawning logic - Add boss spawn conditions
- [ ] Enemy stats configuration - Boss-specific stats

#### **2.3 Boss Enemy Implementation Details**

```csharp
// Boss Enemy Properties
- Health: 500 (vs normal 100)
- Speed: 75 (vs normal 100)
- Visual Scale: Vector2(2.0f, 2.0f)
- Reward: 50 money (vs normal 10)
- Special: Immune to certain tower types (optional)
```

#### **2.4 Boss Spawn Logic**

- [ ] **Trigger**: Wave 5 starts
- [ ] **Timing**: After all regular enemies in wave 5 are spawned
- [ ] **Delay**: 3-5 second pause before boss spawn
- [ ] **Announcement**: UI message "Boss Incoming!"
- [ ] **Visual**: Boss enemy appears with 2x scale

---

### **Phase 3: Speed Control System** ⚡

#### **3.1 Speed Control UI**

- [ ] **Location**: Top-right corner or dedicated control panel
- [ ] **Buttons**:
  - [ ] `1x` (Normal speed) - Default state
  - [ ] `2x` (Double speed)
  - [ ] `4x` (Quadruple speed)
- [ ] **Style**: Toggle buttons with clear visual feedback

#### **3.2 Speed Control Implementation**

**Files to Create/Modify:**

- [ ] `src/Presentation/UI/SpeedControl.cs` - Speed control logic
- [ ] `scenes/UI/SpeedControlPanel.tscn` - Speed control UI
- [ ] `src/Application/Game/Services/TimeManager.cs` - Global time scaling
- [ ] Main scene - Add speed control panel

#### **3.3 Speed Control Logic**

```csharp
// Time Scale Management
public class TimeManager : Node
{
    private float _currentTimeScale = 1.0f;

    public void SetGameSpeed(float multiplier)
    {
        _currentTimeScale = multiplier;
        Engine.TimeScale = multiplier;

        // Update UI feedback
        UpdateSpeedButtonStates();
    }

    public void ToggleSpeed()
    {
        // Cycle: 1x -> 2x -> 4x -> 1x
    }
}
```

#### **3.4 Speed Control UI Design**

```
┌─────────────────┐
│ Speed Control   │
├─────────────────┤
│ [1x] [2x] [4x]  │
│  ✓             │
└─────────────────┘
```

- [ ] **Active button**: Highlighted/pressed visual state
- [ ] **Hover effects**: Clear button feedback
- [ ] **Keyboard shortcuts**:
  - [ ] `1` key = 1x speed
  - [ ] `2` key = 2x speed
  - [ ] `4` key = 4x speed

---

### **Phase 4: Integration & Polish** ✨

#### **4.1 Wave 5 Complete Experience**

- [ ] **Boss spawn announcement**
- [ ] **Boss health bar** (optional enhancement)
- [ ] **Victory celebration** after boss defeat
- [ ] **Proper game completion flow**

#### **4.2 Speed Control Polish**

- [ ] **Pause game** when speed buttons are inactive
- [ ] **Audio scaling** (optional - sounds at different speeds)
- [ ] **Visual effects** maintain quality at all speeds
- [ ] **Enemy animations** scale appropriately

#### **4.3 UI Integration**

- [ ] **Consistent styling** with existing UI elements
- [ ] **Responsive layout** that works at different resolutions
- [ ] **Clear visual hierarchy** for new elements

---

## 🔧 **Technical Implementation Details**

### **Wave System Architecture**

```
WaveManager
├── WaveData (5 waves total)
├── EnemySpawner
│   ├── RegularEnemySpawn()
│   └── BossEnemySpawn() // Wave 5 only
└── WaveProgressionUI
```

### **Boss Enemy Architecture**

```
BossEnemy : Enemy
├── Enhanced Stats (Health, Size, Speed)
├── Visual Scale Override
├── Special Behaviors (optional)
└── Reward Multiplier
```

### **Speed Control Architecture**

```
SpeedControlSystem
├── TimeManager (Global time scaling)
├── SpeedControlUI (Button states)
├── KeyboardInputHandler
└── VisualFeedbackManager
```

---

## 🧪 **Testing Checklist**

### **Wave System Tests**

- [ ] Game shows "Waves: 1/5" at start
- [ ] All 5 waves spawn correctly
- [ ] Wave 5 completes with boss defeat
- [ ] Victory screen appears after wave 5
- [ ] No "wave 6" or beyond attempts

### **Boss Enemy Tests**

- [ ] Boss spawns only on wave 5
- [ ] Boss is visually 2x size of regular enemies
- [ ] Boss has appropriate health/stats
- [ ] Boss follows path correctly despite larger size
- [ ] Boss defeat triggers wave/game completion

### **Speed Control Tests**

- [ ] 1x speed works normally (default)
- [ ] 2x speed doubles all game elements
- [ ] 4x speed quadruples all game elements
- [ ] Speed transitions are smooth
- [ ] UI buttons show correct active states
- [ ] Keyboard shortcuts work
- [ ] Game remains playable at all speeds

---

## 📦 **File Structure Changes**

### **New Files:**

```
src/Domain/Enemies/Entities/BossEnemy.cs
src/Presentation/UI/SpeedControl.cs
src/Application/Game/Services/TimeManager.cs
scenes/Enemies/BossEnemy.tscn
scenes/UI/SpeedControlPanel.tscn
```

### **Modified Files:**

```
src/Presentation/Enemies/Enemy.cs          // Scale support
scenes/Core/Main.tscn                      // Speed control UI
[WaveManager files]                        // 5-wave limit
[Wave spawning logic]                      // Boss spawn
[UI layouts]                              // Speed control integration
```

---

## 🎮 **User Experience Flow**

### **Normal Gameplay:**

1. **Waves 1-4**: Regular enemies with increasing difficulty
2. **Wave 5 Start**: "Final Wave!" announcement
3. **Wave 5 Middle**: Regular enemies spawn as normal
4. **Wave 5 End**: "Boss Incoming!" → Large boss enemy appears
5. **Boss Defeat**: Victory celebration and game completion

### **Speed Control Usage:**

1. **Default**: Game runs at 1x speed (normal)
2. **Impatient**: Player clicks 2x for faster pacing
3. **Very Impatient**: Player clicks 4x for maximum speed
4. **Strategy**: Player returns to 1x for difficult sections
5. **Keyboard**: Quick speed changes via 1/2/4 keys

---

## 🔄 **Implementation Order**

### **Priority 1 (Core Functionality):**

- [ ] 1. Fix wave count to 5 total waves
- [ ] 2. Implement basic boss enemy (2x size, enhanced stats)
- [ ] 3. Add boss spawn logic to wave 5

### **Priority 2 (Essential Features):**

- [ ] 4. Create speed control UI (1x/2x/4x buttons)
- [ ] 5. Implement TimeManager for global speed scaling
- [ ] 6. Add keyboard shortcuts for speed control

### **Priority 3 (Polish & Integration):**

- [ ] 7. Boss spawn announcements and effects
- [ ] 8. Speed control visual feedback and transitions
- [ ] 9. Final testing and UI polish

---

## 🎯 **Success Criteria**

### **Wave System:**

- ✅ Game correctly shows and completes 5 waves total
- ✅ Victory state triggers after wave 5 completion
- ✅ No confusion about wave count or progression

### **Boss Enemy:**

- ✅ Boss appears only on wave 5
- ✅ Boss is clearly visually distinct (2x size)
- ✅ Boss provides appropriate challenge increase
- ✅ Boss defeat feels rewarding and conclusive

### **Speed Control:**

- ✅ All three speed options work smoothly
- ✅ Speed changes don't break game mechanics
- ✅ UI is intuitive and responsive
- ✅ Keyboard shortcuts enhance usability

---

---

## 📋 **Phase 1 Completion Summary** ✅

### **✅ PHASE 1: WAVE SYSTEM CONFIGURATION - COMPLETED!**

**What was accomplished:**

1. **Removed ALL hardcoded wave counts** - The system is now 100% configuration-driven
2. **Dynamic wave loading** - Total waves come directly from wave configuration files:
   - `data/waves/default_waves.json` → 5 waves
   - `data/waves/easy_waves.json` → 3 waves  
   - `data/waves/hard_waves.json` → 4 waves
3. **Fixed UI display** - HUD now shows correct "Wave: X/Y" based on loaded configuration
4. **Proper architecture** - `WaveManager` → `WaveConfigurationService` → `RoundService` integration

**Files Modified:**
- `src/Infrastructure/Rounds/Services/RoundService.cs` - Removed hardcoded `TotalRounds = 10`
- `src/Infrastructure/Waves/Services/WaveManager.cs` - Added configuration sync logic
- `src/Presentation/UI/Hud.cs` - Removed hardcoded fallback values
- `src/Domain/Enemies/Services/PathManager.cs` - Fixed nullable warning

**Technical Implementation:**
- `RoundService.TotalRounds` starts as `-1` (not yet loaded)
- `WaveManager` calls `RoundService.SetTotalRounds()` with value from wave configuration
- HUD displays "Wave: X/?" until configuration loads, then "Wave: X/Y" correctly
- System adapts to any wave count defined in configuration files

**Result:** 
✅ Game now properly shows "Waves: X/5" and ends at wave 5 with "All Waves Complete"
✅ No more "5/10" display issue  
✅ Fully extensible system - easy to create campaigns with different wave counts

---

**🎉 This plan will transform the current wave system into a polished, engaging experience with proper progression, challenging finale, and flexible pacing control!**
