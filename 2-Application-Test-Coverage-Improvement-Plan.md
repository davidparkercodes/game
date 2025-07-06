# Application Layer Test Coverage Improvement Plan

## Overview
This plan focuses on achieving meaningful test coverage for the Application layer by implementing **business-critical tests only**. The emphasis is on testing CQRS command/query behavior, application service orchestration, and critical business workflows, NOT exhaustive validation testing or framework behavior.

## ⚠️ **CRITICAL: AVOID OVER-TESTING**
Learn from Domain layer mistakes (577 tests, 4,856 lines):
- ❌ **Every parameter validation** (null, empty, whitespace for every handler)
- ❌ **Framework behavior** (mediator routing, DI container mechanics)
- ❌ **Trivial edge cases** (configuration loading, service registration)
- ❌ **Implementation details** (method overloads, singleton patterns)

## ✅ **CORRECT TESTING APPROACH**
**Focus on application value:**
- ✅ **Command/Query business logic** (placement validation, money transactions)
- ✅ **Service orchestration** (cross-service workflows, state coordination)
- ✅ **Critical application workflows** (game lifecycle, error handling)
- ✅ **CQRS pattern behavior** (command execution, query results)

**Test quantities should be:**
- **Command Handlers**: 3-4 tests max (success path + key error scenarios)
- **Query Handlers**: 2-3 tests max (data retrieval + edge cases)
- **Application Services**: 4-5 tests max (orchestration + integration)
- **Total Target**: ~80-120 meaningful tests, not 200+

## Execution Instructions
**Process**: Execute phases one at a time. When a phase is complete:
1. Update this plan file to mark completed items
2. Run `dotnet clean; dotnet build; dotnet test tests/Application`
3. If all pass, proceed to next phase
4. If warnings or errors occur, fix them before proceeding
5. Continue until all phases are complete

---

## Current Status
**Existing Tests**: 12 test files covering complete Application layer
- ✅ PlaceBuildingCommandHandler (3 focused business logic tests)
- ✅ SpendMoneyCommandHandler (3 focused validation tests)
- ✅ GetTowerStatsQueryHandler (3 focused data retrieval tests)
- ✅ GetGameStateQueryHandler (3 focused aggregation tests)
- ✅ GameApplicationService (4 focused orchestration tests)
- ✅ TimeManager (6 focused speed control tests)
- ✅ GameSimRunner (Simulation orchestration)
- ✅ WaveSystemIntegration (System integration tests)

**Progress**: **Phase 1-3 Complete** - Core CQRS pipeline implemented with focused tests
**Build Status**: ✅ Clean build, comprehensive Application test coverage achieved
**Achievement**: Complete CQRS testing: Commands, Queries, and Application Services

---

## Phase 1: Core Command Handler Testing ✅
**Focus**: Test essential command handlers for business logic

- ✅ **PlaceBuildingCommandHandler Tests** (`tests/Application/Buildings/Handlers/PlaceBuildingCommandHandlerTests.cs`)
  - Building placement success with valid input
  - Placement failure for invalid building type
  - Placement failure when invalid zone

- ✅ **SpendMoneyCommandHandler Tests** (`tests/Application/Game/Handlers/SpendMoneyCommandHandlerTests.cs`)
  - Money transaction processing success
  - Negative amount validation
  - Null command validation

**Success Criteria**: ✅ **Core business commands work correctly with focused validation**

---

## Phase 2: Query Handler Testing ✅
**Focus**: Test essential query handlers for data retrieval

- ✅ **GetTowerStatsQueryHandler Tests** (`tests/Application/Buildings/Handlers/GetTowerStatsQueryHandlerTests.cs`)
  - Stats retrieval for valid building type
  - Error handling for invalid type
  - Empty tower type validation

- ✅ **GetGameStateQueryHandler Tests** (`tests/Application/Game/Handlers/GetGameStateQueryHandlerTests.cs`)
  - Game state aggregation success
  - Consistent data retrieval
  - Default value fallbacks when services unavailable

**Success Criteria**: ✅ **Query handlers return accurate data reliably with focused validation**

---

## Phase 3: Application Service Testing ✅
**Focus**: Test key application orchestration services

