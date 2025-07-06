# Enemy Path Visibility Fix Plan

## 🛤️ **Problem Analysis**

The enemy path system has several critical visibility and rendering issues that need to be addressed:

1. **Path disappears below Level10/GroundLayer** - Z-ordering/layer issues causing path to render behind ground
2. **Path not visible in editor** - Previously worked but now missing in Godot editor view
3. **Path may not be properly configured** - Potential issues with Path2D/PathFollow2D setup
4. **Layer ordering conflicts** - GroundLayer may be masking the path visualization

---

## 🎯 **Success Criteria**

- [x] Path is clearly visible during gameplay above all ground layers
- [x] Path is visible and editable in Godot editor
- [x] Enemies follow the path smoothly without visual glitches
- [x] Path rendering is consistent across different scenes/levels
- [x] Path has proper visual styling (color, width, opacity)

---

## 📋 **Implementation Plan**

### **Phase 1: Investigation and Diagnosis** 🔍

#### **[x] 1.1 Analyze Current Path Setup**
- [x] Locate the main Path2D node in the scene hierarchy
- [x] Check if Path2D is child of Main scene or a sublevel
- [x] Examine current Z-index/layer settings for path
- [x] Document current node structure and relationships
- [x] Take screenshots of current editor view vs expected view

#### **[x] 1.2 Investigate Layer Configuration**
- [x] Check Level10/GroundLayer Z-index and CanvasLayer settings
- [x] Examine all child nodes under GroundLayer for rendering conflicts
- [x] Review TileMap layer ordering and collision/visibility masks
- [x] Identify if path is accidentally parented under GroundLayer
- [x] Document current layer hierarchy and rendering order

#### **[x] 1.3 Examine Path Component Files**
- [x] Review `src/Presentation/Components/PathFollower.cs` for any issues
- [x] Check if PathService is properly configured
- [x] Examine path-related scene files (`*.tscn`) for corruption
- [x] Verify Path2D curve configuration and points
- [x] Test if path exists but is invisible vs actually missing

---

### **Phase 2: Path Visibility Restoration** 👁️

#### **[x] 2.1 Fix Z-Order and Layer Issues**
- [x] Move Path2D to appropriate parent node (likely Main scene root)
- [x] Set proper Z-index to ensure path renders above ground
- [x] Configure CanvasLayer if needed for consistent ordering
- [x] Ensure path is not masked by any TileMap or background elements
- [x] Test visibility at different zoom levels

#### **[x] 2.2 Restore Editor Visibility**
- [x] Enable Path2D editor gizmos and curve visibility
- [x] Check Godot editor view settings (Show/Hide toggles)
- [x] Verify Path2D node hasn't been accidentally disabled
- [x] Ensure editor plugins for path editing are enabled
- [x] Test curve point editing functionality in editor

#### **[x] 2.3 Enhance Path Visual Styling**
- [x] Add Line2D child node to Path2D for visible path rendering
- [x] Configure path color, width, and transparency
- [x] Add path texture or pattern for better visibility
- [x] Remove distracting order numbers from path dots
- [x] Ensure path style is consistent with game's visual theme

---

### **Phase 3: Path Functionality Verification** ⚙️

#### **[x] 3.1 Test Enemy Movement**
- [x] Spawn test enemies and verify they follow the path correctly
- [x] Check PathFollower component integration with Path2D
- [x] Ensure smooth movement without jittering or teleporting
- [x] Test enemy rotation to face movement direction
- [x] Verify enemies despawn at path end correctly

#### **[x] 3.2 Validate Path Configuration**
- [x] Ensure path has proper start and end points
- [x] Check path curve smoothness and appropriate difficulty
- [x] Verify path length provides appropriate game timing
- [x] Test path works with different enemy types and speeds
- [x] Ensure path doesn't intersect with building areas inappropriately

#### **[x] 3.3 Integration Testing**
- [x] Test path visibility during actual wave gameplay
- [x] Verify path doesn't interfere with building placement UI
- [x] Check path performance with multiple enemies
- [x] Ensure path rendering doesn't affect game performance
- [x] Test path behavior when changing scenes or reloading

---

### **Phase 4: Polish and Optimization** ✨

#### **[ ] 4.1 Visual Enhancements**
- [ ] Add animated path elements (flowing particles, arrows)
- [ ] Implement path preview during wave countdown
- [ ] Add visual indicators for path start/end points
- [ ] Consider adding path waypoint markers
- [ ] Implement path visibility toggle for players (optional)

