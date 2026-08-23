# Current Work — Open Classroom

Last updated: 2026-08-24 (Europe/Amsterdam)

## ACTIVE GOAL

Finish the Open Classroom VideoTXL 2.5.1 repair before returning to the StefanieInVR Presentation Service / PowerPoint-style presentation tool.

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

## NEW SCRIPTS ADDED TO REPO

- `Scripts/UIManagers/TXLPlayPlaylistButton.cs`
- `Scripts/UIManagers/TXLPlaylistPrivacyFilter.cs`

`PaperTabletTabManager.cs` and `VipAccessManager.cs` in GitHub already match the current scene versions that were checked during this repair.

## VIDEO SOURCE UI SETUP

Current relevant hierarchy:

```text
PlayerControls
└─ PlayerControls
   └─ Video Source UI
      └─ Canvas
         ├─ Panel
         ├─ Content
         └─ Footer
```

Current scene choice:

- `Footer` is disabled so VideoTXL's automatically generated green source buttons do not expose private source names.
- `Content` remains the runtime container where VideoTXL generates the active playlist UI.
- Custom tablet playlist buttons are now the intended way to select/start playlists.

## PRIVATE PLAYLIST WORK — IN PROGRESS

Desired access levels:

1. public playlists
2. VIP/admin-only playlists
3. owner-only playlist(s)

Important rule:

> Hiding access is local UI privacy only. The actual VideoTXL Playlist source must stay active so a private/admin selection can still start synchronized playback for everyone.

Existing `VipAccessManager.enableWhenVip[]` can locally show the relevant custom VIP button objects.

`TXLPlaylistPrivacyFilter` currently supports:

- `vipOnlyPlaylists[]`
- `ownerOnlyPlaylists[]`
- `ownerDisplayName`
- `ownerOnlyButtons[]`
- local hiding of `Video Source UI / Canvas / Content` when the active source is not allowed for the local player

Current owner-only test setup used `Playlist (Live Stream StefanieInVR)` as an owner-only playlist and the actual inner clickable `Button` as an owner-only button. In this scene the inner Button is the layer that must be hidden; hiding the parent was observed to be overridden by the tablet/tab layout behaviour.

## IMPORTANT TXL FINDING

VideoTXL/CommonTXL has a built-in `AccessControl` system with rules for instance owner, master, first join, whitelist and anyone.

This appears to govern who may control the player. We have NOT yet proven that it provides per-playlist visibility/privacy in the new VideoSourceUI.

### EXACT NEXT ACTION

Before changing the privacy architecture further:

1. inspect the actual `SyncPlayer` Inspector in this scene;
2. see whether an `Access Control` component is already assigned;
3. determine whether TXL's built-in whitelist can replace any of our custom access work;
4. do not assume it can hide individual playlists unless verified from code/test.

Then perform a real two-user test with another VIP:

```text
owner starts owner-only stream
→ other VIP receives playback
→ other VIP must not see/use owner-only button
→ other VIP must not see the owner-only playlist contents
```

ClientSim/editor proof is not final VRChat multiplayer proof.

## SERIALIZATION ERROR CLEANUP — RESOLVED

Repeated UdonSharp/Odin `ArgumentNullException: unityObject` errors were traced to an obsolete prefab:

`Assets/StefanieInVR/Prefabs/UIs/UIs 7.prefab`

Unity reported multiple Missing Script components inside that prefab. The prefab was no longer needed and was deleted from the real project. After cleanup the rest of the project/console was reported clean.

Do not treat the privacy scripts as the cause of that old serialization problem.

## PAUSE / HANDOFF

Work stopped for the night here.

Do not start the Presentation Service / PowerPoint tool again until this Classroom VideoTXL privacy/access repair is finished and tested enough to trust the player setup.

## WORKING RULE

Use simple Dutch and one small testable step at a time:

```text
inspect
→ explain briefly
→ change one thing
→ exact test
→ record proven result
→ stop
```
