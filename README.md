# Open Classroom

Open Classroom is a VRChat teaching/presentation world and the implementation authority for Classroom-specific Unity systems.

## Normal Stef / Nova entrypoint

For a normal new Nova/ChatGPT conversation across the StefanieInVR ecosystem, use:

`mailfromstefanie/StefanieInVR-Project-Hub/STARTPROMPT.txt`

The Hub routes to this repository when Classroom truth is needed. It never overrides the real Unity scene or this repository's current-work evidence.

For a deliberately Classroom-only / Codex specialist session, use:

`START_PROMPT.md`

## Current routing

Read in this order for current Classroom work:

1. `START_PROMPT.md`
2. `AGENTS.md`
3. `CURRENT_WORK.md`
4. `RECOVERY_DECISION_2026-09-10.md`
5. only the additional feature/history files named by the current task

Useful durable references when relevant:

- `PRESENTATION_ACCEPTANCE_2026-09-05.md` — proven Presentation acceptance;
- `WORKLOG_2026-09-08.md` — stabilization history and PlayerData/Marker work;
- `EREADER_LIBRARY_HANDOFF_2026-09-05.md` — e-reader/library architecture;
- `PERFORMANCE_AUDIT_2026-09-05.md` — performance findings before related optimization;
- Entrance Text / Poster implementation documents — research/history only unless explicitly reactivated after recovery.

## Current status — 2026-09-10

**CONTROLLED RECOVERY.**

The later editable-title / Persistent Poster / Content-tab scene is rejected as a working baseline after Stef inspected the real Unity project and found the tablet/canvas layout and button behaviour disturbed.

Do not fix forward from that broken scene.

The selected rollback backup is expected to retain:

- working persistent editable entrance welcome/body text;
- proven Presentation integration;
- exact protected VideoTXL 2.5.1;
- e-reader/library baseline;
- reset systems;
- earlier working tablet/canvas layout and controls.

It predates:

- editable entrance title;
- Persistent Poster runtime/scene integration;
- `Panel (Content)` / index-8 integration.

The restored backup becomes the accepted baseline only after Stef restores it and visually/functionally approves the real Unity scene.

Real Unity project:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

## Protected proven Presentation baseline

The later Classroom UI rollback does **not** invalidate the earlier Presentation acceptance.

Proven/reported working in the tested setup:

- standalone Presentation Core independent of VideoTXL;
- own `VRCUnityVideoPlayer`;
- 10 slots;
- First / Previous / Next;
- automatic slide count;
- real two-client synchronization;
- cross-client slide control;
- OFF/ON resumes the same slot/slide;
- late join works;
- VideoTXL 2.5.1 local suspend/restore;
- existing physical projector/screen path;
- projector visibility;
- brightness/contrast;
- final physical-screen output;
- Presentation UI integrated into the physical tablet.

A separate formal Quest-headset PASS is not documented.

Exact proof:

`PRESENTATION_ACCEPTANCE_2026-09-05.md`

## Recovery boundary

Until the selected backup is restored and accepted:

- add no new Classroom feature;
- do not rebuild the rejected Content tab automatically;
- do not salvage scene objects from the broken later scene into the backup;
- do not redesign Presentation, VideoTXL, e-reader/library, resets or unrelated systems;
- compile/Play Mode success alone is not visual acceptance.

After Stef accepts the restored scene, the agreed direction is manual/incremental UI rebuilding with one small change and one immediate visual test at a time.

## Source-of-truth rule

```text
real restored + Stef-approved Unity scene
→ CURRENT_WORK.md / recovery decision
→ active feature evidence
→ durable accepted references
→ historical attempted implementation docs
```

GitHub is durable project memory, but copied repository scene/script snapshots never outrank newer tested real Unity behaviour.
