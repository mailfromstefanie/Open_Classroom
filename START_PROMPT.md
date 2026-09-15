# Start Prompt — Open Classroom

## Entry point role

This is the direct Open Classroom specialist prompt.

For Stef's normal Nova/ChatGPT entrypoint across the StefanieInVR ecosystem, use:

`mailfromstefanie/StefanieInVR-Project-Hub/STARTPROMPT.txt`

The Hub routes cross-project work. It never overrides this repository or the real tested Unity/VRChat world.

Real Unity project:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

---

## Read first — mandatory

Read in this order:

1. `AGENTS.md`
2. `CURRENT_WORK.md`
3. `EREADER_V1_OPEN_CLOSE_DECISION_2026-09-15.md`
4. `EREADER_V1_PRODUCT_SPEC_2026-09-14.md`
5. `EREADER_LIBRARY_HANDOFF_2026-09-05.md` for the earlier working e-reader baseline
6. `HANDOFF_2026-09-13_PERSISTENCE_BUGS.md` only when the parked Entrance Text / Poster block matters
7. `HANDOFF_2026-09-12_BETA_LIVE.md` when release-phase context matters
8. older historical/recovery docs only when needed

Do not preload unrelated systems.

The 2026-09-15 open/close decision overrides the older Product Spec wording where the final drop was allowed to close the reader through Keep Open.

---

## Critical current truth — EReader V1 foundation active

Open Classroom is still a live beta world.

Stef deliberately selected a bounded EReader foundation/productization block.

The previously active Entrance Text / Persistent Poster persistence bugs are:

```text
PARKED
NOT FIXED
NOT CLOSED
```

Their canonical evidence remains in:

`HANDOFF_2026-09-13_PERSISTENCE_BUGS.md`

Do not accidentally resume or rewrite that persistence work while handling the EReader unless Stef asks.

---

## EReader V1 main rule

```text
READING = ALWAYS LOCAL PER PLAYER
```

Keep local:

- loaded/selected book;
- video/screen;
- current page;
- last-read page;
- bookmark data;
- reader UI/playback;
- open/close state;
- distance-close state/timer.

The existing Keep Open / Pin control is legacy current behaviour. Do not blindly delete or repurpose it, but its old role as the thing that decides whether a normal drop closes the reader is no longer the accepted V1 behaviour.

Physical/shared behaviour is a separate layer:

- optional `VRCObjectSync` for physical movement;
- configurable Hide/Show policy;
- physical Reset/Home;
- optional shared Lesson Book links.

Do not make page/bookmark/normal reading/open-close state global merely because the object is networked.

---

## Accepted open / close behaviour

Dropping the physical EReader does not mean the user is finished reading.

Required rule:

```text
final active handle dropped
-> physical hold ends
-> local reader stays open
```

The local reader closes only through:

```text
explicit Close / X
OR
sustained distance from the reader beyond a grace period
```

Desired distance-close pattern:

```text
user moves out of range
-> start grace timer

user returns before timer expires
-> cancel timer
-> stay open

user remains out of range for the full grace period
-> close local reader
```

Starting tuning recommendation is about 4 metres and about 15 seconds, but those values are not runtime-proven and may be tuned after real VR testing.

Handles are physical input. Reader manager/controller owns open/close truth.

---

## Standalone prefab boundary

The EReader must be built as its own `StefanieInVR EReader` product family.

It must not depend on Paper Tablet scripts or Classroom global control scripts.

The EReader manager/controller owns behaviour and exposes public control methods.

Possible inputs may later include:

- Paper Tablet sprite buttons;
- normal Unity UI buttons;
- physical 3D buttons/colliders;
- a book stand;
- other Udon scripts.

Input/adapters request actions; they do not own EReader state.

---

## Cross-project source / target rule for handles

SOURCE PROJECT / proven reference:

`mailfromstefanie/Stefanies-Art-House-Cinema`

Reusable reference family:

`StefanieInVR.HandheldUI`

TARGET PROJECT / implementation authority:

`mailfromstefanie/Open_Classroom`

Cinema's Paper Tablet proves the left/right ParentConstraint handle pattern. It does NOT mean the EReader version is already implemented.

Do not edit Cinema during this Classroom work.

---

## Exact first implementation gate — Book_A only

