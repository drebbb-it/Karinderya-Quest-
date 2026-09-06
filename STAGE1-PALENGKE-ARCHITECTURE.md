# Palengke Stage — Architecture & GameObject Reference

## Core Loop Summary
- Linear side-scrolling market: **Start → Fruits → Vegetables → Store/Supplies → Meat → Fish → Rice**
- Player has a **shopping list/inventory** and must collect **all required ingredients before the timer runs out** (win condition; timer expiry = loss condition)
- Threats along the way:
  1. **Crosswalk traffic** — timed crossing; getting hit costs a **life** (lives-based, not instant fail)
  2. **Bad ingredient stalls** — player avoids wrong/spoiled items
  3. **NPC snatchers** — race the player to ingredients; if an NPC wins, that ingredient is **lost permanently**, pressuring the player to move fast
- Running out of lives = separate loss condition from the timer

---

## System Architecture

Event-driven, decoupled via a static event hub (`GameEvents`) so systems never reference each other directly.

| Script | Role | Status |
|---|---|---|
| `GameEvents` | Static event hub — pickup, steal, inventory complete, vehicle hit, out of lives, timer tick/expired |  BUILT |
| `IngredientData` | ScriptableObject defining a single ingredient (id, name, sprite, category) |  Not yet built |
| `InventoryController` | Tracks required shopping list, listens for collect/steal events, raises win condition | Not yet built  |
| `IngredientPickup` | Sits on each ingredient prefab; fires collected/stolen events, supports NPC `TrySnatch()` |  Not yet built |
| `TimerController` | Countdown timer; fires timer-expired (loss) |  BUILT |
| `LivesController` | Tracks lives; listens for vehicle hits; fires out-of-lives (loss) |  BUILT |
| `PlayerController2D` | Player movement/input | BUILT |
| `TrafficHazard` | Vehicle movement + player-hit detection at crosswalk |  Not yet built |
| `IngredientSnatcher` | NPC AI that races player to an ingredient | Not yet built |
| `InventoryUI` | Binds to `OnIngredientCollected` / `OnInventoryComplete` |  Not yet built |
| `TimerUI` | Binds to `OnTimerTick` |  Not yet built |
| `LivesUI` | Binds to `OnPlayerHitByVehicle` / `OnPlayerOutOfLives` |  Not yet built |

**Open design question:** does touching a "bad ingredient" at a stall cost a life, or is it purely avoidance with no penalty? Affects whether it needs its own event or reuses pickup-rejection logic.

---

## GameObject Breakdown

### Core / Manager
**`StageController`** (empty GameObject, persists for the stage)
- `InventoryController`
- `TimerController`
- `LivesController`
- No visuals — logic only

### Player
**`Player`**
- `PlayerController2D`
- `Rigidbody2D`
- `Collider2D` (non-trigger, for platform collision)
- Tag: `"Player"` (required by `IngredientPickup` and `TrafficHazard`)

### Ingredients
**`Ingredient_[Type]`** (one prefab per ingredient, e.g. `Ingredient_Mango`, `Ingredient_Tilapia`)
- `IngredientPickup`
- `Collider2D` (trigger)
- `SpriteRenderer`
- References an `IngredientData` asset (lives in `Assets`, not the scene)

### Stalls
**`Stall_[Category]`** (e.g. `Stall_Fruits`, `Stall_Meat`) — static level dressing
- `SpriteRenderer`
- Acts as spawn point / parent for that category's `Ingredient_` prefabs
- Bad-ingredient stalls may hold child `BadIngredient` objects with separate reject logic (pending design decision above)

### Crosswalk Hazard
**`CrosswalkZone`** (empty GameObject marking the hazard area)
- Optional `BoxCollider2D` (trigger) to activate vehicles only when player is near

**`Vehicle_[Type]`** (e.g. `Vehicle_Jeep`, `Vehicle_Car`)
- `TrafficHazard`
- `Rigidbody2D` (kinematic, scripted movement)
- `Collider2D` (trigger)

### NPC Snatchers
**`NPC_Snatcher`** (one or more, active near ingredient zones)
- `IngredientSnatcher`
- `Rigidbody2D`
- Detection method to find nearest uncollected `Ingredient_` in its zone

### UI (Canvas)
**`UICanvas`**
- `InventoryUI`
- `TimerUI`
- `LivesUI`

### Level Geometry
- **`Foreground`** — platform colliders (tilemap or sprite-based)
- **`Background`** — non-collidable art layers
- **`MapLayout`** — parent container organizing the Start → Fruits → Vegetables → Store → Meat → Fish → Rice flow
