# Mouse Scroll Wheel Zoom Feature

## Overview
The game now supports mouse scroll wheel zooming functionality that allows players to zoom in and out of the game world for better visibility and control.

## Controls

### Mouse Controls
- **Scroll Wheel Up**: Zoom in (get closer to the action)
- **Scroll Wheel Down**: Zoom out (see more of the game world)

### Keyboard Controls (Alternative)
- **+ (Plus) Key**: Zoom in
- **- (Minus) Key**: Zoom out
- **0 (Zero) Key**: Reset zoom to default level (0.7x)

## Zoom Configuration
- **Default Zoom**: 0.7x (preferred strategic view)
- **Minimum Zoom**: 0.5x (zoomed out, see more)
- **Maximum Zoom**: 3.0x (zoomed in, see details)
- **Zoom Speed**: 0.1x per scroll/keypress
- **Smooth Interpolation**: Camera smoothly transitions between zoom levels

## Technical Implementation

### Camera Integration
- The zoom functionality is integrated into the Player's Camera2D
- Smooth interpolation ensures seamless zoom transitions
- Zoom level is maintained throughout gameplay

### Input Priority
- Zoom input is processed before building placement and tower selection
- Works independently of other game systems
- Non-blocking implementation

### Performance
- Efficient real-time zoom updates in `_PhysicsProcess`
- Clamped zoom values prevent excessive zooming
- Smooth lerp interpolation for professional feel

## Usage Examples

### Basic Zoom
1. Use mouse scroll wheel to zoom in/out while playing
2. Zoom in to see tower details and precise placement
3. Zoom out to get strategic overview of the battlefield

### Keyboard Shortcuts
1. Press `+` to zoom in step by step
2. Press `-` to zoom out gradually
3. Press `0` to quickly return to default zoom (0.7x)

## Developer Notes

### Customization
The zoom settings can be easily adjusted in the Player.cs file:
- `ZoomSpeed`: Controls how fast zoom changes per input
- `MinZoom`/`MaxZoom`: Sets zoom limits
- `ZoomSmoothSpeed`: Controls interpolation speed

### Integration
- Zoom functionality is self-contained in the Player class
- No dependencies on other systems
- Easy to extend with additional zoom features

### Future Enhancements
- Could add zoom-to-mouse-position functionality
- Could integrate with minimap for zoom level indication
- Could add zoom presets for different strategic views
