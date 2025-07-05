# Phase 3: Infrastructure Layer (External Dependencies)

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

## Phase 3: Infrastructure Layer (External Dependencies)
*Data access, file I/O, DI container*

### 3.1 Setup Infrastructure Structure
- [x] Create `src/Infrastructure/Stats/`
- [x] Create `src/Infrastructure/Sound/`
- [x] Create `src/Infrastructure/Waves/`
- [x] Create `src/Infrastructure/DI/`
- [x] Create `tests/Infrastructure/`

### 3.2 Dependency Injection Setup
- [x] Create `Infrastructure/DI/ServiceLocator.cs`
- [x] Configure DI container with interfaces
- [x] Setup service registration
- [x] Write integration tests
- [x] **TEST CHECKPOINT**: DI container working

### 3.3 External Services
- [x] Move & refactor `StatsManager.cs` → `Infrastructure/Stats/`
- [x] Move & refactor `SoundManager.cs` → `Infrastructure/Sound/`
- [x] Move & refactor `WaveConfigLoader.cs` → `Infrastructure/Waves/`
- [x] Implement shared interfaces
- [x] Write integration tests
- [x] **TEST CHECKPOINT**: External services working

### Phase 3 Status: ✅ COMPLETE
**Completed:** December 2024  
**Tests:** 11 infrastructure tests passing, 0 failing  
**Build:** Success (0 warnings, 0 errors)  
**Key Achievements:**
- Dependency injection container implemented and working
- All external services abstracted behind interfaces
- Service adapters created for existing Godot singletons
- Infrastructure services support both Godot and test runtimes
- Clean separation between infrastructure and domain layers
- Ready for Phase 4: Application Layer (CQRS)

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
