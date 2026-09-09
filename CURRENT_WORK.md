# Current Work — Open Classroom

Last updated: 2026-09-10 Europe/Amsterdam

## AUTHORITATIVE CURRENT STATUS

**Open Classroom is now functionally very close to complete.**

Protected working baseline:
- Presentation system = working / beta-ready;
- VideoTXL 2.5.1 integration = working and must remain pinned;
- VRCDN livestream route = investigated with real VRChat logs / Build & Run, not ClientSim;
- e-reader/library = substantially refined and now includes per-user PlayerData persistence for reading progress;
- five Marker Pro objects = dedicated shared reset points added;
- fresh offline and OneDrive backups exist after the latest stabilization work.

The final two planned Classroom feature additions are now present in the real Unity project:
1. persistent synchronized entrance title + body — V1.1 implemented and saved; Unity/UdonSharp compile and scene wiring verified; an earlier real VRChat build indicates personal body persistence appears to work for Stef; title persistence plus shared two-client visibility/late-join/host-leave acceptance remain open;
2. one persistent synchronized movable poster — implemented and editor-smoke-tested on 2026-09-10; the self-contained `Panel (Poster)` is deliberately staged inactive for the planned separate tablet tab; real VRChat URL, movement, multiplayer and late-join acceptance remain open.

The narrow real multiplayer acceptance pass for the new e-reader PlayerData behaviour and Marker Pro reset is still open. Do not lose that test obligation while Entrance Text implementation proceeds.

Do not reopen solved architecture questions or broad performance work unless new evidence appears.

## REAL UNITY PROJECT

Use:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

Do not confuse it with older checkouts.

The tested real Unity project remains the strongest source of truth for current scene wiring.

## LATEST RECOVERY POINT — 2026-09-08

Stef created:
- one fresh offline full backup;
- one online OneDrive backup;
- intermediate backups during major changes;
- screenshots of the old/new e-reader UI.

Treat this as the current strong recovery point.

Do not overwrite/discard it casually.

## LATEST WORKLOG

Read:

`WORKLOG_2026-09-08.md`

It records:
- VRCDN / AVPro investigation;
- e-reader UX changes;
- e-reader PlayerData persistence;
- Marker Pro reset extension;
- backup state;
- remaining multiplayer acceptance;
- final planned Entrance Text and Poster features.

## PRESENTATION — ACCEPTED WORKING BASELINE

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

Only semantic Presentation fields are synchronized:
- `modeActive`;
- `slotIndex`;
- `slideIndex`;
- `revision`.

Proven Presentation behaviour includes:
- 10 slots;
- First / Previous / Next;
- two-client synchronization;
- cross-client slide control;
- OFF/ON sync;
- same slot/slide restored after re-entry;
- late join;
- VideoTXL local suspend/restore;
- projector visibility;
- brightness/contrast;
- physical screen output;
- tablet UI integration.

Detailed acceptance:

`PRESENTATION_ACCEPTANCE_2026-09-05.md`

The old VideoTXL Presentation Playlist design remains superseded.

Do not recreate it.

## VIDEOTXL 2.5.1 — PROTECTED WORKING BASELINE

Keep the exact checked-in VideoTXL 2.5.1.

During Presentation:
- set `SyncPlayer.LocalPlaybackEnabled = false` locally;
- do not mutate VideoTXL's synchronized pause/play state;
- do not use `_TriggerPause()` as the local suspend API;
- do not use raw internal `BaseVRCVideoPlayer.Stop()` as the integration contract.

On exit:
- restore previous screen state;
- set `LocalPlaybackEnabled = true`;
- allow VideoTXL to restore/resync using its own supported path.

### Closed SourceManager incident

A stale null entry left after the removed Presentation playlist once broke all ordinary playlists.

It was removed.

Do not reopen this incident without new evidence.

## VRCDN / AVPRO — LATEST EVIDENCE

The 7/8 September livestream investigation produced an important test rule:

**ClientSim is not a valid AVPro / VRCDN playback acceptance environment.**

ClientSim's AVPro player is stubbed, so an AVPro livestream remaining in Loading there does not prove a VideoTXL or VRCDN defect.

Real VRChat Build & Run logs showed:
- VideoTXL selecting AVPro 1080p low-latency;
- the VRCdn stream route opening through the intended AVPro path;
- the physical video-wall renderer had become disabled and was re-enabled;
- warnings may occur for allowed domains and temporary rate limiting when switching videos too rapidly.

Protect:
- normal VideoTXL playback;
- VRCDN route;
- existing screen/display system;
- exact VideoTXL 2.5.1.

Do not change VideoTXL based only on ClientSim AVPro behaviour.

