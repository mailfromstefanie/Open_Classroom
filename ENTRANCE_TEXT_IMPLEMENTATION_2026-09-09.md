# Entrance Text V1.1 Implementation Snapshot — 2026-09-09

Status: **TITLE + BODY IMPLEMENTED / UNITY COMPILE + WIRING VERIFIED — REAL VRCHAT TITLE/MULTIPLAYER ACCEPTANCE OPEN**

This snapshot records the post-implementation state reported by Codex after the approved Persistent Entrance Text V1 build.

## Implemented files

Created:

- `Assets/!StefanieInVR/Scripts/Managers/EntranceTextManager.cs`
- `Assets/!StefanieInVR/Scripts/Managers/EntranceTextEditorUI.cs`
- required UdonSharp asset files

Updated:

- `Assets/#Classroom/Scenes/Classroom.unity`

## Added scene objects

Added:

- `UIs/Managers/Entrance Text Manager`
- `VipContentRoot/Entrance Text Editor`

Editor UI contains:

- separate title and body TMP input fields;
- Apply / Save;
- Load Default;
- Claim / Start;
- status text.

## Wiring

Reported wired:

- existing `Text (Kop)`;
- existing `Text (Alinea)`;
- `VipAccessManager`;
- editor references;
- button events.

Validation:

- title maximum 64 characters / one normalized line;
- maximum 400 characters;
- maximum 9 lines;
- whitespace-only title/body independently resolve to their defaults.

## Preserved existing entrance behaviour

The existing visitor entrance path remains unchanged.

Preserved:

```text
Button (Open)
-> Canvas (Welcome).SetActive(false)
```

Reported Play Mode verification confirms that this still locally deactivates:

- the welcome Canvas;
- the existing `Collider_Entrance`.

One visitor's dismissal remains intended to be local.

Not changed:

- `Collider_Entrance`;
- existing Open button;
- Presentation;
- VideoTXL;
- unrelated systems.

## Local implementation proof

Reported after implementation:

- UdonSharp compiles without errors;
- Play Mode smoke test completed without errors;
- existing Open behaviour verified;
- scene saved;
- Unity scene reported not dirty after completion.

This is useful local proof only.

It is **not** equivalent to real VRChat networking or persistence acceptance.

## Real VRChat evidence — 2026-09-09

Stef performed a real VRChat build/test after the implementation.

Observed by Stef:
- the editable entrance body text appeared to persist after leaving and returning;
- this is positive evidence that the PlayerData persistence path is functioning for her account in a real build.

Evidence boundary:
- this was not yet a controlled two-user acceptance pass;
- Stef has not yet verified that another player sees the published text;
- late-join reconstruction has not yet been proven;
- host-leave shared-state survival has not yet been proven;
- Group/Public Claim/Start has not yet been proven.

Therefore record:

```text
single-user real VRChat body-text persistence
= POSITIVE OBSERVATION

shared multiplayer visibility
= NOT YET TESTED
```

## V1.1 — editable title implemented

The body implementation was extended in place; it was not rebuilt.

Implemented:
- preserved `Canvas (Welcome)/Text (Kop)` as the visible title;
- added a separate local single-line title input to the existing Entrance Text Editor;
- added title PlayerData key `open_classroom.entrance_title.v1`;
- preserved body PlayerData key `open_classroom.entrance_text.v1` so existing personal body text remains compatible;
- added `[UdonSynced] currentEntranceTitle` beside the existing current-instance body state;
- PlayerData title + body are read only from `OnPlayerRestored` for the local player;
- Apply / Save validates, persists and publishes both values together, with one serialization request;
- Load Default loads both defaults into local drafts only; Apply / Save is still required;
- title validation is plain text, one normalized line and maximum 64 characters;
- blank title/body independently fall back to their configured defaults;
- late join reconstruction uses the synchronized title + body fields;
- authority and ownership transfer do not clear the already synchronized content.

Compact-layout handling:
- editor root remains `20 x 11.8` with scale about `0.796`;
- the existing top label row was split horizontally for `Entrance title` plus its input;
- body input height and button/status positions were preserved;
- title input is single-line and capped at 64 characters;
- body input remains multiline, 400 characters and 9 logical lines;
- visible title auto-sizes from 25 down to 18, does not wrap and uses ellipsis;
- visible body auto-sizes from 16 down to 11, wraps and uses ellipsis if its fixed display area is exceeded;
- inputs remain clipped to their own `RectMask2D` viewports; no outer VIP-tab mask was introduced.

Verified locally after implementation:
- Unity imported and UdonSharp compiled the changed scripts;
- both generated UdonSharp program assets have an empty `assemblyError`;
- scene upgrade completed in the already-open Unity Editor and saved successfully;
- exactly one title input exists;
- manager title display and editor title input references are serialized;
- the temporary scene-upgrade helper was removed after the saved upgrade.

Not tested in this V1.1 pass:
- Play Mode interaction smoke test;
- title PlayerData in a real VRChat build;
- controlled two-client visibility;
- late join;
- host departure/ownership migration;
- Group/Public Claim/Start.

## Required remaining real VRChat acceptance

Still test with real clients:

1. VIP instance owner creates a fresh private instance.
2. Saved/default text initializes only after PlayerData restoration.
3. Apply several updates with two clients.
4. Confirm typing remains local.
5. Confirm late join receives the newest text.
6. Confirm published text survives host departure and Master/network ownership changes.
7. Test explicit Claim/Start in Group/Public if those instance types are in scope.
8. Create another instance later and confirm the teacher's personal saved text returns.
9. Verify Load Default changes only the two local drafts until Apply / Save is pressed.
10. Confirm one visitor pressing Open does not dismiss another visitor's welcome Canvas/blocker.

## Acceptance boundary

Current truthful status:

```text
Persistent Entrance Text V1.1 title + body
= IMPLEMENTED
= COMPILES
= SCENE WIRING VERIFIED
= EARLIER SINGLE-USER REAL VRCHAT BODY PERSISTENCE LOOKS POSITIVE
= TITLE PERSISTENCE NOT YET TESTED
= SHARED MULTIPLAYER VISIBILITY NOT YET TESTED
```

Do not claim:

- two-client PASS;
- late-join PASS;
- host-leave PASS;
- Group/Public Claim PASS;
- cross-instance PlayerData PASS;

until those specific tests are actually observed.

## Related planning document

`ENTRANCE_TEXT_INVESTIGATION_PLAN_2026-09-09.md`

The tested real Unity scene remains authoritative if it differs from the earlier plan.
