# Tower Shooting System Fix Plan

## Problem Analysis

After the refactor, the tower shooting functionality is completely missing. The towers have all the necessary components in the scene files (Timer, Area2D for enemy detection, BulletScene references) but no actual shooting logic is implemented in the C# code.

**Current State:**
- ✅ Tower base class exists with properties (Range, Damage, FireRate, BulletScene)
- ✅ Timer node exists in tower scenes  
- ✅ Area2D node exists for enemy detection
- ✅ Bullet class exists and handles collision/damage
- ✅ Enemy detection logic implemented in towers
- ✅ Target selection logic implemented
- ❌ No projectile firing logic
- ❌ Timer not connected to fire events

---

## Implementation Plan

### Phase 1: Basic Enemy Detection 🎯 ✅ COMPLETED

#### [x] 1.1 Implement Enemy Detection in Building Base Class
- [x] Connect Area2D signals in `_Ready()` method
- [x] Track enemies entering/exiting tower range
- [x] Maintain list of enemies in range
- [x] Add debug logging for enemy detection

#### [x] 1.2 Implement Target Selection Logic  
- [x] Find closest enemy in range
- [x] Prefer enemies closest to path end (if available)
- [x] Handle case when no enemies in range
- [x] Update target when current target dies/leaves range

### Phase 2: Projectile Firing System 🔫

#### [ ] 2.1 Implement Basic Shooting Logic
- [ ] Connect Timer timeout signal to shooting method
- [ ] Calculate direction to target enemy
- [ ] Instantiate bullet from BulletScene
- [ ] Set bullet velocity toward target
- [ ] Set bullet damage from tower stats
- [ ] Start timer only when enemies are in range

#### [ ] 2.2 Implement Sound Integration
- [ ] Add shooting sound properties to tower types
- [ ] Play appropriate sound when firing (basic_tower_shoot, sniper_tower_shoot)
- [ ] Integrate with existing SoundManagerService

#### [ ] 2.3 Implement Tower-Specific Behaviors
- [ ] BasicTower: Standard firing rate, standard bullets
- [ ] SniperTower: Slower firing rate, higher damage bullets
- [ ] Set appropriate impact sounds for different bullet types

### Phase 3: Polish & Optimization 🎨

#### [ ] 3.1 Visual Enhancements
- [ ] Add muzzle flash effect when shooting
- [ ] Rotate tower sprite to face target (optional)
- [ ] Improve bullet trail/visual effects (optional)

#### [ ] 3.2 Performance Optimization
- [ ] Limit enemy detection frequency (use timer instead of every frame)
- [ ] Pool bullet objects to reduce garbage collection
- [ ] Optimize target selection algorithm

#### [ ] 3.3 Testing & Validation
- [ ] Test with multiple towers and enemies
- [ ] Verify different tower types have correct behaviors
- [ ] Test edge cases (enemies dying, leaving range, etc.)
- [ ] Ensure no memory leaks or performance issues

---

## Success Criteria

- [ ] Towers automatically detect enemies in range
- [ ] Towers fire projectiles at correct intervals
- [ ] Projectiles hit enemies and deal appropriate damage
- [ ] Different tower types behave distinctly (damage, fire rate, sounds)
- [ ] Shooting sounds play correctly for each tower type
- [ ] System performs well with multiple towers and enemies
- [ ] No errors or crashes during gameplay

---

## Technical Implementation Notes

### Key Files to Modify:
- `src/Presentation/Buildings/Building.cs` - Main shooting logic
- `src/Presentation/Buildings/BasicTower.cs` - Basic tower specifics  
- `src/Presentation/Buildings/SniperTower.cs` - Sniper tower specifics
- `src/Presentation/Projectiles/Bullet.cs` - May need minor updates

### Signals to Connect:
- Area2D: `area_entered`, `area_exited` (enemy detection)
- Timer: `timeout` (firing rate control)

### Sound Keys to Use:
- Basic Tower: `"basic_tower_shoot"`
- Sniper Tower: `"sniper_tower_shoot"`  
- Bullet Impact: `"basic_bullet_impact"`, `"sniper_bullet_impact"`

---

## Estimated Timeline
- **Phase 1**: 2-3 hours (enemy detection and targeting)
- **Phase 2**: 2-3 hours (shooting and sound integration)  
- **Phase 3**: 1-2 hours (polish and testing)

**Total**: 5-8 hours

---

## Priority
**HIGH** - Core gameplay functionality that needs to work for the game to be playable.