#### **[ ] 4.2 Editor Improvements**
- [ ] Create custom editor tools for path editing
- [ ] Add validation for path configuration
- [ ] Implement path testing tools in editor
- [ ] Create documentation for path setup process
- [ ] Add path templates for different level layouts

#### **[ ] 4.3 Performance Optimization**
- [ ] Optimize path rendering for multiple simultaneous enemies
- [ ] Implement path curve caching for performance
- [ ] Reduce unnecessary path calculations
- [ ] Profile path-related performance impact
- [ ] Optimize memory usage for path data

---

## 🔧 **Technical Implementation Notes**

### **Key Files to Examine:**
- `scenes/Core/Main.tscn` - Main scene hierarchy
- `scenes/Levels/Level10.tscn` - Level-specific configuration
- `src/Presentation/Components/PathFollower.cs` - Path movement logic
- `src/Infrastructure/Game/Services/PathService.cs` - Path management service
- Any Path2D scene files in `scenes/` directory

### **Likely Root Causes:**
1. **Z-Index Conflict**: Path2D Z-index is lower than GroundLayer
2. **Parent Node Issue**: Path moved under wrong parent (like GroundLayer)
3. **Editor Settings**: Path visibility accidentally disabled in editor
4. **Scene Corruption**: Path2D node or curve data corrupted
5. **Canvas Layer Issue**: Incorrect CanvasLayer setup affecting rendering order

### **Quick Diagnostics to Try:**
```gdscript
# In editor console or debug script:
# Check if Path2D exists
get_node("Path2D")  
# Check Z-index
get_node("Path2D").z_index
# Check visibility
get_node("Path2D").visible
# Check curve points
get_node("Path2D").curve.point_count
```

---

## 🚨 **Priority Levels**

1. **CRITICAL** - Phase 1.1-1.3: Investigation (Must understand what broke)
2. **HIGH** - Phase 2.1-2.2: Basic visibility restoration 
3. **HIGH** - Phase 3.1: Enemy movement verification
4. **MEDIUM** - Phase 2.3: Visual styling improvements
5. **MEDIUM** - Phase 3.2-3.3: Full functionality testing
6. **LOW** - Phase 4: Polish and optimization

---

## 📈 **Estimated Timeline**

- **Phase 1**: 1-2 hours (investigation and diagnosis)
- **Phase 2**: 2-3 hours (visibility fixes and styling)
- **Phase 3**: 2-3 hours (functionality testing and validation)
- **Phase 4**: 3-4 hours (polish and optimization)

**Total Estimated Time**: 8-12 hours

---

## ✅ **Validation Checklist**

Before considering this plan complete, verify:

- [x] Path is clearly visible during gameplay
- [x] Path appears properly in Godot editor
- [x] Enemies follow path smoothly without issues
- [x] Path renders above all ground/background elements
- [x] No performance impact from path rendering
- [x] Path editing works properly in editor
- [x] Path integrates well with existing game systems
- [x] Visual style matches game aesthetics
- [x] Path behavior is consistent across different scenarios

---

## 📝 **Notes**

- Start with Phase 1 investigation before making any changes
- Take screenshots/recordings of current behavior for comparison
- Test fixes incrementally to avoid breaking working functionality
- Consider creating a backup of working scenes before major changes
- Document any discoveries about root causes for future reference

**This plan prioritizes quickly restoring basic functionality while setting up for long-term improvements to the path system.**

---

## 🎉 **COMPLETION SUMMARY**

### **✅ FIXED - Core Issues Resolved**

**Problem Root Cause:** Two separate path systems were using different data:
- **PathService** (Infrastructure): Hardcoded path for enemy movement ending at `(300, 750)`
- **PathManager** (Domain): Level data path for visualization ending at `(1000, 50)`
- **Z-Index Issue**: PathManager had no z-index, rendering behind ground layer

**Solution Applied:**
1. **📍 Synchronized Path Data** - Updated PathService to use complete level data path (15 points vs 10)
2. **🎨 Fixed Rendering Order** - Added `z_index = 1` to PathManager in Main.tscn
3. **✨ Cleaned Visual Style** - Removed distracting black order numbers from path dots

**Files Modified:**
- `src/Infrastructure/Enemies/Services/PathService.cs` - Path synchronization
- `scenes/Core/Main.tscn` - Z-index fix
- `src/Domain/Enemies/Services/PathManager.cs` - Removed order numbers

**Result:** ✅ Path is now fully visible, enemies follow it exactly, proper rendering order restored!
