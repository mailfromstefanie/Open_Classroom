# Entrance Text V1.1 — Editable Title — 2026-09-09

Status: **IMPLEMENTED IN REAL UNITY PROJECT / UNITY + UDONSHARP COMPILE VERIFIED / REAL VRCHAT ACCEPTANCE OPEN**

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

## Implemented result

The existing V1 system was extended in place.

Manager changes:
- preserved body PlayerData key `open_classroom.entrance_text.v1`;
- added title PlayerData key `open_classroom.entrance_title.v1`;
- added `entranceTitleDisplay` for the existing `Text (Kop)`;
- added separately restored personal title state;
- added manually synchronized `currentEntranceTitle`;
- title + body initialize, publish, deserialize and update displays together;
- title + body are saved to personal PlayerData by the authorized publishing player;
- leaving the instance does not blank or replace the synchronized values;
- default title is `Stefanie's Open Classroom (Beta)`;
- title limit is 64 characters and one normalized plain-text line.

Editor changes:
- added a separate title `TMP_InputField`;
- dirty-state comparison includes title + body;
- Apply / Save sends both drafts together;
- Load Default restores both local drafts without publishing;
- restored/published drafts refresh together.

Scene changes:
- reused `Text (Kop)` and `Text (Alinea)`;
- preserved `Canvas (Welcome)`, Open button and `Collider_Entrance`;
- kept the reduced editor root at `20 x 11.8`, scale about `0.796`;
- split the existing top label row horizontally for a short title input;
- retained the body input's existing height and the button/status positions;
- title input is single-line, 64 characters, plain text;
- title display auto-sizes from 25 down to 18, does not wrap and uses ellipsis;
- body display auto-sizes from 16 down to 11, wraps and uses ellipsis if needed;
- existing input viewports continue to use `RectMask2D` clipping.

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

## Title validation

- maximum 64 characters;
- one line; newline characters are normalized to spaces;
- plain text;
- blank input resolves to the default title.

Actual layout remains authoritative.

## Local verification performed

- Unity imported and UdonSharp compiled both modified behaviours.
- Generated `EntranceTextManager.asset` and `EntranceTextEditorUI.asset` report no assembly error.
- The already-open Unity Editor completed the bounded scene upgrade and saved `Classroom.unity`.
- The saved scene contains exactly one title input.
- Manager and editor title references are serialized in both UdonSharp proxies and backing Udon behaviours.
- The temporary editor upgrade helper was removed.
- VideoTXL, Presentation, e-reader, local table screens, the entrance collider and Open button were not changed.

No new Play Mode or real VRChat run was performed for the V1.1 title extension.

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

Editable title + body
= implemented
= Unity/UdonSharp compile verified
= saved scene wiring verified

Title PlayerData in real VRChat
= not yet tested
```
