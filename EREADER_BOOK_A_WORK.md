# EReader Book_A — current implementation and handoff

Updated: 2026-09-16 Europe/Amsterdam.

This is the maintained GitHub handoff distilled from the full local work dossier and Stef's test reports. The longer local `EREADER_BOOK_A_WORK.md` retains session history. Operational connection details and account-meter history are intentionally not published.

## Current status

| Area | Actual evidence / status |
|---|---|
| Two-handle VR baseline | Implemented; Stef reported successful VRChat testing on 2026-09-15. |
| PC reading and movement | Stef tested after the separate LEZEN/VERPLAATSEN work and reported PC is now good. |
| Mobile custom reading | Partly functional, not accepted. Swipe also moves the avatar, controls sit too high, page space is wasted and pinch is absent. |
| Compilation | 212 Udon scripts completed in 00:09.290; AnyUdonSharpScriptHasError=False. |
| Native mobile Focus View | Research complete for choosing an experiment; no prototype implemented or device-tested. |
| Wider V1 open/close target | Still incomplete: current VR final drop remains Pin-dependent. |
| Public release | Existing beta-live status remains; no new public upload/release is claimed here. |

Stef performs practical tests. Codex did not start Play Mode, ClientSim, a build or a platform switch during these implementation/research steps.

## Scope and authorization

The completed implementation GO covered only Book_A, separate reading/movement on PC and mobile, preserving successful VR handles. Pinch was outside that work block.

The 2026-09-16 GO covers targeted GitHub documentation synchronization. It does not authorize the proposed native mobile prototype or a broader reader rewrite. A future explicit prototype request can give that GO; do not ask twice once it is given.

Book_B is not converted. No new ObjectSync/network reading state, video player, bookmarks, library, Lesson Books or converter was added. Presentation, VideoTXL, Poster, Entrance Text and Cinema implementation are not part of this block.

## Implemented physical and reading routes

Root in local Classroom scene:
`UIs/Other Toggles and Systems/E-Reader/Book_A_Root`.

- `Book_A`: original body, EReaderBook, local screen/controls and two-source body ParentConstraint.
- `LeftHandle` / `RightHandle`: separate pickups, return constraints, EReaderHandle; references under body.
- `EReaderPhysicalController`: first/second/final hold transitions, release and Home reset.
- `NonVRMoveHandle`: sibling movement-only pickup with EReaderMoveHandle; broad 200 x 50 x 20 mm trigger grip, body-local offset (0, 0.23, 0).
- `Book_A/NonVRActions`: separate WorldSpace UI, LEZEN 220 x 85 mm and TERUGZETTEN 180 x 45 mm.
- `Book_A_FocusCanvas`: custom local WorldSpace reader view driven by EReaderFocusView.

### VR

First VR handle activates reading once; second handle changes physical handling without reactivation. Releasing one of two leaves the reader held. Final release clears physical hold once, then current legacy Pin behaviour decides closure.

The saved baseline has:
- body scale 1;
- body constraint disabled with exactly two sources at 0/0;
- both return constraints enabled;
- both VR handles active/pickupable;
- no ObjectSync added to Book_A;
- physical Home position/rotation unchanged.

### PC and current mobile

- **LEZEN** calls Book_A.OpenReader without requiring pickup.
- **VERPLAATSEN** uses normal VRChat pickup movement but does not activate playback or write progress.
- Grip follows the body at rest and drives body pose only while held; it adds no source to the VR constraints.
- **X** closes the custom reading view, preserving physical placement and page/progress.
- **TERUGZETTEN** explicitly releases pickups and moves body to Home without erasing progress.
- Existing Book_A reset adapter delegates through the new movement handle so held movement is cleaned up before Home.
- Focus blocks the movement pickup while reading and restores availability on close.
- New UI/grip begins unavailable in the saved scene; local non-VR detection enables it and hides the thin VR handles on that client. VR clients keep their existing handle route.

## Custom focus implementation — not native VRChat Focus View

The earlier ScreenSpace overlay had click problems even with Tab. A local WorldSpace panel and matching trigger BoxCollider replaced it.

- Only the focus panel follows `VRCCameraSettings.ScreenCamera`; physical body/handles do not follow the camera.
- Panel height uses 94% of the camera view; distance is at least 0.5 m and beyond near clip.
- `AllowFocusView=false`, CanvasScaler disabled.
- Same `RT_EReader`, local playback manager, navigation and persistence.
- ScrollRect + RectMask2D provide page drag/pan.
- ZoomOut/ZoomIn/FitPage/FitWidth; default fit width; zoom range 0.5–4 times fit.
- PC Tab + wheel zoom; ScrollRect wheel scrolling disabled to prevent simultaneous pan.
- Current mobile layout reserves 32% of panel height plus additional spacing for the VRChat HUD. Stef's screenshot shows this workaround wastes reading space.
- No custom pinch implementation.

Focus release/exit now calls `ReleaseAllHandles()` / `ReleaseAtCurrentPose()`, not automatic `ResetReader()`. State is cleared before Drop callbacks to avoid duplicate release effects.

## Important implementation / product-design gap

[EREADER_V1_OPEN_CLOSE_DECISION_2026-09-15.md](EREADER_V1_OPEN_CLOSE_DECISION_2026-09-15.md) accepts final-drop-stays-open as the V1 target.

Current local source inspection on 2026-09-16 shows:

```text
EReaderBook.EndPhysicalHold
-> clear _isHeld
-> if Pin is off and custom focus is not open
-> CloseBook
```

