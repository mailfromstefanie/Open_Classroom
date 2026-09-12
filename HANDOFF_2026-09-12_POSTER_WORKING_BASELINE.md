# Handoff — 2026-09-12 — Manual Poster Rebuild / Working Baseline

## Authority

This file is the newest Open Classroom session handoff and overrides older poster status in:
- `HANDOFF_2026-09-11_POSTER_REPAIR.md`;
- `PERSISTENT_POSTER_IMPLEMENTATION_2026-09-10.md`;
- older poster sections in `CURRENT_WORK.md`.

Real Unity project:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

The real tested Unity scene remains stronger truth than copied GitHub source/reference files.

## Current project state

The rejected generated poster architecture from 2026-09-10/11 was not repaired forward. Stef manually rebuilt the poster in small steps using the existing tablet style and a simple physical hierarchy.

The new manual poster has progressed far beyond Gate 1:
- direct image loading works;
- trusted direct Imgur URLs work;
- current-instance URL sharing was tested between two players and works;
- physical pickup/root architecture is in place;
- uniform scale 1x through 5x works locally after correcting the slider semantics;
- reset hooks exist;
- automatic emission + shared unload were added at the end of the session but still need acceptance testing;
- future persistence is the next feature after those tests pass.

Stef made a fresh backup before the next acceptance pass.

## Current poster hierarchy

Observed working hierarchy:

```text
Persistent_Poster                 <- physical root
├─ VRC Pickup
├─ Rigidbody (kinematic, gravity off)
├─ VRC Object Sync                <- EXISTING, do not add a second one
├─ PosterImageLoaderLocal
├─ Scale_Root
│  ├─ Persistent_Poster           <- actual square image surface
│  └─ Persistent_Poster_Edge      <- visual edge only
├─ Persistent_Poster_Handle       <- outline/highlight renderer
└─ Poster Grip                    <- exact grip transform

Managers
├─ Poster Shared State
└─ Poster Scale Manager
```

Important physical decisions:
- poster remains square;
- no automatic aspect-ratio fitting system;
- non-square images may stretch;
- scale is uniform only;
- the edge is visual only;
- VRCObjectSync on the top `Persistent_Poster` owns live position/rotation synchronization;
- do not add another VRCObjectSync for scaling.

## Pickup / outline

The poster follows the successful e-reader pickup pattern:
- one main pickup/collider root;
- `Persistent_Poster_Handle` is used as the VRC Pickup Outline Renderer;
- handle mesh does not need its own interaction collider;
- `Poster Grip` is assigned as Exact Grip;
- Pickup orientation = Grip;
- Auto Hold is enabled.

This gives a consistent held orientation without forcing world-upright behaviour.

## Direct image loading — accepted evidence

Current loader script:

`PosterImageLoaderLocal.cs`

Current shared state:

`PosterSharedState.cs`

Known direct test URLs:

```text
https://i.imgur.com/klSe3ij.jpg
https://i.imgur.com/GhVNVXv.jpeg
https://i.imgur.com/oaEbiM2.png
```

Important:
- use direct final image URLs (`i.imgur.com/...jpg/png/jpeg`), not ordinary `imgur.com` gallery/page URLs;
- generic valid direct HTTPS image URLs remain the intended product behaviour;
- own-domain URLs may require Allow Untrusted URLs.

Proven current-instance network result:
- Player A publishes the poster URL;
- `PosterSharedState` obtains ownership using the delayed one-frame/retry pattern copied from the proven Entrance Text manager;
- `[UdonSynced] VRCUrl sharedUrl` is serialized;
- Player B receives it through `OnDeserialization()`;
- Player B loads the same image locally;
- Stef explicitly reported this two-player URL/image sync working.

Late join for the poster has not yet been explicitly accepted in this rebuild.

## Current `PosterSharedState` ownership pattern

Do not regress to immediate `SetOwner -> RequestSerialization` in the same frame.

The working pattern is:

```text
request change
-> SetOwner if needed
-> SendCustomEventDelayedFrames(..., 1)
-> verify ownership
-> retry up to 20 frames
-> write synced state
-> RequestSerialization
```

This is the same reason the Entrance Text publisher became reliable.

