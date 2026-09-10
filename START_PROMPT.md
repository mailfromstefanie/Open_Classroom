# Start Prompt — Open Classroom

Use this file to start the **next** ChatGPT/Codex session.

## Primary project

Repository:

`mailfromstefanie/Open_Classroom`

Real Unity project:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

Cross-project context only:
- `mailfromstefanie/StefanieInVR-Presentation-Service`
- `mailfromstefanie/Stefanies-Art-House-Cinema`

Do not edit those other repositories unless Stef explicitly asks.

## Read first — mandatory

1. `AGENTS.md`
2. `CURRENT_WORK.md`
3. `RECOVERY_DECISION_2026-09-10.md`
4. `WORKLOG_2026-09-08.md`

Only read the Entrance Text / Poster implementation documents after recovery if their technical design becomes relevant again. They describe attempted work and useful architecture, but they are **not an accepted current scene baseline**.

## Critical current truth — 2026-09-10

Tonight's later Unity work did **not** produce an acceptable scene.

Stef inspected the real project and reported:
- the Canvas/tablet layout is badly disturbed;
- multiple / all relevant buttons no longer work correctly;
- the problem is partly visual and cannot be sensibly repaired by continuing automated scene editing.

Therefore:

**DO NOT REPAIR THE CURRENT SCENE.**

**DO NOT CONTINUE BUILDING ON IT.**

Stef has decided to restore the full Unity backup she made at the **beginning of the evening**.

That restored backup must become the candidate baseline.

Earlier GitHub notes saying the Content-tab/editor smoke test passed do not override Stef's later real visual/functional rejection.

## Exact task for the next Codex session

### RECOVERY ONLY

1. Read the files above.
2. Help Stef restore the full beginning-of-evening backup over the current working Unity project.
3. Do not copy, merge or salvage scene objects from the currently broken project into the backup.
4. Open the restored project.
5. Let Unity finish importing/compiling.
6. Check only:
   - old tablet/canvas layout looks restored;
   - pre-existing buttons work again;
   - Presentation is still present;
   - exact VideoTXL 2.5.1 is still present;
   - e-reader/library is still present;
   - reset systems are still present.
7. Do not add Entrance Text title changes, Poster integration, Content tab or any other new feature.
8. Do not redesign anything.
9. Report exactly what the restored backup contains.
10. **STOP and wait for Stef.**

The backup is not officially accepted until Stef herself visually checks the scene and says it is good.

## What is being rolled back

Treat all Unity scene/UI/runtime changes made after the selected beginning-of-evening backup as discarded.

This includes, where absent from that backup:
- tonight's Entrance Text V1.1 title/body UI work;
- Persistent Poster scene/runtime integration;
- `eDit` / `Panel (Content)` tab at index 8;
- moving Entrance Text into that Content panel;
- poster editor layout in that panel;
- related tab/button wiring and later scene changes.

Do not delete their GitHub documentation. It remains reference/history only.

## After recovery — working method changes

Once Stef explicitly accepts the restored backup:

- Stef will build most visual Canvas/UI layout manually.
- Nova/ChatGPT will guide her step by step.
- Codex is used only when a small technical action is genuinely useful.
- One small change at a time.
- Test immediately after each change.
- Never batch several visual/UI changes into one autonomous pass.
- Never treat compile/Play Mode success as proof that the UI looks or feels correct.
- Stef's visual inspection is required for UI acceptance.

## Protected systems

Unless Stef explicitly requests a change, protect:
- Presentation system;
- exact VideoTXL 2.5.1;
- physical projector/screen path;
- e-reader/library architecture;
- PlayerData reading progress;
- Marker Pro reset;
- existing tablet/navigation that is working in the restored backup;
- local table screens;
- unrelated scene objects.

## Existing technical knowledge that may be reused later

GitHub contains documentation for:
- persistent Entrance Text architecture;
- Entrance Text V1.1 title/body concept;
- Persistent Poster architecture;
- PlayerData + synced-instance-state patterns.

These are **reference designs only after rollback**.

Do not automatically recreate them.

If Stef later asks to bring one back:
1. inspect the restored baseline first;
2. agree on one tiny step;
3. implement only that step;
4. Stef visually tests;
5. continue only after approval.

## Working style

- speak Dutch to Stef;
- beginner-friendly;
- explain what and why before technical detail;
- one action at a time;
- exact GameObject/component/Inspector field when known;
- no broad refactors;
- no autonomous visual redesign;
- do not claim VRChat/Quest/multiplayer proof without real testing;
- GitHub is durable project memory, but the **restored and Stef-approved Unity scene is authoritative scene truth**.
