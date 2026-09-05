# Open Classroom Multi E-Reader / Library Handoff — 2026-09-05

Status: **FUNCTIONAL IN CLIENTSIM — REAL VR / MULTIPLAYER / QUEST EVIDENCE STILL OPEN**

This is the durable handoff for the first reusable multi-book e-reader/library implementation in the real Open Classroom Unity project.

## Real Unity project

Use:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

Do not confuse this with the older checkout:

`E:/GitHub/Open_Classroom`

The tested real Unity scene is authoritative.

## Current architecture

The accepted Classroom implementation uses:

```text
one local EReaderLocalPlaybackManager
-> one VRCUnityVideoPlayer
-> one shared RT_EReader
-> multiple lightweight EReaderBook instances
```

The important rule is:

```text
many physical books
!= many video players

one local active e-reader video pipeline per client
```

Current example books:

- `Book_A`
- `Book_B`

Current Home transforms:

- `Book_A_Home`
- `Book_B_Home`

Current hierarchy reported by Codex:

```text
UIs/Managers/EReader Local Playback Manager

Other Toggles and Systems/E-Reader
  Book_A
  Book_B
  Book_A_Home
  Book_B_Home
```

## Existing visual/UI work preserved

The e-reader was not rebuilt from scratch.

The existing tablet/e-reader layout was reused.

Canvas physical dimensions remain:

- 160 x 15
- scale 0.001

Future work should keep UI physically contained on the tablet model.

Do not enlarge the world-space Canvas blindly.

## Per-book configuration

Each physical book has its own:

- direct MP4 URL;
- title;
- optional author/creator metadata;
- local current page/bookmark state;
- local Keep Open state;
- screen renderer;
- local Canvas/UI;
- VRC Pickup;
- kinematic Rigidbody;
- VRCObjectSync;
- existing ResettableObject integration.

Current example books still reportedly use the same test URL until Stef assigns a second real book URL.

For better multiplayer/content-switch proof, assign Book_B a different MP4 before final acceptance.

## Physical pickup/networking model

Physical books are shared through normal VRChat components:

- one VRC Pickup;
- Rigidbody;
- `isKinematic = true`;
- gravity OFF;
- VRCObjectSync;
- Auto Hold ON;
- Orientation = Any.

No custom page/video state is synchronized.

The physical object moves for everyone; reading remains local.

## Left/right hand direction

The implementation follows the existing World Globe style:

- one pickup root;
- Auto Hold;
- Orientation Any;
- no competing second VRC Pickup;
- no forced Exact Grip.

This is intended to support both hands.

**Real headset comfort/orientation is not proven yet.**

Do not mark handedness as accepted until Stef tests it in VR.

## Local reading model

These states are local only:

- active book;
- page;
- bookmark;
- Keep Open;
- loading;
- reader Canvas visibility;
- reader screen visibility.

Remote physical pickup movement must not cause a local MP4 load.

Two users should be able to read different books/pages independently.

## Current controls

Compact controls:

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

Media contract:

```text
one page/spread = one second
```

Current tested real MP4 reported:
- 213 pages detected.

No autoplay.

Direct typed page-number entry is not currently required.

## Keep Open

Local behavior:

### Keep Open OFF

```text
drop book
-> Canvas OFF
-> screen renderer OFF
-> e-reader playback stops
```

### Keep Open ON

```text
drop book
-> physical book stays where placed
-> active local reader remains visible
-> current book remains locally active
```

### X

```text
close reader
-> Keep Open OFF
-> local screen/UI OFF
-> local playback stops
```

## One-active-book / last-touched-wins

Hard local rule:

```text
maximum active EReader video = 1 per local client
```

Example:

```text
Book A active
-> activate Book B
-> A hides locally immediately
-> A playback ownership ends
-> B shows Loading
-> B becomes the only active local reader
```

A prior Keep Open book also loses local reader output when another book becomes active.

## Stale load protection

Rapid switching was explicitly tested.

Scenario:

```text
A begins loading
-> A is dropped / superseded
-> B is activated
-> old A OnVideoReady arrives late
```

Result reported:

- A cannot reactivate itself;
- A cannot overwrite B;
- B remains current active book.

Preserve this protection.

## Loading / URL cooldown

Several-second media load latency is expected and must be treated as normal.

