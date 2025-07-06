# Application Layer Test Coverage Improvement Plan

## Overview
This plan focuses on achieving ~80% test coverage for the Application layer by implementing meaningful, business-logic-focused tests. The emphasis is on testing CQRS patterns, command/query handlers, application services, and complex orchestration logic rather than padding statistics with trivial tests.

## Execution Instructions
**Process**: Execute phases one at a time. When a phase is complete:
1. Update this plan file to mark completed items
2. Run `dotnet clean; dotnet build; dotnet test tests/Application`
3. If all pass, proceed to next phase
4. If warnings or errors occur, fix them before proceeding
5. Continue until all phases are complete

---

## Current Status
**Existing Tests**: 2 test files covering simulation components
- ✅ GameSimRunner (Simulation orchestration)
- ✅ WaveSystemIntegration (System integration tests)

**Target Areas**: Focus on untested CQRS, handlers, and application services

---

## Phase 1: CQRS Infrastructure Testing
**Focus**: Test the core CQRS infrastructure and mediator pattern

- [ ] **Mediator Tests** (`tests/Application/Shared/Cqrs/MediatorTests.cs`)
  - Command routing and handler resolution
  - Query routing and handler resolution
  - Service provider integration
  - Error handling for missing handlers
  - Generic command/query handling with different return types

- [ ] **Command/Query Interface Tests** (`tests/Application/Shared/Cqrs/CqrsInterfaceTests.cs`)
  - Interface contract validation
  - Generic type constraints
  - Handler registration patterns

**Success Criteria**: CQRS pattern works reliably with proper error handling

---

## Phase 2: Command Handler Testing
**Focus**: Test command handlers and their business logic

- [ ] **PlaceBuildingCommandHandler Tests** (`tests/Application/Buildings/Handlers/PlaceBuildingCommandHandlerTests.cs`)
  - Building placement validation logic
  - Zone service integration
  - Building type registry validation
  - Cost validation and money handling
  - Position occupancy checking
  - Error result generation for various failure scenarios

- [ ] **SpendMoneyCommandHandler Tests** (`tests/Application/Game/Handlers/SpendMoneyCommandHandlerTests.cs`)
  - Money deduction logic
  - Insufficient funds handling
  - Transaction recording
  - Reason tracking for spending

- [ ] **StartRoundCommandHandler Tests** (`tests/Application/Rounds/Handlers/StartRoundCommandHandlerTests.cs`)
  - Round initialization logic
  - State transition validation
  - Force start functionality
  - Round number validation

- [ ] **StartWaveCommandHandler Tests** (`tests/Application/Waves/Handlers/StartWaveCommandHandlerTests.cs`)
  - Wave spawning logic
  - Wave configuration validation
  - Enemy spawning coordination

**Success Criteria**: All command business logic properly validated with error scenarios

---

## Phase 3: Query Handler Testing
**Focus**: Test query handlers and data retrieval logic

- [ ] **GetTowerStatsQueryHandler Tests** (`tests/Application/Buildings/Handlers/GetTowerStatsQueryHandlerTests.cs`)
  - Stats retrieval logic
  - Building type validation
  - Performance optimization
  - Caching behavior (if implemented)

- [ ] **GetGameStateQueryHandler Tests** (`tests/Application/Game/Handlers/GetGameStateQueryHandlerTests.cs`)
  - Game state aggregation
  - Multi-source data collection
  - State consistency validation
  - Real-time data accuracy

**Success Criteria**: Query handlers return accurate, consistent data

---

## Phase 4: Application Services Testing
**Focus**: Test high-level application orchestration services

- [ ] **GameApplicationService Tests** (`tests/Application/Game/GameApplicationServiceTests.cs`)
  - Service initialization and singleton pattern
  - Mediator delegation
  - Error handling and fallback behavior
  - Method overloads and parameter validation
  - Integration with infrastructure services

- [ ] **TimeManager Tests** (`tests/Application/Game/Services/TimeManagerTests.cs`)
  - Speed control logic and validation
  - Event broadcasting for speed changes
  - Speed cycling functionality
  - Index validation and bounds checking
  - Singleton instance management

- [ ] **TypeManagementService Tests** (`tests/Application/Shared/Services/TypeManagementServiceTests.cs`)
  - Building and enemy type coordination
  - Registry aggregation logic
  - Configuration validation
  - Error collection and reporting
  - Category and tier filtering

