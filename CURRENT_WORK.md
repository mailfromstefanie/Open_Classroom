# Current Work — Open Classroom

Last updated: 2026-09-05 Europe/Amsterdam

## AUTHORITATIVE CURRENT STATUS

**The standalone StefanieInVR Presentation Core now exists in the real Open Classroom Unity project and the Open Classroom VideoTXL 2.5.1 adapter is substantially working. The next blocker is display fit/aspect on the existing physical VideoTXL screen, followed by real multiplayer and Quest proof.**

Important current truth:

- the old VideoTXL-bound Presentation Playlist architecture remains superseded;
- the reusable Presentation Core is still independent of VideoTXL;
- the real Unity project is the source of truth for current scene wiring;
- the current GitHub repository may still lag the live Unity source files themselves, so do not overwrite tested Unity truth with older repository snapshots;
- normal VideoTXL playback has been restored after removing one stale null SourceManager reference left by the deleted old Presentation playlist;
- the current remaining visual issue is **not presently evidence of a PDF->MP4 converter defect**.

Read `PRESENTATION_ARCHITECTURE_DECISION.md` and `PRESENTATION_INTEGRATION_PLAN.md` before changing architecture.

## REAL UNITY PROJECT

Use:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

Do not confuse it with the older checkout:

`E:/GitHub/Open_Classroom`

## ACCEPTED CORE ARCHITECTURE — STILL VALID

Reusable product:

```text
Standalone StefanieInVR Presentation Core
-> one own VRCUnityVideoPlayer
-> own presentation state/sync
-> own default display path
-> configurable slot catalog
-> local load / seek / pause per client

Optional integration layer
-> VideoTXL 2.5.1 for Open Classroom
-> other players only later if useful
```

The Core must not require VideoTXL, ProTV or USharpVideo to compile/work.

## ACCEPTED V1 NETWORK MODEL

Synchronize only:

```text
modeActive
slotIndex
slideIndex
revision
```

Every client reconstructs playback locally:

```text
receive shared state
-> load selected slot locally if needed
-> seek locally to requested slide
-> pause locally
```

Do not synchronize continuous playback time for the normally paused presentation state.

## CURRENT REAL UNITY IMPLEMENTATION

The real scene now contains a working Presentation implementation, including:

- `PresentationCore`;
- own `VRCUnityVideoPlayer`;
- 10 configurable Presentation Slot URLs;
- automatic slide-count detection from MP4 duration;
- Start / Slot / First / Previous / Next / Stop/Presentation controls;
- synced Presentation state fields;
- status/menu feedback;
- Presentation RenderTexture;
- Open Classroom VideoTXL Presentation adapter;
- existing physical TXL projector screen reuse.

Codex/Sol reported these local Unity files/components changed during the bounded 2026-09-05 integration pass:

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

Unity also generated the associated `.meta` files.

Treat this as reported/tested local Unity truth even if every file is not yet mirrored into this repository.

## VIDEOTXL SOURCEMANAGER FAILURE — FIXED, DO NOT REOPEN

After the old Presentation playlist was removed, `SourceManager.sources` still contained a null element.

Observed cause:

- element 15 was null;
- VideoTXL 2.5.1 binds every source during startup;
- the null source caused a runtime failure in `SourceManager.cs` around line 61;
- SourceManager initialization stopped, which made all ordinary VideoTXL playlists appear broken.

Fix performed in the real scene:

- removed the one null/stale source;
- SourceManager now contains 21 valid sources;
- no VideoTXL source code or ordinary playlist design was changed.

ClientSim after the fix:

- VideoTXL initializes without the SourceManager error;
- `LocalPlaybackEnabled` is normally true;
- ordinary playlists bind/start again.

This was **not a conflict between the two video players**.

Do not diagnose the old SourceManager null entry again unless fresh evidence contradicts this.

## OPEN CLASSROOM VIDEOTXL 2.5.1 ADAPTER — CURRENT IMPLEMENTATION

The reusable Core remains separate from VideoTXL.

Open Classroom now has a dedicated integration layer.

Chosen behavior:

### NORMAL

```text
Presentation mode OFF
-> VideoTXL LocalPlaybackEnabled = true
-> normal VideoTXL playback/output behaves normally
-> existing projector open/close system remains authoritative
-> existing custom screen shader remains in use
-> existing brightness/contrast controls remain active
-> Presentation player is not actively presenting
```

### PRESENTATION MODE

