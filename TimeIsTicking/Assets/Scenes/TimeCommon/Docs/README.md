# TimeSeeker - Graph Exploration Game

TimeSeeker is a virtual graph exploration game based on the TimeBouncer mechanics.

## Game Overview

Players control a needle (clock hand) that rotates around a circular clock face. Arc-shaped objects (points and lines) randomly spawn on the clock border, and players must track these objects with the needle for a certain duration to score points.

## Key Features

1. **Random Object Generation**
   - Point and Line (Arc) type objects
   - Random positioning on circular border
   - Spawn rate varies by difficulty

2. **Object Movement Patterns**
   - Easy: Static (no movement)
   - Normal: Constant rotation
   - Hard: Erratic movement (oscillation + random changes)

3. **Collision Detection**
   - Needle angle must match object angle within tolerance
   - Requires sustained overlap (dwell time) for success
   - Visual feedback through color changes

4. **Score System**
   - Base score: 10 points per object
   - Combo system: 1.5x multiplier for consecutive hits
   - Visual and numerical feedback

5. **Difficulty Progression**
   - Adjustable spawn intervals
   - Variable movement patterns
   - Changing tolerance ranges

6. **UI Elements**
   - Current score display
   - Difficulty level indicator
   - Timer display
   - Combo counter

## Scripts

- `GameManager_TimeSeeker.cs` - Main game logic controller
- `ArcObject.cs` - Individual arc object behavior
- `ArcObjectData.cs` - Data models and enums
- `NeedleController.cs` - Needle rotation controller
- `ClockBorderColliderSetup.cs` - Clock border setup (copied from TimeBouncer)
- `BrightSpriteSetup.cs` - Sprite utilities (copied from TimeBouncer)

## Setup Instructions

See `SETUP_GUIDE.md` in the Scripts folder for detailed setup instructions.

## Controls

- **A** or **Left Arrow** - Rotate needle counter-clockwise
- **D** or **Right Arrow** - Rotate needle clockwise

## Game Flow

1. Game starts with selected difficulty (Easy/Normal/Hard)
2. Arc objects spawn randomly on the clock border
3. Player rotates needle to align with objects
4. Maintaining alignment for dwell time scores points
5. Game ends when time runs out or target score is reached

## Difficulty Settings

| Difficulty | Spawn Interval | Angle Tolerance | Dwell Time | Max Objects | Movement |
|------------|----------------|-----------------|------------|-------------|----------|
| Easy       | 5 seconds      | 10 degrees      | 1.0s       | 2           | Static   |
| Normal     | 3 seconds      | 7 degrees       | 1.0s       | 3           | Rotating |
| Hard       | 2 seconds      | 5 degrees       | 0.8s       | 4           | Erratic  |

## Extension Ideas

- Multiple arc sizes and shapes
- Special objects (bonus items, penalties)
- Power-up system
- Level progression system
- Leaderboard integration
- Sound effects and music
- Particle effects
