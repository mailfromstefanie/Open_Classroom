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
3. `HANDOFF_2026-09-12_BETA_LIVE.md`
4. `CURRENT_WORK.md`
5. feature-specific references only when needed

Useful durable references:

- `PRESENTATION_ACCEPTANCE_2026-09-05.md` — proven Presentation acceptance;
- `HANDOFF_2026-09-12_POSTER_WORKING_BASELINE.md` — accepted manual poster baseline;
- `EREADER_LIBRARY_HANDOFF_2026-09-05.md` — e-reader/library architecture;
- `PERFORMANCE_AUDIT_2026-09-05.md` — performance findings;
- `WORKLOG_2026-09-08.md` — stabilization history;
- older recovery/poster documents — historical evidence only when they do not conflict with current beta truth.

## Current status — 2026-09-12

**BETA WORLD LIVE.**

The current Open Classroom build has been uploaded successfully and the beta-test world is running in VRChat.

Stef has started recruiting beta testers through Facebook.

Current development mode:

```text
real beta use
-> collect concrete bugs / confusion / UX friction
-> reproduce
-> smallest safe fix
-> regression test
-> update beta build
```

The project is no longer in the earlier controlled-recovery phase.

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
real uploaded/tested VRChat beta behaviour
-> HANDOFF_2026-09-12_BETA_LIVE.md
-> CURRENT_WORK.md
-> accepted feature evidence
-> older historical/recovery docs
```

GitHub is durable project memory, but the current real Unity/VRChat behaviour outranks documentation.