## Scale — working local baseline

Current script:

`PosterScaleManager.cs`

Current intended Inspector values:

```text
Poster Scale Manager
Min Scale      = 1
Max Scale      = 5
Default Scale  = 1
Sync Delay     = 0.35

Slider (Scale Poster)
Min Value      = 1
Max Value      = 5
Value          = 1
Whole Numbers  = OFF
```

Slider wiring:

```text
On Value Changed
-> Poster Scale Manager (UdonBehaviour)
-> SendCustomEvent(string)
-> OnScaleSliderChanged
```

Semantics:

```text
slider 1 = 1x original poster size
slider 2 = 2x
slider 3 = 3x
slider 4 = 4x
slider 5 = 5x
```

The original `Scale_Root.localScale` at Start is cached as the 1x base.

Network behaviour:
- local scale reacts immediately;
- every slider movement resets the timer;
- synchronization occurs after about 0.35 s without movement;
- this is intentional and Quest-friendly compared with serializing every drag tick.

Local scaling was explicitly reported working after replacing the earlier normalized 0..1 implementation.
A fresh two-player scale acceptance pass is still recommended.

## Reset behaviour

`PosterScaleManager` exposes:

```text
ResetScale
ResetPosition
```

`ResetScale` returns to default `1x` and synchronizes the scale.

`ResetPosition` uses the EXISTING `VRCObjectSync` on the top `Persistent_Poster` and calls `Respawn()` only after obtaining ownership of that physical root.

For one combined UI reset button, use two OnClick events:

```text
Poster Scale Manager -> SendCustomEvent -> ResetScale
Poster Scale Manager -> SendCustomEvent -> ResetPosition
```

Reset-position acceptance was not clearly closed this session; do not call it proven until retested.

## Emission — latest change, test still pending

Stef does NOT want an emission toggle.

Desired behaviour:

```text
no loaded poster
-> emission OFF

poster image successfully loaded
-> same downloaded texture also assigned to emission texture slot
-> emission ON

Unload
-> normal poster texture restored/cleared to original default
-> emission texture cleared
-> emission OFF
```

Current reference script:

`Scripts/UIManagers/PosterImageLoaderLocal.cs`

Default property names currently assumed:

```text
_EmissionMap
_EmissionColor
```

If the chosen poster shader uses different property names, only these property names should be adjusted; do not redesign the loader first.

The emission addition was made immediately before backup/testing and is NOT yet accepted in real play.

## Unload button — latest change, test still pending

Unload must be shared, not local-only.

Current architecture:
- `PosterImageLoaderLocal.UnloadImage()` performs local material cleanup;
- `PosterSharedState` now has synced `sharedHasPoster` in addition to `sharedUrl`;
- `PosterSharedState.UnloadPoster()` publishes `sharedHasPoster = false`;
- every client then calls `imageLoader.UnloadImage()` through shared-state application.

Unload button wiring:

```text
Button (Unload Image)
-> Poster Shared State (UdonBehaviour)
-> SendCustomEvent(string)
-> UnloadPoster
```

Do NOT wire the button directly to `UnloadImage`, because that would only unload locally.

Current Load button remains:

```text
Poster Shared State
-> SendCustomEvent
-> PublishUrl
```

Latest unload/emission code is stored as reference copies in:
- `Scripts/UIManagers/PosterImageLoaderLocal.cs`;
- `Scripts/UIManagers/PosterSharedState.cs`.

These latest changes still require local then multiplayer acceptance.

## TXLScreenAutoVisibility compile incident

While adding/recompiling Udon poster scripts, UdonSharp repeatedly surfaced an old custom-script error:

```text
Field is not exposed to Udon: 'txlPlayer.playerState'
```

This was NOT caused by PosterScaleManager logic; adding/changing an Udon script caused a broader compile that exposed the stale direct VideoTXL runtime-field access.

Important:
- do NOT modify VideoTXL 2.5.1 itself;
- do NOT remove the manager-side `TXLScreenAutoVisibility` script;
- an old duplicate copy under `Assets/#Classroom/Scripts/Only_Visible_If_Blendshape_Is_Toggle/` was removed by Stef;
- the intended manager script remains under the real project manager scripts.

