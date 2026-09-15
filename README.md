# Open Classroom

Open Classroom is a VRChat teaching/presentation world and the implementation authority for Classroom-specific Unity systems.

## Normal Stef / Nova entrypoint

For a normal new Nova/ChatGPT conversation across the StefanieInVR ecosystem, use:

`mailfromstefanie/StefanieInVR-Project-Hub/STARTPROMPT.txt`

For a deliberately Classroom-only / Codex specialist session, use:

`START_PROMPT.md`

## Current routing

Read in this order for current Classroom work:

1. `START_PROMPT.md`
2. `AGENTS.md`
3. `CURRENT_WORK.md`
4. current feature-specific handoff/spec named there
5. release/history references only when needed

Useful durable references:

- [EREADER_BOOK_A_WORK.md](EREADER_BOOK_A_WORK.md) — current Book_A implementation, evidence and next mobile gate;
- [EREADER_MOBILE_FOCUS_VIEW_RESEARCH_2026-09-16.md](EREADER_MOBILE_FOCUS_VIEW_RESEARCH_2026-09-16.md) — native mobile research, not implementation;
- `EREADER_V1_PRODUCT_SPEC_2026-09-14.md` — wider accepted product design;
- `EREADER_V1_OPEN_CLOSE_DECISION_2026-09-15.md` — accepted target behaviour, including the still-open VR final-drop gap;
- `EREADER_LIBRARY_HANDOFF_2026-09-05.md` — earlier working e-reader/library baseline;
- `HANDOFF_2026-09-13_PERSISTENCE_BUGS.md` — parked but unresolved Entrance Text / Persistent Poster persistence bugs;
- `HANDOFF_2026-09-12_BETA_LIVE.md` — live beta/release-phase truth;
- `HANDOFF_2026-09-12_POSTER_WORKING_BASELINE.md` — accepted manual poster baseline;
- `PRESENTATION_ACCEPTANCE_2026-09-05.md` — proven Presentation acceptance;
- `PERFORMANCE_AUDIT_2026-09-05.md` — performance findings;
- `WORKLOG_2026-09-08.md` — stabilization history;
- older recovery/poster documents — historical evidence only when they do not conflict with current beta truth.

## Current status — 2026-09-16

**BETA WORLD LIVE.**

The current Open Classroom beta remains live.

Stef has deliberately selected a bounded EReader V1 foundation/productization block.

Book_A is implemented locally and compile checked. Stef reports PC good and previously accepted the VR two-handle baseline. Mobile is not accepted: swiping also moves the avatar, layout wastes page space and pinch is absent.

Next proposed gate: a small isolated native mobile Focus View proof, not yet built or authorized. See the work handoff for scope. The accepted V1 final-drop-stays-open rule is not yet implemented in the current Pin-dependent VR drop path.

This is a documentation synchronization, not a Unity project backup, source-code synchronization or new public world release.

The previously active Entrance Text / Persistent Poster persistence bugs are parked, not fixed. See `HANDOFF_2026-09-13_PERSISTENCE_BUGS.md` when returning to them.

## EReader V1 boundary

Accepted main rule:

```text
reading / page / bookmarks / reader screen = LOCAL PER PLAYER
```

Physical movement may optionally use `VRCObjectSync`.
Hide/Show and Reset/Home are separate physical/control responsibilities.
Paper Tablet integration is optional input/adaptation only; the EReader is being designed as a standalone reusable prefab family.

Cinema `StefanieInVR.HandheldUI` is a proven source reference for the left/right handle method. Open Classroom remains the target implementation/acceptance authority.

Canonical design:

`EREADER_V1_PRODUCT_SPEC_2026-09-14.md`

## Future EReader converter direction

A future website converter is planned for PDF/EPUB -> VR-reader media, with preview and either hosted URL or downloadable MP4/self-hosting.

This is planned product direction only and does not mean the live Presentation Service has been changed.

## Public beta contact / access

Questions, feedback and access requests:

- `info@stefanieinvr.com`
- `stefanieinvr.com`

During beta, Classroom Admin access and Presentation upload access are granted manually.

## Protected Presentation baseline

Proven/reported working before the beta launch:

- standalone Presentation Core independent of VideoTXL;
- own `VRCUnityVideoPlayer`;
- 10 slots;
- First / Previous / Next;
- automatic slide count;
- real two-client synchronization;
- cross-client slide control;
- OFF/ON resumes the same slot/slide;
- late join;
- VideoTXL 2.5.1 local suspend/restore;
- existing physical projector/screen path;
- projector visibility;
- brightness/contrast;
- final physical-screen output;
- Presentation UI integrated into the paper tablet.

Exact proof:

`PRESENTATION_ACCEPTANCE_2026-09-05.md`

VideoTXL remains pinned to exact version 2.5.1.

## Quest VideoTXL note

A Quest-only vertically upside-down VideoTXL-output issue was investigated immediately before the beta upload.

The final suspected cause moved away from the physical screen and into the Quest Custom Render Texture / RenderOut path. A narrow CRT-material correction was prepared.

Do not call that issue fully resolved until Stef confirms the uploaded beta build on Quest.

See:

`HANDOFF_2026-09-12_BETA_LIVE.md`

## Beta source-of-truth rule

```text
real current Unity/VRChat behaviour
-> CURRENT_WORK.md
-> current feature spec/handoff
-> HANDOFF_2026-09-12_BETA_LIVE.md for release truth
-> accepted feature evidence
-> older historical/recovery docs
```

GitHub is durable project memory, but the current real Unity/VRChat behaviour outranks documentation.