**Success Criteria**: Application services orchestrate domain logic correctly

---

## Phase 5: Value Objects and Configuration Testing
**Focus**: Test application-specific value objects and configuration

- [ ] **GameState Tests** (`tests/Application/Simulation/ValueObjects/GameStateTests.cs`)
  - State management and transitions
  - Money and lives tracking
  - Score calculation
  - Game over conditions
  - Collection management (buildings, enemies)

- [ ] **SimulationConfig Tests** (`tests/Application/Simulation/ValueObjects/SimulationConfigTests.cs`)
  - Configuration validation
  - Parameter boundary checking
  - Default value handling
  - Configuration serialization

- [ ] **WaveMetrics Tests** (`tests/Application/Simulation/ValueObjects/WaveMetricsTests.cs`)
  - Metrics calculation accuracy
  - Performance indicator computation
  - Timing and duration tracking
  - Completion rate calculations

- [ ] **PlaceBuildingCommand Tests** (`tests/Application/Buildings/Commands/PlaceBuildingCommandTests.cs`)
  - Command construction validation
  - Parameter validation
  - Null handling
  - Result object creation

**Success Criteria**: Value objects maintain consistent state and validation

---

## Phase 6: Service Registration and Integration Testing
**Focus**: Test service registries and complex application logic

- [ ] **BuildingTypeRegistry Tests** (`tests/Application/Buildings/Services/BuildingTypeRegistryTests.cs`)
  - Configuration file loading
  - Type registration and lookup
  - Category and tier organization
  - Default and cheapest type resolution
  - JSON deserialization handling
  - File path resolution logic

- [ ] **WaveMetricsCollector Tests** (`tests/Application/Simulation/Services/WaveMetricsCollectorTests.cs`)
  - Wave tracking lifecycle
  - Enemy spawn/kill timing
  - Metrics aggregation and calculation
  - Performance analytics generation
  - Export functionality
  - State management across waves

- [ ] **Mock Service Tests** (`tests/Application/Simulation/Services/MockServiceTests.cs`)
  - Mock building stats provider
  - Mock enemy stats provider
  - Mock wave service behavior
  - Test data consistency

**Success Criteria**: Service registries provide reliable data access and metrics

---

## Phase 7: Integration and Edge Case Testing
**Focus**: Test complex interactions and edge cases

- [ ] **Command Pipeline Integration Tests** (`tests/Application/Integration/CommandPipelineTests.cs`)
  - End-to-end command execution
  - Handler chain validation
  - Error propagation through pipeline
  - Transaction boundaries

- [ ] **Application Service Integration Tests** (`tests/Application/Integration/ApplicationServiceTests.cs`)
  - Cross-service communication
  - State consistency across services
  - Event handling and propagation
  - Dependency injection resolution

- [ ] **Edge Case Coverage**
  - Null parameter handling across all handlers
  - Invalid configuration scenarios
  - Resource exhaustion (memory, file handles)
  - Concurrent access patterns
  - Performance under load

**Success Criteria**: Complex scenarios work reliably with proper error handling

---

## Phase 8: Coverage Analysis and Optimization
**Focus**: Measure and optimize test coverage

- [ ] **Coverage Assessment**
  - Run coverage analysis: `dotnet test --collect:"XPlat Code Coverage"`
  - Generate coverage reports
  - Identify gaps in critical business logic

- [ ] **Targeted Coverage Improvements**
  - Add tests for uncovered application logic paths
  - Focus on exception handling paths
  - Test configuration loading and validation

- [ ] **Test Quality Review**
  - Ensure tests validate application behavior, not implementation
  - Verify meaningful test names and descriptions
  - Remove any padding tests that don't add value

**Success Criteria**: ~80% coverage with meaningful, maintainable tests

---

## Testing Principles Applied

### CQRS Pattern Focus
- Test command handlers for business rule enforcement
- Test query handlers for data consistency
- Verify mediator routing and error handling

### Application Logic Emphasis
- Prioritize testing orchestration and coordination logic
- Test service integration and dependency management
- Verify configuration and validation logic

### Error Handling Coverage
- Test all failure scenarios in command handlers
- Verify proper error propagation
- Test invalid input handling

### State Management
- Test state transitions and consistency
- Verify lifecycle management
- Test concurrent access scenarios

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
