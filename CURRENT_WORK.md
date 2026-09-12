# Current Work — Open Classroom

Last updated: 2026-09-12 Europe/Amsterdam

## READ THIS FIRST

Newest authoritative handoff:

`HANDOFF_2026-09-12_POSTER_WORKING_BASELINE.md`

It overrides older poster status in:
- `HANDOFF_2026-09-11_POSTER_REPAIR.md`;
- `PERSISTENT_POSTER_IMPLEMENTATION_2026-09-10.md`;
- earlier recovery/poster notes where they conflict.

Real Unity project:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

The real tested Unity scene remains the strongest source of truth.

## CURRENT STATUS

Open Classroom is again on a usable manual-development baseline after the failed generated poster attempt.

Protected systems that remain important:
- Presentation Core/integration = proven working baseline;
- exact VideoTXL 2.5.1 = protected;
- Entrance title/body persistence architecture = working reference;
- e-reader/library = working baseline with PlayerData reading progress;
- Marker/reset systems = preserve;
- local table screens = preserve;
- existing tablet visual style = source of truth.

Current active work:

**manual Persistent Poster rebuild**

The poster is no longer the rejected generated Cube/Quad system from 2026-09-10/11. Stef rebuilt it manually in small steps.

## POSTER — CURRENT MANUAL ARCHITECTURE

```text
Persistent_Poster                 <- physical pickup root
├─ existing VRCObjectSync         <- live position + rotation
├─ VRC Pickup / Rigidbody
├─ PosterImageLoaderLocal
├─ Scale_Root
│  ├─ Persistent_Poster           <- square image surface
│  └─ Persistent_Poster_Edge      <- visual only
├─ Persistent_Poster_Handle       <- outline renderer
└─ Poster Grip                    <- Exact Grip

Managers
├─ Poster Shared State            <- live URL / hasPoster
└─ Poster Scale Manager           <- live uniform scale
```

Hard rules:
- do NOT add a second VRCObjectSync;
- poster stays square;
- no aspect-ratio automation;
- non-square source images may stretch;
- scaling is uniform only;
- physical position/rotation stays with the top-root VRCObjectSync;
- scale is synchronized separately.

## POSTER — PROVEN SO FAR

### Direct image loading

Working direct image chain:

```text
VRCUrlInputField
-> Poster Shared State / PublishUrl
-> PosterImageLoaderLocal
-> VRCImageDownloader
-> poster material
```

Known working direct Imgur examples:

```text
https://i.imgur.com/klSe3ij.jpg
https://i.imgur.com/GhVNVXv.jpeg
https://i.imgur.com/oaEbiM2.png
```

Use direct final image URLs, not ordinary `imgur.com` gallery/page URLs.
Generic supported direct HTTPS URLs remain the intended product behaviour.

### Multiplayer URL/image sync

Two-player current-instance URL/image synchronization was explicitly reported working.

Important ownership rule:
- `SetOwner` is followed by a delayed frame;
- ownership is verified/retried;
- synced values are written only after ownership is confirmed;
- then `RequestSerialization()` is called.

Do not regress to immediate SetOwner + RequestSerialization in the same frame.

### Scale

Local uniform scaling is working with intuitive values:

```text
Min Scale      = 1
Max Scale      = 5
Default Scale  = 1
Sync Delay     = 0.35

Slider Min     = 1
Slider Max     = 5
Slider Value   = 1
Whole Numbers  = OFF
```

Meaning:

```text
1 = 1x
2 = 2x
3 = 3x
4 = 4x
5 = 5x
```

The original `Scale_Root.localScale` at Start is the 1x base.
Local change is immediate; network publish waits until roughly 0.35 seconds after the slider stops moving.

Two-player scale acceptance is still recommended.

### Pickup

Current pickup design uses:
- kinematic Rigidbody;
- Auto Hold;
- Orientation = Grip;
- Exact Grip = `Poster Grip`;
- outline renderer = `Persistent_Poster_Handle`.

This follows the successful e-reader interaction pattern.

## POSTER — LATEST CHANGES STILL TO TEST

### Automatic emission

There is NO emission toggle.

Desired/latest implementation:

```text
no loaded image
-> emission OFF

successful load
-> same downloaded texture in normal + emission slot
-> emission ON

Unload
-> poster returns to original/default texture
-> emission texture cleared
-> emission OFF
```

Default property names currently assumed:

```text
_EmissionMap
_EmissionColor
```

If the poster shader uses different property names, change only those names first.

### Shared unload

Unload is intentionally shared, not local-only.

Button wiring:

```text
Button (Unload Image)
-> Poster Shared State
-> UdonBehaviour.SendCustomEvent(string)
-> UnloadPoster
```

Do NOT wire the UI directly to `PosterImageLoaderLocal.UnloadImage()` because that would only affect the local client.

Current Load button:

```text
Poster Shared State
-> SendCustomEvent
-> PublishUrl
```

### Reset

`PosterScaleManager` exposes:
- `ResetScale`;
- `ResetPosition`.

Combined Reset button can call both.
`ResetPosition` uses the existing top-root VRCObjectSync `Respawn()` after ownership is obtained.

Reset-position acceptance is not yet closed.

## POSTER SCRIPT REFERENCES IN GITHUB

Latest session reference copies now exist in:

