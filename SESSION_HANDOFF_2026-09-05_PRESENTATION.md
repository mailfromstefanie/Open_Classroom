# Session Handoff — Standalone Presentation + Open Classroom Adapter

Date: 2026-09-05 Europe/Amsterdam

## One-line state

The standalone StefanieInVR Presentation Core and its Open Classroom VideoTXL 2.5.1 adapter are substantially working in the real Unity project; the immediate blocker is Presentation screen-fill/aspect on the existing physical VideoTXL screen, followed by real multiplayer, late join and Quest proof.

## Real Unity project

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

## Core architecture preserved

The reusable Presentation Core remains independent from VideoTXL.

Shared semantic state:

```text
modeActive
slotIndex
slideIndex
revision
```

Playback model:

```text
each client
-> load selected MP4 locally
-> seek to slide second
-> pause locally
```

One slide = one second.

Same-slot navigation keeps the MP4 loaded.

## Current local Unity implementation

Reported current implementation includes:

- `PresentationCore`;
- one own `VRCUnityVideoPlayer`;
- ten Presentation slot URLs;
- automatic slide count from video duration;
- Start / Slot / First / Previous / Next / Stop controls;
- menu/status feedback;
- `RT_PresentationVideo`;
- dedicated Open Classroom `VideoTXLPresentationAdapter`;
- existing physical VideoTXL screen reuse.

Codex/Sol reported changes to:

```text
Assets/!StefanieInVR/Scripts/Managers/VideoTXLPresentationAdapter.cs
Assets/!StefanieInVR/Scripts/Managers/VideoTXLPresentationAdapter.asset
Assets/!StefanieInVR/Scripts/Managers/TXLScreenAutoVisibility.cs
Assets/StefanieInVR/Presentation/Scripts/PresentationButton.cs
Assets/StefanieInVR/Presentation/Scripts/PresentationMenuFeedback.cs
Assets/StefanieInVR/Presentation/Scripts/PresentationMenuFeedback.asset
Assets/StefanieInVR/Presentation/Materials/RT_PresentationVideo.renderTexture
Assets/StefanieInVR/Presentation/Materials/M_PresentationScreen.mat
Assets/#Classroom/Scenes/Classroom.unity
```

Associated Unity `.meta` files were generated.

The real Unity project remains source of truth if these exact local files are newer than repository copies.

## VideoTXL SourceManager failure found and fixed

Symptom:

- all ordinary VideoTXL playlists stopped working after the old Presentation integration was removed.

Actual cause:

- `SourceManager.sources` still contained one null reference at element 15;
- VideoTXL 2.5.1 attempted to bind every source during startup;
- the null entry caused a runtime failure around `SourceManager.cs` line 61;
- SourceManager initialization stopped.

Fix:

- removed the single null/stale source entry;
- list now contains 21 valid sources;
- no ordinary playlists or VideoTXL source code were redesigned.

Result:

- VideoTXL initializes again;
- normal playlists work again;
- `LocalPlaybackEnabled` is normally true.

This incident is closed.

## Adapter architecture chosen

### Normal mode

```text
Presentation OFF
-> normal VideoTXL playback/output
-> existing physical screen
-> existing projector visibility
-> existing custom shader
-> existing brightness/contrast
```

### Presentation mode

```text
Presentation ON
-> locally set SyncPlayer.LocalPlaybackEnabled = false
-> do not change VideoTXL shared pause/play state
-> standalone Presentation player remains playback authority
-> Presentation renders to RT_PresentationVideo
-> VideoTXL 2.5.1 ScreenManager texture override places Presentation on existing physical screen
```

### Exit

```text
Presentation OFF
-> stop/leave Presentation playback
-> restore previous VideoTXL ScreenManager override state
-> LocalPlaybackEnabled = true
-> allow VideoTXL to restore/resync through its own logic
```

Hard rules:

- no `_TriggerPause()` for local suspension;
- no raw internal `BaseVRCVideoPlayer.Stop()` integration contract;
- use exact checked-in VideoTXL 2.5.1 source.

## Existing screen systems preserved

### TXLScreenAutoVisibility

Must remain authoritative for:

- projector blendshape/open state;
- physical screen Renderer;
- physical screen colliders.

It now also permits Presentation display while VideoTXL local playback is suspended.

### Custom shader + readability

The existing physical screen custom VideoTXL/Unlit-based material/shader remains the final display path.

`ScreenReadabilityManager` brightness/contrast must work for:

- normal VideoTXL;
- Presentation.

Do not replace the material/shader to solve Presentation display issues.

## ClientSim proof reported

PASS:

- ordinary VideoTXL works outside Presentation Mode;
- Presentation locally suspends VideoTXL;
- Presentation appears on same physical screen;
- projector close/open controls renderer + collider during Presentation;
- brightness/contrast sliders affect Presentation;
- readability Reset works;
- First / Previous / Next work;
- slide navigation included `0 -> 1 -> 0`;
- loaded Presentation reported 15 slides;
- Stop restores VideoTXL and `LocalPlaybackEnabled=true`;
- no new Presentation compile/runtime errors after render-mode correction.

Existing unrelated VRCMarker / MarkerUI errors were not touched.

## Current blocker — screen fill/aspect

Presentation content is visible on the correct screen but does not fill the whole intended physical screen.

Evidence already checked:

- hosted Slot 1 MP4 = 1280x720, 16:9, 15 seconds;
- `RT_PresentationVideo` = 1920x1080, 16:9;
- inspected MP4 does not contain the unwanted outer screen margins.

Conclusion:

**do not start by changing the hosted converter.**

Investigate the Open Classroom display handoff:

- `VideoTXLPresentationAdapter`;
- VideoTXL ScreenManager texture override state;
- ScreenManager fit/aspect state;
- existing physical screen shader/property mapping;
- `_FitMode`;
- `_TexAspectRatio`;
- exact restoration when Presentation stops.

Goal:

Make the 16:9 Presentation fill the intended physical screen while preserving:

- normal VideoTXL;
- projector open/close;
- screen colliders;
- existing custom shader;
- brightness/contrast;
- normal VideoTXL restore/resync.

## Exact next fresh-chat route

1. Read `AGENTS.md`.
2. Read `CURRENT_WORK.md`.
3. Read `PRESENTATION_ARCHITECTURE_DECISION.md`.
4. Read `PRESENTATION_INTEGRATION_PLAN.md`.
5. Inspect the real Unity `VideoTXLPresentationAdapter`, ScreenManager and physical screen material read-only.
6. Fix only the screen-fill/aspect issue.
7. Recheck normal TXL restore.
8. Then do real two-client Build & Test.

## Multiplayer proof still required

ClientSim is not final proof.

Minimum real Build & Test:

```text
Client 1:
Presentation ON
-> Slot
-> Next
-> Next

Client 2:
same mode/slot/slide

Client 2:
Previous

Client 1:
must follow

Stop:
both clients restore normal local VideoTXL
```

Then prove:

- late join reconstruction;
- early Presentation start after join;
- Quest versus PC.

## Current order

```text
screen fill/aspect
-> real multiplayer sync
-> late join / early-start
-> Quest
-> reusable prefab hardening
-> later Art House Cinema integration
```
