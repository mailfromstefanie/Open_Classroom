# Open Classroom

An open classroom system for VRChat.

## Resume work

Read in this order:

1. `START_PROMPT.md`
2. `AGENTS.md`
3. `CURRENT_WORK.md`
4. `PRESENTATION_ACCEPTANCE_2026-09-05.md`
5. exact feature/architecture files only when needed

The repository is durable project memory and backup/reference evidence from the real Unity scene.

Do not assume a copied GitHub scene/script snapshot is newer than Stef's tested Unity project.

## Current status — 2026-09-05

**Open Classroom Presentation is working and beta-ready in the tested setup.**

Current proven direction:

```text
standalone Presentation Core
-> own VRCUnityVideoPlayer
-> sync only mode + slot + slide + revision
-> local load/seek/pause per client
-> Open Classroom VideoTXL 2.5.1 adapter
-> existing projector screen
```

Working behavior includes:

- 10 slots;
- slide navigation;
- two-client synchronization;
- resume same slide after Presentation OFF/ON;
- late join;
- VideoTXL suspend/restore;
- projector visibility;
- brightness/contrast;
- final physical-screen output;
- Presentation UI integrated into the tablet.

The old VideoTXL Presentation Playlist architecture remains superseded.

Do not rebuild it.

Detailed acceptance snapshot:

`PRESENTATION_ACCEPTANCE_2026-09-05.md`

Real Unity project:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

Stef also made a full backup of the complete Unity project folder before the final working-state handoff.
