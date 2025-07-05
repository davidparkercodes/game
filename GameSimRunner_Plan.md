# 🧪 GameSimRunner Plan - Balance Validation Focus

## 🎯 Primary Goal: **FAST BALANCE VALIDATION**
Build a headless, super-fast simulation system for rapid game balance testing and validation. The #1 priority is enabling quick iteration on game balance by running simulations in milliseconds instead of minutes.

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
- **Hardcoded game balance values in C# code**

## 🎯 Single Source of Truth Architecture

### Core Principle: ALL Game Balance in Config Files
**No hardcoded values in .cs files!** All stats, costs, damage, health, etc. must be in `data/` config files.

### Config File Structure
```
data/
├── simulation/                    # Simulation-specific configs
│   ├── scenarios/
│   │   ├── quick-balance.json     # 5-wave quick test
│   │   ├── full-balance.json      # 10-wave complete test
│   │   ├── difficulty-test.json   # Various difficulty levels
│   │   └── enemy-health-test.json # Enemy health scaling tests
│   ├── building-stats.json        # Building costs, damage, range, etc.
│   ├── enemy-stats.json          # Enemy health, speed, rewards, etc.
│   └── wave-configs.json         # Wave progression, enemy counts
├── stats/                         # Production game stats
│   ├── building_stats.json       # Real game building stats
│   └── enemy_stats.json         # Real game enemy stats
└── balance-tests/                 # Balance validation configs
    ├── baseline.json             # Known-good baseline configuration
    ├── stress-tests.json         # Push-the-limits scenarios
    └── regression-tests.json     # Automated regression scenarios
```

### Example Config Files

#### `data/simulation/building-stats.json`
```json
{
  "version": "1.0",
  "description": "Building stats for simulation testing",
  "buildings": {
    "basic_tower": {
      "cost": 100,
      "damage": 25,
      "range": 150.0,
      "fire_rate": 1.0,
      "bullet_speed": 300.0,
      "description": "Basic tower for early waves"
    },
    "sniper_tower": {
      "cost": 250,
      "damage": 100,
      "range": 300.0,
      "fire_rate": 0.5,
      "bullet_speed": 500.0,
      "description": "High damage, long range, slow firing"
    }
  }
}
```

#### `data/simulation/scenarios/quick-balance.json`
```json
{
  "name": "Quick Balance Test",
  "description": "Fast 5-wave validation for rapid iteration",
  "settings": {
    "starting_money": 500,
    "starting_lives": 20,
    "max_waves": 5,
    "fast_mode": true
  },
  "modifiers": {
    "enemy_health_multiplier": 1.0,
    "enemy_speed_multiplier": 1.0,
    "building_cost_multiplier": 1.0
  },
  "test_variations": [
    { "name": "baseline", "enemy_health_multiplier": 1.0 },
    { "name": "+10% enemy health", "enemy_health_multiplier": 1.1 },
    { "name": "+20% enemy health", "enemy_health_multiplier": 1.2 },
    { "name": "+30% enemy health", "enemy_health_multiplier": 1.3 }
  ]
}
```

### Config Loading Architecture
```csharp
// Mock services load from simulation configs, not hardcoded values
public class MockBuildingStatsProvider : IBuildingStatsProvider
{
    private readonly BuildingStatsConfig _config;
    
    public MockBuildingStatsProvider()
    {
        // Load from data/simulation/building-stats.json
        _config = ConfigLoader.LoadBuildingStats("data/simulation/building-stats.json");
    }
    
    public BuildingStats GetBuildingStats(string buildingType)
    {
        // Return stats from loaded config, never hardcoded
        return _config.Buildings[buildingType];
    }
}
```

### Benefits of Config-Driven Design
1. **🎯 Single Source of Truth**: All balance in one place
2. **⚡ Fast Iteration**: Change JSON, run test, no recompilation
3. **🔄 Version Control**: Easy to track balance changes in git
4. **🧪 A/B Testing**: Easy to create config variations
5. **📊 Data Analysis**: Can generate configs programmatically
6. **🚀 Hot Reloading**: Could even reload configs without restart

## 🎮 Balance Validation Workflow

