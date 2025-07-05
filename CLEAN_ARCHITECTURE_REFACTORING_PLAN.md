# Clean Architecture Refactoring Plan

This document outlines the step-by-step refactoring plan to transform our game codebase into Clean Architecture principles. We'll work layer by layer, building tests as we go, and ensuring each step can be manually tested before proceeding.

## Overview
- **Domain Layer**: Pure business logic, no external dependencies
- **Application Layer**: Use cases and orchestration
- **Infrastructure Layer**: External interfaces, file I/O, data access
- **Presentation Layer**: Godot-specific UI and Node logic

---

## Phase 1: Domain Layer (Core Business Logic)
*Foundation layer - pure logic, testable, engine-agnostic*

### 1.1 Setup Domain Structure
- [x] Create `src/Domain/` directory structure
- [x] Create `src/Domain/Entities/` 
- [x] Create `src/Domain/ValueObjects/`
- [x] Create `src/Domain/Services/`
- [x] Create `tests/Domain/` structure

### 1.2 Value Objects (Data containers)
- [ ] Create `Domain/ValueObjects/BuildingStats.cs`
  - [ ] Extract from `BuildingStatsData.cs`
  - [ ] Write unit tests
  - [ ] Validate no Godot dependencies
- [ ] Create `Domain/ValueObjects/EnemyStats.cs`
  - [ ] Extract from `EnemyStatsData.cs` 
  - [ ] Write unit tests
- [ ] Create `Domain/ValueObjects/LevelData.cs`
  - [ ] Extract from current `LevelData.cs`
  - [ ] Write unit tests
- [ ] **TEST CHECKPOINT**: Run tests, ensure no compilation errors

### 1.3 Core Entities (Business Logic)
- [ ] Create `Domain/Entities/BuildingEntity.cs`
  - [ ] Extract pure logic from `Building.cs` (damage, range, cost calculations)
  - [ ] Remove Godot dependencies (Node2D, Timer, etc.)
  - [ ] Write comprehensive unit tests
- [ ] Create `Domain/Entities/EnemyEntity.cs`
  - [ ] Extract pure logic from `Enemy.cs` (health, movement calculations)
  - [ ] Remove Godot dependencies
  - [ ] Write unit tests
- [ ] Create `Domain/Entities/BulletEntity.cs`
  - [ ] Extract pure logic from `Bullet.cs` (damage, velocity calculations)
  - [ ] Write unit tests
- [ ] Create `Domain/Entities/BasicTurretEntity.cs`
  - [ ] Extract from `BasicTurret.cs`
  - [ ] Inherit from `BuildingEntity`
  - [ ] Write unit tests
- [ ] Create `Domain/Entities/SniperTurretEntity.cs`
  - [ ] Extract from `SniperTurret.cs`
  - [ ] Write unit tests
- [ ] Create `Domain/Entities/LootablePickupEntity.cs`
  - [ ] Extract from `LootablePickup.cs`
  - [ ] Write unit tests
- [ ] **TEST CHECKPOINT**: Comprehensive entity testing

### 1.4 Domain Services (Pure Business Rules)
- [ ] Create `Domain/Services/IBuildingZoneValidator.cs` (interface)
- [ ] Create `Domain/Services/BuildingZoneValidatorService.cs`
  - [ ] Extract validation logic from `BuildingZoneValidator.cs`
  - [ ] Remove Godot TileMap dependencies (accept abstract grid data)
  - [ ] Write unit tests with mock grid data
- [ ] Create `Domain/Services/IPathingService.cs` (interface)
- [ ] Create `Domain/Services/PathingService.cs`
  - [ ] Extract pathfinding logic from `PathManager.cs`
  - [ ] Remove Godot dependencies
  - [ ] Write unit tests
- [ ] **TEST CHECKPOINT**: Domain services testing

---

## Phase 2: Infrastructure Layer (External Dependencies)
*Data access, file I/O, external services*

