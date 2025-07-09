# Vaporwave Glow Overlay Implementation Plan

## Goal
Apply a full-screen "vaporwave glow" lighting overlay in our Godot 4 project (C#). Classic vaporwave hues (hot-pink #ff71ce, electric-cyan #01cdfe, deep-purple #b967ff, sunset-orange #ff8b5c). It should tint everything in the current viewport, not depend on in-world lights, and be easy to toggle/adjust later.

## Acceptance Criteria
1. Runs in a 2D scene (CanvasLayer) and automatically fills any resolution
2. Uses additive blending so original sprites remain visible but "washed" by the gradient
3. Gradient direction: vertical (purple/orange at bottom → pink/cyan at top). Slight 10–15 s slow horizontal pan to keep the scene alive
4. Code in C#; no external assets—gradient generated procedurally
5. Provide a public SetPalette(Color[] colors) method so we can swap palettes at runtime
6. FPS-safe: no heavy shaders, just a simple CanvasItem shader or ColorRect with GradientTexture
7. Must survive scene changes (lives on an autoload singleton or a top-level CanvasLayer)

## Execution Instructions

**Process**: Execute phases one at a time. When a phase is complete:

1. Update this plan file to mark completed items
2. Run `dotnet clean && dotnet build && dotnet test`
3. If all pass, proceed to next phase
4. If warnings or errors occur, fix them before proceeding
5. Continue until all phases are complete

---

## Phase 1: Project Setup and Core Structure
- [ ] Create VaporwaveGlowOverlay.cs singleton class
- [ ] Add VaporwaveGlowOverlay to autoload in project.godot
- [ ] Create base CanvasLayer structure with proper layer ordering
- [ ] Set up ColorRect node for full-screen coverage
- [ ] Implement viewport size detection and automatic scaling
- [ ] Add basic initialization and cleanup methods

## Phase 2: Gradient Generation System
- [ ] Create procedural gradient generation using GradientTexture
- [ ] Implement default vaporwave color palette (hot-pink, electric-cyan, deep-purple, sunset-orange)
- [ ] Set up vertical gradient direction (purple/orange bottom → pink/cyan top)
- [ ] Add gradient texture assignment to ColorRect
- [ ] Test gradient appearance and color accuracy

## Phase 3: Additive Blending Implementation
- [ ] Configure ColorRect material with additive blend mode
- [ ] Test that original sprites remain visible but "washed" by gradient
- [ ] Fine-tune blend intensity for optimal visual effect
- [ ] Ensure gradient doesn't overpower underlying content
- [ ] Validate performance impact of blending

## Phase 4: Animation System
- [ ] Implement slow horizontal pan animation (10-15 seconds cycle)
- [ ] Create smooth, continuous movement using Tween
- [ ] Add texture UV offset manipulation for panning effect
- [ ] Test animation smoothness and performance
- [ ] Ensure animation loops seamlessly

## Phase 5: Runtime Palette Control
- [ ] Implement SetPalette(Color[] colors) public method
- [ ] Add palette validation and error handling
- [ ] Create smooth transition between palettes
- [ ] Test with different color combinations
- [ ] Add method to reset to default vaporwave palette

## Phase 6: Scene Persistence and Management
- [ ] Ensure overlay persists across scene changes
- [ ] Add proper cleanup for scene transitions
- [ ] Implement Show/Hide toggle functionality
- [ ] Add overlay enable/disable state management
- [ ] Test with multiple scene transitions

## Phase 7: Performance Optimization
- [ ] Verify FPS performance with overlay active
- [ ] Optimize gradient texture size for best performance/quality ratio
- [ ] Minimize shader complexity
- [ ] Add performance monitoring hooks
- [ ] Test on different resolutions and devices

## Phase 8: Testing and Validation
- [ ] Create test scene to validate all functionality
- [ ] Test all acceptance criteria thoroughly
- [ ] Validate resolution independence
- [ ] Test palette swapping functionality
- [ ] Verify scene persistence works correctly
- [ ] Performance benchmarking

## Phase 9: Documentation and Finalization
- [ ] Add inline code documentation
- [ ] Create usage examples
- [ ] Document public API methods
- [ ] Add troubleshooting notes
- [ ] Final code review and cleanup

---

## Technical Notes

### Color Palette Values
- Hot Pink: #ff71ce (255, 113, 206)
- Electric Cyan: #01cdfe (1, 205, 254)
- Deep Purple: #b967ff (185, 103, 255)
- Sunset Orange: #ff8b5c (255, 139, 92)

### Key Implementation Details
- Use CanvasLayer with high layer value for overlay positioning
- GradientTexture for procedural gradient generation
- ColorRect with BLEND_ADD material for additive blending
- Tween for smooth animation cycles
- Singleton pattern for global access and persistence

### Performance Considerations
- Single ColorRect with simple gradient texture
- Minimal shader usage
- Efficient UV animation instead of texture recreation
- Proper resource management for scene transitions
