# Start Prompt — Open Classroom / StefanieInVR Presentation Prefab

Use this file to start the next fresh ChatGPT session.

## Project

Primary implementation repository:

`mailfromstefanie/Open_Classroom`

Related hosted service:

`mailfromstefanie/StefanieInVR-Presentation-Service`

Later integration target:

`mailfromstefanie/Stefanies-Art-House-Cinema`

Real Unity project:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

## Read first

1. `AGENTS.md`
2. `CURRENT_WORK.md`
3. `PRESENTATION_ARCHITECTURE_DECISION.md`
4. `PRESENTATION_INTEGRATION_PLAN.md`
5. `VIDEOTXL_2_5_1_FINDINGS.md` only when VideoTXL details are needed
6. `REFERENCE/VideoTXL_2.5.1/README.md` and the exact checked-in 2.5.1 package source when adapter details are needed
7. Presentation Service `CURRENT_WORK.md` for hosted/live service truth
8. Cinema `CURRENT_WORK.md` only when planning the later Cinema integration

## Exact current truth — 2026-09-05

The standalone Presentation work is **past the initial local proof stage**.

Current real Unity implementation already includes:

- standalone Presentation Core with one own `VRCUnityVideoPlayer`;
- ten slot URLs;
- automatic slide count from MP4 duration;
- First / Previous / Next / slot / start-stop controls;
- synced state fields:
  - `modeActive`
  - `slotIndex`
  - `slideIndex`
  - `revision`
- Presentation RenderTexture;
- separate Open Classroom VideoTXL 2.5.1 adapter;
- same existing physical VideoTXL screen used for Presentation;
- existing projector open/close behavior preserved;
- existing custom screen shader preserved;
- existing brightness/contrast controls preserved.

The old VideoTXL Presentation Playlist design remains superseded.

Do **not** recreate it.

## Important fixed VideoTXL problem

The normal VideoTXL playlists temporarily stopped because `SourceManager.sources` still contained one null entry left by the deleted old Presentation playlist.

That stale null source was removed.

Current reported scene truth:

- SourceManager has 21 valid sources;
- ordinary VideoTXL playlists initialize again;
- `LocalPlaybackEnabled` is normally true.

This was not a conflict between the standalone Presentation player and VideoTXL.

Do not reopen this diagnosis without new evidence.

## Current VideoTXL adapter behavior

Normal mode:

```text
Presentation OFF
-> normal VideoTXL playback
-> existing physical screen
-> existing projector visibility
-> existing custom shader
-> existing brightness/contrast
```

Presentation mode:

```text
Presentation ON
-> locally set SyncPlayer.LocalPlaybackEnabled = false
-> do not change VideoTXL shared pause/play state
-> standalone Presentation player loads/seeks/pauses locally
-> Presentation output -> RT_PresentationVideo
-> VideoTXL ScreenManager override -> same physical screen
-> existing screen shader stays final output
```

Exit:

```text
Presentation OFF
-> stop/leave Presentation playback
-> restore previous VideoTXL screen override state
-> LocalPlaybackEnabled = true
-> VideoTXL restores/resyncs through its own logic
```

Never use `_TriggerPause()` as the local suspend mechanism.
Never treat raw internal `BaseVRCVideoPlayer.Stop()` as the integration contract.

## Existing Classroom systems that must survive every fix

### TXLScreenAutoVisibility

Preserve:

- projector-screen blendshape/open gate;
- physical screen Renderer;
- screen colliders;
- normal VideoTXL visibility rules;
- Presentation visibility while VideoTXL is locally suspended.

### Custom screen shader / readability

Preserve the existing physical screen material/shader pipeline.

`ScreenReadabilityManager` brightness/contrast must continue to affect:

- normal VideoTXL;
- Presentation.

Do not solve display problems by replacing the physical screen material or bypassing the readability system.

## ClientSim proof already reported

Do not spend a fresh chat re-proving all of this unless a new change touches it:

- normal VideoTXL works outside Presentation Mode;
- Presentation locally suspends VideoTXL;
- Presentation appears on the existing physical screen;
- projector close/open still controls renderer/collider;
- brightness/contrast affect Presentation;
- readability Reset restores values;
- First / Previous / Next work;
- one Presentation reported 15 slides;
- Stop restores VideoTXL and `LocalPlaybackEnabled=true`;
- no new Presentation compile/runtime errors after render-mode correction.

ClientSim does not equal final multiplayer/Quest proof.

## CURRENT BLOCKER — FIX THIS FIRST

The Presentation is visible on the correct physical VideoTXL screen, but it **does not fill the whole intended screen area**.

Already-known evidence:

- inspected hosted Slot 1 MP4 = 1280x720 = 16:9;
- inspected `RT_PresentationVideo` = 1920x1080 = 16:9;
- inspected MP4 itself does not contain the unwanted outer margins;
- therefore do not start by changing the PDF->MP4 converter;
- VRCUnityVideoPlayer Aspect Ratio dropdown choices are not assumed to solve it because Presentation is now routed through RenderTexture + VideoTXL ScreenManager.

Fresh-chat investigation target:

- actual `VideoTXLPresentationAdapter`;
- real VideoTXL `ScreenManager`;
- real physical screen material/shader;
- VideoTXL property mapping such as `_FitMode` and `_TexAspectRatio`;
- difference between normal VideoTXL display state and Presentation override state.

Goal:

**make the 16:9 Presentation fill the intended physical screen correctly while preserving normal VideoTXL, projector visibility, shader, brightness/contrast and correct restoration on exit.**

Choose the safest technical implementation after inspecting the actual Unity wiring.

## After the screen-fill fix

Do real Build & Test with at least two clients.

Prove:

1. Client 1 starts Presentation.
2. Client 1 selects a slot and changes slides.
3. Client 2 reconstructs the same `modeActive`, `slotIndex`, `slideIndex`, `revision`.
4. Client 2 can press Previous and Client 1 follows.
5. Stop restores normal local VideoTXL on both.
6. Late joiner reconstructs an already-running Presentation.
7. One deliberately early Presentation start after join does not break the VideoTXL adapter.
8. Then prove Quest versus PC.

## Hosted service truth

Presentation Service Free Beta is live.

- 10 stable slot MP4s;
- PDF input;
- one slide = one second;
- current live uploader still uses slot codes;
- Slot 1 is the public demo;
- Username-only personal dashboard is prepared but not live.

The current physical-screen-fill issue is not presently evidence of a hosted converter problem.

## How to work with Stef

- Speak Dutch.
- Stef wants to build quickly; do not force a test after every tiny object.
- Use tests only at meaningful risk gates.
- Explain the purpose of a change in simple language.
- Give complete replacement scripts, never fragments.
- Avoid destructive scene changes before inspection.
- Avoid CMD/PowerShell unless it materially helps.
- Codex/Sol may be used as a bounded Unity worker when direct scene inspection is useful, but normal ChatGPT/Nova remains the orchestrator.
- Update GitHub after meaningful proof/decisions.

## Current order

```text
fix screen fill/aspect
-> real multiplayer sync
-> late join / early-start proof
-> Quest
-> reusable prefab hardening
-> later Cinema integration
```

Do not start Website Editor, eReader/eBook or Cinema implementation unless Stef explicitly changes priority.
