# Phase 4: Application Layer (CQRS Commands & Queries)

## Overview
This document outlines the step-by-step refactoring plan to transform our game codebase into Feature-Centric Clean Architecture with CQRS and Mediator patterns. We'll work feature by feature, building tests as we go, and ensuring each step can be manually tested before proceeding.

### Architecture Goals
- **Domain Layer**: Feature-based pure business logic + shared domain interfaces
- **Application Layer**: CQRS commands/queries with handlers + CQRS foundation
- **Infrastructure Layer**: External interfaces, file I/O, data access, DI container
- **Presentation Layer**: Godot-specific UI and Node logic

### Code Standards
- **No Comments**: Code should be self-documenting, no /// <summary> comments
- **Namespace Style**: Use `namespace Game.Feature.Layer;` (semicolon syntax)
- **File Size**: Keep all files under ~200 lines
- **Feature Organization**: Group by feature/domain concept, not technical layer

---

## Phase 4: Application Layer (CQRS Commands & Queries)
*Feature-based commands, queries, and handlers*

### 4.1 Setup Application Structure
- [x] Create `src/Application/Buildings/`
- [x] Create `src/Application/Game/`
- [x] Create `src/Application/Rounds/`
- [x] Create `src/Application/Waves/`
- [x] Create `src/Application/Queries/`
- [x] Create `tests/Application/`

### 4.2 Buildings Feature Application
- [x] Create `Application/Buildings/BuildingManager.cs`
- [x] Create `Application/Buildings/Commands/PlaceBuildingCommand.cs`
- [x] Create `Application/Buildings/Handlers/PlaceBuildingCommandHandler.cs`
- [x] Write comprehensive tests
- [x] **TEST CHECKPOINT**: Building commands working

### 4.3 Game Management Application
- [x] Create `Application/Game/Commands/SpendMoneyCommand.cs`
- [x] Create `Application/Game/Handlers/SpendMoneyCommandHandler.cs`
- [x] Create `Application/Rounds/Commands/StartRoundCommand.cs`
- [x] Create `Application/Rounds/Handlers/StartRoundCommandHandler.cs`
- [x] Create `Application/Game/GameApplicationService.cs`
- [x] Integrate with mediator pattern
- [x] Write comprehensive tests
- [x] **TEST CHECKPOINT**: Game management working

### 4.4 Waves Feature Application
- [x] Create `Application/Waves/Commands/StartWaveCommand.cs`
- [x] Create `Application/Waves/Handlers/StartWaveCommandHandler.cs`
- [x] Implement CQRS for wave operations
- [x] Fix WaveSpawner integration (IsSpawning property)
- [x] Write comprehensive tests
- [x] **TEST CHECKPOINT**: Wave system working

### 4.5 Query Layer
- [x] Create `Application/Queries/GetTurretStatsQuery.cs`
- [x] Create `Application/Queries/Handlers/GetTurretStatsQueryHandler.cs`
- [x] Create `Application/Queries/GetGameStateQuery.cs`
- [x] Create `Application/Queries/Handlers/GetGameStateQueryHandler.cs`
- [x] Add query response DTOs
- [x] Write comprehensive unit tests
- [x] **TEST CHECKPOINT**: Query layer complete

### 4.6 Complete Integration
- [x] Register all command handlers in DI container
- [x] Register all query handlers in DI container
- [x] Create service adapters for runtime safety
- [x] Verify mediator pattern functionality
- [x] Fix async method warnings
- [x] All 94 tests passing
- [x] Clean build with no compilation errors
- [x] **FINAL CHECKPOINT**: Phase 4 Complete!

---

## Testing Strategy

### Unit Tests
- Domain entities and value objects (CQRS handlers)
- CQRS commands and queries (isolated)
- Mediator pattern functionality
- Domain services (with mocked dependencies)

### Integration Tests
- Command/Query execution through mediator
- DI container service resolution
- Infrastructure components with real dependencies
- Feature flows end-to-end

### Manual Tests (After Each Phase)
- Game launches without errors
- CQRS commands execute correctly
- DI container resolves dependencies
- Feature interactions work
- Performance remains acceptable

---

## Architecture Benefits

### Feature-Centric Organization
- **Vertical Slices**: Each feature contains its own domain, application, and presentation code
- **Reduced Coupling**: Features are independent and loosely coupled
- **Team Scalability**: Different teams can work on different features
- **Easier Testing**: Feature boundaries make testing more focused

### CQRS & Mediator Pattern
- **Separation of Concerns**: Commands (write) and Queries (read) are separate
- **Loose Coupling**: Components communicate through mediator
- **Testability**: Easy to mock and test individual handlers
- **Scalability**: Easy to add new commands/queries without changing existing code

### Clean Architecture Principles
- **Dependency Inversion**: High-level modules don't depend on low-level modules
- **Interface Segregation**: Small, focused interfaces
- **Single Responsibility**: Each class has one reason to change
- **Open/Closed**: Open for extension, closed for modification

---

## Phase 4 Completion Summary

### ✅ **PHASE 4 COMPLETE** ✅

**Total Progress:** 35/35 items completed (100%)

**Key Achievements:**
- **Application Layer Structure**: Complete CQRS command/query architecture
- **Building Commands**: PlaceBuildingCommand with handler and tests
- **Game Management**: SpendMoney and StartRound commands with handlers
- **Wave Control**: StartWave command with WaveSpawner integration
- **Query Layer**: TurretStats and GameState queries with response DTOs
- **Full Integration**: All handlers registered in DI container with 94 passing tests
- **Clean Architecture**: Proper separation of concerns and testability

**Files Created:** 25+ new application layer files
**Test Coverage:** Comprehensive unit tests for all commands and queries
**Build Status:** ✅ Clean compilation with no errors
**Test Status:** ✅ All 94 tests passing

**Ready for:** Phase 5 - Presentation Layer Integration

---

## Notes
- Each checkbox represents a testable, deployable increment
- Features are organized vertically (domain → application → presentation)
- No comments policy enforces self-documenting code
- File size limit maintains readability and focus
- CQRS pattern enables clear separation of read/write operations
- Mediator pattern reduces coupling between components
- DI container manages all dependencies centrally