### Target Developer Experience
```bash
# Quick balance check after tweaking enemy stats
$ dotnet run --project GameSimRunner.Console --scenario quick-balance

🧪 Quick Balance Test - Wave 1-10 Validation
===========================================
⚡ Running at 1000x speed...

📊 SCENARIO: Default Configuration
┌─ Wave 1 ████████████████████████ 100% (5/5 enemies) ✅
├─ Wave 2 ████████████████████████ 100% (7/7 enemies) ✅
├─ Wave 3 ██████████████████████▓▓ 95% (18/19 enemies) ⚠️  1 leaked
├─ Wave 4 ████████████████████████ 100% (25/25 enemies) ✅
└─ Result: SUCCESS - 4/4 waves cleared, 19/20 lives remaining

📈 SCENARIO: Enemy Health +20%
┌─ Wave 1 ████████████████████████ 100% (5/5 enemies) ✅
├─ Wave 2 ███████████████████████▓ 98% (6/7 enemies) ⚠️  1 leaked
├─ Wave 3 ████████████████▓▓▓▓▓▓▓▓ 70% (13/19 enemies) ❌ 6 leaked
└─ Result: FAILED - Wave 3, 13/20 lives remaining

⚖️  BALANCE VERDICT: +20% enemy health breaks Wave 3!
💡 RECOMMENDATION: Max +15% enemy health increase safe

⏱️  Total time: 23ms (vs ~3min in real game)
```

### Key Features for Balance Validation
1. **Lightning Fast**: Complete 10-wave runs in <50ms
2. **Visual Progress**: ASCII progress bars and wave-by-wave breakdown
3. **Quick Scenarios**: Predefined balance test scenarios
4. **Comparison Mode**: A/B test different configurations side-by-side
5. **Failure Analysis**: Pinpoint exactly where balance breaks
6. **Recommendations**: Suggest safe balance ranges

## 📋 Core Components

### 1. **GameSimRunner.cs** - Main Entry Point
```csharp
// Location: src/Application/Simulation/GameSimRunner.cs
public class GameSimRunner
{
    public SimulationResult RunSimulation(SimulationConfig config)
    public Task<SimulationResult> RunSimulationAsync(SimulationConfig config)
    public List<SimulationResult> RunMultipleScenarios(List<SimulationConfig> scenarios)
    public ComparisonResult CompareConfigurations(SimulationConfig baseline, SimulationConfig modified)
}
```

### 1.5 **GameSimRunner.Console** - Visual Interface
```csharp
// Location: tools/GameSimRunner.Console/Program.cs
public class ConsoleInterface
{
    // Predefined scenarios for quick balance testing
    public void RunQuickBalanceTest()
    public void RunDifficultyComparison()
    public void RunCustomScenario()
    
    // Output formatting
    public void DisplayResults(SimulationResult result, bool verbose = false)
    public void DisplayComparison(ComparisonResult comparison)
    public void ShowProgressBar(WaveProgress progress)
}
```

### 2. **SimulationConfig.cs** - Configuration
```csharp
// Location: src/Application/Simulation/ValueObjects/SimulationConfig.cs
public class SimulationConfig
{
    // Core settings
    public int StartingMoney { get; set; } = 500;
    public int StartingLives { get; set; } = 20;
    public int MaxWaves { get; set; } = 10;
    public int RandomSeed { get; set; } = 12345;
    
    // Balance modifiers - KEY FOR BALANCE TESTING
    public float EnemyHealthMultiplier { get; set; } = 1.0f;
    public float EnemySpeedMultiplier { get; set; } = 1.0f;
    public float EnemyCountMultiplier { get; set; } = 1.0f;
    public float BuildingCostMultiplier { get; set; } = 1.0f;
    public float BuildingDamageMultiplier { get; set; } = 1.0f;
    
    // Performance settings
    public bool FastMode { get; set; } = true; // Skip animations, instant calculations
    public bool VerboseOutput { get; set; } = false;
    
    // Predefined scenarios for quick testing
    public static SimulationConfig QuickBalance() => new() { MaxWaves = 5, FastMode = true };
    public static SimulationConfig EnemyHealthTest(float multiplier) => new() { EnemyHealthMultiplier = multiplier };
    public static SimulationConfig DifficultyTest(float difficulty) => new() 
    { 
        EnemyHealthMultiplier = difficulty,
        EnemyCountMultiplier = 1.0f + (difficulty - 1.0f) * 0.5f 
    };
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

### 4. **Mock Services** - Config-Driven Test Doubles
```csharp
// Location: src/Application/Simulation/Services/
- MockBuildingStatsProvider.cs  // Loads from data/simulation/building-stats.json
- MockEnemyStatsProvider.cs     // Loads from data/simulation/enemy-stats.json
- MockWaveService.cs            // Loads from data/simulation/wave-configs.json
- MockBuildingService.cs        // Track building placement/actions
- MockSoundService.cs           // No-op sound system
- ConfigLoader.cs               // Centralized config file loading
```

**Critical Rule**: Mock services NEVER contain hardcoded game values. All stats must be loaded from `data/simulation/` config files.

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

## 😁 Implementation Phases

### ✅ Phase 1: Foundation (COMPLETED)
**Goal**: Basic simulation runs without errors

**Tasks**:
- [x] Create `GameSimRunner.cs` skeleton  
- [x] Implement mock services (no-op implementations)
- [x] Create basic `SimulationConfig` and `SimulationResult`
- [x] Wire up with existing Domain services
- [x] **Success Criteria**: Can run a simulation without crashes
- [x] **Result**: All 9 unit tests passing, simulation works!

### Phase 2: Config-Driven Console Interface (NEXT - Week 1)
**Goal**: Visual balance testing interface with all config-driven data

**Priority Tasks** (Config-First Approach):
- [ ] **FIRST**: Create config file structure in `data/simulation/`
- [ ] Create `data/simulation/building-stats.json` with all building stats
- [ ] Create `data/simulation/enemy-stats.json` with all enemy stats
- [ ] Create `data/simulation/scenarios/` with predefined test scenarios
- [ ] Update MockBuildingStatsProvider to load from config files
- [ ] Update MockEnemyStatsProvider to load from config files
- [ ] Remove ALL hardcoded values from existing mock services

**Secondary Tasks** (UI Implementation):
- [ ] Create `GameSimRunner.Console` project
- [ ] Implement ASCII progress bars and wave visualization
- [ ] Command-line arguments for different test modes
- [ ] Quick scenarios: `--scenario quick-balance`, `--scenario difficulty-test`
- [ ] Verbose/minimal output toggle: `--verbose` / `--minimal`
- [ ] JSON export for data analysis: `--export-json`
- [ ] **Success Criteria**: Can run config-driven visual balance tests in <100ms

#### Balance Testing Commands (Config-Driven)
```bash
# Quick validation using predefined scenario
dotnet run --project tools/GameSimRunner.Console --scenario quick-balance