Therefore successful physical handle testing does not prove the accepted final-drop-stays-open rule. Preserve both facts: working baseline, unfinished target requirement. Do not silently change VR semantics during mobile or documentation work.

The separate non-VR movement grip does not call BeginPhysicalHold/EndPhysicalHold and never opens reading by itself.

Current saved Book_A auto-sleep values: 1 metre, 30-second delay, 5-second checks. Generic C# defaults: 3 metres / 60 seconds. The product decision suggests about 4 metres / 15 seconds for future tuning. Those proposed values are not implemented/tested here. Holding/custom focus counts as nearby.

## Evidence boundaries

### Implementation / static checks

- New and existing UI actions target their correct Udon backings.
- Both new buttons fit inside their UI collider.
- Existing seven reading and four zoom actions preserved.
- Saved physical transforms/constraints/pickups and Book_B component snapshot matched the start snapshot.
- EReaderHandle.cs, EReaderPhysicalController.cs and EReaderLocalPlaybackManager.cs source hashes were unchanged during non-VR movement work.
- Book_B focusView/moveHandle remain empty, with its existing direct pickup.
- Scene and program assets saved; Saved=True, dirty=False at completion.
- Compiled programs contain OpenReader, movement pickup/drop, ReleaseAtCurrentPose, ResetReader and existing focus/zoom events.

### User runtime reports

- Prior VR handle baseline: successful according to Stef.
- PC after new reading/movement route: good according to Stef.
- Mobile: visible/partly functional but unresolved movement, layout and pinch complaints.

These broad reports do not prove every reset-during-hold, disable/re-enable, two-client, Quest, iOS, persistence-rejoin or post-change VR regression case. No blanket platform/multiplayer PASS is claimed.

## Native mobile research and next proposed step

Read [EREADER_MOBILE_FOCUS_VIEW_RESEARCH_2026-09-16.md](EREADER_MOBILE_FOCUS_VIEW_RESEARCH_2026-09-16.md).

Research distinguishes:
- native Focus View pan/zoom and avatar-stays-in-place documentation;
- allowing and detecting Focus View versus no confirmed documented enter/exit API;
- actual HUD occupancy and two-finger pinch still needing phone evidence;
- full-page ScrollRect potentially intercepting native gestures;
- canvas disable not being a documented native exit command.

### Proposed M0 — not built, awaiting implementation GO

One isolated stationary WorldSpace canvas with:
- centered pivot;
- UIShape with native Focus View allowed;
- static test page;
- one counter button;
- no ScrollRect, camera-follow loop or custom canvas-disabling close.

This slightly simplifies the research report's original RT/NEXT proposal:
1. Current book activation also opens the existing custom focus panel. A static page avoids mixing two view systems in the first proof.
2. CameraMode=FocusView reports a client mode, not the identity of the canvas. Do not treat every focused world UI as Book_A.

Stef tests entry gesture, pan, actual zoom gesture, avatar movement, counter-button input, HUD visibility/use, portrait/landscape and native exit. Only then decide whether and how to integrate existing RT, navigation, Lezen/X, local progress and fallback. Do not replace the current mobile route before that evidence.

## Changed local implementation files

New during the completed handle/focus/movement work:
- `Assets/Ereader/EReaderHandle.cs`
- `Assets/Ereader/EReaderPhysicalController.cs`
- `Assets/Ereader/EReaderFocusView.cs`
- `Assets/Ereader/EReaderMoveHandle.cs`
- corresponding .cs.meta, Udon .asset and .asset.meta files.

Modified:
- `Assets/Ereader/EReaderBook.cs`;
- `Assets/!StefanieInVR/Scripts/Managers/ResettableObject.cs`: optional generic custom-reset delegation, configured only for Book_A;
- `Assets/#Classroom/Scenes/Classroom.unity`;
- corresponding compiled Udon/program outputs.

EReaderLocalPlaybackManager.cs was preserved.

Latest movement serialized program:
`Assets/SerializedUdonPrograms/ed1ad2cdff7d93d4d961e599a32df773.asset` (+ meta).
Other Book_A program IDs are retained in the full local log. Full Udon compilation may reserialize unrelated compiler outputs; no claim that only these files changed on disk.

## Repository synchronization boundary

This GitHub update publishes the current handoff, research, current-work/start routing and reconciles design status. It does not upload the Unity scene, generated assets, implementation scripts or phone screenshot. This repository is not a complete backup of the local Unity project.

Existing source/reference files in GitHub are not automatically current merely because the documentation is current. For implementation inspect the actual linked local scripts and saved scene.

## Parked work and source projects

- Entrance Text / Poster persistence bugs: unresolved, parked; see `HANDOFF_2026-09-13_PERSISTENCE_BUGS.md`.
- PDF/EPUB converter and full standalone V1/library/visibility policy: later work, not implemented by this block.
- Cinema supplied a read-only physical pattern; no Cinema runtime modifications or new Cinema acceptance.
- Hosted Presentation Service unchanged.
- Existing public beta release status does not prove every subsequent Book_A change shipped publicly.

## Working agreement

Simple Dutch, one practical step at a time. Stef owns runtime acceptance.
Do not start Play Mode/platform switches independently.
No new backup unless a concrete need is discussed; Stef already has backups.
Keep source evidence, compile evidence and device tests separate.
Update the active handoff after completed authorized work and explicitly report whether GitHub was actually updated.
