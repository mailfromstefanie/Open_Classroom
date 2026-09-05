# Current Work — Open Classroom

Last updated: 2026-09-05 Europe/Amsterdam

## AUTHORITATIVE CURRENT STATUS

**The StefanieInVR Presentation system in Open Classroom is now working end-to-end in the tested environment and is ready for beta use in the Classroom.**

The earlier standalone-Core redesign and the Open Classroom VideoTXL 2.5.1 integration have both reached a working state.

Do not reopen solved architecture questions or old blockers unless new evidence appears.

## REAL UNITY PROJECT

Use:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

Do not confuse it with the older checkout:

`E:/GitHub/Open_Classroom`

The real tested Unity project remains the strongest source of truth for current scene wiring.

Stef also made a full backup of the complete Unity project folder on 2026-09-05 before the final cleanup/fixes.

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

## LATE JOIN — WORKING

Stef reports the final Presentation system works, including the late-join scenario discussed at the end of the test pass.

Accepted late-join behavior:

```text
existing client already presenting on slot/slide
-> new client joins
-> new client reconstructs current mode + slot + slide
-> new client does not reset the authoritative Presentation state
```

If a future regression appears, re-prove only this isolated case.

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

Open Classroom can now truthfully be described as ready for beta testing with the Presentation Service in the tested setup.

## CURRENT CROSS-PROJECT STATE

```text
Presentation Service: LIVE
Open Classroom Presentation: WORKING / BETA-READY
Reusable Presentation architecture: PROVEN IN CLASSROOM
Art House Cinema Presentation integration: NOT YET DONE
```

The next broad product target can be Art House Cinema when Stef explicitly resumes that project.

Cinema keeps its own previously accepted control/menu/reset/admin return route; do not skip that foundation just because the Presentation product is now ready.

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
