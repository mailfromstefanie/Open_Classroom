# Current Work — Open Classroom

Last updated: 2026-09-05 Europe/Amsterdam

## AUTHORITATIVE CURRENT STATUS

**Open Classroom currently has two important working tracks: the Presentation system is beta-ready, and the new multi-e-reader/library system has a ClientSim-working first implementation.**

Presentation:
- end-to-end current test pass complete;
- late join proven;
- VideoTXL adapter/restoration working.

E-reader/library:
- multi-book local playback architecture implemented;
- ClientSim functionality working;
- real headset handedness, real two-player proof and Quest profiling still open.

The earlier standalone Presentation-Core redesign and the Open Classroom VideoTXL 2.5.1 integration have both reached a working state.

Do not reopen solved architecture questions or old blockers unless new evidence appears.

## REAL UNITY PROJECT

Use:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

Do not confuse it with the older checkout:

`E:/GitHub/Open_Classroom`

The real tested Unity project remains the strongest source of truth for current scene wiring.

Stef made a full backup of the complete Unity project folder on 2026-09-05, including a fresh backup after the final screen-size fix and successful late-join proof.

## FINAL ACCEPTED PRESENTATION ARCHITECTURE

Reusable Core:

```text
Standalone StefanieInVR Presentation Core
-> one own VRCUnityVideoPlayer
-> own synced semantic presentation state
-> local load / seek / pause on every client
-> configurable MP4 slot catalog
-> no VideoTXL dependency inside the Core
```

Open Classroom integration:

```text
Presentation Core
-> RT_PresentationVideo
-> dedicated VideoTXLPresentationAdapter
-> VideoTXL 2.5.1 ScreenManager override
-> existing physical projector screen
-> existing custom screen shader
-> existing brightness/contrast system
```

The old VideoTXL Presentation Playlist design remains superseded.

Do not recreate it.

## SYNC MODEL — PROVEN

Only these semantic fields are synchronized:

```text
modeActive
slotIndex
slideIndex
revision
```

Every client performs media playback locally.

Proven two-client behavior:

- Presentation ON synchronizes to the other client;
- selected slot/slide synchronizes;
- Next/Previous changes synchronize;
- another client can issue a slide command and the first client follows;
- Presentation OFF synchronizes;
- both clients restore normal VideoTXL playback to the same corresponding video position;
- Presentation re-entry preserves the selected Presentation slot and slide.

## RESUME BEHAVIOR — PROVEN

Accepted behavior:

```text
Presentation on slide N
-> Presentation OFF
-> normal VideoTXL resumes
-> Presentation ON again
-> Presentation returns to the same slot and same slide N
```

A deliberately selected different slot still starts at slide 1.

Implementation rule:

- Stop does not reset synced `slotIndex` or `slideIndex`;
- Start/Toggle reuses valid saved slot/slide state;
- SelectSlot intentionally sets `slideIndex = 0`.

The several-second delay when re-entering Presentation is currently accepted because the Presentation player is actually stopped while inactive and must load the MP4 again. This keeps the design aligned with the Quest goal of not intentionally holding two active playback pipelines.

## LATE JOIN — PROVEN

Stef completed the final late-join test successfully.

Proven behavior:

```text
Client 1 already in Presentation Mode on a later slide
-> Client 2 joins afterward
-> Client 2 automatically enters Presentation Mode
-> Client 2 reconstructs the same slot + slide
-> authoritative Presentation state is not reset
```

This closes the remaining Presentation-specific multiplayer test gate for the current Open Classroom implementation.

## VIDEOTXL 2.5.1 ADAPTER — WORKING

Normal mode:

```text
Presentation OFF
-> VideoTXL LocalPlaybackEnabled = true
-> normal VideoTXL output
```

Presentation mode:

```text
Presentation ON
-> locally set SyncPlayer.LocalPlaybackEnabled = false
-> do not mutate VideoTXL synchronized pause/play state
-> Presentation player loads/seeks/pauses locally
-> Presentation output goes to RT_PresentationVideo
-> ScreenManager routes it to the existing physical TXL screen
```

Exit:

```text
Presentation OFF
-> restore previous VideoTXL display state
-> LocalPlaybackEnabled = true
-> VideoTXL restores/resyncs through its own supported logic
```

Hard rules:

- do not use VideoTXL `_TriggerPause()` as the local suspend API;
- do not use raw internal `BaseVRCVideoPlayer.Stop()` as the integration contract;
- keep using the exact checked-in VideoTXL 2.5.1 source under `REFERENCE/VideoTXL_2.5.1/`.

## VIDEOTXL SOURCEMANAGER INCIDENT — CLOSED

The temporary failure of all ordinary VideoTXL playlists was caused by one stale null element in `SourceManager.sources` left after dismantling the old Presentation playlist.

It was removed.

Current preserved result:

- 21 valid VideoTXL sources;
- normal VideoTXL initializes again;
- this was not a conflict between the two video players.

Do not reopen this incident without new evidence.

## PHYSICAL SCREEN / DISPLAY — WORKING

The final screen/display issue is now reported solved in the real Unity project.

