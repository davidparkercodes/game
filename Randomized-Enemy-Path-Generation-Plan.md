# Randomized Enemy Path Generation Plan

## Execution Instructions

**Process**: Execute phases one at a time. When a phase is complete:

1. Update this plan file to mark completed items
2. Run `dotnet clean && dotnet build && dotnet test`
3. If all pass, proceed to next phase
4. If warnings or errors occur, fix them before proceeding
5. Continue until all phases are complete

---

## 🎯 **Objective**

Implement randomized enemy pathfinding that generates dynamic paths through the tilemap, replacing the current static path system with procedurally generated routes that provide variety while maintaining game balance.

---

## 📋 **Phase 1: Analysis & Architecture Design** 🔍

### **[ ] 1.1 Analyze Existing Systems**
- [ ] Review current `PathManager` (`src/Application/Enemies/Services/PathManager.cs`) interface and implementation
- [ ] Study `PathService` (`src/Infrastructure/Enemies/Services/PathService.cs`) hardcoded path points
- [ ] Examine `GodotPathManager` (`src/Infrastructure/Enemies/GodotPathManager.cs`) visualization system
- [ ] Analyze `LevelDataResource` structure (`data/levels/level_01.tres`) for path storage
- [ ] Understand tilemap structure: lane tile (0,0) vs buildable tile (1,0) in `basic_colors_tilesheet_16x16.png`
- [ ] Document current Level01 -> GroundLayer tilemap dimensions and constraints

### **[ ] 1.2 Define Path Generation Requirements**
- [ ] Set path length constraints (min: 10 tiles, max: 25 tiles, configurable)
- [ ] Define start/end point rules (must begin/end at map edges)
- [ ] Establish movement rules (4-directional only, no diagonal movement)
- [ ] Define path validity constraints (no overlapping, must be traversable)
- [ ] Specify integration points with existing `IPathManager` interface
- [ ] Document seed-based generation for reproducible paths

### **[ ] 1.3 Choose Generation Algorithm**
- [ ] Research pathfinding algorithms suitable for random generation (A* with randomness, recursive backtracking, random walk with bias)
- [ ] Select algorithm: **Biased Random Walk with A* fallback** for balance of randomness and reachability
- [ ] Design algorithm parameters: bias strength, randomness factor, maximum attempts
- [ ] Plan fallback strategy for impossible path scenarios
- [ ] Document algorithm complexity and performance expectations

---

## 📋 **Phase 2: Core Path Generator Implementation** ⚙️

### **[ ] 2.1 Create Path Generator Service**
- [ ] Create `src/Domain/Enemies/Services/IPathGenerator.cs` interface
- [ ] Implement `src/Application/Enemies/Services/PathGenerator.cs` core service
- [ ] Add `GenerateRandomPath(int targetLength, Vector2 startPoint, Vector2 endPoint, int? seed = null)` method
- [ ] Implement `ValidatePath(IReadOnlyList<Vector2> pathPoints)` validation method
- [ ] Add `GetAvailableStartPoints()` and `GetAvailableEndPoints()` for edge detection
- [ ] Include proper error handling and logging integration

### **[ ] 2.2 Implement Generation Algorithm**
- [ ] Create `BiasedRandomWalkGenerator` class with configurable parameters
- [ ] Implement tile validation (check for (0,0) lane tiles vs (1,0) buildable tiles)
- [ ] Add path smoothing and optimization post-processing
- [ ] Implement A* fallback for unreachable scenarios
- [ ] Add retry logic with different seeds for failed generations
- [ ] Ensure generated paths respect existing tilemap boundaries

### **[ ] 2.3 Add Configuration System**
- [ ] Create `PathGenerationConfig` class for algorithm parameters
- [ ] Add configuration properties: `MinPathLength`, `MaxPathLength`, `BiasStrength`, `RandomnessFactor`
- [ ] Integrate with existing level data structure
- [ ] Add validation for configuration parameters
- [ ] Support runtime configuration changes

---

## 📋 **Phase 3: Tilemap Integration** 🗺️

