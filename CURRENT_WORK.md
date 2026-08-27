# Current Work — Open Classroom

Last updated: 2026-08-27 (Europe/Amsterdam)

## ACTIVE GOAL

Finish one real VRChat multiplayer proof of the now-understood VideoTXL 2.5.1 privacy/access architecture, then return to the StefanieInVR Presentation Service.

The read-only Unity/Codex investigation is complete enough to stop broad architecture research.

## CURRENT ARCHITECTURE — VERIFIED DIRECTION

The trusted model is now:

```text
VideoTXL
→ owns playback + synchronization
→ all Playlist/source objects may remain active

native VideoTXL source selector
→ hidden

our tablet UI
→ owns who may DISCOVER / SELECT a source

TXLPlaylistPrivacyFilter
→ locally hides restricted active-playlist CONTENT from unauthorized users
→ currently also hides the owner-only stream button for non-owner users
```

Important distinction:

> A restricted source may still play and synchronize to everybody. Privacy here means who can discover/select the source and who may inspect its playlist content/navigation locally.

Do not use VideoTXL/CommonTXL `AccessControl` as per-playlist privacy. No `AccessControl` scene-instance is currently connected to the player, and `SyncPlayer.accessControl` is null.

## BUTTON FLOW — WORKING

The normal custom playlist buttons are working with VideoTXL 2.5.1:

```text
custom tablet Button
→ TXLPlayPlaylistButton.PlayAndShow()
→ Playlist._MoveTo(0)
→ VideoSourceUI._SelectActive()
→ first track starts and the active playlist is shown locally
```

The old `PlaylistLoadData._Load()` OnClick route is no longer required for playlist sources that already exist as their own Playlist under the new Source Manager.

## READ-ONLY UNITY INSPECTION — COMPLETE ✅

Active Unity project:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

Important note:

`E:/GitHub/Open_Classroom` is a separate older local checkout and must not be treated as the active Unity project. GitHub `mailfromstefanie/Open_Classroom` `main` remains durable repository truth.

The four relevant active Unity script copies were checked against current GitHub content and were meaningfully equal:

- `VipAccessManager.cs`
- `TXLPlaylistPrivacyFilter.cs`
- `TXLPlayPlaylistButton.cs`
- `PaperTabletTabManager.cs`

### Scene map

`SyncPlayer`:

`UIs/Sync Video Player Full`

`SourceManager`:

`UIs/Sync Video Player Full/Source Manager`

`VideoSourceUI`:

`UIs/Paper Tablet System/AudioLinkControllerBody/Objects to Toggle/PlayerControls/PlayerControls/Video Source UI`

`PlayerControls`:

`UIs/Paper Tablet System/AudioLinkControllerBody/Objects to Toggle/PlayerControls/PlayerControls`

`AccessControl`:

- no `Texel.AccessControl` scene-instance found
- `SyncPlayer.accessControl == null`

## SOURCE MANAGER — VERIFIED

The active SourceManager contains 22 Playlist sources.

All 22 were active during inspection:

```text
activeSelf = true
activeInHierarchy = true
```

This confirms that privacy is not implemented by disabling VideoTXL source objects.

The current owner-only source is:

`Live Stream StefanieInVR`

A small unrelated catalog-data mismatch was observed at source index 12: the GameObject naming and `sourceName` do not match. Do not fix this during the privacy gate unless Stef explicitly chooses to clean it up later.

## NATIVE VIDEOTXL SOURCE SELECTOR — VERIFIED HIDDEN

`VideoSourceUI.buttonRoot` points to:

`UIs/Paper Tablet System/AudioLinkControllerBody/Objects to Toggle/PlayerControls/PlayerControls/Video Source UI/Canvas/Footer`

Verified state:

```text
activeSelf = false
activeInHierarchy = false
```

Therefore VideoTXL's automatically generated source-selection buttons are not the user-facing discovery route.

Our own tablet buttons remain the intended navigation route.

## VIDEOSOURCEUI REMOTE BEHAVIOUR — VERIFIED FROM VIDEOTXL 2.5.1 CODE

