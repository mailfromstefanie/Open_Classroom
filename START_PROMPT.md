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
3. `EREADER_V1_PRODUCT_SPEC_2026-09-14.md`
4. `EREADER_LIBRARY_HANDOFF_2026-09-05.md` for the earlier working e-reader baseline
5. `HANDOFF_2026-09-13_PERSISTENCE_BUGS.md` only when the parked Entrance Text / Poster block matters
6. `HANDOFF_2026-09-12_BETA_LIVE.md` when release-phase context matters
7. older historical/recovery docs only when needed

Do not preload unrelated systems.

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
- Keep Open / Pin;
- bookmark data;
- reader UI/playback.

Physical/shared behaviour is a separate layer:

- optional `VRCObjectSync` for physical movement;
- configurable Hide/Show policy;
- physical Reset/Home;
- optional shared Lesson Book links.

Do not make page/bookmark/normal reading state global merely because the object is networked.

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
- Cinema `HandheldUIHandle.cs` and related hierarchy as source reference.

Current known `EReaderBook` behaviour uses direct pickup/drop events to control local reading, so moving the pickup responsibility to two handles requires an explicit bridge/handle-count design.

Required semantics:

```text
first handle pickup
-> one true reader pickup
-> existing local reader opens once

second handle pickup
-> two-hand physical control
-> no duplicate media reload

one of two handles dropped
-> reader is still held
-> reader remains open

final active handle dropped
-> one true reader drop
-> existing Keep Open behaviour applies
```

First acceptance gate:

```text
Book_A works from LEFT
Book_A works from RIGHT
only thin handle highlight appears
existing local reading/navigation/progress is preserved
```

Do not proceed to Book_B until this gate is accepted.

---

## Accepted later EReader V1 scope — not the first Codex task

The accepted design includes later:

- always-active standalone EReader manager;
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

Canonical design:

`EREADER_V1_PRODUCT_SPEC_2026-09-14.md`

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
- existing local EReader media arbitration/page logic beyond the bounded handle integration;
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
Open Classroom -> EReader V1 -> Book_A left/right handle foundation
```

Then ask for/inspect the current Book_A reset/pickup wiring needed before the first change.