## PHYSICAL SCREEN / DISPLAY — WORKING

Preserve:
- existing physical `VideoScreen (quest)`;
- current VideoTXL/Unlit-based material/shader;
- ScreenManager path;
- projector visibility behaviour;
- renderer/collider handling;
- brightness/contrast controls;
- Presentation RenderTexture path.

Do not blindly reapply old screen-fit experiments.

## MULTI E-READER / LIBRARY — CURRENT 2026-09-08 STATUS

Architecture remains:

```text
one local EReaderLocalPlaybackManager
-> one VRCUnityVideoPlayer
-> one shared RT_EReader
-> multiple lightweight physical EReaderBook instances
```

Preserve:
- physical VRC Pickup + kinematic Rigidbody + VRCObjectSync;
- one shared playback pipeline;
- last-touched-wins local arbitration;
- inactive screen/player shutdown;
- reset/toggle integration;
- Presentation/VideoTXL separation.

### Latest UX changes

Reported implemented:
- desktop/mobile controls remain open after release;
- visible pickup highlights added on the sides;
- larger controls with stronger contrast;
- hover and pressed states;
- compact navigation symbols (`|<`, `<`, `>`);
- simplified page display;
- local automatic close after more than about 30 seconds farther than about one metre from the reader.

### PlayerData persistence — new

Last-read page is now stored using VRChat PlayerData.

Current intended/reported semantics:
- storage is per user;
- both physical e-readers use the same storage key/code;
- both readers therefore share that user's reading progress;
- this is personal persistence, not shared page synchronization.

This existing implementation is the preferred in-project reference for future PlayerData usage.

Do not invent a separate persistence framework unless needed.

### Remaining e-reader acceptance

Still recommended before the final new features:
- real two-user/multiplayer check of PlayerData behaviour;
- reconnect/re-entry persistence check where practical;
- confirm both readers share the same user's saved page as intended.

Quest profiling remains a separate future release/performance evidence question; do not fabricate a Quest PASS.

## MARKER PRO RESET — NEW

Five Marker Pro objects now have dedicated reset points.

Reported global reset behaviour:
- force/release as needed;
- return to original position;
- restore original rotation;
- shared result intended for everyone.

A final two-user multiplayer check is still recommended.

## EXACT NEXT SESSION

Do **not** begin with performance optimization.

Do **not** rebuild Presentation, VideoTXL, the e-reader or table screens.

First UI action:

```text
ADD ONE SEPARATE TABLET TAB FOR WELCOME + POSTER EDITING
```

Use the already staged inactive `Panel (Poster)` and the existing Entrance Text V1.1 editor. Do not squeeze either editor back into the reduced VIP layout. Inspect the current tablet tab manager arrays and lock-panel visibility rules before wiring the new tab. Keep the UI move/layout change separate from runtime architecture.

Then run one narrow real VRChat multiplayer acceptance pass covering:
1. entrance title/body synchronization and late join;
2. poster direct URL/image synchronization, movement, scale and late join;
3. unlocked non-VIP editing versus locked VIP-only editing;
4. host/publisher departure while the instance remains alive;
5. e-reader PlayerData last-page persistence and both books sharing it;
6. Marker Pro global reset visibility to both users.

If this passes, record the result as the golden Classroom baseline and keep the offline + OneDrive backups.

## FINAL FEATURE 1 — PERSISTENT ENTRANCE TEXT — ACTIVE

Read-only scene investigation and implementation planning are complete.

Canonical plan:

`ENTRANCE_TEXT_INVESTIGATION_PLAN_2026-09-09.md`

Current investigated entrance truth:

```text
UIs/Other Toggles and Systems/Canvas (Welcome)
├── Collider_Entrance
├── Image
├── Image
├── Text (Kop)
├── Text (Alinea)
└── Button (Open)
```

Important preserved behaviour:
- `Text (Alinea)` is the existing `TextMeshProUGUI` display target;
- its body text is currently Inspector-hardcoded;
- no entrance UdonSharp/C# manager exists yet;
- `Button (Open)` directly calls `Canvas (Welcome).SetActive(false)`;
- this locally hides both that visitor's welcome display and `Collider_Entrance`;
- one visitor opening the passage does not affect other visitors;
- this local dismissal behaviour must remain local and unsynchronized.

Collider finding:
- `Collider_Entrance` currently works but has a fragile Canvas-relative transform;
- do not move or refactor it during V1;
- collider separation is a later independent cleanup only after the text feature is proven.

Agreed state model:

```text
PlayerData
= teacher's personal saved entrance title + body text
= persists across visits / future instances

manually synced currentEntranceTitle + currentEntranceText
= current title + body for this running VRChat instance
= remains while the instance lives
```

