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

### **Phase 2: Boss Enemy Implementation** 👹 ✅ COMPLETED

#### **2.1 Boss Enemy Design** ✅ COMPLETED

- [x] **Visual Size**: 2x scale of regular enemies (programmatically)
- [x] **Health**: 5x health of regular enemies (500 vs 100)
- [x] **Speed**: 0.75x speed (45 vs 60 - slower but tankier)
- [x] **Spawn**: Only on wave 5, at the end of regular enemy spawns (35s delay)
- [x] **Count**: 1 boss enemy on wave 5

#### **2.2 Boss Enemy Files to Create/Modify** ✅ COMPLETED

**New Files:**

- [x] `src/Domain/Enemies/Entities/BossEnemy.cs` - Boss-specific logic with special abilities
- [x] `scenes/Enemies/BossEnemy.tscn` - Boss enemy scene with health bar and distinctive appearance

**Modified Files:**

- [x] `src/Presentation/Enemies/Enemy.cs` - Add scale/size modification support
- [x] `src/Infrastructure/Enemies/Services/WaveSpawnerService.cs` - Add boss spawn conditions and scaling
- [x] `data/stats/enemy_stats.json` - Boss-specific stats configuration

#### **2.3 Boss Enemy Implementation Details**

```csharp
// Boss Enemy Properties
- Health: 500 (vs normal 100)
- Speed: 75 (vs normal 100)
- Visual Scale: Vector2(2.0f, 2.0f)
- Reward: 50 money (vs normal 10)
- Special: Immune to certain tower types (optional)
```

#### **2.4 Boss Spawn Logic** ✅ COMPLETED

- [x] **Trigger**: Wave 5 starts
- [x] **Timing**: After all regular enemies in wave 5 are spawned (35 second delay)
- [x] **Delay**: 35 second pause before boss spawn (configured in wave data)
- [x] **Announcement**: Console announcement "BOSS INCOMING!" with warning
- [x] **Visual**: Boss enemy appears with 2x scale and distinctive red coloring

---

### **Phase 3: Speed Control System** ⚡ ✅ COMPLETED

#### **3.1 Speed Control UI** ✅ COMPLETED

- [x] **Location**: Top-right corner dedicated control panel
- [x] **Buttons**:
  - [x] `1x` (Normal speed) - Default state
  - [x] `2x` (Double speed)
  - [x] `4x` (Quadruple speed)
- [x] **Style**: Toggle buttons with clear visual feedback and color modulation

#### **3.2 Speed Control Implementation** ✅ COMPLETED

**Files Created/Modified:**

- [x] `src/Presentation/UI/SpeedControl.cs` - Speed control logic with event handling
- [x] `scenes/UI/SpeedControlPanel.tscn` - Speed control UI scene
- [x] `src/Application/Game/Services/TimeManager.cs` - Global time scaling service
- [x] `src/Presentation/Core/Main.cs` - Added TimeManager and SpeedControl initialization
- [x] `src/Application/Shared/Services/SpeedControlDebugCommands.cs` - Debug commands for testing

#### **3.3 Speed Control Logic** ✅ COMPLETED

**Implemented TimeManager with:**
- [x] Singleton pattern for global access
- [x] `SetGameSpeed()` method with Engine.TimeScale integration
- [x] Speed cycling: 1x → 2x → 4x → 1x
- [x] Speed change events for UI synchronization
- [x] Keyboard input handling (1/2/4 keys)
- [x] Proper cleanup on exit

#### **3.4 Speed Control UI Design** ✅ COMPLETED

**Implemented UI with:**
- [x] **Active button**: Visual pressed state and color modulation
- [x] **Hover effects**: Color feedback on hover and press
- [x] **Keyboard shortcuts**:
  - [x] `1` key = 1x speed
  - [x] `2` key = 2x speed
  - [x] `4` key = 4x speed
- [x] **Real-time sync**: UI buttons update when speed changes via keyboard
- [x] **Panel positioning**: Top-right corner with proper styling

---

### **Phase 4: Integration & Polish** ✨

#### **4.1 Wave 5 Complete Experience** ✅ COMPLETED

- [x] **Boss spawn announcement** - Console announcements with dramatic messaging
- [x] **Boss battle music** - Epic boss battle music plays when boss spawns
- [x] **Victory celebration** after boss defeat - Victory messages and music stop
- [x] **Proper game completion flow** - All waves complete with boss music management
- [x] **Building construction sounds** - Tower placement plays appropriate build sounds

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

## 📋 **Phase 3 Completion Summary** ✅

