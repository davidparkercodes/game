# GameSimRunner_Plaan2 - Plan to Integrate Console with Real Simulation

## Objective
Fix the Clean Architecture violations by removing Godot dependencies from Domain/Application layers, then integrate the cleaned GameSimRunner with a console interface for meaningful balance testing with visual feedback.

## Key Goals
- [x] **Clean Architecture**: Remove all Godot dependencies from Domain/Application layers
- [ ] **Accurate Simulation**: Leverage real game logic for simulation.
- [ ] **Visual Feedback**: Continue using ASCII progress bars with Spectre.Console for intuitive visual feedback.
- [ ] **Config-Driven**: Ensure all simulations use existing JSON configuration files for stats and scenarios.
- [ ] **Scalability**: Allow for batch processing and different output modes.

## Plan

### 0. Architecture Cleanup (CRITICAL FIRST STEP)
- [x] Remove Godot dependencies from Domain/Application layers
- [x] Replace `Godot.Vector2` with clean `Position` value object in Domain
- [x] Clean up Application Commands to use Domain types only
- [x] Ensure GameSimRunner has zero Infrastructure dependencies
- [x] Verify clean separation: Domain → Application → Infrastructure

### 1. Integrate Real GameSimRunner
- [x] Import the clean `GameSimRunner` logic from `src/Application/Simulation/GameSimRunner.cs` into the standalone console app.
- [x] Ensure that the real logic is executed for both scenario and balance-testing modes.

### 2. Update Console App
- [ ] Modify `GameSimRunner.Standalone` to integrate with the real `GameSimRunner`.
- [ ] Replace the fake simulation with actual waves processing, tower strategy, and enemy defeats.
- [ ] Integrate progress reporting to reflect real-time stats, including gold and lives.

### 3. Configuration
- [ ] Ensure the console app reads building stats and enemy stats from JSON config files located in `data/simulation/`.
- [ ] Implement a fallback mechanism for missing or corrupt configurations.

### 4. User Experience Improvements
- [ ] Provide detailed wave-by-wave breakdown in verbose mode.
- [ ] Ensure minimal output mode retains essential simulation results.
- [ ] Add error handling and feedback for configuration issues or simulation errors.

### 5. Testing
- [ ] Update existing unit tests to validate console outputs and new features.
- [ ] Add integration tests for the console to confirm accurate simulation execution and results.

## Success Criteria
- [ ] Real GameSimRunner simulation runs successfully with the console interface.
- [ ] Accurate reflection of real-time stats via progress bars.
- [ ] Config-driven approach confirmed through JSON stats files.
- [ ] Positive user feedback from intuitive, detailed visual representation.

---

## Next Steps
- [ ] Begin development for the console integration by setting up the project files.
- [ ] Ensure team alignment with the outlined plan.

## Timeline
- [ ] **Week 1**: Setup and integration of real GameSimRunner
- [ ] **Week 2**: UI/UX improvements and detailed testing
- [ ] **Week 3**: Review and refinement based on feedback

