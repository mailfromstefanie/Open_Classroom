# Open Classroom

An open classroom system for VRChat.

## Resume work

Read in this order:

1. `START_PROMPT.md`
2. `AGENTS.md`
3. `CURRENT_WORK.md`
4. `PRESENTATION_ARCHITECTURE_DECISION.md`
5. `PRESENTATION_INTEGRATION_PLAN.md`
6. only the exact scripts/files relevant to the current task

The repository is project memory and backup/reference evidence from the real Unity scene. Do not assume GitHub scene state is newer than Stef's tested Unity scene unless `CURRENT_WORK.md` says so.

## Current active phase — 2026-09-05

The standalone Presentation Core is now implemented in the real Unity project and the Open Classroom VideoTXL 2.5.1 adapter is substantially working in ClientSim.

Current order:

```text
fix Presentation physical-screen fill/aspect
-> real multiplayer sync
-> late join / early-start
-> Quest
-> reusable prefab hardening
-> later Cinema integration
```

Preserve:

- Core independence from VideoTXL;
- sync only mode + slot + slide + revision;
- local load/seek/pause per client;
- existing projector visibility;
- existing custom screen shader;
- existing brightness/contrast controls.

The old VideoTXL Presentation Playlist design remains superseded.

For exact current truth read `CURRENT_WORK.md`.

Detailed latest handoff:

`SESSION_HANDOFF_2026-09-05_PRESENTATION.md`

Research material remains under:

`docs/research/standalone-presentation-player/`
