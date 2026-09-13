# Start Prompt — Open Classroom

## Entry point role

This is the direct Open Classroom specialist prompt.

For Stef's normal Nova/ChatGPT entrypoint across Cinema, Open Classroom and Presentation Service, use:

`mailfromstefanie/StefanieInVR-Project-Hub/STARTPROMPT.txt`

The Hub routes cross-project work. It never overrides this repository or the real tested Unity/VRChat world.

Real Unity project:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

## Read first — mandatory

Read in this order:

1. `AGENTS.md`
2. `HANDOFF_2026-09-13_PERSISTENCE_BUGS.md`
3. `CURRENT_WORK.md`
4. `HANDOFF_2026-09-12_BETA_LIVE.md` when release-phase context matters
5. `HANDOFF_2026-09-12_POSTER_WORKING_BASELINE.md` for accepted poster physical architecture
6. older historical/recovery docs only when needed

Do not ask Stef to reconstruct older recovery history when current beta evidence already answers the question.

## Critical current truth — live beta, persistence debug block active

Open Classroom is live in beta.

The immediate work block is intentionally narrow:

```text
Persistent Entrance Text rejoin/authority bug
-> fix and two-user acceptance
-> inspect current real Persistent Poster persistence implementation
-> reproduce exact poster persistence bug
-> smallest safe fix
-> persistence acceptance
```

Do not default to unrelated feature-building or broad cleanup.

## Entrance Text — reproduced real-client evidence

Stef + Pieter observed:

- Pieter initially joined and saw an older entrance text;
- Stef changed it;
- Pieter rejoined and then saw Stef's new text;
- Pieter changed the text;
- Stef rejoined and still saw her own previously saved text instead of Pieter's current instance text.

This strongly suggests a responsibility/order problem between:

```text
PlayerData = personal persistent state per VRChat user
[UdonSynced] entrance state = current running instance truth
```

Do not assume final root cause before inspecting the COMPLETE current real `EntranceTextManager.cs`.

First technical action:

```text
inspect EntranceTextManager.cs
-> trace OnPlayerRestored
-> PlayerData reads
-> synced writes
-> ownership
-> RequestSerialization
-> OnDeserialization
```

Goal: find where personal restored data can override an already-established current-instance shared value.

Then make only the smallest safe fix and perform a controlled two-user rejoin acceptance test.

## Persistent Poster — current local Unity project is authority

The accepted manual physical poster baseline remains protected:

- one physical top-root `Persistent_Poster`;
- one existing `VRCObjectSync` for live position/rotation;
- current-instance URL/image sharing architecture;
- uniform scaling architecture;
- no second `VRCObjectSync` for scaling;
- rejected generated poster work from 2026-09-10/11 remains rejected.

Important:

The 2026-09-12 GitHub poster handoff predates Stef's later local persistence work. Stef is now reporting poster persistence problems.

Therefore do NOT rebuild from the older plan.

After Entrance Text is understood/fixed:

```text
inspect complete current local poster script family + wiring
-> state exact observed poster bug in one sentence
-> reproduce
-> smallest safe fix
-> persistence acceptance
```

At minimum inspect whichever current versions exist of:

- `PosterSharedState.cs`;
- `PosterImageLoaderLocal.cs`;
- `PosterScaleManager.cs`;
- any newly added persistence / PlayerObject / object-storage script;
- hierarchy/Inspector wiring if relevant.

Real current Unity state outranks older GitHub persistence planning.

## Protected systems

Do not redesign these without reproduced beta evidence:

- exact VideoTXL 2.5.1;
- standalone Presentation Core;
- VideoTXL Presentation adapter/local suspend-restore;
- paper tablet structure/style;
- projector/screen route;
- brightness/contrast/custom screen behaviour;
- e-reader/library;
- PlayerData reading progress;
- reset systems;
- local table screens;
- accepted entrance-text architecture except the reproduced restore/authority bug;
- accepted manual poster physical baseline.

## Presentation baseline

Preserve the accepted architecture:

```text
Standalone Presentation Core
-> own VRCUnityVideoPlayer
-> synced semantic state
-> dedicated VideoTXL adapter
-> local VideoTXL suspend/restore
-> existing physical projector/screen
```

Canonical acceptance:

`PRESENTATION_ACCEPTANCE_2026-09-05.md`

Presentation is not today's work unless one of the persistence fixes causes a demonstrated regression.

## Quest VideoTXL orientation

The previous Quest-only VideoTXL orientation issue still requires explicit beta confirmation if it becomes relevant, but it is parked during this persistence work block unless a reproduced regression forces attention.

Do not change VideoTXL 2.5.1 as part of entrance/poster debugging.

## Beta triage rule

Classify reports first:

1. blocker;
2. functional bug;
3. sync/multiplayer issue;
4. Quest/PC platform issue;
5. usability/confusion;
6. polish request;
7. future feature.

Only the first five normally justify immediate beta work.

## Exact next work

```text
1. get COMPLETE current EntranceTextManager.cs
2. identify PlayerData/current-instance overwrite path
3. smallest safe fix
4. two-user rejoin acceptance
5. get COMPLETE current poster persistence implementation
6. reproduce/name exact poster bug
7. smallest safe fix
8. persistence acceptance
9. update GitHub evidence
```

## Working method with Stef

- Dutch;
- noob-friendly;
- one small manual Unity action at a time;
- explain why before technical action;
- inspect full current scripts/screenshots/logs rather than inventing scene state;
- backup before meaningful risk;
- no broad refactors;
- no autonomous visual redesign;
- Codex only for bounded implementation/debugging when Stef explicitly wants it;
- tested real Unity/VRChat behaviour outranks documentation.

If Stef pastes only this prompt and asks no specific question, orient her directly to the Entrance Text bug and ask for the complete current `EntranceTextManager.cs` as the first action.
