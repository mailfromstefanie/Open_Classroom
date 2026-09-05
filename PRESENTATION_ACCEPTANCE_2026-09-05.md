# Presentation Acceptance Snapshot — 2026-09-05

Status: **WORKING — FINAL LATE-JOIN TEST STILL OPEN**

This file is the durable final handoff for the 2026-09-05 standalone Presentation + Open Classroom VideoTXL integration sprint.

## Product architecture

Reusable Presentation Core:

- own `VRCUnityVideoPlayer`;
- no VideoTXL dependency in Core;
- 10 configurable direct MP4 slots;
- one slide = one second;
- semantic sync only:
  - `modeActive`
  - `slotIndex`
  - `slideIndex`
  - `revision`;
- each client loads/seeks/pauses locally.

Open Classroom adapter:

- dedicated `VideoTXLPresentationAdapter`;
- Presentation output via `RT_PresentationVideo`;
- VideoTXL 2.5.1 ScreenManager routes Presentation to the existing physical projector screen;
- local VideoTXL playback disabled only while Presentation is active;
- normal VideoTXL shared pause/play state is not mutated;
- prior screen/local playback state restored on Presentation exit.

## Working feature set

- Start Presentation;
- Stop Presentation;
- Slot 1..10;
- First;
- Previous;
- Next;
- automatic slide count from MP4 duration;
- local seek + short run + pause;
- same-slot loaded media while Presentation remains active;
- synchronized mode/slot/slide/revision;
- current UI/status feedback.

## Proven two-client behavior

Stef completed a two-client synchronization test.

Observed:

- Presentation ON propagated to both clients;
- slide state propagated;
- client 2 could issue Previous and client 1 followed;
- Presentation OFF propagated;
- normal VideoTXL restored on both clients to the same corresponding playback position.

## Resume behavior

Initial bug:

- Presentation OFF then ON reset both clients to slide 1.

Cause in the current controller:

- Start/Toggle explicitly reset `slideIndex = 0`.

Fix:

- preserve valid synced `slotIndex` + `slideIndex` when Presentation is stopped;
- re-entry resumes that state;
- selecting a different slot intentionally resets to slide 1.

Final result reported working.

## Late join — NOT YET FINAL-PROVEN

Late-join handling is deliberately designed around the synced semantic state, but the final real late-join test is still open.

Required contract to prove:

```text
join while Presentation already active
-> receive current mode/slot/slide/revision
-> locally load the slot
-> seek to the current slide
-> pause
-> do not overwrite authoritative state
```

Stef reports that this is the only remaining Presentation-specific test.

## VideoTXL restoration

Working behavior:

```text
NORMAL
VideoTXL local playback enabled

PRESENTATION
VideoTXL local playback disabled
Presentation player active

EXIT
Presentation player stops
VideoTXL display state restored
VideoTXL local playback enabled again
```

A several-second Presentation re-entry delay is accepted because the Presentation MP4 is actually stopped while inactive and has to load again.

## Screen/display

The final physical-screen presentation display is reported working.

Preserve:

- `VideoScreen (quest)`;
- VideoTXL ScreenManager;
- VideoTXL/Unlit-based material/shader;
- projector visibility logic;
- screen renderer/collider handling;
- brightness/contrast controls.

Known dimensions from investigation:

- demo slot MP4 = 1280x720;
- Presentation RenderTexture = 1920x1080;
- VideoTXL CRT = 1920x1080;
- 16:9 target aspect.

The exact final working scene configuration is authoritative.

Do not overwrite it with earlier speculative Fit/Stretch experiments.

## Projector/readability

Reported working:

- projector open/close;
- screen renderer/collider behavior;
- Presentation through same physical screen;
- brightness;
- contrast;
- readability reset.

## Hierarchy cleanup

Final preferred scene organization:

- `PresentationCore` moved under `UIs/Managers`;
- Presentation Canvas/UI content integrated into the physical tablet;
- controls remained working after cleanup.

## Closed VideoTXL SourceManager incident

During integration, ordinary VideoTXL playlists temporarily failed.

Root cause:

- stale null `SourceManager.sources` entry from the removed old Presentation playlist.

Fix:

- remove that one stale null source.

Result:

- 21 valid sources;
- VideoTXL initializes normally again.

Do not treat this as a two-video-player conflict.

## Backup / recovery

Stef created a full backup of the complete working Unity project folder before final cleanup/handoff.

GitHub remains durable project memory, but the tested Unity project can be newer than repo source snapshots.

## Remaining evidence boundary

Everything currently tested by Stef is reported working. The late-join scenario itself is not yet final-proven.

A separate explicit Quest headset acceptance run is not documented in this chat. If a formal Quest release gate matters later, run and record that specific device proof instead of assuming it from PC/ClientSim evidence.

## Cross-project result

```text
Presentation Service = LIVE
Open Classroom Presentation = WORKING — FINAL LATE-JOIN TEST OPEN
Reusable Presentation architecture = PROVEN IN CLASSROOM
Art House Cinema Presentation integration = NEXT LATER TARGET
```
