# Travel Animation Asset Setup

## Current Implementation

The `TravelAnimationScreen` is set up to load an animated spritesheet from `"UI/travelAnimation"` with the following specifications:

- **Frame Count**: 30 frames
- **Frame Rate**: 15 FPS (for smooth animation)
- **Duration**: ~2 seconds per loop
- **Size**: Should match your window size (1536x1024)

## Asset Requirements

When you're ready to add the travel animation asset:

1. **Create the spritesheet**: A single horizontal strip containing 30 frames of space travel animation
2. **Name the file**: `travelAnimation.png` (or appropriate format)
3. **Place in Content**: `Content/UI/travelAnimation.png`
4. **Add to Content.mgcb**: Include the asset in your MonoGame Content Pipeline

## Temporary Fallback

Currently, the code includes a fallback that loads the `cockpit` texture as a single-frame animation if the travel animation asset is not found. This allows the system to work immediately while you prepare the actual animation.

## Animation Specifications

- **Layout**: Horizontal strip (all frames in a single row)
- **Individual Frame Size**: 1536x1024 pixels (full screen)
- **Total Image Size**: 46,080x1024 pixels (30 frames × 1536 pixels wide)
- **Content**: Should show a journey through space - stars moving past, maybe some visual effects

## Travel Duration Scaling

The animation duration scales based on distance:
- **Base Duration**: 2 seconds
- **Zone Distance Multiplier**: +1.5 seconds per zone difference
- **Examples**:
  - Inner to Inner: ~2 seconds
  - Inner to Outer: ~3.5 seconds  
  - Inner to Fringe: ~5 seconds
  - Outer to Fringe: ~3.5 seconds

## Player Controls

- **Skip Animation**: Space or Enter key (after 1 second)
- **Auto-complete**: Animation completes automatically after calculated duration
- **Status Display**: Shows origin and destination ports