Preserve the working display chain:

- existing physical `VideoScreen (quest)`;
- existing `VideoTXL/Unlit` screen material/shader;
- existing VideoTXL ScreenManager;
- existing projector open/close visibility behavior;
- existing renderer/collider handling;
- existing brightness/contrast controls;
- Presentation RenderTexture path.

Important known source dimensions from the final investigation:

- demo Presentation MP4: 1280x720, 16:9;
- `RT_PresentationVideo`: 1920x1080, 16:9;
- VideoTXL CRT: 1920x1080, target aspect ~1.777777.

The exact final working scene setting is authoritative. Do not blindly reapply older speculative screen-fit experiments.

## SCREEN READABILITY / PROJECTOR — WORKING

Proven preserved behavior:

- projector close/open controls the physical screen as intended;
- Presentation follows the same physical display path;
- brightness/contrast sliders affect Presentation;
- readability Reset works;
- normal VideoTXL still works afterward.

Do not replace the custom screen material or bypass these existing systems.

## TABLET / HIERARCHY CLEANUP — FINAL CURRENT ORGANIZATION

Stef cleaned up the scene hierarchy after the functional tests:

- `PresentationCore` now lives under `UIs/Managers`;
- Presentation UI/canvas content was moved into the physical tablet;
- the controls still work after this reorganization.

Treat this as the preferred current scene organization.

Moving `PresentationCore` under Managers is organizational only; the reusable Core architecture remains standalone.

## CURRENT PRESENTATION CORE FEATURES

Working current feature set includes:

- 10 configurable Presentation Slot URLs;
- own `VRCUnityVideoPlayer`;
- automatic slide count from MP4 duration;
- Start / Stop;
- Slot selection;
- First;
- Previous;
- Next;
- current state feedback;
- local seek/pause;
- same-slot media reuse while active;
- synchronized semantic state;
- resume same slide after Presentation OFF/ON;
- VideoTXL local suspension/restore;
- reuse of the existing physical projector screen.

## TEST / ACCEPTANCE SUMMARY — 2026-09-05

Reported working:

- local Presentation playback;
- automatic 15-slide detection on the current demo;
- First / Previous / Next;
- two-client Presentation synchronization;
- cross-client slide control;
- Presentation OFF/ON sync;
- VideoTXL restore on both clients;
- same Presentation slide restored after re-entry;
- late join behavior;
- projector visibility behavior;
- brightness/contrast behavior;
- physical screen Presentation output;
- tablet UI after hierarchy cleanup.

Detailed final handoff:

`PRESENTATION_ACCEPTANCE_2026-09-05.md`

Quest-specific device evidence is not separately documented in this chat. Do not invent a Quest PASS if a future release decision requires explicit headset proof.

## LIVE HOSTED SERVICE CONTRACT

Cross-project truth from `mailfromstefanie/StefanieInVR-Presentation-Service`:

- ten-slot Presentation Service Free Beta = LIVE;
- hosted input = PDF;
- one slide = one second;
- stable slot MP4 URLs;
- current live uploader uses the proven slot-code flow;
- prepared Username-only dashboard is not live.

Open Classroom Presentation has completed the current functional/multiplayer test pass and is ready to enter beta use in the Classroom.

## CURRENT CROSS-PROJECT STATE

```text
Presentation Service: LIVE
Open Classroom Presentation: WORKING / BETA-READY
Reusable Presentation architecture: PROVEN IN CLASSROOM IMPLEMENTATION; SELLABLE PREFAB PRODUCTIZATION STILL TO DO
Art House Cinema Presentation integration: NOT YET DONE
```

The next Presentation product phase after beta testing is **not Cinema yet**.

Required order:

```text
finish beta test phase
-> harden/clean the reusable Presentation package
-> create the actual distributable/sellable prefab
-> remove Classroom-specific assumptions from the sale package
-> provide creator-facing setup/configuration
-> package documentation + install/setup instructions
-> version/release the commercial/free product package
-> only then treat the prefab as a finished product for external customers
```

Art House Cinema is a later integration target for the proven product and keeps its own control/menu/reset/admin return route.

## WORKING STYLE WITH STEF

- Dutch;
- beginner-friendly;
- build quickly;
- avoid ceremonial retesting of already-proven behavior;
- use tests at meaningful risk gates;
- complete scripts, never fragments;
- inspect before destructive changes;
- Codex/Sol is a bounded implementation/debugging worker when needed;
- normal ChatGPT/Nova remains orchestrator;
- update GitHub after meaningful proof or project transitions.

## GITHUB TRUTH RULE

GitHub is durable project memory.

The tested Unity scene can be newer than copied source/reference files in this repository.

Never overwrite known working Unity scene truth with an older GitHub planning assumption.


## MULTI E-READER / LIBRARY — IMPLEMENTED IN REAL UNITY PROJECT

Codex completed the first reusable multi-e-reader/library implementation in the real Unity project.

Current architecture:

```text
one local EReaderLocalPlaybackManager
-> one VRCUnityVideoPlayer
-> one shared RT_EReader
-> many lightweight EReaderBook instances
```

Current scene examples:

