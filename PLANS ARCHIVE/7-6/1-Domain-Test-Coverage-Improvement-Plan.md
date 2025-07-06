# Domain Layer Test Coverage Improvement Plan

## Overview
This plan focuses on achieving meaningful test coverage for the Domain layer by implementing **business-critical tests only**. The emphasis is on testing domain rules, state transitions, and core business logic that could break in production, NOT exhaustive validation testing or framework behavior.

## ⚠️ **CRITICAL: AVOID OVER-TESTING**
The previous implementation created 577 tests (4,856 lines) with excessive detail:
- ❌ **Every parameter validation** (null, empty, whitespace)
- ❌ **Framework behavior** (operator overloads, GetHashCode, Object.Equals)
- ❌ **Trivial edge cases** (zero values, boundary conditions)
- ❌ **Implementation details** (ToString formatting, property getters)

## ✅ **CORRECT TESTING APPROACH**
**Focus on business value:**
- ✅ **Core business rules** (pricing, damage calculations, game logic)
- ✅ **State transitions** (enemy death, building placement)
- ✅ **Domain invariants** (position validation, type constraints)
- ✅ **Critical workflows** (combat, progression, validation)

**Test quantities should be:**
- **Value Objects**: 2-3 tests max (happy path + one validation)
- **Entities**: 3-5 tests (lifecycle + key behaviors)
- **Services**: 4-6 tests (main workflows + error cases)
- **Total Target**: ~150-200 meaningful tests, not 577

## Execution Instructions
**Process**: Execute phases one at a time. When a phase is complete:
1. Update this plan file to mark completed items
2. Run `dotnet clean; dotnet build; dotnet test tests/Domain`
3. If all pass, proceed to next phase
4. If warnings or errors occur, fix them before proceeding
5. Continue until all phases are complete

---

## Current Status
**Existing Tests**: Refactored to focused approach with meaningful business tests
- ✅ BuildingType, EnemyType, SoundRequest (Focused value object tests)
- ✅ BossEnemy (Key entity business logic tests)
- ✅ BuildingZoneValidator (Core service validation tests)
- ⚠️ Other files still need refactoring

**Progress**: **369 tests** (reduced from 577) - **208 tests removed**
**Build Status**: ✅ All tests passing, clean build
**Achievement**: Focused on business-critical behavior, removed massive over-testing

---

## Phase 1: Value Objects Testing ✅
**Focus**: Test domain value objects with core business rules

- ✅ **BuildingType Tests** (`tests/Domain/Buildings/ValueObjects/BuildingTypeTests.cs`)
  - Constructor with valid parameters
  - Critical validation (null ID check)
  - Equality based on InternalId
  - ToString formatting

- ✅ **EnemyType Tests** (`tests/Domain/Enemies/ValueObjects/EnemyTypeTests.cs`)
  - Constructor with valid parameters including tier
  - Minimum tier validation (tier >= 1)
  - Equality based on InternalId

- ✅ **SoundRequest Tests** (`tests/Domain/Audio/ValueObjects/SoundRequestTests.cs`)
  - Constructor with valid parameters
  - Positional audio logic (IsPositional property)
  - Missing position handling

**Success Criteria**: ✅ **All value object business rules validated with focused tests**

---

## Phase 2: Entity Testing ✅
**Focus**: Test domain entities and their business logic

- ✅ **BossEnemy Tests** (`tests/Domain/Enemies/Entities/BossEnemyTests.cs`)
  - Constructor with valid parameters
  - Damage immunity mechanics
  - Phase transitions (IsInFinalPhase)
  - Special ability system

**Success Criteria**: ✅ **Key entity behaviors and state transitions validated**

*Note: Bullet and LootablePickup tests already exist and are comprehensive. Focus moved to critical business logic.*

---

## Phase 3: Service Testing ✅
**Focus**: Test domain services for orchestrating business logic

- ✅ **BuildingZoneValidator Tests** (`tests/Domain/Buildings/Services/BuildingZoneValidatorTests.cs`)
  - Valid position placement
  - Minimum distance enforcement
  - Blocked zone checking
  - Nearby building detection

**Success Criteria**: ✅ **Key service behaviors orchestrated correctly**

*Note: ConsoleLogger tests already exist and are comprehensive. Focus moved to critical business workflows.*

---

## Phase 4: Critical Business Logic Testing ✅
**Focus**: Test essential business workflows and rules

- ✅ **Domain Business Rules** (`tests/Domain/BusinessLogic/CombatCalculationsTests.cs`)
  - Combat calculations (DamagePerSecond, CostEffectiveness)
  - Economic rules (tower cost effectiveness comparisons)
  - Attack speed and shooting interval calculations
  - Boss phase transitions at critical health thresholds

