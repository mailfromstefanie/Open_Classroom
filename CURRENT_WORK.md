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

## CODEX + UNITY MCP — CONNECTION PROVEN ✅

A working local inspection route was established on 2026-08-27.

Working setup:

```text
Unity Editor
→ KitWright MCP for Unity v1.0.0
→ Transport Mode: Direct HTTP
→ Codex CLI
→ read-only Unity tool call succeeds
```

Important observations:

- KitWright Broker Mode caused a Codex MCP handshake failure on this Windows setup.
- Switching KitWright to `Direct HTTP` resolved the connection problem.
- The Codex desktop GUI did not expose the KitWright MCP reliably during testing.
- The Codex CLI bundled with the desktop app did work.
- `codex mcp list` showed `kitwright` as enabled.
- A real read-only MCP call succeeded:

```text
kitwright.get_scene_info({})
→ active scene: Classroom
→ path: Assets/#Classroom/Scenes/Classroom.unity
```

This proves Codex can read the actual running Unity scene through MCP.

A PowerShell profile was also configured so Stef can start the CLI by typing simply:

```text
codex
```

Do not rely on a hardcoded Codex build-folder path in project documentation; the internal build id may change after updates/reinstall.

## WORKING RELATIONSHIP / REGIE

The new-chat workflow is intentionally:

```text
Stef ↔ Nova in ChatGPT
→ Nova helps think, compare options and make project decisions
→ Nova writes one precise prompt for Codex
→ Stef gives that prompt to Codex CLI
→ Codex inspects Unity read-only
→ Stef brings the result back to Nova
→ Stef + Nova decide the next step
```

Codex is an inspection/execution helper, not the project decision-maker.

During the current investigation Codex must NOT:

- modify GameObjects or components
- change Inspector values
- add/remove scripts
- write assets, prefabs or scenes
- save scene changes
- start/stop Play Mode unless explicitly approved later
- perform automatic fixes

If a question cannot be answered safely read-only, Codex must stop and report that limitation.

## EXACT NEXT ACTION

The MCP connection itself is now proven, so do NOT spend more time on connection setup.

First read-only task in the next chat:

### 1. Verify local Unity scripts against GitHub

Have Codex compare the local project copies of these scripts with repository truth in `mailfromstefanie/Open_Classroom`:

- `Scripts/UIManagers/VipAccessManager.cs`
- `Scripts/UIManagers/TXLPlaylistPrivacyFilter.cs`
- `Scripts/UIManagers/TXLPlayPlaylistButton.cs`
- `Scripts/UIManagers/PaperTabletTabManager.cs`

Report per file only:

```text
GELIJK
VERSCHILLEND
LOKAAL ONTBREEKT
GITHUB ONTBREEKT
NIET ZEKER
```

If different, report the meaningful difference but modify nothing.

Also report any relevant scene-used custom script that appears local but not in GitHub, if this can be established read-only.

### 2. Only after script truth is confirmed

Map the actual live scene read-only:

- SyncPlayer
- Source Manager
- VideoSourceUI
- Playlist / PlaylistData
- AccessControl
- PlayerControls
- our custom playlist/privacy/button components
- their real hierarchy and serialized references

No architecture changes until that inspection is complete.

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
