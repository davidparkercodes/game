# Map Boundaries and Background Implementation Plan

## 🎯 **Objective**
Implement proper map boundaries and visual styling using the existing tilemap system with clear rules for player interaction and wave movement.

## 📋 **Current Assets**
- **Tileset**: `basic_colors_tilesheet_16x16.png` (64px × 32px)
- **Grid**: 8 tiles (4×2), each 16×16 pixels
- **Available Tiles**:
  - **Black tile**: Top-left position (index 0) - Wave paths
  - **White tile**: Top-left + 1 position (index 1) - Buildable areas
- **Map Size**: Currently undefined boundaries
- **Background**: Gray abyss → needs to be `#32283e`

## 🎮 **Game Rules to Implement**

### **White Tiles (Buildable Areas)**
- ✅ **Can build turrets** on these tiles
- ✅ **Can walk to the edge** of white tiles
- ✅ **Can walk into gray abyss** from white tile edges (but not past boundaries)
- ❌ **Cannot walk over** white tiles (they're solid ground for building)

### **Black Tiles (Wave Paths)**
- ✅ **Waves move down** these tiles
- ✅ **Players can walk over** black tiles
- ❌ **Cannot build turrets** on black tiles
- ✅ **Part of the playable area** (no boundaries here)

### **Gray Abyss (Outside Map)**
- ✅ **New color**: `#32283e` (dark purple-gray)
- ✅ **Walkable from white tile edges** (limited distance)
- ❌ **Hard boundary** - cannot go infinitely into abyss
- ❌ **Cannot build** in the abyss

---

## 📋 **Phase 1: Tilemap Analysis & Setup** ✅ **COMPLETED**

### Understanding Current Implementation
- [x] Examine current `Level01.tscn` tilemap structure
- [x] Identify current tile indices for black/white tiles
- [x] Document current map dimensions and layout
- [x] Check existing BuildingZoneValidator integration

**Files examined:**
- ✅ `scenes/Levels/Level01.tscn` (found correct path)
- ✅ `scenes/Core/Main.tscn` (references Level01)
- ✅ `src/Presentation/Systems/BuildingZoneValidator.cs`
- ✅ `assets/sprites/basic_colors_tilesheet_16x16.png`

## 🔍 **Analysis Results:**

### **Current Tilemap Structure**
- **Scene**: `scenes/Levels/Level01.tscn` contains `GroundLayer` (TileMapLayer)
- **Referenced in**: `scenes/Core/Main.tscn` as `Level01` instance
- **Tileset**: 3 sources with `basic_colors_tilesheet_16x16.png` as source 0
- **Map Size**: 70x47 tiles (comprehensive coverage)

### **Tile Layout Analysis**
- **Source 0**: Basic colors tileset (64x32px = 4x2 grid)
  - **Position (0,0)**: Black tile (atlas coords 0,0) → **WAVE PATHS**
  - **Position (1,0)**: White tile (atlas coords 1,0) → **BUILDABLE AREAS**
  - **Position (2,0)**: Unknown color (atlas coords 2,0)
  - **Position (3,0)**: Unknown color (atlas coords 3,0)

### **Current Building System**
- **BuildingZoneValidator** correctly integrated with TileMapLayer
- **Logic**: `isLaneTile = (atlasCoords.X == 0 && atlasCoords.Y == 0)` → Black tiles
- **Can build**: `!isLaneTile` → NOT on black tiles (correct!)
- **Validation**: World position → tile coords → atlas coords check

### **Map Pattern Analysis**
- **Black tiles (0,0)**: Form clear paths through the map
- **White tiles (1,0)**: Fill most buildable areas
- **Mixed layout**: Strategic path network with ample building space
- **No background color**: Currently gray void around edges

### **Key Findings**
- ✅ **Tile indices confirmed**: Black=source 0, atlas (0,0) | White=source 0, atlas (1,0)
- ✅ **Building logic works**: Already prevents building on black tiles (wave paths)
- ✅ **Map dimensions**: 70x47 = 3,290 tiles total
- ❌ **Missing boundary enforcement**: No limits on camera/movement outside map
- ❌ **Gray background**: Default Godot background instead of `#32283e`
- ✅ **Integration ready**: Main.cs properly initializes BuildingZoneValidator with Level01

### **Recommended Next Steps**
1. **Phase 2 (Background)**: Quick win - change gray to `#32283e`
2. **Phase 3 (Boundaries)**: Add map edge detection and movement limits
3. **Phase 4 (Building)**: Enhance current system with visual feedback

---

## 📋 **Phase 2: Background Color Implementation** ✅ **COMPLETED**

### Scene Background Color
- [x] Add background color `#32283e` to main scene
- [x] Ensure background shows around tilemap edges
- [x] Test background visibility in different screen sizes
- [x] Verify color consistency across the game

**Implementation Used:**
✅ **Camera2D Background**: Set background_color on Player's Camera2D
✅ **Project Default**: Added default_clear_color to project.godot

**Files modified:**
- ✅ `scenes/Player/Player.tscn` - Added background_color to Camera2D
- ✅ `project.godot` - Added environment/defaults/default_clear_color

## 🎨 **Implementation Details:**

### **Background Color Applied**
- **Color**: `#32283e` (dark purple-gray)
- **RGB Values**: (50, 40, 62)
- **Normalized**: Color(0.196078, 0.156863, 0.243137, 1)
- **Coverage**: Camera2D renders this color behind all game content

### **Dual Implementation Strategy**
1. **Camera2D background_color**: Primary background for game view
2. **Project default_clear_color**: Fallback for any other viewports/cameras

### **Expected Results**
- ✅ **Gray abyss replaced**: Professional dark purple-gray background
- ✅ **Consistent appearance**: Same color across all screen sizes
- ✅ **Seamless integration**: Background visible around tilemap edges
- ✅ **Performance optimized**: Built-in Godot rendering, no extra nodes needed

---

## 📋 **Phase 3: Boundary System Implementation**

### Map Boundary Detection
- [ ] Create `MapBoundaryService` class
- [ ] Implement boundary detection based on tilemap edges
- [ ] Define walkable buffer zone around white tiles
- [ ] Add hard boundary limits to prevent infinite walking

### Boundary Rules Engine
- [ ] Implement `CanWalkToPosition(Vector2 position)` method
- [ ] Add boundary validation for player movement
- [ ] Integrate with existing building validation system
- [ ] Add visual feedback for boundary violations

**Files to create:**
- `src/Infrastructure/Map/Services/MapBoundaryService.cs`
- `src/Domain/Map/Services/IMapBoundaryService.cs`

**Files to modify:**
- Player movement logic (if exists)
- `BuildingZoneValidator.cs` - integrate boundary checks

---

## 📋 **Phase 4: Building System Integration**

### Enhanced Building Validation
- [ ] Update `BuildingZoneValidator` to use new boundary system
- [ ] Ensure turrets can only be built on white tiles
- [ ] Prevent building in black tiles (wave paths)
- [ ] Prevent building in abyss/boundary areas
- [ ] Add clear visual feedback for invalid build locations

### Building Zone Visual Feedback
- [ ] Add building preview with valid/invalid indicators
- [ ] Show buildable areas when in build mode
- [ ] Highlight wave paths as non-buildable
- [ ] Add boundary zone visualization

**Files to modify:**
- `src/Presentation/Systems/BuildingZoneValidator.cs`
- Building preview system (if exists)
- UI feedback systems

---

## 📋 **Phase 5: Player Movement Integration**

### Movement Boundary Enforcement
- [ ] Identify current player movement system
- [ ] Integrate boundary checks with player movement
- [ ] Add smooth boundary collision/stopping
- [ ] Implement walkable buffer zone around map edges

### Movement Rules Implementation
- [ ] Allow walking over black tiles (wave paths)
- [ ] Prevent walking over white tiles (building areas)
- [ ] Allow limited movement into abyss from white tile edges
- [ ] Add hard boundary limits

**Files to examine/modify:**
- Player character movement scripts
- Input handling for player movement
- Collision detection systems

---

## 📋 **Phase 6: Wave Path Validation**

### Wave Movement Constraints
- [ ] Ensure waves can only move on black tiles
- [ ] Validate wave spawn points are on black tiles
- [ ] Confirm wave end points are on black tiles
- [ ] Add error handling for invalid wave paths

### Path Validation
- [ ] Update `PathService` to respect tile-based constraints
- [ ] Ensure wave paths don't conflict with building areas
- [ ] Add validation for wave configuration files

**Files to modify:**
- `src/Infrastructure/Enemies/Services/PathService.cs`
- Wave configuration validation
- Enemy movement logic

---

## 🔧 **Technical Implementation Details**

### Tilemap Integration
```csharp
public class MapBoundaryService
{
    private TileMapLayer _tileMapLayer;
    
    public bool CanBuildAt(Vector2 position)
    {
        // Check if position is on white tile
        // Ensure not in boundary buffer zone
    }
    
    public bool CanWalkTo(Vector2 position)
    {
        // Allow walking on black tiles
        // Allow limited walking in abyss from white edges
        // Prevent infinite abyss exploration
    }
}
```

### Color Constants
```csharp
public static class MapColors
{
    public static readonly Color ABYSS_BACKGROUND = new Color("#32283e");
    public static readonly int WHITE_TILE_INDEX = 1; // Buildable
    public static readonly int BLACK_TILE_INDEX = 0; // Wave paths
}
```

---

## ⚡ **Success Criteria**
- [ ] Background shows `#32283e` color around map edges
- [ ] Turrets can only be built on white tiles
- [ ] Waves move properly on black tiles
- [ ] Players can walk over black tiles but not white tiles
- [ ] Limited abyss exploration from white tile edges
- [ ] Hard boundaries prevent infinite movement
- [ ] Clear visual feedback for all boundary violations
- [ ] Building system respects all tile-based rules

---

## 🎮 **User Experience Goals**
- **Clear Visual Boundaries**: Players immediately understand playable area
- **Intuitive Building**: Obvious where turrets can/cannot be placed
- **Smooth Movement**: Natural movement within defined boundaries
- **Professional Appearance**: Polished background instead of gray void
- **Consistent Rules**: Tile-based rules are clearly communicated

---

## 📂 **File Structure After Implementation**
```
src/
├── Domain/Map/
│   └── Services/IMapBoundaryService.cs
├── Infrastructure/Map/
│   └── Services/MapBoundaryService.cs
└── Presentation/Systems/
    └── BuildingZoneValidator.cs (updated)

scenes/
└── Level01.tscn (updated background)
```

---

## 🚀 **Implementation Priority**
1. **Phase 2** (Background) - Quick visual improvement
2. **Phase 1** (Analysis) - Understand current system
3. **Phase 3** (Boundaries) - Core functionality
4. **Phase 4** (Building) - Essential game mechanics
5. **Phase 5** (Movement) - If player movement exists
6. **Phase 6** (Waves) - Validate against existing system
