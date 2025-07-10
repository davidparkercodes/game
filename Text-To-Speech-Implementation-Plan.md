# Text-to-Speech Implementation Plan

## Execution Instructions

**Process**: Execute phases one at a time. When a phase is complete:

1. Update this plan file to mark completed items
2. Run `dotnet clean && dotnet build && dotnet test`
3. If all pass, proceed to next phase
4. If warnings or errors occur, fix them before proceeding
5. Continue until all phases are complete

## Project Overview

Implement text-to-speech functionality using Godot's TTS layer with macOS default male voice, enhanced with robotic/AI effects while maintaining human-like qualities.

## Phase 1: Foundation and Research
- [ ] Research Godot 4.4 TTS capabilities and API
- [ ] Investigate macOS text-to-speech system integration
- [ ] Analyze existing audio architecture (`ISoundService`, `SoundService`)
- [ ] Document macOS voice options and parameters
- [ ] Test basic TTS functionality outside of game

## Phase 2: Domain Layer Extensions
- [ ] Create `ITtsService` interface in `Domain/Audio/Services/`
- [ ] Add TTS-specific value objects in `Domain/Audio/ValueObjects/`
  - [ ] `TtsRequest` - encapsulates text, voice settings, effects
  - [ ] `VoiceConfiguration` - voice type, speed, pitch settings
  - [ ] `RoboticEffectSettings` - filter parameters for AI voice
- [ ] Add TTS-related enums in `Domain/Audio/Enums/`
  - [ ] `VoiceType` - male, female, robotic variants
  - [ ] `TtsCategory` - dialogue, ui feedback, narration
- [ ] Update `SoundCategory` enum to include TTS categories

## Phase 3: Infrastructure Implementation
- [ ] Create `TtsService` class in `Infrastructure/Sound/`
- [ ] Implement macOS system voice integration
  - [ ] Use `say` command or native macOS Speech API
  - [ ] Configure default male voice parameters
  - [ ] Implement voice speed and pitch controls
- [ ] Add audio processing pipeline for robotic effects
  - [ ] Low-pass/high-pass filters
  - [ ] Slight vocoder or chorus effects
  - [ ] Subtle reverb for AI ambiance
  - [ ] Pitch modulation for robotic character

## Phase 4: Audio Effects and Processing
- [ ] Create `AudioEffectProcessor` for voice manipulation
- [ ] Implement Godot AudioEffect nodes for voice processing
  - [ ] `AudioEffectFilter` for frequency shaping
  - [ ] `AudioEffectChorus` for subtle robotic texture
  - [ ] `AudioEffectReverb` for AI ambiance
  - [ ] `AudioEffectPitchShift` for robotic modulation
- [ ] Create presets for different robotic voice styles
- [ ] Ensure effects maintain speech intelligibility

## Phase 5: Integration with Existing Audio System
- [ ] Extend `SoundService` to support TTS playback
- [ ] Update `ISoundService` interface with TTS methods
- [ ] Integrate TTS with existing audio player management
- [ ] Ensure TTS respects volume categories and settings
- [ ] Add TTS to dependency injection container in `Di/`

## Phase 6: Presentation Layer Implementation
- [ ] Create TTS component for game objects
- [ ] Add TTS support to UI systems
- [ ] Implement dialogue system with TTS
- [ ] Create debug commands for TTS testing
- [ ] Add TTS controls to existing HUD if needed

## Phase 7: Configuration and Customization
- [ ] Create TTS configuration files
- [ ] Add TTS settings to game options
- [ ] Implement runtime voice effect adjustments
- [ ] Create voice presets (normal AI, alert AI, friendly AI)
- [ ] Add TTS volume controls separate from other audio

## Phase 8: Testing and Validation
- [ ] Create unit tests for TTS domain logic
- [ ] Create integration tests for TTS service
- [ ] Test voice effect quality and intelligibility
- [ ] Test performance impact of real-time voice processing
- [ ] Validate TTS works across different macOS versions

## Phase 9: Game Integration Examples
- [ ] Add TTS to enemy AI announcements
- [ ] Implement TTS for tower/building feedback
- [ ] Add TTS for round/wave announcements
- [ ] Create TTS for tutorial instructions
- [ ] Add TTS for achievement notifications

## Phase 10: Polish and Optimization
- [ ] Optimize TTS processing performance
- [ ] Add TTS caching for repeated phrases
- [ ] Fine-tune robotic effects for best quality
- [ ] Add fallback handling for TTS failures
- [ ] Implement TTS accessibility options

## Technical Considerations

### Voice Processing Pipeline
1. **Text Input** → macOS TTS Engine → **Raw Audio**
2. **Raw Audio** → Godot Audio Processing → **Effect Chain**
3. **Effect Chain** → Audio Bus → **Final Output**

### Robotic Effect Settings
- **Frequency Range**: Slightly compressed (200Hz - 8kHz)
- **Chorus**: Subtle, 0.1-0.2 depth for texture
- **Pitch Modulation**: ±5-10 cents variation
- **Reverb**: Small room, short decay for presence
- **EQ**: Slight mid-range boost, gentle high-cut

### Integration Points
- Extend `SoundCategory` with `TTS` and `VoiceDialogue`
- Add TTS methods to `ISoundService`
- Create `TtsRequest` similar to `SoundRequest`
- Use existing audio player pool for TTS playback

## Files to Create/Modify

### New Files
- `src/Domain/Audio/Services/ITtsService.cs`
- `src/Domain/Audio/ValueObjects/TtsRequest.cs`
- `src/Domain/Audio/ValueObjects/VoiceConfiguration.cs`
- `src/Domain/Audio/ValueObjects/RoboticEffectSettings.cs`
- `src/Domain/Audio/Enums/VoiceType.cs`
- `src/Domain/Audio/Enums/TtsCategory.cs`
- `src/Infrastructure/Sound/TtsService.cs`
- `src/Infrastructure/Sound/AudioEffectProcessor.cs`
- `tests/Domain/Audio/Services/TtsServiceTests.cs`
- `tests/Infrastructure/Sound/TtsServiceIntegrationTests.cs`

### Modified Files
- `src/Domain/Audio/Enums/SoundCategory.cs`
- `src/Domain/Audio/Services/ISoundService.cs`
- `src/Infrastructure/Sound/SoundService.cs`
- `src/Di/DiConfiguration.cs`

## Success Criteria
- [ ] TTS produces clear, intelligible speech
- [ ] Robotic effects enhance AI character without hindering comprehension
- [ ] TTS integrates seamlessly with existing audio system
- [ ] Performance impact is minimal (< 5ms latency for short phrases)
- [ ] All tests pass and build succeeds
- [ ] TTS works reliably on macOS with default male voice