### **[ ] 3.1 Tilemap Path Painter**
- [ ] Create `src/Infrastructure/Levels/TilemapPathPainter.cs` utility
- [ ] Implement `ClearExistingPath(TileMapLayer groundLayer)` method
- [ ] Add `PaintPathToTilemap(IReadOnlyList<Vector2> pathPoints, TileMapLayer groundLayer)` method
- [ ] Ensure proper tile index usage: (0,0) for lane, (1,0) for buildable areas
- [ ] Add validation to prevent painting outside tilemap bounds
- [ ] Include undo/redo functionality for editor integration

### **[ ] 3.2 Update Level Loading System**
- [ ] Modify `LevelDataResource` to include path generation parameters
- [ ] Update level loading to trigger path generation when needed
- [ ] Ensure generated paths are stored in `PathPoints` array for consistency
- [ ] Add versioning support for backward compatibility with static paths
- [ ] Update `level_01.tres` with new generation parameters

### **[ ] 3.3 Visual Verification**
- [ ] Test path rendering in Godot editor using existing `GodotPathManager`
- [ ] Verify path visibility with proper z-index ordering
- [ ] Ensure generated paths appear correctly in both editor and runtime
- [ ] Add debug visualization for path generation process
- [ ] Test with different tilemap sizes and configurations

---

## 📋 **Phase 4: Game System Integration** 🎮

### **[ ] 4.1 Update PathService Integration**
- [ ] Modify `PathService.cs` to use dynamically generated paths instead of hardcoded points
- [ ] Update `GetPathPoints()` method to return generated path
- [ ] Ensure `GetPathPosition(float progress)` works with variable-length paths
- [ ] Update `GetPathDirection(float progress)` for new path structure
- [ ] Maintain backward compatibility with existing enemy movement code

### **[ ] 4.2 Building System Integration**
- [ ] Update building placement validation to respect new lane tiles
- [ ] Ensure `BuildingManager` recognizes dynamically painted lane tiles as unbuildable
- [ ] Add real-time validation when path changes during gameplay
- [ ] Update UI feedback for invalid building placement
- [ ] Test building placement with various generated path configurations

### **[ ] 4.3 Wave and Enemy Spawning**
- [ ] Verify `WaveSpawner` works correctly with generated paths
- [ ] Test enemy pathfinding with various path lengths and complexities
- [ ] Ensure spawning and despawning works at generated start/end points
- [ ] Update enemy movement smoothness for potentially more complex paths
- [ ] Add performance monitoring for enemy navigation on generated paths

---

## 📋 **Phase 5: Configuration & Polishing** ✨

### **[ ] 5.1 Runtime Configuration**
- [ ] Add path generation settings to game configuration system
- [ ] Create UI controls for path generation parameters (if needed)
- [ ] Implement save/load for user preferences
- [ ] Add preset configurations for different difficulty levels
- [ ] Enable runtime path regeneration with new parameters

### **[ ] 5.2 Editor Tools Enhancement**
- [ ] Add editor button to regenerate path manually
- [ ] Create inspector controls for path generation parameters
- [ ] Add path generation preview without applying changes
- [ ] Implement path generation history for undo/redo
- [ ] Add validation warnings for invalid parameter combinations

### **[ ] 5.3 Performance Optimization**
- [ ] Profile path generation algorithm performance
- [ ] Optimize memory allocations during generation
- [ ] Add caching for repeated generations with same parameters
- [ ] Implement background generation for smoother UI experience
- [ ] Monitor impact on game startup and level loading times

---

## 📋 **Phase 6: Testing & Quality Assurance** 🧪

### **[ ] 6.1 Unit Testing**
- [ ] Create comprehensive test suite for `PathGenerator` class
- [ ] Test edge cases: minimum length paths, maximum length paths, impossible scenarios
- [ ] Verify seed-based reproducibility across multiple runs
- [ ] Test path validation logic with various invalid inputs
- [ ] Add performance benchmarks for generation algorithms
- [ ] Test tilemap integration with different tile configurations

### **[ ] 6.2 Integration Testing**
- [ ] Test complete path generation to enemy movement pipeline
- [ ] Verify building placement validation with generated paths
- [ ] Test level loading and saving with generated path data
- [ ] Ensure proper integration with existing `PathManager` interface
- [ ] Test editor functionality with path generation tools
- [ ] Verify visual rendering consistency across different scenarios