### 2.1 Setup Infrastructure Structure
- [ ] Create `src/Infrastructure/` directory structure
- [ ] Create `src/Infrastructure/Stats/`
- [ ] Create `src/Infrastructure/Sound/`
- [ ] Create `src/Infrastructure/Waves/`
- [ ] Create `tests/Infrastructure/`

### 2.2 Stats Infrastructure
- [ ] Move `StatsManager.cs` → `Infrastructure/Stats/`
- [ ] Move `StatsConfigUtility.cs` → `Infrastructure/Stats/`
- [ ] Create interfaces: `IStatsRepository.cs`, `IStatsLoader.cs`
- [ ] Refactor to depend on Domain value objects
- [ ] Write integration tests
- [ ] **TEST CHECKPOINT**: Stats loading still works

### 2.3 Sound Infrastructure
- [ ] Move `SoundManager.cs` → `Infrastructure/Sound/`
- [ ] Move `SoundConfig.cs` → `Infrastructure/Sound/`
- [ ] Move `SoundLoader.cs` → `Infrastructure/Sound/`
- [ ] Move `SoundConfigUtility.cs` → `Infrastructure/Sound/`
- [ ] Create interfaces: `ISoundRepository.cs`, `IAudioPlayer.cs`
- [ ] Write integration tests
- [ ] **TEST CHECKPOINT**: Sound system still works

### 2.4 Wave Configuration Infrastructure
- [ ] Move `WaveConfig.cs` → `Infrastructure/Waves/`
- [ ] Move `WaveConfigLoader.cs` → `Infrastructure/Waves/`
- [ ] Create interfaces: `IWaveConfigRepository.cs`
- [ ] Write integration tests
- [ ] **TEST CHECKPOINT**: Wave loading still works

---

## Phase 3: Application Layer (Use Cases & Orchestration)
*Game flow, managers, coordination between layers*

### 3.1 Setup Application Structure
- [ ] Create `src/Application/` directory structure
- [ ] Create `src/Application/Managers/`
- [ ] Create `src/Application/Waves/`
- [ ] Create `src/Application/UseCases/`
- [ ] Create `tests/Application/`

### 3.2 Core Managers
- [ ] Move `GameManager.cs` → `Application/Managers/`
- [ ] Move `RoundManager.cs` → `Application/Managers/`
- [ ] Move `BuildingManager.cs` → `Application/Managers/`
- [ ] Refactor to use Domain entities and Infrastructure interfaces
- [ ] Create use case interfaces
- [ ] Write integration tests
- [ ] **TEST CHECKPOINT**: Game flow still works

### 3.3 Wave Management
- [ ] Move `WaveProgressTracker.cs` → `Application/Waves/`
- [ ] Move `WaveSpawner.cs` → `Application/Waves/`
- [ ] Move `WaveTimerManager.cs` → `Application/Waves/`
- [ ] Move `WaveEnemySpawner.cs` → `Application/Waves/`
- [ ] Refactor to use Domain entities
- [ ] Write comprehensive tests
- [ ] **TEST CHECKPOINT**: Wave system still works

### 3.4 Use Cases (Business Operations)
- [ ] Create `Application/UseCases/PlaceBuildingUseCase.cs`
- [ ] Create `Application/UseCases/StartWaveUseCase.cs`
- [ ] Create `Application/UseCases/HandleEnemyDeathUseCase.cs`
- [ ] Create `Application/UseCases/ProcessRoundUseCase.cs`
- [ ] Write comprehensive unit tests
- [ ] **TEST CHECKPOINT**: All use cases tested

---

## Phase 4: Presentation Layer (Godot Integration)
*UI, Node-specific logic, input handling*

