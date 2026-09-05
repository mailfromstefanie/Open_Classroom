# Current Work — Open Classroom

Last updated: 2026-09-05 (Europe/Amsterdam)

## AUTHORITATIVE CURRENT STATUS

**The StefanieInVR Presentation Service is live, but the Presentation integration in the current Open Classroom scene has been dismantled and must now be rebuilt.**

Important current truth from Stef:

- the earlier Classroom presentation setup no longer exists in the current scene;
- do not describe the current Classroom as beta-ready with the Presentation Service;
- the real Unity project remains the source of truth for what is currently present;
- GitHub currently does not contain a committed `PresentationController.cs` implementation.

The next active task is to rebuild the Classroom Presentation integration carefully on top of the existing VideoTXL player/screen foundation, then harden that working result into a **reusable VRChat Presentation prefab**.

## REAL UNITY PROJECT

Use:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

Do not confuse it with the older checkout:

`E:/GitHub/Open_Classroom`

## NEXT ACTIVE GOAL — REUSABLE PRESENTATION PREFAB

Target:

```text
current Open Classroom scene
-> inspect what presentation-related pieces, if any, still remain
-> preserve the existing VideoTXL player/screen foundation
-> rebuild the smallest working Presentation integration
-> prove Slot 1 and slide navigation
-> separate Classroom-specific references from reusable configuration
-> package/harden reusable prefab
-> test in small steps
-> later reuse in Art House Cinema and other worlds
```

The prefab must remain useful for creators who host their own compatible presentation MP4s.

## EXACT FIRST ACTION IN THE NEXT CHAT

First:

1. confirm Unity is **OUT OF PLAY MODE**;
2. inspect the current Classroom hierarchy around the existing VideoTXL player, projector/screen, playlist/source objects and tablet UI;
3. identify what Presentation-specific objects/scripts were removed and what useful foundation remains;
4. change nothing until that inventory is clear;
5. then rebuild the smallest working Presentation path, starting with one Slot before adding navigation or prefab packaging.

## ARCHITECTURE TO PRESERVE UNLESS REAL TESTING DISPROVES IT

The accepted design remains:

```text
Presentation UI / prefab controller
-> existing VideoTXL Playlist/source
-> existing VideoTXL 2.5.1 SyncPlayer
-> existing projector/screen
```

Core rules:

- one slide = one second on the presentation MP4 timeline;
- Previous / Next seek by one second;
- reuse the existing VideoTXL SyncPlayer and screen;
- do not create a second video player by default;
- VideoTXL remains playback/synchronization authority;
- presentation controls request slot/seek/pause actions;
- paused presentation slides remain visible;
- preserve multiplayer/sync semantics already proven in the working scene.

Polished Presentation Mode may later retain/restore the previous normal video source/time/play-pause context, but only after the core rebuilt Presentation flow is proven. Do not invent a second player merely for restoration.

Read `PRESENTATION_INTEGRATION_PLAN.md` for the preserved design contract.

## LIVE HOSTED SERVICE CONTRACT

Cross-project truth from `mailfromstefanie/StefanieInVR-Presentation-Service`:

- ten-slot Presentation Service Free Beta = LIVE;
- hosted input = PDF;
- one slide = one second;
- stable Presentation Slot MP4s;
- current live web uploader uses the proven slot-code flow;
- the prepared Username-only personal dashboard is not live.

## VIDEOTXL FOUNDATION — PROVEN REFERENCE

Accepted responsibility split:

```text
VideoTXL
-> playlist/source objects
-> playback
-> synchronization
-> playlist content UI

VipAccessManager / tablet UI
-> public/VIP access
-> tablet lock
-> stage blocker

TXLPlaylistPrivacyFilter
-> local owner-only selection-button visibility only
```

Real VRChat multiplayer proof previously established:

- owner-only selection visibility works;
- ordinary/VIP users do not get the owner-only button when not allowed;
- synchronized playback reaches users;
- permitted playlist switching works;
- VideoTXL UI remains usable.

Do not reintroduce dynamic `VideoSourceUI/Content` hiding for that access requirement.

Detailed reference:

`VIDEOTXL_2_5_1_FINDINGS.md`

## CURRENT CROSS-PROJECT PRIORITY

Current order:

1. rebuild the Presentation integration in Open Classroom, then harden it into a reusable prefab;
2. once stable, later integrate it into Art House Cinema;
3. Website Editor and eReader/eBook work are parked unless Stef explicitly reprioritizes them.

Cinema remains a separate project. Do not change Cinema implementation while packaging the Classroom prefab unless Stef explicitly asks.

## WORKING STYLE WITH STEF

```text
ChatGPT explains one tiny Unity action
-> Stef performs it
-> inspect result
-> complete script file if needed
-> wire exact references
-> test
-> record meaningful proof
-> continue
```

Rules:

- Dutch, beginner-friendly;
- one small Unity action at a time;
- complete scripts, never partial fragments;
- no architecture restart;
- no destructive scene changes without inspection;
- ClientSim/editor evidence is not final PC/Quest/multiplayer proof.

## GITHUB TRUTH RULE

GitHub is durable project memory, but the real tested Unity scene can be newer than the repository snapshot.

For this prefab phase:

```text
real scene inspection
-> record actual working setup in GitHub
-> then extract/harden reusable prefab
```

Never overwrite working scene truth with an older planning assumption.
