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
2. `HANDOFF_2026-09-11_POSTER_REPAIR.md`
3. `CURRENT_WORK.md`
4. `RECOVERY_DECISION_2026-09-10.md`
5. `WORKLOG_2026-09-08.md`

`HANDOFF_2026-09-11_POSTER_REPAIR.md` contains the newest session decision and temporarily overrides the older rollback-only instruction where they conflict.

The 2026-09-10 recovery decision remains useful history and the older backup remains a safe fallback. It is **not the current immediate action** because Stef has deliberately chosen to cautiously repair the current scene first.

## Critical current truth — 2026-09-11

Stef is keeping the current scene for now and attempting a narrow repair rather than immediately restoring the older backup.

Current user-observed state:
- Entrance/welcome text is functionally working well enough;
- Entrance editor UI looks inconsistent with the established tablet style, but that is parked;
- Presentation/PowerPoint UI styling is not a current priority;
- the poster is the active technical problem;
- poster URL input can receive focus/type input in VRChat, but readability is poor;
- image loading has not yet passed a real VRChat test;
- the generated poster physical hierarchy does not match Stef's intended existing-plane design.

A read-only investigation found several UI/style inconsistencies and an accidental shared Claim-button reference.
Stef manually cleared `PersistentPosterEditorUI.claimButton` to `None`.
Do not rebuild broad UI or infer that this fixed the poster itself.

A previous real VRChat log showed a concrete poster failure:

```text
https://imgur.com/oaEbiM2.png
-> VRCImageDownloader
-> Redirect limit exceeded
```

The next test must use a direct final image URL such as:

`https://i.imgur.com/oaEbiM2.png`

This is only a test URL. Do not add Imgur-specific logic; the poster must remain generic for valid direct HTTPS image URLs supported by VRChat.

## Interrupted Codex boundary

Codex reported it was making only two narrow scene edits:
- use the direct `i.imgur.com` test URL;
- make the existing poster URL input readable enough to test.

Codex then ran out of credits while updating GitHub documentation.

Therefore first inspect the real Unity scene and verify whether those local edits actually completed. Do not blindly reapply them.

GitHub documentation was completed by Nova in `HANDOFF_2026-09-11_POSTER_REPAIR.md`.

## Exact next task — poster functionality gate only

Do not begin with full Content UI restyling.
Do not replace the physical poster geometry yet.
Do not modify Entrance Text logic, Presentation, VideoTXL, e-readers, local table screens or reset systems.

First prove this chain in real VRChat:

```text
readable-enough poster URL field
-> direct HTTPS image URL
-> Load / Apply
-> VRCImageDownloader success
-> texture visible on the CURRENT TEMPORARY poster surface
```

Procedure:
1. Inspect whether the direct test URL/readability edits are actually present in the real Unity scene.
2. Make only the smallest missing change if required.
3. Build & Run / test in a real VRChat client.
4. Use a known direct URL such as `https://i.imgur.com/oaEbiM2.png`.
5. If it fails, capture the real `VRCImageDownloader` error/status and repair only that failing link.
6. If it succeeds, record URL -> image visible as accepted technical evidence.
7. STOP before broad styling or geometry work.

## Planned order after poster functionality passes

1. Restyle the Content/Entrance/Poster editor controls to match the existing Classroom tablet.
2. Reuse existing tablet visual/technical patterns instead of inventing a parallel UI system.
3. Stef identifies/selects the exact existing Plane/GameObject intended for the poster.
4. Adapt the proven image-loading path to that renderer/material.
5. Do not generate a replacement Cube/Quad/frame unless Stef explicitly requests it.
6. Then continue movement/scale/network/late-join/persistence acceptance in small tests.

## UI style rule

The existing Classroom tablet is the visual source of truth.

Prefer reusing/copying from known-good existing controls:
- TMP font/material presets;
- button sprites;
- hover/pressed/disabled transitions;
- colors;
- sizing/padding;
- input-field technique;
- layers/raycast conventions.

Compile or Play Mode success does not prove UI acceptance. Stef's visual check is required.

## Protected proven systems

Unless Stef explicitly requests otherwise, protect:
- proven Presentation Core + integration;
- exact VideoTXL 2.5.1;
- physical projector/screen path;
- e-reader/library architecture;
- PlayerData reading progress pattern;
- Marker Pro reset;
- local table screens;
- unrelated tablet/navigation systems;
- currently working Entrance Text logic.

Presentation acceptance remains valid, including:
- real two-client sync;
- cross-client slide control;
- OFF/ON resume;
- late join;
- VideoTXL local suspend/restore.

A separate formal Quest-headset PASS is not documented.

## Backup / recovery fallback

The selected older backup remains a valid safety fallback.

Do not discard it.

If narrow current-scene repair causes further broad breakage or becomes harder to reason about, Stef may still choose the 2026-09-10 rollback route.

Do not silently switch strategies; Stef decides.

## Working method

- Stef builds/approves visual Canvas/UI layout.
- Nova/ChatGPT guides one microstep at a time.
- Codex is used only for small, bounded technical tasks.
- Never batch multiple visual/UI changes.
- Test immediately after every small change.
- No broad refactors.
- No autonomous visual redesign.
- Do not claim VRChat/Quest/multiplayer proof without real evidence.
- Real tested Unity/VRChat behaviour outranks docs.

## Working style

- Dutch to Stef;
- beginner-friendly;
- explain what and why before technique;
- one manual Unity action at a time;
- exact GameObject/component/Inspector field when known;
- GitHub is durable project memory, but the real tested Unity scene remains authoritative scene truth.

## Session close

Update Open Classroom truth only for what actually changed/tested.
Update `mailfromstefanie/StefanieInVR-Project-Hub/CURRENT_ECOSYSTEM.md` only when the recovery/milestone changes the cross-project picture.
Do not rewrite the Hub master startprompt after ordinary Classroom work.
