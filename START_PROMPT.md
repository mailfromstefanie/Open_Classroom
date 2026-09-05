# Start Prompt — Open Classroom / Standalone Presentation Prefab

Use this file to start the next fresh ChatGPT session.

## Project

Primary repository:

`mailfromstefanie/Open_Classroom`

Related hosted service:

`mailfromstefanie/StefanieInVR-Presentation-Service`

Later target:

`mailfromstefanie/Stefanies-Art-House-Cinema`

## Read first

1. `AGENTS.md`
2. `CURRENT_WORK.md`
3. `PRESENTATION_ARCHITECTURE_DECISION.md`
4. `PRESENTATION_INTEGRATION_PLAN.md`
5. `VIDEOTXL_2_5_1_FINDINGS.md` only when working on the VideoTXL adapter
6. `REFERENCE/VideoTXL_2.5.1/README.md` and the exact package source when VideoTXL implementation details are needed
7. Presentation Service `CURRENT_WORK.md` for hosted/live truth

## Exact current truth

- Presentation Service Free Beta is live.
- The previous Classroom Presentation integration has been dismantled.
- Do not describe the current Classroom as beta-ready with the Presentation Service.
- The reusable Presentation product is now intentionally **standalone and video-player independent at Core level**.
- VideoTXL is no longer the default Presentation playback/sync authority.
- The Core will use one own `VRCUnityVideoPlayer` in V1.
- Default standalone display = own rollout Presentation Screen.
- Optional existing Renderer/material output may be added.
- Stef's own Classroom will later get a typed VideoTXL 2.5.1 integration.
- The repository does not yet contain the standalone Presentation implementation.

Real Unity project:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

## Accepted architecture

Shared state only:

```text
modeActive
slotIndex
slideIndex
revision
```

All clients handle playback locally:

```text
receive state
-> load selected slot if needed
-> seek locally
-> pause locally
```

No continuous video-time sync is required for normal slide holding.

V1 same-slot navigation:

```text
load MP4 once
-> seek
-> pause
-> Next / Previous / First = local seek again
```

Do not make snapshot-and-stop the default. Snapshot is only a later performance experiment if Quest testing demonstrates a need.

## Quest rule

Try to keep only one playback pipeline intentionally active at a time.

Standalone product must not blindly stop unknown third-party internal video components.

For Stef's VideoTXL Classroom integration, use the exact checked-in 2.5.1 reference at:

`REFERENCE/VideoTXL_2.5.1/com.texelsaur.video-2.5.1/`

Preferred suspension path:

`SyncPlayer.LocalPlaybackEnabled = false`

while Presentation Mode is active, then restore it to `true` after the Presentation player stops and the screen state is restored.

## First implementation goal

Do **not** start by creating a VideoTXL Presentation Playlist.

Build the smallest standalone proof first:

```text
one standalone VRCUnityVideoPlayer
-> one known direct Presentation MP4
-> one dedicated test screen
-> load
-> seek first/middle/last slide
-> pause
```

Then:

- Next / Previous / First;
- sync mode/slot/slide/revision;
- second client / late join;
- Quest;
- prefab hardening;
- only then VideoTXL 2.5.1 integration into Open Classroom.

## How to work with Stef

- Speak Dutch.
- One tiny Unity action at a time.
- Explain what the action is meant to prove.
- Stef performs Unity scene work manually.
- Give full replacement scripts when code is needed; never snippets.
- No CMD/PowerShell unless unavoidable.
- Do not make destructive scene changes without inspection.
- Update GitHub after meaningful decisions or proof.

## Research material

The Work architecture report belongs in:

`docs/research/standalone-presentation-player/`

Treat research as evidence, not automatically accepted truth. The accepted decision is in `PRESENTATION_ARCHITECTURE_DECISION.md`.

Do not start Website Editor, eReader or Cinema implementation unless Stef explicitly changes priority.
