# Debug Wave Jump Feature Implementation Plan

## 🎯 **Objective**
Add keyboard shortcuts for quick wave testing without going through all waves manually.

**Shortcuts:**
- `Shift+5` = Jump directly to Wave 5 (Boss Wave)
- `Shift+6` = Jump to next wave
- `Shift+7` = Complete current wave instantly

**Estimated Time:** 10-15 minutes
**Difficulty:** ⭐⭐☆☆☆ (Easy)
**Progress:** 🔴 Phase 1 COMPLETE • Phase 2 PENDING • Phase 3 PENDING • Phase 4 PENDING

---

## 📋 **Phase 1: Core Wave Jump Logic** ✅ **COMPLETED**

### WaveManager Enhancement
- [x] Add `JumpToWave(int targetWave)` method to `WaveManager.cs`
- [x] Add `CompleteCurrentWaveInstantly()` method to `WaveManager.cs`
- [x] Add `JumpToNextWave()` method to `WaveManager.cs`
- [x] Add debug logging for wave jump operations

**Files modified:**
- ✅ `src/Infrastructure/Waves/Services/WaveManager.cs`

**Implementation Details:**
- ✅ `JumpToWave()` - Validates range, stops current wave, resets state, jumps to target
- ✅ `CompleteCurrentWaveInstantly()` - Forces current wave completion with proper cleanup
- ✅ `JumpToNextWave()` - Helper method that uses JumpToWave() for next wave
- ✅ Comprehensive debug logging with emojis for all operations
- ✅ Safety checks for edge cases (invalid waves, no wave in progress, etc.)

---

## 📋 **Phase 2: Input Handling**

### Main Scene Input Detection
- [ ] Add `_UnhandledKeyInput()` method to `Main.cs`
- [ ] Detect `Shift+5`, `Shift+6`, `Shift+7` key combinations
- [ ] Call appropriate WaveManager methods
- [ ] Add debug console messages for feedback

**Files to modify:**
- `src/Presentation/Core/Main.cs`

---

## 📋 **Phase 3: Safety & Polish**

### Input Validation & Safety
- [ ] Ensure wave jumps only work when not in wave progress
- [ ] Add bounds checking (can't jump to wave > total waves)
- [ ] Add visual/audio feedback for successful jumps
- [ ] Handle edge cases (jumping to current wave, etc.)

**Files to modify:**
- `src/Infrastructure/Waves/Services/WaveManager.cs` (validation)
- `src/Presentation/Core/Main.cs` (feedback)

---

## 📋 **Phase 4: Testing & Documentation**

### Quality Assurance
- [ ] Test `Shift+5` jumps directly to boss wave
- [ ] Test `Shift+6` advances to next wave properly
- [ ] Test `Shift+7` completes current wave instantly
- [ ] Verify wave completion rewards (money, etc.) work correctly
- [ ] Test edge cases (last wave, first wave, etc.)

### Documentation
- [ ] Add debug shortcuts to console output on game start
- [ ] Document shortcuts in code comments

---

## 🔧 **Technical Implementation Details**

### WaveManager Methods Structure:
```csharp
public void JumpToWave(int targetWave)
{
    // Validate target wave
    // Reset current state
    // Set wave number to target - 1
    // Start target wave
}

public void JumpToNextWave()
{
    // Jump to current wave + 1
}

public void CompleteCurrentWaveInstantly()
{
    // Force complete current wave if in progress
    // Trigger completion logic
}
```

### Input Detection Logic:
```csharp
public override void _UnhandledKeyInput(InputEvent @event)
{
    if (@event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.ShiftPressed)
    {
        switch (keyEvent.Keycode)
        {
            case Key.Key5: // Jump to Wave 5
            case Key.Key6: // Jump to next wave  
            case Key.Key7: // Complete current wave
        }
    }
}
```

---

## ⚡ **Success Criteria**
- [ ] Can instantly jump to boss wave for testing
- [ ] Can advance through waves quickly during development
- [ ] Can complete waves instantly to test wave completion logic
- [ ] No disruption to normal gameplay flow
- [ ] Clean debug output showing what happened

---

## 🎮 **Post-Implementation Usage**
Once complete, testing boss encounters becomes:
1. Start game
2. Press `Shift+5`
3. Boss wave starts immediately
4. Test boss music, mechanics, victory conditions, etc.

**Time saved per test cycle:** ~2-3 minutes → ~5 seconds! 🚀