The reason for this change is physical Quest usability: the current pickup highlight can light too much of the e-reader surface. Stef wants only a thin pickup strip on either side, with left- and right-handed use.

Before changing the Unity scene inspect the current real:

- `EReaderBook.cs`;
- relevant `EReaderLocalPlaybackManager` pieces if needed;
- current Book_A reset component/wiring;
- current Book_A hierarchy;
- current VRC Pickup / highlight configuration;
- current Close / X wiring;
- current Keep Open / Pin use;
- Cinema `HandheldUIHandle.cs` and related hierarchy as source reference.

Current known `EReaderBook` behaviour uses direct pickup/drop events to control local reading. That is existing-code evidence, not the final V1 product rule.

Moving pickup responsibility to two handles therefore requires an explicit bridge/handle-count design **and** separation of physical drop from reader close.

Required semantics:

```text
first handle pickup
-> one true reader pickup/activation
-> existing local reader opens once

second handle pickup
-> two-hand physical control
-> no duplicate media reload

one of two handles dropped
-> reader is still physically held
-> reader remains open

final active handle dropped
-> physical hold ends once
-> reader remains open

Close / X
-> explicit local close request
```

First acceptance gate:

```text
Book_A works from LEFT
Book_A works from RIGHT
only thin handle highlight appears
first pickup opens once
second pickup does not reload
one-handle release does not close
final-handle release does not close
Close / X closes locally
existing local reading/navigation/progress is preserved
```

Do not proceed to Book_B until this gate is accepted.

Distance-close can follow in the same manager block or immediately after this physical handle gate, but it must be tested before standalone V1 open/close behaviour is considered complete.

---

## Accepted later EReader V1 scope — not the first Codex task

The accepted design includes later:

- always-active standalone EReader manager;
- sustained-distance automatic local close with tunable threshold/grace period;
- Hide/Show for performance;
- Local or Global visibility configuration;
- Reset/Home that does not erase reading data;
- in-world compatible book URL input/load path;
- max 5 remembered local books;
- persistent last-read page;
- max 20 persistent bookmarks per book;
- optional bookmark names, max 32 characters;
- unnamed bookmark = `Page <number>`;
- scrollable bookmark panel over the reader;
- explicit Library Full choice instead of automatic deletion;
- up to 5 shared teacher/admin Lesson Book links;
- shared lesson offering, local student reading.

Canonical wider design:

`EREADER_V1_PRODUCT_SPEC_2026-09-14.md`

Canonical open/close refinement:

`EREADER_V1_OPEN_CLOSE_DECISION_2026-09-15.md`

---

## Future website converter — planned only

A future book-converter product is planned:

```text
PDF or EPUB upload
-> title / author
-> preview / format-appropriate options
-> convert
-> hosted StefanieInVR URL OR downloadable MP4 for self-hosting
```

PDF should initially preserve original page layout.
EPUB may later support reflow options such as font size, margins and line spacing.

Hosted access may later use codes and limits for pages, hosted-book count, retention/storage and download permission.

A stable Book ID / metadata concept is desired for robust progress/bookmark identity.

Important:

```text
PLANNED CONVERTER
!=
LIVE PRESENTATION SERVICE CHANGE
```

Do not modify the Presentation Service because this future plan exists.

---

## Protected systems

Do not casually modify:

- exact VideoTXL 2.5.1;
- standalone Presentation Core/integration;
- physical projector/screen path;
- Paper Tablet structure/style;
- existing local EReader media arbitration/page logic beyond the bounded handle/open-close integration;
- current PlayerData last-page behaviour;
- unrelated reset/Marker systems;
- local table screens;
- accepted manual Persistent Poster physical baseline.

---

## Working method with Stef

- Dutch;
- beginner-friendly;
- explain why before technique;
- one small technical action at a time;
- inspect complete real scripts/screenshots/Inspector wiring instead of guessing;
- backup before meaningful risk;
- Codex is a bounded implementation worker, not product owner;
- no broad autonomous refactors;
- real Unity/VRChat behaviour outranks documentation.

If Stef pastes only this prompt and asks no specific question, orient her to:

```text
Open Classroom -> EReader V1 -> Book_A left/right handle + drop-stays-open foundation
```

Then ask for/inspect the current Book_A reset/pickup/Close wiring needed before the first change.