```text
Presentation mode ON
-> locally set SyncPlayer.LocalPlaybackEnabled = false
-> do NOT change VideoTXL synchronized pause/play state
-> own Presentation player loads/seeks/pauses locally
-> Presentation output goes to RT_PresentationVideo
-> VideoTXL ScreenManager texture override routes Presentation to the existing physical TXL screen
-> existing physical screen material/shader remains the final display path
```

### EXIT

```text
Presentation mode OFF
-> Presentation playback stops/leaves active presentation state
-> previous VideoTXL ScreenManager texture override state is restored
-> LocalPlaybackEnabled = true
-> VideoTXL restores/resyncs through its own supported logic
```

Hard rules:

- do not use VideoTXL `_TriggerPause()` for local suspension;
- do not directly stop VideoTXL internal `BaseVRCVideoPlayer` as the integration contract;
- use the exact checked-in VideoTXL 2.5.1 source:
  `REFERENCE/VideoTXL_2.5.1/com.texelsaur.video-2.5.1/`.

## EXISTING SCREEN SYSTEMS THAT MUST BE PRESERVED

### TXLScreenAutoVisibility

The real Classroom already has `TXLScreenAutoVisibility`.

It controls:

- projector-screen blendshape/open gate;
- physical TXL screen renderer visibility;
- associated screen colliders;
- ordinary VideoTXL loading/playing/paused visibility behavior.

It has now been adapted so Presentation Mode can keep the physical screen visible even though VideoTXL local playback is suspended/stopped locally.

Do not replace the projector open/close system.

### Custom screen shader / readability

The physical screen uses Stef's existing custom VideoTXL/Unlit-based material/shader pipeline.

`ScreenReadabilityManager` controls runtime brightness/contrast using the existing material properties.

This must continue to work for:

- ordinary VideoTXL content;
- Presentation content.

Do not replace the physical screen material or bypass the readability manager merely to display Presentation.

## CLIENTSIM PROOF — 2026-09-05

Reported proven in the real Unity project:

- normal VideoTXL playback works when Presentation is off;
- Presentation Mode locally disables VideoTXL playback;
- Presentation appears on the same physical VideoTXL screen through `RT_PresentationVideo`;
- the existing custom screen shader remains in use;
- projector close/open correctly controls renderer and collider during Presentation Mode;
- brightness/contrast sliders affect Presentation content;
- readability Reset restores brightness/contrast to 1;
- First / Previous / Next work;
- navigation proof included slide index transitions `0 -> 1 -> 0`;
- one loaded Presentation reported 15 slides;
- Stop restores VideoTXL display/output and `LocalPlaybackEnabled = true`;
- no new Presentation compile/runtime errors after the VRC render-mode correction;
- unrelated VRCMarker / MarkerUI errors remain intentionally untouched.

ClientSim is useful evidence only. It does not prove real multiplayer, PC/Quest parity or late join.

## CURRENT BLOCKER — PRESENTATION DOES NOT FILL THE WHOLE PHYSICAL SCREEN

The Presentation is visible on the correct physical screen, but currently does not fill the intended full screen area.

Important evidence already gathered:

- inspected hosted Slot 1 MP4: **1280x720**, 16:9, 15 seconds;
- inspected `RT_PresentationVideo`: **1920x1080**, 16:9;
- the inspected MP4 itself does not contain the unwanted outer screen margins;
- therefore do **not** start by changing the PDF->MP4 converter;
- VRCUnityVideoPlayer Aspect Ratio dropdown changes are not assumed to solve this because the Presentation is now injected through a RenderTexture / VideoTXL ScreenManager output path.

Most likely investigation area:

- `VideoTXLPresentationAdapter`;
- actual VideoTXL 2.5.1 `ScreenManager` texture override behavior;
- existing physical screen shader/property mapping;
- `_FitMode`, `_TexAspectRatio`, screen-fit and aspect-ratio state while Presentation Mode is active;
- exact restoration of prior VideoTXL display state when Presentation stops.

Do not replace Stef's shader, projector visibility system or readability controls to fix this.

## EXACT NEXT ACTION FOR A FRESH CHAT

First inspect current real Unity wiring/read-only.

Focus only on the remaining screen-fill issue.

Route:

```text
1. inspect VideoTXLPresentationAdapter in the real Unity project
2. inspect actual VideoTXL ScreenManager references and current overrides
3. inspect the physical screen material/shader properties used for fit/aspect
4. compare normal VideoTXL state vs Presentation Mode state
5. identify why a 16:9 Presentation RenderTexture is not filling the intended physical 16:9 screen
6. make the smallest safe fix
7. prove normal TXL still restores correctly
8. only then move to multiplayer proof
```

