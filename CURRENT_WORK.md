# Current Work — Open Classroom

Last updated: 2026-09-09 Europe/Amsterdam

## AUTHORITATIVE CURRENT STATUS

**Open Classroom is now functionally very close to complete.**

Protected working baseline:
- Presentation system = working / beta-ready;
- VideoTXL 2.5.1 integration = working and must remain pinned;
- VRCDN livestream route = investigated with real VRChat logs / Build & Run, not ClientSim;
- e-reader/library = substantially refined and now includes per-user PlayerData persistence for reading progress;
- five Marker Pro objects = dedicated shared reset points added;
- fresh offline and OneDrive backups exist after the latest stabilization work.

The only currently planned Classroom feature additions are:
1. persistent synchronized entrance text — V1 implemented and saved; local compile/smoke checks pass; real VRChat multiplayer/persistence acceptance still open;
2. one persistent synchronized movable poster with a persistent image URL.

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

First action:

```text
RUN ONE NARROW REAL MULTIPLAYER ACCEPTANCE PASS
```

Check:
1. e-reader PlayerData last-page persistence;
2. both physical e-readers share the same user's stored progress;
3. Marker Pro global reset is seen correctly by both users;
4. optional late join if useful.

If this passes:
- record the result as the golden Classroom baseline;
- keep the offline + OneDrive backups;
- then begin the two final customization features.

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
= teacher's personal saved entrance text
= persists across visits / future instances

manually synced currentEntranceText
= current text for this running VRChat instance
= remains while the instance lives
```

Lifecycle:
- authorized teacher/host initializes a fresh instance from restored PlayerData or default;
- teacher may edit repeatedly during the same session;
- typing stays local;
- Apply / Save persists the teacher value and publishes the current instance value once;
- late joiners receive the newest synced instance text;
- if the original host leaves, the current instance text remains unchanged;
- when the VRChat instance ends, its synced state naturally disappears;
- a future new instance can be initialized from that teacher's latest PlayerData value.

Authority rules:
- do not use `isMaster` as teacher identity;
- use `VipAccessManager` only as teacher/VIP authorization input;
- prefer `Networking.IsInstanceOwner` for automatic creator/host recognition where supported;
- keep product authority separate from network-object ownership;
- Group/Public/Build & Test may require a small explicit teacher-only Claim/Start fallback;
- do not overbuild takeover logic for V1.

Agreed minimal architecture:
- `EntranceTextManager.cs` = PlayerData + synced instance text + initialization + validation + late join;
- `EntranceTextEditorUI.cs` = local TMP input draft + Apply/Save + Reset + status + minimal Claim/Start where needed.

Initial validation target:
- about 400 characters;
- about 9 explicit lines;
- plain text;
- Reset loads default into the draft and still requires Apply / Save.

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

Evidence boundary:
- real VRChat multiplayer, late join, host-departure persistence, Group/Public Claim/Start and PlayerData-across-instances are **not yet accepted**;
- do not call Entrance Text V1 beta-proven until those real-client tests pass.

## FINAL FEATURE 2 — PERSISTENT SYNCHRONIZED MOVABLE POSTER

Desired UX:
- one poster object;
- teacher pastes a direct image URL (own server or supported host);
- poster displays that image;
- pickup interaction should feel like the current e-reader;
- can be placed anywhere;
- one simple slider changes uniform scale;
- position/rotation/scale are synchronized to the instance;
- late joiners see the current poster;
- teacher's configuration persists for future sessions.

Preferred persistent fields:
- image URL;
- position;
- rotation;
- uniform scale.

Preferred separation:

```text
PlayerData
= teacher's saved poster configuration

synced runtime state / ownership
= poster currently active in this instance
```

Reuse only as references:
- e-reader = pickup feel + PlayerData pattern;
- local table screens = UI visual language + scale concept only;
- Marker/reset infrastructure = reset/ownership reference where appropriate.

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

