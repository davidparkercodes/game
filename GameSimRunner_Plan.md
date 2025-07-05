# 🧪 GameSimRunner Plan

## 🎯 Overview
Build a headless, deterministic game simulation that uses only Domain and Application layers to validate game mechanics, balance, and architectural separation.

## 🏗️ Architecture Goals

### ✅ What We're Proving
- **Clean Architecture Works**: Game logic is independent of Godot/UI
- **Domain Isolation**: Core game rules work without infrastructure
- **Testable Design**: Can run automated balance/regression tests
- **Fast Feedback**: Stat changes can be validated without engine startup

### 🚫 What We're Avoiding
- Any Godot dependencies
- Visual rendering
- Real-time constraints
- Infrastructure layer usage

## 📋 Core Components

### 1. **GameSimRunner.cs** - Main Entry Point
```csharp
// Location: src/Application/Simulation/GameSimRunner.cs
public class GameSimRunner
{
    public SimulationResult RunSimulation(SimulationConfig config)
    public Task<SimulationResult> RunSimulationAsync(SimulationConfig config)
    public void RunMultipleScenarios(List<SimulationScenario> scenarios)
}
```

### 2. **SimulationConfig.cs** - Configuration
```csharp
// Location: src/Application/Simulation/ValueObjects/SimulationConfig.cs
public class SimulationConfig
{
    // Wave progression (1-10, or custom waves)
    // Starting money/lives
    // Building placement strategy
    // Difficulty modifiers
    // Random seed for determinism
}
```

### 3. **SimulationResult.cs** - Results
```csharp
// Location: src/Application/Simulation/ValueObjects/SimulationResult.cs
public class SimulationResult
{
    // Success/failure
    // Final stats (money, lives, score)
    // Wave-by-wave breakdown
    // Performance metrics
    // Building efficiency data
}
```

### 4. **Mock Services** - Test Doubles
```csharp
// Location: src/Application/Simulation/Services/
- MockBuildingStatsProvider.cs  // Returns controlled building stats
- MockEnemyStatsProvider.cs     // Returns controlled enemy stats
- MockWaveService.cs            // Deterministic wave spawning
- MockBuildingService.cs        // Track building placement/actions
- MockSoundService.cs           // No-op sound system
```

## 🎮 Simulation Flow

### Phase 1: Basic Wave Clearing
```
1. Initialize game state (money, lives, empty field)
2. Load wave configuration (waves 1-10)
3. For each wave:
   a. Pre-wave: Allow building placement
   b. Combat: Spawn enemies, simulate tower combat
   c. Post-wave: Calculate rewards, check win/loss
4. Return final results
```

### Phase 2: Strategic Decision Making
```
1. Implement building placement strategies:
   - Fixed patterns (e.g., "4 basic towers at corners")
   - Adaptive strategies (e.g., "upgrade when money > X")
   - Economic strategies (e.g., "prioritize money generation")
2. Compare strategy effectiveness
```

### Phase 3: Balance Testing
```
1. Scenario testing:
   - "Can waves 1-10 be cleared with starting money?"
   - "What's minimum buildings needed for wave X?"
   - "How does enemy HP +20% affect difficulty?"
2. Regression tests for stat changes
```

## 📁 File Structure

```
src/Application/Simulation/
├── GameSimRunner.cs                    # Main runner
├── Services/
│   ├── MockBuildingStatsProvider.cs   # Controlled building stats
│   ├── MockEnemyStatsProvider.cs      # Controlled enemy stats
│   ├── MockWaveService.cs             # Deterministic waves  
│   ├── MockBuildingService.cs         # Building simulation
│   ├── MockSoundService.cs            # No-op sound
│   └── SimulationGameService.cs       # Orchestrates game state
├── Strategies/
│   ├── IBuildingStrategy.cs           # Strategy interface
│   ├── BasicCornerStrategy.cs         # Simple 4-corner placement
│   ├── EconomicStrategy.cs            # Money-focused strategy
│   └── AdaptiveStrategy.cs            # Dynamic building strategy
├── ValueObjects/
│   ├── SimulationConfig.cs            # Input configuration
│   ├── SimulationResult.cs            # Output results
│   ├── SimulationScenario.cs          # Test scenario definition
│   ├── WaveResult.cs                  # Per-wave results
│   └── GameState.cs                   # Current game state
└── Commands/
    ├── StartSimulationCommand.cs      # CQRS command
    └── RunScenarioCommand.cs          # CQRS command

tests/Application/Simulation/
├── GameSimRunnerTests.cs              # Core runner tests
├── Strategies/
│   └── BuildingStrategyTests.cs       # Strategy validation
└── Integration/
    └── SimulationIntegrationTests.cs  # Full scenario tests
```

