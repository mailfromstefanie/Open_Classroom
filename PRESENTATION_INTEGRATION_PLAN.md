# Presentation Prefab / Open Classroom Integration Plan

Last updated: 2026-09-05 (Europe/Amsterdam)

## STATUS

**PRESENTATION SERVICE LIVE — OLD CLASSROOM INTEGRATION DISMANTLED — STANDALONE PREFAB ARCHITECTURE ACCEPTED**

The reusable StefanieInVR Presentation product is no longer planned as a VideoTXL-bound playlist feature.

The current accepted direction is a standalone Core with its own lightweight VRChat video backend, plus optional integrations for existing video systems.

Read first:

1. `PRESENTATION_ARCHITECTURE_DECISION.md`
2. `CURRENT_WORK.md`
3. this file
4. `VIDEOTXL_2_5_1_FINDINGS.md` only when implementing the VideoTXL adapter

## PRODUCT CONTRACT

Hosted Presentation Service:

- 10 stable Presentation Slot MP4 URLs;
- PDF input;
- one PDF slide = one second of video;
- Slot 1 may be used as Stef's demo;
- hosted conversion is independent from the reusable prefab.

Reusable prefab:

- must not depend on Stef's hosting;
- creators can configure their own compatible direct MP4 URLs;
- must work without VideoTXL/ProTV/USharpVideo installed;
- standard display path is its own rollout Presentation Screen;
- optional output can target a compatible existing Renderer/material;
- optional typed integrations may suspend/restore existing video players and reuse their physical screen.

## ACCEPTED STANDALONE ARCHITECTURE

```text
Presentation Controller
-> Presentation Sync
-> Presentation Session/state machine
-> own VRCUnityVideoPlayer
-> own display output

Optional adapter layer
-> VideoTXL
-> later ProTV / others
```

The Core must not reference third-party video-player types.

## NETWORK / MULTIPLAYER MODEL

Shared state:

```text
modeActive
slotIndex
slideIndex
revision
```

Every client performs playback locally.

Do not synchronize continuous video time. The presentation's steady state is a held/paused page, so slot + slide is the useful truth.

Late joiner behavior:

```text
receive latest serialized state
-> if modeActive, locally reconstruct selected slot/slide
-> do not mutate authoritative shared state
```

The revision counter exists to reject stale/out-of-order semantic updates.

## V1 PLAYBACK CONTRACT

Backend:

- one `VRCUnityVideoPlayer`;
- no AVPro in V1 unless later device evidence proves a real need;
- direct MP4 only.

Within one selected slot:

```text
LoadURL once
-> wait until ready
-> seek to target slide
-> pause
-> keep MP4 loaded

Next/Previous/First
-> local seek
-> pause again
```

Do not Stop and reload after every slide.

Reason:

- no snapshot is needed for networking;
- reloading may collide with VRChat's global per-client video URL budget/cooldown;
- keeping one presentation MP4 loaded gives responsive navigation;
- Stef already has a practical Quest baseline where one active video player works fine.

## SNAPSHOT STATUS

A persistent RenderTexture snapshot was suggested by research as an optional performance experiment.

Current decision:

**not part of the default V1 architecture.**

Only revisit snapshot-and-stop if measured Quest testing shows that a paused `VRCUnityVideoPlayer` creates a meaningful performance or thermal problem.

Any snapshot optimization must prove:

- navigation remains responsive;
- no extra URL reload is required for same-slot navigation, or the latency is acceptable;
- Quest performance benefit is measurable;
- frame capture is reliable.

## QUEST / DECODER POLICY

Design target:

**one intentionally active playback pipeline at a time.**

```text
NORMAL MODE
normal world player active
Presentation player stopped

PRESENTATION MODE
normal player locally suspended/stopped where safely supported
Presentation player active / usually paused

EXIT
Presentation player stopped
normal player restored/resynced
```

Important limits:

- Pause is not treated as proof that decoder resources are released.
- Unknown third-party players are not hard-stopped by directly calling their internal `BaseVRCVideoPlayer`.
- A generic integration cannot promise strict one-decoder behavior.
- In the standalone product, creators may need to pause/stop their unrelated player themselves.
- Quest claims require real headset tests.

## DISPLAY OPTIONS

### Standard standalone path

Own rollout Presentation Screen.

This is the guaranteed product path and should require no other media prefab.

### Generic existing-screen path

Allow creator configuration for:

- target Renderer;
- material index/property where practical;
- restoration of the prior state.

Use this only when the material/property is not continuously overwritten by another ScreenManager.

### Typed player integration

For managed video systems, prefer their own display/output API instead of fighting their material updates.

## VIDEOTXL 2.5.1 — OPTIONAL CLASSROOM INTEGRATION