### 4.1 Setup Presentation Structure
- [ ] Create `src/Presentation/` directory structure
- [ ] Create `src/Presentation/Core/`
- [ ] Create `src/Presentation/Nodes/Buildings/`
- [ ] Create `src/Presentation/Nodes/Player/`
- [ ] Create `src/Presentation/Nodes/UI/`
- [ ] Create `src/Presentation/Nodes/Components/`
- [ ] Create `src/Presentation/Nodes/Inventory/`

### 4.2 Core Presentation
- [ ] Move `Main.cs` → `Presentation/Core/`
- [ ] Refactor to orchestrate Application layer
- [ ] Remove direct business logic
- [ ] Manual test: Game still launches

### 4.3 Building Presentation
- [ ] Move `BuildingPreview.cs` → `Presentation/Nodes/Buildings/`
- [ ] Refactor current `Building.cs` → `Presentation/Nodes/Buildings/BuildingNode.cs`
- [ ] Create `Presentation/Nodes/Buildings/BasicTurretNode.cs`
- [ ] Create `Presentation/Nodes/Buildings/SniperTurretNode.cs`
- [ ] Refactor to use Domain entities via Application layer
- [ ] Manual test: Building system works

### 4.4 Player Presentation
- [ ] Move `Player.cs` → `Presentation/Nodes/Player/`
- [ ] Move `PlayerMovement.cs` → `Presentation/Nodes/Player/`
- [ ] Move `PlayerBuildingBuilder.cs` → `Presentation/Nodes/Player/`
- [ ] Refactor to use Application use cases
- [ ] Manual test: Player controls work

### 4.5 UI Presentation
- [ ] Move `Hud.cs` → `Presentation/Nodes/UI/`
- [ ] Refactor to display Application layer data
- [ ] Manual test: UI updates correctly

### 4.6 Components Presentation
- [ ] Move `Damageable.cs` → `Presentation/Nodes/Components/`
- [ ] Move `Hitbox.cs` → `Presentation/Nodes/Components/`
- [ ] Move `HpLabel.cs` → `Presentation/Nodes/Components/`
- [ ] Move `StatsComponent.cs` → `Presentation/Nodes/Components/`
- [ ] Manual test: Components work

### 4.7 Inventory Presentation
- [ ] Move `Inventory.cs` → `Presentation/Nodes/Inventory/`
- [ ] Manual test: Inventory works

---

## Phase 5: Testing & Cleanup
*Comprehensive testing, cleanup, documentation*

### 5.1 Test Coverage
- [ ] Achieve 90%+ test coverage on Domain layer
- [ ] Achieve 80%+ test coverage on Application layer
- [ ] Add integration tests for Infrastructure layer
- [ ] Add end-to-end tests for critical game flows

### 5.2 Dependency Injection Setup
- [ ] Create DI container configuration
- [ ] Wire up all interfaces and implementations
- [ ] Test dependency resolution

### 5.3 Performance & Cleanup
- [ ] Remove old `scripts/` directory
- [ ] Update all scene references to new paths
- [ ] Performance test: ensure no regressions
- [ ] Update documentation

### 5.4 Final Validation
- [ ] **FULL GAME TEST**: Complete game flow works
- [ ] **BUILD TEST**: Game builds successfully
- [ ] **PERFORMANCE TEST**: No significant performance impact
- [ ] **CLEAN CODE**: No code smells, proper SOLID principles

---

## Testing Strategy

### Unit Tests
- Domain entities and value objects
- Domain services (with mocked dependencies)
- Application use cases (with mocked infrastructure)

### Integration Tests
- Infrastructure components with real file system
- Application managers with real dependencies

### Manual Tests (After Each Phase)
- Game launches without errors
- Core gameplay mechanics work
- Building system functional
- Wave spawning functional
- Player controls responsive
- UI updates correctly

---

## Notes
- Each checkbox represents a testable, deployable increment
- Manual testing checkpoint after each major section
- Domain layer must have ZERO Godot dependencies
- Use interfaces extensively for testability
- Follow SOLID principles throughout
