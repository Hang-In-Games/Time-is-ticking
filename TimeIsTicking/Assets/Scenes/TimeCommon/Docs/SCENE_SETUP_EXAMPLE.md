# TimeSeeker Scene Configuration Example

This document provides step-by-step instructions for setting up a working TimeSeeker scene in Unity.

## Prerequisites

- Unity 2021.3 or later
- Universal Render Pipeline (URP) package installed
- Input System package installed
- TextMesh Pro (for UI)

## Scene Setup Steps

### Step 1: Create Basic Scene Objects

1. Open Unity and create a new scene or use the copied TimeSeeker.unity
2. Set up the Camera:
   - Select Main Camera
   - Set Projection to Orthographic
   - Set Size to 6
   - Set Position to (0, 0, -10)
   - Ensure it has the Renderer2D (URP)

### Step 2: Create Clock Center

1. Create Empty GameObject: `GameObject > Create Empty`
2. Rename to "ClockCenter"
3. Set Position to (0, 0, 0)

### Step 3: Create Clock Border

1. Create Sprite GameObject: `GameObject > 2D Object > Sprite`
2. Rename to "ClockBorder"
3. Set Position to (0, 0, 0)
4. Assign ClockBorder.svg sprite (from TimeBouncer VectorImages)
5. Set Material to SpriteMaterial_URP
6. Scale as needed to fit your design

### Step 4: Create Needle

1. Create Sprite GameObject: `GameObject > 2D Object > Sprite`
2. Rename to "Needle"
3. Set Position to (0, 0, 0)
4. Assign ClockPaddle.svg sprite (from TimeBouncer VectorImages)
5. **Important**: Set Sprite Pivot to (0, 0.5) - left edge
6. Add Rigidbody2D:
   - Body Type: Kinematic
   - Gravity Scale: 0
7. Optionally add NeedleController script

### Step 5: Create UI Canvas

1. Create Canvas: `GameObject > UI > Canvas`
2. Set Render Mode to Screen Space - Overlay
3. Create Score Text:
   - Right-click Canvas > UI > Text
   - Rename to "ScoreText"
   - Set Text to "Score: 0"
   - Position at top-left (Anchor: Top-Left)
   - Set Font Size to 24
4. Create Difficulty Text:
   - Right-click Canvas > UI > Text
   - Rename to "DifficultyText"
   - Set Text to "Difficulty: Easy"
   - Position at top-center
   - Set Font Size to 24
5. Create Timer Text:
   - Right-click Canvas > UI > Text
   - Rename to "TimerText"
   - Set Text to "Time: 60s"
   - Position at top-right
   - Set Font Size to 24
6. Create Combo Text:
   - Right-click Canvas > UI > Text
   - Rename to "ComboText"
   - Set Text to "Combo: 1x"
   - Position at center-top
   - Set Font Size to 32
   - Set Color to Yellow
   - Initially set as Inactive

### Step 6: Create Game Manager

1. Create Empty GameObject: `GameObject > Create Empty`
2. Rename to "GameManager"
3. Add GameManager_TimeSeeker script
4. Configure in Inspector:

   **Scene Objects:**
   - Clock Center: Drag ClockCenter object
   - Clock Border: Drag ClockBorder object
   - Needle: Drag Needle object

   **Clock Settings:**
   - Clock Radius: 180 (adjust to match your ClockBorder size)

   **Needle Settings:**
   - Needle Rotation Speed: 180
   - Use Player Input: ✓ (checked)

   **Difficulty Settings:**
   - Current Difficulty: Easy
   - Leave Easy/Normal/Hard Settings fields empty (auto-generated)

   **Score Settings:**
   - Point Score: 10
   - Line Score: 10
   - Failure Penalty: -5
   - Combo Multiplier: 1.5

   **Game Settings:**
   - Game Time: 60 (0 for unlimited)
   - Target Score: 100

   **UI References:**
   - Score Text: Drag ScoreText
   - Difficulty Text: Drag DifficultyText
   - Timer Text: Drag TimerText
   - Combo Text: Drag ComboText

### Step 7: Verify Setup

1. Check Hierarchy:
   ```
   Scene
   ├─ Main Camera
   ├─ ClockCenter
   ├─ GameManager (with GameManager_TimeSeeker)
   ├─ ClockBorder
   ├─ Needle
   └─ Canvas
      ├─ ScoreText
      ├─ DifficultyText
      ├─ TimerText
      └─ ComboText
   ```

2. Check Inspector assignments on GameManager
3. Ensure all "None (GameObject)" fields are filled

### Step 8: Test the Scene

1. Press Play
2. Check Console for:
   - "TimeSeeker 게임 시작 - 난이도: Easy"
   - "Input System 설정 완료!"
   - No error messages
3. Test controls:
   - Press A or ← to rotate left
   - Press D or → to rotate right
4. Wait for objects to spawn (should happen within 5 seconds on Easy)
5. Move needle to overlap with spawned objects
6. Hold for 1 second to collect and score points

## Troubleshooting

### Objects not spawning
- Check that GameManager is active
- Verify Clock Radius is correct
- Check Console for errors

### Needle not rotating
- Verify Input System package is installed
- Check that "Use Player Input" is enabled
- Try keyboard fallback (A/D keys)

### UI not updating
- Verify all UI Text objects are assigned in GameManager
- Check that Canvas is set to Screen Space - Overlay

### Objects not visible
- Adjust Clock Radius to match your ClockBorder
- Check object colors (may be too dark)
- Verify Camera can see the Z=0 plane

## Optional Enhancements

1. **Background**: Add a solid color sprite behind the clock
2. **Particles**: Add particle effects when objects are collected
3. **Sound**: Add AudioSource to GameManager for sound effects
4. **Animations**: Animate score/combo text with Unity Animator
5. **Post-Processing**: Add bloom/glow effects to objects

## Example Settings for Different Difficulties

### Easy Mode (Beginner Friendly)
- Clock Radius: 180
- Needle Rotation Speed: 120
- Spawn Interval: 5 seconds
- Angle Tolerance: 10 degrees
- Max Objects: 2

### Normal Mode (Balanced)
- Clock Radius: 180
- Needle Rotation Speed: 180
- Spawn Interval: 3 seconds
- Angle Tolerance: 7 degrees
- Max Objects: 3

### Hard Mode (Challenging)
- Clock Radius: 180
- Needle Rotation Speed: 240
- Spawn Interval: 2 seconds
- Angle Tolerance: 5 degrees
- Max Objects: 4

## Performance Tips

- Limit Max Active Objects to 4-5 for smooth performance
- Use object pooling for frequently spawned/destroyed objects (future enhancement)
- Optimize LineRenderer segment count (currently 20, can reduce to 10-15)

## Next Steps

After basic setup works:
1. Customize object colors and sizes
2. Adjust difficulty curve
3. Add sound effects
4. Create level progression system
5. Implement leaderboard
6. Add tutorial/help screen
