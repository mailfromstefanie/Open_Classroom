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

- `EREADER_V1_PRODUCT_SPEC_2026-09-14.md` — accepted standalone EReader V1 product/design direction and current Book_A foundation gate;
- `EREADER_LIBRARY_HANDOFF_2026-09-05.md` — earlier working e-reader/library baseline;
- `HANDOFF_2026-09-13_PERSISTENCE_BUGS.md` — parked but unresolved Entrance Text / Persistent Poster persistence bugs;
- `HANDOFF_2026-09-12_BETA_LIVE.md` — live beta/release-phase truth;
- `HANDOFF_2026-09-12_POSTER_WORKING_BASELINE.md` — accepted manual poster baseline;
- `PRESENTATION_ACCEPTANCE_2026-09-05.md` — proven Presentation acceptance;
- `PERFORMANCE_AUDIT_2026-09-05.md` — performance findings;
- `WORKLOG_2026-09-08.md` — stabilization history;
- older recovery/poster documents — historical evidence only when they do not conflict with current beta truth.

## Current status — 2026-09-14

**BETA WORLD LIVE.**

The current Open Classroom beta remains live.

Stef has deliberately selected a bounded EReader V1 foundation/productization block.

Current first gate:

```text
Book_A only
-> standalone product boundary
-> left/right ParentConstraint handles
-> only thin handle pickup highlight
-> preserve existing local reader behaviour
-> prove correct first-handle / second-handle / final-drop semantics
```

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