Lifecycle:
- authorized teacher/host initializes a fresh instance from restored PlayerData or default;
- teacher may edit repeatedly during the same session;
- typing stays local;
- Apply / Save persists the teacher's title + body and publishes both current-instance values once;
- late joiners receive the newest synced instance title + body;
- if the original host leaves, the current instance title + body remain unchanged;
- when the VRChat instance ends, its synced state naturally disappears;
- a future new instance can be initialized from that teacher's latest PlayerData values.

Authority rules:
- do not use `isMaster` as teacher identity;
- use `VipAccessManager` only as teacher/VIP authorization input;
- prefer `Networking.IsInstanceOwner` for automatic creator/host recognition where supported;
- keep product authority separate from network-object ownership;
- Group/Public/Build & Test may require a small explicit teacher-only Claim/Start fallback;
- do not overbuild takeover logic for V1.

Agreed minimal architecture:
- `EntranceTextManager.cs` = personal title/body PlayerData + synced current-instance title/body + initialization + validation + late join;
- `EntranceTextEditorUI.cs` = local title/body TMP drafts + Apply/Save + Reset + status + minimal Claim/Start where needed.

Validation:
- title = 64 characters / one normalized line;
- body = 400 characters / 9 explicit lines;
- plain text;
- Reset loads both defaults into the local drafts and still requires Apply / Save.

V1 is now implemented and saved in the real Unity project.

Reported implementation:
- `Assets/!StefanieInVR/Scripts/Managers/EntranceTextManager.cs`;
- `Assets/!StefanieInVR/Scripts/Managers/EntranceTextEditorUI.cs`;
- required UdonSharp assets;
- scene updated at `Assets/#Classroom/Scenes/Classroom.unity`;
- added `UIs/Managers/Entrance Text Manager`;
- added `VipContentRoot/Entrance Text Editor`;
- editor includes TMP input, Apply/Save, Load Default, Claim/Start and status text;
- `Text (Alinea)`, `VipAccessManager`, editor references and button events are wired;
- validation = 400 characters / max 9 lines / whitespace falls back to default.

Reported local proof:
- UdonSharp compiles without errors;
- Play Mode smoke test completed without errors;
- existing Open behaviour verified: local welcome Canvas and collider both deactivate;
- `Collider_Entrance`, Open button, Presentation, VideoTXL and unrelated systems were not changed;
- Unity scene reported saved/clean after implementation.

Evidence update — 2026-09-09 real VRChat build:
- Stef performed a real VRChat build/test after implementation;
- for as far as she could verify in that test, the entrance body text persisted after leaving/returning;
- treat this as **positive single-user persistence evidence**, not full multiplayer acceptance;
- shared visibility to another player has not yet been tested;
- late join, host-departure shared-state survival, Group/Public Claim/Start and two-client synchronization are still open;
- do not call Entrance Text V1 fully beta-proven until those specific real-client tests pass.

V1.1 title + body extension implemented on 2026-09-09:
- preserved the existing `Text (Kop)` title display, V1 body implementation and authority model;
- added a separate single-line title draft input inside the existing editor;
- kept the reduced editor root at `20 x 11.8` with scale about `0.796` and split the existing top row so the body input retained its height;
- title validation = maximum 64 characters / one normalized line / plain text / whitespace falls back to default;
- preserved the original body PlayerData key `open_classroom.entrance_text.v1` for backward compatibility;
- added title PlayerData key `open_classroom.entrance_title.v1`;
- added manually synced current-instance title state alongside the existing body state;
- Apply / Save validates, persists and publishes title + body together;
- Load Default changes both local drafts and still requires Apply / Save;
- visible title now auto-sizes between 18 and 25, stays on one line and uses ellipsis if needed;
- visible body now auto-sizes between 11 and 16, wraps, and uses ellipsis if its fixed area is exceeded;
- UdonSharp program assets report no assembly errors and the saved scene contains exactly one title input with the intended manager/editor references;
- no new Play Mode or real VRChat title/multiplayer acceptance run was performed in this V1.1 pass.

Durable V1.1 implementation and acceptance boundary:

`ENTRANCE_TEXT_V1_1_TITLE_PLAN_2026-09-09.md`

## FINAL FEATURE 2 — PERSISTENT SYNCHRONIZED MOVABLE POSTER

Implemented in the real Unity project on 2026-09-10.

Current architecture:

```text
PlayerData
= local user's saved URL text history + position + rotation + uniform scale

manually synced VRCUrl + uniform scale
+ VRCObjectSync transform
= current poster truth for this running instance
```

