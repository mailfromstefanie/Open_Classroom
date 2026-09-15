# Current Work — Open Classroom

Last updated: 2026-09-15 Europe/Amsterdam

## READ THIS FIRST

Current active product/design route:

`EREADER_V1_PRODUCT_SPEC_2026-09-14.md`

Accepted open/close refinement that overrides the older drop/Keep Open wording in that spec:

`EREADER_V1_OPEN_CLOSE_DECISION_2026-09-15.md`

Existing working e-reader baseline:

`EREADER_LIBRARY_HANDOFF_2026-09-05.md`

Parked unresolved persistence bugs:

`HANDOFF_2026-09-13_PERSISTENCE_BUGS.md`

Release-phase authority:

`HANDOFF_2026-09-12_BETA_LIVE.md`

Real Unity project:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

The real current Unity/VRChat behaviour remains the strongest source of truth.

---

## CURRENT STATUS — BETA WORLD LIVE / EREADER V1 FOUNDATION SELECTED

Open Classroom remains a live beta world.

Stef has deliberately selected a bounded EReader productization/foundation block before returning to the unresolved Entrance Text / Persistent Poster persistence bugs.

Those persistence bugs are **PARKED, NOT FIXED**.

Current active gate:

```text
standalone EReader product boundary
-> Book_A only
-> left/right ParentConstraint handles
-> preserve existing local reader behaviour
-> separate physical drop from reader close
-> prove first-handle / second-handle / final-drop-stays-open semantics
```

Do not broaden the first implementation task into bookmarks, Lesson Books, Book_B, website conversion or unrelated Classroom systems.

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

## ACCEPTED OPEN / CLOSE EXPERIENCE — 2026-09-15

Dropping the reader is no longer treated as "finished reading".

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

Cinema reference proof lowers uncertainty but does not mean the EReader implementation already exists.

---

## CURRENT EREADER CODE EVIDENCE

Current `EReaderBook.cs` was inspected during design.

Important existing behaviour:

```text
OnPickup()
-> _isHeld = true
-> PrepareForLocalReading()
-> ActivateReader()

OnDrop()
-> _isHeld = false
-> if Keep Open is false, CloseBook()
```

That is existing-code evidence, **not the accepted final V1 open/close behaviour**.

The 2026-09-15 product decision now requires physical hold/drop state to be separated from reader open/close state.

Therefore simply moving the `VRC Pickup` to two handles would break semantics, and preserving the old direct `OnDrop() -> CloseBook()` path would also produce the wrong product behaviour.

The two-handle implementation needs a safe input bridge / handle-count responsibility so that:

```text
first handle pickup
-> one true reader-pickup activation

second handle pickup
-> physical two-hand behaviour only
-> no duplicate reader reload

release one of two handles
-> still held
-> do not close reader

release final handle
-> physical hold ends once
-> reader remains open
```

Explicit Close / X and sustained distance-close become separate local close requests owned by reader/controller logic.

Preserve the existing local video/page/progress behaviour.

---

## FIRST IMPLEMENTATION TARGET — BOOK_A ONLY

Do not modify Book_B yet.

Before changing the scene, inspect the complete current real pieces required for safe integration, especially:

- current `EReaderBook.cs`;
- current `EReaderLocalPlaybackManager` family if needed;
- the reset component currently used by Book_A;
- Book_A hierarchy and Inspector wiring;
- current pickup/highlight setup;
- current Close / X wiring;
- current Keep Open / Pin use before changing its role.

Then implement only the smallest foundation needed to prove Book_A left/right pickup and the new drop-stays-open rule.

First acceptance gate:

```text
Book_A can be picked up from LEFT
Book_A can be picked up from RIGHT
only thin handle highlight appears
first handle opens the existing local reader once
second handle does not reload it
releasing one of two handles keeps the reader held/open
releasing final handle leaves the reader open
explicit Close / X closes the local reader
existing local page/navigation/progress behaviour remains intact
```

Quest/real-VR acceptance remains required before calling the new physical interaction proven.

Distance-close can be implemented in the same open/close manager block or immediately after the handle foundation, but must be proven before standalone V1 open/close behaviour is considered complete.

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

```text
1. read EREADER_V1_OPEN_CLOSE_DECISION_2026-09-15.md
2. read EREADER_V1_PRODUCT_SPEC_2026-09-14.md for the wider product design
3. inspect complete current Book_A physical/reset/pickup/Close wiring
4. inspect complete current EReaderBook and required local manager pieces
5. inspect Cinema HandheldUI source family as reference
6. design smallest Book_A left/right-handle bridge with physical drop separated from reader close
7. implement Book_A only
8. compile / ClientSim smoke test
9. VR/Quest physical highlight + handedness + drop-stays-open test
10. record accepted evidence
11. only then choose the next EReader V1 block
```

---

## WORKING STYLE WITH STEF

- Dutch;
- beginner-friendly;
- one small action at a time;
- explain why before technique;
- inspect complete current scripts/screenshots/wiring instead of guessing;
- backup before meaningful risk;
- Codex is a bounded implementation worker, not product owner;
- no broad autonomous redesign;
- tested real Unity/VRChat behaviour outranks documentation.

## SOURCE OF TRUTH

```text
real current Unity/VRChat behaviour
-> EREADER_V1_OPEN_CLOSE_DECISION_2026-09-15.md for accepted open/close semantics
-> EREADER_V1_PRODUCT_SPEC_2026-09-14.md for wider accepted EReader design
-> CURRENT_WORK.md for current gate
-> EREADER_LIBRARY_HANDOFF_2026-09-05.md for earlier proven baseline
-> HANDOFF_2026-09-13_PERSISTENCE_BUGS.md for parked persistence work
-> HANDOFF_2026-09-12_BETA_LIVE.md for release truth
-> older historical/recovery docs
```
