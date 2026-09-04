# Presentation Prefab / Classroom Integration Plan

Last updated: 2026-09-04 (Europe/Amsterdam)

## Status

**ACTIVE — build the first VRChat Presentation controller inside Open Classroom.**

The hosted StefanieInVR Presentation Service Milestone 4 is accepted and live. The Classroom implementation may now begin.

## Product contract

Hosted service:

- exactly 10 stable Presentation Slot MP4 URLs;
- one PDF slide = one second of video;
- Slot 1 currently holds Stef's demo presentation;
- hosted conversion is independent from the future prefab;
- the future prefab must remain configurable so users can supply their own MP4 URLs/hosting.

## Classroom design decision

Reuse the existing **VideoTXL 2.5.1 SyncPlayer and projector screen** instead of creating a second video player.

Add one dedicated VideoTXL Playlist source named conceptually:

`Presentations`

with exactly 10 entries:

`Slot 1 ... Slot 10`

Each entry points at the corresponding stable hosted MP4 URL.

The custom Presentation controller talks to that Playlist and the existing SyncPlayer:

```text
Presentation UI
-> PresentationController
-> VideoTXL Playlist._MoveTo(slotIndex)
-> existing SyncPlayer
-> existing projector/video screen

Previous / Next
-> SyncPlayer._SetTargetTime(...)
-> one slide per second
-> shared synchronized video time
```

This keeps VideoTXL responsible for playback/synchronization, matching the existing Classroom responsibility split.

## Why this route

Verified from the current Classroom and VideoTXL source:

- existing custom buttons already use `Playlist._MoveTo(index)`;
- VideoTXL 2.5.1 `SyncPlayer._SetTargetTime(float)` is a synchronized seek operation and takes player control through VideoTXL;
- `SyncPlayer.paused`, `trackDuration`, `trackPosition` and `seekableSource` are exposed runtime state;
- `SyncPlayer._TriggerPause()` synchronizes pause state;
- existing `TXLScreenAutoVisibility` is already configured to keep the screen visible while paused, so a presentation slide can remain on screen.

Do not add a second AVPro/Unity player unless this proven route fails in real VRChat testing.

## Version 1 controls

Required:

- Slot 1 ... Slot 10 selection;
- Previous slide;
- Next slide;
- First slide;
- current slot label;
- current slide / total slide count;
- automatically pause after a presentation MP4 becomes seekable;
- seek to the selected slide using one-second slide spacing;
- keep controller slot/slide state synchronized for UI/late joiners;
- no autoplay slideshow mode in V1.

Optional later:

- last slide;
- direct page number entry;
- autoplay/timer;
- local pointer/laser;
- owner/VIP-only control filtering;
- custom external URL entry;
- eBook reuse.

## Scene wiring target

No destructive replacement of current Classroom playback systems.

Create a separate root:

`PresentationSystem`

It will contain:

- one `PresentationController` UdonSharp behaviour;
- references to the existing VideoTXL `SyncPlayer`;
- reference to the dedicated Presentation Playlist source;
- optional reference to existing `VideoSourceUI`;
- presentation control UI can live on the existing tablet/VIP panel.

The existing projector mesh/screen and `TXLScreenAutoVisibility` remain unchanged.

## Testing order

1. Unity Editor/ClientSim-safe wiring check where possible.
2. Load Slot 1 demo.
3. Verify it pauses on slide 1.
4. Previous/Next/First.
5. Slot switching.
6. real uploaded VRChat PC test.
7. second-user sync.
8. late join.
9. Quest.
10. only after proof package/generalize as reusable prefab.

## Working rule

```text
one small scene change
-> test
-> next full script/file
-> wire exact Inspector references
-> real VRChat multiplayer proof
-> record result
```

Stef receives full scripts, not partial code fragments.
