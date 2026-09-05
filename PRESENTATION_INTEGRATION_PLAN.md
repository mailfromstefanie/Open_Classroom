# Presentation Prefab / Open Classroom Integration Plan

Last updated: 2026-09-05 (Europe/Amsterdam)

## STATUS

**PRESENTATION SERVICE LIVE — CLASSROOM PRESENTATION INTEGRATION CURRENTLY DISMANTLED — REBUILD PHASE ACTIVE**

The hosted StefanieInVR Presentation Service is live.

Stef confirms that the previous Presentation integration in the real Open Classroom scene has been dismantled. The current Classroom must therefore not be described as beta-ready with the Presentation Service.

Important repository boundary:

- this GitHub snapshot does not currently contain a committed `PresentationController.cs`;
- the real Unity scene must be inspected to see what foundation remains;
- rebuild only the smallest safe Presentation path on top of the existing VideoTXL player/screen architecture.

## PRODUCT CONTRACT

Hosted service:

- 10 stable Presentation Slot MP4 URLs;
- PDF input;
- one PDF slide = one second of video;
- Slot 1 may be used as Stef's demo;
- hosted conversion is independent from the reusable prefab.

Reusable prefab:

- must not depend on Stef's hosting only;
- must allow creators to configure their own compatible MP4 URLs/hosting;
- should reuse an existing compatible VideoTXL player/screen rather than ship a duplicate player by default;
- should be practical to place into another VRChat world, including later Art House Cinema integration.

## ACCEPTED CLASSROOM ARCHITECTURE

Target architecture unless real testing disproves it:

```text
Presentation UI / controller
-> Presentation Playlist/source
-> existing VideoTXL 2.5.1 SyncPlayer
-> existing projector/screen

Previous / Next
-> synchronized seek
-> one second per slide
```

Why:

- existing Classroom custom buttons already use VideoTXL playlist/source routing;
- VideoTXL `SyncPlayer._SetTargetTime(float)` is the accepted synchronized seek path;
- VideoTXL exposes runtime pause/time/seekability state;
- `SyncPlayer._TriggerPause()` provides synchronized pause state;
- the existing screen visibility helper can keep a paused slide visible.

Do not add a second AVPro/Unity player unless real testing proves this route cannot support the prefab.

## USER-FACING V1 PREFAB CONTROLS

Target controls remain:

- Slot 1 ... Slot 10 selection;
- Previous slide;
- Next slide;
- First slide;
- current slot label;
- current slide / total slides;
- pause presentation once seekable;
- synchronized slot/slide state where needed for shared use;
- no autoplay slideshow in V1.

Optional later:

- Last slide;
- direct page entry;
- autoplay/timer;
- local pointer/laser;
- configurable access filtering;
- custom external URL entry;
- eBook reuse.

## PRESENTATION MODE / BACK TO VIDEO

Preferred polished behaviour:

```text
normal VideoTXL video
-> enter Presentation Mode
-> remember previous source/context + approximate time + pause/play state
-> presentation uses same player/screen
-> Back to Video
-> restore previous source/time/play-pause state
```

For the current rebuild phase:

- do not make Back to Video the first blocker;
- first prove one Presentation Slot through the existing VideoTXL player/screen;
- then prove Previous / Next / First and pause/seek;
- treat exact source/time/play-pause restoration as a separate proof step;
- do not create a second player just to avoid understanding VideoTXL state restoration.

## PREFAB EXTRACTION / HARDENING ROUTE

The next phase is:

1. inspect the current Classroom hierarchy and the surviving VideoTXL/player/screen/tablet foundation;
2. inventory any remaining Presentation-related GameObjects/components/scripts;
3. record exact VideoTXL/playlist/screen/UI references;
4. rebuild the smallest working Presentation path;
5. prove Slot 1 first;
6. prove Previous / Next / First and pause/seek;
7. identify what is Classroom-specific;
8. identify what belongs inside the reusable prefab;
9. create the smallest reusable boundary;
10. expose configuration cleanly in Inspector;
11. prove multiplayer/sync/late join as applicable;
12. prove Quest;
13. only then call the prefab reusable.

## FIRST NEXT-CHAT GATE

Before writing or moving anything:

```text
Unity out of Play Mode
-> inspect current presentation hierarchy
-> inspect components/scripts/references
-> change nothing
```

This gate exists because the real scene is newer than the repository snapshot.

## TESTING ORDER

After a reusable boundary exists:

1. no-error Editor wiring check;
2. Slot 1 demo;
3. pause on intended first slide;
4. Previous / Next / First;
5. slot switching;
6. Back to Video if part of current proven scope;
7. uploaded VRChat PC;
8. second user;
9. late join;
10. Quest;
11. package documentation.

## WORKING RULE

```text
one tiny inspected change
-> test
-> record proof
-> next change
```

Stef receives complete scripts, never code fragments.
