# Presentation Prefab / Open Classroom Integration Plan

Last updated: 2026-09-05 (Europe/Amsterdam)

## STATUS

**OPEN CLASSROOM BETA-READY BY STEF'S CONFIRMATION — REUSABLE PREFAB PHASE ACTIVE NEXT**

The hosted StefanieInVR Presentation Service is live.

Stef confirms the real Open Classroom Unity/world setup is already ready for beta testing with that service.

Important repository boundary:

- this GitHub snapshot does not currently contain a committed `PresentationController.cs`;
- therefore the actual Unity scene must be inspected before any reconstruction or extraction;
- older "create PresentationSystem from scratch" instructions are superseded for the next session.

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

Preserve unless real beta-ready scene inspection proves otherwise:

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

However, for the current prefab phase:

- first inspect whether the beta-ready Classroom already implements/proves this;
- preserve proven behaviour;
- if it is not implemented, treat exact source restoration as a separate proof step;
- do not create a second player just to avoid understanding VideoTXL state restoration.

## PREFAB EXTRACTION / HARDENING ROUTE

The next phase is:

1. inspect actual beta-ready Classroom presentation hierarchy;
2. inventory every presentation-related GameObject/component/script;
3. record exact VideoTXL/playlist/screen/UI references;
4. identify what is Classroom-specific;
5. identify what belongs inside the reusable prefab;
6. create the smallest reusable boundary;
7. expose configuration cleanly in Inspector;
8. test the prefab without breaking the working Classroom;
9. prove Slot 1 first;
10. prove navigation;
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
