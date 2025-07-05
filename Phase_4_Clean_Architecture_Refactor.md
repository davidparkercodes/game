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
- [ ] Create `src/Application/Buildings/`
- [ ] Create `src/Application/Game/`
- [ ] Create `src/Application/Rounds/`
- [ ] Create `src/Application/Waves/`
- [ ] Create `src/Application/Queries/`
- [ ] Create `tests/Application/`

### 4.2 Buildings Feature Application
- [ ] Create `Application/Buildings/BuildingManager.cs`
- [ ] Create `Application/Buildings/Commands/PlaceBuildingCommand.cs`
- [ ] Create `Application/Buildings/Handlers/PlaceBuildingCommandHandler.cs`
- [ ] Write comprehensive tests
- [ ] **TEST CHECKPOINT**: Building commands working

### 4.3 Game Management Application
- [ ] Move & refactor `GameManager.cs` → `Application/Game/`
- [ ] Move & refactor `RoundManager.cs` → `Application/Rounds/`
- [ ] Integrate with mediator pattern
- [ ] Write integration tests
- [ ] **TEST CHECKPOINT**: Game management working

### 4.4 Waves Feature Application
- [ ] Move & refactor wave classes → `Application/Waves/`
- [ ] Implement CQRS for wave operations
- [ ] Create query handlers
- [ ] Write comprehensive tests
- [ ] **TEST CHECKPOINT**: Wave system working

### 4.5 Query Layer
- [ ] Create `Application/Queries/GetTurretStatsQuery.cs`
- [ ] Create `Application/Queries/Handlers/GetTurretStatsQueryHandler.cs`
- [ ] Add additional game state queries
- [ ] Write unit tests
- [ ] **TEST CHECKPOINT**: Query layer complete

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

## Notes
- Each checkbox represents a testable, deployable increment
- Features are organized vertically (domain → application → presentation)
- No comments policy enforces self-documenting code
- File size limit maintains readability and focus
- CQRS pattern enables clear separation of read/write operations
- Mediator pattern reduces coupling between components
- DI container manages all dependencies centrally
