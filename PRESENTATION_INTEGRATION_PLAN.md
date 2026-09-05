# Presentation Prefab / Open Classroom Integration Plan

Last updated: 2026-09-05 (Europe/Amsterdam)

## STATUS

**PRESENTATION SERVICE LIVE — STANDALONE CORE + OPEN CLASSROOM VIDEOTXL ADAPTER IMPLEMENTED AND WORKING**

The reusable StefanieInVR Presentation product is no longer planned as a VideoTXL-bound playlist feature.

The accepted direction is now implemented: a standalone Core with its own VRChat video backend, plus an Open Classroom VideoTXL 2.5.1 adapter.

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

Durable exact source reference:

`REFERENCE/VideoTXL_2.5.1/com.texelsaur.video-2.5.1/`

Use that checked-in package for future adapter inspection. It contains `package.json` version 2.5.1, Runtime/Editor package content, CHANGELOG and the preserved upstream MIT LICENSE.

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

## IMPLEMENTATION UPDATE — 2026-09-05

The plan above remains the architectural baseline, but implementation has now progressed beyond the earlier proof order.

### Real Unity implementation now present

The real Open Classroom Unity scene now has:

- standalone Presentation Core;
- one own `VRCUnityVideoPlayer`;
- 10 stable configured slot URLs;
- automatic slide count from MP4 duration;
- Presentation controls and feedback;
- synced semantic state fields;
- `RT_PresentationVideo` RenderTexture output;
- separate Open Classroom `VideoTXLPresentationAdapter`;
- existing physical VideoTXL screen reuse.

### VideoTXL SourceManager incident — resolved

A stale null source remained in `SourceManager.sources` after the old Presentation playlist was deleted.

That null source stopped VideoTXL SourceManager startup and made all normal playlists fail.

The single null entry was removed.

Current reported state:

- 21 valid sources;
- normal VideoTXL playlists initialize again;
- no VideoTXL source-code redesign was needed.

This is closed unless new evidence appears.

### Adapter implementation chosen

Open Classroom-specific adapter behavior now follows the previously preferred architecture:

```text
ENTER PRESENTATION
-> locally set SyncPlayer.LocalPlaybackEnabled = false
-> keep VideoTXL shared pause/play state untouched
-> Presentation VRCUnityVideoPlayer renders into RT_PresentationVideo
-> VideoTXL 2.5.1 ScreenManager texture override places that texture on the existing physical screen

EXIT PRESENTATION
-> stop/leave Presentation playback
-> restore prior VideoTXL ScreenManager texture override state
-> set LocalPlaybackEnabled = true
-> allow VideoTXL to restore/resync normally
```

The reusable Core remains independent from VideoTXL.

### Existing Classroom display systems preserved

The adapter must continue respecting:

- `TXLScreenAutoVisibility` projector blendshape/open gate;
- physical screen renderer and colliders;
- existing custom VideoTXL/Unlit-based screen material/shader;
- `ScreenReadabilityManager` brightness/contrast controls.

ClientSim already proved the projector visibility and brightness/contrast systems still work while Presentation is displayed.

Do not replace those systems merely to simplify Presentation output.

### ClientSim evidence now available

Reported PASS:

- normal VideoTXL works outside Presentation Mode;
- Presentation locally suspends VideoTXL;
- Presentation appears on existing physical TXL screen;
- projector open/close controls renderer + collider;
- brightness/contrast affect Presentation;
- readability Reset works;
- First / Previous / Next work;
- 15-slide duration was detected;
- Stop restores VideoTXL and `LocalPlaybackEnabled=true`;
- no new Presentation compile/runtime errors after render-mode correction.

### Current open blocker: physical-screen fill/aspect

Presentation content is visible but does not fill the intended whole physical screen.

Known evidence:

- inspected Slot 1 MP4 = 1280x720, 16:9, 15 seconds;
- inspected `RT_PresentationVideo` = 1920x1080, 16:9;
- inspected MP4 does not contain the unwanted outer margins.

Therefore the next investigation belongs to the adapter / VideoTXL ScreenManager / custom shader fit-aspect pipeline, not the hosted converter.

Inspect especially:

- `VideoTXLPresentationAdapter`;
- ScreenManager texture override state;
- ScreenManager screen-fit state;
- physical screen material property mapping;
- `_FitMode`;
- `_TexAspectRatio`;
- restore behavior on Presentation exit.

Goal:

**Presentation fills the intended physical screen while the existing shader, readability controls, projector visibility and normal VideoTXL restoration remain unchanged.**

### Updated remaining acceptance order

```text
1. fix physical-screen fit/aspect
2. real two-client Build & Test for mode/slot/slide/revision
3. prove cross-client control/ownership
4. late join reconstruction
5. early-start-after-join adapter safety
6. Quest/PC decode + seek + display + suspend/restore proof
7. reusable prefab hardening/documentation
8. later Cinema integration
```

Do not claim multiplayer or Quest acceptance from ClientSim.


## FINAL IMPLEMENTATION STATUS — 2026-09-05

The implementation described by this plan has now reached a working state in the real Open Classroom Unity project.

Final reported/proven result:

- standalone Core remains independent of VideoTXL;
- one own `VRCUnityVideoPlayer`;
- 10 slot catalog;
- automatic slide count;
- First / Previous / Next;
- semantic sync with `modeActive`, `slotIndex`, `slideIndex`, `revision`;
- real two-client sync works;
- cross-client slide control works;
- Presentation OFF/ON restores the same Presentation slide;
- late join: FINAL TEST STILL OPEN;
- VideoTXL local suspend/restore works;
- existing physical screen reuse works;
- projector visibility works;
- custom shader remains;
- brightness/contrast remain;
- final screen-fit/display issue is reported resolved;
- Presentation UI is integrated into the physical tablet;
- `PresentationCore` is organized under `UIs/Managers`.

Most staged implementation gates are closed. The only remaining Presentation-specific proof is late join.

Current authoritative acceptance snapshot:

`PRESENTATION_ACCEPTANCE_2026-09-05.md`

Quest-specific device proof remains a separate evidence question if formal headset release acceptance is needed later.
