# Handoff — Open Classroom Beta Live — 2026-09-12

## Status

**Open Classroom has been uploaded successfully and the public beta-test world is now live.**

This is the newest Open Classroom session truth for release/beta status. It supersedes older recovery/poster-first status when those files describe the Classroom as not yet beta-live.

The earlier detailed poster handoff remains useful technical history:

`HANDOFF_2026-09-12_POSTER_WORKING_BASELINE.md`

## Live beta milestone

Confirmed by Stef:

- the current Open Classroom world build was uploaded successfully;
- the beta-test world is running in VRChat;
- public-facing beta recruitment has started;
- Stef posted on Facebook to find beta testers;
- the Presentation Service remains part of the beta workflow;
- questions, feedback and access requests are routed through:
  - `info@stefanieinvr.com`
  - `stefanieinvr.com`.

Current access policy during beta:

- Classroom Admin access is provided manually;
- Presentation upload access is provided manually;
- users can contact Stef to request either access or help.

## Upload/build incident immediately before beta upload

During the release/upload attempt, Unity/VRChat build work was blocked by a compiler/package problem involving Memory Profiler/Burst. UdonSharp then refused the world build because Unity was not compile-clean.

Observed explanation from the debugging session:

```text
Memory Profiler / Burst compile error
-> Unity compiler not clean
-> UdonSharp sees compiler failure
-> VRChat SDK build/upload blocked
```

Memory Profiler was removed from the package manifest as the narrow recovery action.

Unity later became unresponsive and had to be closed/reopened. After recovery, the world was successfully uploaded.

Do not treat this as evidence that Open Classroom itself had a runtime memory problem. The incident was an Editor/package/build-pipeline blocker.

## Presentation / VideoTXL release boundary

Protected rules remain:

- VideoTXL stays pinned to exact version 2.5.1;
- do not rebuild Presentation Core;
- Presentation Core remains independent of VideoTXL;
- the accepted VideoTXL suspend/restore integration remains protected;
- the physical projector/screen route is shared with Presentation but the systems have different source paths.

A Quest-only VideoTXL upside-down-output investigation occurred immediately before this beta upload.

Important evidence:

- PC/Windows VideoTXL output was correct;
- Quest VideoTXL output was vertically upside down;
- Presentation output on the same physical screen was correct;
- changing VideoTXL `currentInvert` directly did not fix the Quest result;
- investigation then moved to the Quest Custom Render Texture / RenderOut path;
- Codex identified an incorrect CRT update-material configuration as the likely root cause and prepared a narrow correction.

**Do not mark that Quest VideoTXL orientation issue fully RESOLVED until Stef explicitly confirms the uploaded beta build is correct on Quest.**

The original VideoTXL 2.5.1 ScreenManager logic must remain:

```csharp
currentInvert = !_IsQuest();
```

The temporary `currentInvert = true;` test was diagnostic only and is not the accepted implementation.

## Poster status at beta start

The manually rebuilt poster baseline remains the accepted architecture reference.

Known proven/reported before beta launch:

- direct image loading works;
- current-instance multiplayer URL/image sharing works;
- one existing top-root `VRCObjectSync` owns physical position/rotation;
- uniform local scaling works;
- the generated/rejected poster scene from 2026-09-10/11 remains historical only.

Poster persistence is not automatically considered complete merely because the beta world is live. Use current runtime evidence before claiming URL/position/rotation/scale persistence PASS.

## Beta operating rule

The project has moved from implementation-first work into **real-user beta feedback**.

Default priority now:

```text
real tester evidence
-> reproduce concrete blocker/bug/confusion
-> smallest safe fix
-> regression test
-> update beta build
```

Avoid speculative architecture changes during beta unless a real blocker requires them.

Classify incoming feedback as:

1. release blocker;
2. functional bug;
3. multiplayer/sync issue;
4. Quest/PC platform issue;
5. usability/confusion;
6. visual/polish request;
7. future feature request.

Do not convert every suggestion into immediate scope.

## Current beta test focus

Priority checks from real users should include:

- joining and understanding the world without Stef explaining it live;
- paper tablet navigation;
- projector/video controls;
- normal VideoTXL playback;
- Quest VideoTXL orientation;
- VideoTXL -> Presentation -> VideoTXL switching;
- Presentation First / Previous / Next;
- late join where practical;
- e-reader usability;
- Admin access flow;
- Presentation upload-access flow;
- obvious performance or readability problems on Quest and PC.

## Public-facing beta message direction

Open Classroom should now be described as a live beta VRChat learning/presentation environment rather than an unfinished internal prototype.

Current public framing:

- calm VR space for learning, teaching, presenting and sharing ideas;
- paper-tablet-controlled classroom tools;
- presentation system;
- e-readers;
- video/livestream support;
- visitor guide;
- feedback and beta testers welcome.

Contact/access:

`info@stefanieinvr.com`

`stefanieinvr.com`

## Exact next phase

```text
BETA WORLD LIVE
-> recruit testers
-> collect real feedback
-> confirm Quest VideoTXL orientation in uploaded build
-> triage only concrete beta issues
-> preserve working Presentation/VideoTXL/e-reader/reset foundations
-> close critical beta regressions
-> later freeze beta baseline
-> later package/harden reusable products
```

## Source-of-truth rule

```text
real uploaded/tested VRChat beta behaviour
-> this handoff + CURRENT_WORK.md
-> accepted feature evidence
-> older handoffs/recovery documents
```

If documentation conflicts with what Stef observes in the current uploaded beta world, the real tested beta world wins.