A remote synchronized source change DOES update the receiving player's VideoTXL playback/current source.

However `VideoSourceUI` does NOT automatically switch its selected content panel when another player changes the active source.

Verified code facts:

- `VideoSourceUI` listens for source add/remove and source enable changes.
- It does not listen for `SourceManager.EVENT_URL_READY`, Playlist track-change events, or SyncPlayer source-change events in a way that selects the active panel.
- `VideoSourceUI._SelectActive()` reads the current local `currentUrlSource` only when `_SelectActive()` is called locally.
- `PlayerControls._HandlePlaylist()` can locally call `_SelectActive()` when the playlist/source panel is opened.
- `TXLPlayPlaylistButton.PlayAndShow()` also calls `_SelectActive()` locally after starting its playlist.

Meaning:

```text
owner starts restricted source
→ everyone receives playback
→ other users' VideoSourceUI does NOT automatically jump to that source panel
```

But later opening the playlist panel locally can call `_SelectActive()` and select the currently synchronized source panel if its content root is visible.

This is why a local restricted-content guard still has a real role.

## TXLPlaylistPrivacyFilter — ACTUAL CURRENT ROLE

Current scene instance:

`UIs/Managers/VipAccessManager`

Current references:

- `videoSourceUI` → active VideoSourceUI
- `playlistContentRoot` → `Video Source UI/Canvas/Content`
- `vipAccessManager` → `UIs/Managers/VipAccessManager`
- `vipOnlyPlaylists` → length 0
- `ownerDisplayName` → `StefanieInVR`
- `ownerOnlyPlaylists` → length 1: `Live Stream StefanieInVR`
- `ownerOnlyButtons` → length 1: StefanieInVR Stream button inside VIP content

Therefore the script currently does NOT provide any VIP-playlist content filtering because `vipOnlyPlaylists` is empty.

Its current live privacy responsibility is owner-only:

```text
owner-only source active
+ local player is not StefanieInVR
→ Video Source UI/Canvas/Content is locally SetActive(false)
```

It also locally hides the owner-only stream button for non-owner users.

## CONTENT ACTIVATION — VERIFIED

Exact guarded GameObject:

`Video Source UI/Canvas/Content`

Read-only code/reference inspection found:

- `TXLPlaylistPrivacyFilter.ApplyForSource()` is the only direct scene-relevant callsite that toggles this exact Content GameObject.
- `VideoSourceUI._SelectActive()` does NOT activate Content.
- `VideoSourceUI._Select()` does NOT activate Content; it only changes child content panels.
- `VideoSourceUI._Rebuild()` does not activate Content.
- `PlayerControls._HandlePlaylist()` toggles the parent Canvas, not Content itself.
- `PaperTabletTabManager` also toggles the parent Canvas, not Content itself.
- VideoTXL 2.5.1 was not found to call `contentRoot.SetActive(true/false)`.

Therefore:

```text
restricted source is active
→ privacy filter hides Content locally
→ user opens playlist/source panel
→ VideoTXL may activate Canvas and select a child panel
→ Content itself remains inactive
→ restricted playlist title/tracks/navigation remain hidden
```

This means `TXLPlaylistPrivacyFilter` must NOT simply be removed before a replacement for this narrow content-guard responsibility exists.

## VIP / TABLET ACCESS — CURRENT OWNERSHIP

`VipAccessManager` + `PaperTabletTabManager` already own the main local VIP/tablet filtering.

Intended responsibility split:

```text
VipAccessManager / tablet UI
→ who may see/select VIP-facing controls

TXLPlaylistPrivacyFilter
→ protect VideoSourceUI Content when the currently synchronized source is restricted
→ currently also owner-button visibility

VideoTXL
→ playback + synchronization
```

Avoid adding extra privacy systems unless a real test proves a missing case.

## CURRENT DECISION

Do NOT remove `TXLPlaylistPrivacyFilter` now.

The earlier idea that the privacy filter might be completely unnecessary is rejected by the code inspection: opening the local VideoTXL playlist panel can select the currently synchronized restricted source, so the Content guard prevents unauthorized playlist details/navigation from becoming visible.

