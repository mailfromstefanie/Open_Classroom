# Current Work — Open Classroom

Last updated: 2026-09-13 Europe/Amsterdam

## READ THIS FIRST

Newest active debugging handoff:

`HANDOFF_2026-09-13_PERSISTENCE_BUGS.md`

Release-phase authority remains:

`HANDOFF_2026-09-12_BETA_LIVE.md`

Accepted manual poster baseline:

`HANDOFF_2026-09-12_POSTER_WORKING_BASELINE.md`

Historical recovery/poster files remain useful evidence only where they do not conflict with the live beta truth or the current real Unity project.

Real Unity project:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

The real uploaded/tested VRChat world remains the strongest source of truth.

## CURRENT STATUS — BETA WORLD LIVE / TWO PERSISTENCE BUGS ACTIVE

**Open Classroom is running as a live beta-test world.**

The immediate next work is deliberately narrow:

```text
Persistent Entrance Text rejoin/authority bug
-> fix and two-user acceptance
-> inspect current real Persistent Poster persistence implementation
-> reproduce exact poster persistence bug
-> smallest safe fix
-> persistence acceptance
```

Do not broaden this block into unrelated beta work.

## PERSISTENT ENTRANCE TEXT — REPRODUCED REAL-CLIENT BUG

Stef and Pieter performed a real two-user test.

Observed:

1. Stef opened the instance.
2. Pieter joined and initially saw an older entrance text.
3. Stef changed the text.
4. Pieter rejoined and saw Stef's new text.
5. Pieter changed the text.
6. Stef rejoined.
7. Stef still saw her own previously saved text instead of Pieter's current instance text.

This strongly indicates a responsibility/order problem between:

```text
PlayerData
= personal persistent data for one VRChat user

[UdonSynced] entrance state
= shared truth for the currently running instance
```

The likely bug class is that local PlayerData restoration during join/rejoin can override or republish personal content instead of preserving already-established shared instance state.

Do not treat that as final diagnosis until the COMPLETE current real `EntranceTextManager.cs` is inspected.

Exact next action:

```text
inspect current EntranceTextManager.cs
-> trace OnPlayerRestored / PlayerData read / synced writes / ownership / RequestSerialization / OnDeserialization
-> identify overwrite point
-> smallest safe fix
-> two-user rejoin acceptance
```

Canonical detailed route:

`HANDOFF_2026-09-13_PERSISTENCE_BUGS.md`

## PERSISTENT POSTER — CURRENT LOCAL IMPLEMENTATION MUST BE INSPECTED

The manually rebuilt poster remains the protected accepted baseline.

Reference:

`HANDOFF_2026-09-12_POSTER_WORKING_BASELINE.md`

Known accepted baseline before later persistence work:

- direct image loading works;
- current-instance URL/image synchronization works;
- one existing top-root VRCObjectSync owns live position/rotation;
- uniform scale baseline works;
- rejected generated poster work from 2026-09-10/11 remains historical only.

Important status boundary:

The 2026-09-12 GitHub handoff still says full persistent URL/position/rotation/scale storage was not implemented yet, but Stef continued local Unity work after that point and is now reporting poster persistence problems.

Therefore:

```text
current local Unity poster scripts + scene
= authority

older GitHub persistence plan
= reference only
```

Do not rebuild from the old plan. First inspect the current complete poster script family and state the exact observed poster bug before changing code.

Detailed route:

`HANDOFF_2026-09-13_PERSISTENCE_BUGS.md`

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
- accepted entrance-text architecture except the reproduced restore/authority bug;
- accepted manual Persistent Poster physical baseline.

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

## EXACT NEXT PHASE

```text
1. inspect current real EntranceTextManager
2. fix PlayerData vs current-instance authority/rejoin bug
3. two-user acceptance with Stef + Pieter or equivalent
4. inspect current real poster persistence scripts/scene
5. reproduce and name exact poster bug
6. smallest safe poster fix
7. persistence acceptance
8. update GitHub with observed evidence only
```

## WORKING STYLE WITH STEF

- Dutch;
- beginner-friendly;
- one small technical action at a time;
- explain why before technique;
- inspect complete scripts/screenshots/logs instead of guessing;
- backup before meaningful risk;
- no broad refactors;
- no autonomous visual redesign;
- Codex only for bounded implementation/debugging when Stef explicitly wants it;
- real Unity/VRChat behaviour outranks documentation.

## SOURCE OF TRUTH

```text
real current Unity/VRChat behaviour
-> HANDOFF_2026-09-13_PERSISTENCE_BUGS.md for this active debug block
-> HANDOFF_2026-09-12_BETA_LIVE.md for release-phase truth
-> CURRENT_WORK.md
-> accepted feature evidence
-> older handoffs/recovery docs
```
