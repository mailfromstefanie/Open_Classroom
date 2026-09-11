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

`HANDOFF_2026-09-11_POSTER_REPAIR.md` contains the newest session truth and overrides older poster/rollback instructions where they conflict.

The 2026-09-10 rollback backup remains a protected fallback. Do not discard it. The current immediate direction is a narrow poster rebuild, not an automatic rollback and not continued repair of the rejected generated poster.

## Critical current truth — 2026-09-11

Current user-observed state:
- Entrance/welcome text is functionally working well enough;
- Entrance editor UI looks inconsistent with the established tablet style, but styling is parked;
- Presentation/PowerPoint UI styling is not a current priority;
- the current generated Persistent Poster has **failed functional acceptance**;
- in a real VRChat test the poster URL field can accept visible text after a narrow input correction;
- Load/Apply and the other current poster controls were not operable;
- no poster image appeared;
- the generated Cube/Quad physical poster does not match Stef's intended existing-plane design.

A previous real VRChat log showed:

```text
https://imgur.com/oaEbiM2.png
-> VRCImageDownloader
-> Redirect limit exceeded
```

Use a direct final test URL such as:

`https://i.imgur.com/oaEbiM2.png`

This is only a test URL. Do not add Imgur-specific logic; the final poster must remain generic for valid direct HTTPS image URLs supported by VRChat.

Technical findings from the failed implementation:
- there is one active poster `VRCUrlInputField`, not duplicate fields over each other;
- `onEndEdit` and `onValueChanged` are empty, so confirming the URL field alone does not start the image downloader;
- Load/Apply depends on `posterInitialized && playerDataReady && IsLocalChangeAllowedByLock()`;
- the current visible poster surface is a generated temporary Quad, not Stef's intended existing plane;
- Stef manually cleared the erroneous `PersistentPosterEditorUI.claimButton` reference back to `None`.

Do not spend the next session untangling all old authorization/UI coupling before proving the basic image chain independently.

## Exact next task — NARROW POSTER REBUILD

Do **not** continue repairing the current generated Persistent Poster implementation as the preferred route.
Do **not** begin with persistence, synchronization, movement, scale, authorization, or full UI styling.
Do **not** modify Entrance Text logic, Presentation, VideoTXL, e-readers, local table screens or reset systems.

### Gate 1 — local image proof

1. Stef identifies/selects the exact existing Plane/GameObject that must become the poster surface.
2. Record its exact hierarchy path, Renderer and Material.
3. Build one isolated local chain only:

```text
standalone VRCUrlInputField
-> one existing-style tablet button
-> VRCImageDownloader
-> renderer/material on Stef's selected plane
```

4. Do not involve VideoTXL.
5. Do not create a new Cube, Quad, frame or replacement poster mesh.
6. Test in a real VRChat client with a direct image URL such as `https://i.imgur.com/oaEbiM2.png`.
7. Gate passes only when:

```text
URL visible
-> click Load
-> downloader succeeds
-> image visible on Stef's selected plane
```

8. STOP and record the evidence before adding another layer.

## After Gate 1 passes

Add one responsibility at a time and test immediately:
1. authorization / tablet lock;
2. PlayerData only where useful;
3. synchronized current-instance URL/state;
4. movement/placement if Stef still wants it;
5. uniform scale if still wanted;
6. multiplayer / late join / host-leave acceptance.

Only after the technical chain works should the Content/Entrance/Poster UI be restyled.

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

Prefer duplicating existing good tablet elements over generating parallel UI designs.
Compile or Play Mode success does not prove UI acceptance. Stef's visual check is required.

## Protected proven systems

Unless Stef explicitly requests otherwise, protect:
- currently working Entrance Text logic;
- proven Presentation Core + integration;
- exact VideoTXL 2.5.1;
- physical projector/screen path;
- e-reader/library architecture;
- PlayerData reading progress pattern;
- Marker Pro reset;
- local table screens;
- unrelated tablet/navigation systems.

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

If the narrow rebuild causes broad breakage or becomes harder to reason about, Stef may still choose the 2026-09-10 rollback route.

Do not silently switch strategies; Stef decides.

## Working method

- Stef identifies/approves visual objects and Canvas layout.
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
