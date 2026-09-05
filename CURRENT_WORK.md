# Current Work — Open Classroom

Last updated: 2026-09-05 (Europe/Amsterdam)

## AUTHORITATIVE CURRENT STATUS

**The StefanieInVR Presentation Service is live. The old Classroom Presentation integration has been dismantled. The reusable Presentation prefab is now being redesigned as a standalone, video-player-independent Core.**

Important current truth:

- do not describe the current Classroom as beta-ready with the Presentation Service;
- the previous VideoTXL-bound Presentation setup is not the current architecture;
- GitHub currently does not contain a committed standalone `PresentationController.cs` implementation;
- the real Unity project remains the source of truth for what actually exists in-scene;
- VideoTXL 2.5.1 remains installed in the Classroom and will later receive an optional typed integration;
- the standalone product itself must not depend on VideoTXL.

Read `PRESENTATION_ARCHITECTURE_DECISION.md` before implementation.

## REAL UNITY PROJECT

Use:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

Do not confuse it with the older checkout:

`E:/GitHub/Open_Classroom`

## ACTIVE PRODUCT DIRECTION

Target product:

```text
Standalone StefanieInVR Presentation Core
-> one own VRCUnityVideoPlayer
-> own presentation state/sync
-> own rollout Presentation Screen by default
-> optional Renderer/material output
-> optional VideoTXL adapter for Stef's Classroom
```

The Core must compile and work without VideoTXL, ProTV or USharpVideo.

## ACCEPTED V1 NETWORK MODEL

Synchronize only:

```text
modeActive
slotIndex
slideIndex
revision
```

All media handling happens locally on every client:

```text
receive state
-> load selected slot locally if needed
-> seek locally to requested slide
-> pause locally
```

Do not synchronize continuous playback time for a normally paused presentation.

## ACCEPTED V1 PLAYBACK MODEL

Use one own `VRCUnityVideoPlayer`.

Within the same slot:

```text
load MP4 once
-> seek
-> pause
-> Next / Previous = seek again
-> keep the MP4 loaded
```

Do not make snapshot-and-stop the default V1 behavior.

Snapshot/RenderTexture hold is only a later performance experiment if Quest measurements show a real benefit.

## QUEST / PERFORMANCE RULE

Stef's practical baseline is that one active video player already works acceptably on Quest in the current world.

V1 design goal:

```text
NORMAL
normal world video player active
Presentation player stopped

PRESENTATION MODE
normal player locally suspended/stopped where safely supported
Presentation player active / paused on current slide

EXIT
Presentation player stopped
normal player restored/resynced
```

Do not intentionally run both playback pipelines at once.

Do not blindly Stop an unknown third-party player's internal `BaseVRCVideoPlayer`.

The standalone prefab may require the creator to manage their unrelated video player themselves when no supported adapter exists.

## OPEN CLASSROOM VIDEOTXL INTEGRATION

VideoTXL is now an **optional integration**, not the Presentation Core.

For Stef's Classroom, preferred integration is:

- use VideoTXL 2.5.1;
- on Presentation enter: `LocalPlaybackEnabled = false`;
- use the standalone Presentation player for slot/slide playback;
- optionally route Presentation output onto the existing VideoTXL screen;
- on Presentation exit: stop Presentation playback, restore screen state, then set `LocalPlaybackEnabled = true`;
- let VideoTXL restore/resync through its own logic.

Do not use VideoTXL `_TriggerPause` as a local suspension API because it changes synchronized pause state.

The exact 2.5.1 source research remains useful for this adapter.

## LIVE HOSTED SERVICE CONTRACT

Cross-project truth from `mailfromstefanie/StefanieInVR-Presentation-Service`:

- ten-slot Presentation Service Free Beta = LIVE;
- hosted input = PDF;
- one slide = one second;
- stable Presentation Slot MP4s;
- current live web uploader uses the proven slot-code flow;
- the prepared Username-only personal dashboard is not live.

The reusable prefab must also allow creators to configure their own compatible MP4 URLs/hosting.

## CURRENT NEXT IMPLEMENTATION ORDER

```text
1. build/inspect standalone Core on a dedicated test screen
2. prove one direct MP4 Slot locally
3. prove seek + pause
4. prove Previous / Next / First
5. add mode/slot/slide/revision sync
6. prove late join / ownership
7. prove Quest
8. harden reusable prefab
9. build VideoTXL 2.5.1 adapter for Open Classroom
10. optionally reuse the existing VideoTXL physical screen
11. later consider ProTV / other integrations only if useful
```

Do not start by creating a VideoTXL Presentation Playlist. That earlier design is superseded.

## RESEARCH MATERIAL

Research folder:

`docs/research/standalone-presentation-player/`

The Work-generated architecture report will be uploaded there.

Research evidence is not automatic project truth. Accepted decisions are recorded in:

- `PRESENTATION_ARCHITECTURE_DECISION.md`
- `PRESENTATION_INTEGRATION_PLAN.md`
- this file

## CURRENT CROSS-PROJECT PRIORITY

Current order:

1. standalone Presentation prefab/core;
2. VideoTXL integration into Open Classroom;
3. once stable, later integrate the same Presentation product into Art House Cinema;
4. Website Editor and eReader/eBook work remain parked unless Stef explicitly reprioritizes them.

Cinema remains a separate project.

## WORKING STYLE WITH STEF

```text
ChatGPT explains one tiny Unity action
-> Stef performs it
-> inspect result
-> complete script file if needed
-> wire exact references
-> test
-> record meaningful proof
-> continue
```

Rules:

- Dutch, beginner-friendly;
- one small Unity action at a time;
- complete scripts, never partial fragments;
- no destructive scene changes without inspection;
- no CMD/PowerShell unless unavoidable;
- ClientSim/editor evidence is not final PC/Quest/multiplayer proof.

## GITHUB TRUTH RULE

GitHub is durable project memory, but the tested Unity scene can be newer than the repository snapshot.

Never overwrite tested scene truth with an older planning assumption.
