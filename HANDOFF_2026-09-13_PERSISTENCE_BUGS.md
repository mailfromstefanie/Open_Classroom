# Handoff — 2026-09-13 — Entrance Text + Persistent Poster Bugs

## Authority

This handoff records the concrete beta work selected for the next session.

Real Unity project:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

The real current Unity project and real VRChat behaviour remain stronger truth than GitHub reference copies.

Do not broaden this session into unrelated beta work.

---

## Session focus

Two persistence problems are the only intended work block:

1. Persistent Entrance Text behaviour across users/rejoin.
2. Persistent Poster persistence bugs / incomplete persistent-state path.

Presentation, VideoTXL, e-reader, Cinema and hosted Presentation Service changes are parked unless a regression caused by one of these fixes is actually observed.

---

# 1. Persistent Entrance Text — reproduced multiplayer/rejoin bug

## Real test performed by Stef + Pieter

Observed sequence:

1. Stef opened an Open Classroom instance.
2. Pieter joined and initially saw an older entrance text.
3. Stef changed the entrance text.
4. Pieter rejoined and then saw Stef's newly changed text.
5. Pieter changed the entrance text.
6. Stef rejoined.
7. Stef still saw her own previously saved text instead of Pieter's current instance text.

This is concrete real-client evidence.

## What this strongly indicates

The current system combines two different responsibilities:

```text
PlayerData
= personal persistent data tied to one VRChat user

[UdonSynced] entrance state
= shared truth for the currently running instance
```

The observed behaviour is consistent with local PlayerData restoration overriding or republishing personal text during join/rejoin, instead of allowing already-existing synchronized instance state to remain authoritative.

Do not call this diagnosis final until the current real `EntranceTextManager.cs` is inspected.

## Important existing evidence

Earlier real-world evidence already suggested PlayerData persistence itself works for Stef's account.

Therefore the next investigation should NOT begin by assuming VRChat persistence is broken.

The likely bug class is restore/initialization/authority ordering between:

- `OnPlayerRestored`;
- personal PlayerData;
- current `[UdonSynced]` instance text/title;
- ownership / serialization;
- first-instance initialization versus ordinary rejoin.

## Desired semantics

The intended model should remain conceptually separated:

```text
PERSONAL PERSISTENCE
teacher's saved preferred/default entrance title + body

CURRENT INSTANCE
one shared synchronized entrance title + body seen by everyone
```

A user's personal PlayerData must not automatically replace an already-established current-instance shared value merely because that user rejoins.

For a genuinely new/uninitialized instance, an authorized teacher's persisted value may initialize the instance according to the accepted authority rules.

Exact implementation must be decided only after inspecting the current real script.

## First action next session

Do not test again first.

Inspect the COMPLETE current real:

`EntranceTextManager.cs`

and, if needed after that:

`EntranceTextEditorUI.cs`

Specifically trace:

```text
OnPlayerRestored
-> local PlayerData read
-> display application
-> ownership acquisition
-> synced-field writes
-> RequestSerialization
-> OnDeserialization
```

Goal: find exactly where personal restore can override already-established instance state.

Then make the smallest safe fix and retest with two real users.

## Required acceptance test after fix

Minimum controlled test:

1. Stef creates fresh private instance.
2. Pieter joins.
3. Confirm both see the same current entrance title/body.
4. Stef changes + saves/publishes text.
5. Pieter sees it.
6. Pieter leaves and rejoins -> still sees current instance text.
7. Pieter changes + saves/publishes text if he is authorized for the test.
8. Stef leaves and rejoins -> must see the current instance text, not silently restore her older personal value over it.
9. Later create a genuinely new instance and separately verify intended personal persistence initialization.

Do not mark PASS until these observed behaviours are recorded.

---

# 2. Persistent Poster — active persistence work, current real Unity state outranks repo baseline

## Accepted manual baseline remains protected

The accepted physical/manual architecture remains from:

`HANDOFF_2026-09-12_POSTER_WORKING_BASELINE.md`

Protected baseline includes:

- one physical top-root `Persistent_Poster`;
- one existing `VRCObjectSync` for live position/rotation;
- `PosterSharedState` for current-instance URL / poster-presence sync;
- `PosterImageLoaderLocal` for local image loading/material application;
- `PosterScaleManager` for uniform scale;
- no second `VRCObjectSync` for scaling;
- rejected generated poster architecture from 2026-09-10/11 remains rejected history.

## Important status boundary

The 2026-09-12 GitHub baseline still records full persistent URL/position/rotation/scale storage as NOT IMPLEMENTED YET.

However, Stef continued local Unity work after that baseline and is now reporting poster persistence problems that need to be fixed.

Therefore:

```text
GitHub poster persistence docs
= reference / older baseline

current local Unity poster scripts + scene
= authority for the next debugging session
```

Do NOT rebuild from the old persistence plan and do NOT assume the current local implementation matches the earlier planned PlayerObject/VRCEnablePersistence approach.

## First action for poster next session

After Entrance Text is understood/fixed, inspect the COMPLETE current local poster script family before changing anything.

At minimum collect the current versions of whichever of these actually exist in the project:

- `PosterSharedState.cs`;
- `PosterImageLoaderLocal.cs`;
- `PosterScaleManager.cs`;
- any newly added persistence storage / PlayerObject / object-persistence scripts;
- relevant hierarchy/Inspector screenshot if wiring matters.

Then state the exact observed poster bug in one sentence before implementing a fix.

Do not guess the bug from the older handoff.

## Persistence responsibilities to preserve while debugging

Keep responsibilities explicit:

```text
CURRENT INSTANCE / LIVE SYNC
URL + hasPoster       -> shared-state manager
position + rotation   -> existing VRCObjectSync
scale                 -> scale manager/shared scale state

PERSISTENT STORAGE
must restore saved poster state without creating duplicate physical posters
and without fighting the live current-instance authorities
```

The physical poster must remain one shared scene object unless the current tested implementation proves a deliberately different accepted architecture.

## Poster acceptance boundary

Do not call poster persistence fixed merely because:

- image loading works;
- current-instance URL sync works;
- moving the pickup syncs;
- scaling works in one client;
- compile is clean.

Persistence requires explicit leave/rejoin/new-instance evidence for the intended authorized user semantics.

---

# Order for next session

```text
1. EntranceTextManager current real code
2. isolate personal PlayerData vs instance-sync overwrite
3. smallest entrance-text fix
4. two-user rejoin acceptance
5. inspect current real poster persistence implementation
6. reproduce/name exact poster bug
7. smallest poster fix
8. persistence acceptance test
9. update GitHub with observed evidence only
```

---

## Do not do in this work block

- no Presentation redesign;
- no VRCStreamer integration;
- no Cinema work;
- no hosted Presentation Service changes;
- no broad tablet redesign;
- no resurrection of rejected poster generation work;
- no speculative refactor before current real scripts are inspected.

---

## Working style

- Dutch with Stef;
- explain why before technique;
- one small action at a time;
- inspect complete current scripts/screenshots rather than guessing;
- real VRChat evidence outranks compile-only evidence;
- backup before a meaningful risky change;
- keep fixes narrow during beta.