### **[ ] 6.3 Gameplay Testing**
- [ ] Playtest multiple sessions with different generated paths
- [ ] Verify game balance with variable path lengths and complexities
- [ ] Test for potential soft-locks or unreachable scenarios
- [ ] Ensure enemy AI performs well on all generated path types
- [ ] Validate player building strategies remain viable
- [ ] Test performance under various path generation configurations

---

## 📋 **Phase 7: Documentation & Finalization** 📚

### **[ ] 7.1 Code Documentation**
- [ ] Add comprehensive XML documentation to all new classes
- [ ] Document algorithm choices and implementation decisions
- [ ] Create usage examples for path generation API
- [ ] Update existing documentation to reflect new path system
- [ ] Add troubleshooting guide for common generation issues

### **[ ] 7.2 User Documentation**
- [ ] Create user guide for path generation features
- [ ] Document configuration options and their effects
- [ ] Add best practices for path generation parameters
- [ ] Include visual examples of different path types
- [ ] Create migration guide from static to generated paths

### **[ ] 7.3 Final Verification**
- [ ] Run complete test suite: `dotnet clean && dotnet build && dotnet test`
- [ ] Verify zero build warnings and errors
- [ ] Confirm all planned features are implemented and working
- [ ] Test deployment build for any production issues
- [ ] Prepare release notes and feature summary

---

## 🔧 **Technical Implementation Notes**

### **Key Files to Create/Modify:**
- `src/Domain/Enemies/Services/IPathGenerator.cs` - New interface
- `src/Application/Enemies/Services/PathGenerator.cs` - Core implementation
- `src/Infrastructure/Levels/TilemapPathPainter.cs` - Tilemap integration
- `src/Infrastructure/Enemies/Services/PathService.cs` - Update for dynamic paths
- `data/levels/level_01.tres` - Add generation parameters
- `tests/Unit/Enemies/PathGeneratorTests.cs` - Comprehensive test suite

### **Integration Points:**
- `IPathManager` interface - Ensure compatibility
- `GodotPathManager` - Visual rendering of generated paths
- `LevelDataResource` - Storage of generation parameters
- `BuildingManager` - Validation against generated lane tiles
- `WaveSpawner` - Enemy spawning on generated paths

### **Algorithm Considerations:**
- **Biased Random Walk**: Provides good randomness while trending toward target
- **A* Fallback**: Ensures reachability when random walk fails
- **Seed-based Generation**: Enables reproducible paths for testing and sharing
- **Performance Target**: < 50ms generation time for typical path lengths

---

## 🚨 **Risk Mitigation**

### **Potential Issues:**
1. **Impossible Path Scenarios**: Mitigated by A* fallback and multiple retry attempts
2. **Performance Impact**: Addressed through profiling and optimization in Phase 5
3. **Game Balance**: Controlled through configurable parameters and playtesting
4. **Backward Compatibility**: Maintained through versioning and gradual migration
5. **Visual Inconsistency**: Prevented through comprehensive rendering tests

### **Fallback Strategies:**
- Revert to static paths if generation fails repeatedly
- Provide manual path editing tools for impossible scenarios
- Include curated "good" paths as generation starting points
- Allow mixing static and generated path elements

---

## ✅ **Success Criteria**

- [ ] Generated paths are visually distinct and interesting across multiple generations
- [ ] Path generation completes reliably within performance targets
- [ ] Building placement system correctly respects generated lane tiles
- [ ] Enemy movement is smooth and natural on all generated paths
- [ ] Game balance is maintained across different path configurations
- [ ] System integrates seamlessly with existing codebase
- [ ] All tests pass with zero warnings or errors
- [ ] Editor tools provide intuitive path generation workflow

---

## 📈 **Estimated Timeline**

- **Phase 1**: 4-6 hours (analysis and design)
- **Phase 2**: 8-10 hours (core implementation)
- **Phase 3**: 6-8 hours (tilemap integration)
- **Phase 4**: 6-8 hours (game system integration)
- **Phase 5**: 4-6 hours (configuration and polish)
- **Phase 6**: 8-10 hours (testing and QA)
- **Phase 7**: 4-6 hours (documentation)

**Total Estimated Time**: 40-54 hours

---

This plan provides a comprehensive approach to implementing randomized enemy pathfinding while respecting your existing architecture and ensuring robust integration with the current game systems.