### **✅ PHASE 3: SPEED CONTROL SYSTEM - COMPLETED!**

**What was accomplished:**

1. **Complete Speed Control System** - Players can now control game speed with 1x/2x/4x options
2. **TimeManager Service** - Global time scaling using Godot's `Engine.TimeScale`
3. **UI Integration** - Top-right speed control panel with visual feedback
4. **Keyboard Shortcuts** - Press 1/2/4 keys for instant speed changes
5. **Real-time Synchronization** - UI buttons stay in sync with speed changes

**Files Created:**
- `src/Application/Game/Services/TimeManager.cs` - Global time scaling service
- `src/Presentation/UI/SpeedControl.cs` - Speed control UI component
- `scenes/UI/SpeedControlPanel.tscn` - Speed control panel scene
- `src/Application/Shared/Services/SpeedControlDebugCommands.cs` - Testing commands

**Files Modified:**
- `src/Presentation/Core/Main.cs` - Added TimeManager and SpeedControl initialization

**Technical Features:**
- **Engine Integration**: Uses `Engine.TimeScale` for consistent speed scaling
- **Event System**: SpeedChanged events keep UI synchronized
- **Button States**: Active speed shown with pressed state and color modulation
- **Keyboard Input**: Global input handling for 1/2/4 keys
- **Singleton Pattern**: TimeManager accessible globally via `TimeManager.Instance`
- **Error Handling**: Graceful fallbacks and comprehensive logging

**User Experience:**
- **Default Speed**: Game starts at 1x (normal speed)
- **Speed Options**: 1x → 2x → 4x with smooth transitions
- **Visual Feedback**: Active button highlighted, inactive buttons dimmed
- **Keyboard Shortcuts**: Quick speed changes during gameplay
- **Consistent UI**: Speed panel positioned in top-right corner

**Result:**
✅ Players can now speed up gameplay with 2x and 4x options
✅ UI shows clear visual feedback for current speed
✅ Keyboard shortcuts (1/2/4 keys) work for quick speed changes
✅ Game speed affects all time-based elements consistently
✅ Debug commands available for testing speed functionality

## 📋 **Phase 4.1 Completion Summary** ✅

### **✅ PHASE 4.1: WAVE 5 COMPLETE EXPERIENCE - COMPLETED!**

**What was accomplished:**

1. **Boss Battle Music Integration** - Added epic boss battle music system:
   - Added `boss_battle` music track to `data/audio/sound_config.json`
   - Boss music automatically plays when boss enemy spawns
   - Music stops when all waves are completed for proper game flow
   - Uses dedicated Music category for proper audio mixing

2. **Building Construction Sound System** - Added satisfying construction audio:
   - Added `basic_tower_build` and `sniper_tower_build` sounds to audio config
   - Construction sounds play automatically when towers are successfully placed
   - Sound selection based on tower type (Basic vs Sniper)
   - Integrated with existing SoundManagerService architecture

3. **Victory Celebration System** - Enhanced game completion experience:
   - Dramatic victory messages when all waves are completed
   - Automatic boss music stop on victory
   - Celebratory console announcements for player feedback
   - Proper game flow completion with audio management

4. **Enhanced Boss Experience** - Complete boss encounter polish:
   - Boss spawn triggers epic battle music automatically
   - Console announcements warn players of incoming boss
   - Victory celebration when boss is defeated
   - Complete audio-visual experience for final challenge

**Files Created/Modified:**

**Audio Configuration:**
- `data/audio/sound_config.json` - Added boss music and construction sounds

**Boss Battle Music:**
- `src/Infrastructure/Enemies/Services/WaveSpawnerService.cs` - Boss music trigger
- Added `PlayBossBattleMusic()` method with Music category support

**Construction Sounds:**
- `src/Presentation/Player/PlayerBuildingBuilder.cs` - Construction sound integration
- Added `PlayConstructionSound()` method with tower type detection

**Victory System:**
- `src/Infrastructure/Waves/Services/WaveManager.cs` - Victory celebration
- Added `PlayVictoryCelebration()` and `StopBossBattleMusic()` methods

**Technical Features:**
- **Music Category Support**: Boss music uses proper Music audio category
- **Tower Type Detection**: Construction sounds adapt to building type
- **Audio Flow Management**: Music starts/stops at appropriate game moments
- **Fallback Handling**: Graceful degradation when audio services unavailable
- **Console Feedback**: Rich logging for all audio events and celebrations