VideoTXL remains the strongest known integration for Stef's own Open Classroom.

Preferred enter path:

```text
Presentation requested
-> VideoTXL LocalPlaybackEnabled = false
-> wait/confirm local VideoTXL playback is suspended as needed
-> activate standalone Presentation player
-> display on rollout screen OR existing VideoTXL physical screen
```

Preferred exit path:

```text
Stop Presentation player
-> restore VideoTXL display state
-> VideoTXL LocalPlaybackEnabled = true
-> let VideoTXL reload/resync from its own synchronized state
```

Do not use `_TriggerPause` for local suspension; it changes synchronized pause state.

Do not call raw VideoTXL internal `BaseVRCVideoPlayer.Stop()` as the integration contract.

## VERIFIED VIDEOTXL 2.5.1 SOURCE EVIDENCE

Stef supplied both official 2.5.1 archives:

- `VideoTXL-2.5.1.zip`
- `com.texelsaur.video-2.5.1.zip`

Core files compared byte-for-byte and confirmed identical include:

- `SyncPlayer.cs`
- `TXLVideoPlayer.cs`
- `Playlist.cs`
- `PlaylistData.cs`
- `SourceManager.cs`
- `VideoManager.cs`

Relevant exact 2.5.1 facts retained for adapter work:

- `SyncPlayer.LocalPlaybackEnabled` has a local playback stop/start path;
- `_TriggerPause()` is synchronized and therefore not a local-suspend substitute;
- `ScreenManager` exposes current/capture texture information and texture overrides;
- VideoTXL ultimately controls VRChat `BaseVRCVideoPlayer` backends;
- 2.6.0-beta.2 keeps the main Presentation-relevant integration surface but introduces ownership/security changes and a newer CommonTXL requirement.

Keep Stef's Classroom on VideoTXL 2.5.1 for now.

## SUPERSEDED VIDEOTXL PLAYLIST DESIGN

Historical research previously concluded:

```text
ONE VideoTXL Presentation Playlist
-> Track 0 = Slot 1
...
-> Track 9 = Slot 10
```

That was a valid design **only while VideoTXL was intended to be the Presentation playback/sync authority**.

It is now superseded for the reusable product.

Do not create a new VideoTXL Presentation Playlist as the first implementation step.

The old exact findings about `Playlist._MoveTo`, `_SetTargetTime`, `_TriggerPause` and final-second behavior remain historical/source evidence and may still be useful when understanding VideoTXL, but they are not the new Core architecture.

## OTHER PLAYER SUPPORT

V1 priority:

1. standalone Core;
2. VideoTXL typed adapter.

Later only if useful:

- ProTV: reduced capability may be possible; do not promise strict local hard-stop equivalence without proof;
- USharpVideo: experimental/unsupported for strict suspension unless its public contract improves;
- unknown players: configurable events/material routing may be offered for advanced users as best effort.

Do not delay the standalone product for broad third-party compatibility.

## USER-FACING V1 CONTROLS

Target:

- Presentation on/off;
- Slot 1 ... Slot 10;
- Previous;
- Next;
- First;
- current slot label;
- current slide / total slides;
- no autoplay slideshow in V1.

Optional later:

- Last;
- direct page entry;
- autoplay/timer;
- pointer/laser;
- custom access filtering;
- external URL entry;
- eBook reuse.

## FIRST IMPLEMENTATION PROOF

Start outside the existing VideoTXL integration.

Smallest proof:

```text
standalone test object
-> one VRCUnityVideoPlayer
-> one known direct Presentation MP4
-> one dedicated test screen
-> load
-> seek first/middle/last slide
-> pause
```

Only after this works:

- Next/Previous/First;
- synced state;
- late join;
- Quest;
- reusable prefab boundary;
- VideoTXL adapter.

## TESTING ORDER

1. clean Unity compile/wiring;
2. one direct MP4 Slot on dedicated screen;
3. first/middle/last slide;
4. repeated Previous / Next / First;
5. rapid input during load/seek;
6. slot switch and URL cooldown behavior;
7. manual sync of mode/slot/slide/revision;
8. second user;
9. late join;
10. Quest;
11. stop/cleanup on Presentation exit;
12. VideoTXL 2.5.1 LocalPlaybackEnabled integration;
13. VideoTXL existing-screen takeover/restore if chosen;
14. only then evaluate snapshot performance optimization;
15. package documentation.

## RESEARCH MATERIAL

Store the Work architecture report under:

`docs/research/standalone-presentation-player/`

Research findings are evidence. Accepted project choices live in `PRESENTATION_ARCHITECTURE_DECISION.md`.

## WORKING RULE

```text
one tiny inspected change
-> test
-> record proof
-> next change
```

Stef receives complete scripts, never code fragments.

Real Unity project:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`
