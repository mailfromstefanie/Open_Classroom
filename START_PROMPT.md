# Start Prompt — Open Classroom

## Entry point role

This is the **direct Open Classroom / Codex specialist prompt**.

For Stef's normal Nova/ChatGPT entrypoint across Cinema, Open Classroom and Presentation Service, use:

`mailfromstefanie/StefanieInVR-Project-Hub/STARTPROMPT.txt`

The Hub routes cross-project work. It never overrides this repository or the real Stef-approved Unity scene.

See also: `PROJECT_HUB.md`.

---

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

Only read Entrance Text / Poster implementation documents after recovery if their technical design becomes relevant again. They describe attempted work and useful architecture, but they are **not an accepted current scene baseline**.

## Critical current truth — 2026-09-10

The later title/poster/Content-tab Unity scene is **rejected as a usable baseline**.

Stef inspected the real project and reported:
- the Canvas/tablet layout is badly disturbed;
- multiple/all relevant buttons no longer work correctly;
- the problem is partly visual and should not be repaired by more autonomous scene editing.

Therefore:

**DO NOT REPAIR THE CURRENT BROKEN SCENE.**

**DO NOT CONTINUE BUILDING ON IT.**

Stef selected the rollback backup that already contains the working **persistent welcome/body text**, but predates:
- editable entrance title;
- Persistent Poster runtime/scene integration;
- `Panel (Content)` / index-8 integration.

The restored backup is only a **candidate baseline** until Stef herself visually and functionally approves it.

Earlier compile/editor/Play Mode notes never override Stef's later real visual/functional rejection.

## Exact next task — recovery only

1. Restore the selected backup if Stef has not already done so.
2. Do not copy/merge scene objects from the broken project into the backup.
3. Open the restored project and allow Unity to finish importing/compiling.
4. Verify only:
   - old tablet/canvas layout is restored;
   - pre-existing buttons work again;
   - Presentation remains present;
   - exact VideoTXL 2.5.1 remains present;
   - e-reader/library remains present;
   - reset systems remain present.
5. Add no new feature.
6. Redesign nothing.
7. Report exactly what the restored backup contains.
8. **STOP for Stef's visual approval.**

## What is rolled back

Treat later scene/UI/runtime changes after the selected backup as discarded baseline work, including where absent from the backup:
- Entrance Text V1.1 editable-title/UI additions;
- Persistent Poster scene/runtime integration;
- `eDit` / `Panel (Content)` index-8 tab;
- moving Entrance Text into that Content panel;
- poster editor layout/wiring;
- related later scene changes.

Do not delete their GitHub documentation. It remains research/history/reference only.

## Protected proven systems

Unless Stef explicitly requests otherwise, protect:
- proven Presentation Core + integration;
- exact VideoTXL 2.5.1;
- physical projector/screen path;
- e-reader/library architecture;
- PlayerData reading progress pattern;
- Marker Pro reset;
- working restored tablet/navigation;
- local table screens;
- unrelated scene objects.

Presentation acceptance remains valid despite the later UI rollback, including:
- real two-client sync;
- cross-client slide control;
- OFF/ON resume;
- late join;
- VideoTXL local suspend/restore.

A separate formal Quest-headset PASS is not documented.

## After recovery acceptance

Only after Stef explicitly approves the restored scene:

1. manually separate `Collider_Entrance` from the welcome Canvas while preserving local Open/dismiss behaviour;
2. Stef manually creates the new tablet tab and visual UI;
3. reconnect existing welcome-text logic one tiny reference/button step at a time;
4. add editable welcome title only after the new UI is visually approved;
5. reconsider the poster from first principles;
6. keep poster URL + uniform scale controls on the tablet so tablet lock/VIP rules protect those settings;
7. inspect simple physical movement references such as the globe only where useful;
8. investigate a VRChat-supported persistent URL route, including persistent PlayerObject where technically appropriate;
9. do not automatically recreate the rejected `PersistentPoster*` implementation.

## Working method after recovery

- Stef builds most visual Canvas/UI layout manually.
- Nova/ChatGPT guides one microstep at a time.
- Codex is used only for small, bounded technical tasks.
- Never batch multiple visual/UI changes.
- Test immediately after every small change.
- Compile/Play Mode success is not proof of visual acceptance.
- Stef's visual check is required for UI acceptance.

## Working style

- Dutch to Stef;
- beginner-friendly;
- explain what and why before technique;
- one manual Unity action at a time;
- exact GameObject/component/Inspector field when known;
- no broad refactors;
- no autonomous visual redesign;
- do not claim VRChat/Quest/multiplayer proof without real evidence;
- GitHub is durable project memory, but the **restored and Stef-approved Unity scene is authoritative scene truth**.

## Session close

Update Open Classroom truth only for what actually changed/tested.
Update `mailfromstefanie/StefanieInVR-Project-Hub/CURRENT_ECOSYSTEM.md` only when the recovery/milestone changes the cross-project picture.
Do not rewrite the Hub master startprompt after ordinary Classroom work.
