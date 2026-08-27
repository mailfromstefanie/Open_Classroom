# Current Work — Open Classroom

Last updated: 2026-08-27 (Europe/Amsterdam)

## ACTIVE GOAL

Finish the Open Classroom VideoTXL 2.5.1 repair before returning to the StefanieInVR Presentation Service / PowerPoint-style presentation tool.

The current focus is no longer to keep adding privacy workarounds. First inspect the real scene and verify whether the player/privacy architecture can be simplified substantially.

## CURRENT STATE

The normal custom playlist buttons are working again with VideoTXL 2.5.1.

Working button flow:

```text
custom tablet Button
→ TXLPlayPlaylistButton.PlayAndShow()
→ Playlist._MoveTo(0)
→ VideoSourceUI._SelectActive()
→ first track starts and the active playlist is shown in the TXL playlist UI
```

The old `PlaylistLoadData._Load()` OnClick route is no longer required for playlist sources that already exist as their own Playlist under the new Source Manager.

## CURRENT RESEARCH DIRECTION — SIMPLIFY FIRST

A fresh architecture review on 2026-08-27 suggests the current privacy solution may be more complicated than necessary.

Important hypothesis to verify in the real scene:

```text
VideoTXL
→ owns playback + synchronization

our tablet UI
→ owns local discovery/selection permissions

public user
→ sees public source buttons

VIP/admin
→ sees public + VIP source buttons

owner
→ sees public + VIP + owner source buttons
```

The VideoTXL playlist/source objects themselves should remain active so that playback can still synchronize to everybody in the instance.

Privacy requirement is specifically:

> control who can DISCOVER / SELECT a source locally, not who receives the resulting synchronized video playback.

Example:

```text
owner selects owner-only stream
→ VideoTXL synchronizes playback
→ everyone can watch/hear the stream
→ non-owner users cannot see or select the owner-only source/button
```

## VIDEOTXL UI DIRECTION TO VERIFY

Current scene already has the Video Source UI `Footer` disabled so VideoTXL's automatically generated source-selection buttons do not expose private source names.

This may be exactly the right design direction:

```text
VideoTXL sources/playlists
→ remain active

native VideoTXL source selector
→ hidden from users

our own tablet buttons
→ only permitted navigation path
```

Do NOT treat this as proven yet. Verify it against the live scene and real multiplayer behaviour before removing anything.

## CURRENT CUSTOM SCRIPTS

- `Scripts/UIManagers/TXLPlayPlaylistButton.cs`
- `Scripts/UIManagers/TXLPlaylistPrivacyFilter.cs`
- `Scripts/UIManagers/VipAccessManager.cs`
- `Scripts/UIManagers/PaperTabletTabManager.cs`

`TXLPlayPlaylistButton` is currently useful as the small bridge from our custom UI to VideoTXL.

`TXLPlaylistPrivacyFilter` is now a candidate for simplification or removal, but this is NOT yet approved. First prove whether other users' local VideoSourceUI remains independent when an owner selects a private source.

## IMPORTANT TXL FINDING

VideoTXL/CommonTXL has a built-in `AccessControl` system with rules for instance owner, master, first join, whitelist and anyone.

Current research indicates this primarily controls who may control the player, not per-playlist discovery/visibility. Do not use it as per-playlist privacy unless source code / scene testing proves otherwise.

## FUTURE CONTENT-MANAGEMENT REQUIREMENT

The design should also support the future Open Arthouse Cinema film catalog.

Long-term preference:

```text
fixed world UI + player logic
→ rarely requires world re-upload

changeable catalog/content data
→ should ideally be maintainable externally
```

The Cinema will later need a film catalog with clickable entries/links, posters/text and VideoTXL playback. Avoid an architecture that forces a Unity/world re-upload every time catalog content changes if VideoTXL or VRChat runtime data loading can solve this cleanly.

VideoTXL 2.5.x has Source Manager / playlist catalog functionality that should be inspected before building a separate custom catalog system.

## READ-ONLY UNITY INSPECTION VIA CODEX + UNITY MCP

Stef wants to use Codex with Unity-MCP for the first time to let us inspect the live Unity scene.

Working rule:

```text
Stef ↔ Nova in ChatGPT
→ Nova decides what information is needed
→ Stef gives that exact read-only instruction to Codex
→ Codex inspects Unity via MCP
→ Stef returns the result to this chat
→ Stef + Nova make the decision here
```

Codex is currently an inspection tool only, not the project decision-maker.

During this phase Codex must NOT:

- modify GameObjects or components
- change Inspector values
- add/remove scripts
- write assets, prefabs or scenes
- save scene changes
- start/stop Play Mode unless explicitly approved later
- perform automatic fixes

If information can only be obtained through a potentially mutating operation, Codex must stop and report that limitation first.

## EXACT NEXT ACTION

First establish a safe Unity-MCP connection and perform a read-only connection check only.

Codex should initially report only:

1. which Unity project is connected;
2. which scene is active;
3. whether Unity is in Edit Mode;
4. whether compile errors are present.

STOP after that first connection proof.

Only after the connection is proven should the next read-only inspection map:

- SyncPlayer
- Source Manager
- VideoSourceUI
- Playlist / PlaylistData
- AccessControl
- PlayerControls
- our custom playlist/privacy/button components
- their actual scene hierarchy and serialized references

No architecture changes until this inspection is complete.

## FIRST IMPORTANT MULTIPLAYER PROOF AFTER INSPECTION

The likely decisive real-world test remains:

```text
TXLPlaylistPrivacyFilter temporarily not participating
→ VIP opens a normal public/VIP playlist locally
→ owner starts an owner-only source
→ VIP receives synchronized playback
→ VIP must NOT automatically gain owner-only source navigation/content
```

ClientSim/editor proof is not final VRChat multiplayer proof.

## SERIALIZATION ERROR CLEANUP — RESOLVED

Repeated UdonSharp/Odin `ArgumentNullException: unityObject` errors were traced to an obsolete prefab:

`Assets/StefanieInVR/Prefabs/UIs/UIs 7.prefab`

Unity reported multiple Missing Script components inside that prefab. The prefab was no longer needed and was deleted from the real project. After cleanup the rest of the project/console was reported clean.

Do not treat the privacy scripts as the cause of that old serialization problem.

## PAUSE / HANDOFF

Do not start the Presentation Service / PowerPoint tool again until this Classroom VideoTXL privacy/access/player architecture is understood and tested enough to trust.

## WORKING RULE

Use simple Dutch and one small testable step at a time:

```text
inspect
→ explain briefly
→ change nothing until proven necessary
→ exact test
→ record proven result
→ choose next smallest step
```