At the same time, the filter's purpose is now much narrower and clearer than before. It is not a source playback/synchronization filter.

Do not refactor or split the script until the real multiplayer proof passes. The current working system is more valuable than an untested cleanup.

## EXACT NEXT ACTION — REAL VRCHAT MULTIPLAYER GATE

No more broad Codex scene research before this test.

Run one real two-user VRChat test with:

- Stef / owner
- a second user who is VIP but is NOT `StefanieInVR`

Why VIP is the important second user: the owner-only stream button lives inside VIP content, so a VIP is the strongest relevant unauthorized user for this test.

### Test A — baseline public/VIP navigation

1. Second user opens a normal permitted playlist.
2. Confirm its playlist content/navigation is visible and usable as expected.

### Test B — owner-only source

1. Stef starts `Live Stream StefanieInVR`.
2. Confirm second user receives the synchronized video/audio playback.
3. Confirm the owner-only stream selection button is NOT visible/selectable for the second user.
4. Confirm restricted `VideoSourceUI/Content` is not visible to the second user.
5. Have the second user close/reopen the VideoTXL playlist/source panel.
6. Confirm the owner-only playlist title/tracks/navigation still do NOT become visible.

### Test C — recovery

1. Stef switches back to a normal permitted source.
2. Confirm the second user's permitted playlist content/navigation can become visible normally again.

### PASS GATE

The Classroom blocker is considered cleared for the Presentation Service when the real multiplayer test proves:

```text
restricted source can synchronize playback to everybody
WITHOUT giving unauthorized users discovery/selection access
AND WITHOUT exposing restricted playlist content/navigation
AND normal permitted content recovers correctly afterward
```

If this passes, record the result and return to `mailfromstefanie/StefanieInVR-Presentation-Service` Milestone 3.

If it fails, fix only the exact failed responsibility. Do not redesign the whole architecture.

ClientSim/editor proof is not final multiplayer evidence.

## FUTURE CONTENT-MANAGEMENT REQUIREMENT — PARKED UNTIL BLOCKER PASSES

The architecture should later support externally maintained content for multiple worlds, for example:

```text
StefanieInVR Content Manager
→ Open Classroom catalog
→ Open Arthouse Cinema catalog
```

Potential externally maintained fields include title, description, image URL, media URL, category, order, enabled state and access level.

Do not design/build this yet. First finish the multiplayer gate and return to the Presentation Service milestone that was paused for this investigation.

## CODEX + UNITY MCP — WORKING ROUTE ✅

Working setup:

```text
Unity Editor
→ KitWright MCP for Unity v1.0.0
→ Transport Mode: Direct HTTP
→ Codex CLI
→ read-only Unity tool calls
```

Important observations:

- Broker Mode caused a Codex MCP handshake failure on this Windows setup.
- Direct HTTP resolved it.
- Codex CLI worked reliably for Unity inspection.
- Codex desktop GUI did not expose the same MCP route reliably during testing.
- Stef can start the CLI by typing `codex` in PowerShell.
- Do not document a hardcoded Codex build-folder path because it may change after updates/reinstall.

Preferred workflow:

```text
Stef ↔ Nova
→ Nova decides the question
→ Nova writes a small precise Codex prompt
→ Codex inspects Unity read-only
→ result returns to Nova
→ next decision
```

## SERIALIZATION ERROR CLEANUP — RESOLVED

Repeated UdonSharp/Odin `ArgumentNullException: unityObject` errors were traced to obsolete prefab:

`Assets/StefanieInVR/Prefabs/UIs/UIs 7.prefab`

It contained Missing Script components, was no longer needed and was deleted from the real project. The rest of the project/console was reported clean afterward.

Do not treat the privacy scripts as the cause of that old serialization problem.

## WORKING RULE

Use simple Dutch and one small testable step at a time:

```text
inspect
→ explain briefly
→ change nothing until proven necessary
→ exact real test
→ record proven result
→ choose next smallest step
```