Do not broaden this into a converter rewrite or broad VideoTXL refactor.

## REAL MULTIPLAYER SYNC PROOF — 2026-09-05

Real two-client Build & Test has now PASSED for the core Presentation synchronization path.

Observed:

- Client 1 turning Presentation ON caused Client 2 to enter Presentation Mode;
- selected Presentation state synchronized;
- slide changes synchronized between both clients;
- turning Presentation OFF restored VideoTXL on both clients;
- both clients returned to the same corresponding position in the normal VideoTXL video after Presentation exit.

This proves the current implementation's core shared Presentation state/hand-off path is functioning across two real test clients.

### Resume behavior — FIXED AND PROVEN

The re-entry behavior was corrected in the real Unity `PresentationController.cs`.

Proven behavior:

```text
Presentation active on a later slide
-> Presentation OFF
-> normal VideoTXL resumes correctly
-> Presentation ON again
-> both clients return to the same Presentation slot and same saved slide
```

Changing to a different slot still intentionally starts that newly selected slot at slide 1.

Implementation rule now preserved:

- Presentation OFF does not reset synced `slotIndex` or `slideIndex`;
- Presentation ON reuses the preserved state when it is valid;
- selecting a different slot explicitly resets `slideIndex` to 0.

The expected several-second delay when re-entering Presentation remains because the Presentation video is deliberately stopped while inactive and must be loaded again. This trade-off is currently accepted for V1 to avoid intentionally keeping both video playback pipelines active on Quest.

## AFTER SCREEN-FILL FIX — REQUIRED PROOF ORDER

### Real multiplayer Build & Test

Minimum two clients.

Prove:

```text
Client 1:
Presentation ON
-> select slot
-> Next
-> Next

Client 2:
must reconstruct same mode/slot/slide

Client 2:
Previous

Client 1:
must follow

Stop:
both clients must restore normal local VideoTXL behavior
```

Shared fields to prove:

- `modeActive`;
- `slotIndex`;
- `slideIndex`;
- `revision`.

### Late join

With Client 1 already presenting on a later slide:

- Client 2 joins;
- Client 2 must reconstruct current Presentation mode/slot/slide without mutating authoritative state.

### Early-start-after-join

One early ClientSim attempt occurred while VideoTXL itself had not fully initialized.

After normal initialization the local suspend/restore path was stable.

Still perform one deliberately quick Presentation start after join to prove the adapter behaves safely during early initialization.

### Quest / PC

Finally prove:

- Quest direct MP4 decode;
- slide seek/pause;
- Presentation screen output;
- normal VideoTXL local suspension/restore;
- performance with only the intended active playback pipeline.

Do not claim Quest acceptance before device proof.

## LIVE HOSTED SERVICE CONTRACT

Cross-project truth from `mailfromstefanie/StefanieInVR-Presentation-Service`:

- ten-slot Presentation Service Free Beta = LIVE;
- hosted input = PDF;
- one slide = one second;
- stable public slot MP4 URLs;
- current uploader uses the proven slot-code flow;
- prepared Username-only dashboard is not live.

The reusable prefab must also allow creators to configure their own compatible MP4 hosting.

## CURRENT CROSS-PROJECT PRIORITY

```text
1. fix Presentation physical-screen fill/aspect
2. real multiplayer sync proof
3. late join / early-start proof
4. Quest proof
5. reusable prefab hardening
6. later integrate proven prefab into Art House Cinema
```

Website Editor and eReader/eBook remain parked unless Stef explicitly reprioritizes them.

Cinema remains a separate project.

## WORKING STYLE WITH STEF

Current session preference is faster MVP-oriented building rather than stopping after every tiny object.

Still:

- Dutch;
- beginner-friendly;
- explain why before technical detail;
- avoid unnecessary testing;
- test only meaningful risk gates;
- complete script files, never fragments;
- avoid CMD/PowerShell unless it materially helps;
- use Codex/Sol as a bounded Unity implementation/debugging worker only when direct scene inspection is worth the credits;
- normal ChatGPT/Nova remains the orchestrator.

## GITHUB TRUTH RULE

GitHub is durable project memory.

The real tested Unity scene may be newer than this repository's copied source/reference files.

Never replace known working scene truth with an older plan merely because the older plan is already committed.