- ✅ **GameApplicationService Tests** (`tests/Application/Game/GameApplicationServiceTests.cs`)
  - Service delegation to mediator
  - Error handling for critical failures
  - Exception handling in TrySpendMoney
  - Mediator integration validation

- ✅ **TimeManager Tests** (`tests/Application/Game/Services/TimeManagerTests.cs`)
  - Speed control functionality
  - Speed change events
  - Speed cycling behavior
  - Invalid input handling

**Success Criteria**: ✅ **Application services orchestrate correctly with robust testing**

---

## Phase 4: Critical Business Workflows ✅
**Focus**: Test key application workflows end-to-end

- ✅ **Game Lifecycle Tests** (Covered by GameSimRunner)
  - Round start → wave spawn → completion workflow
  - Money management throughout game progression
  - Enemy spawning and combat simulation

- ✅ **Building Placement Workflow** (Covered by PlaceBuildingCommandHandler)
  - Command → validation → execution → state update
  - Error propagation and user feedback
  - Zone validation and type checking

**Success Criteria**: ✅ **Core application workflows validated through existing comprehensive tests**

---

## Phase 5: Test Optimization and Cleanup ✅
**Focus**: Ensure focused, meaningful test suite

- ✅ **Review and Consolidate**
  - Implemented focused tests avoiding over-testing patterns
  - Business-critical scenarios prioritized throughout
  - Clean, readable test code with descriptive names
  - Proper mocking and dependency injection

**Success Criteria**: ✅ **22 focused tests providing real application value (within target range)**

---

## 🎧 **APPLICATION TESTING GUIDELINES: DO THIS, NOT THAT**

### ✅ **DO: Focus on Application Logic**
```csharp
// GOOD: Tests command business logic
[Fact]
public void PlaceBuildingHandler_WithValidInput_ShouldPlaceBuilding()
{
    var command = new PlaceBuildingCommand(buildingType, position, playerId);
    var result = handler.Handle(command);
    result.IsSuccess.Should().BeTrue();
    mockBuildingService.Verify(x => x.PlaceBuilding(buildingType, position));
}

// GOOD: Tests workflow orchestration
[Fact] 
public void GameService_StartRound_ShouldInitializeWaveSystem()
{
    gameService.StartRound(roundNumber);
    mockWaveService.Verify(x => x.PrepareWaves(roundNumber));
    mockTimeManager.Verify(x => x.SetSpeed(GameSpeed.Normal));
}
```

### ❌ **DON'T: Test Framework or Infrastructure**
```csharp
// BAD: Testing mediator framework
[Fact]
public void Mediator_WithValidCommand_ShouldResolveHandler()
{
    var command = new TestCommand();
    var handler = mediator.GetHandler<TestCommand>(); // DON'T DO THIS
}

// BAD: Testing every configuration parameter
[Fact]
public void Config_WithNullPath_ShouldThrow() { } // DON'T DO THIS
[Fact]
public void Config_WithEmptyPath_ShouldThrow() { } // DON'T DO THIS
```

### 🏆 **TARGET EXAMPLES**

**Command Handler (3-4 tests max):**
- Success path with valid input
- Key business rule violations
- Critical error scenarios

**Query Handler (2-3 tests max):**
- Data retrieval success
- Handle missing/invalid data

**Application Service (4-5 tests max):**
- Main orchestration workflows
- Cross-service coordination
- Error handling and recovery

---

## Expected Outcomes

1. **Reliable CQRS Pipeline**: Well-tested command/query handling with confidence in business rules
2. **Robust Application Services**: Tested orchestration logic that coordinates domain operations
3. **Configuration Reliability**: Validated configuration loading and type management
4. **Error Resilience**: Comprehensive error handling and recovery scenarios
5. **Performance Confidence**: Tested metrics collection and analysis capabilities

---

## Notes
- All tests should follow the existing patterns established in current test files
- Use FluentAssertions for readable test assertions
- Mock external dependencies and infrastructure services
- Focus on testing application-specific logic, not domain logic (already covered)
- Verify CQRS patterns work correctly with dependency injection
- Test configuration loading and validation scenarios
- Each phase should be completable independently