# Load custom scenario file
dotnet run --project tools/GameSimRunner.Console --scenario data/simulation/scenarios/enemy-health-test.json

# Compare baseline vs modified config
dotnet run --project tools/GameSimRunner.Console --compare data/balance-tests/baseline.json data/balance-tests/modified.json

# Run all scenarios in a directory
dotnet run --project tools/GameSimRunner.Console --batch data/simulation/scenarios/

# Test with custom building stats file
dotnet run --project tools/GameSimRunner.Console --scenario quick-balance --building-stats data/experimental/new-building-stats.json

# Verbose wave-by-wave breakdown
dotnet run --project tools/GameSimRunner.Console --scenario quick-balance --verbose

# Export results for analysis
dotnet run --project tools/GameSimRunner.Console --scenario quick-balance --export-json results.json
```

#### Config File Workflow Example
```bash
# 1. Edit building costs in config file
vim data/simulation/building-stats.json
# Change basic_tower cost from 100 to 120

# 2. Test immediately without recompilation
dotnet run --project tools/GameSimRunner.Console --scenario quick-balance

# 3. If good, update production config
cp data/simulation/building-stats.json data/stats/building_stats.json
```

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

## 📈 Success Metrics

### Balance Validation Speed (PRIMARY GOAL)
- **Quick Test**: < 50ms for 5-wave quick balance check
- **Full Test**: < 200ms for complete 10-wave simulation  
- **Comparison**: < 300ms for A/B testing two configurations
- **Batch Analysis**: < 5 seconds for 100 scenario statistical analysis

### Balance Detection Accuracy
- **Break Point Detection**: Identify exact wave where balance fails
- **Safe Range Calculation**: Determine maximum safe stat modifications  
- **Regression Detection**: Catch balance changes in automated tests
- **Difficulty Validation**: Verify easy/medium/hard feel appropriately challenging

### Developer Experience
- **Visual Feedback**: Clear ASCII progress bars and wave breakdowns
- **Quick Commands**: One-liner commands for common balance tests
- **Export Options**: JSON data for deeper analysis when needed
- **Integration**: Easy to run as part of development workflow

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

## 🚀 Immediate Next Steps

### Priority 1: Config Migration (Remove Hardcoded Values)
1. **Create config file structure**:
   ```bash
   mkdir -p data/simulation/scenarios
   mkdir -p data/balance-tests
   ```

2. **Create initial config files**:
   - `data/simulation/building-stats.json` - Move hardcoded building stats
   - `data/simulation/enemy-stats.json` - Move hardcoded enemy stats
   - `data/simulation/scenarios/quick-balance.json` - Quick test scenario

3. **Update mock services**:
   - Remove hardcoded values from `MockBuildingStatsProvider.cs`
   - Remove hardcoded values from `MockEnemyStatsProvider.cs`
   - Add config loading logic

4. **Verify tests still pass** after config migration

### Priority 2: Console Interface
5. **Create GameSimRunner.Console project**
6. **Implement visual balance testing interface**
7. **Add command-line scenario support**

**Goal**: Enable the workflow where you edit a JSON file and immediately run a simulation to test balance changes, without touching any C# code.

**Next Steps**: Ready to migrate to config-driven architecture, then build the visual console interface!
