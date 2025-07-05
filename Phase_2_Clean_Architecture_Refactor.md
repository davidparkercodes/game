# Phase 2: Domain Layer (Feature-Based)

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

## Phase 2: Domain Layer (Feature-Based)
*Pure business logic organized by game features*

### 2.1 Setup Feature-Based Domain Structure
- [x] Create `src/Domain/Buildings/` feature
- [x] Create `src/Domain/Enemies/` feature
- [x] Create `src/Domain/Projectiles/` feature
- [x] Create `src/Domain/Items/` feature
- [x] Create `src/Domain/Levels/` feature
- [x] Create corresponding test structure

### 2.2 Buildings Feature Domain
- [x] Move existing `BuildingStats.cs` → `Domain/Buildings/ValueObjects/`
- [x] Update namespace to `Game.Domain.Buildings.ValueObjects;`
- [x] Remove comments, ensure <200 lines
- [x] Create `Domain/Buildings/Entities/Building.cs` (pure logic)
- [x] Create `Domain/Buildings/Entities/BasicTurret.cs`
- [x] Create `Domain/Buildings/Entities/SniperTurret.cs`
- [x] Create `Domain/Buildings/Services/BuildingZoneValidator.cs`
- [x] Move and update unit tests
- [x] **TEST CHECKPOINT**: Buildings domain complete

### 2.3 Enemies Feature Domain
- [x] Move existing `EnemyStats.cs` → `Domain/Enemies/ValueObjects/`
- [x] Update namespace to `Game.Domain.Enemies.ValueObjects;`
- [x] Remove comments, ensure <200 lines
- [x] Create `Domain/Enemies/Entities/Enemy.cs` (pure logic)
- [x] Create `Domain/Enemies/Services/PathManager.cs`
- [x] Move and update unit tests
- [x] **TEST CHECKPOINT**: Enemies domain complete

### 2.4 Additional Feature Domains
- [x] Create `Domain/Projectiles/Entities/Bullet.cs`
- [x] Create `Domain/Items/Entities/LootablePickup.cs`
- [x] Move existing `LevelConfiguration.cs` → `Domain/Levels/ValueObjects/LevelData.cs`
- [x] Update namespaces and remove comments for all
- [x] Move and update unit tests
- [x] Resolve namespace conflicts (LevelData type alias)
- [x] **TEST CHECKPOINT**: All domain features complete

### Phase 2 Status: ✅ COMPLETE
**Completed:** December 2024  
**Tests:** 69 passing, 0 failing  
**Build:** Success (4 nullable warnings only)  
**Key Achievements:**
- Feature-based domain structure implemented
- All domain entities, value objects, and services created
- Namespace conflicts resolved with type aliases
- Clean code standards enforced (no comments, semicolon namespaces)
- Comprehensive unit test coverage
- Ready for Phase 3: Infrastructure Layer

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
