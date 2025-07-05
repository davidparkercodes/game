# Phase 5: Presentation Layer (Godot Integration)

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

## Phase 5: Presentation Layer (Godot Integration)
*Clean separation between game engine and business logic*

### 5.1 Setup Presentation Structure
- [x] Create `src/Presentation/Core/`
- [x] Create `src/Presentation/Buildings/`
- [x] Create `src/Presentation/Player/`
- [x] Create `src/Presentation/UI/`
- [x] Create `src/Presentation/Components/`
- [x] Create `src/Presentation/Inventory/`

### 5.2 Core Presentation
- [x] Move `Main.cs` → `Presentation/Core/`
- [x] Integrate with DI container
- [x] Setup mediator dependency
- [x] Update scene file to new script location
- [x] Backup original Main.cs
- [x] Build successfully compiles
- [x] All 94 tests still passing
- [x] Fixed BuildingZoneValidator initialization timing
- [x] **READY FOR MANUAL TEST**: Game launches

### 5.3 Feature-Based Presentation
- [ ] Move `BuildingPreview.cs` → `Presentation/Buildings/`
- [ ] Refactor to use CQRS commands via mediator
- [ ] Move player classes → `Presentation/Player/`
- [ ] Move UI classes → `Presentation/UI/`
- [ ] Move component classes → `Presentation/Components/`
- [ ] Manual test: All features work

### 5.4 Integration Testing
- [ ] Test complete feature flows
- [ ] Verify CQRS command/query execution
- [ ] Validate DI container resolution
- [ ] **TEST CHECKPOINT**: Full integration working

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
