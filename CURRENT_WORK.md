# Current Work — Open Classroom

Last updated: 2026-09-16 Europe/Amsterdam

## READ THIS FIRST

Current implementation and evidence:
- [EREADER_BOOK_A_WORK.md](EREADER_BOOK_A_WORK.md) — maintained Book_A handoff; PC accepted, previous VR handle baseline accepted, mobile not accepted.
- [EREADER_MOBILE_FOCUS_VIEW_RESEARCH_2026-09-16.md](EREADER_MOBILE_FOCUS_VIEW_RESEARCH_2026-09-16.md) — research only; native mobile prototype not built or authorized yet.

Accepted product design:
- [EREADER_V1_PRODUCT_SPEC_2026-09-14.md](EREADER_V1_PRODUCT_SPEC_2026-09-14.md).
- [EREADER_V1_OPEN_CLOSE_DECISION_2026-09-15.md](EREADER_V1_OPEN_CLOSE_DECISION_2026-09-15.md) — final-drop-stays-open remains a product requirement, not completed implementation evidence.

Historical baseline: `EREADER_LIBRARY_HANDOFF_2026-09-05.md`.
Parked persistence: `HANDOFF_2026-09-13_PERSISTENCE_BUGS.md`.
Release authority: `HANDOFF_2026-09-12_BETA_LIVE.md`.

Real Unity project: `E:/Projects/Open_Classroom/#Unity/Open_Classroom`.
The real current Unity/VRChat behaviour remains the strongest implementation evidence.
This repository update synchronizes documentation, not the Unity scene/assets or a new world release.

---

## COMMERCIAL V1 RESEARCH — COMPLETED, NOT IMPLEMENTED

