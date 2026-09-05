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

## Current next phase — 2026-09-05

The previous Classroom Presentation integration has been dismantled.

The active direction is now:

```text
standalone Presentation Core
-> one own VRCUnityVideoPlayer
-> own rollout screen by default
-> sync only mode + slot + slide + revision
-> local seek/pause on every client
-> optional VideoTXL integration for Stef's Classroom
```

Do not treat the current Classroom as already beta-ready with the Presentation Service.

Do not start by rebuilding the older VideoTXL Presentation Playlist design; that architecture is superseded for the reusable product.

Research material for the standalone player belongs in:

`docs/research/standalone-presentation-player/`