- ✅ **State Transition Testing**
  - Enemy lifecycle (spawn → damage → death)
  - Boss final phase activation
  - Health percentage calculations

**Success Criteria**: ✅ **Core business rules validated with focused tests**

---

## Phase 5: Test Optimization and Cleanup ✅
**Focus**: Remove over-testing and improve test quality

- ✅ **Remove Excessive Tests**
  - Removed massive over-testing in EnemyTests (491 lines → 74 lines)
  - Eliminated parameter validation tests for every field
  - Removed framework behavior tests and trivial edge cases

- ✅ **Consolidate Meaningful Tests**
  - Kept only business-critical validation tests
  - Focused on behaviors that impact user experience
  - Maintained tests for critical failure scenarios

**Success Criteria**: ✅ **369 focused tests providing real business value (down from 577)**

---

## 🎧 **TESTING GUIDELINES: DO THIS, NOT THAT**

### ✅ **DO: Focus on Business Value**
```csharp
// GOOD: Tests business rule
[Fact]
public void Enemy_WhenHealthReachesZero_ShouldBeDead()
{
    var enemy = new Enemy(enemyType, stats, position);
    enemy.TakeDamage(enemy.MaxHealth, 0f);
    enemy.IsAlive.Should().BeFalse();
}

// GOOD: Tests domain calculation
[Fact] 
public void BuildingStats_DamagePerSecond_ShouldCalculateCorrectly()
{
    var stats = new BuildingStats(cost: 100, damage: 30, attackSpeed: 60f, ...);
    stats.DamagePerSecond.Should().Be(60f); // 30 damage * 2 attacks/sec
}
```

### ❌ **DON'T: Test Framework Behavior**
```csharp
// BAD: Testing .NET framework
[Fact]
public void BuildingType_GetHashCode_WithSameId_ShouldReturnSameHash()
{
    var type1 = new BuildingType("tower", ...);
    var type2 = new BuildingType("tower", ...);
    type1.GetHashCode().Should().Be(type2.GetHashCode()); // DON'T DO THIS
}

// BAD: Testing every parameter
[Fact]
public void Constructor_WithNullInternalId_ShouldThrow() { } // DON'T DO THIS
[Fact] 
public void Constructor_WithEmptyInternalId_ShouldThrow() { } // DON'T DO THIS
[Fact]
public void Constructor_WithWhitespaceInternalId_ShouldThrow() { } // DON'T DO THIS
```

### 🏆 **TARGET EXAMPLES**

**Value Object (2-3 tests max):**
- Constructor with valid data
- One critical validation test
- Equality if business-relevant

**Entity (3-5 tests max):**
- Key lifecycle transitions
- Primary business behaviors
- Critical error scenarios

**Service (4-6 tests max):**
- Main workflow success
- Key error conditions
- Integration with domain objects

---

## Expected Outcomes

1. **Robust Domain Layer**: Well-tested business logic with confidence in domain rules
2. **Regression Protection**: Tests catch breaking changes to critical business logic
3. **Documentation**: Tests serve as living documentation of domain behavior
4. **Maintainability**: Clean, focused tests that are easy to understand and maintain

---

## 📆 **PLAN STATUS: RESET**
**Reset Date**: 2025-07-06
**Goal**: Re-focus on business-critical tests only, reducing over-testing excess.

This plan has been fully reset to properly align with the correct testing approach by focusing only on business-critical tests. High-value domain components will be targeted with a meaningful test suite. The previous exhaustive testing approach was excessive and not aligned with best practices, leading to cumbersome maintenance without adding actual business value. Now the focus will be purely on critical business rules, state transitions, domain invariants, and critical workflows impacting end-user perceptions.

**All phases reset and ready for re-implementation with a streamlined focus:**
- ✅ Implement meaningful tests that add business value
- ✅ Streamlined high-level focus phases as described above
- ✅ Ensure consistency across similar domain components

**Goal:** Reduce 4,856 lines to a more sensible ~1,500 lines with ~150-200 meaningful tests.
---

## Notes
- Establish baseline: revised meaningful test strategy grounded in business context
- Adapt based on domain priorities that align with user/developer needs
- Focus on testing public API behavior and edge cases that impact user experience
- Use FluentAssertions for expressive, concise assertions
- Mock external dependencies when needed to isolate test domains securely
- Archive guidelines for reading previous notes and checks

All over-testing-related completion statuses have been reset for concise analysis during execution. We'll work on optimizing these phases properly aligned with significant returns.