- `Book_A`
- `Book_B`
- `Book_A_Home`
- `Book_B_Home`

Hierarchy:

```text
UIs/Managers/EReader Local Playback Manager
Other Toggles and Systems/E-Reader/
  Book_A
  Book_B
  Book_A_Home
  Book_B_Home
```

Important implementation facts:

- each book has its own URL/title/local page state;
- reading state is local only;
- physical pickup uses VRC Pickup + kinematic Rigidbody + VRCObjectSync;
- Auto Hold enabled;
- orientation = Any;
- one shared e-reader player and RenderTexture only;
- inactive books disable their Canvas and screen renderer;
- idle e-reader player is stopped;
- no per-book video player;
- no per-book `Update()`;
- existing Canvas physical fit was preserved at 160 x 15 with scale 0.001;
- existing ResettableObject infrastructure is reused;
- Reset Group 6 is the intended book reset group;
- Presentation and VideoTXL scripts were not modified.

### E-reader controls

Current compact local controls:

```text
First
-10
Prev
Page X / Y
Next
+10
Keep Open
X
```

Current media contract remains one page/spread per second.

### Keep Open / last-touched-wins

Local behavior:

- Keep Open OFF + drop -> reader Canvas/screen/player closes;
- Keep Open ON + drop -> current book remains locally active where placed;
- activating another book always closes the previously active local reader;
- X closes the reader and disables Keep Open;
- stale callbacks from a prior book are ignored;
- only the latest locally activated book may own/show the shared e-reader output.

### Local bookmark behavior

The last local page is remembered per book for the current world session and restored when that book is reopened.

No page/bookmark/Keep Open/active-reader state is network-synchronized.

### Current ClientSim proof

Reported working:

- real MP4 loaded and 213 pages detected;
- Loading state;
- First / Previous / Next / +/-10;
- page bounds;
- local last-page restore;
- Keep Open ON/OFF drop behavior;
- Book A -> Book B arbitration;
- rapid switching while a prior URL is still loading;
- stale load callback protection;
- toggle OFF closes active reader;
- Reset returns Book B exactly to Home;
- inactive screen renderers/player stop correctly;
- existing Presentation Slot 1 still loads/pauses;
- Presentation stop restores physical screen and local VideoTXL playback;
- Unity compile clean / no compile errors.

### E-reader evidence still open

Do NOT mark these as proven yet:

1. real headset left/right-hand comfort and orientation;
2. real two-player VRChat proof of physical VRCObjectSync plus independent local reading sessions;
3. Quest device profiling/performance proof.

The structure is Quest-oriented, but no formal Quest profiler/device PASS exists yet.

### E-reader source changes reported by Codex

Real project changes include:

- `Assets/Ereader/EReaderLocalPlaybackManager.cs`
- `Assets/Ereader/EReaderBook.cs`
- corresponding UdonSharp program assets;
- `Assets/#Classroom/Scenes/Classroom.unity`

Old `LocalEReader.cs` / orphaned `LocalEReader.asset` were removed.

The tested real Unity project remains authoritative until these local changes are deliberately mirrored/committed as source snapshots.


## PERFORMANCE AUDIT — 2026-09-05

A targeted read-only performance analysis was completed after the e-reader work.

Current important findings:

- idle desktop ClientSim CPU/Udon/UI/geometry costs are low;
- e-reader and VideoTXL Quest RenderTextures currently use 2x MSAA + automatic mipmaps and are candidates for focused optimization;
- large transparent Graphlit glass is the main likely Quest fill-rate risk;
- three NPOT posters account for about 69 MB reported uncompressed PC texture memory;
- `M_EReaderScreen.mat` has a suspicious emission reference to `VideoTXLCRT-Quest`;
- no performance changes were applied during the audit;
- scene was left clean, out of Play Mode, with no errors.

Read before performance changes:

`PERFORMANCE_AUDIT_2026-09-05.md`

Optimization order is deliberately conservative: e-reader material -> e-reader RT -> VideoTXL RT -> real Quest evidence -> only then glass/material redesign.


## EXACT NEXT SESSION — BACKUP FIRST

No more changes are planned tonight.

Before the next performance/cleanup round, Stef will make a **fresh full backup of the current working Unity project including the new multi-e-reader implementation**.

Important distinction:

- the earlier full backup protects the accepted Presentation state;
- the new e-reader implementation was added afterward;
- therefore a new backup is required before touching RenderTexture/material performance settings.

After that backup, the next narrow optimization round is:

```text
verify/fix M_EReaderScreen emission mapping
-> validate e-reader
-> RT_EReader: 1x MSAA + automatic mipmaps OFF
-> validate e-reader
-> VideoTXLCRT-Quest: 1x MSAA + automatic mipmaps OFF
-> validate VideoTXL
-> validate Presentation takeover/restore
-> STOP
```

Do not include transparent glass, poster normalization or Presentation RenderTexture changes in that same round.

Detailed e-reader handoff:

`EREADER_LIBRARY_HANDOFF_2026-09-05.md`

Detailed performance audit:

`PERFORMANCE_AUDIT_2026-09-05.md`