- `Scripts/UIManagers/PosterImageLoaderLocal.cs`;
- `Scripts/UIManagers/PosterSharedState.cs`;
- `Scripts/UIManagers/PosterScaleManager.cs`.

These are reference copies of the latest session code. The real Unity project remains authoritative for actual scene wiring and whether a local file was saved/compiled.

## TXL SCREEN VISIBILITY — CURRENT COMPILE FIX

Adding/recompiling poster Udon scripts exposed an older custom helper problem:

```text
Field is not exposed to Udon: 'txlPlayer.playerState'
```

This is not a poster-scaler design failure.
It came from direct access to VideoTXL runtime fields in custom `TXLScreenAutoVisibility`.

Do NOT edit VideoTXL 2.5.1 itself.

The corrected reference now lives at:

`Scripts/UIManagers/TXLScreenAutoVisibility.cs`

It reads:
- `playerState`;
- `paused`;

through `UdonBehaviour.GetProgramVariable(...)` instead of direct `txlPlayer.playerState` / `txlPlayer.paused` access.

An obsolete duplicate copy under the old `Assets/#Classroom/Scripts/Only_Visible_If_Blendshape_Is_Toggle/` route was deleted by Stef.
The manager-side script must remain.

### Presentation regression requirement

This helper is important for switching between VideoTXL and Presentation/PowerPoint.
It must preserve:

```text
projector open
AND
(VideoTXL visible state OR PresentationController.modeActive)
```

Before closing the fix, test:
1. normal video playback;
2. Video -> Presentation;
3. slide navigation;
4. Presentation -> Video;
5. pause/play;
6. projector open/close.

Do not call this regression PASS until Stef actually tests it.

## PRESENTATION — PROTECTED BASELINE

Preserve the accepted architecture:

```text
Standalone Presentation Core
-> own VRCUnityVideoPlayer
-> own synced semantic presentation state
-> dedicated VideoTXLPresentationAdapter
-> VideoTXL 2.5.1 local suspend/restore
-> existing physical projector/screen
```

Previously proven includes:
- 10 slots;
- First / Previous / Next;
- real two-client synchronization;
- cross-client slide control;
- OFF/ON same slot/slide restore;
- late join;
- VideoTXL local suspend/restore;
- brightness/contrast;
- physical screen output.

Do not rebuild this while finishing the poster.

## ENTRANCE TEXT — REFERENCE, NOT CURRENT TASK

Entrance title/body is the useful persistence/networking reference:

```text
PlayerData
= teacher's personal saved title/body

[UdonSynced]
= current running-instance title/body
```

`OnPlayerRestored()` loads personal state.
Publishing waits for ownership before `RequestSerialization()`.

Do not reopen Entrance Text logic unless new evidence requires it.

## POSTER PERSISTENCE — NEXT FEATURE, NOT YET IMPLEMENTED

Desired persistent state:
- poster URL / hasPoster;
- position;
- rotation;
- scale.

Important current design direction:
- live instance state remains in the existing working components;
- future persistence should be a separate storage layer;
- investigate/use persistent PlayerObject + `VRCEnablePersistence` for secure URL/state persistence where appropriate;
- keep only ONE real physical poster in the scene;
- do not turn the physical pickup into a per-player duplicated PlayerObject.

Likely separation:

```text
LIVE INSTANCE
PosterSharedState   -> URL + hasPoster
VRCObjectSync       -> position + rotation
PosterScaleManager  -> scale

PERSISTENCE LAYER
persistent PlayerObject storage
-> saved URL/hasPoster
-> saved position
-> saved rotation
-> saved scale
```

Persistence remains user-bound unless a deliberate external backend is added later.
Do not describe this as a global world database.

## EXACT NEXT SESSION

Stef made a fresh backup before further testing.

Start with tests, NOT persistence code:

1. Confirm clean Unity/Udon compile.
2. Run the VideoTXL <-> Presentation visibility regression.
3. Test poster locally:
   - Load direct image;
   - image visible;
   - emission ON;
   - Unload;
   - image cleared/restored;
   - emission OFF.
4. Test with two players:
   - Load sync;
   - Unload sync;
   - scale agreement;
   - move/rotate agreement;
   - ResetScale / ResetPosition;
   - late join if practical.
5. Record evidence.
6. Only then implement poster persistence.

## WORKING STYLE WITH STEF

- Dutch;
- beginner-friendly;
- one small technical action at a time;
- explain why before technique;
- inspect screenshots instead of guessing;
- complete script replacements when code changes are needed;
- backup-first at meaningful risk gates;
- no broad refactors;
- no autonomous visual redesign;
- protect Presentation, VideoTXL, e-readers, local table screens and Entrance Text from poster work;
- Codex only for bounded work when Stef explicitly wants it;
- GitHub is durable memory, but tested Unity/VRChat behaviour outranks documentation.

## HISTORICAL FILES

Keep older documents as history/recovery evidence, but do not let them override this file or the 2026-09-12 handoff:
- `HANDOFF_2026-09-11_POSTER_REPAIR.md` = failed generated-poster acceptance and narrow-rebuild decision;
- `PERSISTENT_POSTER_IMPLEMENTATION_2026-09-10.md` = rejected older generated implementation;
- `RECOVERY_DECISION_2026-09-10.md` = protected rollback/recovery context.
