# Current Work — Open Classroom

Last updated: 2026-09-12 Europe/Amsterdam

## READ THIS FIRST

Newest authoritative handoff:

`HANDOFF_2026-09-12_BETA_LIVE.md`

Then read:

`HANDOFF_2026-09-12_POSTER_WORKING_BASELINE.md`

for the accepted manual poster baseline and detailed poster architecture.

Historical recovery/poster files remain useful evidence only where they do not conflict with the live beta truth.

Real Unity project:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

The real uploaded/tested VRChat world remains the strongest source of truth.

## CURRENT STATUS — BETA WORLD LIVE

**Open Classroom has been uploaded successfully and the beta-test world is now running in VRChat.**

Stef has started recruiting beta testers through Facebook.

The project phase is no longer recovery-first or feature-first. The default phase is now:

```text
real beta use
-> collect concrete bugs / confusion / UX friction
-> reproduce evidence
-> smallest safe fix
-> regression test
-> update beta build
```

Do not broaden scope based only on ideas or speculative cleanup while the beta is running.

## PUBLIC BETA ACCESS / CONTACT

Questions, feedback and access requests:

- `info@stefanieinvr.com`
- `stefanieinvr.com`

During beta:

- Classroom Admin access is granted manually;
- Presentation upload access is granted manually;
- users can contact Stef for help with the Classroom or Presentation workflow.

## PROTECTED WORKING FOUNDATIONS

Preserve unless a reproduced beta bug proves a change is necessary:

- Presentation Core/integration;
- exact VideoTXL 2.5.1;
- VideoTXL local suspend/restore during Presentation;
- physical projector/screen path;
- brightness/contrast/custom screen behaviour;
- paper tablet visual/interaction style;
- e-reader/library baseline;
- PlayerData reading progress;
- Marker/reset systems;
- local table screens;
- persistent entrance-text architecture;
- manual Persistent Poster baseline.

No broad refactor during beta without evidence.

## PRESENTATION — CURRENT RELEASE TRUTH

The accepted Presentation architecture remains:

```text
Standalone Presentation Core
-> own VRCUnityVideoPlayer
-> synced semantic presentation state
-> dedicated VideoTXL adapter
-> VideoTXL local suspend/restore
-> existing physical projector/screen
```

Previously proven/reported includes:

- 10 slots;
- First / Previous / Next;
- automatic slide count;
- real two-client synchronization;
- cross-client slide control;
- OFF/ON same slot/slide restore;
- late join;
- VideoTXL local suspend/restore;
- final physical screen output;
- Presentation UI integrated into the paper tablet.

Canonical acceptance:

`PRESENTATION_ACCEPTANCE_2026-09-05.md`

## QUEST VIDEOTXL ORIENTATION — VERIFY IN LIVE BETA

Immediately before the beta upload, a Quest-only VideoTXL problem was investigated:

- Windows VideoTXL output was correct;
- Quest VideoTXL output was vertically upside down;
- Presentation was correct on the same physical screen;
- direct `currentInvert = true;` testing did not fix the Quest result;
- the investigation moved to the Quest Custom Render Texture / RenderOut path;
- Codex identified an incorrect CRT update-material setup as the likely root cause and prepared a narrow correction.

Do **not** call this issue fully resolved until Stef confirms the uploaded beta build is correct on Quest.

VideoTXL source must remain exact 2.5.1 and its original ScreenManager orientation logic remains:

```csharp
currentInvert = !_IsQuest();
```

The temporary hardcoded `true` test was diagnostic only.

## BUILD / UPLOAD INCIDENT — CLOSED ENOUGH FOR BETA, KEEP AS EVIDENCE

The beta upload was initially blocked by a Unity compiler/package issue involving Memory Profiler/Burst. UdonSharp then blocked the VRChat build because Unity was not compile-clean.

Memory Profiler was removed from the package manifest as the narrow recovery action. Unity later became unresponsive and had to be restarted.

After recovery, the world uploaded successfully.

Do not describe this as a runtime memory/performance problem in Open Classroom; it was an Editor/build-pipeline blocker.

## PERSISTENT POSTER — BETA BASELINE

The manually rebuilt poster remains the accepted design baseline.

Reference:

`HANDOFF_2026-09-12_POSTER_WORKING_BASELINE.md`

Known proven/reported before beta launch:

- direct image loading works;
- current-instance multiplayer URL/image synchronization works;
- one existing top-root VRCObjectSync owns physical position/rotation;
- uniform local scaling works;
- rejected generated poster work from 2026-09-10/11 remains historical only.

Do not infer that persistent URL/position/rotation/scale storage is complete merely because the beta world is live. Persistence requires its own evidence.

## BETA FEEDBACK TRIAGE

Classify incoming tester feedback before changing anything:

1. release blocker;
2. functional bug;
3. multiplayer/sync problem;
4. Quest/PC platform problem;
5. usability/confusion;
6. visual/polish request;
7. future feature request.

Beta fixes should be narrow and evidence-backed.

## CURRENT BETA TEST FOCUS

Useful checks include:

- can a new visitor understand the world without live explanation;
- paper tablet navigation;
- projector/video controls;
- ordinary VideoTXL playback;
- Quest VideoTXL orientation;
- VideoTXL -> Presentation -> VideoTXL switching;
- Presentation navigation and late join;
- e-reader usability;
- Admin access flow;
- Presentation upload-access flow;
- Quest/PC readability and performance.

## EXACT NEXT PHASE

```text
beta world live
-> recruit testers
-> gather real reports
-> confirm Quest VideoTXL orientation in uploaded build
-> fix only reproduced beta issues
-> preserve proven foundations
-> close critical regressions
-> freeze accepted beta baseline
-> later package/harden reusable products
```

## WORKING STYLE WITH STEF

- Dutch;
- beginner-friendly;
- one small technical action at a time;
- explain why before technique;
- inspect screenshots/logs instead of guessing;
- backup before meaningful risk;
- no broad refactors;
- no autonomous visual redesign;
- Codex only for bounded implementation/debugging when Stef explicitly wants it;
- real Unity/VRChat behaviour outranks documentation.

## SOURCE OF TRUTH

```text
real uploaded/tested beta behaviour
-> HANDOFF_2026-09-12_BETA_LIVE.md
-> CURRENT_WORK.md
-> accepted feature evidence
-> older handoffs/recovery docs
```
