# Presentation Architecture Decision — 2026-09-05

## STATUS

**ACCEPTED, IMPLEMENTED AND PROVEN IN OPEN CLASSROOM V1**

The reusable StefanieInVR Presentation prefab is **video-player independent at its Core**.

VideoTXL is not the playback authority for the reusable product. It is implemented as an optional integration in Stef's own Open Classroom.

This decision supersedes the earlier plan where a VideoTXL Presentation Playlist was the default prefab architecture.

Implementation/acceptance evidence: `PRESENTATION_ACCEPTANCE_2026-09-05.md`.

## PRODUCT ARCHITECTURE

```text
StefanieInVR Presentation
|
+-- Core
|   +-- Presentation Controller
|   +-- Presentation Sync
|   +-- Presentation Session / state machine
|   +-- one own VRCUnityVideoPlayer
|   +-- slot catalog
|   +-- local seek/pause logic
|
+-- Displays
|   +-- own rollout Presentation Screen
|   +-- Generic Renderer / Material output
|
+-- Optional Integrations
    +-- VideoTXL
    +-- ProTV later if useful
    +-- USharpVideo later/experimental if useful
```

Core must compile and work without VideoTXL, ProTV or USharpVideo installed.

## NETWORK MODEL

The presentation is normally a held/paused slide, so continuous video-time synchronization is unnecessary.

Authoritative shared state is intentionally small:

```text
modeActive
slotIndex
slideIndex
revision
```

Every client reconstructs playback locally:

```text
receive presentation state
-> load the selected slot locally if needed
-> seek locally to the requested slide
-> pause locally on that slide
-> show the result
```

The `revision` value is only a technical ordering/version counter so stale updates cannot overwrite a newer command.

URLs stay local in the configured slot catalog. Synchronize indices, not URL strings.

## V1 PLAYBACK MODEL

Use **one own `VRCUnityVideoPlayer`** in the standalone Core.

V1 does not require AVPro unless later PC/Quest testing proves a concrete need.

Presentation MP4 contract remains:

- direct MP4;
- one slide = one second;
- no autoplay slideshow in V1;
- Next / Previous means seek to the corresponding local slide time.

### Default hold behavior

For V1:

```text
load selected MP4 once
-> seek to requested slide
-> pause on that slide
-> keep the same MP4 loaded while navigating within that slot
```

Do **not** make snapshot-and-stop the default.

Reason:

- a snapshot is not needed for synchronization;
- stopping after every slide could force a reload before the next slide;
- VRChat has a shared per-client video URL loading budget/cooldown;
- Stef's existing Quest experience already shows one active video player performs acceptably.

Snapshot / RenderTexture hold remains an optional future Quest performance experiment only if measured device testing shows a meaningful benefit.

## QUEST / PERFORMANCE RULE

Primary goal:

**Do not intentionally run the normal world video player and the Presentation player as active playback pipelines at the same time.**

Preferred hand-off:

```text
NORMAL MODE
normal world player active
Presentation player stopped

PRESENTATION MODE
normal world player locally suspended/stopped where safely supported
Presentation player active / paused on current slide

EXIT
Presentation player stopped
normal world player restored/resynced through its own controller
```

Important:

- Pause is not assumed to release decoder resources.
- Stop is not blindly called on unknown third-party player's internal `BaseVRCVideoPlayer`.
- The standalone product does not promise it can hard-stop every possible third-party player.
- With unknown players, the creator may use the standalone rollout screen and manage their existing player themselves.
- Quest performance claims must come from device testing, not inference.

## DISPLAY MODEL

The product should offer two normal ways to display presentations:

1. **Own rollout Presentation Screen**
   - guaranteed standalone path;
   - no dependency on another video system.

2. **Existing screen/material output**
   - creator may target a compatible Renderer/material;
   - package-specific adapters may provide a cleaner takeover when the other player owns/re-writes that screen.

Do not promise a raw material swap will work against every ScreenManager.

## VIDEOTXL INTEGRATION — OPEN CLASSROOM

For Stef's own Classroom, build a typed VideoTXL 2.5.1 integration around the independent Core.

Preferred behavior:

```text
enter Presentation Mode
-> VideoTXL LocalPlaybackEnabled = false
-> Presentation player becomes active
-> use own rollout screen OR route Presentation output onto the existing VideoTXL screen
-> navigate slides locally from synced slot/slide state

exit Presentation Mode
-> stop Presentation player
-> restore VideoTXL screen state
-> VideoTXL LocalPlaybackEnabled = true
-> allow VideoTXL to reload/resync through its own logic
```

Do not use `_TriggerPause` as the VideoTXL suspension mechanism because that changes synchronized pause state.

Do not make VideoTXL a Core dependency.

## OTHER PLAYER SUPPORT

Current research direction:

- VideoTXL: strongest optional integration candidate.
- ProTV: may support a reduced/local-pause integration later, but do not market it as equivalent to VideoTXL hard local suspension without proof.
- USharpVideo: do not make it a V1 requirement; any deep integration remains experimental until a supported local suspend contract exists.
- Unknown players: generic events/material routing may be offered as an advanced best-effort option, never as a strict compatibility guarantee.

## SUPERSEDED DESIGN

The following earlier V1 proposal is now **historical only**:

```text
ONE VideoTXL Presentation Playlist
-> ten Presentation tracks
-> VideoTXL SyncPlayer owns presentation playback/sync
```

The exact VideoTXL 2.5.1 source research remains useful for the optional VideoTXL adapter, but it is no longer the default standalone product architecture.

## RESEARCH REPORT

The completed Work-generated architecture report is stored at:

`docs/research/standalone-presentation-player/Standalone_VRChat_Presentation_Player_Architecture_Decision.docx`

Research findings do not automatically become project truth. This file records the choices actually accepted for the product.

## OPEN PROOF GATES

Before calling the prefab production-ready:

- direct MP4 load on PC;
- direct MP4 load on Quest;
- accurate first/middle/last slide seeking;
- repeated Next/Previous while paused;
- slot switch behavior and VRChat URL cooldown;
- late join reconstruction from mode/slot/slide/revision;
- ownership/permission behavior;
- clean stop on Presentation exit;
- generic rollout screen;
- VideoTXL 2.5.1 suspend/resume integration;
- same VideoTXL screen takeover/restore if chosen;
- Quest performance with normal player suspended and Presentation player active;
- only after measurement: decide whether any snapshot optimization is worth adding.

## IMPLEMENTATION ORDER

```text
1. standalone local Core on its own test screen
2. one Slot
3. local seek + pause
4. Next / Previous / First
5. synced mode + slot + slide + revision
6. late join / ownership tests
7. Quest proof
8. prefab cleanup / configuration
9. VideoTXL adapter for Open Classroom
10. optional existing-screen takeover
11. later ProTV / other adapters only if there is real demand
```

## SOURCE-OF-TRUTH RULE

Real Unity/VRChat test results override planning assumptions.

The real Unity project remains:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`
