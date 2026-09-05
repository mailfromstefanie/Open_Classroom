# Open Classroom

An open classroom system for VRChat.

## Resume work

Read in this order:

1. `START_PROMPT.md`
2. `AGENTS.md`
3. `CURRENT_WORK.md`
4. `EREADER_LIBRARY_HANDOFF_2026-09-05.md` when e-reader/library work matters
5. `PERFORMANCE_AUDIT_2026-09-05.md` before performance changes
6. `PRESENTATION_ACCEPTANCE_2026-09-05.md`
7. exact feature/architecture files only when needed

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

Stef made a fresh full backup of the complete Unity project folder after the final screen-size fix and successful late-join proof.


## E-reader/library current status

A first multi-book Classroom e-reader implementation now works in ClientSim:

- one local shared e-reader video manager;
- one VRCUnityVideoPlayer;
- one shared RenderTexture;
- multiple lightweight physical books;
- kinematic Rigidbody + VRCObjectSync;
- local page/bookmark/Keep Open state;
- last-touched-wins;
- inactive screens/player off;
- compact page controls;
- existing reset/toggle systems reused.

Still open:
- real headset left/right-hand comfort;
- real two-player pickup + independent reading proof;
- Quest profiling.

Durable handoff:

`EREADER_LIBRARY_HANDOFF_2026-09-05.md`

Performance findings:

`PERFORMANCE_AUDIT_2026-09-05.md`

### Exact next session rule

Before any performance optimization, Stef will first make a **fresh full backup of the current Unity project including the new e-reader implementation**.

Do not optimize before that backup exists.
