# Enhanced Wave Progression

This document describes the new challenging wave progression featuring diverse enemy types.

## Campaign: "Enhanced Campaign"

**Description:** A challenging progression featuring diverse enemy types

### Wave Breakdown

#### Wave 1: Scout Patrol 
**Theme:** Introduction - Basic enemies only  
**Challenge:** Learn the basics  
**Enemies:** 6 basic enemies  
**Bonus:** $25

Simple introduction wave with basic enemies to let players learn the mechanics.

#### Wave 2: Mixed Forces
**Theme:** Enemy type diversity introduced  
**Challenge:** Fast enemies require different strategy  
**Enemies:** 8 basic + 3 fast  
**Bonus:** $35

Introduces the fast enemy type. Players must learn that different enemies require different approaches.

#### Wave 3: Heavy Reinforcements  
**Theme:** Tank enemies + multi-group coordination  
**Challenge:** High HP enemies + timing  
**Enemies:** 4 tank + 6 basic + 5 fast  
**Bonus:** $50

- Tank enemies lead (high HP, slow)
- Basic enemies follow up (medium threat)  
- Fast enemies flank (timing challenge)

#### Wave 4: Elite Strike Team
**Theme:** Elite enemies + complex timing  
**Challenge:** High-value targets + sustained pressure  
**Enemies:** 3 elite + 5 tank + 8 fast  
**Bonus:** $75

- Elite enemies open (high stats, priority targets)
- Tank enemies sustain pressure  
- Fast enemies swarm

#### Wave 5: Total War
**Theme:** All enemy types + overwhelming numbers  
**Challenge:** Resource management + endurance  
**Enemies:** 4+3 elite + 6 tank + 10 basic + 12 fast  
**Bonus:** $100

Epic finale with multiple waves:
1. Elite vanguard (4 enemies)
2. Tank support (6 enemies) 
3. Basic army (10 enemies)
4. Fast swarm (12 enemies)
5. Elite cleanup crew (3 super-buffed)

## Enemy Type Strategy

### Basic Enemy (`basic_enemy`)
- **Stats:** 100 HP, 60 speed, 10 damage
- **Role:** Standard threat, good for learning
- **Counter:** Any tower works well

### Fast Enemy (`fast_enemy`) 
- **Stats:** 60 HP, 90 speed, 8 damage
- **Role:** Pressure, flanking, overwhelming towers
- **Counter:** High fire rate towers (rapid_tower)

### Tank Enemy (`tank_enemy`)
- **Stats:** 200 HP, 30 speed, 15 damage  
- **Role:** Damage sponge, forces sustained fire
- **Counter:** High damage towers (sniper_tower, heavy_tower)

### Elite Enemy (`elite_enemy`)
- **Stats:** 150 HP, 75 speed, 20 damage
- **Role:** High value target, balanced threat
- **Counter:** Focus fire, multiple tower types

## Progression Difficulty

### Wave Scaling
Each wave progressively:
- Increases enemy count
- Introduces new enemy types
- Adds wave modifiers (health/speed multipliers)
- Requires better tower positioning
- Demands economic management

### Money Economy
- **Early waves:** Lower rewards, affordable basic towers
- **Mid waves:** Medium rewards, can afford sniper towers  
- **Late waves:** High rewards, need diverse tower mix

### Strategic Evolution
1. **Wave 1-2:** Learn basics, single tower type works
2. **Wave 3-4:** Need tower diversity, positioning matters
3. **Wave 5:** Full strategic depth, economic management critical

## Balancing Philosophy

### Enemy Rewards
- Base reward from enemy stats JSON
- Additional wave bonus from wave config
- Higher tier enemies = higher base rewards
- Later waves = higher wave bonuses

### Wave Modifiers
- **Health Multipliers:** 1.0-2.0x (makes enemies tankier)
- **Speed Multipliers:** 1.0-1.4x (increases pressure)
- Applied on top of base stats from JSON

### Timing & Pacing
- **Pre-wave delays:** Give build time (0-3 seconds)
- **Post-wave delays:** Give upgrade time (3-5 seconds)  
- **Group start delays:** Create tactical timing challenges
- **Spawn intervals:** Control pressure intensity

## Recommended Tower Strategy

### Early Game (Waves 1-2)
- Basic towers for coverage
- 1-2 sniper towers for damage

### Mid Game (Waves 3-4)  
- Mix of basic and sniper towers
- Consider rapid towers for fast enemies
- Strategic positioning crucial

### Late Game (Wave 5)
- Diverse tower portfolio
- Heavy towers for tank enemies
- Rapid towers for fast swarms
- Sniper towers for elite targeting
- Optimal placement and upgrades essential

This progression ensures players experience the full depth of the tower defense mechanics while facing increasingly complex challenges that require strategic thinking and adaptation.
