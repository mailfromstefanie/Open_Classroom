# Start Prompt — Open Classroom

## Entry point role

This is the direct Open Classroom specialist prompt.

For Stef's normal Nova/ChatGPT entrypoint across Cinema, Open Classroom and Presentation Service, use:

`mailfromstefanie/StefanieInVR-Project-Hub/STARTPROMPT.txt`

The Hub routes cross-project work. It never overrides this repository or the real Stef-approved Unity scene.

Real Unity project:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

## Read first — mandatory

Read in this order:

1. `AGENTS.md`
2. `HANDOFF_2026-09-12_POSTER_WORKING_BASELINE.md`
3. `CURRENT_WORK.md`
4. only then older historical/recovery docs if needed

The 2026-09-12 handoff is the newest session truth and overrides older poster claims where they conflict.

Historical files that are NOT current poster truth:
- `HANDOFF_2026-09-11_POSTER_REPAIR.md` = failed generated-poster state and rebuild decision;
- `PERSISTENT_POSTER_IMPLEMENTATION_2026-09-10.md` = rejected older generated implementation;
- `RECOVERY_DECISION_2026-09-10.md` = rollback/recovery safety context.

Do not ask Stef to choose between these documents. Route to the newest truth automatically.

## Critical current truth — 2026-09-12

Open Classroom is on a usable manual-development baseline.

Protected systems:
- Presentation Core/integration is proven and must not be rebuilt;
- VideoTXL stays pinned to exact 2.5.1;
- Entrance title/body persistence is a useful proven pattern;
- e-reader/library and PlayerData reading progress are preserved;
- local table screens and reset systems are protected.

Active task:

**finish acceptance of the manually rebuilt Persistent Poster, then add persistence as a separate layer.**

## Current poster architecture

```text
Persistent_Poster                 <- physical pickup root
├─ existing VRCObjectSync         <- position + rotation
├─ VRC Pickup / Rigidbody
├─ PosterImageLoaderLocal
├─ Scale_Root
│  ├─ Persistent_Poster           <- square image surface
│  └─ Persistent_Poster_Edge      <- visual only
├─ Persistent_Poster_Handle       <- pickup outline renderer
└─ Poster Grip                    <- Exact Grip

Managers
├─ Poster Shared State            <- URL / hasPoster shared state
└─ Poster Scale Manager           <- scale shared state
```

Rules:
- do not add a second VRCObjectSync;
- poster remains square;
- no aspect-ratio automation;
- non-square images may stretch;
- scale is uniform;
- top-root VRCObjectSync remains responsible for physical position/rotation.

## Proven poster evidence

### Image loading

Direct `VRCImageDownloader` loading works.
Use direct final image URLs, for example:

```text
https://i.imgur.com/klSe3ij.jpg
https://i.imgur.com/GhVNVXv.jpeg
https://i.imgur.com/oaEbiM2.png
```

Do not add Imgur-specific logic.

### Multiplayer URL/image sharing

Two-player current-instance URL/image synchronization was explicitly reported working.

The working ownership rule is:

```text
SetOwner if needed
-> wait one frame
-> verify/retry ownership
-> write synced state
-> RequestSerialization
```

Do not regress to immediate SetOwner + serialization in the same frame.

### Scale

Local scale is confirmed working.

Use:

```text
Min Scale      = 1
Max Scale      = 5
Default Scale  = 1
Sync Delay     = 0.35

Slider Min     = 1
Slider Max     = 5
Slider Value   = 1
Whole Numbers  = OFF
```

Meaning: slider `1` = 1x, `5` = 5x.
Local scaling is immediate; network commit waits about 0.35 seconds after movement stops.

## Latest poster changes — NOT YET ACCEPTED

### Emission

There is no emission toggle.

Required behaviour:

```text
no poster -> emission OFF
successful load -> same image in emission map + emission ON
Unload -> image/default restored + emission texture cleared + emission OFF
```

Current expected shader properties:
- `_EmissionMap`;
- `_EmissionColor`.

### Shared unload

Unload button must call:

```text
Poster Shared State
-> UdonBehaviour.SendCustomEvent(string)
-> UnloadPoster
```

Do not wire the UI directly to local `UnloadImage()`.

Latest code references:
- `Scripts/UIManagers/PosterImageLoaderLocal.cs`;
- `Scripts/UIManagers/PosterSharedState.cs`;
- `Scripts/UIManagers/PosterScaleManager.cs`.

## TXLScreenAutoVisibility — regression gate

Recompiling Udon exposed an old custom direct-field problem:

```text
Field is not exposed to Udon: 'txlPlayer.playerState'
```

Do NOT modify VideoTXL itself.

Corrected reference:

`Scripts/UIManagers/TXLScreenAutoVisibility.cs`

It now reads VideoTXL `playerState` and `paused` through `UdonBehaviour.GetProgramVariable(...)`.

It MUST preserve the Presentation switch rule:

```text
projector open
AND
(VideoTXL visible OR presentationController.modeActive)
```

An old duplicate script copy under the obsolete `Assets/#Classroom/...Only_Visible_If_Blendshape_Is_Toggle/` route was deleted by Stef.
The manager-side script remains and must not be deleted.

Test before calling this fixed:
1. normal VideoTXL playback;
2. Video -> Presentation;
3. slide navigation;
4. Presentation -> Video;
5. pause/play;
6. projector open/close.

## Persistence — planned next, not implemented

Do NOT start a new session by immediately writing persistence code.

Desired saved poster state:
- URL / hasPoster;
- position;
- rotation;
- scale.

Current design direction:
- keep existing live systems intact;
- add persistence as a separate storage layer;
- investigate/use persistent PlayerObject + `VRCEnablePersistence` for secure URL/state where appropriate;
- keep only ONE physical poster in the scene;
- do not turn the physical pickup into a per-player duplicate.

Likely split:

```text
LIVE INSTANCE
PosterSharedState   -> URL + hasPoster
VRCObjectSync       -> position + rotation
PosterScaleManager  -> scale

PERSISTENCE STORAGE
persistent PlayerObject
-> saved URL/hasPoster
-> saved position
-> saved rotation
-> saved scale
```

Persistence is user-bound unless an external backend is deliberately introduced later.

## Exact next session

Stef made a fresh backup before testing further.

Start here:

1. confirm Unity/Udon compile clean;
2. test VideoTXL <-> Presentation switching;
3. local poster Load -> emission ON -> Unload -> emission OFF;
4. two-player Load/Unload + scale + movement/reset + late join where practical;
5. record results;
6. only then implement persistence.

## Working method with Stef

- Dutch;
- noob-friendly;
- one small manual Unity action at a time;
- explain why before technical action;
- inspect screenshots rather than inventing scene state;
- complete scripts when replacement is needed;
- backup before meaningful risk;
- no broad refactors;
- no autonomous visual redesign;
- Codex only for small bounded work when Stef explicitly wants it;
- tested real Unity/VRChat behaviour outranks documentation.

If Stef pastes only this prompt and asks no specific question, give a compact status and ask whether she wants to continue exactly where the test gate stopped or do something else.
