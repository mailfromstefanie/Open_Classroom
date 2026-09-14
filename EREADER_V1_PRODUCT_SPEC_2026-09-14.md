# StefanieInVR EReader V1 Product Specification — 2026-09-14

Status: **DESIGN ACCEPTED / IMPLEMENTATION NOT STARTED**

This document records Stef's accepted product direction for the next EReader work block. It is a product/architecture specification, not runtime proof.

Real Unity project remains authoritative:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

Existing proven Classroom baseline remains documented in:

`EREADER_LIBRARY_HANDOFF_2026-09-05.md`

The goal is to evolve the working Classroom e-reader into a clean, standalone `StefanieInVR EReader` prefab without coupling it to the Paper Tablet or Classroom-specific global control scripts.

---

## 1. Main product rule

Reading is always LOCAL per player.

Always-local reader responsibilities:

- selected/loaded book;
- actual book screen/video output;
- current page;
- last-read page;
- Keep Open / Pin state;
- bookmark list;
- bookmark names;
- bookmark navigation;
- reader UI visibility;
- video loading/playback state.

Do not turn page state, bookmarks or normal reader playback into shared/global state.

Physical-world responsibilities are separate and may optionally be shared:

- physical e-reader position/rotation when a creator adds `VRCObjectSync`;
- Hide/Show policy, configurable as Local or Global;
- Reset policy, configurable where technically valid;
- optional shared Lesson Book links from an admin/teacher.

The physical/network layer must never silently make reading state global.

---

## 2. Standalone prefab boundary

The EReader must not depend on the Open Classroom Paper Tablet scripts.

The prefab owns its own always-active manager and exposes simple public control methods so external inputs may drive it.

Possible external inputs include:

- normal Unity UI/sprite buttons;
- Paper Tablet buttons;
- physical 3D buttons/colliders;
- a book stand with physical controls;
- another Udon script.

External input is a request only. The EReader manager/controller remains the behavioural authority.

Conceptual split:

```text
PHYSICAL HANDHELD
left/right handles + constraints + optional VRCObjectSync

LOCAL READING
book URL + video/screen + page + progress + bookmarks

EREADER MANAGER
visibility/reset policy + public control API + persistent local library coordination

OPTIONAL LESSON BROADCASTER
shared teacher-selected book links only

INPUT ADAPTERS
Paper Tablet / sprite buttons / physical buttons / other Udon
```

---

## 3. Left/right physical pickup design

Accessibility/handedness requirement:

- the e-reader must be comfortably pickup-able from the LEFT side;
- the e-reader must be comfortably pickup-able from the RIGHT side;
- both handles may be held at the same time;
- only the thin left/right pickup strips should highlight, not the full reader plane/body.

SOURCE PROJECT / reference:

`mailfromstefanie/Stefanies-Art-House-Cinema`

Proven reference family:

`StefanieInVR.HandheldUI`

Reference scripts:

- `HandheldUIHandle.cs`;
- `HandheldUIReset.cs` as architectural reference only where useful.

TARGET PROJECT / implementation authority:

`mailfromstefanie/Open_Classroom`

The Cinema HandheldUI pattern is reusable reference evidence. It does not mean the Classroom EReader implementation already exists.

Target conceptual hierarchy for one reader:

```text
EReader_Root
├── LeftHandle
│   ├── VRC Pickup
│   ├── collider / thin pickup-highlight renderer
│   ├── ParentConstraint -> LeftHandleRef
│   └── HandheldUIHandle
├── RightHandle
│   ├── VRC Pickup
│   ├── collider / thin pickup-highlight renderer
│   ├── ParentConstraint -> RightHandleRef
│   └── HandheldUIHandle
└── Book_Body
    ├── Rigidbody
    ├── ParentConstraint <- LeftHandle + RightHandle
    ├── optional VRCObjectSync
    ├── EReader reading/controller components
    ├── model / Screen / Canvas
    ├── LeftHandleRef
    └── RightHandleRef
```

Exact hierarchy/wiring may be adjusted after inspecting the real current Unity hierarchy. Avoid parent/constraint cycles.

Required hold semantics:

```text
first handle pickup
-> physical reader follows that handle
-> local reader activates once

second handle pickup
-> two-hand constraint behaviour
-> do NOT restart/reload the local book

release one of two handles
-> other handle continues controlling reader
-> local reader remains held/open

release final active handle
-> one true reader-drop event
-> Keep Open decides whether local reading stays visible
```

