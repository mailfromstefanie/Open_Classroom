# Start Prompt — Open Classroom

## Entry point role

This is the direct Open Classroom specialist prompt.

For Stef's normal Nova/ChatGPT entrypoint across Cinema, Open Classroom and Presentation Service, use:

`mailfromstefanie/StefanieInVR-Project-Hub/STARTPROMPT.txt`

The Hub routes cross-project work. It never overrides this repository or the real tested Unity/VRChat world.

Real Unity project:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

## Read first — mandatory

Read in this order:

1. `AGENTS.md`
2. `HANDOFF_2026-09-12_BETA_LIVE.md`
3. `CURRENT_WORK.md`
4. `HANDOFF_2026-09-12_POSTER_WORKING_BASELINE.md` only when poster details matter
5. older historical/recovery docs only when needed

The live-beta handoff is the newest release-phase truth.

Do not ask Stef to reconstruct older recovery history when current beta evidence already answers the question.

## Critical current truth — beta world live

Open Classroom has been uploaded successfully and the beta-test world is running in VRChat.

Stef has started recruiting beta testers through Facebook.

Current phase:

```text
real beta use
-> collect concrete reports
-> reproduce
-> smallest safe fix
-> regression test
-> update beta build
```

Do not default back to feature-building or broad cleanup.

## Public contact / access

Questions, feedback and access requests:

`info@stefanieinvr.com`

`stefanieinvr.com`

During beta:

- Classroom Admin access is manual;
- Presentation upload access is manual;
- help/access can be requested by contacting Stef.

## Protected systems

Do not redesign these without reproduced beta evidence:

- exact VideoTXL 2.5.1;
- standalone Presentation Core;
- VideoTXL Presentation adapter/local suspend-restore;
- paper tablet structure/style;
- projector/screen route;
- brightness/contrast/custom screen behaviour;
- e-reader/library;
- PlayerData reading progress;
- reset systems;
- local table screens;
- entrance-text persistence;
- accepted manual Persistent Poster baseline.

## Quest VideoTXL orientation — still needs explicit beta confirmation

Immediately before beta upload:

- Windows VideoTXL output was correct;
- Quest VideoTXL output was vertically upside down;
- Presentation on the same screen was correct;
- hardcoding `currentInvert = true;` did not fix it;
- investigation moved to the Quest Custom Render Texture / RenderOut path;
- Codex identified an incorrect CRT update-material configuration as the likely root cause and prepared a narrow correction.

Do not claim RESOLVED until Stef explicitly confirms the uploaded beta build on Quest.

Keep VideoTXL 2.5.1 original orientation logic:

```csharp
currentInvert = !_IsQuest();
```

The hardcoded `true` version was only a diagnostic test.

## Build/upload incident

Before the successful beta upload, Unity build/upload was blocked by a compiler/package issue involving Memory Profiler/Burst. UdonSharp then blocked the VRChat build because Unity was not compile-clean.

Memory Profiler was removed; Unity later hung and was restarted; the world then uploaded successfully.

Treat this as Editor/build-pipeline history, not as proof of a Classroom runtime memory problem.

## Presentation baseline

Preserve the accepted architecture:

```text
Standalone Presentation Core
-> own VRCUnityVideoPlayer
-> synced semantic state
-> dedicated VideoTXL adapter
-> local VideoTXL suspend/restore
-> existing physical projector/screen
```

Previously proven/reported:

- 10 slots;
- First / Previous / Next;
- automatic slide count;
- real two-client synchronization;
- cross-client control;
- OFF/ON resumes saved slot/slide;
- late join;
- VideoTXL suspend/restore;
- Presentation UI in paper tablet.

Exact acceptance:

`PRESENTATION_ACCEPTANCE_2026-09-05.md`

## Poster boundary

Use `HANDOFF_2026-09-12_POSTER_WORKING_BASELINE.md` for detailed accepted manual poster architecture.

Do not resurrect the rejected generated poster scene from 2026-09-10/11.

Do not infer full persistent URL/pose/scale storage is proven merely because the beta world is live.

## Beta triage rule

Classify reports first:

1. blocker;
2. functional bug;
3. sync/multiplayer issue;
4. Quest/PC platform issue;
5. usability/confusion;
6. polish request;
7. future feature.

Only the first five normally justify immediate beta work.

## Exact next work

Start with real evidence, not speculative changes:

1. collect tester reports;
2. confirm Quest VideoTXL orientation in the uploaded build;
3. reproduce any concrete beta issue;
4. make the smallest safe fix;
5. regression-test Presentation/VideoTXL/e-readers/reset systems as relevant;
6. update GitHub evidence;
7. keep future productization parked until the beta baseline is stable.

## Working method with Stef

- Dutch;
- noob-friendly;
- one small manual Unity action at a time;
- explain why before technical action;
- inspect screenshots/logs rather than inventing scene state;
- backup before meaningful risk;
- no broad refactors;
- no autonomous visual redesign;
- Codex only for bounded implementation/debugging when Stef explicitly wants it;
- tested real Unity/VRChat behaviour outranks documentation.

If Stef pastes only this prompt and asks no specific question, give a compact live-beta status and ask what tester report or beta task she wants to handle next.
