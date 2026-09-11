# Handoff — 2026-09-11 — Poster Failed Acceptance / Narrow Rebuild

## Status

Stef has chosen to keep the current Open Classroom scene for now, but the **current Persistent Poster implementation has failed functional acceptance** and should no longer be repaired forward as the preferred path.

The 2026-09-10 rollback backup remains a protected fallback. Do not discard it.

Real Unity project:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

## Current user-observed truth

### Entrance Text

Entrance/welcome text is functionally working well enough for now.

Its editor UI does not visually match the established Classroom tablet style, but that is a later styling task.

Do not reopen Entrance Text logic now unless new functional evidence appears.

### Presentation / PowerPoint UI

Stef also sees visual inconsistency here, but this is not a current priority.
Do not modify Presentation or VideoTXL during the poster rebuild.

## Current Poster — FAILED ACCEPTANCE

The generated Persistent Poster must not be described as working, editor-proven, or ready for multiplayer acceptance.

Latest tested result from 2026-09-11:
- most of the Content UI is not clickable in ClientSim/editor;
- in a real VRChat test build the poster URL field can accept **visible text** after a narrow input correction;
- confirming/submitting that field does **not** produce an image;
- `Load / Apply` and the other poster controls were not operable in the real test;
- no downloaded poster image appeared on the temporary generated surface.

## Technical findings

### URL input

There is one active poster `VRCUrlInputField`; this is not caused by two duplicate fields sitting on top of one another.

The narrow input correction made the field usable enough to type visibly in a real VRChat build:
- only the appropriate input root receives the relevant raycast interaction;
- background/caret/text visibility was improved enough for testing.

However:
- `onEndEdit` is empty;
- `onValueChanged` is empty;
- therefore pressing Enter / confirming the URL field alone does not start `VRCImageDownloader`.

### Load / Apply gate

`Load / Apply` is controlled by `CanLocalPublish()` and depends on:

```text
posterInitialized
&& playerDataReady
&& IsLocalChangeAllowedByLock()
```

In the failed real test this control path was not operable.

Do not spend another session trying to untangle all current authorization/UI coupling before the basic image chain has been proven independently.

### Imgur failure

A previous real VRChat log showed:

```text
https://imgur.com/oaEbiM2.png
-> VRCImageDownloader
-> Redirect limit exceeded
```

Use a direct final image URL for future tests, for example:

`https://i.imgur.com/oaEbiM2.png`

This is only a test URL. The poster must remain generic for valid direct HTTPS image URLs supported by VRChat. Do not add Imgur-specific logic.

### Wrong physical poster design

The current visible poster surface is a generated Unity Quad inside a generated Cube/Quad hierarchy.

This is **not** Stef's intended design.

Stef wants to identify an existing Plane/GameObject herself, using the same simple mindset as the existing local table screens. That chosen plane becomes the visual target for the rebuilt image loader.

## Manual change already made by Stef

The poster editor incorrectly referenced the Entrance Text editor's `Button (Claim Start)`.

Stef cleared:

`PersistentPosterEditorUI.claimButton = None`

The Entrance Text UI had already been functionally usable; this erroneous cross-reference was real but was not the main poster failure.

## AGREED NEXT DIRECTION — REBUILD NARROWLY

Do **not** continue trying to make the rejected generated poster architecture fully work first.

Do **not** begin with persistence, synchronization, authorization, movement, scale, or full UI styling.

### Gate 1 — local image proof on Stef's exact plane

1. Stef identifies/selects the exact existing Plane/GameObject that must become the poster surface.
2. Build one isolated local chain only:

```text
standalone VRCUrlInputField
-> one existing-style tablet button
-> VRCImageDownloader
-> renderer/material on Stef's selected plane
```

3. Do not involve VideoTXL.
4. Do not generate a new Cube, Quad, frame, or replacement poster mesh.
5. Test in a **real VRChat client** with a direct URL such as `https://i.imgur.com/oaEbiM2.png`.
6. Acceptance for this gate is only:

```text
URL visible
-> click Load
-> downloader succeeds
-> image visible on Stef's selected plane
```

### Gate 2 — only after Gate 1 passes

Add one layer at a time:
- authorization / tablet lock;
- PlayerData where genuinely useful;
- synchronized instance URL/state;
- movement / placement if Stef still wants it;
- uniform scale if still wanted;
- multiplayer / late join / host-leave tests.

### Gate 3 — styling

Only after the technical chain works, make the panel visually match the existing Classroom tablet.

Use known-good existing tablet UI as the source of truth:
- existing TMP font/material;
- button sprites;
- hover/pressed/disabled transitions;
- colors;
- padding/dimensions;
- input-field technique;
- layers/raycast conventions.

Prefer duplicating/reusing existing good tablet elements over inventing new UI components.

## Protected systems

Do not modify during this poster rebuild unless Stef explicitly redirects the work:
- working Entrance Text logic;
- Presentation Core / Presentation integration;
- VideoTXL 2.5.1;
- e-readers/library;
- Marker Pro reset;
- local table screens;
- unrelated tablet panels/navigation.

## Interrupted GitHub documentation work

Codex attempted to update `CURRENT_WORK.md`, `PERSISTENT_POSTER_IMPLEMENTATION_2026-09-10.md`, and later `RECOVERY_DECISION_2026-09-10.md` from a temporary GitHub checkout.

The first patch attempts failed because of document encoding/context mismatch. Codex then prepared safer append-only authoritative corrections in the temporary checkout.

Before a commit/push was completed, the Codex usage limit was reached.

Therefore those temporary checkout edits are not GitHub truth.

This handoff is the authoritative 2026-09-11 session correction and must be read before older poster implementation claims.

## Exact next action

Do not ask Codex to resume the old generated poster repair.

Next poster session:
1. Stef selects the exact intended existing plane in Unity.
2. Record its exact hierarchy path / renderer / material.
3. Build only the isolated local URL -> Load -> image chain on that plane.
4. Test it in real VRChat.
5. STOP and record evidence before adding any persistence/networking/UI complexity.
