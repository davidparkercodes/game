# Domain Layer Test Coverage Improvement Plan

## Overview
This plan focuses on achieving ~80% test coverage for the Domain layer by implementing meaningful, business-logic-focused tests. The emphasis is on testing critical domain behavior, validation logic, and entity interactions rather than padding statistics with trivial tests.

## Execution Instructions
**Process**: Execute phases one at a time. When a phase is complete:
1. Update this plan file to mark completed items
2. Run `dotnet clean; dotnet build; dotnet test tests/Domain`
3. If all pass, proceed to next phase
4. If warnings or errors occur, fix them before proceeding
5. Continue until all phases are complete

---

## Current Status
**Existing Tests**: 6 test files covering foundational classes
- ✅ BuildingStats, Position (Value Objects)
- ✅ Building, Tower, Enemy (Core Entities) 
- ✅ EnemyStats (Value Objects)

**Target Areas**: Focus on untested business-critical components

---

## Phase 1: Critical Value Objects Testing
**Focus**: Test domain value objects with business validation logic

- [ ] **BuildingType Tests** (`tests/Domain/Buildings/ValueObjects/BuildingTypeTests.cs`)
  - Constructor validation (null/empty string handling)
  - Equality comparison logic (based on InternalId)
  - ToString formatting
  - Operator overloads (==, !=)

- [ ] **EnemyType Tests** (`tests/Domain/Enemies/ValueObjects/EnemyTypeTests.cs`)
  - Constructor validation including tier validation (≥1)
  - Equality comparison logic
  - ToString formatting with tier information
  - Edge cases for tier boundaries

- [ ] **SoundRequest Tests** (`tests/Domain/Audio/ValueObjects/SoundRequestTests.cs`)
  - Constructor with optional parameters
  - IsPositional property logic
  - Position and ListenerPosition combinations
  - Volume and distance validation

**Success Criteria**: All value object business rules properly validated

---

## Phase 2: Entity Business Logic Testing
**Focus**: Test complex domain entities with rich behavior

- [ ] **BossEnemy Tests** (`tests/Domain/Enemies/Entities/BossEnemyTests.cs`)
  - Damage immunity mechanics
  - Special ability cooldown system
  - Phase-based behavior (IsInFinalPhase)
  - Override behavior for TakeDamage method
  - Scale multiplier validation

- [ ] **Bullet Tests** (`tests/Domain/Projectiles/Entities/BulletTests.cs`)
  - Projectile physics and movement
  - Distance tracking and max distance limits
  - Target calculation and velocity computation
  - Collision detection (IsNearTarget)
  - Lifecycle management (activation/deactivation)

- [ ] **LootablePickup Tests** (`tests/Domain/Items/Entities/LootablePickupTests.cs`)
  - Item collection mechanics
  - Expiration system and timing
  - Pickup radius detection
  - Factory methods for different item types
  - State management (Active, Collected, Expired)

**Success Criteria**: All entity state transitions and business rules covered

---

## Phase 3: Domain Services Testing
**Focus**: Test service classes containing business logic

- [ ] **BuildingZoneValidator Tests** (`tests/Domain/Buildings/Services/BuildingZoneValidatorTests.cs`)
  - Building placement validation logic
  - Minimum distance enforcement
  - Blocked zone management
  - Boundary checking
  - Building collection management
  - Proximity calculations (GetNearbyBuildings, GetClosestBuilding)

- [ ] **ConsoleLogger Tests** (`tests/Domain/Common/Services/ConsoleLoggerTests.cs`)
  - Log level filtering
  - Message formatting with prefixes
  - Different log types (Error, Warning, Information, Debug)
  - Emoji formatting in messages

**Success Criteria**: All service business logic and validation rules tested

---

## Phase 4: Integration and Edge Case Testing
**Focus**: Test interactions between domain components

- [ ] **Domain Integration Tests** (`tests/Domain/Integration/`)
  - Building and BuildingZoneValidator interaction
  - Enemy and BossEnemy inheritance behavior
  - Bullet collision with various entities
  - Item pickup during different game states

- [ ] **Edge Case Coverage**
  - Floating-point precision in distance calculations
  - DateTime handling in expiration logic
  - State machine transitions in entities
  - Null/empty validation across all components

**Success Criteria**: Complex scenarios and edge cases properly handled

---

## Phase 5: Coverage Analysis and Optimization
**Focus**: Measure and optimize test coverage

- [ ] **Coverage Assessment**
  - Run coverage analysis: `dotnet test --collect:"XPlat Code Coverage"`
  - Generate coverage reports
  - Identify gaps in critical business logic

- [ ] **Targeted Coverage Improvements**
  - Add tests for uncovered business logic paths
  - Focus on exception handling paths
  - Test complex property calculations

- [ ] **Test Quality Review**
  - Ensure tests validate business behavior, not implementation
  - Verify meaningful test names and descriptions
  - Remove any padding tests that don't add value

**Success Criteria**: ~80% coverage with meaningful, maintainable tests

---

## Testing Principles Applied

### Business Logic Focus
- Prioritize testing domain rules and validations
- Test state transitions and entity lifecycle
- Verify calculations and business formulas

### Quality Over Quantity
- Each test should validate specific business behavior
- Avoid testing trivial getters/setters without logic
- Focus on scenarios that could break in production

### Meaningful Test Names
- Use descriptive test method names
- Include business context in test organization
- Group related tests logically

### Edge Case Coverage
- Test boundary conditions
- Validate error handling
- Test null/empty scenarios

---

## Expected Outcomes

1. **Robust Domain Layer**: Well-tested business logic with confidence in domain rules
2. **Regression Protection**: Tests catch breaking changes to critical business logic
3. **Documentation**: Tests serve as living documentation of domain behavior
4. **Maintainability**: Clean, focused tests that are easy to understand and maintain

---

## Notes
- All tests should follow the existing patterns established in current test files
- Use FluentAssertions for readable test assertions
- Focus on testing public API and business behavior
- Mock external dependencies where needed
- Each phase should be completable independently
