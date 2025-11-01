# Unity Essentials

This module is part of the Unity Essentials ecosystem and follows the same lightweight, editor-first approach.
Unity Essentials is a lightweight, modular set of editor utilities and helpers that streamline Unity development. It focuses on clean, dependency-free tools that work well together.

All utilities are under the `UnityEssentials` namespace.

```csharp
using UnityEssentials;
```

## Installation

Install the Unity Essentials entry package via Unity's Package Manager, then install modules from the Tools menu.

- Add the entry package (via Git URL)
    - Window → Package Manager
    - "+" → "Add package from git URL…"
    - Paste: `https://github.com/CanTalat-Yakan/UnityEssentials.git`

- Install or update Unity Essentials packages
    - Tools → Install & Update UnityEssentials
    - Install all or select individual modules; run again anytime to update

---

# Camera Focus Point Ray Caster

> Quick overview: Forward ray- or sphere‑cast to derive camera focus distance, with separate near/far speeds, layer filtering, debug gizmo, and optional Cinemachine integration.

Automatic focus distance is derived each frame from what the camera looks at. A straight ray or a small weighted sphere can be cast forward to find a target point; the resulting focus distance is smoothed with different speeds for moving nearer or farther. When a Cinemachine camera is present, its physical lens focus is updated; otherwise the built‑in camera’s physical focus distance is used.

![screenshot](Documentation/Screenshot.png)

## Features
- Two focus detection modes
  - Spot mode (Raycast): precise hit along the camera forward
  - Centered Weight (SphereCast): weighted area around the center using `WeightRadius`
- Smooth focus transitions
  - Independent speeds for Near→Far and Far→Near changes
  - Exposes `CurrentFocusDistance` and `CurrentTargetPoint` for debugging/UX
- Configurable range and layers
  - `MaxDistance` cap and `DefaultDistance` fallback
  - LayerMask filtering to restrict valid focus targets
- Rendering integrations
  - Cinemachine: sets `CinemachineCamera.Lens.PhysicalProperties.FocusDistance` when available
  - Built‑in/URP/HDRP: falls back to `Camera.focusDistance` (physical camera)
  - A Depth‑of‑Field volume prefab may be instantiated from `Resources` for convenience
- Debug visualization
  - Optional wire sphere gizmo at the current target point (radius reflects mode)
- Lightweight runtime
  - Single component; per‑frame physics query and Lerp smoothing

## Requirements
- Unity 6000.0+
- A Camera with Physical Camera enabled if using `Camera.focusDistance`
- Valid colliders on target geometry and a correctly configured LayerMask
- Optional: Cinemachine (with `CinemachineCamera`) for direct lens control
- Optional: a DoF volume setup in your pipeline (URP/HDRP) to visualize focus blur

## Usage
1) Add to a Camera
   - Select your Camera and add `CameraFocusPointRaycaster`
   - If using Cinemachine 3, ensure `CinemachineCamera` is present on the same GameObject
2) Configure settings
   - Range: `MaxDistance`, `DefaultDistance`
   - Layers: set `Layers` to include focusable geometry
   - Mode: toggle `CenteredWeight` and tune `WeightRadius` for sphere casting
   - Smoothing: set `SpeedNearToFar` and `SpeedFarToNear`
   - Debug: toggle `ShowDebugSphere`
3) Play and observe
   - The component updates focus distance toward the first hit along the view direction
   - If nothing is hit, the distance trends toward `DefaultDistance` and target point at `MaxDistance`

## How It Works
- Origin/direction are taken from the Camera transform each frame
- Depending on mode:
  - Raycast: the first hit point determines target distance
  - SphereCast: a sphere is swept forward; hit distance is expanded by `WeightRadius` for a center‑weighted feel
- A focus speed is selected based on whether the target is farther or nearer than the current distance
- `CurrentTargetPoint` and `CurrentFocusDistance` are eased via `Vector3.Lerp`/`Mathf.Lerp`
- The focus distance is applied to either `CinemachineCamera` (if present) or `Camera.focusDistance`

## Notes and Limitations
- Physics dependency: focus computation requires colliders on visible geometry and correct layer setup
- Pipeline behavior: physical camera support and Depth‑of‑Field visualization depend on the active render pipeline and settings
- Cinemachine usage: only applied if a `CinemachineCamera` component is on the same GameObject; otherwise the built‑in camera is used
- Sphere casting: `WeightRadius` influences both detection and the effective distance (distance + radius)
- Resources: a DoF volume prefab named `UnityEssentials_Prefab_CameraDoFVolume` may be instantiated from `Resources`; if unavailable, focus will still be computed and applied

## Files in This Package
- `Runtime/CameraFocusPointRaycaster.cs` – Focus distance computation and application
- `Runtime/UnityEssentials.CameraFocusPointRayCaster.asmdef` – Runtime assembly definition
- `Resources/UnityEssentials_Prefab_CameraDoFVolume` – Optional DoF volume prefab (if present)

## Tags
unity, camera, focus, autofocus, raycast, spherecast, depth-of-field, dof, physical-camera, cinemachine, runtime, rendering