Current `EReaderBook.cs` uses one `_isHeld` boolean and direct `OnPickup()` / `OnDrop()` events. The two-handle implementation must therefore add a safe handle-count/bridge responsibility rather than treating every handle drop as the final book drop.

---

## 4. Local screen behaviour

World entry:

- physical reader exists at its Home position;
- screen/reader content is inactive until the local user activates/picks up the reader.

Pickup:

- first local handle pickup activates the local reading screen and existing book behaviour;
- reading remains local even when physical movement is networked.

If another user takes/moves the shared physical reader while a local user's reader was open, the local reader should close rather than continue reading a physically unavailable reader.

This behaviour needs explicit multiplayer acceptance before being called proven.

---

## 5. Optional physical movement sync

Movement networking is creator-selectable before upload.

Desired rule:

```text
no VRCObjectSync
-> physical movement need not be shared

VRCObjectSync added/configured by creator
-> other players may see physical movement
```

The EReader reading model remains local in both cases.

Do not automatically add page/bookmark/global-reading sync merely because a `VRCObjectSync` exists.

---

## 6. Reset / Home

Each reader has a Home/reference position.

Public manager responsibility:

`ResetReader()`

Reset means ONLY:

- return the physical reader to Home position/rotation;
- safely release active physical handles when necessary.

Reset must NOT:

- clear current page;
- delete bookmarks;
- delete persistent progress;
- forget a book profile;
- change the user's reading history.

Reset controls must be input-agnostic. They may be driven by:

- a normal UI sprite/button;
- Paper Tablet integration;
- a physical 3D Reset button;
- a book stand control.

Local/global reset configuration must respect the networking reality of the physical object. In particular, a locally moved transform cannot remain independently local if an authoritative `VRCObjectSync` is actively synchronizing the same object.

---

## 7. Hide / Show for performance

The standalone manager remains active even when reader visuals are hidden.

Creator-selectable policy before upload:

```text
Visibility Mode = Local
or
Visibility Mode = Global
```

Hide should close/disable the expensive local reader path as well as visuals:

```text
Hide
-> close local reader
-> stop local video/playback responsibility
-> screen OFF
-> Canvas/UI OFF
-> physical reader visuals OFF
-> always-active manager remains available
```

Show makes the physical reader available again without deleting local reading progress/bookmarks.

Global visibility, when implemented, requires reconstructable shared state for late joiners; do not rely on a one-shot network event as the only truth.

---

## 8. User-entered book URL

The standalone prefab should include a creator/user-facing URL input path so a user can load a compatible book MP4 in-world.

Desired experience:

```text
Book URL [________________________]
[ LOAD ]
```

After a book is loaded, the local persistent library attempts to recognize the book and restore its saved progress/bookmarks.

Do not assume a stored string can always be silently reconstructed into a runtime `VRCUrl` without user selection/input. Design the persistence identity separately from the actual safe URL-loading action.

---

## 9. Persistent personal library

Accepted V1 limits:

- maximum **5 remembered books per local player**;
- maximum **20 bookmarks per remembered book**;
- bookmark name is optional;
- bookmark name maximum **32 characters**;
- unnamed bookmark displays as `Page <number>`.

When all 5 book profiles are occupied and the user loads a new unrecognized book:

- do NOT automatically delete an older book;
- show a clear `Library full` message;
- user must choose which saved book/profile to remove/replace.

Each saved book profile conceptually stores:

- stable book identity;
- last-read page;
- up to 20 bookmark page numbers;
- optional names for those bookmarks.

The current working last-page persistence is preserved as a requirement alongside bookmarks.

---

## 10. Persistent bookmarks UI

The bookmark UI is a small panel that slides/opens over the reader rather than permanently lengthening the existing control strip.

Concept:

```text
BOOKMARKS

Introduction          p.12   [x]
Important part        p.53   [x]
Page 104             p.104   [x]

        [ + BOOKMARK ]
```

Requirements:

- scrollable list (`ScrollRect` style);
- selecting a bookmark jumps directly to its stored page;
- each row allows deletion, preferably with a small `X`;
- `+ BOOKMARK` bookmarks the current page;
- optional name input;
- empty name becomes `Page <number>`;
- bookmarks persist locally for the player.

The exact UI layout can be refined later, but the behaviour above is accepted.

---