Created runtime components:
- `PersistentPoster.cs` = VRChat image download, aspect-ratio fit, pickup authorization and placement handoff;
- `PersistentPosterManager.cs` = PlayerData, synchronized instance URL/scale, initialization, reset and lock enforcement;
- `PersistentPosterEditorUI.cs` = direct URL input, Load/Apply, reset draft, scale preview and release-only publish;
- `Persistent Poster` scene object = kinematic Rigidbody + VRCPickup + VRCObjectSync + frame/image/placeholder;
- `Persistent Poster Manager` scene object;
- inactive `Panel (Poster)` under the existing tablet `Panels` container, ready to be attached to the planned separate welcome/poster tab without squeezing the current VIP layout.

Authority:
- tablet unlocked: everyone may load, resize and move the poster;
- tablet locked: only a locally verified VIP may change or move it;
- temporary object ownership is used only for synchronization and never grants product permission by itself.

Scale behaviour:
- range `0.4x` to `2.0x`;
- dragging previews locally;
- only PointerUp/release requests synchronization;
- minimum one-second publish interval;
- if several releases happen during the interval, the latest value wins.

Reset behaviour:
- Reset only loads the empty/default URL, `1.0x` scale and default placement as a local draft;
- Load/Apply is deliberately required before personal/shared state changes.

Image loading:
- uses `VRCImageDownloader` and a real `VRCUrlInputField`;
- each client downloads the synchronized direct image URL locally;
- the previous successful image remains if a replacement URL fails;
- empty URL shows the placeholder;
- the visible surface and pickup frame preserve the downloaded source aspect ratio.

Known VRChat platform boundary:
- PlayerData can store the URL as a normal string, but Udon cannot construct a new `VRCUrl` from that restored string and cannot write arbitrary text into `VRCUrlInputField`;
- therefore a direct URL synchronizes correctly inside the running instance and reaches late joiners through the synced `VRCUrl`, but the host must paste it once again when establishing a future new instance;
- position, rotation and uniform scale do restore normally from PlayerData;
- do not describe the future-session URL as automatically restorable unless VRChat exposes a supported string-to-`VRCUrl` path later.

Editor validation completed:
- Unity C# compile: PASS;
- all three UdonSharp program assets: Current Version / no assembly errors;
- scene references, VRCUrlInputField, buttons, PointerUp event, VRCPickup, VRCObjectSync and PlayerData fields: wired;
- Play Mode smoke: PlayerData restore reached, instance initialized, poster pickup enabled while unlocked, empty URL placeholder active;
- scale smoke: `1.2x` drag changed local preview while synchronized value stayed `1.0x`; release published `1.2x`;
- cooldown smoke: releases at `1.3x` then `1.4x` kept `1.2x` during the interval and published the latest `1.4x` afterward;
- reset smoke: Reset previewed `1.0x` without changing synchronized `1.4x`; Apply then published `1.0x` and restored the default pose;
- scene saved clean after leaving Play Mode.

Not yet proven:
- real VRChat remote image download, including domain permissions and invalid URL feedback;
- two-client movement/rotation/scale/image agreement;
- late join;
- owner/host departure while the instance remains alive;
- unlocked non-VIP editing and locked VIP-only editing with real accounts/controllers;
- Quest behaviour.

Durable implementation detail and acceptance checklist:

`PERSISTENT_POSTER_IMPLEMENTATION_2026-09-10.md`

Hard boundary:
- do NOT convert or rewrite the working local table screens;
- do NOT couple poster to VideoTXL or Presentation without evidence;
- do NOT refactor e-reader to make the poster;
- smallest isolated manager/component family only.

## CROSS-PROJECT STATE

```text
Presentation Service = LIVE
Open Classroom Presentation = WORKING / BETA-READY
Open Classroom overall = VERY CLOSE TO COMPLETE
Reusable Presentation architecture = PROVEN IN CLASSROOM
Sellable Presentation prefab = productization later
Art House Cinema Presentation integration = NOT YET DONE
```

Hosted-service truth:
`mailfromstefanie/StefanieInVR-Presentation-Service`

Cinema return route:
`mailfromstefanie/Stefanies-Art-House-Cinema`

## WORKING STYLE WITH STEF

- Dutch;
- beginner-friendly;
- one small technical action at a time;
- inspect before changing;
- backup-first at meaningful risk gates;
- do not ceremonially retest solved systems;
- complete scripts, never fragments;
- protect working systems from adjacent feature work;
- Codex is implementation/debugging worker;
- Nova/ChatGPT is orchestrator/project memory keeper;
- update GitHub after meaningful proof or project transitions.

## GITHUB TRUTH RULE

GitHub is durable project memory.

The tested Unity scene may still be newer than copied source/reference files.

Never overwrite known working real-scene truth with older planning assumptions.