Current UX includes a Loading state.

Do not spam repeated PlayURL requests.

Respect VRChat URL/video cooldown behavior.

## Local bookmark behavior

Each book remembers the last local page during the current world session.

Example:

```text
read Book A page 53
-> close/drop
-> reopen Book A
-> load
-> return to page 53
```

No persistent cross-session bookmark system is required yet.

## Toggle / reset integration

Existing Classroom systems are reused.

### Toggle

If an active book is toggled OFF:
- its local reader session closes cleanly.

### Reset

Books use the existing ResettableObject / ResetObjectManager approach.

Current intended book group:

`Reset Group 6`

Each book has its own Home transform.

Preserve separation:

```text
Toggle = availability/visibility
Reset = physical position
```

Do not make Reset implicitly change toggle state.

Do not make Toggle implicitly reset physical position.

## ClientSim evidence — PASSED

Reported working:

- real MP4 load;
- 213 page detection;
- Loading state;
- First;
- Previous;
- Next;
- -10;
- +10;
- page bounds;
- local bookmark restore;
- Keep Open OFF;
- Keep Open ON;
- X close;
- Book A -> Book B arbitration;
- rapid A/B switch during loading;
- stale callback protection;
- Toggle OFF closes active reader;
- Reset brings Book B exactly Home;
- inactive e-reader renderers OFF;
- idle e-reader playback stopped;
- Unity compile clean;
- no compile errors.

Regression checks also reported:

- Presentation Slot 1 still loads;
- Presentation can pause at slide 1;
- Presentation stop restores physical screen;
- local VideoTXL playback restores.

Presentation and VideoTXL scripts were not modified for the e-reader implementation.

## Evidence still open

Do not claim these as proven yet:

1. real headset left-hand comfort;
2. real headset right-hand comfort;
3. actual physical orientation/grip quality;
4. real two-player VRCObjectSync proof for e-reader pickups;
5. real two-player independent local reading;
6. Quest device profiling/performance.

## Performance audit

Read:

`PERFORMANCE_AUDIT_2026-09-05.md`

Important current findings:

- desktop ClientSim idle CPU/Udon/UI/geometry cost is low;
- `RT_EReader` currently uses 2x MSAA + automatic mipmaps;
- `VideoTXLCRT-Quest` currently uses 2x MSAA + automatic mipmaps;
- `M_EReaderScreen.mat` has a suspicious emission map pointing to `VideoTXLCRT-Quest`;
- large transparent Graphlit glass is a likely Quest fill-rate risk, not yet a proven bottleneck;
- three NPOT PC poster textures account for about 69 MB reported uncompressed desktop texture memory;
- no performance optimization changes were applied during the audit.

## Exact next session rule — 2026-09-06

**FIRST make a fresh full backup of the current working Unity project including the new e-reader implementation.**

Do not begin optimization before that backup exists.

After the fresh backup, use this conservative order:

```text
1. verify/fix M_EReaderScreen emission reference
2. validate e-reader visually
3. RT_EReader:
   MSAA 1x
   automatic mipmaps OFF
4. retest e-reader
5. VideoTXLCRT-Quest:
   MSAA 1x
   automatic mipmaps OFF
6. retest normal VideoTXL
7. retest Presentation takeover/restore
8. stop
```

Do NOT in the same round:

- change transparent glass;
- normalize poster textures;
- change RT_PresentationVideo;
- redesign Presentation/VideoTXL architecture;
- broadly refactor the Classroom.

Only keep each optimization if the focused regression test stays clean.

## Later acceptance route

After the narrow performance cleanup:

```text
assign Book_B a different real MP4
-> headset hand-comfort test
-> real two-player pickup + independent reading test
-> Quest device/performance test
-> beta library use
```

## Later sellable prefab direction

The current Classroom implementation is technical proof, not the finished commercial product.

Later productization may include:

- clean standalone prefab/package;
- automatic manager registration;
- creator-friendly book duplication;
- creator-friendly URL/title/author/cover fields;
- optional reset/toggle adapters;
- cover workflow;
- optional persistent bookmarks;
- conversion tools;
- documentation;
- clean-project import test;
- PC/Quest/multiplayer release matrix.

Do not productize this during the current Classroom stabilization phase.