## 🛠️ Implementation Phases

### Phase 1: Foundation (Week 1)
**Goal**: Basic simulation runs without errors

**Tasks**:
- [ ] Create `GameSimRunner.cs` skeleton
- [ ] Implement mock services (no-op implementations)
- [ ] Create basic `SimulationConfig` and `SimulationResult`
- [ ] Wire up with existing Domain services
- [ ] **Success Criteria**: Can run a simulation without crashes

### Phase 2: Combat Simulation (Week 2)
**Goal**: Simulate basic tower defense mechanics

**Tasks**:
- [ ] Implement enemy spawning simulation
- [ ] Simulate tower targeting and damage
- [ ] Calculate enemy health/death
- [ ] Track money/score accumulation
- [ ] **Success Criteria**: Can clear wave 1 with proper stats

### Phase 3: Strategy Implementation (Week 3)
**Goal**: Multiple building strategies work

**Tasks**:
- [ ] Create `IBuildingStrategy` interface
- [ ] Implement 2-3 basic strategies
- [ ] Compare strategy effectiveness
- [ ] **Success Criteria**: Different strategies produce different results

### Phase 4: Balance Testing (Week 4)
**Goal**: Useful balance validation

**Tasks**:
- [ ] Create scenario definitions
- [ ] Implement regression test suite
- [ ] Performance benchmarking
- [ ] **Success Criteria**: Can detect balance changes via automated tests

## 🧪 Test Scenarios

### Regression Tests
```csharp
[Test]
public void Waves1To10_WithStartingMoney_ShouldBeClearable()
{
    var config = SimulationConfig.Default();
    var result = runner.RunSimulation(config);
    
    result.Success.Should().BeTrue();
    result.FinalLives.Should().BeGreaterThan(0);
}

[Test]
public void EnemyHealth20PercentIncrease_ShouldStillBeClearable()
{
    var config = SimulationConfig.Default();
    config.EnemyHealthMultiplier = 1.2f;
    
    var result = runner.RunSimulation(config);
    result.Success.Should().BeTrue("Balance change broke game");
}
```

### Strategy Comparison
```csharp
[Test]
public void EconomicStrategy_ShouldOutperform_BasicStrategy()
{
    var economicResult = runner.RunSimulation(config, new EconomicStrategy());
    var basicResult = runner.RunSimulation(config, new BasicStrategy());
    
    economicResult.FinalScore.Should().BeGreaterThan(basicResult.FinalScore);
}
```

## 🔄 Domain Services Integration

### Required Domain Services
- `IBuildingStatsProvider` - Building statistics and data
- `IEnemyStatsProvider` - Enemy statistics and data
- `IBuildingService` - Building placement logic
- `IWaveService` - Wave configuration and progression

### Mock Implementation Strategy
```csharp
// Use real Domain value objects, mock Infrastructure
public class MockBuildingStatsProvider : IBuildingStatsProvider
{
    // Return deterministic building stats for testing
    // Allow configuration of specific scenarios
    // Track calls for verification
}

public class MockEnemyStatsProvider : IEnemyStatsProvider
{
    // Return deterministic enemy stats for testing
    // Allow configuration of specific scenarios
    // Track calls for verification
}
```

## 📊 Success Metrics

### Technical Metrics
- **Build Time**: Simulation runs < 1 second
- **Determinism**: Same seed produces identical results
- **Coverage**: Tests exercise all major game mechanics
- **Isolation**: Zero Godot/Infrastructure dependencies

### Game Balance Metrics
- **Difficulty Curve**: Each wave is harder than previous
- **Strategy Viability**: Multiple strategies can win
- **Economic Balance**: Money/upgrade progression feels right
- **Tuning Speed**: Stat changes validated in seconds, not minutes

## 🚀 Future Extensions

### Analytics Integration
- Export simulation data for analysis
- Generate difficulty curves
- Identify balance problems automatically

### AI Training
- Use simulation for ML model training
- Optimize strategies via genetic algorithms
- Auto-balance stat generation

### CI Integration
- Automated balance regression tests
- Performance benchmarking in CI
- Stat change validation workflows

## 🎯 Success Definition

**Architecture Success**: 
- Game simulation runs completely without Infrastructure layer
- Proves Domain/Application separation works

**Testing Success**:
- Can run 1000+ simulations per minute
- Deterministic results enable regression testing
- Balance changes are validated automatically

**Development Success**:
- Stat tuning becomes fast and data-driven
- Game balance is measurable and trackable
- New features can be tested before implementation
