# Open Classroom

An open classroom system for VRChat.

## Resume work

Read in this order:

1. `START_PROMPT.md`
2. `AGENTS.md`
3. `CURRENT_WORK.md`
4. `WORKLOG_2026-09-08.md` for the latest stabilization work and final planned features
5. `EREADER_LIBRARY_HANDOFF_2026-09-05.md` when e-reader/library internals matter
6. `PERFORMANCE_AUDIT_2026-09-05.md` before performance changes
7. `PRESENTATION_ACCEPTANCE_2026-09-05.md`
8. exact feature/architecture files only when needed

The repository is durable project memory and backup/reference evidence from the real Unity scene.

Do not assume a copied GitHub scene/script snapshot is newer than Stef's tested Unity project.

## Current status — 2026-09-08

**Open Classroom is functionally very close to complete. Presentation is beta-ready, the current e-readers are substantially refined, VRCDN playback has been investigated in the real VRChat client, and the Marker Pro reset path has been extended.**

Real Unity project:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

### Current protected working baseline

Presentation:
- standalone Presentation Core;
- own VRCUnityVideoPlayer;
- VideoTXL 2.5.1 adapter;
- 10 slots;
- slide navigation;
- two-client synchronization;
- resume same slide after OFF/ON;
- late join;
- VideoTXL suspend/restore;
- projector visibility;
- brightness/contrast;
- final physical-screen output;
- Presentation UI integrated into the tablet.

VRCDN / VideoTXL:
- ClientSim AVPro behaviour is not a valid VRCDN acceptance test;
- real VRChat Build & Run logs showed the livestream route opening through VideoTXL using AVPro 1080p low-latency;
- the video-wall renderer was re-enabled;
- keep the exact checked-in VideoTXL 2.5.1;
- do not redesign VideoTXL based on ClientSim livestream behaviour.

E-reader/library:
- one local shared e-reader video manager;
- one VRCUnityVideoPlayer;
- one shared RenderTexture;
- multiple physical readers/books;
- improved pickup feedback and larger/clearer controls;
- desktop/mobile controls remain available after release;
- automatic local close after being away for about 30 seconds / more than about one metre;
- last-read page now persists per user with VRChat PlayerData;
- both physical e-readers share the same user's saved reading progress.

Marker Pro:
- five Marker Pro objects have dedicated reset points;
- global reset now returns them to original position/rotation and releases them as needed.

## Latest recovery point

Stef created:
- a fresh offline full backup;
- a second online backup in OneDrive;
- additional intermediate backups/screenshots during the 7/8 September work.

Treat this as a strong recovery point before the final planned additions.

## Remaining acceptance check

Before adding new features, do one narrow real multiplayer pass:

1. verify e-reader PlayerData persistence with real users/reconnect;
2. verify both physical e-readers share the same user's saved progress as intended;
3. verify Marker Pro global reset appears correctly for both users;
4. optionally check late join once if useful.

If this passes, record the result as the golden Classroom baseline.

## Final planned Classroom additions

Only two creator/teacher customization features are currently planned before calling the Classroom complete.

### Persistent entrance text

Teacher can:
- enter custom text for the entrance;
- save it persistently;
- bring that saved text into a newly established Classroom instance;
- have the current instance text synchronized to everyone, including late joiners.

Preferred split:
- PlayerData = teacher's personal saved value;
- small synced variable/state = current instance text.

### Persistent synchronized movable poster

Teacher can:
- enter a direct image URL;
- load a poster image;
- pick the poster up with interaction similar to the e-reader;
- place it anywhere;
- scale it uniformly with one simple slider;
- have movement/placement synchronized to others;
- persist URL + position + rotation + scale for future sessions.

Reuse references, not existing working systems:
- e-reader = pickup feel + PlayerData pattern;
- table screens = UI visual language + scale concept only;
- Marker/reset systems = ownership/reset reference where useful.

Do **not** turn the local table screens into synchronized/persistent screens and do not couple the poster to VideoTXL/Presentation unless real evidence requires it.

Full latest handoff:

`WORKLOG_2026-09-08.md`

## Presentation architecture

Current proven direction:

```text
standalone Presentation Core
-> own VRCUnityVideoPlayer
-> sync only mode + slot + slide + revision
-> local load/seek/pause per client
-> Open Classroom VideoTXL 2.5.1 adapter
-> existing projector screen
```

The old VideoTXL Presentation Playlist architecture remains superseded.

Do not rebuild it.

Detailed acceptance snapshot:

`PRESENTATION_ACCEPTANCE_2026-09-05.md`

## E-reader/library reference

Durable earlier handoff:

`EREADER_LIBRARY_HANDOFF_2026-09-05.md`

Latest changes and persistence truth:

`WORKLOG_2026-09-08.md`

Performance findings:

`PERFORMANCE_AUDIT_2026-09-05.md`