**User Experience:**
- **Epic Boss Encounters**: Boss spawns trigger dramatic music for intense atmosphere
- **Satisfying Construction**: Tower placement provides immediate audio feedback
- **Victory Satisfaction**: Clear celebration when all challenges are overcome
- **Audio Polish**: Professional audio integration enhances immersion
- **Seamless Flow**: Music and sounds integrate naturally with gameplay

**Result:**
✅ Boss battle music creates epic atmosphere when boss spawns
✅ Construction sounds provide satisfying feedback for tower placement
✅ Victory celebration marks successful game completion
✅ All audio integrates seamlessly with existing sound system
✅ Game flow enhanced with proper music management

---

### **✅ PHASE 2: BOSS ENEMY IMPLEMENTATION - COMPLETED!**

**What was accomplished:**

1. **Boss Enemy Configuration** - Added complete boss enemy stats to `data/stats/enemy_stats.json`:
   - Health: 500 (5x regular enemies)
   - Speed: 45 (0.75x regular speed for tankier feel)
   - Reward: 100 gold (massive reward for defeating)
   - Damage: 30 (high threat level)

2. **Domain Layer Enhancement** - Created `BossEnemy.cs` with advanced capabilities:
   - Inherits from base `Enemy` class following clean architecture
   - Special abilities: Damage immunity cooldown system
   - Scale multiplier support (2x visual size)
   - Phase-based behavior (final phase at 25% health)
   - Enhanced logging and debugging information

3. **Presentation Layer Scaling** - Enhanced `Enemy.cs` with scale support:
   - `SetScaleMultiplier()` method for dynamic sizing
   - `ApplyVisualScale()` for real-time visual updates
   - `IsBossEnemy()` detection method
   - Automatic scale application during initialization

4. **Wave Configuration Integration** - Updated `default_waves.json` wave 5:
   - Added boss enemy spawn group with 35-second delay
   - Positioned after all regular enemies (total assault + boss finale)
   - Speed multiplier 0.75x for slower, more menacing movement
   - Reward: 100 gold for defeating the boss

5. **Boss Scene Creation** - Created specialized `BossEnemy.tscn`:
   - Larger collision box (32x32 vs 16x16)
   - Distinctive red coloring for visual identification
   - Built-in health bar for boss-specific UI
   - Higher z-index for prominence
   - Pre-configured as "boss_enemy" type

6. **Spawning Logic Enhancement** - Updated `WaveSpawnerService.cs`:
   - Dynamic scene selection (Boss vs Regular enemy scenes)
   - Automatic 2x scaling application for boss enemies
   - Boss spawn announcements with warnings
   - Enhanced logging for boss encounters

**Files Created:**
- `src/Domain/Enemies/Entities/BossEnemy.cs` - Advanced boss entity with special abilities
- `scenes/Enemies/BossEnemy.tscn` - Specialized boss enemy scene with health bar

**Files Modified:**
- `data/stats/enemy_stats.json` - Added boss enemy configuration
- `data/waves/default_waves.json` - Added boss to wave 5 finale
- `src/Presentation/Enemies/Enemy.cs` - Added scaling and boss detection support
- `src/Infrastructure/Enemies/Services/WaveSpawnerService.cs` - Boss spawning logic and announcements

**Technical Features:**
- **Config-Driven Design**: All boss stats come from JSON configuration files
- **Clean Architecture**: Boss inherits from base Enemy following domain patterns
- **Visual Scaling**: Programmatic 2x scaling with collision box adjustments
- **Special Abilities**: Damage immunity system with cooldown mechanics
- **Dynamic Scene Loading**: Automatic boss scene selection based on enemy type
- **Enhanced Logging**: Detailed boss spawn announcements and warnings

**User Experience:**
- **Final Challenge**: Wave 5 culminates with a massive boss enemy
- **Visual Impact**: Boss is clearly 2x the size with distinctive red coloring
- **Reward System**: 100 gold reward makes boss defeat highly rewarding
- **Progressive Difficulty**: Boss spawns after regular enemies for escalating tension
- **Clear Feedback**: Console announcements warn players of incoming boss

**Result:**
✅ Boss enemy spawns only on wave 5 after a 35-second dramatic pause
✅ Boss is visually 2x the size of regular enemies with red coloring
✅ Boss has 5x health (500 HP) making it a significant challenge
✅ Boss moves 25% slower but deals massive damage
✅ Boss defeat provides 100 gold reward for the accomplishment
✅ System is fully configuration-driven and extensible

---

**🎉 This plan will transform the current wave system into a polished, engaging experience with proper progression, challenging finale, and flexible pacing control!**
