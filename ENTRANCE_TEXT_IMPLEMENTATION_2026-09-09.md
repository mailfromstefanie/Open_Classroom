# Entrance Text V1 Implementation Snapshot — 2026-09-09

Status: **IMPLEMENTED / LOCALLY CLEAN — SINGLE-USER REAL VRCHAT PERSISTENCE LOOKS POSITIVE; MULTIPLAYER ACCEPTANCE OPEN; TITLE V1.1 PENDING**

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

- TMP input field;
- Apply / Save;
- Load Default;
- Claim / Start;
- status text.

## Wiring

Reported wired:

- existing `Text (Alinea)`;
- `VipAccessManager`;
- editor references;
- button events.

Validation:

- maximum 400 characters;
- maximum 9 lines;
- whitespace-only text resolves to the default.

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

## Pending V1.1 — editable title

After the first real build, Stef identified one missing requirement:

`Canvas (Welcome)/Text (Kop)` must also be editable by the teacher/host.

Required V1.1 direction:
- preserve the existing `Text (Kop)` TextMeshProUGUI as the visible title;
- add a local title TMP input to the existing Entrance Text Editor;
- add personal PlayerData persistence for the title;
- add synced current-instance title state;
- initialize title together with body;
- Apply / Save publishes and persists title + body together;
- late join should receive both values;
- host departure must leave both shared values unchanged;
- Load Default should load default title + default body into the local draft only;
- user still presses Apply / Save to publish;
- keep the current authority model, Canvas, Open button and collider unchanged;
- do not rebuild the entrance feature from scratch.

Use a sensible shorter title limit, approximately 60–80 characters / 1–2 lines unless the actual layout test suggests otherwise.

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
9. Verify Load Default changes only the local draft until Apply / Save is pressed.
10. Confirm one visitor pressing Open does not dismiss another visitor's welcome Canvas/blocker.

## Acceptance boundary

Current truthful status:

```text
Persistent Entrance Text V1 body
= IMPLEMENTED
= COMPILES
= LOCAL SMOKE TEST CLEAN
= SINGLE-USER REAL VRCHAT PERSISTENCE LOOKS POSITIVE
= SHARED MULTIPLAYER VISIBILITY NOT YET TESTED

Title editing
= NOT YET IMPLEMENTED
= V1.1 PENDING
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
