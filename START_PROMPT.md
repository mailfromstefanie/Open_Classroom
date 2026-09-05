# Start Prompt — Open Classroom / StefanieInVR Presentation

Use this file to start a fresh ChatGPT session about the current Open Classroom Presentation system.

## Projects

Primary repository:

`mailfromstefanie/Open_Classroom`

Hosted service:

`mailfromstefanie/StefanieInVR-Presentation-Service`

Later integration target:

`mailfromstefanie/Stefanies-Art-House-Cinema`

Real Unity project:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

## Read first

1. `AGENTS.md`
2. `CURRENT_WORK.md`
3. `PRESENTATION_ACCEPTANCE_2026-09-05.md`
4. `PRESENTATION_ARCHITECTURE_DECISION.md`
5. `PRESENTATION_INTEGRATION_PLAN.md`
6. `VIDEOTXL_2_5_1_FINDINGS.md` only when VideoTXL internals are relevant
7. exact VideoTXL 2.5.1 checked-in source only when implementation details are needed
8. Presentation Service `CURRENT_WORK.md` for hosted/live truth
9. Cinema `CURRENT_WORK.md` only when deliberately moving to Cinema

## Exact current truth

**The Open Classroom Presentation system is working and the previously open Presentation blockers are closed in the tested environment.**

Do not start by rebuilding or re-debugging the system.

Current working architecture:

```text
Standalone Presentation Core
-> own VRCUnityVideoPlayer
-> sync modeActive + slotIndex + slideIndex + revision
-> local MP4 load/seek/pause per client
-> RT_PresentationVideo
-> Open Classroom VideoTXLPresentationAdapter
-> VideoTXL 2.5.1 ScreenManager
-> existing physical projector screen
```

The reusable Core itself remains independent of VideoTXL.

The old VideoTXL Presentation Playlist design is superseded.

## Proven behavior

Current reported working behavior includes:

- 10 Presentation slot URLs;
- automatic slide-count detection;
- First / Previous / Next;
- two-client synchronization;
- cross-client slide control;
- Presentation ON/OFF synchronization;
- VideoTXL restoration on both clients;
- resume same Presentation slot/slide after OFF/ON;
- late-join reconstruction;
- physical screen output;
- projector open/close behavior;
- brightness/contrast behavior;
- tablet UI after hierarchy cleanup.

## Important preserved implementation rules

### Resume

Presentation OFF preserves synced slot/slide.

Presentation ON resumes that saved slot/slide.

Selecting a different slot intentionally starts that slot at slide 1.

### VideoTXL

During Presentation:

`SyncPlayer.LocalPlaybackEnabled = false`

locally only.

Do not use `_TriggerPause()` as the suspend mechanism.

On exit restore the VideoTXL screen state and local playback.

### Display

Do not replace the working:

- physical VideoScreen;
- VideoTXL/Unlit-based material/shader;
- ScreenManager path;
- projector visibility system;
- brightness/contrast system.

The old screen-fill issue is reported solved in the real scene.

### Hierarchy

Current preferred organization:

- `PresentationCore` under `UIs/Managers`;
- Presentation canvas/UI integrated into the physical tablet.

## Closed incident

Do not reopen the old VideoTXL SourceManager failure unless new evidence appears.

Cause was one stale null source left by the removed old Presentation playlist.

It was removed and normal VideoTXL works again.

## Current status / next broad decision

```text
Presentation Service = LIVE
Open Classroom Presentation = WORKING / BETA-READY
Cinema Presentation = NOT INTEGRATED YET
```

If Stef wants to continue Open Classroom, preserve the working state and only make deliberate feature/prefab-polish changes.

If Stef wants to move to the next broad project phase, read the Cinema repository and resume its existing control/menu/reset/admin route before integrating the proven Presentation product.

Quest-specific headset acceptance is not separately documented in this chat; do not fabricate a Quest PASS.

## Working style

- speak Dutch;
- do not ask Stef to reconstruct old work;
- do not re-test already-proven behavior without a reason;
- complete scripts only, never fragments;
- inspect the real Unity scene before changing working systems;
- GitHub is project memory, but tested Unity scene truth can be newer.
