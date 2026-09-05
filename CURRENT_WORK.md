# Current Work — Open Classroom

Last updated: 2026-09-05 (Europe/Amsterdam)

## CURRENT STATUS

**Presentation prefab / Classroom integration is designed and ready to resume, but temporarily parked while Stef finishes website baseline work, the local Website Editor, and then eReader/eBook design.**

This is a scheduling pause only. The Presentation architecture is not cancelled or reset.

## PRESERVED PRESENTATION ARCHITECTURE

First proof:

```text
10 stable Presentation Slot MP4s
-> dedicated VideoTXL Playlist source
-> PresentationController
-> existing VideoTXL SyncPlayer
-> existing projector/screen
-> one slide = one second
-> Previous / Next seek by one second
```

Do not create a second video player unless the existing VideoTXL route fails in real testing.

Polished Presentation Mode must also support:

```text
normal video
-> enter Presentation Mode
-> remember previous source/time/play-pause state
-> presentation takes over same player/screen
-> Back to Video
-> restore previous source/time/play-pause state
```

Read `PRESENTATION_INTEGRATION_PLAN.md` before implementation.

## EXACT FIRST ACTION WHEN PRESENTATION WORK RESUMES

Use the real Unity project:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

Then:

1. confirm Unity is out of Play Mode;
2. create `Assets/StefanieInVR/Presentation`;
3. create empty hierarchy root `PresentationSystem`;
4. add the first complete `PresentationController.cs`;
5. wire existing VideoTXL references one at a time;
6. test Slot 1 demo first.

Stef receives complete scripts and microsteps.

## EXISTING VIDEOTXL / PLAYLIST FOUNDATION — PROVEN

The previous multiplayer blocker is cleared.

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

Real VRChat multiplayer proof established:

- StefanieInVR sees the owner-only stream-selection button;
- VIP non-owner does not;
- ordinary users cannot reach that owner-only button;
- synchronized playback reaches users;
- permitted playlist switching works;
- VideoTXL UI remains usable.

Do not reintroduce dynamic `VideoSourceUI/Content` hiding for this access requirement.

Durable technical reference:

`VIDEOTXL_2_5_1_FINDINGS.md`

## ACTIVE UNITY PROJECT

Use:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

Do not confuse with the older local checkout:

`E:/GitHub/Open_Classroom`

## CURRENT CROSS-PROJECT SCHEDULE

Before Classroom Presentation implementation resumes, Stef is intentionally doing:

1. public StefanieInVR website + translation cleanup;
2. local Windows StefanieInVR Website Editor;
3. eReader/eBook design.

Those tasks are separate from Open Classroom implementation.

## FUTURE IDEAS — PARKED

Online JSON/content-manager work remains future research.

Do not build it merely because VideoTXL runtime PlaylistData loading is technically plausible.

## WORKING RULE

```text
inspect
-> one small permanent-oriented change
-> test
-> record proof
-> continue
```

For non-trivial code changes, always give Stef the complete replacement script.

## 2026-09-05 TODAY — Presentation integration reactivated

The Presentation prefab / Classroom integration is **ACTIVE TODAY**.

Stef will build it manually in the real Unity project with normal ChatGPT guidance. Codex is not the Unity executor for this work block.

Use the existing accepted architecture in `PRESENTATION_INTEGRATION_PLAN.md`.

Exact working style:

```text
ChatGPT explains one small Unity action
-> Stef performs it
-> ChatGPT provides complete script files when needed
-> Stef wires references
-> test
-> continue
```

Do not restart architecture planning. Do not create a second video player unless the accepted VideoTXL route fails in real testing.