The cross-project commercial feasibility, architecture, privacy, licensing, packaging, sales and release-gate research is complete:
[full Project Hub report](https://github.com/mailfromstefanie/StefanieInVR-Project-Hub/blob/main/RESEARCH/EREADER_CONVERTER_COMMERCIAL_V1_RESEARCH_2026-09-16.md).

Decision relevant to Open Classroom:
- commercial V1 is a standalone paid EReader prefab plus PDF-only companion converter;
- target package is a namespaced UnityPackage, with VRChat Worlds/UdonSharp installed separately through Creator Companion;
- Desktop and only explicitly proven VR platforms may be advertised;
- mobile phone remains unsupported/experimental until real Android and iOS Focus View acceptance passes;
- the current final-drop-stays-open implementation gap remains a paid-release blocker;
- Book_B, EPUB, bookmarks/library, accounts, subscriptions, permanent hosting and advanced networking are cut from V1;
- clean-project import, drag-prefab/configure/build and upgrade-with-overrides tests must pass before first sale.

This records decisions only. It does not authorize or claim Unity/Udon implementation, packaging, testing or a new world upload.

---

## CURRENT STATUS — BOOK_A PC ACCEPTED / VR BASELINE PRESERVED / MOBILE OPEN

Open Classroom remains a live beta world. No new public release is claimed by this documentation update.

The bounded Book_A implementation has been built and saved locally:
- two separate VR ParentConstraint handles and EReader-specific controller;
- separate non-VR LEZEN, movement-only VERPLAATSEN grip and TERUGZETTEN;
- local custom WorldSpace reading view with zoom/pan;
- X closes custom focus while preserving physical placement and reading progress;
- 212 Udon scripts compiled successfully; this is separate from runtime acceptance.

Evidence reported by Stef:
- previous two-handle VRChat implementation: passed;
- PC after separate reading/movement implementation: good;
- mobile: partly functional, but swiping also moves the avatar, layout wastes page space and pinch is absent;
- no new post-change VR/Quest/multiplayer acceptance is inferred.

Current next gate: propose a small isolated native mobile Focus View proof with a static page and counter button. Await a separate implementation GO, then Stef tests it on her phone. Research is complete for choosing that experiment, not proof of native pinch/HUD/exit behaviour.

**Implementation/design gap:** current `EndPhysicalHold()` can still close the VR reader when Pin is off. The accepted final-drop-stays-open product decision is not implemented merely because handles passed. Keep that requirement open; do not silently change the proven VR path during mobile work.

Entrance Text / Persistent Poster persistence bugs remain **PARKED, NOT FIXED**.
Book_B, bookmarks/library, converter, networking and unrelated systems stay outside this block.

---

## EREADER — ACCEPTED PRODUCT DIRECTION

Canonical base design:

`EREADER_V1_PRODUCT_SPEC_2026-09-14.md`

Open/close behaviour override:

`EREADER_V1_OPEN_CLOSE_DECISION_2026-09-15.md`

Main accepted rule:

```text
READING = ALWAYS LOCAL PER PLAYER
```

Local-only reader responsibilities include:

- loaded/selected book;
- book video/screen;
- current page;
- last-read page;
- bookmark data;
- reader UI/playback state;
- open/close state;
- distance-close timer/state.

The existing Keep Open / Pin feature is legacy current behaviour. Its old meaning as the thing that decides whether a normal drop closes the reader is superseded for V1. Do not delete or repurpose it blindly before inspecting its current real use.

Physical/shared responsibilities are separate and configurable where appropriate:

- optional physical movement sync through `VRCObjectSync`;
- Hide/Show policy Local or Global;
- physical Reset/Home policy where technically valid;
- optional shared teacher/admin Lesson Book links.

Do not silently make reading/page/bookmark/open-close state global because the physical reader is networked.

---

## ACCEPTED OPEN / CLOSE EXPERIENCE — PRODUCT TARGET, NOT FULLY IMPLEMENTED

The accepted V1 design says dropping the reader is not "finished reading". The current VR implementation still has the legacy Pin-dependent final-drop close; this section describes the remaining product target, not a runtime PASS.

Accepted behaviour:

```text
release final active handle
-> physical hold ends
-> reader remains open
-> local reading state remains intact
```

The local reader closes only when:

1. the user explicitly presses Close / X; or
2. the user remains sufficiently far away for a grace period.

Distance-close must use a grace timer so briefly crossing the threshold does not instantly close the reader.

Starting tuning recommendation for testing:

```text
about 4 metres away
-> start timer
-> about 15 seconds still out of range
-> close local reader
```

Those exact values are starting defaults, not yet runtime-proven constants.

This behaviour is local per player.

---

## STANDALONE PREFAB BOUNDARY

The new EReader must not depend on Paper Tablet scripts or Classroom-specific global control managers.

The prefab should own an always-active EReader manager/controller and expose simple public functions that can later be called from:

- Paper Tablet sprite/UI buttons;
- normal Unity UI;
- physical 3D controls/colliders;
- a book stand;
- another Udon behaviour.

Paper Tablet integration is an adapter/input path only, not product ownership.

The manager/controller owns reader open/close truth. Handles only request/represent physical hold state.

---

## LEFT / RIGHT HANDHELD FOUNDATION

Problem motivating this work:

Quest pickup highlighting currently lights too much of the reader surface. Stef wants only a thin left or right pickup strip to highlight, matching the successful Paper Tablet interaction pattern.

Accessibility requirement:

- left-hand pickup;
- right-hand pickup;
- optional two-hand hold;
- only thin handle renderer/highlight per side.

SOURCE PROJECT / proven reference:

`mailfromstefanie/Stefanies-Art-House-Cinema`

Reusable source family:

`StefanieInVR.HandheldUI`

Relevant source scripts:

- `HandheldUIHandle.cs`;
- `HandheldUIReset.cs` as reference where useful.

TARGET PROJECT / acceptance authority:

`mailfromstefanie/Open_Classroom`

Cinema was inspected read-only as the source reference. The Book_A integration now exists and Stef reported the two-handle VRChat baseline successful. Current implementation uses EReaderHandle/EReaderPhysicalController, not a runtime dependency on Cinema scripts. Preserve it; see EREADER_BOOK_A_WORK.md for evidence limits.

---

## CURRENT EREADER CODE EVIDENCE — 2026-09-16

Local source inspection confirms:
- `EReaderPhysicalController` handles first/second/final physical hold transitions and two constraint sources.
- `EReaderBook.BeginPhysicalHold()` activates on VR first hold; Book_A non-VR movement is excluded.
- `EndPhysicalHold()` clears hold and calls `CloseBook` if Pin is off and custom focus is not open. Final-drop-stays-open is therefore still pending for VR.
- `OpenReader()` explicitly prepares and activates non-VR reading.
- `EReaderMoveHandle` moves/releases without activating playback or writing progress.
- custom focus release/exit uses cleanup at the current pose; only explicit reset goes Home.
- distance-close exists, but active hold/custom focus counts as nearby.

Saved Book_A scene tuning is 1 metre / 30 seconds / 5-second checks; generic script defaults are 3 metres / 60 seconds. The product decision's 4 metres / 15 seconds is a proposed future tuning target, not the current Book_A wiring or accepted runtime proof.

Current scripts/scene remain in the local Unity project. Existing repository script material is reference evidence and was not synchronized in this documentation-only update.

---

## CURRENT BOUNDED TARGET — MOBILE PROOF, BOOK_A ONLY

Preserve the accepted PC path and previous VR handle baseline. Book_B is not converted.

Proposed M0 test:
- one separate stationary WorldSpace canvas with UIShape and native Focus View allowed;
- centered pivot, static test image, one counter button;
- no page-wide ScrollRect and no camera-follow loop;
- native user entry/exit; no canvas-disable shortcut to force exit.

Stef tests actual entry, HUD occupancy, avatar movement, pan, zoom gesture, button interaction, portrait/landscape and exit. No autonomous Play Mode or platform switching.

Two integration issues remain for later:
1. `CameraMode == FocusView` does not identify which canvas is focused.
2. Existing Book_A activation opens the custom camera-follow panel; isolate the first native proof before connecting shared RT/playback.

M0 is proposed, **not implemented and not yet authorized**. The GitHub documentation GO does not authorize it. The older first-handle build checklist is history, not an instruction to rebuild working handles.

---

## ACCEPTED LATER EREADER V1 FEATURES — NOT FIRST GATE

After the physical foundation is accepted, the product plan includes:

- always-active standalone EReader manager;
- sustained-distance automatic local close with tunable threshold/grace period;
- Hide/Show for performance;
- Local/Global visibility configuration;
- physical Reset/Home that does not erase reading progress;
- in-world compatible book URL input/load path;
- personal persistent library of max **5 remembered books**;
- persistent last-read page per remembered book;
- max **20 persistent bookmarks per book**;
- optional bookmark name, max **32 characters**;
- unnamed bookmark label = `Page <number>`;
- scrollable bookmark panel over the reader;
- explicit Library Full message instead of automatic deletion;
- up to **5 shared Lesson Book links** offered by an authorized teacher/admin;
- lesson selection shared, actual reading/page/bookmarks local.

Do not implement all of these in one Codex task.

---

## FUTURE BOOK CONVERTER — PLANNED ONLY

Future website direction is recorded in the EReader V1 spec.

Desired high-level workflow:

```text
Upload PDF or EPUB
-> title / author
-> format-appropriate layout options
-> preview
-> convert
-> either StefanieInVR-hosted URL or downloadable MP4 for self-hosting
```

PDF direction: preserve original page layout.

EPUB direction: later allow reflow-oriented options such as font size, margins and line spacing where technically suitable.

Hosted access may later use codes/limits for maximum pages, number of hosted books, retention/storage rules and download permission.

A stable Book ID / metadata concept is desired for robust personal progress/bookmark recognition.

Important boundary:

**This converter is PLANNED. It is not implemented and does not authorize changes to the live Presentation Service.**

---

## PARKED — ENTRANCE TEXT / PERSISTENT POSTER

The 2026-09-13 persistence debugging block remains unresolved.

Canonical handoff:

`HANDOFF_2026-09-13_PERSISTENCE_BUGS.md`

Known Entrance Text real-client bug and the poster persistence investigation remain valid evidence.

Do not describe either as closed.

When Stef returns to that block, resume from the handoff rather than rebuilding from memory.

---

## PROTECTED WORKING FOUNDATIONS

Preserve unless current evidence proves a necessary change:

- Presentation Core/integration;
- exact VideoTXL 2.5.1;
- VideoTXL local suspend/restore during Presentation;
- physical projector/screen path;
- brightness/contrast/custom screen behaviour;
- paper tablet visual/interaction style;
- existing working local e-reader video/page arbitration;
- current PlayerData last-page progress behaviour;
- Marker/reset systems outside the bounded EReader integration;
- local table screens;
- accepted manual Persistent Poster physical baseline.

No broad refactor during beta.

---

## EXACT NEXT PHASE

1. Read this file and `EREADER_BOOK_A_WORK.md`.
2. Read `EREADER_MOBILE_FOCUS_VIEW_RESEARCH_2026-09-16.md`.
3. On a future prototype GO, inspect only the relevant current Book_A wiring and installed SDK APIs.
4. Build the isolated M0 test, compile and save.
5. Stef tests on her actual phone; record observations, not inferred passes.
6. Only after that evidence choose a full mobile integration and its entry/exit UX.

No mobile native prototype is built yet. The accepted wider open/close decision remains an explicit pending gap; it is not silently cancelled or implemented during documentation synchronization.

---

## WORKING STYLE WITH STEF

- Dutch;
- beginner-friendly;
- one small action at a time;
- explain why before technique;
- inspect complete current scripts/screenshots/wiring instead of guessing;
- Stef already has backups; do not create another without a concrete reason discussed with her;
- Stef performs runtime tests; code compilation is separate evidence;
- Codex is a bounded implementation worker, not product owner;
- no broad autonomous redesign;
- tested real Unity/VRChat behaviour outranks documentation.

## SOURCE OF TRUTH

```text
real current Unity/VRChat behaviour
-> EREADER_BOOK_A_WORK.md for current implementation and reported test evidence
-> EREADER_V1_OPEN_CLOSE_DECISION_2026-09-15.md for accepted target open/close semantics
-> EREADER_V1_PRODUCT_SPEC_2026-09-14.md for wider accepted EReader design
-> CURRENT_WORK.md for current gate
-> EREADER_LIBRARY_HANDOFF_2026-09-05.md for earlier proven baseline
-> HANDOFF_2026-09-13_PERSISTENCE_BUGS.md for parked persistence work
-> HANDOFF_2026-09-12_BETA_LIVE.md for release truth
-> older historical/recovery docs
```
