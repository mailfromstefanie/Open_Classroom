# Entrance Text V1.1 — Editable Title Plan — 2026-09-09

Status: **PENDING SMALL EXTENSION**

Persistent Entrance Text V1 body text is already implemented.

A real VRChat build was performed after implementation. Stef observed that the editable body text appeared to persist after leaving and returning. This is positive single-user persistence evidence, but shared multiplayer visibility has not yet been tested.

## Missing requirement

The entrance title must also be editable.

Existing display object:

`UIs/Other Toggles and Systems/Canvas (Welcome)/Text (Kop)`

Do not replace this object.

## V1.1 goal

Extend the existing Entrance Text system so the teacher/host can edit both:

- Title = `Text (Kop)`
- Body = `Text (Alinea)`

Both should use the same Apply / Save flow.

## Persistence and synchronization

Title requires the same two-state model as body:

```text
PlayerData title
= teacher's personal saved title across visits/instances

synced current instance title
= title shown in the current running Classroom instance
```

Apply / Save should publish and persist title + body together.

Typing remains local.

Load Default should load both default title and default body into local drafts only. It must not publish until Apply / Save is pressed.

## Preserve

Do not change:

- Canvas (Welcome);
- Button (Open);
- Collider_Entrance;
- visitor-local dismissal;
- current authority model;
- Presentation;
- VideoTXL;
- VRCDN;
- e-reader;
- table screens;
- Marker Pro;
- unrelated systems.

Do not rebuild the feature from scratch.

## Suggested title validation

Initial target:

- approximately 60–80 characters;
- maximum 1–2 lines;
- plain text;
- reject over-limit input clearly.

Actual layout remains authoritative.

## Acceptance still open

After V1.1, real VRChat testing still needs to prove:

- another player sees the published title + body;
- late join gets the newest title + body;
- host departure leaves both values intact;
- repeated Apply works;
- personal title + body return in a later new instance;
- Group/Public Claim/Start if that route is in scope.

Current truthful status:

```text
Body persistence in real build
= positive single-user observation

Multiplayer shared visibility
= not yet tested

Editable title
= pending V1.1
```