The corrected reference implementation is now stored in:

`Scripts/UIManagers/TXLScreenAutoVisibility.cs`

It no longer uses:

```text
txlPlayer.playerState
txlPlayer.paused
```

directly. It reads runtime variables through `UdonBehaviour.GetProgramVariable("playerState")` and `GetProgramVariable("paused")`.

Crucially it preserves Presentation integration:

```text
projector open
AND
(VideoTXL should be visible OR presentationController.modeActive)
```

Therefore Presentation Mode can keep the physical screen visible while VideoTXL local playback is suspended.

This corrected script still needs a regression acceptance pass covering:
1. normal VideoTXL playback;
2. switch Video -> Presentation;
3. Presentation slide use;
4. switch Presentation -> Video;
5. pause/play;
6. physical projector open/close visibility.

Do not call this regression closed until Stef tests it.

## Protected Presentation truth

Do not change the established Presentation architecture while testing the visibility helper.

Preserve:
- Presentation Core independent of VideoTXL;
- `VideoTXLPresentationAdapter` local suspend/restore path;
- exact VideoTXL 2.5.1;
- `SyncPlayer.LocalPlaybackEnabled` local suspension behaviour;
- existing ScreenManager/render path;
- proven same-slot/same-slide resume;
- late join and two-client Presentation behaviour already proven earlier.

The current compile fix is only about how the visibility helper observes VideoTXL state.

## Entrance Text — current useful persistence reference

Entrance title/body is now a proven architectural reference for a hybrid state model:
- personal persistence via PlayerData;
- current-instance shared state via `[UdonSynced]`;
- ownership acquisition before serialization;
- `OnPlayerRestored` used for personal persisted data.

Do not reopen Entrance Text logic during poster persistence unless evidence requires it.

## Poster persistence — NOT IMPLEMENTED YET

This is the next feature only AFTER emission/unload and the TXL visibility regression pass are stable.

Desired persistent poster state:

```text
URL / has-poster state
position
rotation
scale
```

Important design correction from this session:
- PlayerData can persist ordinary scalar/vector/string values;
- the restored URL is awkward if stored only as string because runtime reconstruction of arbitrary `VRCUrl` is restricted;
- the planned solution to investigate/implement is a persistent PlayerObject / `VRCEnablePersistence` storage layer for the secure synced `VRCUrl` plus the rest of the poster state, while keeping the ONE real physical poster in the scene;
- do not put a PlayerObject component on the physical pickup itself and accidentally create per-player physical poster copies.

Likely separation:

```text
LIVE CURRENT INSTANCE
PosterSharedState        -> URL + hasPoster
VRCObjectSync            -> position + rotation
PosterScaleManager       -> scale

FUTURE PERSISTENCE STORAGE
persistent PlayerObject  -> saved URL/hasPoster/pose/scale for the authorized teacher
```

Persistence semantics must remain user-bound unless an external backend is intentionally introduced later.
Do not describe it as a global world database shared across unrelated users/instances.

## Exact next session route

Do NOT start by writing persistence code immediately.

First acceptance gate:

1. Confirm Unity/Udon compile is clean with the corrected `TXLScreenAutoVisibility`.
2. Test Presentation <-> VideoTXL switching regression.
3. Test poster locally:
   - Load direct image;
   - image visible;
   - emission becomes active;
   - Unload;
   - image removed/restored to default;
   - emission off.
4. Test with two players:
   - Load propagates;
   - Unload propagates;
   - scale agreement;
   - move/rotate agreement;
   - reset scale/position;
   - late join if practical.
5. Only when these pass, begin poster persistence as a separate storage layer.

## Working style reminder

Stef wants the manual Nova/ChatGPT route here:
- one small Unity action at a time;
- explain why before the action;
- inspect screenshots rather than inventing scene state;
- prefer complete scripts when code replacement is needed;
- no autonomous visual redesign;
- protect working systems;
- Codex only for bounded investigation/implementation when Stef explicitly wants it.

## Backup

At the end of this session Stef explicitly stopped and made a backup before testing further.
Treat that backup as the new safety point for the current manual poster rebuild.
