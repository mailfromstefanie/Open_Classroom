# Start Prompt — Open Classroom

Use this file to start a fresh ChatGPT/Codex session about the current Open Classroom state.

## Projects

Primary repository:

`mailfromstefanie/Open_Classroom`

Hosted Presentation service:

`mailfromstefanie/StefanieInVR-Presentation-Service`

Later Cinema integration target:

`mailfromstefanie/Stefanies-Art-House-Cinema`

Real Unity project:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

## Read first

### Mandatory Open Classroom truth
1. `AGENTS.md`
2. `CURRENT_WORK.md`
3. `WORKLOG_2026-09-08.md`
4. `ENTRANCE_TEXT_IMPLEMENTATION_2026-09-09.md`
5. `ENTRANCE_TEXT_V1_1_TITLE_PLAN_2026-09-09.md`

### Mandatory cross-project context
6. `mailfromstefanie/StefanieInVR-Presentation-Service/CURRENT_WORK.md`
   - understand the live hosted Presentation Service, product direction, beta state, and how Open Classroom fits into the broader StefanieInVR product line;
7. `mailfromstefanie/Stefanies-Art-House-Cinema/CURRENT_WORK.md`
   - understand the parked Cinema project, its existing control/menu/reset/admin route, and what work is intended after Open Classroom;

These two cross-project reads are context only. Do not edit or implement in those repositories unless Stef explicitly asks.

### Read when relevant
8. `EREADER_LIBRARY_HANDOFF_2026-09-05.md` when e-reader internals matter
9. `PRESENTATION_ACCEPTANCE_2026-09-05.md` when Presentation internals matter
10. `PERFORMANCE_AUDIT_2026-09-05.md` only before deliberate performance work
11. exact feature/architecture files only when needed

## Exact current truth — 2026-09-09

**Open Classroom is functionally very close to complete.**

Do not start by rebuilding or re-debugging working systems.

Protected working baseline:
- Presentation = working / beta-ready;
- VideoTXL = exact 2.5.1 and working;
- VRCDN livestream path investigated using real VRChat logs / Build & Run;
- e-readers substantially refined;
- e-reader last-read page now uses VRChat PlayerData per user;
- five Marker Pro objects have dedicated global reset points;
- Stef has fresh offline and OneDrive backups.



## Cross-project identity / roadmap context

Do not treat Open Classroom as an isolated one-off world.

The broader StefanieInVR direction is:

```text
Presentation Service
= live hosted PDF -> MP4 service / beta product

Open Classroom
= current active VRChat implementation / beta classroom / proving ground

Reusable Presentation + e-reader patterns
= future productization candidates

Art House Cinema
= later integration target using its own existing control/menu/reset/admin architecture
```

Understand this roadmap before proposing architecture.

Important:
- Open Classroom is the active implementation truth for this session;
- Presentation Service defines hosted-service/product truth;
- Cinema is later, not currently active, but its return route must be respected;
- do not create Open Classroom systems in a way that obviously conflicts with later reuse/productization unless there is a good reason;
- do not prematurely implement Cinema work from Open Classroom;
- preserve working systems and prefer small reusable patterns where practical.

Stef is building toward reusable VRChat teaching/presentation tools and eventually commercializable prefabs/services. Keep recommendations practical, lightweight, beginner-maintainable, and reuse-friendly.

## Important latest evidence

### VRCDN / AVPro

ClientSim is not a valid AVPro/VRCDN playback acceptance environment because its AVPro player is stubbed.

Do not diagnose or modify VideoTXL based solely on ClientSim livestream Loading behaviour.

Real VRChat Build & Run logs showed the VRCdn route using VideoTXL + AVPro 1080p low-latency.

The video-wall renderer was re-enabled.

Keep exact VideoTXL 2.5.1.

### E-reader

Current architecture remains:

```text
one EReaderLocalPlaybackManager
-> one VRCUnityVideoPlayer
-> one shared RT_EReader
-> multiple physical EReaderBook instances
```

Latest reported improvements:
- better desktop/mobile controls after release;
- pickup highlights;
- larger/high-contrast UI;
- compact page/navigation symbols;
- automatic local sleep/close after being away;
- last-read page persisted per user with PlayerData;
- both physical e-readers share the same user's stored reading progress.

Do not replace this with a new persistence framework.

### Marker Pro reset

Five Marker Pro objects now have own reset points and are intended to release/return to original position and rotation for everyone on global reset.

## Exact next action

**Do not start performance optimization.**

First do one narrow real multiplayer acceptance pass:

1. verify e-reader PlayerData persistence;
2. verify both physical e-readers share the same user's saved progress;
3. verify Marker Pro global reset for both users;
4. optional late-join check if useful.

If this passes, record it as the golden Classroom baseline.

## Final two planned Classroom features

After the multiplayer acceptance pass, only these two additions are planned.

### 1. Persistent entrance text

Teacher experience:
- edit entrance text;
- Apply/Save;
- persist that value for the teacher;
- when that teacher establishes a new instance, their stored text can become the shared instance text;
- everyone and late joiners see the same current text;
- Reset to Default.

Preferred architecture:

```text
PlayerData = teacher personal saved text
synced small runtime state = text for current instance
```

Wait for PlayerData restoration before reading it.

### 2. Persistent synchronized movable poster

Teacher experience:
- paste direct image URL;
- load poster;
- pickup interaction similar to the current e-reader;
- move/place it anywhere;
- uniform scale slider only;
- synchronized placement for everyone;
- late join sees current poster;
- persist URL + position + rotation + scale for future teacher sessions.

Preferred architecture:

```text
PlayerData = teacher saved poster configuration
synced runtime state / ownership = current instance poster
```

Reuse references only:
- e-reader = pickup feel + PlayerData pattern;
- local table screens = UI visual language / scale concept;
- Marker/reset system = ownership/reset reference.

Do not:
- modify the working local table screens;
- refactor the e-reader into the poster;
- couple poster to VideoTXL/Presentation without evidence;
- broaden scope.

## Presentation rules to preserve

Current architecture:

```text
Standalone Presentation Core
-> own VRCUnityVideoPlayer
-> sync modeActive + slotIndex + slideIndex + revision
-> local MP4 load/seek/pause per client
-> RT_PresentationVideo
-> VideoTXLPresentationAdapter
-> VideoTXL 2.5.1 ScreenManager
-> existing physical projector screen
```

Do not use VideoTXL `_TriggerPause()` as the local suspend mechanism.

Do not rebuild the old Presentation Playlist architecture.

## Working style

- speak Dutch;
- explain simply;
- inspect first;
- one bounded technical task at a time;
- protect the current working Classroom;
- no broad refactors;
- do not claim multiplayer/Quest proof without real evidence;
- complete scripts only when code replacement is requested;
- GitHub is durable memory, but the tested Unity scene can be newer.

