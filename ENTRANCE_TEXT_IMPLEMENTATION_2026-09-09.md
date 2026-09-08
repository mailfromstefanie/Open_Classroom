# Entrance Text V1 Implementation Snapshot — 2026-09-09

Status: **IMPLEMENTED / LOCALLY CLEAN — REAL VRCHAT ACCEPTANCE OPEN**

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

## Required real VRChat acceptance

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

Until the above real-client tests pass, describe the feature as:

```text
Persistent Entrance Text V1
= IMPLEMENTED
= COMPILES
= LOCAL SMOKE TEST CLEAN
= REAL MULTIPLAYER/PERSISTENCE ACCEPTANCE OPEN
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