## 11. Lesson Books / teacher-admin shared links

Optional lesson module: an authorized admin/teacher can publish up to **5 shared Lesson Book links**.

The shared/global responsibility is ONLY the offered lesson-book selection, not the student's reading state.

Concept:

```text
LESSON BOOKS
1. Lesson book A
2. Lesson book B
3. Assignment
4. Reference
5. Extra reading
```

Student flow:

```text
teacher publishes/selects lesson URL globally
-> student sees/selects offered book
-> student chooses LOAD
-> book loads into that student's LOCAL reader
-> current page / last page / bookmarks remain local
```

Once a student starts reading a lesson book, it may use one of the student's five persistent personal book profiles so progress/bookmarks can be restored later.

The teacher/admin UI may later be integrated into a Paper Tablet, but the lesson-link component itself must not depend on the Paper Tablet prefab.

---

## 12. Future Book Converter website — PLANNED, NOT IMPLEMENTED

This is future product direction only. Do not modify the live Presentation Service merely because this plan exists.

Desired website entry:

```text
CREATE VR BOOK
[ Upload PDF ] [ Upload EPUB ]
```

Common metadata:

- title;
- author;
- optional cover / metadata as product design develops.

### PDF path

V1 direction:

- preserve original page layout/images as faithfully as possible;
- convert pages into the media format expected by the VR reader;
- do not promise arbitrary font-size reflow while preserving exact PDF layout.

### EPUB path

Planned reflow-friendly options may include:

- font size;
- margins;
- line spacing;
- reader page dimensions;
- image preservation where practical;
- preview before final conversion.

Fixed-layout EPUB and DRM-protected EPUB require separate handling/rules and are not automatically supported by this design note.

### Preview and output

Desired flow:

```text
upload
-> title / author
-> layout options where supported
-> preview
-> confirm
-> convert
```

Output choices:

```text
HOST ON STEFANIEINVR
-> creator gets a VRChat-usable URL

or

DOWNLOAD MP4
-> creator self-hosts on own compatible server
```

Hosted access may later use admin-issued codes with configurable limits such as:

- maximum pages per book;
- maximum number of hosted books;
- retention duration;
- storage/removal rules;
- upload size;
- download permission.

A stable Book ID / metadata concept is desired so personal progress/bookmarks can identify a book more robustly than by a mutable server URL alone. Exact runtime metadata transport is still to be designed and must not be invented during the physical-handle foundation task.

---

## 13. V1 implementation phases

Do not ask Codex to build the whole product at once.

Accepted order:

```text
1. standalone prefab architecture boundary
2. Book_A left/right ParentConstraint handles
3. preserve/reconnect existing local pickup -> reading behaviour
4. prove first-handle / second-handle / final-drop semantics
5. hide/show manager
6. physical reset/home path
7. Local/Global configuration where valid
8. in-world URL loader
9. 5-book persistent personal library
10. persistent last-page integration
11. 20 persistent bookmarks per book + ScrollRect UI
12. up to 5 shared Lesson Book links
13. PC / PCVR / Quest / multiplayer acceptance
14. only then design/build the website converter implementation
```

---

## 14. Exact next Codex gate

FIRST implementation target is **Book_A only**.

Before modifying the Unity scene, inspect the complete current real e-reader family needed for safe integration, especially:

- current `EReaderBook.cs`;
- current `EReaderLocalPlaybackManager` family as needed;
- current reset component used by `Book_A`;
- current `Book_A` hierarchy/Inspector wiring;
- current pickup/highlight setup.

Use Cinema `StefanieInVR.HandheldUI` as SOURCE reference.
Open Classroom is TARGET authority.

First acceptance gate:

```text
Book_A can be picked up from LEFT or RIGHT
-> only thin handle highlight is used
-> first handle opens existing local reader once
-> second handle does not reload reader
-> releasing one of two handles keeps reader held
-> releasing final handle produces one normal drop
-> existing local reading/navigation/progress behaviour is not regressed
```

Do not proceed to Book_B, bookmarks, global controls or converter work until this gate is observed and accepted.

---

## 15. Parked existing Classroom work

The previously active Entrance Text / Persistent Poster persistence bugs are NOT resolved by this design session.

They remain documented in:

`HANDOFF_2026-09-13_PERSISTENCE_BUGS.md`

They are temporarily parked while Stef deliberately works on the EReader V1 foundation. Do not rewrite them as fixed or closed.
